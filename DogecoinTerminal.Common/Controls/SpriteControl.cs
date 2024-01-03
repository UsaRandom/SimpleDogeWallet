
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DogecoinTerminal.Common
{
    public class SpriteControl : PageControl
	{
		private int _spriteWidth = 0;
		private int _spriteHeight = 0;


		private TimeSpan _timeSinceLastFrame = TimeSpan.Zero;

		private Point _imgDim;
		private Point _center;


		private int _currentFrame;
		public SpriteControl(XElement element) : base(element)
		{
			SpriteSheetSource = element.Attribute(nameof(SpriteSheetSource)).Value;
			Rows = Int32.Parse(element.Attribute(nameof(Rows)).Value);
			Columns = Int32.Parse(element.Attribute(nameof(Columns)).Value);
			FrameCount = Int32.Parse(element.Attribute(nameof(FrameCount)).Value);
			FrameTimeMs = Int32.Parse(element.Attribute(nameof(FrameTimeMs)).Value);
			StartPosition = GetPoint(element.Attribute(nameof(StartPosition)));
			EndPosition = GetPoint(element.Attribute(nameof(EndPosition)));

			_currentFrame = 0;

			_imgDim = new Point(EndPosition.X - StartPosition.X, EndPosition.Y - StartPosition.Y);
			_center = new Point(StartPosition.X + (_imgDim.X / 2), StartPosition.Y + (_imgDim.Y / 2));
		}


		private Rectangle GetSpriteRectangleForFrame(Texture2D source)
		{
			var row = _currentFrame == 0 ? 0 : (_currentFrame / Columns);
			var col = _currentFrame == 0 ? 0 : (_currentFrame % Columns);

			if(_spriteWidth == 0)
			{
				_spriteWidth = source.Width / Columns;
				_spriteHeight = source.Height / Rows;
				
			}

			return new Rectangle(_spriteWidth * col, _spriteHeight * row, _spriteWidth, _spriteHeight);

		}

		public string SpriteSheetSource { get; set; }
		public int Rows { get; set; }
		public int Columns { get; set; }
		public int FrameCount { get; set; }
		public int FrameTimeMs { get; set; }
		public Point StartPosition { get; set; }
		public Point EndPosition { get; set; }




		public override bool ContainsPoint(Point point)
		{
			return false;
		}

		public override void Draw(GameTime time, IServiceProvider services)
		{
			var imgages = services.GetService<Images>();
			var screen = services.GetService<VirtualScreen>();

			var img = imgages.GetImage(SpriteSheetSource);


			var rec = GetSpriteRectangleForFrame(img);

			screen.DrawSprite(img, rec, _center, _imgDim);

		}

		public override void Update(GameTime time, IServiceProvider services)
		{
			_timeSinceLastFrame = _timeSinceLastFrame.Add(time.ElapsedGameTime);

			if(_timeSinceLastFrame.Milliseconds > FrameTimeMs)
			{
				_currentFrame++;
				_timeSinceLastFrame = TimeSpan.Zero;
			}

			if (_currentFrame == FrameCount)
			{
				_currentFrame = 0;
			}
		}


		public override void AcceptVisitor(IControlVisitor visitor)
		{
			visitor.VisitSprite(this);
		}
	}
}
