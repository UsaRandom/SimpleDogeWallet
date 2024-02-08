using DogecoinTerminal.Common;
using Lib.Dogecoin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DogecoinTerminal
{
	//TODO: Remove duplicate code
	internal class SimpleSPVNodeService
	{
		private string SPV_CHECKPOINT_FILE = "spvcheckpoint";

		private SPVNode _spvNode;
		private SimpleDogeWallet _currentWallet;

		public SimpleSPVNodeService()
		{
			CurrentBlock = new SPVNodeBlockInfo()
			{
				Hash = "5bbc9176db424e1e55d94e0ec79f22974a225c2675d09b90e73b59e58c9f109f",
				BlockHeight = 5079600,
				Timestamp = DateTimeOffset.FromUnixTimeSeconds(1707271081)
			};
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

		public bool IsRunning
		{
			get
			{
				return _spvNode != null && _spvNode.IsRunning;
			}
		}

		public int PeerCount
		{
			get
			{
				if (!IsRunning)
				{
					return 0;
				}
				return _spvNode.GetPeerCount();
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
				.OnTransaction(HandleOnTransaction)
				.Build() ;

			_spvNode.Start();
		}

		public void Rescan(SimpleDogeWallet wallet, SPVNodeBlockInfo startPoint)
		{
			_spvNode?.Stop();

			wallet.UTXOs.Clear();
			wallet.PendingSpentUTXOs.Clear();

			wallet.Save();

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
			Messenger.Default.Send(new SPVSyncCompletedMessage());
		}

		private void HandleOnBlock(SPVNodeBlockInfo previous,  SPVNodeBlockInfo next)
		{
			CurrentBlock = next;

			Messenger.Default.Send(next);
		}

		private void HandleOnTransaction(SPVNodeTransaction tx)
		{
			bool walletChanged = false;

			foreach (var spentUtxo in tx.In)
			{
				if (_currentWallet.PendingSpentUTXOs.Remove(spentUtxo))
				{
					walletChanged = true;
				}

				if (_currentWallet.UTXOs.Remove(spentUtxo))
				{
					walletChanged = true;
				}
			}

			foreach (var newUtxo in tx.Out)
			{
				var utxoAddress = newUtxo.ScriptPubKey.GetP2PKHAddress();

				if (!string.IsNullOrEmpty(utxoAddress) &&
					utxoAddress == _currentWallet.Address)
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
		}
	}


	class SPVUpdatedWalletMessage
	{

	}

	class SPVSyncCompletedMessage
	{

	}
}
