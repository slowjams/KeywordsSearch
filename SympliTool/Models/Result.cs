using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SympliTool.Models
{
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

        public override string ToString()
        {        
            return OccurrenceList.Count() == 0 ? "0" : string.Join(",", OccurrenceList.ToArray());
        }
    }
}
