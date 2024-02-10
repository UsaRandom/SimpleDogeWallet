using DogecoinTerminal.Common;
using DogecoinTerminal.Common.Pages;
using FontStashSharp.RichText;
using Lib.Dogecoin.Interop;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace DogecoinTerminal.Pages
{
	[PageDef("Pages/Xml/SPVNodePage.xml")]
	internal class SPVNodePage : Page
	{
		SimpleSPVNodeService _spvNodeService;
		Strings _strings;

		TextControl _onlineOffline;
		ButtonControl _startStop;

		public SPVNodePage(IPageOptions	options, Navigation navigation, SimpleSPVNodeService spvNodeService, Strings strings, IClipboardService clipboard) : base(options)
		{
			_strings = strings;
			_spvNodeService = spvNodeService;


			_startStop = GetControl<ButtonControl>("StartStopButton");
			_onlineOffline = GetControl<TextControl>("OnlineOfflineText");

			OnClick("BackButton", _ =>
			{
				navigation.Pop();
			});

			OnClick("StartStopButton", _ =>
			{
				if (_spvNodeService.IsRunning)
				{
					_spvNodeService.Stop();
				}
				else
				{
					spvNodeService.Start();
				}
			});

			OnClick("CopyButton", _ =>
			{
				spvNodeService.PrintDebug();
				clipboard.SetClipboardContents(GetControl<TextControl>("SPVInfoText").Text);
			});
			OnClick("RescanButton", async _ =>
			{
				await navigation.PushAsync<SPVRescanPage>(("wallet", options.GetOption<SimpleDogeWallet>("wallet")));
			});
		}


		public override void Draw(GameTime gameTime, IServiceProvider services)
		{
			var screen = services.GetService<VirtualScreen>();


			screen.DrawRectangle(TerminalColor.Grey, new Point(10, 38), new Point(90, 72));

			base.Draw(gameTime, services);
		}

		public override void Update(GameTime gameTime, IServiceProvider services)
		{

			_onlineOffline.StringDef = _spvNodeService.IsRunning ?
							"terminal-spv-online" : "terminal-spv-offline";
			_startStop.StringDef = _spvNodeService.IsRunning ?
							"terminal-spv-stop" : "terminal-spv-start";

			_onlineOffline.Color = _spvNodeService.IsRunning ? TerminalColor.Green : TerminalColor.Red;
			_startStop.BackgroundColor = _spvNodeService.IsRunning ? TerminalColor.Red : TerminalColor.Green;
			

			var sb = new StringBuilder();

			sb.AppendLine($"{_strings.GetString("terminal-spv-peers")}: {_spvNodeService.PeerCount}");
			sb.AppendLine($"{_strings.GetString("terminal-spv-txcount")}: {_spvNodeService.TxCount}");
			sb.AppendLine($"{_strings.GetString("terminal-spv-spentutxo")}: {_spvNodeService.SpentUTXOCount}");
			sb.AppendLine($"{_strings.GetString("terminal-spv-newutxo")}: {_spvNodeService.NewUTXOCount}");
			sb.AppendLine($"{_strings.GetString("terminal-spv-netutxo")}: {_spvNodeService.NewUTXOCount-_spvNodeService.SpentUTXOCount}");
			sb.AppendLine();
			sb.AppendLine($"{_strings.GetString("terminal-spv-currentblock")}:");
			sb.AppendLine($"->{_strings.GetString("terminal-spv-currentblock-hash")}: {_spvNodeService.CurrentBlock.Hash}");
			sb.AppendLine($"->{_strings.GetString("terminal-spv-currentblock-height")}: {_spvNodeService.CurrentBlock.BlockHeight}");
			sb.AppendLine($"->{_strings.GetString("terminal-spv-currentblock-time")}: {_spvNodeService.CurrentBlock.Timestamp.ToLocalTime()}");
			sb.AppendLine();
			sb.AppendLine($"{_strings.GetString("terminal-spv-estimatemaxblock")}: {_spvNodeService.EstimatedHeight}");
			sb.AppendLine(_strings.GetString("terminal-spv-estimatemaxblock-hint"));

			GetControl<TextControl>("SPVInfoText").Text = sb.ToString();



			base.Update(gameTime, services);
		}
	}
}
