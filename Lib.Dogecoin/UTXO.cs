using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.Dogecoin
{
	public class UTXO
	{

		public override bool Equals(object? obj)
		{
			if (obj == null || !(obj is UTXO))
			{
				return false;
			}

			return this == (UTXO)obj;
		}

		public static bool operator ==(UTXO left, UTXO right)
		{
			return left.TxId == right.TxId &&
					left.VOut == right.VOut;
		}


		public static bool operator !=(UTXO left, UTXO right)
		{
			return left.VOut != right.VOut || left.TxId != right.TxId;
		}

		public string TxId { get; set; }
		
		public int VOut { get; set; }

		public long? AmountKoinu { get; set; }

		public decimal Amount
		{
			get
			{
				return AmountKoinu.HasValue ? AmountKoinu.Value / 100000000M : 0;
			}
			set
			{
				AmountKoinu = (long)(value / 100000000M);
			}
		}

		public string? ScriptPubKey { get; set; }
	}
}
