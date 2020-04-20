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
        public string Tag { get; set; }

        [Display(Name = "Código")]
        public long Id { get; set; }

        public List<FornecedorQuebra> RegistroFornecedorQuebra { get; set; }
        public List<SankhyaTop> RegistroSankhyaTop { get; set; }
        public List<Configuracao> RegistroConfiguracao { get; set; }
        public List<RemessaUsuario> RegistroRemessaUsuario { get; set; }
        public List<RemessaConfiguracao> RegistroRemessaConfiguracao { get; set; }

        public string BotaoEvento { get; set; }

        #region [FORNECEDOR QUEBRA] AutoComplete
        public class AutoComplete
        {
            public long Data { get; set; }
            public string Value { get; set; }
        }
        #endregion

        #region [FORNECEDOR QUEBRA]
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
            [Display(Name = "Codigo")]
            public long Id { get; set; }
            public long Id_Filial_Sankhya { get; set; }
            public string Filial { get; set; }
            public long Pct_Estorno_Frete { get; set; }
            public long Pct_Desvalorizacao { get; set; }
            public decimal Vlr_Minimo_Envio { get; set; }
            public long Prazo_Envio_Automatico { get; set; }
            public long Prazo_Descarte { get; set; }

            public string BotaoEvento { get; set; }
        }
        #endregion

        #region [GAR_REMESSA_USR]
        public class RemessaUsuario
        {
            [Display(Name = "Codigo")]
            public long Id { get; set; }
            public string Id_Usr { get; set; }

            public string BotaoEvento { get; set; }
        }
        #endregion

        #region [GAR_REMESSA_CONFIG]
        public class RemessaConfiguracao
        {
            [Display(Name = "Codigo")]
            public long Id { get; set; }
            public long Id_Filial_Sankhya { get; set; }
            public string Filial { get; set; }
            public string Cod_Fornecedor { get; set; }
            public long Automatica { get; set; }
            public long Vlr_Minimo { get; set; }
            public long Total { get; set; }

            public string BotaoEvento { get; set; }
        }
        #endregion

        public static string GridNome { get; set; }
        public static object[] GridColunas { get; set; }
        public enum TAG { REM_CONFIG, REM_USUARIO, CONFIG, FORN_QUEBRA, SANKHYA_TOP }

        /// <summary>
        /// Dicionário de TAGs
        /// </summary>
        public static Dictionary<int, string> DicTagsValidas
        {
            get { return new Dictionary<int, string>() { { 0, "REM_CONFIG" }, { 1, "REM_USUARIO" }, { 2, "CONFIG" }, { 3, "FORN_QUEBRA" }, { 4, "SANKHYA_TOP" } }; }
        }

        /// <summary>
        /// Dicionáriode TAG x Consultas SQL
        /// </summary>
        public static Dictionary<string, string> DicTagConsultaSQL
        {
            get
            {
                return new Dictionary<string, string>()
                {
                    {"FORN_QUEBRA", SQL.FornecedorQuebraListar },
                    {"SANKHYA_TOP", SQL.SankhyaTopListar },
                    {"CONFIG", SQL.ConfiguracaoListar },
                    {"REM_CONFIG", SQL.RemessaConfiguracaoListar },
                    {"REM_USUARIO", SQL.RemessaUsuarioListar },
                };
            }
        }

        /// <summary>
        /// Dicionario de TAG x Nome Grid View
        /// </summary>
        public static Dictionary<string, string> DicTagGridNome
        {
            get
            {
                return new Dictionary<string, string>()
                {
                    {"FORN_QUEBRA", "#gridFornecedorQuebra" },
                    {"SANKHYA_TOP", "#gridSankhyaTop" },
                    {"CONFIG", "#gridConfiguracao" },
                    {"REM_CONFIG", "#gridRemessaConfig" },
                    {"REM_USUARIO", "#gridRemessaUsuario" },
                };
            }
        }

        /// <summary>
        /// Dicionário TAG x Colunas
        /// </summary>
        public static Dictionary<string, object> DicTagGridColuna
        {
            get
            {
                return new Dictionary<string, object>()
                {
                    {"FORN_QUEBRA", new object[]
                    {
                        new { data = "BotaoEvento" }, new { data = "Id", title = "Id Registro" }, new { data = "Cod_Fornecedor", title = "CNPJ" },
                        new { data = "NomeFantasia", title =  "Nome Fantasia" }, new { data = "RazaoSocial", title = "Razão Social" }
                    }},
                    {"SANKHYA_TOP", new object[]  { new { data = "BotaoEvento" }, new { data = "Id", title = "Id Registro" }, new { data = "Top", title = "Top" }, new { data = "Descricao", title = "Descrição" } }},
                    {"REM_USUARIO", new object[] { new { data = "BotaoEvento" }, new { data = "Id", title = "Id Registro" }, new { data = "Id_Usr", title = "Id Usuário" } } },
                    {"REM_CONFIG", new object[]
                    {
                        new { data = "BotaoEvento" }, new { data = "Id", title = "Id Registro" }, new { data = "Id_Filial_Sankhya", title = "Id Filial Sankhya" }, new { data = "Filial", title = "Filial" },
                        new { data = "Cod_Fornecedor", title = "Código Fornecedor" }, new { data = "Automatica", title = "Automática" }, new { data = "Vlr_Minimo", title = "Valor Minímo" },
                        new { data = "Total", title = "Total" }
                    }},
                    {"CONFIG", new object[]
                    {
                        new { data = "BotaoEvento" }, new { data = "Id", title = "Id Registro" }, new { data = "Id_Filial_Sankhya", title = "Id Filial Sankhya" }, new { data = "Filial", title = "Filial" },
                        new { data = "Pct_Estorno_Frete", title = "Estorno Frete" }, new { data = "Pct_Desvalorizacao", title = "Desvalorização" }, new { data = "Vlr_Minimo_Envio", title = "Valor Minímo Envio" },
                        new { data = "Prazo_Envio_Automatico", title = "Prazo Envio Automático" }, new { data = "Prazo_Descarte", title = "Prazo Descarte" }
                    }},
                };
            }
        }

        /// <summary>
        /// {0} Tag(Tabela) | {1} Id Registro
        /// </summary>
        public static string botaoExcluirTemplate
        {
            get { return "<button type=\"button\" class=\"btn btn-danger\" onclick=\"RegistroExcluir('{0}',{1});\"><i class=\"fa fa-trash-o\" data-toggle=\"tooltip\" data-original-title=\"Excluir Registro\"></i></button>"; }
        }

        #region Catálogo Comandos SQL
        public static class SQL
        {
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
            public static string RemessaConfiguracaoListar { get { return "SELECT Id, Id_Filial_Sankhya, Filial, Cod_Fornecedor, Automatica, Vlr_Minimo, Total  FROM gar_remessa_config"; } }

            /// <summary>
            /// {0} Id_Filial_Sankhya | {1} Filial | {2} Cod_Fornecedor | {3} Automatica | {4} Vlr_Minimo | {5} Total
            /// </summary>
            public static string RemessaConfiguracaoIncluir { get { return String.Concat("INSERT INTO gar_remessa_config(Id_Filial_Sankhya, Filial, Cod_Fornecedor, Automatica, Vlr_Minimo, Total) VALUES({0}, '{1}', '{2}', {3}, {4}, {5})"); } }
            #endregion

            #region Remessa Usuário
            public static string RemessaUsuarioListar { get { return "SELECT Id, Id_Usr FROM gar_remessa_usr"; } }

            /// <summary>
            /// {0} Id_Usr
            /// </summary>
            public static string RemessaUsuarioIncluir { get { return String.Concat("INSER INTO gar_remessa_usr(Id_Usr) VALUES({0})"); } }
            #endregion

            #region Configuração
            public static string ConfiguracaoListar { get { return "SELECT Id, Id_Filial_Sankhya, Filial, Pct_Estorno_Frete, Pct_Desvalorizacao, Vlr_Minimo_Envio, Prazo_Envio_Automatico, Prazo_Descarte FROM gar_config"; } }

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
        }
        #endregion

        public static JavaScriptSerializer SerializarJS = new JavaScriptSerializer() { MaxJsonLength = 9999999 };
    }
}
