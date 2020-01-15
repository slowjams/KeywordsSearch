using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SympliTool.Models
{
    public class HtmlParseChecker
    {
        public IEnumerable<int> ParseCheck(string result, string searchText, string delimiter)
        {
            //use LINQ to seach if the output contains the url
            var occurrenceList = result.Split(delimiter).Select((x, index) =>
                    new {
                        Value = x,
                        Index = index + 1,
                        Contains = x.Contains(searchText)
                    }).Where(x => x.Contains)
                      .Select(x => x.Index);
            return occurrenceList;
        }
    }
}
