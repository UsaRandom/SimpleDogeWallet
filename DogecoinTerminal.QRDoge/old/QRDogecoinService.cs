using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using DogecoinTerminal.Common.old;
using Microsoft.Xna.Framework;
using static DogecoinTerminal.Common.old.DisplayQRPage;

namespace DogecoinTerminal.QRDoge.old
{
    /*
	 * QRDoge v0
	 * 
	 * this is an example dogecoin service 
	 * 
	 */
    public class QRDogecoinService : IDogecoinService
    {
        private Game _game;

        public QRDogecoinService(Game game)
        {
            _game = game;
        }

        public void GetUTXOs(string address, string pin, Action<IEnumerable<UTXOInfo>> callback)
        {
            _game.Services.GetService<Router>().Route("displayqr", new DisplayQRPageSettings($"qrdoge:0-update:{address}", "Scan with QRDoge App (Update UTXOs)", true, true), true, (scanAcknowledged) =>
            {
                if (scanAcknowledged)
                {

                    _game.Services.GetService<Router>().Route("scanqr", "Scan QR Code display'd in App.", true, (qrString) =>
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
                                    VOut = int.Parse(lineParts[1]),
                                    Amount = decimal.Parse(lineParts[2])
                                });
                            }

                        }
                        catch (Exception) { }
                        finally
                        {
                            callback(utxoList);
                        }
                    });
                }
                else
                {
                    callback(new List<UTXOInfo>());
                }
            });


        }

        public void OnDeleteAddress(string address, string pin, Action callback)
        {
            //the corresponding QRDoge app will request that the node stop watching this address.
            //
            // qrdoge-0-delete:address
            //_game.Services.GetService<Router>().Route("displayqr", new DisplayQRPageSettings($"qrdoge:0-delete:{address}", "Scan with QRDoge App (Delete Address)", true), true, (scanAcknowledged) =>
            //{
            //	//nothing else to do
            //	callback();
            //});



            callback();
        }

        public void OnNewAddress(string address, string pin, Action<bool> callback)
        {

            //the corresponding QRDoge app will request that the node start watching this address.

            _game.Services.GetService<Router>().Route("displayqr", new DisplayQRPageSettings($"qrdoge:0-new:{address}", "Scan with QRDoge App (New Address)", true, true), true, (scanAcknowledged) =>
            {
                //nothing else to do
                callback(scanAcknowledged);
            });
        }

        public void OnSetup(Action<bool> callback)
        {
            callback(true);
        }

        public void SendTransaction(string transaction, string pin, Action<bool> callback)
        {
            //the corresponding QRDoge app will request that the node broadcast this transaction.
            _game.Services.GetService<Router>().Route("displayqr", new DisplayQRPageSettings($"qrdoge:0-send:{transaction}", "Scan with QRDoge App (Send Transaction)", true, true), true, (scanAcknowledged) =>
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
