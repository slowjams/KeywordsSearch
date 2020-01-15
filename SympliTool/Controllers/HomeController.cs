using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using SympliTool.Models;
using System.Net.Http;

namespace SympliTool.Controllers
{
    public class HomeController : Controller
    {
        private IMemoryCache cache;
        private IEnumerable<ISearchEngineFactory> searchEngineFactories;
        private IHttpClientFactory clientFactory;

        public HomeController(IHttpClientFactory clientFactory, IMemoryCache cache, IEnumerable<ISearchEngineFactory> searchEngineFactories)
        {
            this.cache = cache;
            this.searchEngineFactories = searchEngineFactories;
            this.clientFactory = clientFactory;
        }

        public ActionResult Index()
        {
            var engineNames = searchEngineFactories.Select(s => s.CreateSearchEngine().Name);
            return View("Index", string.Join(", ", engineNames));
        }

        public async Task<ActionResult> Search(string keywords, string url)
        {
            List<Result> resultModels = new List<Result>();

            foreach (var s in searchEngineFactories)
            {
                ISearchEngine searchEngine = s.CreateSearchEngine();

                var result = cache.Get<string>(searchEngine.Name);

                if (result == null)
                {
                    var client = clientFactory.CreateClient();
                    result = await client.GetStringAsync($"{searchEngine.SearchString}{keywords}");
                    setCache(result, searchEngine.Name);
                }
              
                var htmlParseChecker = new HtmlParseChecker();
                var occurrenceList = htmlParseChecker.ParseCheck(result, url + "<", searchEngine.ResultDelimiter);

                resultModels.Add(new Result { OccurrenceList = occurrenceList, Name = searchEngine.Name });

                if (resultModels.Count == 0)
                {
                    resultModels.Add(new Result { Name = searchEngine.Name });
                }
            }
            return View("Search", resultModels);
        }

        private void setCache(string result, string engineName)
        {
            MemoryCacheEntryOptions cacheExpirationOptions = new MemoryCacheEntryOptions();
            cacheExpirationOptions.AbsoluteExpiration = DateTime.Now.AddHours(1);
            cacheExpirationOptions.Priority = CacheItemPriority.Normal;
            cache.Set(engineName, result, cacheExpirationOptions);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier, StatusCode = HttpContext.Response.StatusCode });
        }
    }
}
