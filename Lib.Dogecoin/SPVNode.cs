using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;
using Lib.Dogecoin.Interop;

namespace Lib.Dogecoin
{

	public class SPVNodeBuilder
	{
		private SPVCheckpoint _start;
		private bool _useMainNet = false;
		private bool _useDebug = false;
		private Action<SPVNodeTransaction> _onTransaction;
		private Action<SPVNodeBlockInfo, SPVNodeBlockInfo> _onNextBlock;

		public ISPVCheckpointTracker CheckpointTracker { get; set; }


		public SPVNodeBuilder StartAt(SPVCheckpoint checkpoint)
		{
			_start = checkpoint;
			return this;
		}

		public SPVNodeBuilder StartAt(string blockHash, uint blockHeight)
		{
			return StartAt(new SPVCheckpoint
			{
				BlockHash = blockHash,
				BlockHeight = blockHeight
			});
		}

		public SPVNodeBuilder EnableDebug()
		{
			_useDebug = true;
			return this;
		}

		public SPVNodeBuilder UseMainNet()
		{
			_useMainNet = true;
			return this;
		}

		public SPVNodeBuilder UseTestNet()
		{
			_useMainNet = false;
			return this;
		}

		public SPVNodeBuilder OnTransaction(Action<SPVNodeTransaction> action)
		{
			_onTransaction = action;
			return this;
		}


		public SPVNodeBuilder OnNextBlock(Action<SPVNodeBlockInfo, SPVNodeBlockInfo> action)
		{
			_onNextBlock = action;
			return this;
		}

		public SPVNode Build()
		{
			var node = new SPVNode(CheckpointTracker, _useMainNet, _useDebug, _start);

			node.OnTransaction = _onTransaction;
			node.OnNextBlock = _onNextBlock;

			return node;
		}

	}

	internal class SPVFileCheckpointTracker : ISPVCheckpointTracker
	{
		private string _file;

		public SPVFileCheckpointTracker(string file)
		{
			_file = file;
		}

		public SPVCheckpoint GetCheckpoint()
		{
			if (File.Exists(_file))
			{
				var content = File.ReadAllText(_file);

				if (string.IsNullOrEmpty(content))
				{
					return null;
				}

				var parts = content.Split(":");

				return new SPVCheckpoint
				{
					BlockHash = parts[0],
					BlockHeight = uint.Parse(parts[1])
				};
			}

			return null;
		}

		public void SaveCheckpoint(SPVCheckpoint checkpoint)
		{
			try
			{
				File.WriteAllText(_file, $"{checkpoint.BlockHash}:{checkpoint.BlockHeight}");
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"Failed to save checkpoing: {checkpoint.BlockHash}:{checkpoint.BlockHeight}");
				Debug.WriteLine(ex);
			}
		}
	}

	public static class SPVFileCheckpointTrackerExtensions
	{
		public static SPVNodeBuilder UseCheckpointFile(this SPVNodeBuilder builder, string file)
		{
			builder.CheckpointTracker = new SPVFileCheckpointTracker(file);

			return builder;
		}
	}

	public interface ISPVCheckpointTracker
	{
		SPVCheckpoint GetCheckpoint();

		void SaveCheckpoint(SPVCheckpoint checkpoint);
	}


	public class SPVCheckpoint
	{
		public string BlockHash { get; set; }
		public uint BlockHeight { get; set; }
	}

	public class SPVNode
	{
		private const int BLOCKS_BETWEEN_CHECKPOINTS = 10;

		private uint _lastSPVCheckpointHeight = 0;

		private bool _isDebug;
		private Thread _thread;
		private IntPtr _spvNodeRef;
		private static dogecoin_spv_client.sync_transaction_delegate syncTransactionCallback;

		private ISPVCheckpointTracker _checkpointTracker;
		private SPVCheckpoint _startPoint;

		public SPVNode(ISPVCheckpointTracker tracker, bool isMainNet, bool isDebug, SPVCheckpoint startPoint)
		{
			_checkpointTracker = tracker;
			_isDebug = isDebug;
			IsMainNet = isMainNet;
			_startPoint = startPoint;
		}

		public bool IsRunning { get; private set; }

		public bool IsMainNet { get; private set; }

		public SPVNodeBlockInfo CurrentBlockInfo { get; private set; }

		public Action<SPVNodeTransaction> OnTransaction { get; set; }

		public Action<SPVNodeBlockInfo, SPVNodeBlockInfo> OnNextBlock { get; set; }

		private void BeforeOnNextBlock(SPVNodeBlockInfo previousBlock, SPVNodeBlockInfo nextBlock)
		{
			if (_checkpointTracker != null && previousBlock != null)
			{
				if (previousBlock.BlockHeight - _lastSPVCheckpointHeight >= BLOCKS_BETWEEN_CHECKPOINTS)
				{
					_checkpointTracker.SaveCheckpoint(new SPVCheckpoint
					{
						BlockHash = previousBlock.Hash,
						BlockHeight = previousBlock.BlockHeight
					});
					_lastSPVCheckpointHeight = previousBlock.BlockHeight;
				}
			}

			if (OnNextBlock != null)
			{
				OnNextBlock(previousBlock, nextBlock);
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

				LibDogecoinInterop.dogecoin_spv_client_discover_peers(_spvNodeRef, null);

				unsafe
				{
					var client = Marshal.PtrToStructure<dogecoin_spv_client>(_spvNodeRef);

					var headerDb = *client.headers_db;

					if (headerDb.has_checkpoint_start(client.headers_db_ctx) == 0)
					{
						SPVCheckpoint checkpoint = null;

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
														  HexStringToLittleEndianByteArray(checkpoint.BlockHash),
														  checkpoint.BlockHeight);
						}
					}

				}

				LibDogecoinInterop.dogecoin_spv_client_runloop(_spvNodeRef);
				IsRunning = false;
			});

			_thread.Start();
		}

		public void Stop()
		{
			var spv = Marshal.PtrToStructure<dogecoin_spv_client>(_spvNodeRef);

			LibDogecoinInterop.dogecoin_node_group_shutdown(spv.nodegroup);
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

			var net = IsMainNet ? LibDogecoinContext._mainChain : LibDogecoinContext._testChain;

			_spvNodeRef = LibDogecoinInterop.dogecoin_spv_client_new(net, _isDebug, true, false, true);


			syncTransactionCallback = new dogecoin_spv_client.sync_transaction_delegate(SyncTransaction);

			Marshal.WriteIntPtr(_spvNodeRef,
				Marshal.OffsetOf(typeof(dogecoin_spv_client),
				nameof(dogecoin_spv_client.sync_transaction)).ToInt32(), Marshal.GetFunctionPointerForDelegate(syncTransactionCallback));
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
				dogecoin_tx_in vin = Marshal.PtrToStructure<dogecoin_tx_in>(*(vinList.data));

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
