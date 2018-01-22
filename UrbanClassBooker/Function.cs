using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

using Amazon.Lambda.Core;
using CsQuery;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace UrbanClassBooker {
    public class Function {
        private static readonly string classEndpoint = "https://urbanclimb.com.au/uc-services/member-class/login.aspx?classId={0}&url=https://urbanclimb.com.au/uc-services/member-class/enrol.aspx?classId={0}";

        public async Task FunctionHandler(InputDto input, ILambdaContext context) {
            if(String.IsNullOrWhiteSpace(input.ClassId)) {
                throw new ArgumentNullException(nameof(input.ClassId));
            }

            if(input.MemberIds == null) {
                throw new ArgumentNullException(nameof(input.MemberIds));
            }

            if(!input.MemberIds.Any()) {
                throw new ArgumentException("At least one member ID is required", nameof(input.MemberIds));
            }

            using(var client = new HttpClient()) {
                var url = String.Format(classEndpoint, input.ClassId);

                var formContent = await client.GetStringAsync(url);
                var formCq = new CQ(formContent);
                var viewState = formCq.Select("input[name=__VIEWSTATE]").Val();
                var eventValidation = formCq.Select("input[name=__EVENTVALIDATION]").Val();
                var barcodeName = formCq.Select("#cphBody_txtBarcode").Attr("name");
                var submitName = "ctl00$cphBody$btnSubmit";// formCq.Select("#cphBody_btnSubmit").Attr("name");

                var content = new Dictionary<string, string> {
                    {"__EVENTVALIDATION", eventValidation},
                    {"__VIEWSTATE", viewState},
                    {submitName, "LOGIN"}
                };

                var tasks = input.MemberIds.Select(id => Book(client, url, content, barcodeName, id));

                await Task.WhenAll(tasks);
            }
        }

        private async Task Book(HttpClient client, string url, Dictionary<string, string> content, string memberField, string memberId) {
            content[memberField] = memberId;

            using(var response = await client.PostAsync(url, new FormUrlEncodedContent(content))) {
                response.EnsureSuccessStatusCode();

                var body = await response.Content.ReadAsStringAsync();

                if(!body.Contains("Welcome back")) {
                    throw new ApplicationException($"Something is not right: \n{body}");
                }
            }
        }
    }
}
