using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Lib.Dogecoin.Interop;

namespace Lib.Dogecoin
{
	public class SPVNode
	{
		private const int BLOCKS_BETWEEN_CHECKPOINTS = 10;

		private uint _lastSPVCheckpointHeight = 0;

		private bool _syncComplete = false;
		private bool _isDebug;
		private Thread _thread;
		private IntPtr _spvNodeRef;
		private static dogecoin_spv_client.sync_transaction_delegate syncTransactionCallback;
		private static dogecoin_spv_client.sync_completed_delegate syncCompletedCallback;

		private ISPVCheckpointTracker _checkpointTracker;
		private SPVNodeBlockInfo _startPoint;

		public SPVNode(ISPVCheckpointTracker tracker, bool isMainNet, bool isDebug, SPVNodeBlockInfo startPoint)
		{
			_checkpointTracker = tracker;
			_isDebug = isDebug;
			IsMainNet = isMainNet;
			_startPoint = startPoint;
		}

		public bool IsRunning { get; private set; }

		public bool IsMainNet { get; private set; }

		public bool SyncComplete
		{
			get
			{
				if(!_syncComplete)
				{
					if(_spvNodeRef != IntPtr.Zero && IsRunning)
					{
						var spv = Marshal.PtrToStructure<dogecoin_spv_client>(_spvNodeRef);

						return spv.called_sync_completed;
					}
				}
				return _syncComplete;
			}
		}

		public SPVNodeBlockInfo CurrentBlockInfo { get; private set; }

		public Action OnSyncComplete { get; set; }

		public Action<SPVNodeTransaction> OnTransaction { get; set; }

		public Action<SPVNodeBlockInfo, SPVNodeBlockInfo> OnNextBlock { get; set; }

		private void BeforeOnNextBlock(SPVNodeBlockInfo previousBlock, SPVNodeBlockInfo nextBlock)
		{
			if (_checkpointTracker != null && previousBlock != null)
			{
				if (previousBlock.BlockHeight - _lastSPVCheckpointHeight >= BLOCKS_BETWEEN_CHECKPOINTS)
				{
					_checkpointTracker.SaveCheckpoint(nextBlock);
					_lastSPVCheckpointHeight = previousBlock.BlockHeight;
				}
			}

			if (OnNextBlock != null)
			{
				OnNextBlock(previousBlock, nextBlock);
			}
		}

		public unsafe int GetPeerCount()
		{
			if(_spvNodeRef == IntPtr.Zero)
			{
				return 0;
			}

			var client = Marshal.PtrToStructure<dogecoin_spv_client>(_spvNodeRef);

			var nodeGroup = Marshal.PtrToStructure<dogecoin_node_group>(client.nodegroup);

			var nodeList = *nodeGroup.nodes;

			var connectedNodes = 0;

			for (var i = 0; i < nodeList.len; i++)
			{
				dogecoin_node node = Marshal.PtrToStructure<dogecoin_node>(*(nodeList.data + i));
				
				if ((NODE_STATE)node.state == NODE_STATE.NODE_CONNECTED)
				{
					connectedNodes++;
				}
			}

			return connectedNodes;
		}

		public unsafe void PrintDebug()
		{
			try
			{
				Debug.WriteLine("Thread Status: " + (IsRunning ? "Online" : "Offline"));

				

				var client = Marshal.PtrToStructure<dogecoin_spv_client>(_spvNodeRef);

				var nodeGroup = Marshal.PtrToStructure<dogecoin_node_group>(client.nodegroup);

				var nodeList = *nodeGroup.nodes;

				Debug.WriteLine("Nodes: " + nodeList.len);

				var connectedNodes = 0;
				for (var i = 0; i < nodeList.len; i++)
				{
					dogecoin_node node = Marshal.PtrToStructure<dogecoin_node>(*(nodeList.data + i));

					Debug.WriteLine($"{i}: {Enum.GetName((NODE_STATE)node.state)} - {DateTimeOffset.FromUnixTimeSeconds((long)node.lastping).ToLocalTime()}");
					if((NODE_STATE)node.state == NODE_STATE.NODE_CONNECTED)
					{
						connectedNodes++;
					}
				}
				
				Debug.WriteLine("Connected Nodes: " + connectedNodes);

				Debug.WriteLine($"Block.Hash: {this.CurrentBlockInfo.Hash}");
				Debug.WriteLine($"Block.BlockHeight: {this.CurrentBlockInfo.BlockHeight}");
				Debug.WriteLine($"Block.Timestamp: {this.CurrentBlockInfo.Timestamp.ToLocalTime()}");

				Debug.WriteLine($"spv.stateflags: {client.stateflags}");
				Debug.WriteLine($"spv.last_statecheck_time: {DateTimeOffset.FromUnixTimeSeconds((long)client.last_statecheck_time).ToLocalTime()}");
				Debug.WriteLine($"spv.last_headersrequest_time: {DateTimeOffset.FromUnixTimeSeconds((long)client.last_headersrequest_time).ToLocalTime()}");
		
				Debug.WriteLine($"nodeGroup.desired_amount_connected_nodes: {nodeGroup.desired_amount_connected_nodes}");
				Debug.WriteLine($"nodeGroup.clientstr: {nodeGroup.clientstr.TerminateNull()}");
				

			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex);
			}
		}


		public void Start()
		{
			if (_thread != null && _thread.IsAlive)
			{
				throw new Exception("SPV Already Running!");
			}

			CreateSPVClient();

			_thread = new Thread(() =>
			{
				IsRunning = true;
				_syncComplete = false;

				LibDogecoinInterop.dogecoin_spv_client_discover_peers(_spvNodeRef, null);
				var client = Marshal.PtrToStructure<dogecoin_spv_client>(_spvNodeRef);


				unsafe
				{
					var headerDb = *client.headers_db;

					if (headerDb.has_checkpoint_start(client.headers_db_ctx) == 0)
					{
						SPVNodeBlockInfo checkpoint = null;

						if (_startPoint != null)
						{
							checkpoint = _startPoint;
						}
						else if (_checkpointTracker != null)
						{
							checkpoint = _checkpointTracker.GetCheckpoint();
						}

						if (checkpoint != null)
						{
							headerDb.set_checkpoint_start(client.headers_db_ctx,
														  HexStringToLittleEndianByteArray(checkpoint.Hash),
														  checkpoint.BlockHeight);
						}
					}

				}


			//	LibDogecoinInterop.dogecoin_node_group_event_loop(client.nodegroup);
				LibDogecoinInterop.dogecoin_spv_client_runloop(_spvNodeRef);
				IsRunning = false;
			});

			_thread.Start();
		}

		public unsafe void Stop()
		{
			var spv = Marshal.PtrToStructure<dogecoin_spv_client>(_spvNodeRef);

			LibDogecoinInterop.dogecoin_node_group_shutdown(spv.nodegroup);
			

			LibDogecoinInterop.dogecoin_node_group_event_break(spv.nodegroup);
			//var nodeList = *nodeGroup.nodes;

			//Debug.WriteLine("Nodes: " + nodeList.len);

			//var connectedNodes = 0;
			//for (var i = 0; i < nodeList.len; i++)
			//{
			//	dogecoin_node node = Marshal.PtrToStructure<dogecoin_node>(*(nodeList.data + i));

			//	Debug.WriteLine($"{i}: {Enum.GetName((NODE_STATE)node.state)} - {DateTimeOffset.FromUnixTimeSeconds((long)node.lastping).ToLocalTime()}");
			//	if ((NODE_STATE)node.state == NODE_STATE.NODE_CONNECTED || (NODE_STATE)node.state == NODE_STATE.NODE_CONNECTING)
			//	{
			//		connectedNodes++;
			//	}
			//	LibDogecoinInterop.dogecoin_node_free(*(nodeList.data + i));
			//	Debug.WriteLine($"{i}: {Enum.GetName((NODE_STATE)node.state)} - {DateTimeOffset.FromUnixTimeSeconds((long)node.lastping).ToLocalTime()}");

			//}
		}


		~SPVNode()
		{
			if(_spvNodeRef != IntPtr.Zero)
			{
				LibDogecoinInterop.dogecoin_spv_client_free(_spvNodeRef);
			}
		}


		private unsafe void CreateSPVClient()
		{
			

			LibDogecoinInterop.dogecoin_spv_client_free(_spvNodeRef);

			var net = IsMainNet ? LibDogecoinContext._mainChain : LibDogecoinContext._testChain;

			_spvNodeRef = LibDogecoinInterop.dogecoin_spv_client_new(net, _isDebug, true, false, true);
			_syncComplete = false;
			IsRunning = false;
			

			syncTransactionCallback = new dogecoin_spv_client.sync_transaction_delegate(SyncTransaction);
			syncCompletedCallback = new dogecoin_spv_client.sync_completed_delegate(SyncCompleted);

			Marshal.WriteIntPtr(_spvNodeRef,
				Marshal.OffsetOf(typeof(dogecoin_spv_client),
				nameof(dogecoin_spv_client.sync_transaction)).ToInt32(), Marshal.GetFunctionPointerForDelegate(syncTransactionCallback));

			Marshal.WriteIntPtr(_spvNodeRef,
				Marshal.OffsetOf(typeof(dogecoin_spv_client),
				nameof(dogecoin_spv_client.sync_completed)).ToInt32(), Marshal.GetFunctionPointerForDelegate(syncCompletedCallback));
		}

		private unsafe void SyncCompleted(IntPtr spv)
		{
			_syncComplete = true;
			if (OnSyncComplete != null)
			{
				OnSyncComplete();
			}
		}


		private unsafe void SyncTransaction(IntPtr ctx, IntPtr tx, uint pos, IntPtr blockindex)
		{
			var transaction = Marshal.PtrToStructure<dogecoin_tx>(tx);

			byte[] txHashBytes = new byte[32];
			LibDogecoinInterop.dogecoin_tx_hash(tx, txHashBytes);
			var txId = ByteArrayToHexString(txHashBytes.Reverse().ToArray());

			var blockIdx = Marshal.PtrToStructure<dogecoin_blockindex>(blockindex);
			var blockTimestamp = DateTimeOffset.FromUnixTimeSeconds(blockIdx.header.timestamp);


			if (CurrentBlockInfo == null || CurrentBlockInfo.BlockHeight < blockIdx.height)
			{
				var previousBlock = CurrentBlockInfo;

				CurrentBlockInfo = new SPVNodeBlockInfo
				{
					BlockHeight = blockIdx.height,
					Hash = LittleEndianByteArrayToHexString(blockIdx.hash),
					Timestamp = blockTimestamp,
				};

				BeforeOnNextBlock(previousBlock, CurrentBlockInfo);
			}

			var nodeTransaction = new SPVNodeTransaction();
			var inList = new List<UTXO>();
			var outList = new List<UTXO>();

			nodeTransaction.TxId = txId;
			nodeTransaction.BlockHeight = blockIdx.height;
			nodeTransaction.Timestamp = blockTimestamp;


			//handle inputs
			var vinList = *transaction.vin;
			for (var i = 0; i < vinList.len; i++)
			{
				dogecoin_tx_in vin = Marshal.PtrToStructure<dogecoin_tx_in>(*(vinList.data + i));

				inList.Add(new UTXO
				{
					TxId = ByteArrayToHexString(vin.prevout.hash.Reverse().ToArray()),
					VOut = (int)vin.prevout.n
				});
			}


			//handle outputs
			var voutList = *transaction.vout;

			for (int i = 0; i < voutList.len; i++)
			{
				dogecoin_tx_out vout = Marshal.PtrToStructure<dogecoin_tx_out>(*(voutList.data + i));

				outList.Add(new UTXO
				{
					TxId = txId,
					VOut = i,
					AmountKoinu = vout.value,
					ScriptPubKey = Marshal.PtrToStringAnsi((IntPtr)vout.script_pubkey->str)
				});
			}

			nodeTransaction.In = inList.ToArray();
			nodeTransaction.Out = outList.ToArray();

			if (OnTransaction != null)
			{
				OnTransaction(nodeTransaction);
			}
		}

		private static string ByteArrayToHexString(byte[] bytes)
		{
			var hex = new StringBuilder(bytes.Length * 2);
			foreach (byte b in bytes)
			{
				hex.AppendFormat("{0:x2}", b);
			}
			return hex.ToString();
		}

		public static string LittleEndianByteArrayToHexString(byte[] bytes)
		{
			if (bytes == null)
				return null;

			StringBuilder sb = new StringBuilder();
			for (int i = bytes.Length - 1; i >= 0; i--)
			{
				sb.Append(bytes[i].ToString("X2"));
			}

			return sb.ToString();
		}

		private static byte[] HexStringToLittleEndianByteArray(string hex)
		{
			if (hex == null)
				return null;

			hex = hex.ToUpper();

			if (hex.Length % 2 == 1)
				hex = "0" + hex;

			byte[] bytes = new byte[hex.Length / 2];
			for (int i = 0; i < hex.Length; i += 2)
			{
				bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
			}

			Array.Reverse(bytes);
			return bytes;
		}
	}
}
