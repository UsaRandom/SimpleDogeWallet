using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.Dogecoin
{
	public class SPVFileCheckpointTracker : ISPVCheckpointTracker
	{
		private string _file;

		private int _blocksBehind = 10;

		private ConcurrentQueue<SPVNodeBlockInfo> _previousBlocks;

		public SPVFileCheckpointTracker(string file, int blocksBehind = 10)
		{
			_file = file;
			_previousBlocks = new ConcurrentQueue<SPVNodeBlockInfo>();
			_blocksBehind = blocksBehind;
		}

		public SPVNodeBlockInfo GetCheckpoint()
		{
			if (File.Exists(_file))
			{
				var content = File.ReadAllText(_file);

				if (string.IsNullOrEmpty(content))
				{
					return null;
				}

				var parts = content.Split(":");
				
				var checkpoint = new SPVNodeBlockInfo
				{
					Hash = parts[0],
					BlockHeight = uint.Parse(parts[1])
				};


				_previousBlocks = new ConcurrentQueue<SPVNodeBlockInfo>();
				_previousBlocks.Enqueue(checkpoint);

				return checkpoint;
			}

			return null;
		}

		public void SaveCheckpoint(SPVNodeBlockInfo checkpoint)
		{
			_previousBlocks.Enqueue(checkpoint);

			try
			{
				if (_previousBlocks.Count >= _blocksBehind)
				{
					string checkpointData = $"{checkpoint.Hash}:{checkpoint.BlockHeight}";
					string tempFilePath = Path.GetTempFileName();

					File.WriteAllText(tempFilePath, checkpointData);

					File.Move(tempFilePath, _file, true);
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"Failed to save checkpoint: {checkpoint.Hash}:{checkpoint.BlockHeight}");
				Debug.WriteLine(ex);
			}
		}
	}
}
