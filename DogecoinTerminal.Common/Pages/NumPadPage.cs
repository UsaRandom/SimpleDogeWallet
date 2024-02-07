using Cyotek.Drawing.BitmapFont;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DogecoinTerminal.Common.Pages
{
    [PageDef("Pages/Xml/NumPadPage.xml")]
	public  class NumPadPage : PromptPage
	{
		private ImageControl _submitButton;

		private TextControl _titleControl;
		private ButtonControl _userTextControl;
		private string		_regex = string.Empty;
		private bool		_isValueMode = false;
		private string		_hint = string.Empty;

		public NumPadPage(IPageOptions options) : base(options)
		{
			_titleControl = GetControl<TextControl>("Title");
			_submitButton = GetControl<ImageControl>("SubmitButton");
			_userTextControl = GetControl<ButtonControl>("UserTextInput");

			GetControl<TextControl>("HintText").Text = options.GetOption("hint", string.Empty);

			HandleOptions();

			for (var i = 0; i < 10; i++)
			{
				var index = i;
				OnClick($"Button_{i}", _ => {
					AddText(index.ToString());
				});
			}

			OnClick("BackButton", _ =>
			{
				Cancel();
			});

			OnClick($"Button_.", _ => { AddText("."); });
			OnClick($"Button_Delete", _ => { DeleteChar(); });

			OnClick("SubmitButton", _ =>
			{
				if(CanSubmit())
				{
					Submit(_userTextControl.Text);
				}
			});
		}


		private void HandleOptions()
		{
			_regex = Options.GetOption<string>("regex", string.Empty);
			_isValueMode = Options.GetOption<bool>("value-mode", false);
			_hint = Options.GetOption("hint", string.Empty);

			var title = Options.GetOption<string>("title");

			if (!string.IsNullOrEmpty(title))
			{
				_titleControl.StringDef = string.Empty;
				_titleControl.Text = title;
			}

			AddText(Options.GetOption("start-value", string.Empty));

			
		}


		private void AddText(string text)
		{
			var oldText = _userTextControl.Text;
			var newText = oldText;

			if (_isValueMode)
			{
				if (text == "." && oldText.Contains("."))
				{
					return;
				}

				newText += text;

				if (newText == "Đ.")
				{
					newText = "Đ0.";
				}
			}
			else
			{
				newText += text;
			}

			_userTextControl.Text = newText;
		}

		public override void Update(GameTime gameTime, IServiceProvider services)
		{
			if(!CanSubmit())
			{
				GetControl<ImageControl>("SubmitButton").BackgroundColor = TerminalColor.LightGrey;
			}
			else
			{
				GetControl<ImageControl>("SubmitButton").BackgroundColor = TerminalColor.Green;
			}
			base.Update(gameTime, services);
		}

		protected override bool CanSubmit()
		{
			if (!string.IsNullOrEmpty(_regex))
			{
				if (!Regex.IsMatch(_userTextControl.Text, _regex))
				{
					return false;
				}
			}

			if(string.IsNullOrEmpty(_userTextControl.Text))
			{
				return false;
			}
			return true;
		}

		private void DeleteChar()
		{
			if (!string.IsNullOrEmpty(_userTextControl.Text))
			{
				_userTextControl.Text = _userTextControl.Text.Remove(_userTextControl.Text.Length - 1);
			}
		}
	}
}
