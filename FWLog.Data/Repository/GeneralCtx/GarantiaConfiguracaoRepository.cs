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

        #region [Fornecedor Quebra] - Validar se código de fornecedor já esta cadastrado
        private bool FornecedorQuebraPodeSerCadastrado(GarantiaConfiguracao fornecedor)
        {
            try
            {
                var _processamento = 0;
                using (var conn = new OracleConnection(Entities.Database.Connection.ConnectionString))
                {
                    conn.Open();
                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        string cmdSQL = String.Format("SELECT COUNT(1) V FROM gar_forn_quebra WHERE cod_fornecedor='{0}'", fornecedor.Cod_Fornecedor);
                       _processamento =  conn.ExecuteScalar<Int32>(cmdSQL);
                    }
                    conn.Close();
                }

                return _processamento.Equals(0) ? true : false;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region [Fornecedor Quebra] - Inclusão
        public void IncluirFornecedorQuebra(GarantiaConfiguracao fornecedor)
        {
            try
            {
                if (!FornecedorQuebraPodeSerCadastrado(fornecedor))
                    throw new Exception(String.Format("Já existe o código de fornecedor [{0}] cadastrado no sistema!", fornecedor.Cod_Fornecedor));

                using (var conn = new OracleConnection(Entities.Database.Connection.ConnectionString))
                {
                    conn.Open();
                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        string cmdSQL = String.Format("INSERT INTO gar_forn_quebra(Cod_Fornecedor) VALUES('{0}')", fornecedor.Cod_Fornecedor);
                        conn.ExecuteScalar(cmdSQL);
                    }
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion
    }
}
