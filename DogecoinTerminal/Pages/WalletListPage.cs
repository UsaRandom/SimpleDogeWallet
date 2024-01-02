using DogecoinTerminal.Common;
using DogecoinTerminal.Common.Pages;

namespace DogecoinTerminal.Pages
{
	internal class WalletListPage : Page
	{
		private const string SETTINGS_BUTTON_NAME = "SettingsButton";
		private const string LOCK_BUTTON_NAME = "LockButton";

		public WalletListPage(IPageOptions options, Navigation navigation, Strings strings, ITerminalService terminalService) : base(options)
		{

			OnClick(LOCK_BUTTON_NAME, _ => {
				terminalService.Lock();
				navigation.Pop();
			});

			OnClick(SETTINGS_BUTTON_NAME, async _ => {
				var numPadResponse = await navigation.PromptAsync<NumPadPage>(("title", strings["terminal-enteroppin-title"]));

				if (terminalService.ConfirmOperatorPin(numPadResponse.Value.ToString()) &&
				   numPadResponse.Response == PromptResponse.YesConfirm)
				{
				//	await navigation.PushAsync<SettingsPage>();
				}
			});


			for (var i = 0; i < 6; i++)
			{
				OnClick($"WalletSlotButton_{i}", async _ => {

					var slot = terminalService.GetWalletSlot(i);

					if (slot.IsEmpty)
					{
			//			await navigation.PushAsync<FillWalletSlotPage>(("slot", slot));
					}
					else
					{
						var numPadResponse = await navigation.PromptAsync<NumPadPage>(("title", strings["terminal-enterslotpin-title"]));

						if (numPadResponse.Response == PromptResponse.YesConfirm &&
							slot.Unlock(numPadResponse.Value.ToString()))
						{
					//		await navigation.PushAsync<WalletPage>(("slot", slot));
						}
					}

				});
			}
		}
	}

}
