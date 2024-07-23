
using Microsoft.Xna.Framework;
using System;

namespace SimpleDogeWallet.Common
{
    public interface IPageControl
    {
        void Update(GameTime time, IServiceProvider services);
        void Draw(GameTime time, IServiceProvider services);

        void OnControlShown(IServiceProvider services);

        void OnControlHidden(IServiceProvider services);

		string Name { get; }

        bool Enabled { get; set; }

        bool ContainsPoint(Point point);

        void AcceptVisitor(IControlVisitor visitor);
    }
}
