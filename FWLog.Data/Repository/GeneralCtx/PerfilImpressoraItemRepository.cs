﻿using FWLog.Data.Models;
using FWLog.Data.Repository.CommonCtx;
using System.Collections.Generic;
using System.Linq;

namespace FWLog.Data.Repository.GeneralCtx
{
    public class PerfilImpressoraItemRepository : GenericRepository<PerfilImpressoraItem>
    {
        public PerfilImpressoraItemRepository(Entities entities) : base(entities)
        {

        }

        public List<PerfilImpressoraItem> ObterPorIdPerfilImpressora(long idPerfilImpressora)
        {
            return Entities.PerfilImpressoraItem.AsNoTracking().Where(w => w.IdPerfilImpressora == idPerfilImpressora).ToList();
        }

        public void Delete(List<PerfilImpressoraItem> itens)
        {
            foreach (var item in itens)
            {
                var perfilImpressoraItem = Entities.PerfilImpressoraItem.FirstOrDefault(f => f.IdImpressaoItem == item.IdImpressaoItem && f.IdImpressora == item.IdImpressora && f.IdPerfilImpressora == item.IdPerfilImpressora);

                Entities.PerfilImpressoraItem.Remove(perfilImpressoraItem);
            }
        }
    }
}
