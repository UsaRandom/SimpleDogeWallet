namespace DogecoinTerminal.Common.Pages
{
    public class PromptResult
    {
        public PromptResult(PromptResponse userResponse, object value = null)
        {
            Response = userResponse;
            Value = value;
        }

        public PromptResponse Response { get; private set; }
        public object Value { get; private set; }
    }
}
