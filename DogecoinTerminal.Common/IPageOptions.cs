namespace DogecoinTerminal.Common
{
	public interface IPageOptions
	{
		void AddOption<T>(string name, T value);
		T GetOption<T>(string name, T valueIfDefault = default(T));
	}
}
