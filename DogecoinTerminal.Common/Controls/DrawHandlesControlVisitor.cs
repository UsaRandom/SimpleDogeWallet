using DogecoinTerminal.Common.Controls;
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
    public class DrawHandlesControlVisitor : IControlVisitor
    {

        private SpriteBatch _batch;
        private VirtualScreen _screen;

        public DrawHandlesControlVisitor(SpriteBatch batch, VirtualScreen screen)
        {
            _screen = screen;
            _batch = batch;
        }


		public void VisitButton(ButtonControl control)
        {
            var topLeft = control.StartPosition;
            var bottomRight = control.EndPosition;

            if(_screen.RenderScale > Vector2.Distance(Mouse.GetState().Position.ToVector2(), _screen.VirtualCoordToWindowCoord(topLeft).ToVector2()))
            {
				_screen.DrawText(topLeft.ToString(), TerminalColor.White, 2, new Point(topLeft.X, topLeft.Y - 2));
            }
			if(_screen.RenderScale > Vector2.Distance(Mouse.GetState().Position.ToVector2(), _screen.VirtualCoordToWindowCoord(bottomRight).ToVector2()))
			{

				_screen.DrawText(bottomRight.ToString(), TerminalColor.White, 2, new Point(bottomRight.X, bottomRight.Y - 2));
			}

			_batch.DrawCircle(_screen.VirtualCoordToWindowCoord(topLeft).ToVector2(), 1 * _screen.RenderScale, 16, TerminalColor.White.Color);
			_batch.DrawCircle(_screen.VirtualCoordToWindowCoord(bottomRight).ToVector2(), 1 * _screen.RenderScale, 16, TerminalColor.White.Color);
		}

        public void VisitImage(ImageControl control)
		{
			var topLeft = control.StartPosition;
			var bottomRight = control.EndPosition;

			if (_screen.RenderScale > Vector2.Distance(Mouse.GetState().Position.ToVector2(), _screen.VirtualCoordToWindowCoord(topLeft).ToVector2()))
			{
				_screen.DrawText(topLeft.ToString(), TerminalColor.White, 2, new Point(topLeft.X, topLeft.Y - 2));
			}
			if (_screen.RenderScale > Vector2.Distance(Mouse.GetState().Position.ToVector2(), _screen.VirtualCoordToWindowCoord(bottomRight).ToVector2()))
			{

				_screen.DrawText(bottomRight.ToString(), TerminalColor.White, 2, new Point(bottomRight.X, bottomRight.Y - 2));
			}

			_batch.DrawCircle(_screen.VirtualCoordToWindowCoord(topLeft).ToVector2(), 1 * _screen.RenderScale, 16, TerminalColor.White.Color);
			_batch.DrawCircle(_screen.VirtualCoordToWindowCoord(bottomRight).ToVector2(), 1 * _screen.RenderScale, 16, TerminalColor.White.Color);

		}

        public void VisitSprite(SpriteControl control)
        {

			var topLeft = control.StartPosition;
			var bottomRight = control.EndPosition;

			if (_screen.RenderScale > Vector2.Distance(Mouse.GetState().Position.ToVector2(), _screen.VirtualCoordToWindowCoord(topLeft).ToVector2()))
			{
				_screen.DrawText(topLeft.ToString(), TerminalColor.White, 2, new Point(topLeft.X, topLeft.Y - 2));
			}
			if (_screen.RenderScale > Vector2.Distance(Mouse.GetState().Position.ToVector2(), _screen.VirtualCoordToWindowCoord(bottomRight).ToVector2()))
			{

				_screen.DrawText(bottomRight.ToString(), TerminalColor.White, 2, new Point(bottomRight.X, bottomRight.Y - 2));
			}

			_batch.DrawCircle(_screen.VirtualCoordToWindowCoord(topLeft).ToVector2(), 1 * _screen.RenderScale, 16, TerminalColor.White.Color);
			_batch.DrawCircle(_screen.VirtualCoordToWindowCoord(bottomRight).ToVector2(), 1 * _screen.RenderScale, 16, TerminalColor.White.Color);
		}

        public void VisitText(TextControl control)
		{
			var center = control.Position;


			if (_screen.RenderScale > Vector2.Distance(Mouse.GetState().Position.ToVector2(), _screen.VirtualCoordToWindowCoord(center).ToVector2()))
			{
				_screen.DrawText(center.ToString(), TerminalColor.White, 2, new Point(center.X, center.Y - 2));
			}
			_batch.DrawCircle(_screen.VirtualCoordToWindowCoord(center).ToVector2(), 1 * _screen.RenderScale, 16, TerminalColor.White.Color);

		}

		public void VisitContact(ContactControl contactControl)
		{

		}
	}
}
