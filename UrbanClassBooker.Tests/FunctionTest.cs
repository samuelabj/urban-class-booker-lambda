using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Xunit;
using Amazon.Lambda.Core;
using Amazon.Lambda.TestUtilities;

using UrbanClassBooker;

namespace UrbanClassBooker.Tests {
    public class FunctionTest {
        [Fact]
        public async Task Book() {
            var function = new Function();
            var context = new TestLambdaContext();

            await function.FunctionHandler(new InputDto {
                ClassId = "a68f744f-1109-4dc9-95a5-9edf11e18883",
                MemberIds = new string[] { "92050" }
            }, context);
        }
    }
}
