using DogecoinTerminal.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DogecoinTerminal.Pages
{
	internal class PinCodePage : AppPage
	{

		public PinCodePage(
			)
		{

			Interactables.Add(
				new AppButton("1",
						(21, 41), (39,51),
						TerminalColor.DarkGrey,
						TerminalColor.White,
						1,
						(isFirst, self) =>
						{

						}));


			Interactables.Add(
				new AppButton("2",
						(41,41), (59,51),
						TerminalColor.DarkGrey,
						TerminalColor.White,
						1,
						(isFirst, self) =>
						{

						}));

			Interactables.Add(
				new AppButton("3",
						(61, 41), (79, 51),
						TerminalColor.DarkGrey,
						TerminalColor.White,
						1,
						(isFirst, self) =>
						{

						}));

			Interactables.Add(
				new AppButton("4",
						(21, 53), (39, 63),
						TerminalColor.DarkGrey,
						TerminalColor.White,
						1,
						(isFirst, self) =>
						{

						}));

			Interactables.Add(
				new AppButton("5",
						(41, 53), (59, 63),
						TerminalColor.DarkGrey,
						TerminalColor.White,
						1,
						(isFirst, self) =>
						{

						}));

			Interactables.Add(
				new AppButton("6",
						(61, 53), (79, 63),
						TerminalColor.DarkGrey,
						TerminalColor.White,
						1,
						(isFirst, self) =>
						{

						}));


			Interactables.Add(
				new AppButton("7",
						(21, 65), (39, 75),
						TerminalColor.DarkGrey,
						TerminalColor.White,
						1,
						(isFirst, self) =>
						{

						}));

			Interactables.Add(
				new AppButton("8",
						(41, 65), (59, 75),
						TerminalColor.DarkGrey,
						TerminalColor.White,
						1,
						(isFirst, self) =>
						{

						}));

			Interactables.Add(
				new AppButton("9",
						(61, 65), (79, 75),
						TerminalColor.DarkGrey,
						TerminalColor.White,
						1,
						(isFirst, self) =>
						{

						}));



			Interactables.Add(
				new AppButton(".",
						(21, 77), (39, 87),
						TerminalColor.DarkGrey,
						TerminalColor.White,
						1,
						(isFirst, self) =>
						{
							
						}));

			Interactables.Add(
				new AppButton("0",
						(41, 77), (59, 87),
						TerminalColor.DarkGrey,
						TerminalColor.White,
						1,
						(isFirst, self) =>
						{

						}));

			Interactables.Add(
				new AppButton("<",
						(61, 77), (79, 87),
						TerminalColor.Red,
						TerminalColor.White,
						1,
						(isFirst, self) =>
						{

						}));
		}

		public override void OnBack()
		{
			throw new NotImplementedException();
		}

		public override void OnReturned(dynamic value)
		{
			throw new NotImplementedException();
		}

		public override void Update()
		{
		}

		protected override void Draw(VirtualScreen screen)
		{
		}

		protected override void OnNav(dynamic value, bool backable)
		{

		}
	}
}
