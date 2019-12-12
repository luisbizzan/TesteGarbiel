﻿using FWLog.Data.Models;
using FWLog.Data.Repository.CommonCtx;
using System.Collections.Generic;
using System.Linq;

namespace FWLog.Data.Repository.GeneralCtx
{
    public class PerfilUsuarioRepository : GenericRepository<PerfilUsuario>
    {
        public PerfilUsuarioRepository(Entities entities) : base(entities)
        {
        }

        public PerfilUsuario GetByUserId(string userId)
        {
            return Entities.PerfilUsuario.FirstOrDefault(f => f.UsuarioId == userId);
        }

        public IQueryable<PerfilUsuario> Tabela()
        {
            return Entities.PerfilUsuario;
        }
    }
}
