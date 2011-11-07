using System.Collections.Generic;
using System.Linq;
using Giles.Core.Utility;
using Machine.Specifications;

namespace Giles.Specs.Core.Utility
{
    interface IHaveAChicken
    {
    }

    class Coop : IHaveAChicken
    {
    }

    [Subject(typeof(AssemblyExtensions))]
    public class when_getting_instances_of_T_from_assembly_by_interface
    {
        static string path;
        static IEnumerable<IHaveAChicken> result;

        Establish context = () =>
            path = typeof(Coop).Assembly.Location;

        Because of = () =>
            result = AssemblyExtensions.FromAssemblyGetInstancesOfType<IHaveAChicken>(path);

        It should_return_the_types_that_implement_that_interface = () =>
            {
                result.Count().ShouldEqual(1);
                result.First().ShouldBeOfType<Coop>();
            };
    }
}