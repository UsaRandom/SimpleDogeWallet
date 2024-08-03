using Lib.Dogecoin;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleDogeWallet.Common
{
	internal class SPVFastSyncService
	{
		private SPVNodeBlockInfo _startBlock = new SPVNodeBlockInfo()
		{
			Hash = "266ef099a151f6b4f9fe4834a1dde26b26ecf1eae78d685f8c6c410258e2640d",
			BlockHeight = 5280000,
			Timestamp = DateTimeOffset.FromUnixTimeSeconds(1720070798)
		};

		private SPVNode _spvNode;
		private uint _bestKnownHeight = 0;
		private int _updateCount = 1;

		public SPVNodeBlockInfo ChainTip;

		public void Run()
		{
			ChainTip = _startBlock;
			_spvNode = new SPVNodeBuilder()
						.StartAt(ChainTip)
						.UseMainNet()
						.OnSyncCompleted(OnSyncCompleted)
						.OnHeaderMessageProcessed(OnHeaderMessageProcessed)
						.Build();

			_spvNode.Start();
		}

		public void Stop()
		{
			_spvNode?.Stop();
		}

		private uint GetBestKnownHeight()
		{
			if (_bestKnownHeight == 0)
			{
				_bestKnownHeight = _spvNode.GetBestKnownHeight();
			}
			return _bestKnownHeight;
		}

		private void OnSyncCompleted()
		{
			_spvNode.Stop();
			Messenger.Default.Send(new SPVSyncProgressMessage(GetBestKnownHeight(), ChainTip, 1.0M));
		}

		private void OnHeaderMessageProcessed(SPVNodeBlockInfo newChainTip)
		{

			var currentBlockHeight = ChainTip.BlockHeight;
			var bestKnownHeight = GetBestKnownHeight();


			ChainTip = newChainTip;

			var percentageLeft = (decimal)(GetBestKnownHeight() - ChainTip.BlockHeight) / (decimal)(GetBestKnownHeight() - _startBlock.BlockHeight);


			Messenger.Default.Send(new SPVSyncProgressMessage(GetBestKnownHeight(), newChainTip, 1.0M - percentageLeft));



			_updateCount++;

			if (bestKnownHeight == currentBlockHeight)
			{
				_spvNode.Stop();
				return;
			}
		}


	}
	public class SPVSyncProgressMessage
	{
		public SPVSyncProgressMessage(uint bestKnownHeight, SPVNodeBlockInfo block, decimal percentDone)
		{
			BestKnownHeight = bestKnownHeight;
			Block = block;
			PercentDone = percentDone;
		}

		public uint BestKnownHeight
		{
			get;
			set;
		}

		public SPVNodeBlockInfo Block
		{
			get; set;
		}

		public decimal PercentDone
		{
			get;
			set;
		}
	}
}