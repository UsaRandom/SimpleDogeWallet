
using System;
using System.Runtime.InteropServices;


namespace DogecoinTerminal.Common
{
	public class WindowsClipboardService : IClipboardService
	{
		public string GetClipboardContents()
		{
			string str;
			if (OpenClipboard(IntPtr.Zero))
			{
				try
				{
					IntPtr hGlobal = GetClipboardData(CF_UNICODETEXT);
					if (hGlobal != IntPtr.Zero)
					{
						str = Marshal.PtrToStringUni(GlobalLock(hGlobal));
						GlobalUnlock(hGlobal);
					}
					else
					{
						str = string.Empty;
					}
				}
				finally
				{
					CloseClipboard();
				}
			}
			else
			{
				throw new Exception("Failed to open the clipboard.");
			}

			return str;
		}

		public void SetClipboardContents(string str)
		{
			if (OpenClipboard(IntPtr.Zero))
			{
				try
				{
					EmptyClipboard();

					IntPtr hGlobal = Marshal.StringToHGlobalUni(str);
					SetClipboardData(CF_UNICODETEXT, hGlobal);
					Marshal.FreeHGlobal(hGlobal);
				}
				finally
				{
					CloseClipboard();
				}
			}
			else
			{
				throw new Exception("Failed to open the clipboard.");
			}
		}

		[DllImport("user32.dll", SetLastError = true)]
		private static extern bool OpenClipboard(IntPtr hWndNewOwner);

		[DllImport("user32.dll", SetLastError = true)]
		private static extern bool CloseClipboard();

		[DllImport("user32.dll", SetLastError = true)]
		private static extern bool EmptyClipboard();

		[DllImport("user32.dll", SetLastError = true)]
		private static extern IntPtr SetClipboardData(uint uFormat, IntPtr data);

		[DllImport("user32.dll", SetLastError = true)]
		private static extern IntPtr GetClipboardData(uint uFormat);

		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern IntPtr GlobalLock(IntPtr hMem);

		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern bool GlobalUnlock(IntPtr hMem);

		private const uint CF_UNICODETEXT = 13;
	}
}
