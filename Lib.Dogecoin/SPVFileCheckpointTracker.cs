using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.Dogecoin
{
	internal class SPVFileCheckpointTracker : ISPVCheckpointTracker
	{
		private string _file;

		public SPVFileCheckpointTracker(string file)
		{
			_file = file;
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

				return new SPVNodeBlockInfo
				{
					Hash = parts[0],
					BlockHeight = uint.Parse(parts[1])
				};
			}

			return null;
		}

		public void SaveCheckpoint(SPVNodeBlockInfo checkpoint)
		{
			try
			{
				File.WriteAllText(_file, $"{checkpoint.Hash}:{checkpoint.BlockHeight}");
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"Failed to save checkpoing: {checkpoint.Hash}:{checkpoint.BlockHeight}");
				Debug.WriteLine(ex);
			}
		}
	}
}
