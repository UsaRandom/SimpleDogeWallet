using DogecoinTerminal.Common.Pages;
using DogecoinTerminal.Common;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lib.Dogecoin;
using System.IO;
using System.Transactions;
using System.Xml.Linq;

namespace DogecoinTerminal.Pages
{

	[PageDef("Pages/Xml/SetupWalletPage.xml")]
	internal class SetupWalletPage : Page
	{

		private Game _game;

		public SetupWalletPage(IPageOptions options, Navigation navigation, Strings strings, Game game, ITerminalSettings settings, LibDogecoinContext ctx) : base(options)
		{
			_game = game;

			OnClick("NewWalletButton", async _ =>
			{
				SetupWallet(true, navigation, strings, settings, ctx);
			});

			OnClick("LoadWalletButton", async _ =>
			{
				SetupWallet(false, navigation, strings, settings, ctx);
			});

		
		}


		private async void SetupWallet(bool isNew, Navigation navigation, Strings strings, ITerminalSettings settings, LibDogecoinContext ctx)
		{

			await navigation.PushAsync<LoadingPage>();

			var newPin = string.Empty;
			var confirmPin = string.Empty;

			while (newPin != confirmPin || newPin.Length < SimpleDogeWallet.MIN_PIN_LENGTH)
			{
				var enterPin = await navigation.PromptAsync<NumPadPage>(
					("title", strings.GetString("terminal-setup-setpin")),
					("hint", strings.GetString("terminal-setup-setpin-hint")),
					("regex", ".{" + SimpleDogeWallet.MIN_PIN_LENGTH + ",16}"));

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

			string address = string.Empty;
			bool createdWallet = false;

			string mnemonic = string.Empty;

			if (isNew)
			{
				mnemonic = ctx.GenerateMnemonicEncryptWithTPM(SimpleDogeWallet.TPM_FILE_NUMBER, lang: strings.Language.LanguageCode, space: "-");
				await navigation.PromptAsync<BackupCodePage>(("mnemonic", mnemonic), ("editmode", false));

			}
			else
			{

				while(true)
				{
					var getMnemonic = await navigation.PromptAsync<BackupCodePage>(("title", strings.GetString("terminal-backupcodes-load-title")),
																					("editmode", true),
																					("mnemonic", mnemonic));

					if (getMnemonic.Response == PromptResponse.YesConfirm)
					{
						//we need to confirm
						mnemonic = ((string)getMnemonic.Value).Trim('-');

						if (!IsValidMnemonic(strings.Language.LanguageCode, mnemonic))
						{
							var retry = await navigation.PromptAsync<YesNoPage>(("message", strings.GetString("terminal-backupcodes-load-badmnemonic")));


							if(retry.Response == PromptResponse.YesConfirm)
							{
								continue;
							}
							else
							{
								navigation.Pop();
								return;
							}
						}

						//we have a valid mnemonic, encrypt it with key stored in tpm
						var mnemonicKey = ctx.GenerateMnemonicEncryptWithTPM(SimpleDogeWallet.TPM_FILE_NUMBER, lang: "eng", space: "-");

						File.WriteAllText(SimpleDogeWallet.LOADED_MNEMONIC_FILE, Crypto.Encrypt(mnemonic, mnemonicKey));

						break;
					}
					else
					{
						navigation.Pop();
						return;
					}
				}

			}

			var masterKeys = ctx.GenerateHDMasterPubKeypairFromMnemonic(mnemonic.Replace("-", " "));

			if (ctx.VerifyHDMasterPubKeyPair(masterKeys.privateKey, masterKeys.publicKey))
			{
				address = ctx.GetDerivedHDAddressByPath(masterKeys.privateKey, Crypto.HDPATH, false);

				File.WriteAllText("address", Crypto.Encrypt(address, newPin));

				createdWallet = true;

				settings.Set("user-entered-mnemonic", !isNew);
			}

			

			GC.Collect();

			if (createdWallet)
			{
				await navigation.TryInsertBeforeAsync<UnlockTerminalPage, LoadingPage>(("address", address));
				await navigation.PopToPage<UnlockTerminalPage>();
			}
			else
			{
				await navigation.PromptAsync<ShortMessagePage>(("message", "i have no idea how you did that"));
				navigation.Pop();
			}
		}


		private bool IsValidMnemonic(string languageCode, string mnemonic)
		{
			//TODO: Verify more than just the number of words.
			var length = mnemonic.Split("-").Length;

			if(!(length == 12 || length == 24))
			{
				return false;
			}
			

			return true;
		}

		public override void Draw(GameTime gameTime, IServiceProvider services)
		{
			var screen = services.GetService<VirtualScreen>();

			screen.DrawRectangle(TerminalColor.DarkGrey, new Point(5, 30), new Point(48, 80));
			screen.DrawRectangle(TerminalColor.DarkGrey, new Point(52, 30), new Point(95, 80));

			screen.DrawRectangle(TerminalColor.Blue, new Point(5, 30), new Point(48, 40));
			screen.DrawRectangle(TerminalColor.Green, new Point(52, 30), new Point(95, 40));



			base.Draw(gameTime, services);
		}
	}
}
