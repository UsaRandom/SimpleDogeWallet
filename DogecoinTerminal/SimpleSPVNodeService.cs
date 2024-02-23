﻿using DogecoinTerminal.Common;
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

		private SPVNode _spvNode;
		private SimpleDogeWallet _currentWallet;

		private uint _staleTimerLastBlock;
		private uint _staleTimerCounter = 0;

		public SimpleSPVNodeService()
		{
			TxCount = 0;

			Transactions = new ConcurrentQueue<SPVNodeTransaction>();

			CurrentBlock = new SPVNodeBlockInfo()
			{
				Hash = "5bbc9176db424e1e55d94e0ec79f22974a225c2675d09b90e73b59e58c9f109f",
				BlockHeight = 5079600,
				Timestamp = DateTimeOffset.FromUnixTimeSeconds(1707271081)
			};

			//Task.Run(() =>
			//{
			//	while(!CancelToken.IsCancellationRequested)
			//	{
					
			//		if (Transactions.TryDequeue(out SPVNodeTransaction tx))
			//		{
						
			//		}
			//	}
			//});
		}


		private CancellationTokenSource CancelToken = new CancellationTokenSource();
		private ConcurrentQueue<SPVNodeTransaction> Transactions
		{
			get;
			set;
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

		public void Start()
		{
			if (_spvNode != null && _spvNode.IsRunning)
			{
				return;
			}


			_spvNode = new SPVNodeBuilder()
			//	.StartAt(CurrentBlock.Hash, CurrentBlock.BlockHeight)
				.UseCheckpointFile(SPV_CHECKPOINT_FILE)
				.UseMainNet()
				.OnSyncCompleted(OnSyncComplete)
				.OnNextBlock(HandleOnBlock)
				.EnableDebug()
				.OnTransaction(HandleOnTransaction)
				.Build() ;

			_spvNode.Start();

		}

		public void Rescan(SimpleDogeWallet wallet, SPVNodeBlockInfo startPoint)
		{
			_spvNode?.Stop();

			wallet.UTXOs.Clear();

			wallet.Save();

			_spvNode = new SPVNodeBuilder()
					.StartAt(startPoint)
				.UseCheckpointFile(SPV_CHECKPOINT_FILE)
				.UseMainNet()
				.OnSyncCompleted(OnSyncComplete)
				.OnNextBlock(HandleOnBlock)
				.OnTransaction(HandleOnTransaction)
				.Build();

			Transactions.Clear();
			_spvNode.Start();

		}

		private void OnSyncComplete()
		{
			Messenger.Default.Send(new SPVSyncCompletedMessage());
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

	class SPVSyncCompletedMessage
	{

	}
}
