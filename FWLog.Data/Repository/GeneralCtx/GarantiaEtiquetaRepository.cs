using Dapper;
using ExtensionMethods.String;
using FWLog.Data.Models;
using FWLog.Data.Models.DataTablesCtx;
using FWLog.Data.Models.FilterCtx;
using FWLog.Data.Repository.CommonCtx;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;

namespace FWLog.Data.Repository.GeneralCtx
{
    public class GarantiaEtiquetaRepository : GenericRepository<Garantia>
    {
        #region Contexto
        public GarantiaEtiquetaRepository(Entities entities) : base(entities) { }
        #endregion
    }
}
