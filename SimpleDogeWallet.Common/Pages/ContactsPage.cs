using SimpleDogeWallet.Common;
using SimpleDogeWallet.Common.Pages;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleDogeWallet.Common.Pages
{
	/*
	 * Navigating to the Contacts Page:
	 * 
	 * If you have ("edit-mode", true) as an option, the Contacts page should be pushed into the nav stack (PushAsync)
	 * If you are in contact select mode ("edit-mode", false), then you should use PromptAsync.
	 */ 
	[PageDef("Pages/Xml/ContactsPage.xml")]
	public class ContactsPage : PromptPage
	{

		private string _previousText;

		private Contact _previousContact;
		private ContactService _contactService;

		public ContactsPage(IPageOptions options, IClipboardService clipboard, ContactService contactService, Navigation navigation) : base(options)
		{
			EditMode = options.GetOption<bool>("edit-mode", true);
			SelectedContact = default;
			_previousText = default;
			_previousContact = default;

			_contactService = contactService;

			if (EditMode)
			{
				GetControl<TextControl>("TitleText").StringDef = "terminal-contacts-title";
				GetControl<ImageControl>("SubmitButton").Enabled = false;
				GetControl<ImageControl>("EditButton").Enabled = true;
				GetControl<ImageControl>("CopyButton").Enabled = true;
				GetControl<ImageControl>("DeleteButton").Enabled = true;
			}
			else
			{
				GetControl<TextControl>("TitleText").StringDef = "terminal-contacts-title-sending";
				GetControl<ImageControl>("SubmitButton").Enabled = true;
				GetControl<ImageControl>("EditButton").Enabled = false;
				GetControl<ImageControl>("CopyButton").Enabled = false;
				GetControl<ImageControl>("DeleteButton").Enabled = false;
			}

			OnClick("SubmitButton", _ =>
			{
				if(SelectedContact != default)
				{
					Submit(SelectedContact);
				}
			});

			OnClick("BackButton", _ =>
			{
				if(!EditMode)
				{
					Cancel();
				}
				else
				{
					navigation.Pop();
				}
			});

			OnClick("EditButton", async _ =>
			{
				if (SelectedContact != default)
				{
					var updateResult = await navigation.PromptAsync<UpdateContactPage>(("contact", SelectedContact));

					if(updateResult.Response == PromptResponse.YesConfirm)
					{
						var updatedContact = (Contact)updateResult.Value;

						foreach (var control in Controls)
						{
							if (control is ContactControl)
							{
								var contactControl = (ContactControl)control;

								if (contactControl.IsSelected && contactControl.Contact == SelectedContact)
								{
									contactControl.Contact = updatedContact;
									break;
								}
							}
						}
						_contactService.UpdateContact(SelectedContact, updatedContact);

						SelectedContact = updatedContact;

					}
				}
			});

			OnClick("CopyButton", _ =>
			{
				if(SelectedContact != default)
				{
					clipboard.SetClipboardContents(SelectedContact.Address);
				}
			});

			OnClick("PasteButton", _ =>
			{
				var clipboardContent = clipboard.GetClipboardContents()?.Trim();

				GetControl<TextInputControl>("SearchBar").Text = clipboardContent;
			});

			OnClick("QRButton", async  _ =>
			{
				await navigation.PushAsync<LoadingPage>();

				var qrScanResult = await navigation.PromptAsync<QRScannerPage>();

				if(qrScanResult.Response == PromptResponse.YesConfirm)
				{
					GetControl<TextInputControl>("SearchBar").Text = (string)qrScanResult.Value;
				}

				navigation.Pop();
			});

			OnClick("LeftArrow", _ =>
			{
				if(CurrentPage > 0)
				{
					CurrentPage--;
					GoToPage(CurrentPage);
				}
			});

			OnClick("RightArrow", _ =>
			{
				if (CurrentPage < PageCount-1)
				{
					CurrentPage++;
					GoToPage(CurrentPage);
				}
			});

			OnClick("DeleteButton", _ =>
			{
				if(SelectedContact != default)
				{
					_contactService.RemoveContact(SelectedContact);
					_previousContact = default;
					SelectedContact = default;

					var enteredText = GetControl<TextInputControl>("SearchBar").Text?.Trim() ?? string.Empty;

					if(enteredText !=  string.Empty)
					{
						Contacts = _contactService.Contacts.Where((contact) =>
						{
							//use levenstein distance to filter contacts
							return contact.Name.ToLowerInvariant().Contains(enteredText.ToLowerInvariant()) ||
								   contact.Address.Contains(enteredText);

						}).OrderBy(c => c.Name).ToList();
					}
					else
					{
						Contacts = _contactService.Contacts;
					}

					GoToPage(0);
				}
			});

		}


		public override void Update(GameTime gameTime, IServiceProvider services)
		{
			var searchBar = GetControl<TextInputControl>("SearchBar");
			var enteredText = searchBar.Text?.Trim() ?? string.Empty;

			if (_previousText != enteredText)
			{
				var isValidP2pkh = Crypto.VerifyP2PKHAddress(enteredText);

				//if valid P2PKH entered, we create a contact or return it to sending dogecoin app flow
				if (isValidP2pkh)
				{
					Contact matchingContact = default;

					//check to see if we have contact, create one
					foreach (var contact in _contactService.Contacts)
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
						//edit mode, go to create contact screen/update screen

						Task.Run(async () =>
						{
							Contact contactToUpdate = default;
							foreach(var contact in _contactService.Contacts)
							{
								if(enteredText == contact.Address)
								{
									contactToUpdate = contact;
									break;
								}
							}
							PromptResult editAddResult;

							if(contactToUpdate != null)
							{
								editAddResult = await services.GetService<Navigation>()
															 .PromptAsync<UpdateContactPage>(
																("contact", contactToUpdate));
							}
							else
							{
								editAddResult = await services.GetService<Navigation>()
															 .PromptAsync<AddContactPage>(
																("address", enteredText));

							}

							if (editAddResult.Response == PromptResponse.YesConfirm)
							{
								var updatedContact = (Contact)editAddResult.Value;


								if(contactToUpdate == default)
								{
									_contactService.AddContact(updatedContact);
								}
								else
								{
									_contactService.UpdateContact(contactToUpdate, updatedContact);
								}

								Contacts = _contactService.Contacts;

								GoToPage(0);
							}
						});

						GetControl<TextInputControl>("SearchBar").Text = string.Empty;

					}
				}
				else
				{
					//if not a valid P2PKH address, we filter our results
					//Update filter
					

					Contacts = _contactService.Contacts.Where((contact) =>
					{
						//use levenstein distance to filter contacts
						return contact.Name.ToLowerInvariant().Contains(enteredText.ToLowerInvariant()) ||
							   contact.Address.Contains(enteredText);

					}).OrderBy(c => c.Name).ToList();

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
			var contactsOnPage = Contacts.Skip(page * 9).Take(9).ToList();

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
			PageCount = Math.Max(1,(int)Math.Ceiling((double)Contacts.Count / 9));


			if(CurrentPage + 1 == PageCount)
			{
				GetControl<ImageControl>("RightArrow").ImageSource = @"Content\simple-arrow-right-dim.png";
			}
			else
			{
				GetControl<ImageControl>("RightArrow").ImageSource = @"Content\simple-arrow-right.png";
			}

			if (CurrentPage == 0)
			{
				GetControl<ImageControl>("LeftArrow").ImageSource = @"Content\simple-arrow-left-dim.png";
			}
			else
			{
				GetControl<ImageControl>("LeftArrow").ImageSource = @"Content\simple-arrow-left.png";
			}



			GetControl<TextControl>("PageCountText").Text = $"{CurrentPage+1}/{PageCount}";
		}

		private int CurrentPage { get; set; }
		private int PageCount { get; set; }

		private IList<Contact> Contacts { get; set; }


		public bool EditMode { get; set; } = true;

		public Contact SelectedContact { get; set; }


	}
}
