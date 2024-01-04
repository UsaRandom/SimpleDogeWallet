using System;

namespace DogecoinTerminal.Common.Pages
{
    public class PageDefAttribute : Attribute
    {

        public PageDefAttribute(string fileName)
        {
            FileName = fileName;
        }


        public string FileName
        {
            get;
            private set;
        }
    }
}
