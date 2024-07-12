using SimpleDogeWallet.Common.Interop;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml.Linq;

namespace SimpleDogeWallet.Common
{
	public class TextInputControl : ButtonControl
	{
		public TextInputControl(XElement element)
			: base(element)
		{
		}


		private bool _backspaceActive;
		private double _backKeyPressedTime;
		private double _backspaceTimer;

		public bool Editable { get; set; } = true;


		public override void Update(GameTime time, IServiceProvider services)
		{
			if (!Enabled) return;

			if (IsSelected && Editable)
			{
				var inputService = services.GetService<IUserInputService>();

				var userInput = inputService.GetTextInput();


				if (!_backspaceActive && inputService.IsKeyDown(Keys.Back) && !string.IsNullOrEmpty(Text))
				{
					Text = Text.Substring(0, Text.Length - 1);
					_backspaceActive = true;
					_backKeyPressedTime = time.TotalGameTime.TotalMilliseconds;
				}


				if (_backspaceActive && !inputService.IsKeyDown(Keys.Back))
				{
					_backspaceActive = false;
					_backKeyPressedTime = 0;
				}

				if (_backspaceActive && (time.TotalGameTime.TotalMilliseconds - _backKeyPressedTime) >= 700)
				{
					_backspaceTimer += time.ElapsedGameTime.TotalMilliseconds;
					if (_backspaceTimer >= 60)
					{
						_backspaceTimer = 0;
						if (Text.Length > 0)
						{
							Text = Text.Substring(0, Text.Length - 1);
						}
					}
				}

				Text += userInput;
			}
		}



	}
}
