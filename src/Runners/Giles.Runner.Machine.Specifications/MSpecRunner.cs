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
        IEnumerable<string> filters;

        public IEnumerable<string> RequiredAssemblies()
        {
            return new[] { Assembly.GetAssembly(typeof(MSpecRunner)).Location, "ImpromptuInterface.dll" };
        }

        public SessionResults RunAssembly(Assembly assembly, IEnumerable<string> filters)
        {
            this.filters = filters;
            var mspecAssembly = LoadMSpec(assembly);
            MSpecTypes.Types = mspecAssembly.GetExportedTypes();
            
            var sessionResults = new SessionResults();
            var runner = GetRunner(sessionResults);

            runner.RunAssembly(assembly);
            return sessionResults;
        }

        private dynamic GetRunner(SessionResults sessionResults)
        {
            dynamic dynamicRunListener = GetMSpecRunListener(sessionResults);

            var appDomainRunnerType = MSpecTypes.Types.First(x => x.Name == "AppDomainRunner");
            return Activator.CreateInstance(appDomainRunnerType, dynamicRunListener, GetRunOptions());
        }

        dynamic GetRunOptions()
        {
            var runOptionsType = MSpecTypes.Types.First(x => x.Name == "RunOptions");
            var includeTags = new string[] { };
            var excludeTags = new string[] { };
            return Activator.CreateInstance(runOptionsType, includeTags, excludeTags, filters);
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