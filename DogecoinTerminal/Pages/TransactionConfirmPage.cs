using DogecoinTerminal.Common;
using DogecoinTerminal.Common.Pages;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DogecoinTerminal.Pages
{
	[PageDef("Pages/Xml/TransactionConfirmPage.xml")]
	internal class TransactionConfirmPage : PromptPage
	{
		public TransactionConfirmPage(IPageOptions options, Strings strings) : base(options)
		{
			var tx = options.GetOption<DogecoinTransaction>("tx");

			var target = options.GetOption<Contact>("target");
			var targetName = target.Name == strings.GetString("common-contacts-unknown") ? target.ShortAddress : target.Name;

			var format = "Đ {0:#,0.000}";

			GetControl<TextControl>("TransactionAmountVal").Text = string.Format(format, tx.Amount);
			GetControl<TextControl>("TransactionToVal").Text = targetName;
			GetControl<TextControl>("TransactionFeeVal").Text = string.Format(format, tx.Fee);
			GetControl<TextControl>("TransactionTotalVal").Text = string.Format(format, tx.Total);



			OnClick("SendButton", _ =>
			{
				Submit();
			});

			OnClick("BackButton", _ =>
			{
				Cancel();
			});

		}


		public override void Draw(GameTime gameTime, IServiceProvider services)
		{
			var screen = services.GetService<VirtualScreen>();


			screen.DrawRectangle(TerminalColor.Grey, new Point(17, 38), new Point(79, 58));
			screen.DrawRectangle(TerminalColor.DarkGrey, new Point(17, 58), new Point(79, 67));

			base.Draw(gameTime, services);
		}
	}
}
