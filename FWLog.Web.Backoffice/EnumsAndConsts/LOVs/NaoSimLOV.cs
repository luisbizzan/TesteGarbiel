using System.Collections.Generic;
using System.Web.Mvc;
using Res = Resources.GeneralStrings;

namespace FWLog.Web.Backoffice.EnumsAndConsts.LOVs
{
    public class NaoSimLOV
    {
        public IEnumerable<SelectListItem> Items
        {
            get
            {
                yield return new SelectListItem
                {
                    Value = false.ToString(),
                    Text = Res.No
                };

                yield return new SelectListItem
                {
                    Value = true.ToString(),
                    Text = Res.Yes,
                };
            }
        }
    }
}