using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleDogeWallet.Common
{

	[Flags]
	public enum KeyboardKey
	{
		None = 0,
		Back = 1 << 0,
		LShift = 1 << 1,
		RShift = 1 << 2
	}

	public interface IUserInputService
	{
		string GetTextInput();

		bool IsKeyDown(Keys key);
	}
}
