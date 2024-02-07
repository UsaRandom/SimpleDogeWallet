using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.Dogecoin
{
	public class SPVNodeTransaction
	{
		public string TxId { get; set; }

		public uint BlockHeight { get; set; }

		public DateTimeOffset Timestamp { get; set; }

		public UTXO[] In { get; set; }

		public UTXO[] Out { get; set;}
	}

	public class SPVNodeBlockInfo
	{
		public static SPVNodeBlockInfo MAIN_GENISIS_BLOCK = new SPVNodeBlockInfo
		{
			Hash = "1a91e3dace36e2be3bf030a65679fe821aa1d6ef92e7c9902eb318182c355691",
			BlockHeight = 0,
			Timestamp = DateTimeOffset.FromUnixTimeSeconds(1386325540)
		};

		public string Hash { get; set; }

		public uint BlockHeight { get; set; }

		public DateTimeOffset Timestamp { get; set; }
	}
}
