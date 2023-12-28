# DogecoinTerminal

DogecoinTerminal is a multi-user **offline** dogecoin wallet made using *Monogame* and *[libdogecoin](https://github.com/dogecoinfoundation/libdogecoin)* for maximum portability.


![image](https://github.com/UsaRandom/DogecoinTerminal/assets/2897796/f0fb780f-2960-4a5c-8e46-4cf1ec3e675e)

![image](https://github.com/UsaRandom/DogecoinTerminal/assets/2897796/f2ff4988-407c-489e-a37b-02f91d50a2ed)



## Bridging to the Network

**DogecoinTerminal cannot send transactions on it's own**, it can only sign them.

To send transactions, you'll need a bridge. (an implimentation of `IDogecoinService`)

Currently there is one bridge, `DogecoinTerminal.Common.QRDoge.QRDogeService`.

It uses QR codes to pass messages back and forth with it's companion app:

### [QRDoge](https://github.com/UsaRandom/QRDoge)

A rudamentary android companion app that acts as a bridge between your [Dogecoin Core Node](https://github.com/dogecoin/dogecoin) and DogecoinTerminal.


![image](https://github.com/UsaRandom/DogecoinTerminal/assets/2897796/876af895-1897-46d0-be58-1e05c223e231)



## Creating your own Bridge

To create your own bridge, create a new project with a reference to `DogecoinTerminal.Common` and impliment the `IDogecoinService` interface.

```csharp
	//NOTE: not a final interface, only an example.
	public interface IDogecoinService
	{
		void OnSetup(Action<bool> callback);

		void OnNewAddress(string address, string pin, Action<bool> callback);

		void OnDeleteAddress(string address, string pin, Action callback);

		void UpdatePin(string address, string oldPin, string newPin, Action<bool> callback);

		void GetUTXOs(string address, string pin, Action<IEnumerable<UTXOInfo>> callback);

		void SendTransaction(string transaction, string pin, Action<bool> callback);
	}
```


The `DogecoinTerminal.Common.QRDoge.QRDogecoinService` class is a good reference on creating a service.




**Disclaimer**: I am illiterate and retarded. 
