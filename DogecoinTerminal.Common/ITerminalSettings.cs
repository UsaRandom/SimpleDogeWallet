namespace DogecoinTerminal.Common
{
	public interface ITerminalSettings
	{

		bool GetBool(string settingName, bool valueIfEmpty = default);

		decimal GetDecimal(string settingName, decimal valueIfEmpty = default);

		void Set(string settingName, object value);

		bool IsSet(string settingName);
	}
}
