using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DogecoinTerminal.Common.old.Components;
using DogecoinTerminal.Common.old;
using Microsoft.Xna.Framework;
using DogecoinTerminal.old;

namespace DogecoinTerminal.old.Pages
{
    internal class TerminalSettingsPage : AppPage
    {

        public TerminalSettingsPage(Game game)
            : base(game, true)
        {
            Interactables.Add(new AppText("Terminal Settings", TerminalColor.White, 5, (50, 10)));



            Interactables.Add(
                new AppButton("Update Op. Pin", (30, 40), (49, 55),
                              TerminalColor.DarkGrey, TerminalColor.White, 4,
                              (isFirst, self) =>
                              {
                                  Game.Services.GetService<Router>().Route("pin", new PinCodePageSettings("Enter New Pin:", false), true,
                                    (newPin) =>
                                    {
                                        Game.Services.GetService<Router>().Route("pin", new PinCodePageSettings("Confirm Pin:", false), true,
                                        (confirmPin) =>
                                        {
                                            if (string.IsNullOrEmpty(newPin) || newPin != confirmPin)
                                            {
                                                Game.Services.GetService<Router>().Route("msg", "Could not update pin!", true);
                                            }
                                            else
                                            {
                                                Game.Services.GetService<ITerminalService>().UpdateOperatorPin(newPin);

                                                Game.Services.GetService<Router>().Route("msg", "Operator Pin Updated!", true);
                                            }
                                        });
                                    });
                              }));

            Interactables.Add(
                new AppButton("Transaction\n"
                             + "  Settings", (51, 40), (70, 55),
                              TerminalColor.DarkGrey, TerminalColor.White, 4,
                              (isFirst, self) =>
                              {
                                  Game.Services.GetService<Router>().Route("transactionsettings", null, true);
                              }));


            Interactables.Add(
                new AppButton("Remove Wallet", (30, 59), (49, 74),
                              TerminalColor.Red, TerminalColor.White, 4,
                              (isFirst, self) =>
                              {
                                  Game.Services.GetService<Router>().Route("pin", new PinCodePageSettings("Enter Slot Number to Remove (1,2,3,4,5,6):", false, "[123456]"), true, (input) =>
                                  {
                                      int slotNumber = -1;

                                      if (int.TryParse(input, out slotNumber) && slotNumber >= 1 && slotNumber <= 6)
                                      {
                                          Game.Services.GetService<ITerminalService>().ClearSlot(slotNumber - 1);
                                          Game.Services.GetService<Router>().Route("msg", $"Cleared out Slot #{slotNumber}!", true);
                                      }
                                      else
                                      {
                                          Game.Services.GetService<Router>().Route("msg", $"Ummm? I said 1-6, you gave me: '{slotNumber}'", true);
                                      }

                                  });

                              }));
        }


        public override void OnBack()
        {
            Game.Services.GetService<Router>().ClearCallbackStack();
        }

        protected override void OnNav(dynamic value, bool backable)
        {

        }
    }
}
