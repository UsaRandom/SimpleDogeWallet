using DogecoinTerminal.Common;
namespace DogecoinTerminal
{




	//Messenger should be internal






	class SettingsPage : Page
	{
		public SettingsPage(IPageOptions options) : base(options)
		{

		}
	}



	class WalletPage : Page
	{
		public WalletPage(IPageOptions options) : base(options)
		{
		}
	}
	class FillWalletSlotPage : Page
	{
		public FillWalletSlotPage(IPageOptions options) : base(options)
		{
		}
	}


	


	class NumberPadPage : PromptPage
	{
		private string UserInput;

		public NumberPadPage(IPageOptions options) : base(options)
		{
			UserInput = string.Empty;

			for (var i = 0; i < 10; i++)
			{
				OnClick($"NumberButton_{0}", _ => { AddText($"{i}"); });
			}
		}


		private void AddText(string text)
		{

			UserInput += text;
		}


	}


	interface ITerminalModule
	{

	}



	



}
