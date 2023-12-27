using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using DogecoinTerminal.Common;
using Microsoft.Xna.Framework;
using static DogecoinTerminal.Common.DisplayQRPage;

namespace DogecoinTerminal.QRDoge
{
	public class QRDogecoinService : IDogecoinService
	{
		private Game _game;

		public QRDogecoinService(Game game)
		{
			_game = game;
		}

		public void GetUTXOs(string address, string pin, Action<IEnumerable<UTXOInfo>> callback)
		{
			// This is a super simple implimentation.
			// 
			// This creates a very low limit on the number of UTXOs the terminal can support.
			// Multiple scans is the best way forward.

			_game.Services.GetService<Router>().Route("scanqr", "Press 'Get UTXOs' on Phone", true, (qrString) =>
			{					
				var utxoList = new List<UTXOInfo>();

				try
				{
					var lines = qrString.Split('\n');


					foreach (var line in lines)
					{
						if (string.IsNullOrEmpty(line))
						{
							continue;
						}

						var lineParts = line.Split('|');

						utxoList.Add(new UTXOInfo
						{
							TransactionId = lineParts[0],
							VOut = Int32.Parse(lineParts[1]),
							Amount = decimal.Parse(lineParts[2])
						});
					}

				} finally
				{
					callback(utxoList);
				}
			});
		}

		public void OnDeleteAddress(string address, string pin, Action callback)
		{
			//the corresponding QRDoge app will request that the node stop watching this address.
			//
			// qrdoge-0-delete:address
			_game.Services.GetService<Router>().Route("displayqr", new DisplayQRPageSettings($"qrdoge:0-delete:{address}", "Scan with QRDoge App (Delete Address)", true), true, (scanAcknowledged) =>
			{
				//nothing else to do
				callback();
			});
		}

		public void OnNewAddress(string address, string pin, Action<bool> callback)
		{

			//the corresponding QRDoge app will request that the node start watching this address.

			_game.Services.GetService<Router>().Route("displayqr", new DisplayQRPageSettings($"qrdoge:0-new:{address}", "Scan with QRDoge App (New Address)", true), true, (scanAcknowledged) =>
			{
				//nothing else to do
				callback(scanAcknowledged);
			});
		}

		public void OnReset(Action<bool> callback)
		{
			callback(true);
		}

		public void OnSetup(Action<bool> callback)
		{
			callback(true);
		}

		public void SendTransaction(string transaction, string pin, Action<bool> callback)
		{
			//the corresponding QRDoge app will request that the node broadcast this transaction.
			_game.Services.GetService<Router>().Route("displayqr", new DisplayQRPageSettings($"qrdoge:0-send:{transaction}", "Scan with QRDoge App (Send Transaction)", true), true, (scanAcknowledged) =>
			{
				callback(scanAcknowledged);
			});
		}

		public void UpdatePin(string address, string oldPin, string newPin, Action<bool> callback)
		{
			callback(true);
		}
	}
}
