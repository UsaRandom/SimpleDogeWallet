using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.Dogecoin
{
	public interface ISPVCheckpointTracker
	{
		SPVNodeBlockInfo GetCheckpoint();

		void SaveCheckpoint(SPVNodeBlockInfo checkpoint);
	}
}
