using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FWLog.Web.Backoffice.Models.BOLogSystemCtx
{
    public class BOLogSystemColumnChangesViewModel
    {
        public string TranslatedName { get; set; }
        public string OriginalName { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
    }
}