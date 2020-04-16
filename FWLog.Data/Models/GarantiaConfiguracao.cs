using System;
using System.Collections.Generic;

namespace FWLog.Data.Models
{
    public class GarantiaConfiguracao
    {
        public long Id { get; set; }
        public string Cod_Fornecedor { get; set; }
        public string[] Codigos { get; set; }
        public string Value { get; set; }
        public string NomeFantasia { get; set; }
        public string RazaoSocial { get; set; }

        public string Tag { get; set; }
        public string BotaoEvento { get; set; }

        public string Descricao { get; set; }
        public string Top { get; set; }

        public static string GridNome { get; set; }
        public static object[] GridColunas { get; set; }

        public enum TagsValidas { REM_CONFIG, REM_USUARIO, CONFIG, FORN_QUEBRA, SANKHYA_TOP }

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
                    {"FORN_QUEBRA", ConsultaSQL.ListarFornecedorQuebra },
                    {"SANKHYA_TOP", ConsultaSQL.ListarSankhyaTop },
                    {"CONFIG", ConsultaSQL.ListarConfiguracao },
                    {"REM_CONFIG", ConsultaSQL.ListarRemessaConfiguracao },
                    {"REM_USUARIO", ConsultaSQL.ListarRemessaUsuario },
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

        #region Catálogo com Consultas SQL 
        public static class ConsultaSQL
        {
            #region Consulta Fornecedor Quebra
            public static string ListarFornecedorQuebra
            {
                get
                {
                    return String.Concat("SELECT FQ.ID, FQ.COD_FORNECEDOR, F.\"NomeFantasia\", F.\"RazaoSocial\" ",
                        "FROM GAR_FORN_QUEBRA FQ ",
                        "INNER JOIN \"Fornecedor\" F ON LTRIM(RTRIM(F.cnpj)) = LTRIM(RTRIM(FQ.cod_fornecedor)) ",
                        "ORDER BY FQ.ID");
                }
            }
            #endregion

            #region Consulta Remessa Configuração
            public static string ListarRemessaConfiguracao { get { return "SELECT Id, Id_Filial_Sankhya, Filial, Cod_Fornecedor, Automatica, Vlr_Minimo, Total  FROM gar_remessa_config"; } }
            #endregion

            #region Consulta Remessa Usuário
            public static string ListarRemessaUsuario { get { return "SELECT Id, Id_Usr FROM gar_remessa_usr"; } }
            #endregion

            #region Consulta Configuração
            public static string ListarConfiguracao { get { return "SELECT Id, Id_Filial_Sankhya, Filial, Pct_Estorno_Frete, Pct_Desvalorizacao, Vlr_Minimo_Envio, Prazo_Envio_Automatico, Prazo_Descarte FROM gar_config"; } }
            #endregion

            #region Consulta Sankhya Top
            public static string ListarSankhyaTop { get { return "SELECT Id, Descricao, Top FROM geral_sankhya_tops"; } }
            #endregion
        }
        #endregion
    }
}
