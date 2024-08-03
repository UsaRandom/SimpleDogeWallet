using SimpleDogeWallet.Common;
using SimpleDogeWallet.Common.Pages;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace SimpleDogeWallet.Pages
{
	[PageDef("Pages/Xml/DisclaimerPage.xml")]
	internal class DisclaimerPage : PromptPage
	{

		private int _startTime = -1;
		private ButtonControl _wowButton;

		private bool _canPressWow = false;

		private const int WAIT_TIME_SECONDS = 10;

		public DisclaimerPage(IPageOptions options, Game game) : base(options)
		{
			_wowButton = GetControl<ButtonControl>("WowButton");

			var wow = game.Content.Load<Song>("wow");
			
			MediaPlayer.Volume = 0.10f;

			OnClick("WowButton", _ =>
			{
				if (_canPressWow)
				{
					MediaPlayer.Play(wow);
					Submit();
				}
			});
		}

		public override void Draw(GameTime gameTime, IServiceProvider services)
		{
			var screen = services.GetService<VirtualScreen>();


			screen.DrawRectangle(TerminalColor.Grey, new Point(5, 36), new Point(95, 64));

			base.Draw(gameTime, services);
		}

		public override void Update(GameTime gameTime, IServiceProvider services)
		{
			if(_startTime == -1)
			{
				_startTime = (int)gameTime.TotalGameTime.TotalSeconds;
			}

			if (!_canPressWow)
			{
				var diff = WAIT_TIME_SECONDS - (int)gameTime.TotalGameTime.TotalSeconds - _startTime;

				if (diff > -1)
				{
					_wowButton.Text = diff.ToString();

					if (diff == 0)
					{
						_canPressWow = true;
						_wowButton.BackgroundColor = TerminalColor.Blue;
						_wowButton.Text = services.GetService<Strings>().GetString("terminal-disclaimer-wow");
					}
				}
			}

			base.Update(gameTime, services);
		}

	}
}
