using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SympliTool.Models
{
    //View Model
    public class Result
    {
        public IEnumerable<int> OccurrenceList
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        // to return a format string that shows the occurrence e.g "2,3,5" or "0"
        public override string ToString()
        {        
            return OccurrenceList.Count() == 0 ? "0" : string.Join(",", OccurrenceList.ToArray());
        }
    }
}
