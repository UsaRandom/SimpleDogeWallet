using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DogecoinTerminal.Common
{
    public class Messenger
    {
        private static readonly ConcurrentDictionary<Type, List<object>> _messageReceivers = new();

        public static Messenger Default { get; } = new Messenger();

        public void Register<TMessage>(IReceiver<TMessage> receiver) where TMessage : class
        {
            if (!_messageReceivers.ContainsKey(typeof(TMessage)))
            {
                _messageReceivers[typeof(TMessage)] = new List<object>();
            }

            _messageReceivers[typeof(TMessage)].Add(receiver);
        }

        public void Deregister<TMessage>(IReceiver<TMessage> receiver) where TMessage : class
        {
            if (!_messageReceivers.ContainsKey(typeof(TMessage)))
            {
                return;
            }

            _messageReceivers[typeof(TMessage)].Remove(receiver);
        }

        public void Send<TMessage>(TMessage message) where TMessage : class
        {

            if (_messageReceivers.TryGetValue(typeof(TMessage), out var receiverList))
            {
                foreach (var receiverObj in receiverList)
                {
                    Task.Run(() =>
                    {
                        var receiver = (IReceiver<TMessage>)receiverObj;

                        receiver.Receive(message);
                    });
                }
            }
        }
    }
}
