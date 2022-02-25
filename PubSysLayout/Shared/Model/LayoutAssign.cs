using System;
using System.Collections.Generic;

namespace PubSysLayout.Shared.Model
{
    public partial class LayoutAssign
    {
        public int IdServer { get; set; }
        public int IdSection { get; set; }
        public int IdQslayout { get; set; }
        public int IdLayoutdefinition { get; set; }
        public bool RefererRequired { get; set; }

        public virtual LayoutDefinition IdLayoutdefinitionNavigation { get; set; }
        public virtual Qslayout IdQslayoutNavigation { get; set; }
    }
}
