using DogecoinTerminal.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DogecoinTerminal.Pages
{
	[PageDef("Pages/Xml/BackupCode.xml")]
	internal class BackupCodePage : Page
	{
		//We can pull in services just by adding to constructor
		public BackupCodePage(IPageOptions options, Navigation navigation) : base(options)
		{
			var mnemonic = options.GetOption<string>("mnemonic");
			

			var words = mnemonic.Split(' ');

			for (int i = 0; i < words.Length; i++)
			{
				var wordControl = GetControl<ButtonControl>($"BackupWordButton_{i}");

				wordControl.Text = $"{i+1}. {words[i]}";
			}


			OnClick("BackButton", _ => {


				//this creates some fliker... i'll have to address later.
				navigation.Pop();

			});


			OnClick("NextButton", _ => {

				navigation.Pop();

			});


		}


	}
}
