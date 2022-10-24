using Azure.Messaging.EventHubs;
using Healthwise.Sdo.Events.AzurePipelines;
using Healthwise.Sdo.Events.Jira;
using Healthwise.Sdo.Functions.EventHub;
using Healthwise.Sdo.Functions.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Healthwise.Sdo.UnitTests.ProcessEventTests
{
    [TestClass]
    public class When_Processing_Correctly_Formatted_PipelineExecutionEvent
    {
        private Mock<IStorageService> _mockStorageService;
        private Mock<Microsoft.Extensions.Logging.ILogger> _mockLogger;
        private PipelineExecutionEvent _pipelineExecutionEvent;

        [TestInitialize]
        public void InitializeTestContext()
        {
            _mockStorageService = new Mock<IStorageService>();
            _mockLogger = new Mock<Microsoft.Extensions.Logging.ILogger>();
            _pipelineExecutionEvent = new PipelineExecutionEvent()
            {
                Type = "PipelineExecutionEvent",
                SourceId = "PipelineName-9999"
            };

        }

        [TestMethod]
        public async Task It_Should_Be_Added_To_Storage()
        {
            BinaryData eventBody = new BinaryData(_pipelineExecutionEvent);
            EventData eventData = new EventData(eventBody);
            EventData[] eventDataArray = { eventData };
            ProcessEvent processEvent = new ProcessEvent(_mockStorageService.Object);
            await processEvent.Run(eventDataArray, _mockLogger.Object);

            _mockStorageService.Verify(x => x.AddEventAsync(eventData), Times.Once());
        }
    }
}
