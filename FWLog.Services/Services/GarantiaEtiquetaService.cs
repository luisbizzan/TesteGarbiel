﻿using FWLog.Data;
using FWLog.Data.Models;
using System.Collections.Generic;

namespace FWLog.Services.Services
{
    public class GarantiaEtiquetaService
    {
        #region Variáveis Contexto
        private readonly UnitOfWork _uow;
        public GarantiaEtiquetaService(UnitOfWork uow)
        {
            _uow = uow;
        }
        #endregion
    }
}