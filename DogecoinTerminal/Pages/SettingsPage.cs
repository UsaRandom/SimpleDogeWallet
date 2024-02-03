using DogecoinTerminal.Common;
using DogecoinTerminal.Common.Pages;
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
		public SettingsPage(IPageOptions options, ITerminalSettings settings, Navigation navigation) : base(options)
		{
			GetControl<CheckboxControl>("ToggleBackground").IsChecked = settings.GetBool("terminal-background");
			GetControl<CheckboxControl>("ToggleFullscreen").IsChecked = settings.GetBool("terminal-fullscreen");
			GetControl<CheckboxControl>("ToggleDevMode").IsChecked = settings.GetBool("terminal-devmode");


			OnClick("BackButton", async _ =>
			{
				navigation.Pop();
			});


			OnClick("ToggleBackground", async _ =>
			{
				var checkbox = GetControl<CheckboxControl>("ToggleBackground");
				checkbox.IsChecked = !checkbox.IsChecked;
				settings.Set("terminal-background", checkbox.IsChecked);
			});

			OnClick("ToggleFullscreen", async _ =>
			{
				var checkbox = GetControl<CheckboxControl>("ToggleFullscreen");
				checkbox.IsChecked = !checkbox.IsChecked;
				settings.Set("terminal-fullscreen", checkbox.IsChecked);
			});

			OnClick("ToggleDevMode", async _ =>
			{
				var checkbox = GetControl<CheckboxControl>("ToggleDevMode");
				checkbox.IsChecked = !checkbox.IsChecked;
				settings.Set("terminal-devmode", checkbox.IsChecked);
			});






		}
	}
}
