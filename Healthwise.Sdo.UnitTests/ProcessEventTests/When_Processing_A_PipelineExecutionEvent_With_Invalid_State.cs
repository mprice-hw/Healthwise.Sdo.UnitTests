using Azure.Messaging.EventHubs;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Healthwise.Sdo.UnitTests.ProcessEventTests
{
    [TestClass]
    public class When_Processing_A_PipelineExecutionEvent_With_Invalid_State
    {
        private Mock<IStorageService> _mockStorageService;
        private Mock<Microsoft.Extensions.Logging.ILogger> _mockLogger;
        private EventData _eventData;

        [TestInitialize]
        public void InitializeTestContext()
        {
            _mockStorageService = new Mock<IStorageService>();
            _mockLogger = new Mock<Microsoft.Extensions.Logging.ILogger>();

            var pipelineExecutionEvent = new
            {
                Source = "Azure DevOps",
                Type = "PipelineExecutionEvent",
                SourceId = "Name-RunId",
                Version = "1.0.0",
                Timestamp = new DateTimeOffset(DateTime.Now),
                Name = "Foobar",
                RunId = 9999,
                TeamName = "TeamName",
                ServiceName = "ServiceName",
                ServiceVersion = "1.1.1",
                RepoName = "RepoName",
                BranchName = "BranchName",
                CommitHash = "CommitHash",
                State = "Foobar",
                Result = "Uknown"
            };

            var binaryEventBody = new BinaryData(pipelineExecutionEvent);
            _eventData = new EventData(binaryEventBody);
        }

        [TestMethod]
        public async Task It_Should_Not_Pass_Validation()
        {
            var eventBody = _eventData.GetEventBody<PipelineExecutionEvent>();
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
