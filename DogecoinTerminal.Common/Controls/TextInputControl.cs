using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace DogecoinTerminal.Common
{
	/*
	 * TODO: Internationalization Support
	 * 
	 * Currently, this only supports latin characters and doesn't support japanes/korean/chinese input.
	 * We'll have to bind directly to the keyboard input to support the other languages.
	 * 
	 */
	public class TextInputControl : ButtonControl
	{
		public TextInputControl(XElement element)
			: base(element)
		{
		}


		private KeyboardState _previousState;
		private bool _backspaceActive;
		private double _backKeyPressedTime;
		private double _backspaceTimer;

		public bool Editable { get; set; } = true;


		public override void Update(GameTime time, IServiceProvider services)
		{
			if(IsSelected && Editable)
			{
				var currentKeyboardState = Keyboard.GetState();

				var currentKeys = currentKeyboardState.GetPressedKeys();

				var useUpperCase = ((currentKeyboardState.IsKeyDown(Keys.LeftShift) ||
										currentKeyboardState.IsKeyDown(Keys.RightShift)) && !currentKeyboardState.CapsLock) ||
										currentKeyboardState.CapsLock;


				// Determine which keys are newly pressed down
				var newlyPressedKeys = new List<Keys>();
				foreach (Keys key in currentKeys)
				{
					if (!_previousState.IsKeyDown(key))
					{
						newlyPressedKeys.Add(key);
					}
				}

				if(newlyPressedKeys.Contains(Keys.Back) && Text.Length > 0)
				{
					Text = Text.Substring(0, Text.Length - 1);
					_backspaceActive = true;
					_backKeyPressedTime = time.TotalGameTime.TotalMilliseconds;
				}


				if (!currentKeyboardState.IsKeyDown(Keys.Back))
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
			
				// Do something with newly pressed keys
				foreach (Keys key in newlyPressedKeys)
				{

					var character = ((char)key).ToString();

					if("ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789.".Contains(character))
					{

						if (useUpperCase)
						{
							character = character.ToUpper();
						}
						else
						{
							character = character.ToLower();
						}

						Text += character;
					}

				}

				_previousState = currentKeyboardState;
			}
			else
			{
				_previousState = default;
			}
		}



	}
}
