namespace DogecoinTerminal.Common
{
	public interface ITerminalSettingsService
	{
		T Get<T>(string settingName);

		void Set(string settingName, object value);

		bool IsSet(string settingName);
	}
}
