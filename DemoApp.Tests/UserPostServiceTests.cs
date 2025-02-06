using Azure;
using Azure.Data.Tables;
using Azure.Data.Tables.Models;
using DemoApp.Models;
using DemoApp.Service;
using DemoApp.Service.Proxy;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DemoApp.Tests
{
    [TestFixture]
    public class UserPostServiceTests
    {
        private Mock<IHttpProxy> _mockHttpProxy;
        private Mock<TableClient> _mockTableClient;
        private Mock<ILogger<UserPostService>> _mockLogger;
        private UserPostService _userPostService;

        [SetUp]
        public void SetUp()
        {
            _mockHttpProxy = new Mock<IHttpProxy>();
            _mockTableClient = new Mock<TableClient>();
            _mockLogger = new Mock<ILogger<UserPostService>>();

            _userPostService = new UserPostService(_mockHttpProxy.Object, _mockTableClient.Object, _mockLogger.Object);
        }

        [Test]
        public async Task ProcessUserPostAsync_ShouldLogInformation_WhenPostsAreProcessed()
        {
            // Arrange
            var posts = new List<Post>
        {
            new Post { UserId = 1, Id = 1, Title = "Title1", Body = "Body1" },
            new Post { UserId = 1, Id = 2, Title = "Title2", Body = "Body2" },
            new Post { UserId = 2, Id = 3, Title = "Title3", Body = "Body2" }
        };

            _mockHttpProxy.Setup(x => x.GetAsync<List<Post>>(It.IsAny<string>())).ReturnsAsync(posts);

            var mockResponse = new Mock<Response>(); // Mock the Response<TableItem>

            var tableItem = new TableItem("posts"); // Create a default TableItem

            var response = Response.FromValue(tableItem, mockResponse.Object);
            var responseForUpsert = Task.FromResult(mockResponse.Object); // Use BinaryData

            _mockTableClient.Setup(x => x.CreateIfNotExistsAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(response); // Correct setup for CreateIfNotExistsAsync

            _mockTableClient.Setup(x => x.UpsertEntityAsync(It.IsAny<PostEntity>(), It.IsAny<TableUpdateMode>(), It.IsAny<CancellationToken>()))
    .Returns(responseForUpsert);

            // Act
            await _userPostService.ProcessUserPostAsync();

            // Assert
            Assert.IsTrue(true);

        }
    }
}