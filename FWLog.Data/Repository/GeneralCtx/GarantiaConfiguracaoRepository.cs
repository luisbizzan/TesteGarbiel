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
                            RegistroIncluir.RegistroConfiguracao.ToList().ForEach(delegate (GarantiaConfiguracao.Configuracao item)
                            {
                                using (var conn = new OracleConnection(Entities.Database.Connection.ConnectionString))
                                {
                                    conn.Open();
                                    if (conn.State == System.Data.ConnectionState.Open)
                                    {
                                        var comandoSQL = conn.ExecuteScalar<Int32>(String.Format(GarantiaConfiguracao.SQL.ConfiguracaoJaConsta, item.Id_Filial_Sankhya)).Equals(0) ?
                                        String.Format(GarantiaConfiguracao.SQL.ConfiguracaoIncluir, item.Id_Filial_Sankhya, item.Filial, item.Pct_Estorno_Frete.ToString().Replace(",", "."),
                                        item.Pct_Desvalorizacao.ToString().Replace(",", "."), item.Vlr_Minimo_Envio.ToString().Replace(",", "."), item.Prazo_Envio_Automatico, item.Prazo_Descarte) :
                                        String.Format(GarantiaConfiguracao.SQL.ConfigurarAtualizar, item.Pct_Estorno_Frete, item.Pct_Desvalorizacao, item.Vlr_Minimo_Envio.ToString().Replace(",", "."),
                                        item.Prazo_Envio_Automatico, item.Prazo_Descarte, item.Id_Filial_Sankhya);

                                        conn.ExecuteScalar(comandoSQL);
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
                                using (var conn = new OracleConnection(Entities.Database.Connection.ConnectionString))
                                {
                                    conn.Open();
                                    if (conn.State == System.Data.ConnectionState.Open)
                                    {
                                        conn.ExecuteScalar(String.Format(GarantiaConfiguracao.SQL.RemessaConfiguracaoIncluir,
                                            item.Id_Filial_Sankhya, item.Filial, item.Cod_Fornecedor, item.Automatica, item.Vlr_Minimo.ToString().Replace(",", "."), item.Total));
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
                                        conn.ExecuteScalar(String.Format(GarantiaConfiguracao.SQL.SankhyaTopIncluir, item.Top, item.Descricao, item.Id_Negociacao));
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

                    #region Fornecedor Grupo
                    case GarantiaConfiguracao.GarantiaTag.FornecedorGrupo:
                        {
                            RegistroIncluir.RegistroFornecedorGrupo.ToList().ForEach(delegate (GarantiaConfiguracao.FornecedorGrupo item)
                            {
                                if (ValidarInclusaoFornecedorGrupo(item))
                                    using (var conn = new OracleConnection(Entities.Database.Connection.ConnectionString))
                                    {
                                        conn.Open();
                                        if (conn.State == System.Data.ConnectionState.Open)
                                        {
                                            conn.ExecuteScalar(String.Format(GarantiaConfiguracao.SQL.FornecedorGrupoIncluir, item.Cod_Forn_Pai, item.Cod_Forn_Filho));
                                        }
                                        conn.Close();
                                    }
                            });
                        }
                        break;
                    #endregion

                    #region Motivo Laudo
                    case GarantiaConfiguracao.GarantiaTag.MotivoLaudo:
                        {
                            RegistroIncluir.RegistroMotivoLaudo.ToList().ForEach(delegate (GarantiaConfiguracao.MotivoLaudo item)
                            {
                                using (var conn = new OracleConnection(Entities.Database.Connection.ConnectionString))
                                {
                                    conn.Open();
                                    if (conn.State == System.Data.ConnectionState.Open)
                                    {
                                        conn.ExecuteScalar(String.Format(GarantiaConfiguracao.SQL.MotivoLaudoIncluir, item.Id_Tipo, item.MotivoLaudoDescricao));
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
                    Registro.Tag.Equals(GarantiaConfiguracao.GarantiaTag.Configuracao) ? String.Format(cmdSQL, "gar_config") :
                    Registro.Tag.Equals(GarantiaConfiguracao.GarantiaTag.RemessaConfiguracao) ? String.Format(cmdSQL, "gar_remessa_config") :
                    Registro.Tag.Equals(GarantiaConfiguracao.GarantiaTag.RemessaUsuario) ? String.Format(cmdSQL, "gar_remessa_usr") :
                    Registro.Tag.Equals(GarantiaConfiguracao.GarantiaTag.FornecedorQuebra) ? String.Format(cmdSQL, "gar_forn_quebra") :
                    Registro.Tag.Equals(GarantiaConfiguracao.GarantiaTag.SankhyaTop) ? String.Format(cmdSQL, "geral_sankhya_tops") :
                    Registro.Tag.Equals(GarantiaConfiguracao.GarantiaTag.FornecedorGrupo) ? String.Format(cmdSQL, "gar_forn_grupo") :
                    Registro.Tag.Equals(GarantiaConfiguracao.GarantiaTag.MotivoLaudo) ? String.Format(cmdSQL, "gar_motivo_laudo") :
                    String.Empty;
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
                var comandoSQL = GarantiaConfiguracao.Contexto.DicTagConsultaSQL.Where(SQL => SQL.Key.Equals(TAG)).FirstOrDefault().Value.ToString();
                #endregion

                #region Consulta Banco Dados 
                using (var conn = new OracleConnection(Entities.Database.Connection.ConnectionString))
                {
                    conn.Open();
                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        #region Configuração
                        if (_garantia.Tag.Equals(GarantiaConfiguracao.GarantiaTag.Configuracao))
                        {
                            _garantia.RegistroConfiguracao = conn.Query<GarantiaConfiguracao.Configuracao>(comandoSQL).ToList();
                            _garantia.RegistroConfiguracao.ForEach(delegate (GarantiaConfiguracao.Configuracao item)
                            {
                                item.BotaoEvento = String.Format(GarantiaConfiguracao.Contexto.botaoExcluirTemplate, TAG, item.Id);
                                item.Vlr_Minimo_EnvioView = String.Format("{0:0,0.00}", item.Vlr_Minimo_Envio);
                                item.Pct_DesvalorizacaoView = String.Format("{0}%", item.Pct_Desvalorizacao.ToString().Replace(",", "."));
                                item.Pct_Estorno_FreteView = String.Format("{0}%", item.Pct_Estorno_Frete.ToString().Replace(",", "."));
                            });
                        }
                        #endregion

                        #region Fornecedor Quebra
                        if (_garantia.Tag.Equals(GarantiaConfiguracao.GarantiaTag.FornecedorQuebra))
                        {
                            _garantia.RegistroFornecedorQuebra = conn.Query<GarantiaConfiguracao.FornecedorQuebra>(comandoSQL).ToList();
                            _garantia.RegistroFornecedorQuebra.ForEach(delegate (GarantiaConfiguracao.FornecedorQuebra item) { item.BotaoEvento = String.Format(GarantiaConfiguracao.Contexto.botaoExcluirTemplate, TAG, item.Id); });
                        }
                        #endregion

                        #region Remessa Configuração
                        if (_garantia.Tag.Equals(GarantiaConfiguracao.GarantiaTag.RemessaConfiguracao))
                        {
                            _garantia.RegistroRemessaConfiguracao = conn.Query<GarantiaConfiguracao.RemessaConfiguracao>(comandoSQL).ToList();
                            _garantia.RegistroRemessaConfiguracao.ForEach(delegate (GarantiaConfiguracao.RemessaConfiguracao item)
                            {
                                item.BotaoEvento = String.Format(GarantiaConfiguracao.Contexto.botaoExcluirTemplate, TAG, item.Id);
                                item.Vlr_MinimoView = String.Format("{0:0,0.00}", item.Vlr_Minimo);
                                item.AutomaticaView = item.Automatica.Equals(1) ? GarantiaConfiguracao.Contexto.botaoCheked : GarantiaConfiguracao.Contexto.botaoUnCheked;
                                item.TotalView = item.Total.Equals(1) ? GarantiaConfiguracao.Contexto.botaoCheked : GarantiaConfiguracao.Contexto.botaoUnCheked;
                            });
                        }
                        #endregion

                        #region Remessa Usuário
                        if (_garantia.Tag.Equals(GarantiaConfiguracao.GarantiaTag.RemessaUsuario))
                        {
                            _garantia.RegistroRemessaUsuario = conn.Query<GarantiaConfiguracao.RemessaUsuario>(comandoSQL).ToList();
                            _garantia.RegistroRemessaUsuario.ForEach(delegate (GarantiaConfiguracao.RemessaUsuario item) { item.BotaoEvento = String.Format(GarantiaConfiguracao.Contexto.botaoExcluirTemplate, TAG, item.Id); });
                        }
                        #endregion

                        #region Sankhya Top
                        if (_garantia.Tag.Equals(GarantiaConfiguracao.GarantiaTag.SankhyaTop))
                        {
                            _garantia.RegistroSankhyaTop = conn.Query<GarantiaConfiguracao.SankhyaTop>(comandoSQL).ToList();
                            _garantia.RegistroSankhyaTop.ForEach(delegate (GarantiaConfiguracao.SankhyaTop item) { item.BotaoEvento = String.Format(GarantiaConfiguracao.Contexto.botaoExcluirTemplate, TAG, item.Id); });
                        }
                        #endregion

                        #region Fornecedor Grupo
                        if (_garantia.Tag.Equals(GarantiaConfiguracao.GarantiaTag.FornecedorGrupo))
                        {
                            _garantia.RegistroFornecedorGrupo = conn.Query<GarantiaConfiguracao.FornecedorGrupo>(comandoSQL).ToList();
                            _garantia.RegistroFornecedorGrupo.ForEach(delegate (GarantiaConfiguracao.FornecedorGrupo item) { item.BotaoEvento = String.Format(GarantiaConfiguracao.Contexto.botaoExcluirTemplate, TAG, item.Id); });

                            var _listaAgrupada = _garantia.RegistroFornecedorGrupo.GroupBy(g => g.Cod_Forn_Pai).Select(s => s.ToList()).ToList();
                            _garantia.RegistroFornecedorGrupo = new List<GarantiaConfiguracao.FornecedorGrupo>();
                            _listaAgrupada.ForEach(delegate (List<GarantiaConfiguracao.FornecedorGrupo> fg)
                            {
                                var registro = new GarantiaConfiguracao.FornecedorGrupo() { Cod_Forn_Pai = fg.FirstOrDefault().Cod_Forn_Pai, divFilhos = String.Empty };
                                fg.ForEach(delegate (GarantiaConfiguracao.FornecedorGrupo fgItem)
                                {
                                    registro.divFilhos += String.Format(GarantiaConfiguracao.Contexto.botaoExcluirFornecedorFilho, fgItem.Cod_Forn_Filho, TAG, fgItem.Id);
                                });
                                registro.divFilhos = String.Format(GarantiaConfiguracao.Contexto.botaoDivFornecedorGrupo, fg.FirstOrDefault().Id, registro.divFilhos);
                                _garantia.RegistroFornecedorGrupo.Add(registro);
                            });
                        }
                        #endregion

                        #region Motivo Laudo
                        if (_garantia.Tag.Equals(GarantiaConfiguracao.GarantiaTag.MotivoLaudo))
                        {
                            _garantia.RegistroMotivoLaudo = conn.Query<GarantiaConfiguracao.MotivoLaudo>(comandoSQL).ToList();
                            _garantia.RegistroMotivoLaudo.ForEach(delegate (GarantiaConfiguracao.MotivoLaudo item) { item.BotaoEvento = String.Format(GarantiaConfiguracao.Contexto.botaoExcluirTemplate, TAG, item.Id); });
                        }
                        #endregion
                    }
                    conn.Close();
                }
                #endregion

                return _garantia;
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("{0} - {1}", TAG.ToString(), ex.Message));
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
                                    "WHERE \"Ativo\" = 1 ",
                                    "AND ROWNUM <= 10 ",
                                    "AND \"RazaoSocial\" LIKE '{0}%' ",
                                    "GROUP BY \"RazaoSocial\", cnpj ",
                                    "ORDER BY \"RazaoSocial\""), _AutoComplete.palavra.ToUpper());

                            if (_AutoComplete.tagAutoComplete.Equals(GarantiaConfiguracao.AutoCompleteTag.Filial))
                                _AutoComplete.comandoSQL = String.Format(String.Concat(
                                    "SELECT \"IdEmpresa\" Data, '['||\"Sigla\"||'] '|| \"NomeFantasia\" Value ",
                                    "FROM \"Empresa\" ",
                                    "WHERE \"Ativo\" = 1 ",
                                    "AND ROWNUM <= 10 ",
                                    "AND \"NomeFantasia\" LIKE UPPER('%{0}%') ",
                                    "ORDER BY \"NomeFantasia\""), _AutoComplete.palavra);
                        }
                        break;
                    #endregion

                    #region Configuração
                    case GarantiaConfiguracao.GarantiaTag.Configuracao:
                        {
                            if (_AutoComplete.tagAutoComplete.Equals(GarantiaConfiguracao.AutoCompleteTag.Filial))
                                _AutoComplete.comandoSQL = String.Format(String.Concat(
                                    "SELECT e.\"IdEmpresa\" Data, '['||e.\"Sigla\"||'] '|| e.\"NomeFantasia\" Value, ",
                                    "c.Pct_Estorno_Frete, c.Pct_Desvalorizacao, c.Vlr_Minimo_Envio, c.Prazo_Envio_Automatico, c.Prazo_Descarte, ",
                                    "CASE WHEN c.Id IS NULL THEN 0 ELSE 1 END RegistroCadastrado ",
                                    "FROM \"Empresa\" e ",
                                    "LEFT JOIN gar_config c ON  c.Id_Filial_Sankhya = e.\"IdEmpresa\" ",
                                    "WHERE e.\"Ativo\" = 1 ",
                                    "AND ROWNUM <= 10 ",
                                    "AND e.\"NomeFantasia\" LIKE UPPER('%{0}%') ",
                                    "ORDER BY e.\"NomeFantasia\""), _AutoComplete.palavra);
                        }
                        break;
                    #endregion

                    #region Fornecedor Grupo
                    case GarantiaConfiguracao.GarantiaTag.FornecedorGrupo:
                        _AutoComplete.comandoSQL = String.Format(String.Concat(
                            "SELECT cnpj Data, '('|| cnpj ||') '||\"RazaoSocial\" Value ",
                            "FROM \"Fornecedor\" ",
                            "WHERE ROWNUM <= 10 ",
                            "AND \"Ativo\" = 1 ",
                            "AND \"RazaoSocial\" LIKE '{0}%' ",
                            "OR cnpj LIKE '%{0}%' ",
                            "GROUP BY \"RazaoSocial\", cnpj ",
                            "ORDER BY \"RazaoSocial\""), _AutoComplete.palavra.ToUpper());
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

                #region Formatar Retorno
                if (_AutoComplete.tag.Equals(GarantiaConfiguracao.GarantiaTag.Configuracao) && _AutoComplete.tagAutoComplete.Equals(GarantiaConfiguracao.AutoCompleteTag.Filial))
                    _listaAutoComplete.ForEach(delegate (GarantiaConfiguracao.AutoComplete ac) { ac.Vlr_Minimo_EnvioView = String.Format("{0:0,0.00}", ac.Vlr_Minimo_Envio); });
                #endregion

                return _listaAutoComplete;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region [Sankhya Tops] Select - Id Negociação
        public GarantiaConfiguracao ListarIdNegociacao()
        {
            var _lista = new GarantiaConfiguracao();
            try
            {
                var cmdSQL = String.Format(
                    @"SELECT codtipvenda Data, descrtipvenda Value
                        FROM tgftpv 
                        WHERE ativo = 'S'
                        GROUP BY codtipvenda, descrtipvenda
                        ORDER BY descrtipvenda");

                using (var conn = new OracleConnection(ConfigurationManager.ConnectionStrings["Sankya"].ToString()))
                {
                    conn.Open();
                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        _lista.ListaAutoComplete = new List<GarantiaConfiguracao.AutoComplete>(conn.Query<GarantiaConfiguracao.AutoComplete>(cmdSQL).ToList());
                    }
                    conn.Close();
                }

                _lista.ListaAutoComplete.ForEach(delegate (GarantiaConfiguracao.AutoComplete select) { select.Value = String.IsNullOrEmpty(select.Value) ? "SEM TIPO DE VENDA" : select.Value.ToUpper(); });

                return _lista;
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

        #region [Fornecedor Grupo] - Validar se cadastro fornecedor grupo pode ser incluído no banco dados
        /// <summary>
        ///  retorno verdadeiro o registro pode ser cadastrado 
        /// </summary>
        private bool ValidarInclusaoFornecedorGrupo(GarantiaConfiguracao.FornecedorGrupo registro)
        {
            try
            {
                var _processamento = 0;
                using (var conn = new OracleConnection(Entities.Database.Connection.ConnectionString))
                {
                    conn.Open();
                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        #region Validar se fornecedor Filho e fornecedor Pai não estão com mesmo código
                        if (registro.Cod_Forn_Filho == registro.Cod_Forn_Pai)
                            throw new Exception("O fornecedor Pai e fornecedor Filho não pode ser o mesmo!");
                        #endregion

                        #region Validar se Pai já é Filho
                        string cmdSQL = String.Format("SELECT COUNT(1) V FROM gar_forn_grupo WHERE cod_forn_filho='{0}'", registro.Cod_Forn_Pai);
                        _processamento = conn.ExecuteScalar<Int32>(cmdSQL);
                        if (!_processamento.Equals(0))
                            throw new Exception("O fornecedor já consta cadastrado como Filho e não pode ser cadastrado como Pai!");
                        #endregion

                        #region Validar se Filho já é Filho de outro Pai
                        cmdSQL = String.Format("SELECT COUNT(1) V FROM gar_forn_grupo WHERE cod_forn_filho='{0}'", registro.Cod_Forn_Filho);
                        _processamento = conn.ExecuteScalar<Int32>(cmdSQL);
                        if (!_processamento.Equals(0))
                            throw new Exception("O fornecedor já consta cadastrado como Filho e não pode ser cadastrado como Filho de outro Pai!");
                        #endregion

                        #region Validar se Filho já é Pai
                        cmdSQL = String.Format("SELECT COUNT(1) V FROM gar_forn_grupo WHERE cod_forn_Pai='{0}'", registro.Cod_Forn_Filho);
                        _processamento = conn.ExecuteScalar<Int32>(cmdSQL);
                        if (!_processamento.Equals(0))
                            throw new Exception("O fornecedor já consta cadastrado como Pai e não pode ser cadastrado como Filho!");
                        #endregion

                    }
                    conn.Close();
                }
                return _processamento.Equals(0) ? true : false;
            }
            catch (Exception erro)
            {
                throw new Exception(erro.Message);
            }
        }
        #endregion
    }
}
