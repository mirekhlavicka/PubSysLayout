using System;
using System.Collections.Generic;

namespace PubSysLayout.Shared.Model
{
    public partial class ModuleLocal
    {
        public int IdModule { get; set; }
        public int IdLanguage { get; set; }
        public string DesktopSrc { get; set; }
        public string MobileSrc { get; set; }

        public virtual Module IdModuleNavigation { get; set; }
    }
}
