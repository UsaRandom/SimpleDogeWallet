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
		private bool _devMode = false;
		private const string DEFAULT_CONTROLS = 
			"BackButton,TitleText,DevModeText,ToggleDevMode,BackupCodesButton,LanguageButton,DeleteButton";

		public SettingsPage(IPageOptions options, SimpleSPVNodeService spvService , IServiceProvider services, ITerminalSettings settings, Navigation navigation, Strings strings, LibDogecoinContext ctx) : base(options)
		{
			GetControl<CheckboxControl>("ToggleBackground").IsChecked = settings.GetBool("terminal-background", true);
			GetControl<CheckboxControl>("ToggleFullscreen").IsChecked = settings.GetBool("terminal-fullscreen", false);
			
			GetControl<CheckboxControl>("ToggleDevMode").IsChecked = (_devMode = settings.GetBool("terminal-devmode", false));

			foreach(var control in Controls)
			{
				if (!DEFAULT_CONTROLS.Contains(control.Name))
				{
					control.Enabled = _devMode;
				}
			}


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

				_devMode = checkbox.IsChecked;

                foreach (var control in Controls)
                {
                    if (!DEFAULT_CONTROLS.Contains(control.Name))
                    {
                        control.Enabled = _devMode;
                    }
                }

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
                SimpleDogeWallet.Instance.PendingTxTime = DateTime.MaxValue;


//                Messenger.Default.Send(new UpdateSendButtonMessage());

				navigation.Pop();
			});




			OnClick("DeleteButton", async _ =>
			{
				spvService.Pause();
				await navigation.PushAsync<LoadingPage>();

				var sendYesNo = await navigation.PromptAsync<YesNoPage>(("message", strings.GetString("terminal-settings-delete-confirm")));

				if (sendYesNo.Response != PromptResponse.YesConfirm)
				{
					navigation.Pop();
                    spvService.Resume();
                    return;
				}

				SimpleDogeWallet.ClearWallet();
				await navigation.PromptAsync<ShortMessagePage>(("message", strings.GetString("terminal-settings-delete-wallet-deleted")));
				await navigation.TryInsertBeforeAsync<SetupWalletPage, WalletPage>();
				await navigation.PromptAsync<LanguageSelectionPage>();
				await navigation.PopToPage<SetupWalletPage>();
			});

		}



	}
}
