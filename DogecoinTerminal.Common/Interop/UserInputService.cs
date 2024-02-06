using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace DogecoinTerminal.Common.Interop
{
	public class UserInputService : IUserInputService
	{
		private Game _game;
		public UserInputService(Game game)
		{
			_game = game;
			_game.Window.TextInput += Window_TextInput;
		}

		private void Window_TextInput(object sender, TextInputEventArgs e)
		{
			if (!char.IsControl(e.Character))
			{
				lock (this)
				{
					_currentInput += e.Character;
				}
			}
		}

		public string GetTextInput()
		{
            lock (this)
			{
				var res = _currentInput;

				_currentInput = string.Empty;

				return res;
			}
		}


		private string _currentInput = string.Empty;


		public bool IsKeyDown(Keys key)
		{
			return Keyboard.GetState().IsKeyDown(key);
		}

		private unsafe string GetTextFromInputEvent(SDL2.SDL.SDL_TextInputEvent textInput)
		{
			return Marshal.PtrToStringUTF8((IntPtr)textInput.text);
		}
	}
}
