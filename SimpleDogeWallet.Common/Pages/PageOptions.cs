using System.Collections;
using System.Collections.Generic;
using ZXing.OneD;

namespace SimpleDogeWallet.Common.Pages
{

    internal class PageOptions : IPageOptions
    {
        private Dictionary<string, object> _options = new();

        public PageOptions((string key, object value)[] options)
        {
            if (options != null)
            {
                foreach (var option in options)
                {
                    _options.Add(option.key, option.value);
                }
            }
        }

        public void AddOption<T>(string name, T value)
        {
            _options[name] = value;
        }

        public T GetOption<T>(string name, T valueIfDefault = default)
        {
            if (!_options.ContainsKey(name))
            {
                return valueIfDefault;
            }

            return (T)_options[name] ?? valueIfDefault;
        }
    }
}
