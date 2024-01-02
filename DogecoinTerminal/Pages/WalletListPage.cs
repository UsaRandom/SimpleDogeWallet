using DogecoinTerminal.Common;
using DogecoinTerminal.Common.Pages;
using Microsoft.Xna.Framework;
using System;
using System.Text;

namespace DogecoinTerminal.Pages
{

	//I forgot we need this attribute to tell it which file to use for controls
	[PageDef("Pages/Xml/WalletList.xml")]
	internal class WalletListPage : Page
	{
		private const string SETTINGS_BUTTON_NAME = "SettingsButton";
		private const string LOCK_BUTTON_NAME = "LockButton";

		private const string SLOT_BUTTON_PREFIX = "WalletSlotButton_";

		public WalletListPage(IPageOptions options, Navigation navigation, Strings strings, ITerminalService terminalService) : base(options)
		{

			OnClick(LOCK_BUTTON_NAME, _ => {
				terminalService.Lock();
				navigation.Pop();
			});



			OnClick(SETTINGS_BUTTON_NAME, async _ => {
				var numPadResponse = await navigation.PromptAsync<NumPadPage>(("title", strings["terminal-enteroppin-title"]));

				if (numPadResponse.Response == PromptResponse.YesConfirm
				   && terminalService.ConfirmOperatorPin(numPadResponse.Value.ToString()))
				{

				}
			});


			for (var i = 0; i < TerminalService.MAX_SLOT_COUNT; i++)
			{
				//have to be careful with loops and callbacks
				var index = i;
				OnClick(SLOT_BUTTON_PREFIX + index, async _ => {

					var slot = terminalService.GetWalletSlot(index);

					if (slot.IsEmpty)
					{
						//ok, so now it's time to create a wallet and fill a wallet slot.

						//
						// Steps to create a slot:
						//
						/*
						 *  1. Create Pin.
						 *  2. Create Slot.
						 *  3. Notify IDogecoinService
						 *  4. Either delete or show backup codes, depending on IDogecoinService's response.
						 *  5. Open Wallet
						 */
						var enterPinResponse = await navigation.PromptAsync<NumPadPage>(("title", strings["terminal-walletlist-newwallet-enterpin"]));

						if (enterPinResponse.Response != PromptResponse.YesConfirm)
						{
							return;
						}


						var confirmPinResponse = await navigation.PromptAsync<NumPadPage>(("title", strings["terminal-walletlist-newwallet-confirmpin"]));

						var enteredPin = (string)enterPinResponse.Value;
						var confirmPin = (string)confirmPinResponse.Value;

						//confirm they said yes, and the responses match!
						if (confirmPinResponse.Response != PromptResponse.YesConfirm ||
							enteredPin != confirmPin)

						{
							return;
						}


						//alright, so i know what is going on with the exception.
						//this is part of a refactor, so i'm going to hack it up real good.

						var newSlotPin = (string)confirmPinResponse.Value;

						slot.Init(newSlotPin);

						//ok we've created our slot!

						//TODO: now we need to confirm with an IDogecoinService (which isn't implimented currently, so lets skip)


						//then, we need to show the user their backup codes.

						var mnemonic = slot.GetMnemonic();

						await navigation.PushAsync<BackupCodePage>(("mnemonic", mnemonic));



					}
					else
					{
						//Note: Might be nice to have some kind of authentication service for stuff like this.

						var numPadResponse = await navigation.PromptAsync<NumPadPage>(("title", strings["terminal-enterslotpin-title"]));

						if (numPadResponse.Response == PromptResponse.YesConfirm &&
							slot.Unlock(numPadResponse.Value.ToString()))
						{

							//we have a wallet page, lets show it!

							await navigation.PushAsync<WalletPage>(("slot", slot));
						}
					}

				});
			}
		}




		public override void Update(GameTime gameTime, IServiceProvider services)
		{
			//we can update wallet slot buttons, or we can create a new control and use it for wallet slots.


			var terminalService = services.GetService<ITerminalService>();

			for(var i = 0; i < TerminalService.MAX_SLOT_COUNT; i++)
			{
				var slot = terminalService.GetWalletSlot(i);
				var slotButton = GetControl<ButtonControl>(SLOT_BUTTON_PREFIX + i);


				if(slot.IsEmpty)
				{
					//tell slots to use the 'empty slot' string (not langugae specific)
					slotButton.StringDef = "terminal-walletlist-emptyslot";
					slotButton.BackgroundColor = TerminalColor.LightGrey;
				}
				else
				{
					//by setting StringDef to empty, we say not to use internationalization, use the provided string.
					slotButton.StringDef = string.Empty;
					slotButton.BackgroundColor = TerminalColor.DarkGrey;
					slotButton.Text = GetShortAddress(slot.Address);
				}
			}



			base.Update(gameTime, services);
		}

		private string GetShortAddress(string address)
		{
			var builder = new StringBuilder();
			builder.Append(address.Substring(0, 4));
			builder.Append("..");
			builder.Append(address.Substring(address.Length - 3, 3));
			return builder.ToString();
		}

	}

}
