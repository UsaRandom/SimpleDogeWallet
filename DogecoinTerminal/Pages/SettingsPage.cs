using DogecoinTerminal.Common;
using DogecoinTerminal.Common.Pages;
using DogecoinTerminal.old;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DogecoinTerminal.Pages
{
    [PageDef("Pages/Xml/SettingsPage.xml")]
	internal class SettingsPage : Page
	{
		public SettingsPage(IPageOptions options, ITerminalSettings settings, Navigation navigation, ITerminalService terminalService) : base(options)
		{
			OnClick("BackButton", async _ =>
			{
				navigation.Pop();
			});


			OnClick("EnableDevModeButton", async _ =>
			{
				var previousSetting = settings.GetBool("terminal-devmode", false);

				settings.Set("terminal-devmode", !previousSetting);
			});

			OnClick("EnableBackgroundButton", async _ =>
			{
				var previousSetting = settings.GetBool("terminal-background", false);

				settings.Set("terminal-background", !previousSetting);
			});


			OnClick("FullscreenButton", async _ =>
			{
				var previousSetting = settings.GetBool("terminal-fullscreen", false);

				settings.Set("terminal-fullscreen", !previousSetting);
			});


			OnClick("DeleteSlotsButton", async _ =>
			{

				//lets confirm

				var response = await navigation.PromptAsync<ShortMessagePage>(("message", "WARNING: Pressing 'OK' deletes ALL wallet slots."));

				if(response.Response == PromptResponse.YesConfirm)
				{
					for(var i = 0; i < 6; i++)
					{
						terminalService.ClearSlot(i);
					}
				}


			});

		}
	}
}
