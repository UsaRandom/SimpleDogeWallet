using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DogecoinTerminal
{
	internal class Router
	{
		private Stack<Action<dynamic>> _callbackStack = new Stack<Action<dynamic>>();

		private Stack<AppPage> _backStack = new Stack<AppPage>();
		private IDictionary<string, AppPage> _pages;
		private AppPage _currentPage;


		public static Router Instance { get; private set; }

		public Router((string path, AppPage page)[] routes)
		{
			if(Instance != null)
			{
				throw new Exception("Singleton AHH!!!!");
			}

			_pages = new Dictionary<string, AppPage>();

			foreach(var route in routes)
			{
				_pages.Add(route.path, route.page);
			}
			Instance = this;
		}

		public void Route(string path, object value, bool backable, Action<dynamic> callback = null)
		{
			var nextPage = _pages[path];

			if(backable)
			{
				_backStack.Push(_currentPage);
			}

			if(callback != null)
			{
				_callbackStack.Push(callback);
			}

			_currentPage?.Cleanup();
			_currentPage = nextPage;

			_currentPage.OnNavigation(value, backable);
		}


		public AppPage GetPage()
		{
			return _currentPage;
		}

		public void Back()
		{
			_currentPage.Cleanup();
			_currentPage = _backStack.Pop();

			_currentPage.OnBack();
		}


		public void Return(object value)
		{
			_currentPage.Cleanup();
			_currentPage = _backStack.Pop();

			if(_callbackStack.Count > 0)
			{
				var callback = _callbackStack.Pop();

				callback.Invoke(value);
			}

		}

		public void ClearCallbackStack()
		{
			_callbackStack.Clear();
		}
	}
}
