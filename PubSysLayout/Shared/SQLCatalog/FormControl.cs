﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PubSysLayout.Shared.SQLCatalog
{
    public class FormControl
    {
        public int IdControl { get; set; }
        public int IdFControl { get; set; }
        public string Title { get; set; }
        public int DataType { get; set; }
        public bool Searchable { get; set; }
        public bool ShowInList { get; set; }
    }
}