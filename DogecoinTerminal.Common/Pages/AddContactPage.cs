using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZXing.QrCode;
using ZXing;
using System.Runtime.CompilerServices;

namespace DogecoinTerminal.Common.Pages
{
	[PageDef("Pages/Xml/AddContactPage.xml")]
	public class AddContactPage : PromptPage
	{
		private string _address;

		private string _previousLabel;
		private string _label;


		private TextInputControl _labelControl;

		public AddContactPage(IPageOptions options, Navigation navigation, IClipboardService clipboardService) : base(options)
		{
			_address = options.GetOption<string>("address");

			_label = string.Empty;

			_labelControl = GetControl<TextInputControl>("LabelText");
			GetControl<ButtonControl>("AddButton").Enabled = false;
			GetControl<ButtonControl>("AddressText").Text = _address;

			OnClick("BackButton", _ =>
			{
				Cancel();
			});

			OnClick("LabelPasteButton", _ =>
			{
				_label = _labelControl.Text = clipboardService.GetClipboardContents();
			});


			OnClick("AddButton", _ =>
			{
				var newContact = new Contact();
				newContact.Address = _address;
				newContact.Name = _label;

				Submit(newContact);
			});


		}




		public override void Update(GameTime gameTime, IServiceProvider services)
		{
			_label = GetControl<TextInputControl>("LabelText").Text?.Trim() ?? string.Empty;

			bool allValid = true;


			if (_previousLabel != _label)
			{
				if (string.IsNullOrWhiteSpace(_label))
				{
					GetControl<ButtonControl>("AddButton").Enabled = false;
				}
				else
				{
					GetControl<ButtonControl>("AddButton").Enabled = true;
				}
			}

			_previousLabel = _label;

			base.Update(gameTime, services);
		}

	}
}
