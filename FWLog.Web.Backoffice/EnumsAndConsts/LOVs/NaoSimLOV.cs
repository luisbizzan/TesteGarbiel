using ExtensionMethods;
using FWLog.Data.EnumsAndConsts;
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
                    Value = NaoSimEnum.Nao.ToString(),
                    Text = NaoSimEnum.Nao.GetDisplayName(),
                };

                yield return new SelectListItem
                {
                    Value = NaoSimEnum.Sim.ToString(),
                    Text = NaoSimEnum.Sim.GetDisplayName(),
                };
            }
        }
    }
}