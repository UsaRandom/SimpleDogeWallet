using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleDogeWallet
{
	public class Contact
	{
		private string _name;

		public string Name
		{
			get
			{
				return _name;
			}
			set
			{
				_name = value?.Substring(0, Math.Min(value.Length, 16));
			}
		}

		public string ShortAddress
		{
			get
			{
				if (!string.IsNullOrEmpty(Address))
				{
					return Address.Substring(0, 6) + "..." + Address.Substring(Address.Length - 4, 4);
				}
				return string.Empty;
			}
		}

		public string Address { get; set; }
	}
}
