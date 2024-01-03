using Lib.Dogecoin;
using System;
using System.IO;
using System.Net;
using System.Text;

namespace DogecoinTerminal
{

	internal class WalletSlot : IWalletSlot
    {
        //address file
        //key file

        private string SlotAddressFile
        {
            get
            {
                return $"slot_{SlotNumber}_address.dtf";
            }
        }

        private string KeyFile
        {
            get
            {
                return $"slot_{SlotNumber}_key.dtf";
            }
        }

        private IServiceProvider _services;
        internal string _opPin;
        internal string _slotPin;

        private bool _isEmpty;
        private string _address;
        private string _shortAddress;

        public WalletSlot(IServiceProvider services, string opPin, int slotNumber)
        {
            _services = services;
            _opPin = opPin;
            _slotPin = string.Empty;
            SlotNumber = slotNumber;

            _isEmpty = !File.Exists(SlotAddressFile);
            UTXOStore = new UTXOStore(this);
        }

        public bool IsEmpty
        {
            get
            {
                return _isEmpty;
            }
        }

        public bool IsUnlocked { get; private set; }

        public int SlotNumber { get; private set; }

        public string Address
        {
            get
            {
                if (!IsEmpty && _opPin != string.Empty && File.Exists(SlotAddressFile) && string.IsNullOrEmpty(_address))
                {
					_address = Crypto.Decrypt(File.ReadAllText(SlotAddressFile), _opPin);
                }

                return _address;
            }
        }

		public string ShortAddress
        {
            get
            {
                if(string.IsNullOrEmpty(_shortAddress) && !string.IsNullOrEmpty(Address))
				{
					var builder = new StringBuilder();
					builder.Append(_address.Substring(0, 4));
					builder.Append("..");
					builder.Append(_address.Substring(_address.Length - 3, 3));
					_shortAddress = builder.ToString();
				}

                return _shortAddress;
            }
        }

		public string SlotPin
        {
            get
            {
                return _slotPin;
            }
        }

        public IUTXOStore UTXOStore
        {
            get; private set;
        }


		public void Init(string slotPin)
        {
            ClearSlot();

            _slotPin = slotPin;

            //create mnemonic
            using (var ctx = LibDogecoinContext.CreateContext())
            {
                var newMnemonic = ctx.GenerateRandomEnglishMnemonic(LibDogecoinContext.ENTROPY_SIZE_256);

                var masterKeys = ctx.GenerateHDMasterPubKeypairFromMnemonic(newMnemonic);

                var pubKey = ctx.GetDerivedHDAddressByPath(masterKeys.privateKey, Crypto.HDPATH, false);


                if (ctx.VerifyHDMasterPubKeyPair(masterKeys.privateKey, masterKeys.publicKey))
                {
                    File.WriteAllText(KeyFile, Crypto.Encrypt(Crypto.Encrypt(newMnemonic, _slotPin), _opPin));
                    File.WriteAllText(SlotAddressFile, Crypto.Encrypt(pubKey, _opPin));
                }
                else
                {
                    throw new Exception("Could not generate keys, sorry :(");
                }
            }

            _isEmpty = false;

			//new wallet slots are initialized unlocked.
			Unlock(slotPin);
        }


        public string CalculateBalance()
        {
            var sum = 0M;

            foreach (var utxo in UTXOStore.UTXOs)
            {
                sum += utxo.Amount;
            }

            return sum.ToString("#,##0.000");
        }


        public bool Unlock(string slotPin)
        {
            if (IsEmpty)
            {
                return false;
            }



            if (!UTXOStore.Unlock(_opPin, slotPin))
            {
                //UTXO file exists, but error parsing it.
                return false;
            }



            if (!File.Exists(KeyFile))
            {
                //key file is mandatory
                return false;
            }


            try
            {
                var mnemonic = Crypto.Decrypt(Crypto.Decrypt(File.ReadAllText(KeyFile), _opPin), slotPin);

                if (mnemonic.Split(' ').Length == 24)
                {
                    _slotPin = slotPin;
                    IsUnlocked = true;
                    return true;
                }
            }
            catch
            {
                //error parsing key file, most likely incorrect pin
                return false;
            }

            return false;
        }

        public void Lock()
        {
            IsUnlocked = false;
            _slotPin = string.Empty;
        }

        public void UpdateOperatorPin(string newOperatorPin)
        {
            if (IsEmpty)
            {
                _opPin = newOperatorPin;
                return;
            }

            //update address file
            var address = Address;

            File.WriteAllText(SlotAddressFile, Crypto.Encrypt(address, newOperatorPin));

            UTXOStore.UpdateOperatorPin(newOperatorPin);

            //update key file
            var keyContent = Crypto.Decrypt(File.ReadAllText(KeyFile), _opPin);
            File.WriteAllText(KeyFile, Crypto.Encrypt(keyContent, newOperatorPin));


            _opPin = newOperatorPin;
        }


        public void UpdateSlotPin(string newSlotPin)
        {
            if (!IsUnlocked)
            {
                return;
            }


            UTXOStore.UpdateSlotPin(newSlotPin);

            //update key file
            var keyContent = Crypto.Decrypt(Crypto.Decrypt(File.ReadAllText(KeyFile), _opPin), _slotPin);
            File.WriteAllText(KeyFile, Crypto.Encrypt(Crypto.Encrypt(keyContent, newSlotPin), _opPin));

            _slotPin = newSlotPin;
        }


        public void ClearSlot()
        {
            UTXOStore.OnWalletSlotDelete();

            File.Delete(SlotAddressFile);
            File.Delete(KeyFile);


            _slotPin = string.Empty;
            _address = string.Empty;
            _isEmpty = true;
        }

        public IDogecoinTransaction CreateTransaction(string receipient, decimal amount)
        {
            var transaction = new DogecoinTransaction(_services, this);

            if (transaction.Send(receipient, amount) && transaction.Sign())
            {
                return transaction;
            }
            else
            {
                transaction.Dispose();
            }

            return null;
        }


        public string GetMnemonic()
        {
            if (IsUnlocked)
            {
                return Crypto.Decrypt(Crypto.Decrypt(File.ReadAllText(KeyFile), _opPin), _slotPin);
            }
            return string.Empty;
        }

    }
}
