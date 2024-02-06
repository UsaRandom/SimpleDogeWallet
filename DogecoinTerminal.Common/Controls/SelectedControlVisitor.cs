using DogecoinTerminal.Common.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DogecoinTerminal.Common.Controls
{
	public class SelectedControlVisitor : IControlVisitor
	{
		private bool _isSelecting = false;

		private MouseState _previousState = default;
		private MouseState _currentState = default;

		private VirtualScreen _screen;

		public SelectedControlVisitor(VirtualScreen screen)
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



		public void VisitButton(ButtonControl buttonControl)
		{
			if (_isSelecting)
			{
				if (buttonControl.ContainsPoint(_screen.WindowCoordToVirtualCoord(new Point(_previousState.X, _previousState.Y))))
				{
					buttonControl.IsSelected = true;
				}
				else
				{
					buttonControl.IsSelected = false;
				}
			}
		}

		public void VisitImage(ImageControl imageControl) { }

		public void VisitSprite(SpriteControl spriteControl) { }

		public void VisitText(TextControl textControl) { }

		public void VisitContact(ContactControl contactControl)
		{
			if (_isSelecting)
			{
				if (contactControl.ContainsPoint(_screen.WindowCoordToVirtualCoord(new Point(_previousState.X, _previousState.Y))))
				{
					contactControl.IsSelected = true;
				}
			}
		}
	}
}
