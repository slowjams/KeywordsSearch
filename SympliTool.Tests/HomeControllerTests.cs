using System;
using System.Net;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using SympliTool.Controllers;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;
using System.Web.Http;
using System.Linq;
using SympliTool.Models;
using Xunit;
using Moq;

namespace SympliTool.Tests
{
    public class HomeControllerTests
    {
        [Fact]
        public void CanReturnOccurrenceList()
        {
            //Arrange
            var htmlParseChecker = new HtmlParseChecker();
            string mockInput = "xxx|hello|xhello|xxxx|hellxx|hello";  // "|" is the record delimiter
            string searchKeywords = "hello";
            string nonExistence = "abc";

            //Act
            var occurrenceList = htmlParseChecker.ParseCheck(mockInput, searchKeywords, "|"); 
            string result = occurrenceList.Count() == 0 ? "0" : string.Join(",", occurrenceList.ToArray());

            var occurrenceListZero = htmlParseChecker.ParseCheck(mockInput, nonExistence, "|");
            string resultZero = occurrenceListZero.Count() == 0 ? "0" : string.Join(",", occurrenceListZero.ToArray());

            //Assert
            Assert.Equal("2,3,6", result);
            Assert.Equal("0", resultZero);
        }

        [Fact]
        public void MockCacheWork()
        {
            //Arrange
            string key = "Google";
            object expected = "This is the cache output for Google";
            var memoryCache = MockMemoryCacheService.GetMemoryCache(key, expected);

            //Act
            var cachedResponse = memoryCache.Get<string>(key);

            //Assert
            Assert.Equal(expected, cachedResponse);
        }

        [Fact]
        public void MockHttpClientWork()
        {
            //Arrange
            IHttpClientFactory factory = MockHttpClientService.MockHttpClientFactory("Hello World");       
            var client = factory.CreateClient();
            var url = "http://fakeurl.com";

            //Act
            var result = client.GetStringAsync(url);

            //Assert
            Assert.Equal("Hello World", result.Result.Trim('"'));
        }

        [Fact]
        public void SearchActionWorkWithCache()
        {
            //Arrange
            List<ISearchEngineFactory> searchEngineFactories = new List<ISearchEngineFactory> { new ITestGoogleFactory() };
            string key = "Google";
            string htmlOutput = "xxx|hello<|xhello<|xxxx|hellxx|hello<";
            var memoryCache = MockMemoryCacheService.GetMemoryCache(key, htmlOutput);
            IHttpClientFactory factory = MockHttpClientService.MockHttpClientFactory("New Content After One Hour");
            var controller = new HomeController(factory, memoryCache, searchEngineFactories, new HtmlParseChecker());
            string placeholder = "placeholder";
            string searchText = "hello";

            //Act
            var model = (controller.Search(placeholder, searchText).Result as ViewResult)?.ViewData.Model as List<Result>;

            //Assert
            Assert.Equal("2,3,6", model[0].ToString());
        }
    }

    public class DelegatingHandlerStub : DelegatingHandler
    {
        private readonly Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> _handlerFunc;
        public DelegatingHandlerStub()
        {
            _handlerFunc = (request, cancellationToken) => Task.FromResult(request.CreateResponse(HttpStatusCode.OK));
        }

        public DelegatingHandlerStub(Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> handlerFunc)
        {
            _handlerFunc = handlerFunc;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return _handlerFunc(request, cancellationToken);
        }
    }

    public static class MockHttpClientService
    {
        public static IHttpClientFactory MockHttpClientFactory(string expected)
        {
            var mockFactory = new Mock<IHttpClientFactory>();
            var configuration = new HttpConfiguration();
            var clientHandlerStub = new DelegatingHandlerStub((request, cancellationToken) => {
                request.SetConfiguration(configuration);
                var response = request.CreateResponse(HttpStatusCode.OK, expected);
                return Task.FromResult(response);
            });
            var client = new HttpClient(clientHandlerStub);

            mockFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);

            return mockFactory.Object;
        }
    }

    public static class MockMemoryCacheService
    {
        public static IMemoryCache GetMemoryCache(string key, object expectedValue)
        {
            var mockMemoryCache = new Mock<IMemoryCache>();
            mockMemoryCache
                .Setup(x => x.TryGetValue(key, out expectedValue))
                .Returns(true);
            return mockMemoryCache.Object;
        }
    }

    public class ITestGoogleFactory : ISearchEngineFactory
    {
        public ISearchEngine CreateSearchEngine()
        {
            return new Google(@"https://www.google.com/search?num=100&q=", "Google", "|");
        }
    }


}