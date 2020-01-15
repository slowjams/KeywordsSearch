using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using SympliTool.Controllers;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;
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
            string mockInput = "xxx|hello|xhello|xxxx|hellxx|hello";
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

        //[Fact]
        //public void SearchActionModelResultMatch()
        //{
        //    var mockFactory = new Mock<IHttpClientFactory>();
        //    var configuration = new HttpConfiguration();
        //    var clientHandlerStub = new DelegatingHandlerStub((request, cancellationToken) => {
        //        request.SetConfiguration(configuration);
        //        var response = request.CreateResponse(HttpStatusCode.OK, expected);
        //        return Task.FromResult(response);
        //    });
        //    var client = new HttpClient(clientHandlerStub);

        //    mockFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);
        //    var mockCache = new Mock<IMemoryCache>();
        //    mockCache.Setup(c => c.Set("Google", "xxx|hello|xxhello|xxxx|hellox", new MemoryCacheEntryOptions { SlidingExpiration = TimeSpan.FromHours(1) }));

        //    List<ISearchEngineFactory> searchEngineFactories = new List<ISearchEngineFactory> { new ITestGoogleFactory() };

        //    var controller = new HomeController(mockCache.Object, searchEngineFactories);
        //    string testKeywords = "hello";
        //    string url = "www.xxx.com";  // just a placeholder
        //    var model = (controller.Search(testKeywords, url).Result as ViewResult)?.ViewData.Model as Result[];
        //    Assert.Equal("2,3,5", model[0].ToString());
        //}
    }

    public class ITestGoogleFactory : ISearchEngineFactory
    {
        public ISearchEngine CreateSearchEngine()
        {
            return new Google("placeholder", "Google", "|");
        }
    }
}