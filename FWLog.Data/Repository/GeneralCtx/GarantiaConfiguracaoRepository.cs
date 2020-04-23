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

        #region [Genérico] - Inclusão
        public void RegistroIncluir(GarantiaConfiguracao RegistroIncluir)
        {
            try
            {
                #region Processamento 
                switch (RegistroIncluir.Tag)
                {
                    #region Configuração
                    case GarantiaConfiguracao.GarantiaTag.Configuracao:
                        {
                            RegistroIncluir.RegistroFornecedorQuebra.ToList().ForEach(delegate (GarantiaConfiguracao.FornecedorQuebra item)
                            {
                                if (FornecedorQuebraPodeSerCadastrado(item.Cod_Fornecedor.Trim().ToUpper()))
                                    using (var conn = new OracleConnection(Entities.Database.Connection.ConnectionString))
                                    {
                                        conn.Open();
                                        if (conn.State == System.Data.ConnectionState.Open)
                                        {
                                            conn.ExecuteScalar(String.Format(GarantiaConfiguracao.SQL.FornecedorQuebraIncluir, item.Cod_Fornecedor));
                                        }
                                        conn.Close();
                                    }
                            });
                        }
                        break;
                    #endregion

                    #region Fornecedor Quebra
                    case GarantiaConfiguracao.GarantiaTag.FornecedorQuebra:
                        {
                            RegistroIncluir.RegistroFornecedorQuebra.ToList().ForEach(delegate (GarantiaConfiguracao.FornecedorQuebra item)
                            {
                                if (FornecedorQuebraPodeSerCadastrado(item.Cod_Fornecedor.Trim().ToUpper()))
                                    using (var conn = new OracleConnection(Entities.Database.Connection.ConnectionString))
                                    {
                                        conn.Open();
                                        if (conn.State == System.Data.ConnectionState.Open)
                                        {
                                            conn.ExecuteScalar(String.Format(GarantiaConfiguracao.SQL.FornecedorQuebraIncluir, item.Cod_Fornecedor));
                                        }
                                        conn.Close();
                                    }
                            });
                        }
                        break;
                    #endregion

                    #region Remessa Configuração
                    case GarantiaConfiguracao.GarantiaTag.RemessaConfiguracao:
                        {
                            RegistroIncluir.RegistroRemessaConfiguracao.ToList().ForEach(delegate (GarantiaConfiguracao.RemessaConfiguracao item)
                            {
                                if (FornecedorQuebraPodeSerCadastrado(item.Cod_Fornecedor.Trim().ToUpper()))
                                    using (var conn = new OracleConnection(Entities.Database.Connection.ConnectionString))
                                    {
                                        conn.Open();
                                        if (conn.State == System.Data.ConnectionState.Open)
                                        {
                                            conn.ExecuteScalar(String.Format(GarantiaConfiguracao.SQL.RemessaConfiguracaoIncluir,
                                                item.Id_Filial_Sankhya, item.Filial, item.Cod_Fornecedor, item.Automatica, item.Vlr_Minimo, item.Total));
                                        }
                                        conn.Close();
                                    }
                            });
                        }
                        break;
                    #endregion

                    #region Sankhya Top
                    case GarantiaConfiguracao.GarantiaTag.SankhyaTop:
                        {
                            RegistroIncluir.RegistroSankhyaTop.ToList().ForEach(delegate (GarantiaConfiguracao.SankhyaTop item)
                            {
                                using (var conn = new OracleConnection(Entities.Database.Connection.ConnectionString))
                                {
                                    conn.Open();
                                    if (conn.State == System.Data.ConnectionState.Open)
                                    {
                                        conn.ExecuteScalar(String.Format(GarantiaConfiguracao.SQL.SankhyaTopIncluir, item.Top, item.Descricao));
                                    }
                                    conn.Close();
                                }
                            });
                        }
                        break;
                    #endregion

                    #region Remessa Usuário
                    case GarantiaConfiguracao.GarantiaTag.RemessaUsuario:
                        {
                            RegistroIncluir.RegistroRemessaUsuario.ToList().ForEach(delegate (GarantiaConfiguracao.RemessaUsuario item)
                            {
                                using (var conn = new OracleConnection(Entities.Database.Connection.ConnectionString))
                                {
                                    conn.Open();
                                    if (conn.State == System.Data.ConnectionState.Open)
                                    {
                                        conn.ExecuteScalar(String.Format(GarantiaConfiguracao.SQL.RemessaUsuarioIncluir, item.Id_Usr));
                                    }
                                    conn.Close();
                                }
                            });
                        }
                        break;
                    #endregion

                    #region Default
                    default:
                        throw new Exception(String.Format("[RegistroIncluir] A Tag {0} informada é inválida!", RegistroIncluir.Tag));
                        #endregion
                }
                #endregion

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region [Genérico] - Exclusão
        public void RegistroExcluir(GarantiaConfiguracao Registro)
        {
            try
            {
                #region formatar comando exclusão pela TAG
                var cmdSQL = String.Concat("DELETE FROM {0} WHERE ID = ", Registro.Id);

                cmdSQL =
                    Registro.Tag.Equals(GarantiaConfiguracao.GarantiaTag.Configuracao.ToString()) ? String.Format(cmdSQL, "gar_config") :
                    Registro.Tag.Equals(GarantiaConfiguracao.GarantiaTag.RemessaConfiguracao.ToString()) ? String.Format(cmdSQL, "gar_remessa_config") :
                    Registro.Tag.Equals(GarantiaConfiguracao.GarantiaTag.RemessaUsuario.ToString()) ? String.Format(cmdSQL, "gar_remessa_usr") :
                    Registro.Tag.Equals(GarantiaConfiguracao.GarantiaTag.FornecedorQuebra.ToString()) ? String.Format(cmdSQL, "gar_forn_quebra") :
                    Registro.Tag.Equals(GarantiaConfiguracao.GarantiaTag.SankhyaTop.ToString()) ? String.Format(cmdSQL, "geral_sankhya_tops") : String.Empty;
                #endregion

                using (var conn = new OracleConnection(Entities.Database.Connection.ConnectionString))
                {
                    conn.Open();
                    if (conn.State == System.Data.ConnectionState.Open) { conn.ExecuteScalar(cmdSQL); }
                    conn.Close();
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region [Genérico] - Listar
        public GarantiaConfiguracao RegistroListar(GarantiaConfiguracao.GarantiaTag TAG)
        {
            try
            {
                var _garantia = new GarantiaConfiguracao() { Tag = TAG };

                #region formatar comando consulta pela TAG
                var cmdSQL = GarantiaConfiguracao.DicTagConsultaSQL.Where(w => w.Key.Equals(TAG)).FirstOrDefault().Value;
                #endregion

                #region Consulta Banco Dados 
                using (var conn = new OracleConnection(Entities.Database.Connection.ConnectionString))
                {
                    conn.Open();
                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        if (_garantia.Tag.Equals(GarantiaConfiguracao.GarantiaTag.Configuracao))
                        {
                            _garantia.RegistroConfiguracao = conn.Query<GarantiaConfiguracao.Configuracao>(cmdSQL).ToList();
                            _garantia.RegistroConfiguracao.ForEach(delegate (GarantiaConfiguracao.Configuracao item) { item.BotaoEvento = String.Format(GarantiaConfiguracao.botaoExcluirTemplate, TAG, item.Id); });
                        }

                        if (_garantia.Tag.Equals(GarantiaConfiguracao.GarantiaTag.FornecedorQuebra))
                        {
                            _garantia.RegistroFornecedorQuebra = conn.Query<GarantiaConfiguracao.FornecedorQuebra>(cmdSQL).ToList();
                            _garantia.RegistroFornecedorQuebra.ForEach(delegate (GarantiaConfiguracao.FornecedorQuebra item) { item.BotaoEvento = String.Format(GarantiaConfiguracao.botaoExcluirTemplate, TAG, item.Id); });
                        }

                        if (_garantia.Tag.Equals(GarantiaConfiguracao.GarantiaTag.RemessaConfiguracao))
                        {
                            _garantia.RegistroRemessaConfiguracao = conn.Query<GarantiaConfiguracao.RemessaConfiguracao>(cmdSQL).ToList();
                            _garantia.RegistroRemessaConfiguracao.ForEach(delegate (GarantiaConfiguracao.RemessaConfiguracao item) { item.BotaoEvento = String.Format(GarantiaConfiguracao.botaoExcluirTemplate, TAG, item.Id); });
                        }

                        if (_garantia.Tag.Equals(GarantiaConfiguracao.GarantiaTag.RemessaUsuario))
                        {
                            _garantia.RegistroRemessaUsuario = conn.Query<GarantiaConfiguracao.RemessaUsuario>(cmdSQL).ToList();
                            _garantia.RegistroRemessaUsuario.ForEach(delegate (GarantiaConfiguracao.RemessaUsuario item) { item.BotaoEvento = String.Format(GarantiaConfiguracao.botaoExcluirTemplate, TAG, item.Id); });
                        }

                        if (_garantia.Tag.Equals(GarantiaConfiguracao.GarantiaTag.SankhyaTop))
                        {
                            _garantia.RegistroSankhyaTop = conn.Query<GarantiaConfiguracao.SankhyaTop>(cmdSQL).ToList();
                            _garantia.RegistroSankhyaTop.ForEach(delegate (GarantiaConfiguracao.SankhyaTop item) { item.BotaoEvento = String.Format(GarantiaConfiguracao.botaoExcluirTemplate, TAG, item.Id); });
                        }
                    }
                    conn.Close();
                }
                #endregion

                return _garantia;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region [Genérico] AutoComplete 
        public List<GarantiaConfiguracao.AutoComplete> AutoComplete(GarantiaConfiguracao.AutoComplete _AutoComplete)
        {
            try
            {
                var _listaAutoComplete = new List<GarantiaConfiguracao.AutoComplete>();

                #region Formatar Consulta SQL
                switch (_AutoComplete.tag)
                {
                    #region Fornecedor Quebra
                    case GarantiaConfiguracao.GarantiaTag.FornecedorQuebra:
                        _AutoComplete.comandoSQL = String.Format(String.Concat(
                            "SELECT cnpj Data, \"RazaoSocial\" Value ",
                            "FROM \"Fornecedor\" ",
                            "WHERE ROWNUM <= 10 ",
                            "AND \"Ativo\" = 1 ",
                            "AND \"RazaoSocial\" LIKE '{0}%' ",
                            "AND cnpj NOT IN (SELECT DISTINCT cod_fornecedor FROM gar_forn_quebra) ",
                            "GROUP BY \"RazaoSocial\", cnpj ",
                            "ORDER BY \"RazaoSocial\""), _AutoComplete.palavra.ToUpper());
                        break;
                    #endregion

                    #region Remessa Usuário
                    case GarantiaConfiguracao.GarantiaTag.RemessaUsuario:
                        _AutoComplete.comandoSQL = String.Format(String.Concat(
                            "SELECT \"Id\" Data, \"UserName\" ||' ('|| \"Email\" ||')' Value ",
                            "FROM \"AspNetUsers\" ",
                            "WHERE ROWNUM <= 10 ",
                            "AND \"Id\" NOT IN (SELECT Id_Usr FROM gar_remessa_usr) ",
                            "AND \"Email\" LIKE '{0}%' ",
                            "OR \"UserName\" LIKE '{0}%' ",
                            "ORDER BY \"UserName\""), _AutoComplete.palavra);
                        break;
                    #endregion

                    #region Remessa Configuração
                    case GarantiaConfiguracao.GarantiaTag.RemessaConfiguracao:
                        {
                            if (_AutoComplete.tagAutoComplete.Equals(GarantiaConfiguracao.AutoCompleteTag.Fornecedor))
                                _AutoComplete.comandoSQL = String.Format(String.Concat(
                                    "SELECT cnpj Data, \"RazaoSocial\" Value ",
                                    "FROM \"Fornecedor\" ",
                                    "WHERE ROWNUM <= 10 ",
                                    "AND \"Ativo\" = 1 ",
                                    "AND \"RazaoSocial\" LIKE '{0}%' ",
                                    "AND cnpj NOT IN (SELECT DISTINCT cod_fornecedor FROM gar_remessa_config) ",
                                    "GROUP BY \"RazaoSocial\", cnpj ",
                                    "ORDER BY \"RazaoSocial\""), _AutoComplete.palavra.ToUpper());

                            if (_AutoComplete.tagAutoComplete.Equals(GarantiaConfiguracao.AutoCompleteTag.Filial))
                                _AutoComplete.comandoSQL = String.Format(String.Concat(
                                    "SELECT \"IdEmpresa\" Data, \"NomeFantasia\" Value ",
                                    "FROM \"Empresa\" ",
                                    "WHERE ROWNUM <= 10 ",
                                    "AND \"Ativo\" = 1 ",
                                    "AND \"IdEmpresa\" NOT IN (SELECT Id_Filial_Sankhya FROM gar_remessa_config) ",
                                    "AND \"NomeFantasia\" LIKE UPPER('%{0}%') ",
                                    "ORDER BY \"NomeFantasia\""), _AutoComplete.palavra);
                        }
                        break;
                        #endregion
                }
                #endregion

                #region Processar Consulta Banco Dados
                using (var conn = new OracleConnection(Entities.Database.Connection.ConnectionString))
                {
                    conn.Open();
                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        _listaAutoComplete = conn.Query<GarantiaConfiguracao.AutoComplete>(_AutoComplete.comandoSQL).ToList();
                    }
                    conn.Close();
                }
                #endregion

                return _listaAutoComplete;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
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
    }
}
