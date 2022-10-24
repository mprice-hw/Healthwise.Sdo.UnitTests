using Azure.Messaging.EventHubs;
using Healthwise.Sdo.Events.Jira;
using Healthwise.Sdo.Functions.Services;
using Healthwise.Sdo.Functions.EventHub;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.Core.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging;

namespace Healthwise.Sdo.UnitTests.ProcessEventTests
{
    [TestClass]
    public class When_Processing_Correctly_Formatted_IssueTransitionEvent
    {
        private Mock<IStorageService> _mockStorageService;
        private Mock<Microsoft.Extensions.Logging.ILogger> _mockLogger;
        private JiraIssueTransitionEvent _issueTransitionEvent;

        [TestInitialize]
        public void InitializeTestContext()
        {
            _mockStorageService = new Mock<IStorageService>();
            _mockLogger = new Mock<Microsoft.Extensions.Logging.ILogger>();
            _issueTransitionEvent = new JiraIssueTransitionEvent()
            {
                Type = "IssueTransitionEvent",
                SourceId = "CLN-9999"
            };

        }

        [TestMethod]
        public async Task It_Should_Be_Added_To_Storage()
        {
            BinaryData eventBody = new BinaryData(_issueTransitionEvent);
            EventData eventData = new EventData(eventBody);
            EventData[] eventDataArray = { eventData };
            ProcessEvent processEvent = new ProcessEvent(_mockStorageService.Object);
            await processEvent.Run(eventDataArray, _mockLogger.Object);
            
            _mockStorageService.Verify(x => x.AddEventAsync(eventData), Times.Once());
        }
    }
}
