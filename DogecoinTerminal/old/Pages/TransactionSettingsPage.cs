using DogecoinTerminal.Common.old.Components;
using DogecoinTerminal.Common.old;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DogecoinTerminal.old.Pages
{
    internal class TransactionSettingsPage : AppPage
    {

        private AppText _feeSetting;
        private AppText _dustSetting;


        public TransactionSettingsPage(Game game)
            : base(game, true)
        {
            Interactables.Add(new AppText("Transaction Settings", TerminalColor.White, 5, (50, 10)));


            var settingsService = Game.Services.GetService<ITerminalSettingsService>();

            Interactables.Add(
                _feeSetting = new AppText("Fee per UTXO: Đ0.02", TerminalColor.White, 4, (50, 20)));

            Interactables.Add(
                new AppButton("Update Fee per UTXO", (35, 25), (65, 35),
                              TerminalColor.DarkGrey, TerminalColor.White, 4,
                              (isFirst, self) =>
                              {
                                  Game.Services.GetService<Router>().Route("pin", new PinCodePageSettings($"Fee per UTXO (default: 0.02)", true), true,
                                      (userInput) =>
                                      {
                                          Game.Services.GetService<ITerminalSettingsService>().Set("fee-per-utxo", decimal.Parse(userInput));

                                          _feeSetting.Text = $"Fee per UTXO: Đ{settingsService.Get<decimal>("fee-per-utxo")}";
                                      });
                              }));




            Interactables.Add(
                _dustSetting = new AppText("Dust Limit: Đ0.001", TerminalColor.White, 4, (50, 40)));

            Interactables.Add(
                new AppButton("Update Dust Limit", (35, 45), (65, 55),
                              TerminalColor.DarkGrey, TerminalColor.White, 4,
                              (isFirst, self) =>
                              {

                                  Game.Services.GetService<Router>().Route("pin", new PinCodePageSettings($"Dust Limit (default: 0.001):", true), true,
                                      (userInput) =>
                                      {
                                          Game.Services.GetService<ITerminalSettingsService>().Set("dust-limit", decimal.Parse(userInput));
                                          _dustSetting.Text = $"Dust Limit: Đ{settingsService.Get<decimal>("dust-limit")}";

                                      });
                              }));

        }


        public override void OnBack()
        {
            Game.Services.GetService<Router>().ClearCallbackStack();
        }

        protected override void OnNav(dynamic value, bool backable)
        {
            var settingsService = Game.Services.GetService<ITerminalSettingsService>();
            _feeSetting.Text = $"Fee per UTXO: Đ{settingsService.Get<decimal>("fee-per-utxo")}";
            _dustSetting.Text = $"Dust Limit: Đ{settingsService.Get<decimal>("dust-limit")}";
        }
    }

}
