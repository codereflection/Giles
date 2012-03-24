using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NSpec;

namespace ClassLibraryWithNSpec
{
    public class describe_Batman : nspec
    {
        Batman bruce;

        void as_the_caped_crusader()
        {
            before = () => bruce = new Batman();
            it["his utility belt should have gadgets"] = () => bruce.UtilityBelt.should_not_be_empty();
        }
    }
}
