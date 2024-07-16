using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.Dogecoin
{
	public class SPVNodeBuilder
	{
		private SPVNodeBlockInfo _start;
		private bool _useMainNet = false;
		private bool _fullSync = false;
		private bool _useDebug = false;
		private int _peerCount = 24;
		private Action _onSyncCompleted;
		private Action<SPVNodeBlockInfo> _onHeaderMessage;
		private Action<SPVNodeTransaction> _onTransaction;
		private Action<SPVNodeBlockInfo, SPVNodeBlockInfo> _onNextBlock;

		public ISPVCheckpointTracker CheckpointTracker { get; set; }


		public SPVNodeBuilder StartAt(SPVNodeBlockInfo checkpoint)
		{
			_start = checkpoint;
			return this;
		}

		public SPVNodeBuilder StartAt(string blockHash, uint blockHeight)
		{
			return StartAt(new SPVNodeBlockInfo
			{
				Hash = blockHash,
				BlockHeight = blockHeight
			});
		}

		public SPVNodeBuilder EnableDebug()
		{
			_useDebug = true;
			return this;
		}

		public SPVNodeBuilder FullSync()
		{
			_fullSync = true;
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

		public SPVNodeBuilder OnSyncCompleted(Action action)
		{
			_onSyncCompleted = action;
			return this;
		}

		public SPVNodeBuilder OnHeaderMessageProcessed(Action<SPVNodeBlockInfo> action)
		{
			_onHeaderMessage = action;
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
			node.OnSyncComplete = _onSyncCompleted;
			node.OnProcessedHeaders = _onHeaderMessage;

			return node;
		}

	}

}
