﻿using DogecoinTerminal.Common.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DogecoinTerminal.Common;
using Microsoft.Xna.Framework;

namespace DogecoinTerminal.Pages
{
	internal class WalletListPage : AppPage
	{


		private AppButton[] SlotButtons;



		public WalletListPage(Game game)
			:base(game, true)
		{
			SlotButtons = new AppButton[6];


			Interactables.Add(
				new AppText("Select a Wallet/Slot:", TerminalColor.White, 6, (50, 20))
				) ;



			Interactables.Add(
				new AppButton("Lock", (2, 88), (12, 98),
					TerminalColor.DarkGrey, TerminalColor.White, 3,
					(isFirst, self) =>
					{
						Game.Services.GetService<ITerminalService>().Lock();
						Game.Services.GetService<Router>().Route("home", null, false);
					}));

			Interactables.Add(
				new AppButton("Settings", (88, 88), (98, 98),
					TerminalColor.DarkGrey, TerminalColor.White, 3,
					(isFirst, self) =>
					{
						Game.Services.GetService<Router>().Route("settings", null, true);
					}));
		}


		public void AddSlotButtons()
		{
			AddSlotButton(0, (15, 40), (35, 60));
			AddSlotButton(1, (40, 40), (60, 60));
			AddSlotButton(2, (65, 40), (85, 60));
			AddSlotButton(3, (15, 65), (35, 85));
			AddSlotButton(4, (40, 65), (60, 85));
			AddSlotButton(5, (65, 65), (85, 85));
		}

		public void AddSlotButton(int slotNumber, (int x, int y) start, (int x, int y) end)
		{
			var terminalService = Game.Services.GetService<ITerminalService>();
			var router = Game.Services.GetService<Router>();

			var slot = terminalService.GetWalletSlot(slotNumber);

			SlotButtons[slotNumber] =
				new AppButton(slot.IsEmpty ? $"(Slot {slotNumber})" : GetShortAddress(slot.Address), start, end,
							slot.IsEmpty ? TerminalColor.LightGrey : TerminalColor.DarkGrey,
							TerminalColor.White, 5,
							(isFirst, self) =>
							{
								if(slot.IsEmpty)
								{
									router.Route("pin", new PinCodePageSettings("Enter Pin", false), true,
									(enteredPin) =>
									{
										if (string.IsNullOrEmpty(enteredPin))
										{
											return;
										}

										router.Route("pin", new PinCodePageSettings("Confirm Pin", false), true,
										(confirmPin) =>
										{
											if (enteredPin != confirmPin)
											{
												return;
											}

											router.Route("msg", "Prepare to write down your backup phrases.", true, _ =>
											{
												slot.Init(enteredPin);

												router.Route("codes", slot.GetMnemonic(), true, _ =>
												{
													router.Route("wallet", slotNumber, true);
												});
											});

										});
									});
								}
								else
								{
									router.Route("pin", new PinCodePageSettings("Enter Pin", false), true,
												(enteredPin) =>
												{
													if(slot.Unlock(enteredPin))
													{
														router.Route("wallet", slotNumber, true);
													}
												});
								}

							});

		}


		private string GetShortAddress(string address)
		{
			var builder = new StringBuilder();
			builder.Append(address.Substring(0, 4));
			builder.Append("..");
			builder.Append(address.Substring(address.Length - 3, 3));
			return builder.ToString();
		}


		private void RefreshSlots()
		{
			if (SlotButtons[0] != null)
			{
				//refresh the slot buttons
				foreach (var slotButton in SlotButtons)
				{
					Interactables.Remove(slotButton);
				}
			}

			AddSlotButtons();


			//refresh the slot buttons
			foreach (var slotButton in SlotButtons)
			{
				Interactables.Add(slotButton);
			}
		}

		public override void OnBack()
		{
			RefreshSlots();
		}

		protected override void OnNav(dynamic value, bool backable)
		{
			Game.Services.GetService<Router>().ClearCallbackStack();
			RefreshSlots();
		}
	}
}
