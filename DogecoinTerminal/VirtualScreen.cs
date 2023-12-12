using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DogecoinTerminal
{
	/// <summary>
	/// basically a 0-100 grid that is projected to the physical screen.
	/// also provides a simplified interface for drawing to the screen.
	/// </summary>
	internal class VirtualScreen
	{
		private SpriteBatch _spriteBatch;
		private GraphicsDevice _graphicsDevice;

		private InteractionMonitor _interactionMonitor;

		private SpriteFont _font;
		private Game1 _game;

		private int _width;
		private int _height;

		private decimal _heightScale;
		private decimal _widthScale;

		public void Init(GraphicsDevice graphicsDevice, int height, int width)
		{
			_graphicsDevice = graphicsDevice;

			TerminalColor.Init(_graphicsDevice);
			
			_height = height;
			_width = width;


			_heightScale = _height / 100M;
			_widthScale = _width / 100M;

			_interactionMonitor = new InteractionMonitor();
		}

		public void Load(Game1 game)
		{
			_game = game;
			_font = _game.Content.Load<SpriteFont>("basic"); 
		}


		public void Update(AppPage page)
		{
			var mouseState = Mouse.GetState(_game.Window);

			var interactionResult = _interactionMonitor.GetInteraction(mouseState);

			if(interactionResult.HasValue)
			{
				var interaction = interactionResult.Value;

				var vertPos = (x: (int)(interaction.x / _widthScale),
							   y: (int)(interaction.y / _heightScale));


				
				foreach(var interactable in page.Interactables)
				{
					if (vertPos.x >= interactable.Start.x &&
					   vertPos.y >= interactable.Start.y &&
					   vertPos.x <= interactable.End.x &&
					   vertPos.y <= interactable.End.y)
					{
						interactable.OnInteract(interaction.isFirst);
						break;
					}
				}
			}
		}



		public void Draw(SpriteBatch spriteBatch, AppPage page)
		{
			_spriteBatch = spriteBatch;

			page.DrawScreen(this);
		}



		public void DrawRectangle(TerminalColor color,
								  (int x, int y) start, (int x, int y) end)
		{
			_spriteBatch.Draw(color.Texture,
				new Rectangle(
					(int)Math.Round(start.x * _widthScale),
					(int)Math.Round(start.y * _heightScale),
					(int)Math.Round((end.x - start.x) * _widthScale),
					(int)Math.Round((end.y - start.y) * _heightScale)),
				color.Color);
		}


		public void DrawText(string text, TerminalColor color, double scale, (int x, int y) pos)
		{

			var textSize = _font.MeasureString(text);


			
			_spriteBatch.DrawString(_font, text,
				new Vector2(
					(int)(pos.x * _widthScale) - textSize.X/2,
					(int)(pos.y * _heightScale) - textSize.Y/2	
					),
				color.Color);

		}




	}
}
