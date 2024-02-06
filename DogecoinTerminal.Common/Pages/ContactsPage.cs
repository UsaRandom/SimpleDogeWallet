using DogecoinTerminal.Common;
using DogecoinTerminal.Common.Pages;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DogecoinTerminal.Common.Pages
{

	[PageDef("Pages/Xml/ContactsPage.xml")]
	public class ContactsPage : PromptPage
	{

		private string _previousText;

		private Contact _previousContact;

		public ContactsPage(IPageOptions options, IClipboardService clipboard) : base(options)
		{
			EditMode = options.GetOption<bool>("edit-mode", true);
			SelectedContact = default;
			_previousText = default;
			_previousContact = default;

			if (EditMode)
			{
				GetControl<ImageControl>("SubmitButton").Enabled = false;
				GetControl<ImageControl>("EditButton").Enabled = true;
				GetControl<ImageControl>("CopyButton").Enabled = true;
				GetControl<ImageControl>("DeleteButton").Enabled = true;
			}
			else
			{
				GetControl<ImageControl>("SubmitButton").Enabled = true;
				GetControl<ImageControl>("EditButton").Enabled = false;
				GetControl<ImageControl>("CopyButton").Enabled = false;
				GetControl<ImageControl>("DeleteButton").Enabled = false;
			}

			OnClick("CopyButton", _ =>
			{
				if(SelectedContact != default)
				{
					clipboard.SetClipboardContents(SelectedContact.Address);
				}
			});

			OnClick("PasteButton", _ =>
			{
				var clipboardContent = clipboard.GetClipboardContents().Trim();

				GetControl<TextInputControl>("SearchBar").Text = clipboardContent;
			});


		}


		public override void Update(GameTime gameTime, IServiceProvider services)
		{
			var enteredText = GetControl<TextInputControl>("SearchBar").Text?.Trim() ?? string.Empty;
			var contactService = services.GetService<ContactService>();

			if (_previousText != enteredText)
			{
				var isValidP2pkh = Crypto.VerifyP2PKHAddress(enteredText);

				//if valid P2PKH entered, we create a contact or return it to sending dogecoin app flow
				if (isValidP2pkh)
				{
					Contact matchingContact = default;

					//check to see if we have contact, create one
					foreach (var contact in contactService.Contacts)
					{
						if (contact.Address == enteredText)
						{
							matchingContact = contact;
						}
					}

					if (!EditMode)
					{
						Submit(matchingContact ?? new Contact
						{
							Name = services.GetService<Strings>().GetString("common-contacts-unknown"),
							Address = enteredText
						});
					}
					else
					{
						//edit mode, go to create contact screen
					}
				}
				else
				{
					//if not a valid P2PKH address, we filter our results
					//Update filter
					

					Contacts = contactService.Contacts.Where((contact) =>
					{
						//use levenstein distance to filter contacts
						return contact.Name.ToLowerInvariant().Contains(enteredText.ToLowerInvariant()) ||
							   contact.Address.Contains(enteredText);

					}).ToList();

					GoToPage(0);

				}
			}

			_previousText = enteredText;


			//handle selection/buttons



			

			bool newContactSelected = false;

			foreach (var control in Controls)
			{
				if(control is ContactControl)
				{
					var contactControl = (ContactControl)control;

					if (contactControl.IsSelected && contactControl.Contact != _previousContact)
					{
						newContactSelected = true;
						SelectedContact = contactControl.Contact;
						break;
					}
				}
			}

			//disable previously selected contact
			if(newContactSelected)
			{
				foreach (var control in Controls)
				{
					if (control is ContactControl)
					{
						var contactControl = (ContactControl)control;

						if (contactControl.IsSelected && contactControl.Contact != SelectedContact)
						{
							contactControl.IsSelected = false;
						}
					}
				}
			}
			else
			{
				SelectedContact = _previousContact;
			}

			_previousContact = SelectedContact;

			if (EditMode)
			{
				if(SelectedContact != null)
				{
					GetControl<ImageControl>("EditButton").BackgroundColor = TerminalColor.Blue;
					GetControl<ImageControl>("CopyButton").BackgroundColor = TerminalColor.DarkGrey;
					GetControl<ImageControl>("DeleteButton").BackgroundColor = TerminalColor.Red;
				}
				else
				{
					GetControl<ImageControl>("EditButton").BackgroundColor = TerminalColor.LightGrey;
					GetControl<ImageControl>("CopyButton").BackgroundColor = TerminalColor.LightGrey;
					GetControl<ImageControl>("DeleteButton").BackgroundColor = TerminalColor.LightGrey;
				}
			}
			else
			{
				if (SelectedContact != null)
				{
					GetControl<ImageControl>("SubmitButton").BackgroundColor = TerminalColor.Green;
				}
				else
				{
					GetControl<ImageControl>("SubmitButton").BackgroundColor = TerminalColor.LightGrey;
				}
			}


			base.Update(gameTime, services);
		}


		private void GoToPage(int page)
		{
			// 9 contacts per page.
			var contactsOnPage = Contacts.Skip((page - 1) * 9).Take(9).ToList();

			SelectedContact = _previousContact = default;

			foreach (var control in Controls)
			{
				if (control is ContactControl)
				{
					var contactControl = ((ContactControl)control);

					contactControl.IsSelected = false;

					//each name is like Contact0, or Contact8
					var contactIdx = Int32.Parse(contactControl.Name.Substring(7, 1));

					if(contactsOnPage.Count > contactIdx)
					{
						contactControl.Enabled = true;
						contactControl.Contact = contactsOnPage[contactIdx];
					}
					else
					{
						contactControl.Enabled = false;
					}
				}
			}

			// Update the current page and page count.
			CurrentPage = page;
			PageCount = (int)Math.Ceiling((double)Contacts.Count / 9);
		}

		private int CurrentPage { get; set; }
		private int PageCount { get; set; }

		private IList<Contact> Contacts { get; set; }


		public bool EditMode { get; set; } = true;

		public Contact SelectedContact { get; set; }


	}
}
