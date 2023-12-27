using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DogecoinTerminal.Common
{
	//NOTE: This is not a final interface.
	public interface IDogecoinService
	{
		void OnSetup(Action<bool> callback);

		void OnReset(Action<bool> callback);

		void OnNewAddress(string address, string pin, Action<bool> callback);

		void OnDeleteAddress(string address, string pin, Action callback);

		void UpdatePin(string address, string oldPin, string newPin, Action<bool> callback);

		void GetUTXOs(string address, string pin, Action<IEnumerable<UTXOInfo>> callback);

		void SendTransaction(string transaction, string pin, Action<bool> callback);
	}
}
