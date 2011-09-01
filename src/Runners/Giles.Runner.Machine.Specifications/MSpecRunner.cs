using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Giles.Core.Runners;
using ImpromptuInterface;

namespace Giles.Runner.Machine.Specifications
{
    public class MSpecRunner : IFrameworkRunner
    {
        public IEnumerable<string> RequiredAssemblies()
        {
            return new[] { Assembly.GetAssembly(typeof(MSpecRunner)).Location, "ImpromptuInterface.dll" };
        }

        public SessionResults RunAssembly(Assembly assembly)
        {
            var mspecAssembly = LoadMSpec(assembly);
            MSpecTypes.Types = mspecAssembly.GetExportedTypes();
            
            var sessionResults = new SessionResults();
            var runner = GetRunner(sessionResults);

            runner.RunAssembly(assembly);
            return sessionResults;
        }

        private static dynamic GetRunner(SessionResults sessionResults)
        {
            dynamic dynamicRunListener = GetMSpecRunListener(sessionResults);

            var runOptionsType = MSpecTypes.Types.First(x => x.Name == "RunOptions");
            var runOptions = Activator.CreateInstance(runOptionsType, new string[] { }, new string[] { }, new string[] { });

            var appDomainRunnerType = MSpecTypes.Types.First(x => x.Name == "AppDomainRunner");
            return Activator.CreateInstance(appDomainRunnerType, dynamicRunListener, runOptions);
        }

        private static object GetMSpecRunListener(SessionResults sessionResults)
        {
            var specificationRunListenerType = MSpecTypes.Types.First(x => x.Name == "ISpecificationRunListener");
            return Impromptu.DynamicActLike(GilesMSpecRunListener.GetAnonymousListener(sessionResults, new List<TestResult>(), new ResultFormatterFactory()),
                                            specificationRunListenerType);
        }

        private static Assembly LoadMSpec(Assembly assembly)
        {
            return Assembly.Load(assembly.GetReferencedAssemblies().FirstOrDefault(x => x.FullName.StartsWith("Machine.Specifications")));
        }
    }
}