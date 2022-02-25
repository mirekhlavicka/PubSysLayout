using System;
using System.Collections.Generic;

namespace PubSysLayout.Shared.Model
{
    public partial class Server
    {
        public int IdServer { get; set; }
        public int IdLanguage { get; set; }
        public string Name { get; set; }
        public int IdSectionDefault { get; set; }
        public bool Del { get; set; }
    }
}
