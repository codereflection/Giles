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
        public SessionResults RunAssembly(Assembly assembly)
        {
            var mspecAssembly = LoadMSpec(assembly);
            var exportedTypes = mspecAssembly.GetExportedTypes();
            
            MSpecTypes.Types = exportedTypes;

            var appDomainRunnerType = exportedTypes.First(x => x.Name == "AppDomainRunner");
            var specificationRunListenerType = exportedTypes.First(x => x.Name == "ISpecificationRunListener");

            var runOptionsType = exportedTypes.First(x => x.Name == "RunOptions");
            
            var sessionResults = new SessionResults();
            var testResults = new List<TestResult>();
            var resultFormatterFactory = new ResultFormatterFactory();
            
            var dynamicRunListener = Impromptu.DynamicActLike(GilesMSpecRunListener.GetAnonymousListener(sessionResults, testResults, resultFormatterFactory),
                                                     specificationRunListenerType);

            var runOptions = Activator.CreateInstance(runOptionsType, new string[] { }, new string[] { }, new string[] { });

            var runner = Activator.CreateInstance(appDomainRunnerType, dynamicRunListener, runOptions);

            runner.RunAssembly(assembly);
            return sessionResults;
        }

        private static Assembly LoadMSpec(Assembly assembly)
        {
            return Assembly.Load(assembly.GetReferencedAssemblies().FirstOrDefault(x => x.FullName.StartsWith("Machine.Specifications")));
        }

        public IEnumerable<string> RequiredAssemblies()
        {
            return new[] { Assembly.GetAssembly(typeof(MSpecRunner)).Location, "ImpromptuInterface.dll" };
        }
    }
}