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
		public const decimal DEFAULT_DUST_LIMIT = 0.001M;
		public const decimal DEFAULT_FEE_PER_BYTE = 0.001M;

		public SettingsPage(IPageOptions options, ITerminalSettings settings, Navigation navigation, Strings strings) : base(options)
		{
			GetControl<CheckboxControl>("ToggleBackground").IsChecked = settings.GetBool("terminal-background", true);
			GetControl<CheckboxControl>("ToggleFullscreen").IsChecked = settings.GetBool("terminal-fullscreen", false);
			GetControl<CheckboxControl>("ToggleDevMode").IsChecked = settings.GetBool("terminal-devmode", false);


			GetControl<ButtonControl>("SetDustLimitButton").Text = settings.GetDecimal("dust-limit", DEFAULT_DUST_LIMIT).ToString();
			GetControl<ButtonControl>("SetFeePerByteButton").Text = settings.GetDecimal("fee-per-byte", DEFAULT_FEE_PER_BYTE).ToString();

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


			OnClick("SetDustLimitButton", async _ =>
			{
				var updateResult = navigation.PromptAsync<NumPadPage>(("title", strings["terminal-settings-dustlimit"]),
																	("value-mode", true),
																	("regex", "0\\.[0-9]{1,8}"));

			});
		}


		
	}
}
