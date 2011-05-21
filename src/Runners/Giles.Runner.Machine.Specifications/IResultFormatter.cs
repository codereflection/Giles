using System;
using Machine.Specifications;
using Machine.Specifications.Runner;

namespace Giles.Runner.Machine.Specifications
{
    public interface IResultFormatter
    {
        string FormatResult(SpecificationInfo specification, Result result);
    }

    public class PassedResultFormatter : IResultFormatter
    {
        public string FormatResult(SpecificationInfo specification, Result result)
        {
            return String.Format("\t» {0}", specification.Name);
        }
    }

    public class FailedResultFormatter : IResultFormatter
    {
        public string FormatResult(SpecificationInfo specification, Result result)
        {
            return String.Format("\t» {0} (FAIL)\n{1}", specification.Name, result.Exception);
        }
    }

    public class NotImplementedResultFormatter : IResultFormatter
    {
        public string FormatResult(SpecificationInfo specification, Result result)
        {
            return String.Format("\t» {0} (NOT IMPLEMENTED)", specification.Name);
        }
    }

    public class IgnoredResultFormatter : IResultFormatter
    {
        public string FormatResult(SpecificationInfo specification, Result result)
        {
            return String.Format("\t» {0} (IGNORED)", specification.Name);
        }
    }

}