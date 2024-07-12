using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

namespace SimpleDogeWallet.Common
{
	// TODO: need way for a plugin/whatever to provide strings...
	public class Strings
	{
		private static Strings _current;
		private Dictionary<string, string> _strings = new();


		

		static Strings()
		{
			Current = new Strings();
		}


		public void SelectLanguage(Language language)
		{
			_strings.Clear();

			Language = language;

			var stringFiles = Directory.GetFiles("Strings");
			var prefix = language.LanguageCode + ".";

			foreach (string stringFile in stringFiles)
			{
				if(Path.GetFileName(stringFile).StartsWith(prefix))
				{
					var root = XElement.Load(stringFile);

					foreach(var stringDef in root.Elements())
					{
						_strings.Add(stringDef.Attribute("Id").Value, stringDef.Value);
					}
				}
			}
		}

		public static Strings Current
		{
			get
			{
				return _current;
			}
			internal set
			{
				_current = value;
			}
		}


		public Language Language
		{
			get;
			private set;
		}


		public string GetString(string key)
		{
			return this[key];
		}

		public string this[string index]
		{
			get
			{
				return _strings.GetValueOrDefault(index) ?? index;
			}
		}
	}
}
