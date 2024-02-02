using DogecoinTerminal.Common;
using DogecoinTerminal.Common.Pages;
using Lib.Dogecoin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DogecoinTerminal.Pages
{
    [PageDef("Pages/Xml/WalletPage.xml")]
    internal class WalletPage : Page
	{
		private const string SETTINGS_BUTTON_NAME = "SettingsButton";
		private const string LOCK_BUTTON_NAME = "LockButton";

		public WalletPage(IPageOptions options, Navigation navigation, Strings strings) : base(options)
        {
            var addressTextControl = GetControl<TextControl>("AddressText");
            addressTextControl.Text = "";// slot.Address;


            var balanceTextControl = GetControl<TextControl>("BalanceText");
            balanceTextControl.Text = "Đ";// slot.CalculateBalance();



			OnClick(SETTINGS_BUTTON_NAME, async _ => {

				await navigation.PushAsync<BlankPage>();

				var numPadResponse = await navigation.PromptAsync<NumPadPage>(("title", strings["terminal-enteroppin-title"]));

				if (numPadResponse.Response == PromptResponse.YesConfirm)
				 //  && terminalService.ConfirmOperatorPin(numPadResponse.Value.ToString()))
				{

					//Let's create a settings page! (puts it before our blank page, which we remove)
					await navigation.TryInsertBeforeAsync<SettingsPage, BlankPage>();
				}

				navigation.Pop();
			});


			OnClick("BackButton", async _ =>
            {
                navigation.Pop();
            });

            OnClick("ReceiveButton", async _ =>
            {
				await navigation.PromptAsync<DisplayQRPage>(
                        ("message", ""),//slot.Address),
                        ("qr", "")//slot.Address)
                       );

			});


            OnClick("RemoveButton", async _ =>
            {
                //ok, we want to remove this wallet.
                //first put in a loading page for flicker
                await navigation.PushAsync<BlankPage>();

                var numPadResult = await navigation.PromptAsync<NumPadPage>();

                //We need to get the users pin confirmed on delete.
                if (numPadResult.Response == PromptResponse.YesConfirm)
               //     && slot.SlotPin == (string)numPadResult.Value)
                {
                    //we've confirmed pin
                    //now lets delete the slot.
            //        slot.ClearSlot();
					//remove loading screen.
					navigation.Pop();
                    //remove wallet page
                    navigation.Pop();
				}
                else
                {
                    //just remove loading screen
					navigation.Pop();
				}


			});

            OnClick("ShowSeedButton", async _ =>
            {
                await navigation.PushAsync<BlankPage>();

                var acknowledge = await navigation.PromptAsync<ShortMessagePage>(("message", "Don't share seed phrase, have pen & paper ready!"));

                if(acknowledge.Response == PromptResponse.YesConfirm)
                {
                    string mnemonic = string.Empty;

                    using(var ctx = LibDogecoinContext.CreateContext())
                    {
                        mnemonic = "";//mnemonicProvider.GetMnemonic(ctx, slot.SlotNumber);
					}

                    if(!string.IsNullOrEmpty(mnemonic))
					{
						await navigation.TryInsertBeforeAsync<BackupCodePage, BlankPage>(("mnemonic", mnemonic));
					}
                }

                navigation.Pop();

            });



   //         OnClick("UpdatePinButton", async _ =>
   //         {
   //             //User wants to update pin

   //             await navigation.PushAsync<BlankPage>();

   //             var numPadResult = await navigation.PromptAsync<NumPadPage>(("title", strings["terminal-wallet-updatepin-newpin"]));

			//	var enteredPin = (string)numPadResult.Value;

			//	if (numPadResult.Response != PromptResponse.YesConfirm
   //                 || string.IsNullOrEmpty(enteredPin))
			//	{
			//		//just remove loading screen
			//		navigation.Pop();
   //                 return;
			//	}

			//	numPadResult = await navigation.PromptAsync<NumPadPage>(("title", strings["terminal-wallet-updatepin-confirmpin"]));

			//	if (numPadResult.Response != PromptResponse.YesConfirm
   //                 && enteredPin != (string)numPadResult.Value)
			//	{

   //                 //the new pin was not updated, so lets notify user!

   //                 await navigation.PromptAsync<ShortMessagePage>(("message", "ERROR: Pin was NOT updated!"));

			//		navigation.Pop();
			//	}
   //             else
   //             {
   //                 //update the pin

   //                 slot.UpdateSlotPin(enteredPin);

   //                 await navigation.PromptAsync<ShortMessagePage>(("message", "SUCCESS: Pin was updated!"));

			//		navigation.Pop();

			//	}
			//});

		}
    }
}
