using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DogecoinTerminal.Common
{
	//TODO: Not really happy with this class as it is
	public class InteractionMonitor
	{



		private int _padding = 5;

		private (int x, int y) lastEvtPos = (0,0);

		private MouseState _lastState = new MouseState();


		

		public (bool isFirst, int x, int y)? GetInteraction(TouchCollection touches, MouseState nextState)
		{
			//we check for touch interaction first
			if(touches.Count > 0)
			{
				var touch = touches[0];
				
				return (true, (int)touch.Position.X, (int)touch.Position.Y);	
			}

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
