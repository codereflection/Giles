using System;
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
    }
}
