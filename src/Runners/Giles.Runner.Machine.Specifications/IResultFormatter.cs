using System;
using Machine.Specifications.Runner;

namespace Giles.Runner.Machine.Specifications
{
    public interface IResultFormatter
    {
        string FormatResult(SpecificationInfo specification);
    }

    public class PassedResultFormatter : IResultFormatter
    {
        public string FormatResult(SpecificationInfo specification)
        {
            return String.Format("» {0}", specification.Name);
        }
    }

    public class FailedResultFormatter : IResultFormatter
    {
        public string FormatResult(SpecificationInfo specification)
        {
            return String.Format("» {0} (FAIL)", specification.Name);
        }
    }

    public class NotImplementedResultFormatter : IResultFormatter
    {
        public string FormatResult(SpecificationInfo specification)
        {
            return String.Format("» {0} (NOT IMPLEMENTED)", specification.Name);
        }
    }

    public class IgnoredResultFormatter : IResultFormatter
    {
        public string FormatResult(SpecificationInfo specification)
        {
            return String.Format("» {0} (IGNORED)", specification.Name);
        }
    }

}