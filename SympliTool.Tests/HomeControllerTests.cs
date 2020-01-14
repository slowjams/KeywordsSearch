using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using SympliTool.Controllers;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using SympliTool.Models;
using Xunit;
using Moq;

namespace SympliTool.Tests
{
    public class HomeControllerTests
    {
        [Fact]
        public void SearchActionModelResultMatch()
        {
            var mockCache = new Mock<IMemoryCache>();
            mockCache.Setup(c => c.Set("Google", "xxx|hello|xxhello|xxxx|hellox", new MemoryCacheEntryOptions { SlidingExpiration = TimeSpan.FromHours(1) }));

            List<ISearchEngineFactory> searchEngineFactories = new List<ISearchEngineFactory> { new ITestGoogleFactory() };

            var controller = new HomeController(mockCache.Object, searchEngineFactories);
            string testKeywords = "hello";
            string url = "www.xxx.com";  // just a placeholder
            var model = (controller.Search(testKeywords, url).Result as ViewResult)?.ViewData.Model as Result[];
            Assert.Equal("2,3,5", model[0].ToString());
        }     
    }

    public class ITestGoogleFactory : ISearchEngineFactory
    {
        public ISearchEngine CreateSearchEngine()
        {
            return new Google("placeholder", "Google", "|");
        }
    }
}