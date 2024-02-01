using System.Collections.Generic;

namespace DogecoinTerminal.Common
{
	public class Language
	{
		public string Name { get; private set; }
		public string LanguageCode { get; private set; }

		static Language()
		{
			var languages = new Dictionary<string, Language>
			{
				{ "en", new Language { LanguageCode = "en", Name = "English" } }
			};

			Languages = languages;
		}

		public static IReadOnlyDictionary<string, Language> Languages { get; private set; }
	}

}
