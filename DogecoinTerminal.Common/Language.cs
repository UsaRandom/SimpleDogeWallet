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
				{ "eng", new Language { LanguageCode = "eng", Name = "English" } },
				{ "jpn", new Language { LanguageCode = "jpn", Name = "Japanese" } },
				{ "kor", new Language { LanguageCode = "kor", Name = "Korean" } },
				{ "spa", new Language { LanguageCode = "spa", Name = "Spanish" } },
				{ "sc", new Language { LanguageCode = "sc", Name = "ChineseSimplified" } },
				{ "ct", new Language { LanguageCode = "ct", Name = "ChineseTraditional" } },
				{ "fra", new Language { LanguageCode = "fra", Name = "French" } },
				{ "ita", new Language { LanguageCode = "ita", Name = "Italian" } },
				{ "cze", new Language { LanguageCode = "cze", Name = "Czech" } },
				{ "por", new Language { LanguageCode = "por", Name = "Portuguese" } }
			};

			Languages = languages;
		}

		public static IReadOnlyDictionary<string, Language> Languages { get; private set; }
	}

}
