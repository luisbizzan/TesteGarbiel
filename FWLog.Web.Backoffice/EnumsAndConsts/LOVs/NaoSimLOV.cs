using System.Collections.Generic;
using System.Web.Mvc;

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
                    Text = "Inativo"
                };

                yield return new SelectListItem
                {
                    Value = true.ToString(),
                    Text = "Ativo"
                };
            }
        }
    }
}