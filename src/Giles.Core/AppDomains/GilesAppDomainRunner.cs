using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Giles.Core.Runners;

namespace Giles.Core.AppDomains
{
    /// <summary>
    /// GilesAppDomainRunner - the intention is for this class to run in a separate app domain than Giles
    /// The test assembly is loaded into memory and the tests are executed
    /// </summary>
    public class GilesAppDomainRunner : MarshalByRefObject
    {
        public IEnumerable<SessionResults> Run(string testAssemblyPath)
        {            
            var testAssembly = Assembly.LoadFrom(testAssemblyPath);

            var testFrameworkRunner = new TestRunnerResolver().Resolve(testAssembly).ToList();

            var result = new List<SessionResults>();
            testFrameworkRunner.ForEach(x => result.Add(x.RunAssembly(testAssembly)));
            return result;
        }
    }
}