using SimpleDogeWallet.Common;
using SimpleDogeWallet.Common.Pages;
using FontStashSharp.RichText;
using Lib.Dogecoin.Interop;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace SimpleDogeWallet.Pages
{
	[PageDef("Pages/Xml/ExceptionPage.xml")]
	internal class ExceptionPage : Page
	{
		public ExceptionPage(IPageOptions options, IClipboardService clipboard, Game game) : base(options)
		{
			GetControl<TextControl>("ErrorText").Text = options.GetOption<string>("error");
			GetControl<TextControl>("TitleText").Text = options.GetOption<string>("title");

			OnClick("CopyButton", _ =>
			{
				clipboard.SetClipboardContents(GetControl<TextControl>("ErrorText").Text);
			});
			OnClick("ExitButton", async _ =>
			{
				game.Exit();
			});
		}


		public override void Draw(GameTime gameTime, IServiceProvider services)
		{
			var screen = services.GetService<VirtualScreen>();


			screen.DrawRectangle(TerminalColor.Grey, new Point(5, 32), new Point(95, 68));

			base.Draw(gameTime, services);
		}

	}
}
