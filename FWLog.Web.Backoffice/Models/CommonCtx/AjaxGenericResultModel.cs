using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FWLog.Web.Backoffice.Models.CommonCtx
{
    public class AjaxGenericResultModel
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string Data { get; set; }
    }

}
