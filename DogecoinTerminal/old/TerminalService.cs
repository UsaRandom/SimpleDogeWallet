using DogecoinTerminal.Common;
using System;
using System.IO;


namespace DogecoinTerminal.old
{
    internal class TerminalService : ITerminalService
    {
        public const int MAX_SLOT_COUNT = 6;
        private const string OP_PIN_VERIFY_FILE = "op_pin_verify.dtf";
        private const string OP_PIN_VERIFY_STATEMENT = "much verify, very wow";


        private IServiceProvider _services;
        private string _opPin;
        private IWalletSlot[] _slots;

        private ITerminalSettings _settings;

        public bool IsUnlocked { get; private set; }

        public TerminalService(IServiceProvider services)
        {
            _slots = new IWalletSlot[MAX_SLOT_COUNT];
            _services = services;
            _opPin = string.Empty;
            IsUnlocked = false;
        }

        public void ClearSlot(int slot)
        {
            if (IsUnlocked)
            {
                GetWalletSlot(slot).ClearSlot();
            }
        }

        public IWalletSlot GetWalletSlot(int slot)
        {
            if (IsUnlocked)
            {
                if (_slots[slot] == null)
                {
                    _slots[slot] = new WalletSlot(_services, _opPin, slot);
                }

                return _slots[slot];
            }

            return null;
        }

        public void Lock()
        {
            _opPin = string.Empty;
            IsUnlocked = false;
        }


        public bool ConfirmOperatorPin(string pin)
        {
            if (OpPinIsSet() && pin != string.Empty)
            {
                try
                {
                    return Crypto.Decrypt(File.ReadAllText(OP_PIN_VERIFY_FILE), pin) == OP_PIN_VERIFY_STATEMENT;
                }
                catch { }

                return false;
            }

            if (!OpPinIsSet() && pin == string.Empty)
            {

                return true;
            }

            return false;
        }


        public bool Unlock(string operatorPin)
        {
            if (OpPinIsSet() && operatorPin != string.Empty)
            {
                bool verified = false;
                try
                {
                    verified = Crypto.Decrypt(File.ReadAllText(OP_PIN_VERIFY_FILE), operatorPin) == OP_PIN_VERIFY_STATEMENT;
                }
                catch { }

                if (verified)
                {
                    _opPin = operatorPin;
                    IsUnlocked = true;
                    return true;
                }
            }

            //Op pin not set, we unlock for operator setup.
            if (!OpPinIsSet() && operatorPin == string.Empty)
            {
                _opPin = operatorPin;
                IsUnlocked = true;

                return true;
            }

            return false;
        }

        public bool UpdateOperatorPin(string newOperatorPin)
        {
            if (!IsUnlocked)
            {
                return false;
            }

            if (string.IsNullOrEmpty(newOperatorPin))
            {
                return false;
            }

            for (var i = 0; i < MAX_SLOT_COUNT; i++)
            {
                var walletSlot = GetWalletSlot(i);

                walletSlot.UpdateOperatorPin(newOperatorPin);
            }

            File.Delete(OP_PIN_VERIFY_FILE);
            File.WriteAllText(OP_PIN_VERIFY_FILE, Crypto.Encrypt(OP_PIN_VERIFY_STATEMENT, newOperatorPin));

            _opPin = newOperatorPin;

            return true;
        }

        private bool OpPinIsSet()
        {
            return File.Exists(OP_PIN_VERIFY_FILE);
        }



    }


}
