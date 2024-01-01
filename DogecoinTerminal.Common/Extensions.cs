using System;

namespace DogecoinTerminal.Common
{
	public static class Extensions
	{
		public static T GetService<T>(this IServiceProvider serviceProvider) where T : class
		{
			return (T)serviceProvider.GetService(typeof(T));
		}

	}
}
