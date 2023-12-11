using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DogecoinTerminal
{
	internal class Router
	{
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

		public void Route(string path, object value, bool backable)
		{
			var nextPage = _pages[path];

			if(backable)
			{
				_backStack.Push(_currentPage);
			}

			_currentPage = nextPage;

			_currentPage.OnNav(value);
		}


		public AppPage GetPage()
		{
			return _currentPage;
		}

		public void Back()
		{
			_currentPage = _backStack.Pop();

			_currentPage.OnBack();
		}


		public void Return(object value)
		{
			_currentPage = _backStack.Pop();

			_currentPage.OnReturned(value);
		}
	}
}
