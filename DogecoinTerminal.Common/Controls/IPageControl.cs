
using Microsoft.Xna.Framework;
using System;

namespace DogecoinTerminal.Common
{
    public interface IPageControl
    {
        void Update(GameTime time, IServiceProvider services);
        void Draw(GameTime time, IServiceProvider services);

        string Name { get; }

        bool ContainsPoint(Point point);

        void AcceptVisitor(IControlVisitor visitor);
    }
}
