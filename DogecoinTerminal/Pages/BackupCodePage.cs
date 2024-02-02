using DogecoinTerminal.Common;
using DogecoinTerminal.Common.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DogecoinTerminal.Pages
{
    [PageDef("Pages/Xml/BackupCodePage.xml")]
	internal class BackupCodePage : PromptPage
	{

		private bool _editMode = false; 

		public BackupCodePage(IPageOptions options) : base(options)
		{
			var title = options.GetOption<string>("title");

			if(!string.IsNullOrEmpty(title))
			{
				var titleControl = GetControl<TextControl>("Title");
				titleControl.StringDef = string.Empty;
				titleControl.Text = title;
			}

			_editMode = options.GetOption<bool>("editmode");

			var mnemonic = options.GetOption<string>("mnemonic");
			

			if(!string.IsNullOrEmpty(mnemonic))
			{
				var words = mnemonic.Split('-');

				for (int i = 0; i < words.Length; i++)
				{
					var wordControl = GetControl<TextInputControl>($"BackupWordButton_{i}");

					wordControl.Text =words[i];
				}
			}

			if(!_editMode)
			{

				for (int i = 0; i < 24; i++)
				{
					var wordControl = GetControl<TextInputControl>($"BackupWordButton_{i}");

					wordControl.Editable = false;
				}

			}

			OnClick("NextButton", _ => {

				string mnemonic = string.Empty;
				if (_editMode)
				{
					var wordList = new List<string>();

					for (int i = 0; i < 24; i++)
					{
						var wordControl = GetControl<TextInputControl>($"BackupWordButton_{i}");

						wordList.Add(wordControl.Text);
					}

					mnemonic = string.Join("-", wordList);
				}

				Submit(mnemonic);

			});


		}


	}
}
