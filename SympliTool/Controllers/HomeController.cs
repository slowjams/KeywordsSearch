using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using SympliTool.Models;
using System.Net.Http;
using System.Text.RegularExpressions;

namespace SympliTool.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHttpClientFactory httpClientFactory;
        private IMemoryCache cache;
        private IEnumerable<ISearchEngine> searchEngines;

        public HomeController(IHttpClientFactory httpClientFactory, IMemoryCache cache, IEnumerable<ISearchEngine> searchEngines)
        {
            this.httpClientFactory = httpClientFactory;
            this.cache = cache;
            this.searchEngines = searchEngines;
        }

        public ActionResult Index()
        {
            var engineNames = searchEngines.Select(s => s.Name);
            return View("Index", string.Join(", ", engineNames));
        }

        public async Task<ActionResult> Search(string keywords, string url)
        {
            List<Result> resultModels = new List<Result>();

            foreach (var s in searchEngines)
            {
                var result = cache.Get<string>(s.Name);

                if (result == null)
                {
                    var client = httpClientFactory.CreateClient();
                    result = await client.GetStringAsync($"{s.GetSearchString}{keywords}");
                    setCache(result, s.Name);
                }

                var occurrenceList = result.Split("|").Select((x, index) =>
                    new {
                        Value = x,
                        Index = index,
                        Contains = x.Contains(url)
                    }).Where(x => x.Contains)
                      .Select(x => x.Index);

                resultModels.Add(new Result { OccurrenceList = occurrenceList, Name = s.Name }); 
            }

            return View("Search", resultModels);
            /*
            var result = cache.Get<string>("Result");

            if (result == null)
            {
                var client = httpClientFactory.CreateClient();
                result = await client.GetStringAsync($"{searchEngine.GetSearchString}{keywords}");
                setCache(result);
            }

            Regex rgx = new Regex(url);
            
            return View("Search", rgx.Matches(result).Count);
            */
        }

        private void setCache(string result, string engineName)
        {
            MemoryCacheEntryOptions cacheExpirationOptions = new MemoryCacheEntryOptions();
            cacheExpirationOptions.AbsoluteExpiration = DateTime.Now.AddHours(1);
            cacheExpirationOptions.Priority = CacheItemPriority.Normal;
            cache.Set(engineName, result, cacheExpirationOptions);
        }
    }
}
