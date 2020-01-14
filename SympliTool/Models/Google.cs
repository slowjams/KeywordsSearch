using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SympliTool.Models
{
    public class Google : ISearchEngine
    {
        private string searchString;
        private string name;
        private string resultDelimiter;

        public Google(string searchString, string name, string resultDelimiter)
        {
            this.searchString = searchString;
            this.name = name;
            this.resultDelimiter = resultDelimiter;
        }

        public string SearchString  
        {
            get => searchString;
            
        }

        public string Name
        {
            get => name;
        }

        public string ResultDelimiter
        {
           get => resultDelimiter;       
        }

    }
}
