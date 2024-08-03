using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleDogeWallet.Common.Interop
{
	public class TextCopyClipboardService : IClipboardService
	{
		public string GetClipboardContents()
		{
			return TextCopy.ClipboardService.GetText();
		}

		public void SetClipboardContents(string contents)
		{
			TextCopy.ClipboardService.SetText(contents);
		}
	}
}
