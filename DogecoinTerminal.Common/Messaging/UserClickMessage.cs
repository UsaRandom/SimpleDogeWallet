using Microsoft.Xna.Framework;

namespace DogecoinTerminal.Common
{
	public class UserClickMessage
	{

		public UserClickMessage(Point point)
		{
			this.ClickLocation = point;
		}

		public Point ClickLocation { get; set; }
	}
}
