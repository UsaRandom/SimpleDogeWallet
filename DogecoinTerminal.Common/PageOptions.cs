namespace DogecoinTerminal.Common
{

	internal class PageOptions : IPageOptions
	{
		private (string key, object value)[] options;

		public PageOptions((string key, object value)[] options)
		{
			this.options = options;
		}

		public void AddOption<T>(string name, T value)
		{
			throw new System.NotImplementedException();
		}

		public T GetOption<T>(string name, T valueIfDefault = default)
		{
			throw new System.NotImplementedException();
		}
	}
}
