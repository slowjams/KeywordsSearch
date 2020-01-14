using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SympliTool.Models
{
    public class IGoogleFactory : ISearchEngineFactory
    {
        public ISearchEngine CreateSearchEngine()
        {
            return new Google(@"https://www.google.com/search?num=100&q=", "Google", "BNeawe vvjwJb AP7Wnd");
        }
    }
}
