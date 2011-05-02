using System.Reflection;

namespace Giles.Core.Runners
{
    public interface IFrameworkRunner
    {
        SessionResults SessionResults(Assembly assembly);
        //TestRunState RunNamespace(ITestListener testListener, Assembly assembly, string ns);
        //TestRunState RunMember(ITestListener testListener, Assembly assembly, MemberInfo member);
    }
}