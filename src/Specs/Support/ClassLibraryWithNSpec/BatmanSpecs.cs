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

        public void specify_his_utility_belt_should_have_gadgets()
        {
            before = () => bruce = new Batman();
            bruce.UtilityBelt.should_not_be_empty();
        }
    }
}
