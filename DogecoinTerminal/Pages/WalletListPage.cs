using DogecoinTerminal.Common;
using DogecoinTerminal.Common.Pages;
using DogecoinTerminal.old;
using Microsoft.Xna.Framework;
using System;
using System.Text;

namespace DogecoinTerminal.Pages
{
    /*
	 * Notes: 
	 * 
	 * So, we have to change a few things for TPM.
	 * The wallet functionality is proven, but not flushed out.
	 * We can create valid signed transactions if we are given UTXOs. 
	 * 
	 * How we get UTXOs, store keys, broadcast trasnactions,
	 * 
	 * 
	 */


    [PageDef("Pages/Xml/WalletListPage.xml")]
	internal class WalletListPage : Page
	{
		private const string SETTINGS_BUTTON_NAME = "SettingsButton";
		private const string LOCK_BUTTON_NAME = "LockButton";

		private const string SLOT_BUTTON_PREFIX = "WalletSlotButton_";

		public WalletListPage(IPageOptions options, Navigation navigation, Strings strings, ITerminalService terminalService) : base(options)
		{

			OnClick(LOCK_BUTTON_NAME, async _ => {
				terminalService.Lock();
				navigation.Pop();
			});



			OnClick(SETTINGS_BUTTON_NAME, async _ => {

				await navigation.PushAsync<BlankPage>();

				var numPadResponse = await navigation.PromptAsync<NumPadPage>(("title", strings["terminal-enteroppin-title"]));

				if (numPadResponse.Response == PromptResponse.YesConfirm
				   && terminalService.ConfirmOperatorPin(numPadResponse.Value.ToString()))
				{

					//Let's create a settings page! (puts it before our blank page, which we remove)
					await navigation.TryInsertBeforeAsync<SettingsPage, BlankPage>();
				}

				navigation.Pop();
			});


			for (var i = 0; i < TerminalService.MAX_SLOT_COUNT; i++)
			{
				//have to be careful with loops and callbacks
				var index = i;
				OnClick(SLOT_BUTTON_PREFIX + index, async _ => {

					var slot = terminalService.GetWalletSlot(index);

					if (slot.IsEmpty)
					{
						/*
						 * Notes:
						 * 
						 * I wouldn't mind pulling this out into some sort of wallet slot factory. 
						 * It would be cool to support multiple slots.
						 * 
						 */

						//push loading page to nav stack, so when pages switch, the loading screen is rendered between instead of wallet list page
						await navigation.PushAsync<BlankPage>();

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
						//var enterPinResponse = await navigation.PromptAsync<NumPadPage>(("title", strings["terminal-walletlist-newwallet-enterpin"]));

						//var enteredPin = (string)enterPinResponse.Value;

						//if (enterPinResponse.Response != PromptResponse.YesConfirm
						//	|| string.IsNullOrEmpty(enteredPin))
						//{
						//	return;
						//}


						//var confirmPinResponse = await navigation.PromptAsync<NumPadPage>(("title", strings["terminal-walletlist-newwallet-confirmpin"]));

						//var confirmPin = (string)confirmPinResponse.Value;

						////confirm they said yes, and the responses match!
						//if (confirmPinResponse.Response != PromptResponse.YesConfirm
						//	|| enteredPin != confirmPin)

						//{
						//	//remove loading page, return to wallet list page.
						//	navigation.Pop();
						//	return;
						//}


						//alright, so i know what is going on with the exception.
						//this is part of a refactor, so i'm going to hack it up real good.

						//		var newSlotPin = (string)confirmPinResponse.Value;


						var response = await navigation.PromptAsync<ShortMessagePage>(("message", $"Press 'Ok' to create a new Wallet here ({index + 1})"));

						if(response.Response == PromptResponse.YesConfirm && slot.Init("420.69"))
						{
							await navigation.TryInsertBeforeAsync<WalletPage, BlankPage>(("slot", slot));
						}

						navigation.Pop();
						//ok we've created our slot!

						//TODO: now we need to confirm with an IDogecoinService (which isn't implimented currently, so lets skip)




					}
					else
					{
						//Note: Might be nice to have some kind of authentication service for stuff like this.

					//	await navigation.PushAsync<BlankPage>();

						//var numPadResponse = await navigation.PromptAsync<NumPadPage>(("title", strings["terminal-enterslotpin-title"]));

						//if (numPadResponse.Response == PromptResponse.YesConfirm &&
						//	slot.Unlock(numPadResponse.Value.ToString()))
						//{

						//	//we have a wallet page, lets show it!
						//	await navigation.TryInsertBeforeAsync<WalletPage, BlankPage>(("slot", slot));

						//}

						await navigation.PushAsync<BlankPage>();

						slot.Unlock("420.69");

						await navigation.TryInsertBeforeAsync<WalletPage, BlankPage>(("slot", slot));

						navigation.Pop();

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
					slotButton.Text = slot.ShortAddress;
				}
			}



			base.Update(gameTime, services);
		}


	}

}
