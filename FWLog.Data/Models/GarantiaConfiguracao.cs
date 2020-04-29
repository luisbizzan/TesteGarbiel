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
        public List<AutoComplete> ListaAutoComplete { get; set; }
        #endregion

        public string BotaoEvento { get; set; }

        #region [GENÉRICO] AutoComplete
        public class AutoComplete
        {
            public string Data { get; set; }
            public string Value { get; set; }

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
            [Display(Name = "Código do Fornecedor")]
            public string Cod_Fornecedor { get; set; }

            [Display(Name = "Nome Fantasia")]
            public string NomeFantasia { get; set; }

            [Display(Name = "Razão Social")]
            public string RazaoSocial { get; set; }

            public string BotaoEvento { get; set; }
        }
        #endregion

        #region [GERAL_SANKHYA_TOPS]
        public class SankhyaTop
        {
            [Display(Name = "Codigo")]
            public long Id { get; set; }
            [Required]
            [Display(Name = "Nome Top")]
            public string Top { get; set; }
            [Required]
            [Display(Name = "Descrição da Top")]
            public string Descricao { get; set; }

            public string BotaoEvento { get; set; }
        }
        #endregion

        #region [GAR_CONFIG]
        public class Configuracao
        {
            [Display(Name = "Código")]
            public long Id { get; set; }
            [Required]

            [Display(Name = "Código Filial")]
            public long Id_Filial_Sankhya { get; set; }
            [Required]
            [Display(Name = "Filial")]
            public string Filial { get; set; }
            [Required]
            [Display(Name = "Percentual Estorno Frete")]
            public long Pct_Estorno_Frete { get; set; }
            public string Pct_Estorno_FreteView { get; set; }
            [Required]
            [Display(Name = "Percentual Desvalorização")]
            public long Pct_Desvalorizacao { get; set; }
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
            [Display(Name = "Código de Usuário")]
            public string Id_Usr { get; set; }

            public string Usuario { get; set; }
            public string Email { get; set; }

            public string BotaoEvento { get; set; }
        }
        #endregion

        #region [GAR_REMESSA_CONFIG]
        public class RemessaConfiguracao
        {
            [Display(Name = "Código")]
            public long Id { get; set; }
            [Required]
            [Display(Name = "Código Filial")]
            public long Id_Filial_Sankhya { get; set; }
            [Required]
            [Display(Name = "Filial")]
            public string Filial { get; set; }
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

            public string BotaoEvento { get; set; }
        }
        #endregion

        #region [GAR_FORN_GRUPO]
        public class FornecedorGrupo
        {
            [Display(Name = "Codigo")]
            public long Id { get; set; }

            [Required]
            [Display(Name = "Fornecedor Pai")]
            public string Cod_Forn_Pai { get; set; }

            [Required]
            [Display(Name = "Fornecedor Filho")]
            public string Cod_Forn_Filho { get; set; }

            public string BotaoEvento { get; set; }
        }
        #endregion

        public static string GridNome { get; set; }
        public static object[] GridColunas { get; set; }
        public enum GarantiaTag { RemessaConfiguracao, RemessaUsuario, Configuracao, FornecedorQuebra, SankhyaTop, FornecedorGrupo }
        public enum AutoCompleteTag { Fornecedor, Filial }

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
                    { 5, GarantiaTag.FornecedorGrupo.ToString() }};
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
                    {GarantiaTag.FornecedorGrupo, SQL.FornecedorGrupoListar }
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
                    {GarantiaTag.FornecedorGrupo , "#gridFornecedorGrupo" }
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
                    {GarantiaTag.FornecedorQuebra, new object[]
                    {
                        new { data = "BotaoEvento" }, new { data = "Id", title = "Id Registro" }, new { data = "Cod_Fornecedor", title = "CNPJ" },
                        new { data = "NomeFantasia", title =  "Nome Fantasia" }, new { data = "RazaoSocial", title = "Razão Social" }
                    }},
                    {GarantiaTag.SankhyaTop, new object[]  { new { data = "BotaoEvento" }, new { data = "Id", title = "Id Registro" }, new { data = "Top", title = "Top" }, new { data = "Descricao", title = "Descrição" } }},
                    {GarantiaTag.RemessaUsuario, new object[] { new { data = "BotaoEvento" }, new { data = "Id", title = "Id Registro" }, new { data = "Usuario", title = "Usuário" }, new { data = "Email", title = "E-mail" } } },
                    {GarantiaTag.RemessaConfiguracao, new object[]
                    {
                        new { data = "BotaoEvento" }, new { data = "Id", title = "Id Registro" }, new { data = "Id_Filial_Sankhya", title = "Id Filial" }, new { data = "Filial", title = "Filial" },
                        new { data = "Cod_Fornecedor", title = "Fornecedor (CNPJ)" }, new { data = "AutomaticaView", title = "Automática" }, new { data = "Vlr_MinimoView", title = "R$ Minímo" },
                        new { data = "Total", title = "Total" }
                    }},
                    {GarantiaTag.Configuracao, new object[]
                    {
                        new { data = "BotaoEvento" }, new { data = "Id", title = "Id Registro" }, new { data = "Id_Filial_Sankhya", title = "Id Filial Sankhya" }, new { data = "Filial", title = "Filial" },
                        new { data = "Pct_Estorno_FreteView", title = "Estorno Frete" }, new { data = "Pct_DesvalorizacaoView", title = "Desvalorização" }, new { data = "Vlr_Minimo_EnvioView", title = "R$ Minímo Envio" },
                        new { data = "Prazo_Envio_Automatico", title = "Prazo Envio Automático" }, new { data = "Prazo_Descarte", title = "Prazo Descarte" }
                    }},
                    {GarantiaTag.FornecedorGrupo, new object[]
                    {
                        new { data = "BotaoEvento" }, new { data = "Id", title = "Id Registro" }, new { data = "Cod_Forn_Pai", title = "Código Pai" }, new { data = "Cod_Forn_Filho", title = "Código Filho" },
                    }
                    }
                };
            }
        }

        /// <summary>
        /// {0} Tag(Tabela) | {1} Id Registro
        /// </summary>
        public static string botaoExcluirTemplate
        {
            get { return "<a onclick=\"RegistroExcluir('{0}',{1});\" class=\"btn btn-link btn-row-actions\" data-toggle=\"tooltip\" data-placement=\"top\" data-original-title=\"Excluir Registro\"><i class=\"glyphicon glyphicon-trash text-danger\"></i></a>"; }
        }

        #region Catálogo Comandos SQL
        public static class SQL
        {
            #region Fornecedor Grupo
            public static string FornecedorGrupoListar
            {
                get
                {
                    return String.Concat("SELECT fg.Id, fp.\"RazaoSocial\" Cod_Forn_Pai, ff.\"RazaoSocial\" Cod_Forn_Filho ",
                        "FROM gar_forn_grupo fg ",
                        "INNER JOIN \"Fornecedor\" fp ON fp.cnpj = fg.cod_forn_pai ",
                        "INNER JOIN \"Fornecedor\" ff ON ff.cnpj = fg.cod_forn_filho ",
                        "ORDER BY fg.Id");
                }
            }

            /// <summary>
            /// {0} Código Fornecedor Pai | {1} Código Fornecedor Filho
            /// </summary>
            public static string FornecedorGrupoIncluir { get { return String.Concat("INSERT INTO gar_forn_grupo(Cod_Forn_Pai, Cod_Forn_Filho) VALUES('{0}','{1}')"); } }
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
            /// {0} Código Fornecedor
            /// </summary>
            public static string FornecedorQuebraIncluir { get { return String.Concat("INSERT INTO gar_forn_quebra(Cod_Fornecedor) VALUES('{0}')"); } }
            #endregion

            #region Remessa Configuração
            public static string RemessaConfiguracaoListar
            {
                get
                {
                    return String.Concat(
                        "SELECT 	rc.Id, rc.Id_Filial_Sankhya, rc.Filial, f.\"RazaoSocial\" Cod_Fornecedor, rc.Automatica, rc.Vlr_Minimo, rc.Total ",
                        "FROM gar_remessa_config rc ",
                        "INNER JOIN \"Fornecedor\" f ON TRIM(f.cnpj)=TRIM(rc.Cod_Fornecedor)");
                }
            }

            /// <summary>
            /// {0} Id_Filial_Sankhya | {1} Filial | {2} Cod_Fornecedor | {3} Automatica | {4} Vlr_Minimo | {5} Total
            /// </summary>
            public static string RemessaConfiguracaoIncluir
            {
                get { return String.Concat("INSERT INTO gar_remessa_config(Id_Filial_Sankhya, Filial, Cod_Fornecedor, Automatica, Vlr_Minimo, Total) VALUES({0}, '{1}', {2}, {3}, {4}, {5})"); }
            }
            #endregion

            #region Remessa Usuário
            public static string RemessaUsuarioListar
            {
                get
                {
                    return String.Concat(
                        "SELECT usr.Id, asp.\"UserName\" Usuario, asp.\"Email\" Email ",
                        "FROM \"AspNetUsers\" asp ",
                        "INNER JOIN gar_remessa_usr usr ON usr.Id_Usr = asp.\"Id\"");
                }
            }

            /// <summary>
            /// {0} Id_Usr
            /// </summary>
            public static string RemessaUsuarioIncluir { get { return String.Concat("INSERT INTO gar_remessa_usr(Id_Usr) VALUES('{0}')"); } }
            #endregion

            #region Configuração
            public static string ConfiguracaoListar
            {
                get
                {
                    return String.Concat("SELECT c.Id, c.Id_Filial_Sankhya, e.\"NomeFantasia\" ||' (' || c.Filial ||')' Filial, c.Pct_Estorno_Frete, c.Pct_Desvalorizacao, ",
                        "c.Vlr_Minimo_Envio, c.Prazo_Envio_Automatico, c.Prazo_Descarte ",
                        "FROM gar_config c ",
                        "INNER JOIN \"Empresa\" e ON e.\"IdEmpresa\" = c.Id_Filial_Sankhya");
                }
            }

            /// <summary>
            /// {0} Id_Filial_Sankhya | {1} Filial | {2} Pct_Estorno_Frete | {3} Pct_Desvalorizacao | {4} Vlr_Minimo_Envio | {5} Prazo_Envio_Automatico | {6} Prazo_Descarte
            /// </summary>
            public static string ConfiguracaoIncluir
            {
                get
                {
                    return String.Concat("INSERT INTO gar_config(Id_Filial_Sankhya, Filial, Pct_Estorno_Frete, Pct_Desvalorizacao, Vlr_Minimo_Envio, Prazo_Envio_Automatico, Prazo_Descarte) ",
                        "VALUES({0}, '{1}', {2}, {3}, {4}, {5}, {6})");
                }
            }
            #endregion

            #region Sankhya Top
            public static string SankhyaTopListar { get { return "SELECT Id, Descricao, Top FROM geral_sankhya_tops"; } }
            /// <summary>
            /// {0} Top | {1} Descricao 
            /// </summary>
            public static string SankhyaTopIncluir { get { return String.Concat("INSERT INTO geral_sankhya_tops(Top, Descricao) VALUES('{0}', '{1}')"); } }
            #endregion

            #region Fornecedor Grupo
            
            #endregion
        }
        #endregion

        public static JavaScriptSerializer SerializarJS = new JavaScriptSerializer() { MaxJsonLength = 9999999 };
    }
}
