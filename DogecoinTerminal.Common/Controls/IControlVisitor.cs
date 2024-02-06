using DogecoinTerminal.Common.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DogecoinTerminal.Common
{
    public interface IControlVisitor
    {
        void VisitButton(ButtonControl buttonControl);
        void VisitImage(ImageControl imageControl);
        void VisitSprite(SpriteControl spriteControl);
        void VisitText(TextControl textControl);

        void VisitContact(ContactControl contactControl);
    }
}
