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
using System.Linq;

namespace FWLog.Data.Repository.GeneralCtx
{
    public class GarantiaConfiguracaoRepository : GenericRepository<Garantia>
    {
        #region Contexto
        public GarantiaConfiguracaoRepository(Entities entities) : base(entities) { }
        #endregion

        #region [Fornecedor Quebra] - Inclusão
        public void IncluirFornecedorQuebra(GarantiaConfiguracao item)
        {
            using (var conn = new OracleConnection(Entities.Database.Connection.ConnectionString))
            {
                conn.Open();
                if (conn.State == System.Data.ConnectionState.Open)
                {
                    string sQuery = @"
                    INSERT INTO geral_historico
	                    ( Id_Categoria, Id_Ref, Id_Usr, Historico, Dt_Cad )
                    VALUES
	                    ( :Id_Categoria, :Id_Ref, :Id_Usr, :Historico, SYSDATE )
                    ";
                    conn.Query<GarantiaConfiguracao>(sQuery, new
                    {
                        item.Id,
                        item.Cod_Fornecedor
                    });
                }
                conn.Close();
            }
        }
        #endregion
    }
}
