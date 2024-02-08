using DogecoinTerminal.Common;
using DogecoinTerminal.Common.Pages;
using Lib.Dogecoin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DogecoinTerminal.Pages
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
			GetControl<ButtonControl>("SetFeePerUTXOButton").Text = settings.GetDecimal("fee-per-utxo").ToString();

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

			OnClick("SetFeePerUTXOButton", async _ =>
			{
				var updateResult = await navigation.PromptAsync<NumPadPage>(("title", strings["terminal-settings-feeperutxo"]),
																	("value-mode", true),
																	("start-value", settings.GetDecimal("fee-per-utxo").ToString()));

				if (updateResult.Response == PromptResponse.YesConfirm)
				{
					var newFeePerUTXO = decimal.Parse(updateResult.Value.ToString());

					if (newFeePerUTXO > 0)
					{
						settings.Set("fee-per-utxo", newFeePerUTXO);
						GetControl<ButtonControl>("SetFeePerUTXOButton").Text = newFeePerUTXO.ToString();
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
				   !SimpleDogeWallet.TryOpen((string)oldPinResponse.Value, services, out SimpleDogeWallet _))
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

				var oldPinResponse = await navigation.PromptAsync<NumPadPage>(
					("title", strings.GetString("terminal-enteroppin-title")),
					("regex", ".{" + SimpleDogeWallet.MIN_PIN_LENGTH + ",16}"));


				if (oldPinResponse.Response == PromptResponse.YesConfirm &&
				   SimpleDogeWallet.TryOpen((string)oldPinResponse.Value, services, out SimpleDogeWallet wallet))
				{

					await navigation.PromptAsync<BackupCodePage>(("title", strings.GetString("terminal-backupcodes-title")),
												                 ("editmode", false),
																 ("mnemonic", wallet.GetMnemonic()));
				}

				navigation.Pop();
			});


			OnClick("SPVButton", async _ =>
			{
				spvService.PrintDebug();
			});

			OnClick("DeleteButton", async _ =>
			{
				await navigation.PushAsync<LoadingPage>();

				var oldPinResponse = await navigation.PromptAsync<NumPadPage>(
					("title", strings.GetString("terminal-enteroppin-title")),
					("regex", ".{" + SimpleDogeWallet.MIN_PIN_LENGTH + ",16}"));


				if (oldPinResponse.Response == PromptResponse.YesConfirm &&
				   SimpleDogeWallet.TryOpen((string)oldPinResponse.Value, services, out SimpleDogeWallet wallet))
				{
					SimpleDogeWallet.ClearWallet();
					await navigation.TryInsertBeforeAsync<StartPage, WalletPage>();
					await navigation.PopToPage<StartPage>();
					return;
				}

				navigation.Pop();
			});

		}



	}
}
