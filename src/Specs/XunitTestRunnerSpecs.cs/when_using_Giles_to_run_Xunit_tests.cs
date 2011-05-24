using System;
using System.Linq;
using Xunit;

namespace Giles.Tests.Runner.Xunit
{
    public class when_using_Giles_to_run_Xunit_tests {

        [Fact]
        public void it_should_work() {
            Assert.True(true); 
        }

        [Fact]
        public void it_should_log_a_failing_test() {
            Assert.True(false);
        }

        [Fact(Skip="Should skip this one")]
        public void it_should_skip_a_test_when_told() {
            Assert.True(false);
        }


        [Fact]
        public void it_should_load_the_xunit_assembly_into_the_AppDomain() {
            Assert.True(AppDomain.CurrentDomain.GetAssemblies().Any(a=> a.FullName.Contains("xunit")));
            
        }
    }
}
