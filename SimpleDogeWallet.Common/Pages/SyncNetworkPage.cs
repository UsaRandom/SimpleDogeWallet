using Microsoft.VisualBasic;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection.Emit;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace SimpleDogeWallet.Common.Pages
{

	[PageDef("Pages/Xml/SyncNetworkPage.xml")]
	public class SyncNetworkPage : PromptPage, IReceiver<SPVSyncProgressMessage>
	{

		private SPVFastSyncService _spvSyncService;
		private SimpleSPVNodeService _spvNodeService;
		private Strings _strings;

		public SyncNetworkPage(IPageOptions options, Strings strings, SimpleSPVNodeService spvNodeService) : base(options)
		{
			_strings = strings;
			_spvSyncService = new SPVFastSyncService();
			_spvNodeService = spvNodeService;

			Messenger.Default.Register<SPVSyncProgressMessage>(this);
			_spvSyncService.Run();

			_message = _strings.GetString("terminal-spv-fast-sync-connecting");


			OnClick("BackButton", _ =>
			{
				this.Cancel();
			});
		}

		public override void Cleanup()
		{
			Messenger.Default.Deregister<SPVSyncProgressMessage>(this);
			_spvSyncService.Stop();
			base.Cleanup();
		}

		public void Receive(SPVSyncProgressMessage message)
		{
			if(message.BestKnownHeight < message.Block.BlockHeight)
			{
				_message = _strings.GetString("terminal-spv-fast-sync-connecting");

				_spvNodeService.Stop();
				_spvNodeService.Start();
				return;
			}

			XPos = (int)(10 + (80 * message.PercentDone));
			_message = message.Block.BlockHeight.ToString("N0") + " / " + message.BestKnownHeight.ToString("N0");

			if (message.PercentDone == 1.0M)
			{
				_spvNodeService.NEW_WALLET_START_BLOCK = message.Block;
				File.WriteAllText(SimpleSPVNodeService.SPV_CHECKPOINT_FILE, $"{message.Block.Hash}:{message.Block.BlockHeight}");
				this.Submit(message.Block);
			}
		}

		private int XPos = 10;
		private string _message = string.Empty;


		public override void Draw(GameTime gameTime, IServiceProvider services)
		{
			var screen = services.GetService<VirtualScreen>();

			screen.DrawRectangle(TerminalColor.DarkGrey, new Point(10, 65), new Point(90, 75));
			//screen.DrawRectangleBorder(TerminalColor.White, new Point(10, 65), new Point(90, 75));
			
			screen.DrawRectangle(TerminalColor.Blue, new Point(10, 65), new Point(XPos, 75));
			screen.DrawText(_message, TerminalColor.White, 3, new Point(50, 70), TextAnchor.Center);


			base.Draw(gameTime, services);
		}


		public override void Update(GameTime gameTime, IServiceProvider services)
		{
			base.Update(gameTime, services);
		}


	}
}
