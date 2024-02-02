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

namespace DogecoinTerminal.Pages
{

	[PageDef("Pages/Xml/SetupWalletPage.xml")]
	internal class SetupWalletPage : Page
	{

		private const int MIN_PIN_LENGTH = 4;


		public SetupWalletPage(IPageOptions options, Navigation navigation, Strings strings, Game game) : base(options)
		{

			OnClick("NewWalletButton", async _ =>
			{
				SetupWallet(true, navigation, strings);
			});

			OnClick("LoadWalletButton", async _ =>
			{
				SetupWallet(false, navigation, strings);
			});

		
		}


		private async void SetupWallet(bool isNew, Navigation navigation, Strings strings)
		{

			await navigation.PushAsync<BlankPage>();

			var newPin = string.Empty;
			var confirmPin = string.Empty;

			while (newPin != confirmPin || newPin.Length < MIN_PIN_LENGTH)
			{
				var enterPin = await navigation.PromptAsync<NumPadPage>(("title", strings.GetString("terminal-setup-setpin")));

				if (enterPin.Response == PromptResponse.YesConfirm)
				{
					newPin = (string)enterPin.Value;
				}
				else
				{
					navigation.Pop();
					return;
				}

				var confirm = await navigation.PromptAsync<NumPadPage>(("title", strings.GetString("terminal-setup-confirmpin")));

				if (confirm.Response == PromptResponse.YesConfirm)
				{
					confirmPin = (string)confirm.Value;
				}
			}

			string address = string.Empty;
			bool createdWallet = false;

			using (var ctx = LibDogecoinContext.CreateContext())
			{
				string mnemonic = string.Empty;

				if (isNew)
				{
					mnemonic = ctx.GenerateMnemonicEncryptWithTPM(DogecoinTerminalGame.TPM_FILE_NUMBER, lang: strings.Language.LanguageCode, space: "-");
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
							mnemonic = (string)getMnemonic.Value;

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

							break;
						}
						else
						{
							navigation.Pop();
							return;
						}
					}

				}

				var masterKeys = ctx.GenerateHDMasterPubKeypairFromMnemonic(mnemonic);

				if (ctx.VerifyHDMasterPubKeyPair(masterKeys.privateKey, masterKeys.publicKey))
				{
					address = ctx.GetDerivedHDAddressByPath(masterKeys.privateKey, Crypto.HDPATH, false);

					File.WriteAllText("address", Crypto.Encrypt(address, newPin));

					await navigation.PromptAsync<BackupCodePage>(("mnemonic", mnemonic), ("editmode", false));

					createdWallet = true;
				}

			}

			GC.Collect();

			if (createdWallet)
			{
				await navigation.TryInsertBeforeAsync<WalletPage, StartPage>(("address", address));
				await navigation.PopToPage<WalletPage>();
			}
			else
			{
				await navigation.PromptAsync<ShortMessagePage>(("message", "??????"));
				navigation.Pop();
			}
		}


		private bool IsValidMnemonic(string languageCode, string mnemonic)
		{
			//TODO: Verify more than just the number of words.
			var length = mnemonic.Split("-").Length;

			if(length != 12 || length != 24)
			{
				return false;
			}
			

			return true;
		}

		public override void Draw(GameTime gameTime, IServiceProvider services)
		{
			var screen = services.GetService<VirtualScreen>();

			screen.DrawRectangle(TerminalColor.LightGrey, new Point(49, 28), new Point(50, 68));

			base.Draw(gameTime, services);
		}
	}
}
