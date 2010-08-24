using System;
using System.Collections.Generic;
using System.Text;

namespace ClassLibrary1
{
    public interface IItem
    {
        string ID
        {
            get;
            set;
        }

        string Name
        {
            get;
            set;
        }
    }
}
