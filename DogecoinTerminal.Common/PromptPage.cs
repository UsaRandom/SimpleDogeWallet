namespace DogecoinTerminal.Common
{
	public abstract class PromptPage : Page
	{

		public PromptPage(IPageOptions options) : base(options)
		{
		}



		protected virtual bool CanCancel()
		{
			return true;
		}

		public void Cancel(object result = null)
		{
			if (CanSubmit())
			{
				Messenger.Default.Send(new PromptResult(PromptResponse.NoCancelBack, result));
			}
		}

		protected virtual bool CanSubmit()
		{
			return true;
		}

		public void Submit(object result = null)
		{
			if (CanSubmit())
			{
				Messenger.Default.Send(new PromptResult(PromptResponse.YesConfirm, result));
			}
		}

	}

}
