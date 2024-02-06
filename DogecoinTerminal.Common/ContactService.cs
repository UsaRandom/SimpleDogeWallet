using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace DogecoinTerminal
{
	public class ContactService
	{
		public string CONTACT_LIST_FILE = "contacts.json";

		private IList<Contact> _contacts;

		public ContactService()
		{
			_contacts = new List<Contact>();
			Load();

			if(_contacts.Count == 0)
			{
				for(var i = 0; i < 13; i++)
				{
					AddContact(new Contact
					{
						Name = "UnnamedContact" + Random.Shared.Next(),
						Address = "DLPaeuaJi2JLUcvYHD4ddLxadwnGaVSt4p"
					});
				}
			}
		}



		public IEnumerable<Contact> Contacts
		{
			get
			{
				return _contacts;
			}
		}

		public void AddContact(Contact contact)
		{
			_contacts.Add(contact);
			Save();
		}

		private void Save()
		{
			File.WriteAllText(CONTACT_LIST_FILE, JsonSerializer.Serialize(_contacts));
		}

		private void Load()
		{
			if(File.Exists(CONTACT_LIST_FILE))
			{
				try
				{
					_contacts = JsonSerializer.Deserialize<List<Contact>>(File.ReadAllText(CONTACT_LIST_FILE));
				}
				catch (Exception ex)
				{
					Debug.WriteLine("Failed to load contacts file");
					Debug.WriteLine(ex.ToString());
				}
			}
		}
	}
}
