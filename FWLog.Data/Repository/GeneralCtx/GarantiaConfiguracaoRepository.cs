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
        private bool FornecedorQuebraPodeSerCadastrado(string fornecedor)
        {
            try
            {
                var _processamento = 0;
                using (var conn = new OracleConnection(Entities.Database.Connection.ConnectionString))
                {
                    conn.Open();
                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        string cmdSQL = String.Format("SELECT COUNT(1) V FROM gar_forn_quebra WHERE cod_fornecedor='{0}'", fornecedor);
                        _processamento = conn.ExecuteScalar<Int32>(cmdSQL);
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

        #region [Fornecedor Quebra] AutoComplete 
        public List<GarantiaConfiguracao> FornecedorQuebraAutoComplete(string nome)
        {
            try
            {
                var select = new List<GarantiaConfiguracao>();
                using (var conn = new OracleConnection(Entities.Database.Connection.ConnectionString))
                {
                    conn.Open();
                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        string cmdSQL = String.Format(String.Concat(
                            "SELECT cnpj Id, \"RazaoSocial\" Value ",
                            "FROM \"Fornecedor\" ",
                            "WHERE ROWNUM <= 10 ",
                            "AND \"RazaoSocial\" LIKE '{0}%' ",
                            "AND cnpj NOT IN (SELECT DISTINCT cod_fornecedor FROM gar_forn_quebra) ",
                            "GROUP BY \"RazaoSocial\", cnpj ",
                            "ORDER BY \"RazaoSocial\""), nome);

                        select = conn.Query<GarantiaConfiguracao>(cmdSQL).ToList();
                    }
                    conn.Close();
                }
                return select;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region [Fornecedor Quebra] - Inclusão
        public void FornecedorQuebraIncluir(GarantiaConfiguracao fornecedor)
        {
            try
            {
                fornecedor.Codigos.ToList().ForEach(delegate (string codigo)
                {
                    if (FornecedorQuebraPodeSerCadastrado(codigo.Trim().ToUpper()))
                        using (var conn = new OracleConnection(Entities.Database.Connection.ConnectionString))
                        {
                            conn.Open();
                            if (conn.State == System.Data.ConnectionState.Open)
                            {
                                string cmdSQL = String.Format("INSERT INTO gar_forn_quebra(Cod_Fornecedor) VALUES('{0}')", codigo.Trim().ToUpper());
                                conn.ExecuteScalar(cmdSQL);
                            }
                            conn.Close();
                        }
                });
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region [Fornecedor Quebra] - Exclusão
        public void FornecedorQuebraExcluir(int Id)
        {
            try
            {
                using (var conn = new OracleConnection(Entities.Database.Connection.ConnectionString))
                {
                    conn.Open();
                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        string cmdSQL = String.Format("DELETE FROM gar_forn_quebra WHERE ID ={0}", Id);
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

        #region [Fornecedor Quebra] - Listar
        public IEnumerable<GarantiaConfiguracao> FornecedorQuebraListar()
        {
            try
            {
                IEnumerable<GarantiaConfiguracao> _lista = new List<GarantiaConfiguracao>();
                using (var conn = new OracleConnection(Entities.Database.Connection.ConnectionString))
                {
                    conn.Open();
                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        string cmdSQL = String.Concat(
                            "SELECT FQ.ID, FQ.COD_FORNECEDOR, F.\"NomeFantasia\", F.\"RazaoSocial\" ",
                            "FROM GAR_FORN_QUEBRA FQ ",
                            "INNER JOIN \"Fornecedor\" F ON LTRIM(RTRIM(F.cnpj)) = LTRIM(RTRIM(FQ.cod_fornecedor)) ",
                            "ORDER BY FQ.ID");
                        _lista = conn.Query<GarantiaConfiguracao>(cmdSQL).ToList();
                    }
                    conn.Close();
                }
                return _lista;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion
    }
}
