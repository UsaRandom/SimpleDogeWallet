using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleDogeWallet.Common
{
	public class FeeEstimator
	{
		private IServiceProvider _services;

		public FeeEstimator(IServiceProvider services)
		{
			_services = services;
		}

		public decimal EstimatedFee
		{
			get
			{
				return _services.GetService<SimpleSPVNodeService>().EstimatedRate * 226 * _services.GetService<ITerminalSettings>().GetDecimal("fee-coeff");
			}
		}

	}
}
