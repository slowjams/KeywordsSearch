using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SympliTool.Models
{
    public class Google : ISearchEngine
    {
        public string GetSearchString  
        {
            get => @"https://www.google.com/search?num=100&q=";
           
        }

        public string Name
        {
            get => "Google";
        }

        public string ResultDelimiter
        {
            get => "BNeawe vvjwJb AP7Wnd";
        }

    }
}
