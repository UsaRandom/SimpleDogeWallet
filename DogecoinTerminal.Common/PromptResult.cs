namespace DogecoinTerminal.Common
{
	public class PromptResult
	{
		public PromptResult(PromptResponse userResponse, object value = null)
		{
			Response = userResponse;
			Value = Value;
		}

		public PromptResponse Response { get; private set; }
		public object? Value { get; private set; }
	}
}
