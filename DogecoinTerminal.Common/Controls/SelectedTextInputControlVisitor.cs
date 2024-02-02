using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DogecoinTerminal.Common.Controls
{
	public class SelectedTextInputControlVisitor : IControlVisitor
	{
		private bool _isSelecting = false;

		private MouseState _previousState = default;
		private MouseState _currentState = default;

		private VirtualScreen _screen;

		public SelectedTextInputControlVisitor(VirtualScreen screen)
		{
			_screen = screen;
		}


		public void UpdateMouse()
		{
			_currentState = Mouse.GetState();

			_isSelecting = false;

			if (_currentState.LeftButton == ButtonState.Pressed && _previousState.LeftButton == ButtonState.Released)
			{
				_isSelecting = true;
			}

			_previousState = _currentState;
		}


		public void VisitTextInput(TextInputControl textInputControl)
		{
			if (_isSelecting)
			{
				if (textInputControl.ContainsPoint(_screen.WindowCoordToVirtualCoord(new Point(_previousState.X, _previousState.Y))))
				{
					textInputControl.IsSelected = true;
				}
				else
				{
					textInputControl.IsSelected = false;
				}
			}
		}


		public void VisitButton(ButtonControl buttonControl) { }

		public void VisitImage(ImageControl imageControl) { }

		public void VisitSprite(SpriteControl spriteControl) { }

		public void VisitText(TextControl textControl) { }
	}
}
