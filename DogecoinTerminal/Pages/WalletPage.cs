using DogecoinTerminal.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DogecoinTerminal.Pages
{
    [PageDef("Pages/Xml/Wallet.xml")]
    internal class WalletPage : Page
    {
        public WalletPage(IPageOptions options) : base(options)
        {
            var slot = options.GetOption<IWalletSlot>("slot");

            var addressTextControl = GetControl<TextControl>("AddressText");
            addressTextControl.Text = slot.Address;


            var balanceTextControl = GetControl<TextControl>("BalanceText");
            balanceTextControl.Text = "Đ" + slot.CalculateBalance();

		}
    }
}
