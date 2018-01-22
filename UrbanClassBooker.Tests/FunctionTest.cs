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
                ClassId = "2b88fba7-898f-4bf2-be10-1b75710770e6",
                MemberIds = new string[] { "92050" }
            }, context);
        }
    }
}
