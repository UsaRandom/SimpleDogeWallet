using SimpleDogeWallet.Common;
using Lib.Dogecoin;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Transactions;
using ZXing;

namespace SimpleDogeWallet
{
	//TODO: Remove duplicate code
	public class SimpleSPVNodeService
	{
		private string SPV_CHECKPOINT_FILE = "spvcheckpoint";

		public SPVNodeBlockInfo NEW_WALLET_START_BLOCK = new SPVNodeBlockInfo()
		{
			Hash = "32a1625e7fe68f3b5e237e4351d6df12e92eb7f9130b4d60a8b1c803f09fafee",
			BlockHeight = 5105268,
			Timestamp = DateTimeOffset.FromUnixTimeSeconds(1708906209)
		};

		private SPVNode _spvNode;

		private uint _staleTimerLastBlock;
		private uint _staleTimerCounter = 0;

		private const int SPV_CHECKPOINT_BLOCKS_BEHIND = 30;



		long currentBlock = 0;
		long currentMinFee = long.MaxValue;
		long sumOfFees = 0;
		int txsWithFee = 0;

		long blockSize = 0;

		private LimitedQueue<long> blockFees = new LimitedQueue<long>(30);
		private UTXOSampleIndex utxos = new UTXOSampleIndex(25000);


		

		public SimpleSPVNodeService()
		{
			TxCount = 0;
			CurrentBlock = NEW_WALLET_START_BLOCK;
			blockFees.Enqueue(1000);
		}


		public decimal EstimatedRate
		{
			get; set;
		}

		public uint EstimatedHeight
		{
			get
			{
				var diff = DateTimeOffset.UtcNow - CurrentBlock.Timestamp;
				return (uint)diff.TotalMinutes + CurrentBlock.BlockHeight;
			}
		}

		public SPVNodeBlockInfo CurrentBlock
		{
			get;
			private set;
		}

		public ulong TxCount
		{
			get; private set;
		}

		public ulong SpentUTXOCount
		{
			get; private set;
		}

		public ulong NewUTXOCount
		{
			get; private set;
		}

		public bool IsRunning
		{
			get
			{
				return _spvNode != null && _spvNode.IsRunning;
			}
		}


		private int _peerCount = 0;
		private long _timeBetweenRefresh = 500;
		private long _lastRefreshTime = 0;

		public int PeerCount
		{
			get
			{
				if (!IsRunning)
				{
					return 0;
				}
				var currentTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();

				if(currentTime - _lastRefreshTime > _timeBetweenRefresh)
				{
					//peercount is an expensive call, so we only refresh so often
					_peerCount = _spvNode.GetPeerCount();
					_lastRefreshTime = currentTime;
				}

				return _peerCount;
			}
		}


		public bool SyncCompleted
		{
			get
			{
				return _spvNode.SyncComplete;
			}
		}

		public void PrintDebug()
		{
			_spvNode?.PrintDebug();
		}

		public void Start(bool isNew = false)
		{
			if (_spvNode != null && _spvNode.IsRunning)
			{
				return;
			}


			if (isNew)
			{
				_spvNode = new SPVNodeBuilder()
					.StartAt(NEW_WALLET_START_BLOCK.Hash, NEW_WALLET_START_BLOCK.BlockHeight)
					.UseCheckpointFile(SPV_CHECKPOINT_FILE, SPV_CHECKPOINT_BLOCKS_BEHIND)
					.UseMainNet()
					.OnSyncCompleted(OnSyncComplete)
					.OnNextBlock(HandleOnBlock)
					.EnableDebug()
					.OnTransaction(HandleOnTransaction)
					.Build();
			}
			else
			{
				_spvNode = new SPVNodeBuilder()
					.UseCheckpointFile(SPV_CHECKPOINT_FILE, SPV_CHECKPOINT_BLOCKS_BEHIND)
					.UseMainNet()
					.OnSyncCompleted(OnSyncComplete)
					.OnNextBlock(HandleOnBlock)
					.EnableDebug()
					.OnTransaction(HandleOnTransaction)
					.Build();
			}
			

			_spvNode.Start();

		}

		public void Rescan(SPVNodeBlockInfo startPoint)
		{
			_spvNode?.Stop();

			utxos.Clear();
			blockFees.Clear();

			//	wallet.UTXOs.Clear();

			//wallet.Save();

			_spvNode = new SPVNodeBuilder()
					.StartAt(startPoint)
				.UseCheckpointFile(SPV_CHECKPOINT_FILE, SPV_CHECKPOINT_BLOCKS_BEHIND)
				.UseMainNet()
				.OnSyncCompleted(OnSyncComplete)
				.OnNextBlock(HandleOnBlock)
				.OnTransaction(HandleOnTransaction)
					.EnableDebug()
				.Build();

			_spvNode.Start();

		}

		private void OnSyncComplete()
		{
			Messenger.Default.Send(new UpdateSendButtonMessage());
		}

		private void HandleOnBlock(SPVNodeBlockInfo previous,  SPVNodeBlockInfo next)
		{
			if (!_spvNode.IsRunning)
			{
				return;
			}

			if (CurrentBlock.BlockHeight + 1 != next.BlockHeight)
			{
				Debug.WriteLine("Out of order? Ok on first block");
			}

			CurrentBlock = next;

			Messenger.Default.Send(next);
		}


		private unsafe void HandleOnTransaction(SPVNodeTransaction tx)
		{
			if(!_spvNode.IsRunning)
			{
				return;
			}

			bool walletChanged = false;

			TxCount++;

			if(tx.TxId.ToUpper() == SimpleDogeWallet.Instance.PendingTxHash?.ToUpper())
			{
				SimpleDogeWallet.Instance.PendingTxHash = string.Empty;
				SimpleDogeWallet.Instance.PendingAmount = 0;
			}

			SpentUTXOCount += (ulong)tx.In.Length;
			NewUTXOCount += (ulong)tx.Out.Length;

			foreach (var spentUtxo in tx.In)
			{
				UTXO targetUtxoToRemove = default;

				foreach(var utx in SimpleDogeWallet.Instance.UTXOs)
				{
					if(spentUtxo.TxId == utx.TxId && spentUtxo.VOut == utx.VOut)
					{
						walletChanged = true;
						targetUtxoToRemove = spentUtxo;
						break;
					}
				}
				if(targetUtxoToRemove != default)
				{
					SimpleDogeWallet.Instance.UTXOs.Remove(targetUtxoToRemove);
				}
			}

			foreach (var newUtxo in tx.Out)
			{
				var utxoAddress = LibDogecoinContext.Instance.UnsafeGetP2PKHAddress(newUtxo.ScriptPubKey);

				if (!string.IsNullOrEmpty(utxoAddress) &&
					utxoAddress == SimpleDogeWallet.Instance.Address &&
					!SimpleDogeWallet.Instance.UTXOs.Contains(newUtxo))
				{
						SimpleDogeWallet.Instance.UTXOs.Add(newUtxo);
						walletChanged = true;
					
				}
			}

			if (walletChanged)
			{
				SimpleDogeWallet.Instance.Save();

				Messenger.Default.Send(new UpdateSendButtonMessage());
			}




			//Fee Calculation
			
			if (tx.BlockHeight > currentBlock)
			{
				if (currentBlock != 0)
				{
					if (currentMinFee != long.MaxValue)
					{
						blockFees.Enqueue(currentMinFee);
					}


					EstimatedRate = ((decimal)(blockFees.Min()) / (decimal)100000000);
					
					currentMinFee = long.MaxValue;
				}

				currentBlock = tx.BlockHeight;
				blockSize = 0;
			}


			bool allInputsPresent = true;
			long inputVal = 0;

			foreach (var input in tx.In)
			{
				var i = utxos.GetUTXOOrDefault(input.TxId, input.VOut);
				if (i == default(UTXO))
				{
					allInputsPresent = false;
					break;
				}
				inputVal += i.AmountKoinu.Value;
			}

			if (allInputsPresent)
			{
				var outputVal = tx.Out.Sum(o => o.AmountKoinu);
				var fee = (inputVal - outputVal) / tx.SizeBytes;

				if (fee <= currentMinFee)
				{
					currentMinFee = (long)fee;
				}
			}

			blockSize += tx.SizeBytes;

			foreach (var o in tx.Out)
			{
				utxos.Enqueue(o);
			}

			

		}


		public void Stop()
		{
			_spvNode?.Stop();
			TxCount = 0;
			SpentUTXOCount = 0;
			NewUTXOCount = 0;
		}
	}


	class SPVUpdatedWalletMessage
	{

	}

	class UpdateSendButtonMessage
	{

	}

	class UpdateSPVTextMessage
	{

	}


	class UTXOSampleIndex : Queue<UTXO>
	{
		private readonly int _limit;
		private Dictionary<string, UTXO> _index;

		public UTXOSampleIndex(int limit) : base(limit)
		{
			_limit = limit;
			_index = new Dictionary<string, UTXO>(limit);
		}

		public new void Enqueue(UTXO item)
		{
			if (Count >= _limit)
			{
				var utxoToRemvoe = Dequeue(); // Kick out the oldest utxo
				_index.Remove(utxoToRemvoe.TxId + utxoToRemvoe.VOut);
			}
			base.Enqueue(item);
		}

		public UTXO GetUTXOOrDefault(string txId, int vout)
		{
			return _index.GetValueOrDefault(txId + vout);
		}
	}

	class LimitedQueue<T> : Queue<T>
	{
		private readonly int _limit;

		public LimitedQueue(int limit) : base(limit)
		{
			_limit = limit;
		}

		public new void Enqueue(T item)
		{
			if (Count >= _limit)
			{
				Dequeue(); // Kick out the oldest item
			}
			base.Enqueue(item);
		}
	}
}
