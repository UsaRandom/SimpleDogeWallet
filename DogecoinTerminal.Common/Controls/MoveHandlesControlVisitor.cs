using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DogecoinTerminal.Common
{
    public class MoveHandlesControlVisitor : IControlVisitor
    {
        private bool _isSelecting = false;
		private bool _isTopLeft = true;

		private IPageControl _selectedControl = default;
        
        private MouseState _previousState = default;
        private MouseState _currentState = default;

        private VirtualScreen _screen;

        public MoveHandlesControlVisitor(VirtualScreen screen)
        {
            _screen = screen;
        }


        public void UpdateMouse()
        {
            _currentState = Mouse.GetState();

            _isSelecting = false;

            if (_currentState.RightButton == ButtonState.Pressed && _previousState.RightButton == ButtonState.Released)
            {
                _isSelecting = true;
            }

            if(_currentState.RightButton == ButtonState.Released)
            {
                _selectedControl = default;
			}

			_previousState = _currentState;
		}

        public void VisitButton(ButtonControl control)
        {
            if (_isSelecting)
			{
				var topLeft = control.StartPosition;
				var bottomRight = control.EndPosition;

                if(_screen.RenderScale > Vector2.Distance(_screen.VirtualCoordToWindowCoord(topLeft).ToVector2(), _currentState.Position.ToVector2()))
                {
                    _isSelecting = false;
					_isTopLeft = true;
					_selectedControl = control;
                }
                else if (_screen.RenderScale > Vector2.Distance(_screen.VirtualCoordToWindowCoord(bottomRight).ToVector2(), _currentState.Position.ToVector2()))
                {
					_isSelecting = false;
					_isTopLeft = false;
					_selectedControl = control;
				}
			}

            if(_selectedControl == control)
            {
                if (_isTopLeft)
                {
					control.StartPosition = _screen.WindowCoordToVirtualCoord(_currentState.Position);
                }
                else
                {

					control.EndPosition = _screen.WindowCoordToVirtualCoord(_currentState.Position);
				}
            }
		}

        public void VisitImage(ImageControl control)
		{
			if (_isSelecting)
			{
				var topLeft = control.StartPosition;
				var bottomRight = control.EndPosition;

				if (_screen.RenderScale > Vector2.Distance(_screen.VirtualCoordToWindowCoord(topLeft).ToVector2(), _currentState.Position.ToVector2()))
				{
					_isSelecting = false;
					_isTopLeft = true;
					_selectedControl = control;
				}
				else if (_screen.RenderScale > Vector2.Distance(_screen.VirtualCoordToWindowCoord(bottomRight).ToVector2(), _currentState.Position.ToVector2()))
				{
					_isSelecting = false;
					_isTopLeft = false;
					_selectedControl = control;
				}
			}

			if (_selectedControl == control)
			{
				if (_isTopLeft)
				{
					control.StartPosition = _screen.WindowCoordToVirtualCoord(_currentState.Position);
				}
				else
				{

					control.EndPosition = _screen.WindowCoordToVirtualCoord(_currentState.Position);
				}
			}

		}

        public void VisitSprite(SpriteControl control)
        {

			if (_isSelecting)
			{
				var topLeft = control.StartPosition;
				var bottomRight = control.EndPosition;

				if (_screen.RenderScale > Vector2.Distance(_screen.VirtualCoordToWindowCoord(topLeft).ToVector2(), _currentState.Position.ToVector2()))
				{
					_isSelecting = false;
					_isTopLeft = true;
					_selectedControl = control;
				}
				else if (_screen.RenderScale > Vector2.Distance(_screen.VirtualCoordToWindowCoord(bottomRight).ToVector2(), _currentState.Position.ToVector2()))
				{
					_isSelecting = false;
					_isTopLeft = false;
					_selectedControl = control;
				}
			}

			if (_selectedControl == control)
			{
				if (_isTopLeft)
				{
					control.StartPosition = _screen.WindowCoordToVirtualCoord(_currentState.Position);
				}
				else
				{

					control.EndPosition = _screen.WindowCoordToVirtualCoord(_currentState.Position);
				}
			}
		}

        public void VisitText(TextControl control)
        {

			if (_isSelecting)
			{
				var center = control.Position;

				if (_screen.RenderScale > Vector2.Distance(_screen.VirtualCoordToWindowCoord(center).ToVector2(), _currentState.Position.ToVector2()))
				{
					_isSelecting = false;
					_selectedControl = control;
				}
			}

			if (_selectedControl == control)
			{
				control.Position = _screen.WindowCoordToVirtualCoord(_currentState.Position);
			}
		}
    }
}
