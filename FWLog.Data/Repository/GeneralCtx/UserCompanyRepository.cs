﻿using FWLog.Data.Models;
using FWLog.Data.Models.GeneralCtx;
using FWLog.Data.Repository.CommonCtx;
using System.Collections.Generic;
using System.Linq;

namespace FWLog.Data.Repository.GeneralCtx
{
    public class UsuarioEmpresaRepository : GenericRepository<UsuarioEmpresa>
    {
        public UsuarioEmpresaRepository(Entities entities) : base(entities)
        {

        }

        public List<long> GetAllEmpresasByUserId(string userId)
        {
            return Entities.UsuarioEmpresa.Where(w => w.UserId == userId).Select(s => s.CompanyId).ToList();
        }

        public void DeleteByUserId(string userId, long idEmpresa)
        {
            var rel = Entities.UsuarioEmpresa.Where(w => w.UserId == userId && w.CompanyId == idEmpresa).FirstOrDefault();

            if (rel != null)
            {
                Entities.UsuarioEmpresa.Remove(rel);
            }
        }
    }
}
