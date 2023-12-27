using DogecoinTerminal.Common;
using Lib.Dogecoin;
using Microsoft.Xna.Framework;
using OpenCvSharp;
using OpenCvSharp.ImgHash;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Numerics;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


namespace DogecoinTerminal
{
	internal class TerminalService : ITerminalService
	{
		private const int MAX_SLOT_COUNT = 6;
		private const string OP_PIN_VERIFY_FILE = "op_pin_verify.dtf";
		private const string OP_PIN_VERIFY_STATEMENT = "much verify, very wow";


		private Game _game;
		private string _opPin;
		private IWalletSlot[] _slots;

		public bool IsUnlocked { get; private set; }

		public TerminalService(Game game)
		{
			_slots = new IWalletSlot[MAX_SLOT_COUNT];
			_game = game;
			_opPin = string.Empty;
			IsUnlocked = false;
		}

		public void ClearSlot(int slot)
		{
			if(IsUnlocked)
			{
				GetWalletSlot(slot).ClearSlot();
			}
		}

		public IWalletSlot GetWalletSlot(int slot)
		{
			if (_slots[slot] == null)
			{
				_slots[slot] = new WalletSlot(_game, _opPin, slot);
			}

			return _slots[slot];
		}

		public void Lock()
		{
			_opPin = string.Empty;
			IsUnlocked = false;
		}

		public bool Unlock(string operatorPin)
		{
			if(OpPinIsSet() && operatorPin != string.Empty)
			{
				bool verified = false;
				try
				{
					verified = Crypto.Decrypt(File.ReadAllText(OP_PIN_VERIFY_FILE), operatorPin) == OP_PIN_VERIFY_STATEMENT;
				}
				catch { }

				if(verified)
				{
					_opPin = operatorPin;
					IsUnlocked = true;
					return true;
				}
			}

			//Op pin not set, we unlock for operator setup.
			if(!OpPinIsSet() && operatorPin == string.Empty)
			{
				_opPin = operatorPin;
				IsUnlocked = true;
				return true;
			}

			return false;
		}

		public bool UpdateOperatorPin(string newOperatorPin)
		{
			if(!IsUnlocked)
			{
				return false;
			}

			if(string.IsNullOrEmpty(newOperatorPin))
			{
				return false;
			}
			
			for(var i = 0; i < MAX_SLOT_COUNT; i++)
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
