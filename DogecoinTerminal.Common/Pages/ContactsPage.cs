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
	public class ContactsPage : Page
	{

		private string _previousText;

		public ContactsPage(IPageOptions options, IClipboardService clipboard) : base(options)
		{
			EditMode = options.GetOption<bool>("edit-mode", true);
			SelectedContact = default;
			_previousText = default;

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
		

			foreach (var control in Controls)
			{
				if(control is ContactControl)
				{
					((ContactControl)control).Contact = new Contact
					{
						Name = "Imaginary Friend",
						Address = "DLPaeuaJi2JLUcvYHD4ddLxadwnGaVSt4p"
					};
				}
			}

			OnClick("PasteButton", _ =>
			{
				var clipboardContent = clipboard.GetClipboardContents().Trim();

				GetControl<TextInputControl>("SearchBar").Text = clipboardContent;
			});


		}


		public override void Update(GameTime gameTime, IServiceProvider services)
		{
			var enteredText = GetControl<TextInputControl>("SearchBar").Text?.Trim() ?? string.Empty;

			if (_previousText != enteredText)
			{
				var isValidP2pkh = Crypto.VerifyP2PKHAddress(enteredText);

				//if valid P2PKH entered, we create a contact or return it to sending dogecoin app flow
				if (isValidP2pkh)
				{
					if (!EditMode)
					{
						//check to see if we have contact, create one
					}
					else
					{
					
					}
				}
				else
				{
					//if not a valid P2PKH address, we filter our results
					//Update filter
					var contactService = services.GetService<ContactService>();

					Contacts = contactService.Contacts.Where((contact) =>
					{
						//use levenstein distance to filter contacts
						return contact.Name.ToLowerInvariant().Contains(enteredText.ToLowerInvariant()) ||
							   contact.Address.ToLowerInvariant().Contains(enteredText.ToLowerInvariant()) ;

					}).ToList();

					GoToPage(0);

				}
			}


			_previousText = enteredText;

			base.Update(gameTime, services);
		}


		private void GoToPage(int page)
		{
			// 9 contacts per page.
			var contactsOnPage = Contacts.Skip((page - 1) * 9).Take(9).ToList();

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


		private static int EditDistance(string source1, string source2) 
		{
			var source1Length = source1.Length;
			var source2Length = source2.Length;

			var matrix = new int[source1Length + 1, source2Length + 1];

			// First calculation, if one entry is empty return full length
			if (source1Length == 0)
				return source2Length;

			if (source2Length == 0)
				return source1Length;

			// Initialization of matrix with row size source1Length and columns size source2Length
			for (var i = 0; i <= source1Length; matrix[i, 0] = i++) { }
			for (var j = 0; j <= source2Length; matrix[0, j] = j++) { }

			// Calculate rows and collumns distances
			for (var i = 1; i <= source1Length; i++)
			{
				for (var j = 1; j <= source2Length; j++)
				{
					var cost = (source2[j - 1] == source1[i - 1]) ? 0 : 1;

					matrix[i, j] = Math.Min(
						Math.Min(matrix[i - 1, j] + 1, matrix[i, j - 1] + 1),
						matrix[i - 1, j - 1] + cost);
				}
			}
			// return result
			return matrix[source1Length, source2Length];
		}
	}
}
