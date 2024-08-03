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
				var feeEstimate = _services.GetService<SimpleSPVNodeService>().EstimatedRate * 226 * _services.GetService<ITerminalSettings>().GetDecimal("fee-coeff");

				return Math.Max(feeEstimate, _services.GetService<ITerminalSettings>().GetDecimal("dust-limit"));

//				return _services.GetService<SimpleSPVNodeService>().EstimatedRate * 226 * _services.GetService<ITerminalSettings>().GetDecimal("fee-coeff");
			}
		}

	}
}
