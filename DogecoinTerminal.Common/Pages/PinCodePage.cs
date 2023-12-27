
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using DogecoinTerminal.Common.Components;
using DogecoinTerminal.Common;
using Microsoft.Xna.Framework;
using System.Text.RegularExpressions;

namespace DogecoinTerminal.Common
{
	public class PinCodePage : AppPage
	{

		public string TypedValue { get; set; }

		public AppText UserText { get; set; }

		public AppText Title { get; set; }

		private PinCodePageSettings _settings;

		private AppButton _returnButton;


		public PinCodePage(Game game)
			: base(game, true)
		{

			Title = new AppText(string.Empty, TerminalColor.White, 6, (50, 10));

			Interactables.Add(Title);

			UserText = new AppText(string.Empty, TerminalColor.White, 5, (50, 20));

			Interactables.Add(UserText);

			_returnButton = new AppButton(">", (88, 88), (98, 98), TerminalColor.Green, TerminalColor.White, 5, (isFirst, self) =>
			{
				Game.Services.GetService<Router>().Return(UserText.Text.Replace("Đ", string.Empty));
			});

			Interactables.Add(
				new AppButton("1",
						(21, 41), (39,51),
						TerminalColor.DarkGrey,
						TerminalColor.White,
						5,
						(isFirst, self) =>
						{
							AddCharacter("1");
						}));


			Interactables.Add(
				new AppButton("2",
						(41,41), (59,51),
						TerminalColor.DarkGrey,
						TerminalColor.White,
						5,
						(isFirst, self) =>
						{

							AddCharacter("2");
						}));

			Interactables.Add(
				new AppButton("3",
						(61, 41), (79, 51),
						TerminalColor.DarkGrey,
						TerminalColor.White,
						5,
						(isFirst, self) =>
						{
							AddCharacter("3");
						}));

			Interactables.Add(
				new AppButton("4",
						(21, 53), (39, 63),
						TerminalColor.DarkGrey,
						TerminalColor.White,
						5,
						(isFirst, self) =>
						{

							AddCharacter("4");
						}));

			Interactables.Add(
				new AppButton("5",
						(41, 53), (59, 63),
						TerminalColor.DarkGrey,
						TerminalColor.White,
						5,
						(isFirst, self) =>
						{

							AddCharacter("5");
						}));

			Interactables.Add(
				new AppButton("6",
						(61, 53), (79, 63),
						TerminalColor.DarkGrey,
						TerminalColor.White,
						5,
						(isFirst, self) =>
						{

							AddCharacter("6");
						}));


			Interactables.Add(
				new AppButton("7",
						(21, 65), (39, 75),
						TerminalColor.DarkGrey,
						TerminalColor.White,
						5,
						(isFirst, self) =>
						{

							AddCharacter("7");
						}));

			Interactables.Add(
				new AppButton("8",
						(41, 65), (59, 75),
						TerminalColor.DarkGrey,
						TerminalColor.White,
						5,
						(isFirst, self) =>
						{

							AddCharacter("8");
						}));

			Interactables.Add(
				new AppButton("9",
						(61, 65), (79, 75),
						TerminalColor.DarkGrey,
						TerminalColor.White,
						5,
						(isFirst, self) =>
						{

							AddCharacter("9");
						}));



			Interactables.Add(
				new AppButton(".",
						(21, 77), (39, 87),
						TerminalColor.DarkGrey,
						TerminalColor.White,
						5,
						(isFirst, self) =>
						{

							AddCharacter(".");
						}));

			Interactables.Add(
				new AppButton("0",
						(41, 77), (59, 87),
						TerminalColor.DarkGrey,
						TerminalColor.White,
						5,
						(isFirst, self) =>
						{

							AddCharacter("0");
						}));

			Interactables.Add(
				new AppButton("<",
						(61, 77), (79, 87),
						TerminalColor.Red,
						TerminalColor.White,
						5,
						(isFirst, self) =>
						{
							DeleteCharacter();
						}));
		}


		public void DeleteCharacter()
		{
			if(UserText.Text.Length > 0)
			{
				UserText.Text = UserText.Text.Remove(UserText.Text.Length - 1);
				UpdateReturnButton();
			}
		}

		public void AddCharacter(string character)
		{
			if (_settings.IsValueMode)
			{
				if(character == "." && UserText.Text.Contains("."))
				{
					return;
				}

				UserText.Text += character;

				if (UserText.Text == "Đ.")
				{
					UserText.Text = "Đ0.";
				}

				//if (!UserText.Text.StartsWith("Đ"))
				//{
				//	UserText.Text = "Đ" + UserText.Text;
				//}
			}
			else
			{
				UserText.Text += character;
			}



			UpdateReturnButton();

		}

		private void UpdateReturnButton()
		{
			if (_settings.IsValueMode && !UserText.Text.StartsWith("Đ"))
			{
				UserText.Text = "Đ" + UserText.Text;
			}

			if (_settings.IsValueMode && double.TryParse(UserText.Text.Replace("Đ", string.Empty), out double value))
			{
				EnableReturn();
			}
			else if (!_settings.IsValueMode && Regex.IsMatch(UserText.Text, _settings.RegexRule))
			{
				EnableReturn();
			}
			else
			{
				DisableReturn();
			}
			
		}

		private void EnableReturn()
		{
			if(!Interactables.Contains(_returnButton))
			{
				Interactables.Add(_returnButton);
			}
		}

		private void DisableReturn()
		{
			Interactables.Remove(_returnButton);
		}


		public override void OnBack()
		{
			Game.Services.GetService<Router>().Back();
		}

		protected override void OnNav(dynamic value, bool backable)
		{
			_settings = value;

			UserText.Text = string.Empty;
			Title.Text = _settings.Title;

			UpdateReturnButton();
		}


	}


	public struct PinCodePageSettings
	{
		public PinCodePageSettings(string title, bool isValueMode, string regexRule = ".*")
		{
			Title = title;
			IsValueMode = isValueMode;
			RegexRule = regexRule;
		}
		public string Title;
		public bool IsValueMode;
		public string RegexRule;
	}
}
