using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DogecoinTerminal
{
	internal class InteractionMonitor
	{



		private int _padding = 5;

		private (int x, int y) lastEvtPos = (0,0);

		private MouseState _lastState = new MouseState();


		


		public (bool isFirst, int x, int y)? GetInteraction(MouseState nextState)
		{
			//mouse impl, just click, no hodl.
			
			if(_lastState.LeftButton == ButtonState.Released &&
				nextState.LeftButton == ButtonState.Pressed)
			{
				_lastState = nextState;
				return (true, nextState.Position.X, nextState.Position.Y);
			}


			_lastState = nextState;

			return null;
		}

	}
}
