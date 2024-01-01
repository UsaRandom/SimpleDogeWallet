namespace DogecoinTerminal.Common
{
	public interface IReceiver<T> where T : class
    {
        void Receive(T message);
    }

}
