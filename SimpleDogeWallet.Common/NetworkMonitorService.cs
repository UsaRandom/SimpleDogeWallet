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
        private Timer _spvAutoStartTimer;
        private Timer _networkChangedTimer;
        private Timer _networkChangedRemoveFlagTimer;
        private bool _networkAddressChangedEventHandled;

        private object _lock = new object();

        public NetworkMonitorService(SimpleSPVNodeService simpleSPVNodeService)
        {
            _networkChangedTimer = new Timer(HandleNetworkChange, null, Timeout.Infinite, Timeout.Infinite);
            _networkChangedRemoveFlagTimer = new Timer(ClearFlag, null, Timeout.Infinite, Timeout.Infinite);

            _spvAutoStartTimer = new Timer(CheckSPV, null, 0, (int)TimeSpan.FromSeconds(30).TotalMilliseconds);
            _simpleSPVNodeService = simpleSPVNodeService;

            NetworkChange.NetworkAddressChanged += NetworkAddressChanged;
        }




        private void CheckSPV(object e)
        {
            try
            {
                lock (_lock)
                {
                    if (!_simpleSPVNodeService.IsPaused && !_simpleSPVNodeService.IsRunning && SimpleDogeWallet.Instance != null && !_networkAddressChangedEventHandled)
                    {
                        _simpleSPVNodeService.Start();
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
            }
        }


        private void NetworkAddressChanged(object sender, EventArgs e)
        {
            lock(_lock)
            {
                if (!_networkAddressChangedEventHandled)
                {
                    _networkAddressChangedEventHandled = true;
                    _networkChangedTimer.Change(500, Timeout.Infinite);
                }
            }
        }

        private void HandleNetworkChange(object state)
        {
            lock (_lock)
            {
                _networkChangedRemoveFlagTimer.Change((int)TimeSpan.FromSeconds(5).TotalMilliseconds, Timeout.Infinite);

                if (!_simpleSPVNodeService.IsPaused)
                {
                    _simpleSPVNodeService.Stop();
                    _simpleSPVNodeService.Start();
                }
            }

        }
        private void ClearFlag(object state)
        {
            lock (_lock)
            {
                _networkAddressChangedEventHandled = false;
            }
        }



    }
}
