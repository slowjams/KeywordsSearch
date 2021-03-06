﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SympliTool.Models
{
    public interface ISearchEngine
    {
        string SearchString
        {
            get;
        }

        string Name
        {
            get;
        }

        string ResultDelimiter
        {
            get;
        }
    }
}
