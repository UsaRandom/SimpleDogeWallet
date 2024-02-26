using DogecoinTerminal.Common;
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

namespace DogecoinTerminal
{
	//TODO: Remove duplicate code
	internal class SimpleSPVNodeService
	{
		private string SPV_CHECKPOINT_FILE = "spvcheckpoint";

		public SPVNodeBlockInfo NEW_WALLET_START_BLOCK = new SPVNodeBlockInfo()
		{
			Hash = "32a1625e7fe68f3b5e237e4351d6df12e92eb7f9130b4d60a8b1c803f09fafee",
			BlockHeight = 5105268,
			Timestamp = DateTimeOffset.FromUnixTimeSeconds(1708906209)
		};

		private SPVNode _spvNode;
		private SimpleDogeWallet _currentWallet;

		private uint _staleTimerLastBlock;
		private uint _staleTimerCounter = 0;

		public SimpleSPVNodeService()
		{
			TxCount = 0;
			CurrentBlock = NEW_WALLET_START_BLOCK;
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


		public void SetWallet(SimpleDogeWallet wallet)
		{
			_currentWallet = wallet;
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
					.UseCheckpointFile(SPV_CHECKPOINT_FILE)
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
					.UseCheckpointFile(SPV_CHECKPOINT_FILE)
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

		//	wallet.UTXOs.Clear();

			//wallet.Save();

			_spvNode = new SPVNodeBuilder()
					.StartAt(startPoint)
				.UseCheckpointFile(SPV_CHECKPOINT_FILE)
				.UseMainNet()
				.OnSyncCompleted(OnSyncComplete)
				.OnNextBlock(HandleOnBlock)
				.OnTransaction(HandleOnTransaction)
				.Build();

			_spvNode.Start();

		}

		private void OnSyncComplete()
		{
			Messenger.Default.Send(new UpdateSendButtonMessage());
		}

		private void HandleOnBlock(SPVNodeBlockInfo previous,  SPVNodeBlockInfo next)
		{
			if(CurrentBlock.BlockHeight + 1 != next.BlockHeight)
			{
				Debug.WriteLine("Out of order? Ok on first block");
			}

			CurrentBlock = next;

			Messenger.Default.Send(next);
		}


		private void HandleOnTransaction(SPVNodeTransaction tx)
		{
			bool walletChanged = false;

			TxCount++;

			if(tx.TxId.ToUpper() == _currentWallet.PendingTxHash.ToUpper())
			{
				_currentWallet.PendingTxHash = string.Empty;
				_currentWallet.PendingAmount = 0;
			}

			SpentUTXOCount += (ulong)tx.In.Length;
			NewUTXOCount += (ulong)tx.Out.Length;

			foreach (var spentUtxo in tx.In)
			{
				if (_currentWallet.UTXOs.Remove(spentUtxo))
				{
					walletChanged = true;
				}
			}

			foreach (var newUtxo in tx.Out)
			{
				var utxoAddress = newUtxo.ScriptPubKey.GetP2PKHAddress();

				if (!string.IsNullOrEmpty(utxoAddress) &&
					utxoAddress == _currentWallet.Address &&
					!_currentWallet.UTXOs.Contains(newUtxo))
				{
						_currentWallet.UTXOs.Add(newUtxo);
						walletChanged = true;
					
				}
			}

			if (walletChanged)
			{
				_currentWallet.Save();

				Messenger.Default.Send(new SPVUpdatedWalletMessage());
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
}
