using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;

namespace SimpleDogeWallet.Common
{
    public class NetworkMonitorService
    {
        private SimpleSPVNodeService _simpleSPVNodeService;
        private Timer _timer;
        
        public NetworkMonitorService(SimpleSPVNodeService simpleSPVNodeService)
        {
            _timer = new Timer(CheckSPV, null, 0, (int)TimeSpan.FromSeconds(30).TotalMilliseconds);
            _simpleSPVNodeService = simpleSPVNodeService;

            NetworkChange.NetworkAddressChanged += NetworkAddressChanged;
        }


        private void CheckSPV(object e)
        {
            try
            {
                if (!_simpleSPVNodeService.IsPaused && !_simpleSPVNodeService.IsRunning && SimpleDogeWallet.Instance != null)
                {
                    _simpleSPVNodeService.Start();
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
            }
        }


        private void NetworkAddressChanged(object sender, EventArgs e)
        {
            if(!_simpleSPVNodeService.IsPaused)
            {
                _simpleSPVNodeService.Stop();
                _simpleSPVNodeService.Start();

            }
        }



    }
}
