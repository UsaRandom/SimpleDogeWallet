using SimpleDogeWallet.Common;
using SimpleDogeWallet.Common.Pages;
using Lib.Dogecoin;
using OpenCvSharp.Dnn;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SimpleDogeWallet.Pages
{
    [PageDef("Pages/Xml/SettingsPage.xml")]
	internal class SettingsPage : Page
	{
		public SettingsPage(IPageOptions options, SimpleSPVNodeService spvService , IServiceProvider services, ITerminalSettings settings, Navigation navigation, Strings strings, LibDogecoinContext ctx) : base(options)
		{
			GetControl<CheckboxControl>("ToggleBackground").IsChecked = settings.GetBool("terminal-background", true);
			GetControl<CheckboxControl>("ToggleFullscreen").IsChecked = settings.GetBool("terminal-fullscreen", false);
			GetControl<CheckboxControl>("ToggleDevMode").IsChecked = settings.GetBool("terminal-devmode", false);


			GetControl<ButtonControl>("SetDustLimitButton").Text = settings.GetDecimal("dust-limit").ToString();
			GetControl<ButtonControl>("SetFeeCoeffButton").Text = settings.GetDecimal("fee-coeff").ToString();

			OnClick("BackButton", async _ =>
			{
				navigation.Pop();
			});


			OnClick("ToggleBackground", async _ =>
			{
				var checkbox = GetControl<CheckboxControl>("ToggleBackground");
				checkbox.IsChecked = !checkbox.IsChecked;
				settings.Set("terminal-background", checkbox.IsChecked);
			});

			OnClick("ToggleFullscreen", async _ =>
			{
				var checkbox = GetControl<CheckboxControl>("ToggleFullscreen");
				checkbox.IsChecked = !checkbox.IsChecked;
				settings.Set("terminal-fullscreen", checkbox.IsChecked);
			});

			OnClick("ToggleDevMode", async _ =>
			{
				var checkbox = GetControl<CheckboxControl>("ToggleDevMode");
				checkbox.IsChecked = !checkbox.IsChecked;
				settings.Set("terminal-devmode", checkbox.IsChecked);
			});


			OnClick("SetDustLimitButton", async _ =>
			{
				var updateResult = await navigation.PromptAsync<NumPadPage>(("title", strings["terminal-settings-dustlimit"]),
																	("value-mode", true),
																	("start-value", settings.GetDecimal("dust-limit").ToString()));

				if(updateResult.Response == PromptResponse.YesConfirm)
				{
					var newDustLimit = decimal.Parse(updateResult.Value.ToString());

					if(newDustLimit > 0)
					{
						settings.Set("dust-limit", newDustLimit);
						GetControl<ButtonControl>("SetDustLimitButton").Text = newDustLimit.ToString();
					}
				}
			});

			OnClick("SetFeeCoeffButton", async _ =>
			{
				var updateResult = await navigation.PromptAsync<NumPadPage>(("title", strings["terminal-settings-feecoeff"]),
																	("value-mode", true),
																	("start-value", settings.GetDecimal("fee-coeff").ToString()));

				if (updateResult.Response == PromptResponse.YesConfirm)
				{
					var newFeeCoeff = decimal.Parse(updateResult.Value.ToString());

					if (newFeeCoeff > 0)
					{
						settings.Set("fee-coeff", newFeeCoeff);
						Messenger.Default.Send(new UpdateSPVTextMessage());
						GetControl<ButtonControl>("SetFeeCoeffButton").Text = newFeeCoeff.ToString();
					}
				}
			});


			OnClick("UpdatePinButton", async _ =>
			{
				await navigation.PushAsync<LoadingPage>();

				var newPin = string.Empty;
				var confirmPin = string.Empty;
				
				var oldPinResponse = await navigation.PromptAsync<NumPadPage>(
					("title", strings.GetString("terminal-settings-oldpin")),
					("regex", ".{" + SimpleDogeWallet.MIN_PIN_LENGTH + ",16}"));


				if(oldPinResponse.Response == PromptResponse.NoCancelBack ||
				   !SimpleDogeWallet.TryOpen((string)oldPinResponse.Value))
				{
					navigation.Pop();
					return;
				}

				var oldPin = (string)oldPinResponse.Value;

				while (newPin != confirmPin || newPin.Length < SimpleDogeWallet.MIN_PIN_LENGTH)
				{
					var enterPin = await navigation.PromptAsync<NumPadPage>(
						("title", strings.GetString("terminal-setup-setpin")),
						("hint", strings.GetString("terminal-setup-setpin-hint")),
						("regex", ".{"+ SimpleDogeWallet.MIN_PIN_LENGTH + ",16}"));

					if (enterPin.Response == PromptResponse.YesConfirm)
					{
						newPin = (string)enterPin.Value;
					}
					else
					{
						navigation.Pop();
						return;
					}

					var confirm = await navigation.PromptAsync<NumPadPage>(
						("title", strings.GetString("terminal-setup-confirmpin")),
						("hint", strings.GetString("terminal-setup-confirmpin-hint")),
						("regex", ".{"+ SimpleDogeWallet.MIN_PIN_LENGTH + ",16}"));

					if (confirm.Response == PromptResponse.YesConfirm)
					{
						confirmPin = (string)confirm.Value;
					}
				}
				await navigation.PromptAsync<ShortMessagePage>(("message", strings.GetString("terminal-settings-pin-updated")));

				SimpleDogeWallet.UpdatePin(oldPin, confirmPin);

				navigation.Pop();
			});


			OnClick("LanguageButton", async _ =>
			{
				await navigation.PromptAsync<LanguageSelectionPage>();
			});

			OnClick("BackupCodesButton", async _ =>
			{
				await navigation.PushAsync<LoadingPage>();

				//var oldPinResponse = await navigation.PromptAsync<NumPadPage>(
				//	("title", strings.GetString("terminal-enteroppin-title")),
				//	("regex", ".{" + SimpleDogeWallet.MIN_PIN_LENGTH + ",16}"));


				//if (oldPinResponse.Response == PromptResponse.YesConfirm &&
				//   SimpleDogeWallet.TryOpen((string)oldPinResponse.Value))
				//{

					var mnemonic = SimpleDogeWallet.Instance.GetMnemonic();

					if(!string.IsNullOrWhiteSpace(mnemonic))
					{
						await navigation.PromptAsync<BackupCodePage>(("title", strings.GetString("terminal-backupcodes-title")),
																	 ("editmode", false),
																	 ("mnemonic", mnemonic.Trim()));
					}

				//}

				navigation.Pop();
			});


			OnClick("SPVButton", async _ =>
			{
				await navigation.PushAsync<SPVNodePage>(("wallet", options.GetOption<SimpleDogeWallet>("wallet")));
			});

			OnClick("ClearPendingButton", async _ =>
			{
				await navigation.PushAsync<LoadingPage>();

				var sendYesNo = await navigation.PromptAsync<YesNoPage>(("message", strings.GetString("terminal-settings-clearpending-confirm")));

				if (sendYesNo.Response != PromptResponse.YesConfirm)
				{
					navigation.Pop();
					return;
				}

				SimpleDogeWallet.Instance.PendingAmount = 0;
				SimpleDogeWallet.Instance.PendingTxHash = string.Empty;


				Messenger.Default.Send(new UpdateSendButtonMessage());

				navigation.Pop();
			});




			OnClick("DeleteButton", async _ =>
			{
				spvService.Stop();
				await navigation.PushAsync<LoadingPage>();

				var sendYesNo = await navigation.PromptAsync<YesNoPage>(("message", strings.GetString("terminal-settings-delete-confirm")));

				if (sendYesNo.Response != PromptResponse.YesConfirm)
				{
					navigation.Pop();
					return;
				}

				var oldPinResponse = await navigation.PromptAsync<NumPadPage>(
					("title", strings.GetString("terminal-settings-delete-confirm-pin")),
					("regex", ".{" + SimpleDogeWallet.MIN_PIN_LENGTH + ",16}"));


				if (oldPinResponse.Response == PromptResponse.YesConfirm &&
				   SimpleDogeWallet.TryOpen((string)oldPinResponse.Value))
				{
					SimpleDogeWallet.ClearWallet();
					await navigation.PromptAsync<ShortMessagePage>(("message", strings.GetString("terminal-settings-delete-wallet-deleted")));
					await navigation.TryInsertBeforeAsync<SetupWalletPage, WalletPage>();
					await navigation.PromptAsync<LanguageSelectionPage>();
					await navigation.PopToPage<SetupWalletPage>();
					return;
				}

				spvService.Start();
				navigation.Pop();
			});

		}



	}
}
