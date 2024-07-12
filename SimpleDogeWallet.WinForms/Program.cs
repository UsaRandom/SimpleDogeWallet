
using Lib.Dogecoin;
using System;
using System.Windows.Forms;

namespace SimpleDogeWallet.WinForms
{
	internal class Program
	{
		[STAThread]
		private static void Main(string[] args)
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			var game = new SimpleDogeWalletWinFormGame();
			game.Run();
		}
	}
}