using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FWLog.Web.Backoffice.Models.ExampleCtx
{
    public class MultiSelectModel
    {
        public string SingleValue { get; set; }
        public List<string> ListValues { get; set; }
    }
}