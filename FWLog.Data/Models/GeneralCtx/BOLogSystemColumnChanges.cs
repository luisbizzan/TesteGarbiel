using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FWLog.Data.Models.GeneralCtx
{
    public class BOLogSystemColumnChanges
    {
        public string TranslatedName { get; set; }
        public string OriginalName { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
    }
}
