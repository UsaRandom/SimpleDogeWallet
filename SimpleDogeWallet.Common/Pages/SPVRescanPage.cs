using SimpleDogeWallet.Common.Pages;
using SimpleDogeWallet.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using OpenCvSharp.ImgHash;

namespace SimpleDogeWallet.Pages
{
	[PageDef("Pages/Xml/SPVRescanPage.xml")]
	internal class SPVRescanPage : Page
	{
		public SPVRescanPage(IPageOptions options, Navigation navigation, SimpleSPVNodeService spvNodeService, Strings strings, IClipboardService clipboard) : base(options)
		{
			// BlockHashPasteButton BlockHashText BlockHeightText BlockHeightPasteButton SyncButton

			OnClick("BlockHashPasteButton", _ =>
			{
				GetControl<TextInputControl>("BlockHashText").Text = clipboard.GetClipboardContents();
			});


			OnClick("BlockHeightPasteButton", _ =>
			{
				GetControl<TextInputControl>("BlockHeightText").Text = clipboard.GetClipboardContents();
			});

			OnClick("BackButton", _ =>
			{
				navigation.Pop();
			});

			OnClick("SyncButton", async _ =>
			{

				var hash = GetControl<TextInputControl>("BlockHashText").Text;
				var height = GetControl<TextInputControl>("BlockHeightText").Text;

				if(string.IsNullOrWhiteSpace(hash) || string.IsNullOrWhiteSpace(height))
				{
					return;
				}

				uint blockHeight = 0;

				try
				{
					blockHeight = uint.Parse(Regex.Replace(height, @"[^\d]", ""));
				}
				catch { return; }
				
				spvNodeService.Rescan(new Lib.Dogecoin.SPVNodeBlockInfo()
				{
					Hash = GetControl<TextInputControl>("BlockHashText").Text,
					BlockHeight = uint.Parse(Regex.Replace(GetControl<TextInputControl>("BlockHeightText").Text, @"[^\d]", ""))
				});
				navigation.Pop();
			});
		}
	}
}
