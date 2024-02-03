using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DogecoinTerminal.Common
{
	public class CheckboxControl : ButtonControl
	{
		private bool _isChecked = false;

		public CheckboxControl(XElement element)
			: base(element)
		{
			Text = string.Empty;
		}

		public bool IsChecked
		{
			get
			{
				return _isChecked;
			}
			set
			{
				_isChecked = value;
				if (_isChecked )
				{
					Text = "X";
				}
				else
				{
					Text = string.Empty;
				}
			}
		}




	}
}
