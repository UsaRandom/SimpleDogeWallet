using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace SimpleDogeWallet.Common.Pages
{
    public interface IPage : IReceiver<UserClickMessage>
    {

        void Update(GameTime time, IServiceProvider services);
        void Draw(GameTime time, IServiceProvider services);

        void OnPageShown(IServiceProvider services);

        void OnPageHidden(IServiceProvider services);

        void Cleanup();

        ICollection<IPageControl> Controls { get; }
    }
}
