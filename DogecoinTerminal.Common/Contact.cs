using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DogecoinTerminal
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
				_name = value?.Substring(0, 16);
			}
		}

		public string ShortAddress
		{
			get
			{
				if (!string.IsNullOrEmpty(Address))
				{
					return Address.Substring(0, 4) + "..." + Address.Substring(Address.Length - 4, 4);
				}
				return string.Empty;
			}
		}

		public string Address { get; set; }
	}
}
