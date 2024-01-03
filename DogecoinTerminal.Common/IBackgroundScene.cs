using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DogecoinTerminal.Common
{
	public interface IBackgroundScene
	{
		void Draw(GameTime gameTime, SpriteBatch spriteBatch, IServiceProvider services);

		void Update(GameTime gameTime, IServiceProvider services);

	}
}
