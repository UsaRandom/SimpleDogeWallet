using DogecoinTerminal.Common;
using DogecoinTerminal.Common.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DogecoinTerminal.Pages
{
	[PageDef("Pages/Xml/LangaugeSelectionPage.xml")]
	internal class LanguageSelectionPage : PromptPage
	{
		public LanguageSelectionPage(IPageOptions options) : base(options)
		{
			SelectedLanguageCode = Strings.Current.Language.LanguageCode;

			OnClick("SubmitButton", _ =>
			{
				Submit();
			});

			OnClick("EnglishButton", _ =>
			{
				SelectedLanguageCode = "eng";
				Strings.Current.SelectLanguage(Language.Languages[SelectedLanguageCode]);
			});

			OnClick("JapaneseButton", _ =>
			{
				SelectedLanguageCode = "jpn";
				Strings.Current.SelectLanguage(Language.Languages[SelectedLanguageCode]);
			});

			OnClick("KoreanButton", _ =>
			{
				SelectedLanguageCode = "kor";
				Strings.Current.SelectLanguage(Language.Languages[SelectedLanguageCode]);
			});

			OnClick("SpanishButton", _ =>
			{
				SelectedLanguageCode = "spa";
				Strings.Current.SelectLanguage(Language.Languages[SelectedLanguageCode]);
			});

			OnClick("ChineseSimplifiedButton", _ =>
			{
				SelectedLanguageCode = "sc";
				Strings.Current.SelectLanguage(Language.Languages[SelectedLanguageCode]);
			});

			OnClick("ChineseTraditionalButton", _ =>
			{
				SelectedLanguageCode = "ct";
				Strings.Current.SelectLanguage(Language.Languages[SelectedLanguageCode]);
			});

			OnClick("FrenchButton", _ =>
			{
				SelectedLanguageCode = "fra";
				Strings.Current.SelectLanguage(Language.Languages[SelectedLanguageCode]);
			});

			OnClick("ItalianButton", _ =>
			{
				SelectedLanguageCode = "ita";
				Strings.Current.SelectLanguage(Language.Languages[SelectedLanguageCode]);
			});

			OnClick("CzechButton", _ =>
			{
				SelectedLanguageCode = "cze";
				Strings.Current.SelectLanguage(Language.Languages[SelectedLanguageCode]);
			});

			OnClick("PortugueseButton", _ =>
			{
				SelectedLanguageCode = "por";
				Strings.Current.SelectLanguage(Language.Languages[SelectedLanguageCode]);
			});
		}

		public string SelectedLanguageCode { get; set; }
	}
}
