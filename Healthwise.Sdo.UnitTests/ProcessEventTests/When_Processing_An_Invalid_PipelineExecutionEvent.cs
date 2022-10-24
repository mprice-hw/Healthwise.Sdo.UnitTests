using Azure.Messaging.EventHubs;
using Healthwise.Sdo.Events.AzurePipelines;
using Healthwise.Sdo.Events.Jira;
using Healthwise.Sdo.Functions.Services;
using Healthwise.Sdo.Functions.EventHub;
using Healthwise.Sdo.Functions.Extentions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Healthwise.Sdo.Events;

namespace Healthwise.Sdo.UnitTests.ProcessEventTests
{
    [TestClass]
    public class When_Processing_An_Invalid_PipelineExecutionEvent
    {
        private Mock<IStorageService> _mockStorageService;
        private Mock<Microsoft.Extensions.Logging.ILogger> _mockLogger;
        private EventData _eventData;

        [TestInitialize]
        public void InitializeTestContext()
        {
            _mockStorageService = new Mock<IStorageService>();
            _mockLogger = new Mock<Microsoft.Extensions.Logging.ILogger>();
            
            //It's missing RunId which is required.
            var pipelineExecutionEvent = new PipelineExecutionEvent()
            {
                Source = "Azure DevOps",
                //Type = "PipelineExecutionEvent",
                //SourceId = "PipelineName-RunId",
                //Version = "1.0.0",
                //Timestamp = new DateTimeOffset(DateTime.Now),
                //Name = "Foobar",
                //TeamName = "TeamName",
                //ServiceName =  "ServiceName",
                //ServiceVersion = "1.1.1",
                //RepoName = "RepoName",
                //BranchName = "BranchName",
                //CommitHash = "CommitHash",
                State = PipelineState.InProgress,
                Result = PipelineResult.Uknown
            };

            var binaryEventBody = new BinaryData(pipelineExecutionEvent);
            _eventData = new EventData(binaryEventBody);
        }

        [TestMethod]
        public async Task It_Should_Not_Pass_Validation()
        {
            var eventBody = await _eventData.GetEventBodyAsync<PipelineExecutionEvent>();
            Assert.IsFalse(eventBody.IsValid);
        }

        [TestMethod]
        public async Task It_Should_Not_Be_Added_To_Storage()
        {
            EventData[] eventDataArray = { _eventData };
            var eventProcessor = new EventProcessor(_mockStorageService.Object);
            await eventProcessor.ProcessEvent(eventDataArray, _mockLogger.Object);

            _mockStorageService.Verify(x => x.AddEventAsync(_eventData), Times.Never());
        }
    }
}
