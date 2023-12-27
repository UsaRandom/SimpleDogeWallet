
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DogecoinTerminal.Common.Components;
using DogecoinTerminal.Common;
using Microsoft.Xna.Framework;
using static DogecoinTerminal.Common.DisplayQRPage;
using System.Diagnostics.Metrics;
using OpenCvSharp.ML;

namespace DogecoinTerminal.Pages
{
	internal class WalletPage : AppPage
	{
		private int SlotNumber;

		private List<Interactable> _walletControls; 


		public WalletPage(Game game)
			: base(game, true)
		{
			_walletControls = new List<Interactable>();


		}


		private void LoadWalletSlot(int slotNumber)
		{
			foreach(var control in _walletControls)
			{
				Interactables.Remove(control);
			}
			_walletControls = new List<Interactable>();

			SlotNumber = slotNumber;

			var terminalService = Game.Services.GetService<ITerminalService>();
			var router = Game.Services.GetService<Router>();
			var slot = terminalService.GetWalletSlot(slotNumber);
			var dogeService = Game.Services.GetService<IDogecoinService>();

			var balanceStr = slot.CalculateBalance();
			decimal balance = decimal.Parse(balanceStr);


		
			_walletControls.Add(new AppText(slot.Address, TerminalColor.White, 4, (50, 20)));

			_walletControls.Add( new AppText("Đ " + balanceStr, TerminalColor.White, 8, (50, 30)));


			_walletControls.Add(
				new AppButton("Send", (30, 40), (49, 55),
							  TerminalColor.DarkGrey, TerminalColor.White, 4,
							  (isFirst, self) =>
							  {
								  router.Route("pin", new PinCodePageSettings("Amount to Send:", true), true,
									  (dynamic value) =>
									  {
										  var amountToSend = decimal.Parse(value);

										  if(amountToSend > balance)
										  {
											  router.Route("msg", "Not enough dogecoin!", true);
											  return;
										  }

										  router.Route("scanqr", "Scan Recipient's Address", true,
											  (dynamic receipient) =>
											  {
											  //remove preceiding 'dogecoin:'
											  if (receipient.Contains(":"))
											  {
												  receipient = receipient.Split(':')[1];
											  }


											  IDogecoinTransaction transaction = slot.CreateTransaction(receipient, amountToSend);


											  router.Route("msg",
												  $"Sending: Đ {transaction.Amount.ToString("#,##0.00")}\nFrom: {transaction.From}\n"
												  + $"To: {transaction.Recipient}\nFee: Đ {transaction.Fee.ToString("#,##0.00")}\nTotal: Đ {transaction.Total.ToString("#,##0.00")}", true,
												  _ =>
													  {
															  dogeService.SendTransaction(transaction.GetRawTransaction(), slot.SlotPin,
																(Action<bool>)((sendConfirmed) =>
																{

																	if (sendConfirmed)
																	{
																		transaction.Commit();
																		slot.UTXOStore.Save();

																		LoadWalletSlot(slot.SlotNumber);
																	}
																	transaction.Dispose();
																}));
														});


											  });


									  });
							  }));

			_walletControls.Add(
				new AppButton("Receive", (51, 40), (70, 55),
							  TerminalColor.DarkGrey, TerminalColor.White, 4,
							  (isFirst, self) =>
							  {
								  Game.Services.GetService<Router>().Route("displayqr", new DisplayQRPageSettings("dogecoin:"+slot.Address, slot.Address, false), true);
							  }));

			_walletControls.Add(
				new AppButton("Update Pin", (30, 59), (49, 74),
							  TerminalColor.DarkGrey, TerminalColor.White, 4,
							  (isFirst, self) =>
							  {

								  Game.Services.GetService<Router>().Route("pin", new PinCodePageSettings("Enter New Pin:", false), true,
									  (dynamic newPin) =>
									  {
										  Game.Services.GetService<Router>().Route("pin", new PinCodePageSettings("Confirm Pin:", false), true,
										  (dynamic confirmPin) =>
										  {
											  if(newPin != confirmPin)
											  {
												  router.Route("msg", "Pins did not match!", false);
												  return;
											  }

											  dogeService.UpdatePin(slot.Address, slot.SlotPin, newPin,
												  (Action<bool>)((updatePinConfirmed) =>
												  {
													  if(updatePinConfirmed)
													  {
														  slot.UpdateSlotPin(newPin);
														  router.Route("msg", "Pin Updated!", false);
													  }
													  else
													  {
														  router.Route("msg", "Could not update Pin!", false);
													  }

												  }));
										  });
									  });
							  }));


			_walletControls.Add(
				new AppButton("Show Backup\n    Phrases", (51, 59), (70, 74),
							  TerminalColor.DarkGrey, TerminalColor.White, 4,
							  (isFirst, self) =>
							  {
								  Game.Services.GetService<Router>().Route("codes", slot.GetMnemonic(), true);
							  }));




			_walletControls.Add(
				new AppButton("Refresh Balance", (30, 78), (49, 93),
							  TerminalColor.Blue, TerminalColor.White, 4,
							  (isFirst, self) =>
							  {
								  Game.Services.GetService<IDogecoinService>().GetUTXOs(slot.Address, null, (utxos) =>
								  {
									  slot.UTXOStore.RemoveAll();
									  
									  foreach(var utxo in utxos)
									  {
										  slot.UTXOStore.AddUTXO(utxo);
									  }

									  slot.UTXOStore.Save();
									  LoadWalletSlot(slot.SlotNumber);
								  });
							  }));


			_walletControls.Add(
				new AppButton("Delete", (51, 78), (70, 93),
							  TerminalColor.Red, TerminalColor.White, 4,
							  (isFirst, self) =>
							  {
								  Game.Services.GetService<Router>().Route("pin", new PinCodePageSettings("Confirm Pin to Delete:", false), true,
									  (dynamic enteredPin) =>
									  {
										  if(enteredPin == slot.SlotPin)
										  {
											  dogeService.OnDeleteAddress(slot.Address, slot.SlotPin,
											  () =>
											  {
												  slot.ClearSlot();
												  slot.Lock();
												  router.Route("wallets", null, false);
											  });
										  }
									  });
							  }));

			foreach(var control in _walletControls)
			{
				Interactables.Add(control);
			}
		}



		private void OnUpdatePinButtonClicked(bool isFirstInteraction, Interactable sender)
		{

		}

		public override void OnBack()
		{
			Game.Services.GetService<Router>().ClearCallbackStack();


		}

		protected override void OnNav(dynamic value, bool backable)
		{
			LoadWalletSlot(value);
		}
	}
}
