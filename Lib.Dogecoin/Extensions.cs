
namespace Lib.Dogecoin
{
	internal static class Extensions
	{
		public static char[] NullTerminate(this string str)
		{
			var result = new char[str.Length + 1];
			str.CopyTo(0, result, 0, str.Length);
			result[str.Length] = '\0';
			return result;
		}

		public static string TerminateNull(this char[] chars)
		{
			int nullIndex = Array.IndexOf(chars, '\0');

			nullIndex = (nullIndex == -1) ? chars.Length : nullIndex;
			return new string(chars, 0, nullIndex);
		}
	}
}
