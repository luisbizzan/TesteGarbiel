using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Script.Serialization;

namespace FWLog.Data.Models
{
    public class GarantiaConfiguracao
    {

        [Required]
        [Display(Name = "TAG")]
        public GarantiaTag Tag { get; set; }

        [Display(Name = "Código")]
        public long Id { get; set; }

        #region Listas de Inclusão
        public List<FornecedorQuebra> RegistroFornecedorQuebra { get; set; }
        public List<SankhyaTop> RegistroSankhyaTop { get; set; }
        public List<Configuracao> RegistroConfiguracao { get; set; }
        public List<RemessaUsuario> RegistroRemessaUsuario { get; set; }
        public List<RemessaConfiguracao> RegistroRemessaConfiguracao { get; set; }
        public List<FornecedorGrupo> RegistroFornecedorGrupo { get; set; }
        public List<MotivoLaudo> RegistroMotivoLaudo { get; set; }
        public List<AutoComplete> ListaAutoComplete { get; set; }
        #endregion

        public string BotaoEvento { get; set; }
        public enum GarantiaTag { RemessaConfiguracao, RemessaUsuario, Configuracao, FornecedorQuebra, SankhyaTop, FornecedorGrupo, MotivoLaudo }
        public enum AutoCompleteTag { Fornecedor, Filial }
        public enum TipoGeral
        {
            Automatico = 1, Manual = 2, Fornecedor = 3, Entrada = 4, Origem = 5, RetornoFornecedor = 6, EnvioCliente = 7, Defeito = 8, Sinistro = 9
        }

        #region Variáveis de Contexto
        public static class Contexto
        {
            public static JavaScriptSerializer SerializarJS = new JavaScriptSerializer() { MaxJsonLength = 9999999 };
            public static string GridNome { get; set; }
            public static object[] GridColunas { get; set; }

            /// <summary>
            /// Dicionário de TAGs
            /// </summary>
            public static Dictionary<int, string> DicTagsValidas
            {
                get
                {
                    return new Dictionary<int, string>()
                {
                    { 0, GarantiaTag.RemessaConfiguracao.ToString() },
                    { 1, GarantiaTag.RemessaUsuario.ToString() },
                    { 2, GarantiaTag.Configuracao.ToString() },
                    { 3, GarantiaTag.FornecedorQuebra.ToString() },
                    { 4, GarantiaTag.SankhyaTop.ToString() },
                    { 5, GarantiaTag.FornecedorGrupo.ToString() },
                    { 6, GarantiaTag.MotivoLaudo.ToString() }
                };
                }
            }

            /// <summary>
            /// Dicionáriode TAG x Consultas SQL
            /// </summary>
            public static Dictionary<GarantiaTag, string> DicTagConsultaSQL
            {
                get
                {
                    return new Dictionary<GarantiaTag, string>()
                {
                    {GarantiaTag.FornecedorQuebra, SQL.FornecedorQuebraListar },
                    {GarantiaTag.SankhyaTop, SQL.SankhyaTopListar },
                    {GarantiaTag.Configuracao, SQL.ConfiguracaoListar },
                    {GarantiaTag.RemessaConfiguracao, SQL.RemessaConfiguracaoListar },
                    {GarantiaTag.RemessaUsuario, SQL.RemessaUsuarioListar },
                    {GarantiaTag.FornecedorGrupo, SQL.FornecedorGrupoListar },
                    {GarantiaTag.MotivoLaudo, SQL.MotivoLaudoListar}
                };
                }
            }

            /// <summary>
            /// Dicionario de TAG x Nome Grid View
            /// </summary>
            public static Dictionary<GarantiaTag, string> DicTagGridNome
            {
                get
                {
                    return new Dictionary<GarantiaConfiguracao.GarantiaTag, string>()
                {
                    {GarantiaTag.FornecedorQuebra, "#gridFornecedorQuebra" },
                    {GarantiaTag.SankhyaTop, "#gridSankhyaTop" },
                    {GarantiaTag.Configuracao, "#gridConfiguracao" },
                    {GarantiaTag.RemessaConfiguracao, "#gridRemessaConfig" },
                    {GarantiaTag.RemessaUsuario, "#gridRemessaUsuario" },
                    {GarantiaTag.FornecedorGrupo, "#gridFornecedorGrupo" },
                    {GarantiaTag.MotivoLaudo, "#gridMotivoLaudo" }
                };
                }
            }

            /// <summary>
            /// Dicionário TAG x Colunas
            /// </summary>
            public static Dictionary<GarantiaTag, object> DicTagGridColuna
            {
                get
                {
                    return new Dictionary<GarantiaTag, object>()
                {
                    { GarantiaTag.FornecedorQuebra, new object[]
                    {
                        new { data = "BotaoEvento" }, new { data = "Id", title = "Id Registro" },
                        new { data = "Cod_Fornecedor", title = "CNPJ" }, new { data = "NomeFantasia", title =  "Nome Fantasia" }, new { data = "RazaoSocial", title = "Razão Social" }
                    }},

                    { GarantiaTag.SankhyaTop, new object[]
                    {
                        new { data = "BotaoEvento" }, new { data = "Id", title = "Id Registro" },
                        new { data = "Top", title = "Top" }, new { data = "Descricao", title = "Descrição" }, new { data = "Id_NegociacaoView", title = "Negociação" },
                        new { data = "VendaMin", title = "Venda Mínima" }, new { data = "VendaMax", title = "Venda Máxima" }
                    }},

                    { GarantiaTag.RemessaUsuario, new object[]
                    {
                        new { data = "BotaoEvento" }, new { data = "Id", title = "Id Registro" },
                        new { data = "Usuario", title = "Usuário" }, new { data = "Filial", title = "Filial" }, new { data = "Nome", title = "Nome" }, new { data = "Email", title = "E-mail" }
                    } },

                    { GarantiaTag.RemessaConfiguracao, new object[]
                    {
                        new { data = "BotaoEvento" }, new { data = "Id", title = "Id Registro" },
                        new { data = "Filial", title = "Filial" }, new { data = "Cod_Fornecedor", title = "Fornecedor (CNPJ)" },
                        new { data = "AutomaticaView", title = "Automática" }, new { data = "Vlr_MinimoView", title = "R$ Minímo" }, new { data = "TotalView", title = "Total" }
                    }},

                    { GarantiaTag.Configuracao, new object[]
                    {
                        new { data = "BotaoEvento" }, new { data = "Id", title = "Id Registro" },
                        new { data = "Filial", title = "Filial" }, new { data = "Pct_Estorno_FreteView", title = "Estorno Frete" },
                        new { data = "Pct_DesvalorizacaoView", title = "Desvalorização" }, new { data = "Vlr_Minimo_EnvioView", title = "R$ Minímo Laudo" },
                        new { data = "Prazo_Envio_Automatico", title = "Prazo Envio Laudo" }, new { data = "Prazo_Descarte", title = "Prazo Descarte" }
                    }},

                    { GarantiaTag.FornecedorGrupo, new object[]
                    {
                        new { data = "Cod_Forn_Pai", title = "Fornecedor Pai" },new { data = "divFilhos", title = "Fornecedor Filho" },
                    }},

                    { GarantiaTag.MotivoLaudo, new object[]
                    {
                        new { data = "BotaoEvento" }, new { data = "Id", title = "Id Registro" },
                        new { data = "TipoLaudo", title = "Tipo" }, new { data = "MotivoLaudoDescricao", title = "Descrição" },
                    }}
                };
                }
            }

            /// <summary>
            /// {0} Tag(Tabela) | {1} Id Registro
            /// </summary>
            public static string botaoExcluirTemplate
            {
                get
                {
                    return "<a onclick=\"RegistroExcluir('{0}',{1});\" class=\"btn btn-link btn-row-actions\" data-toggle=\"tooltip\" data-placement=\"top\" data-original-title=\"Excluir Registro\"><i class=\"glyphicon glyphicon-trash text-danger\"></i></a>";
                }
            }
            /// <summary>
            /// {0} Tag(Tabela) | {1} Id Registro | {2} Parametros (Separador ";")
            /// </summary>
            public static string botaoEditarTemplate
            {
                get
                {
                    return "<a onclick=\"RegistroEditar('{0}',{1},'{2}');\" class=\"btn btn-link btn-row-actions\" data-toggle=\"tooltip\" data-placement=\"top\" data-original-title=\"Modificar Registro\"><i class=\"glyphicon glyphicon-pencil text-success\"></i></a>";
                }
            }
            public static string botaoCheked { get { return "<h4 class=\"text-center\"><i class=\"glyphicon glyphicon-ok-circle text-success\" data-toggle=\"tooltip\" data-original-title=\"Sim\"></i></h4>"; } }
            public static string botaoUnCheked { get { return "<h4 class=\"text-center\"><i class=\"glyphicon glyphicon-remove-circle text-danger\" data-toggle=\"tooltip\" data-original-title=\"Não\"></i></h4>"; } }
            /// <summary>
            /// {0} Nome Fornecedor Filho | {1} Tag | {2} Id Registro
            /// </summary>
            public static string botaoExcluirFornecedorFilho
            {
                get
                {
                    return "<a onclick=\"RegistroExcluir('{1}',{2});\" class=\"btn btn-link btn-row-actions\" data-toggle=\"tooltip\" data-placement=\"top\" data-original-title=\"Excluir {0}\"><i class=\"glyphicon glyphicon-trash text-danger\"></i></a> {0}</br>";
                }
            }
            /// <summary>
            /// {0} Id Primeiro Filho | {1} Cod Fornecedor Filho
            /// </summary>
            public static string botaoDivFornecedorGrupo
            {
                get
                {
                    return String.Concat("<a class=\"btn btn-link\" data-toggle=\"collapse\" data-target=\"#div{0}\">",
                        "<i class=\"glyphicon glyphicon-list-alt text-warning\" data-toggle=\"tooltip\" data-original-title=\"Visualizar Filho(s)\"></i></a>",
                        "<div id=\"div{0}\" class=\"collapse\">{1}</div>");
                }
            }
        }
        #endregion

        #region [GENÉRICO] AutoComplete
        public class AutoComplete
        {
            public string Data { get; set; }
            public string Value { get; set; }

            public long Pct_Estorno_Frete { get; set; }
            public long Pct_Desvalorizacao { get; set; }
            public decimal Vlr_Minimo_Envio { get; set; }
            public string Vlr_Minimo_EnvioView { get; set; }
            public long Prazo_Envio_Automatico { get; set; }
            public long Prazo_Descarte { get; set; }
            public Boolean RegistroCadastrado { get; set; }

            [Required]
            [Display(Name = "TAG")]
            public GarantiaTag tag { get; set; }
            [Required]
            [Display(Name = "Texto")]
            public string palavra { get; set; }

            public AutoCompleteTag tagAutoComplete { get; set; }

            public string comandoSQL { get; set; }
        }
        #endregion

        #region [GAR_FORN_QUEBRA]
        public class FornecedorQuebra
        {
            [Display(Name = "Codigo")]
            public long Id { get; set; }

            [Required]
            [Display(Name = "Id Empresa")]
            public long Id_Empresa { get; set; }

            [Required]
            [Display(Name = "Código do Fornecedor")]
            public string Cod_Fornecedor { get; set; }

            [Display(Name = "Nome Fantasia")]
            public string NomeFantasia { get; set; }

            [Display(Name = "Razão Social")]
            public string RazaoSocial { get; set; }

            public string BotaoEvento { get; set; }
            public static string MenuConfiguracao { get; set; }
        }
        #endregion

        #region [GERAL_SANKHYA_TOPS]
        public class SankhyaTop
        {
            [Display(Name = "Codigo")]
            public long Id { get; set; }
            [Required]
            [Display(Name = "Nome Top")]
            public long Top { get; set; }
            [Display(Name = "Descrição da Top")]
            public string Descricao { get; set; }
            [Required]
            [Display(Name = "Negociação")]
            public long Id_Negociacao { get; set; }
            [Display(Name = "Negociação")]
            public string Id_NegociacaoView { get; set; }

            [Display(Name = "Venda Mínima")]
            public string VendaMin { get; set; }
            [Display(Name = "Venda Máxima")]
            public string VendaMax { get; set; }

            public string BotaoEvento { get; set; }
        }
        #endregion

        #region [GAR_CONFIG]
        public class Configuracao
        {
            [Display(Name = "Código")]
            public long Id { get; set; }
            [Display(Name = "Filial")]
            public string Filial { get; set; }
            [Required]
            [Display(Name = "Id Empresa")]
            public long Id_Empresa { get; set; }
            [Display(Name = "Empresa")]
            public string Id_EmpresaView { get; set; }
            [Required]
            [Display(Name = "Percentual Estorno Frete")]
            public decimal Pct_Estorno_Frete { get; set; }
            public string Pct_Estorno_FreteView { get; set; }
            [Required]
            [Display(Name = "Percentual Desvalorização")]
            public decimal Pct_Desvalorizacao { get; set; }
            public string Pct_DesvalorizacaoView { get; set; }
            [Required]
            [MinLength(1)]
            [Display(Name = "Valor Minímo")]
            public decimal Vlr_Minimo_Envio { get; set; }
            public string Vlr_Minimo_EnvioView { get; set; }
            [Required]
            [MinLength(1)]
            [Display(Name = "Prazo Envio Automático")]
            public long Prazo_Envio_Automatico { get; set; }
            [Required]
            [MinLength(1)]
            [Display(Name = "Prazo Descarte")]
            public long Prazo_Descarte { get; set; }

            public string BotaoEvento { get; set; }
        }
        #endregion

        #region [GAR_REMESSA_USR]
        public class RemessaUsuario
        {
            [Display(Name = "Código")]
            public long Id { get; set; }

            [Required]
            [Display(Name = "Id Empresa")]
            public long Id_Empresa { get; set; }

            [Required]
            [Display(Name = "Código de Usuário")]
            public string Id_Usr { get; set; }

            public string Usuario { get; set; }
            public string Filial { get; set; }
            public string Nome { get; set; }
            public string Email { get; set; }

            public string BotaoEvento { get; set; }
        }
        #endregion

        #region [GAR_REMESSA_CONFIG]
        public class RemessaConfiguracao
        {
            [Display(Name = "Código")]
            public long Id { get; set; }
            [Display(Name = "Filial")]
            public string Filial { get; set; }
            [Required]
            [Display(Name = "Id Empresa")]
            public long Id_Empresa { get; set; }
            [Display(Name = "Empresa")]
            public string Id_EmpresaView { get; set; }
            [Required]
            [Display(Name = "Fornecedor")]
            public string Cod_Fornecedor { get; set; }
            [Required]
            [Display(Name = "Automática")]
            public long Automatica { get; set; }
            public string AutomaticaView { get; set; }
            [Required]
            [Display(Name = "Valor Minímo")]
            public decimal Vlr_Minimo { get; set; }
            public string Vlr_MinimoView { get; set; }
            [Required]
            [Display(Name = "Valor Total")]
            public long Total { get; set; }
            public string TotalView { get; set; }

            public string BotaoEvento { get; set; }
        }
        #endregion

        #region [GAR_FORN_GRUPO]
        public class FornecedorGrupo
        {
            [Display(Name = "Codigo")]
            public long Id { get; set; }

            [Required]
            [Display(Name = "Id Empresa")]
            public long Id_Empresa { get; set; }

            [Required]
            [Display(Name = "Fornecedor Pai")]
            public string Cod_Forn_Pai { get; set; }
            public string divFilhos { get; set; }

            [Required]
            [Display(Name = "Fornecedor Filho")]
            public string Cod_Forn_Filho { get; set; }

            public string BotaoEvento { get; set; }
        }
        #endregion

        #region [GAR_MOTIVO_LAUDO]
        public class MotivoLaudo
        {
            [Display(Name = "Codigo")]
            public long Id { get; set; }

            [Required]
            [Display(Name = "Id Tipo Motivo")]
            public string Id_Tipo { get; set; }

            [Display(Name = "Tipo Laudo")]
            public string TipoLaudo { get; set; }

            [Required]
            [Display(Name = "Motivo Laudo")]
            public string MotivoLaudoDescricao { get; set; }

            public string BotaoEvento { get; set; }
        }
        #endregion        

        #region Catálogo Comandos SQL
        public static class SQL
        {
            #region Validar Permissão de Cadastro do Usuário
            public static string ValidarPermissaoUsuario
            {
                get
                {
                    return String.Concat("SELECT COUNT(1) isTemPermissaoCadastro ",
                        "FROM tgfemp@sankhya te ",
                        "INNER JOIN \"Empresa\" e ON e.\"CodigoIntegracao\" = te.ad_codempgarantia ",
                        "INNER JOIN tsiemp@sankhya ts ON ts.codemp = te.codemp AND ts.ad_filial = e.\"Sigla\" ",
                        "WHERE e.\"IdEmpresa\" = :IdEmpresaUsuario ");
                }
            }
            #endregion

            #region Fornecedor Grupo
            public static string FornecedorGrupoListar
            {
                get
                {
                    return String.Concat("SELECT fg.Id, fp.\"RazaoSocial\" Cod_Forn_Pai, ff.\"RazaoSocial\" Cod_Forn_Filho ",
                        "FROM gar_forn_grupo fg ",
                        "INNER JOIN \"Fornecedor\" fp ON fp.cnpj = fg.cod_forn_pai ",
                        "INNER JOIN \"Fornecedor\" ff ON ff.cnpj = fg.cod_forn_filho ",
                        "GROUP BY fg.Id, fp.\"RazaoSocial\", ff.\"RazaoSocial\" ",
                        "ORDER BY fg.Id");
                }
            }

            /// <summary>
            /// {0} Código Fornecedor Pai | {1} Código Fornecedor Filho | {2} Id Empresa
            /// </summary>
            public static string FornecedorGrupoIncluir { get { return String.Concat("INSERT INTO gar_forn_grupo(Cod_Forn_Pai, Cod_Forn_Filho, Id_Empresa) VALUES(:Cod_Forn_Pai, :Cod_Forn_Filho, :Id_Empresa)"); } }
            #endregion

            #region Fornecedor Quebra
            public static string FornecedorQuebraListar
            {
                get
                {
                    return String.Concat("SELECT FQ.ID, FQ.COD_FORNECEDOR, F.\"NomeFantasia\", F.\"RazaoSocial\" ",
                        "FROM GAR_FORN_QUEBRA FQ ",
                        "INNER JOIN \"Fornecedor\" F ON LTRIM(RTRIM(F.cnpj)) = LTRIM(RTRIM(FQ.cod_fornecedor)) ",
                        "ORDER BY FQ.ID");
                }
            }

            /// <summary>
            /// {0} Código Fornecedor | {1} Id Empresa
            /// </summary>
            public static string FornecedorQuebraIncluir { get { return String.Concat("INSERT INTO gar_forn_quebra(Cod_Fornecedor, Id_Empresa) VALUES(:Cod_Fornecedor, :Id_Empresa)"); } }
            #endregion

            #region Remessa Configuração
            public static string RemessaConfiguracaoListar
            {
                get
                {
                    return String.Concat(
                        "SELECT 	rc.Id, rc.Id_Empresa, e.\"Sigla\" Filial, f.\"RazaoSocial\" Cod_Fornecedor, rc.Automatica, rc.Vlr_Minimo, rc.Total ",
                        "FROM gar_remessa_config rc ",
                        "INNER JOIN \"Fornecedor\" f ON TRIM(f.cnpj)=TRIM(rc.Cod_Fornecedor) ",
                        "INNER JOIN \"Empresa\" e ON rc.Id_Empresa=e.\"IdEmpresa\"");
                }
            }

            /// <summary>
            /// {0} Id_Empresa | {1} Cod_Fornecedor | {2} Automatica | {3} Vlr_Minimo | {4} Total
            /// </summary>
            public static string RemessaConfiguracaoIncluir
            {
                get
                {
                    return String.Concat("INSERT INTO gar_remessa_config(Id_Empresa, Cod_Fornecedor, Automatica, Vlr_Minimo, Total) ",
                  "VALUES(:Id_Empresa, :Cod_Fornecedor, :Automatica, :Vlr_Minimo, :Total)");
                }
            }
            #endregion

            #region Remessa Usuário
            public static string RemessaUsuarioListar
            {
                get
                {
                    return String.Concat(
                        "SELECT gru.Id, asp.\"UserName\" Usuario, e.\"NomeFantasia\" ||' ('|| e.\"Sigla\"||')' Filial, pu.\"Nome\", asp.\"Email\" Email ",
                        "FROM GAR_REMESSA_USR gru ",
                        "INNER JOIN \"PerfilUsuario\" pu ON gru.ID_USR = pu.\"UsuarioId\" ",
                        "INNER JOIN \"AspNetUsers\" asp ON gru.ID_USR = asp.\"Id\" ",
                        "INNER JOIN \"Empresa\" e ON e.\"IdEmpresa\" = pu.\"EmpresaId\" ");
                }
            }

            /// <summary>
            /// {0} Id Usuario | {1} Id Empresa
            /// </summary>
            public static string RemessaUsuarioIncluir { get { return String.Concat("INSERT INTO gar_remessa_usr(Id_Usr, Id_Empresa) VALUES(:Id_Usr, :Id_Empresa)"); } }
            #endregion

            #region Configuração
            public static string ConfiguracaoListar
            {
                get
                {
                    return String.Concat("SELECT c.Id, e.\"NomeFantasia\" ||' (' || e.\"Sigla\" ||')' Filial, c.Pct_Estorno_Frete, c.Pct_Desvalorizacao, ",
                        "c.Vlr_Minimo_Envio, c.Prazo_Envio_Automatico, c.Prazo_Descarte ",
                        "FROM gar_config c ",
                        "INNER JOIN \"Empresa\" e ON e.\"IdEmpresa\" = c.Id_Empresa");
                }
            }

            /// <summary>
            /// {0} Id Empresa
            /// </summary>
            public static string ConfiguracaoJaConsta
            {
                get
                {
                    return String.Concat("SELECT COUNT(1) FROM gar_config WHERE Id_Empresa = {0}");
                }
            }

            /// <summary>
            /// {0} Id_Empresa | {1} Pct_Estorno_Frete | {2} Pct_Desvalorizacao | {3} Vlr_Minimo_Envio | {4} Prazo_Envio_Automatico | {5} Prazo_Descarte
            /// </summary>
            public static string ConfiguracaoIncluir
            {
                get
                {
                    return String.Concat("INSERT INTO gar_config(Id_Empresa, Pct_Estorno_Frete, Pct_Desvalorizacao, Vlr_Minimo_Envio, Prazo_Envio_Automatico, Prazo_Descarte) ",
                        "VALUES(:Id_Empresa, :Pct_Estorno_Frete, :Pct_Desvalorizacao, :Vlr_Minimo_Envio, :Prazo_Envio_Automatico, :Prazo_Descarte)");
                }
            }

            /// <summary>
            /// {0} Percentagem Estorno Frete | {1} Percetagem Desvalorizacao | {2} Valor Minimo Envio | {3} Prazo Envio Automatico | {4} Prazo Descarte | {5} Id Empresa
            /// </summary>
            public static string ConfigurarAtualizar
            {
                get
                {
                    return String.Concat("UPDATE gar_config ",
                        "SET Pct_Estorno_Frete = :Pct_Estorno_Frete, Pct_Desvalorizacao = :Pct_Desvalorizacao, Vlr_Minimo_Envio = :Vlr_Minimo_Envio, ",
                        "Prazo_Envio_Automatico = :Prazo_Envio_Automatico, Prazo_Descarte = :Prazo_Descarte ",
                        "WHERE Id_Empresa = :Id_Empresa");
                }
            }
            #endregion

            #region Sankhya Top
            public static string SankhyaTopListar
            {
                get
                {
                    return String.Concat("SELECT gst.Id, gst.Top, gst.Descricao, gst.Id_Negociacao, tgf.DescrTipVenda Id_NegociacaoView, tgf.VendaMin, tgf.VendaMax ",
                        "FROM geral_sankhya_tops gst ",
                        "INNER JOIN tgftpv@sankhya tgf ON gst.Id_Negociacao = tgf.CodTipVenda ",
                        "GROUP BY gst.Id, gst.Top, gst.Descricao, gst.Id_Negociacao, tgf.DescrTipVenda, tgf.VendaMin, tgf.VendaMax");
                }
            }
            /// <summary>
            /// {0} Top | {1} Id Negociacao | {2} Id Registro 
            /// </summary>
            public static string SankhyaTopAtualizar { get { return String.Concat("UPDATE geral_sankhya_tops SET Top = :Top, Id_Negociacao = :Id_Negociacao WHERE Id = :Id"); } }
            #endregion

            #region Motivo Laudo
            public static string MotivoLaudoListar
            {
                get
                {
                    return String.Concat("SELECT 	ml.Id, ml.Id_Tipo, UPPER(gt.Descricao) TipoLaudo, ml.Descricao MotivoLaudoDescricao ",
                        "FROM gar_motivo_laudo ml ",
                        "INNER JOIN geral_tipo gt ON (gt.id = ml.Id_Tipo) ",
                        "WHERE ml.Id_Tipo = 8");
                }
            }

            /// <summary>
            /// {0} Id Tipo | {1} Descrição
            /// </summary>
            public static string MotivoLaudoIncluir { get { return String.Concat("INSERT INTO gar_motivo_laudo(Id_Tipo, Descricao) VALUES(:Id_Tipo, :Descricao)"); } }
            #endregion
        }
        #endregion        

        public static string modeloMenu
        {
            get
            {
                return String.Concat("<li class=\"nav-item active\"><a id=\"SankhyaTop\" class=\"nav-link\" data-toggle=\"tab\" href=\"#TabSankhyaTop\" role=\"tab\" aria-selected=\"false\">Sankhya Tops</a></li>",
              "<li class=\"nav-item\"><a id=\"Configuracao\" class=\"nav-link\" data-toggle=\"tab\" href=\"#TabConfiguracao\" role=\"tab\" aria-selected=\"false\">Configuração</a></li>",
              "<li class=\"nav-item\"><a id=\"MotivoLaudo\" class=\"nav-link\" data-toggle=\"tab\" href=\"#TabMotivoLaudo\" role=\"tab\" aria-selected=\"false\">Motivo Laudo</a></li>",
              "<li class=\"nav-item\"><a id=\"FornecedorQuebra\" class=\"nav-link\" data-toggle=\"tab\" href=\"#TabFornecedorQuebra\" role=\"tab\" aria-selected=\"true\" aria-expanded=\"true\" {0}>Fornecedor Quebra</a></li>",
              "<li class=\"nav-item\"><a id=\"RemessaConfiguracao\" class=\"nav-link\" data-toggle=\"tab\" href=\"#TabRemessaConfiguracao\" role=\"tab\" aria-selected=\"false\" {0}>Remessa Configuração</a></li>",
              "<li class=\"nav-item\"><a id=\"RemessaUsuario\" class=\"nav-link\" data-toggle=\"tab\" href=\"#TabRemessaUsuario\" role=\"tab\" aria-selected=\"false\" {0}>Remessa Usuário</a></li>",
              "<li class=\"nav-item\"><a id=\"RemessaUsuario\" class=\"nav-link\" data-toggle=\"tab\" href=\"#TabRemessaUsuario\" role=\"tab\" aria-selected=\"false\" {0}>Remessa Usuário</a></li>");
            }
        }

    }
}
