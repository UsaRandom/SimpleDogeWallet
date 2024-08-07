﻿using System;

namespace SimpleDogeWallet.Common
{
	public interface ITerminalSettings
	{

		string GetString(string settingName, string valueIfEmpty = default);

		bool GetBool(string settingName, bool valueIfEmpty = default);

		decimal GetDecimal(string settingName, decimal valueIfEmpty = default);

		DateTime GetDateTime(string settingName, DateTime valueIfEmpty = default);
		
		int GetInt(string settingName, int valueIfEmpty = default);

		void Set(string settingName, object value);

		bool IsSet(string settingName);
	}
}
