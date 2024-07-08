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

		public UTXO[] Out { get; set; }
		public int SizeBytes { get; set; }
	}

	
}
