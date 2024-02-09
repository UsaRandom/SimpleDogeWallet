using DogecoinTerminal.Common.Pages;
using DogecoinTerminal.Common;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace DogecoinTerminal.Pages
{
	internal class StartPage : Page
	{
		public StartPage(IPageOptions options, Navigation navigation, DogecoinTerminalGame game) : base(options)
		{
		}

		public override void Update(GameTime gameTime, IServiceProvider services)
		{

			base.Update(gameTime, services);
		}

	}
}
