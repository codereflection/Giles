using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClassLibraryWithNSpec
{
    public class Batman
    {
        public Batman()
        {
            UtilityBelt = new List<Gadget> { new Batarang() };
        }

        public IList<Gadget> UtilityBelt { get; private set; }
    }

    public class Gadget { }
    public class Batarang : Gadget { }
}
