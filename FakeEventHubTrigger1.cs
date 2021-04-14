using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.EventHubs;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Net;
using System.Data.SqlClient;
using System.Data;
using System.Diagnostics;

namespace DevBootcampPrecompiledFunctions
{
    public class FakeEventHubTrigger1
    {
        [FunctionName("FakeEventHubTrigger1")]
        public async Task Run([EventHubTrigger("eh1", Connection = "MyEventHubConn", ConsumerGroup = "$Default")] EventData[] events, ILogger log, ExecutionContext context)
        {
            var exceptions = new List<Exception>();

            foreach (EventData eventData in events)
            {
                try
                {
                    //structured logging via Ilogger:
                    var body = eventData.Body;
                    var invocationId = context.InvocationId;
                    var functionName = context.FunctionName;
                    log.LogInformation("EventBody={body}, FunctionInvocationId={invocationId}, FunctionName={functionName", body, invocationId, functionName);
                }
                catch (Exception e)
                {
                    // We need to keep processing the rest of the batch - capture this exception and continue.
                    // Also, consider capturing details of the message that failed processing so it can be processed again later.
                    exceptions.Add(e);
                }
            }

            // Once processing of the batch is complete, if any messages in the batch failed processing throw an exception so that there is a record of the failure.

            if (exceptions.Count > 1)
                throw new AggregateException(exceptions);

            if (exceptions.Count == 1)
                throw exceptions.Single();
        }
    }
}
