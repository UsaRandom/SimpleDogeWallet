using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace SimpleDogeWallet
{
	public class ContactService
	{
		public string CONTACT_LIST_FILE = "contacts.json";

		private IList<Contact> _contacts;

		public ContactService()
		{
			_contacts = new List<Contact>();
			Load();
		}



		public IList<Contact> Contacts
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

		public void RemoveContact(Contact contact)
		{
			_contacts.Remove(contact);
			Save();
		}

		public void UpdateContact(Contact oldContact, Contact newContact)
		{
			_contacts.Remove(oldContact);
			_contacts.Add(newContact);
			Save();
		}


		public void Save()
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
