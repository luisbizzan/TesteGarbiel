﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Web.Script.Serialization;

namespace FWLog.Data.Models
{
    public class GarantiaEtiqueta
    {

        #region Documento Impressao
        public class DocumentoImpressao
        {
            public string EnderecoIP { get; set; }
            public int PortaConexao { get; set; }
            public ETIQUETA TipoEtiqueta { get; set; }
            public StringBuilder ConteudoImpressao { get; set; }
            public List<ItemEtiqueta> ListaImprimir { get; set; }
            public List<string> IdsEtiquetasImprimir { get; set; }
            public string ComandoSQL { get; set; }
        }
        #endregion

        public static class SQL
        {
            public static string ConsultaEtiquetas
            {
                get
                {
                    return String.Concat("SELECT s.id CodigoEtiqueta, si.id CodigoRegistro, t.descricao TipoEtiqueta, si.refx, null Localizacao, p.descrprod Descricao, ",
                        "LPAD(TO_CHAR(s.id), 10) || RPAD(si.id_prod_skw, 10) CodigoBarras, si.quant QtdeEmbalagem ",
                        "FROM gar_solicitacao s ",
                        "INNER JOIN gar_solicitacao_item si ON (si.id_solicitacao = s.id) ",
                        "INNER JOIN geral_tipo t ON (t.id = s.id_tipo) ",
                        "INNER JOIN tgfpro@sankhya p ON (p.ad_refx = si.refx) ",
                        "{0} ",
                        "ORDER BY s.id ASC");
                }
            }
        }

        #region Colunas Banco Dados
        public class ItemEtiqueta
        {
            public int CodigoEtiqueta { get; set; }
            public int CodigoRegistro { get; set; }
            public ETIQUETA TipoEtiqueta { get; set; }

            public string Refx { get; set; }
            public string Localizacao { get; set; }
            public string Descricao { get; set; }
            public string CodigoBarras { get; set; }
            public int QtdeEmbalagem { get; set; }
            public int CodigoSolicitacao { get; set; }
            public string Cliente { get; set; }
            public string Representante { get; set; }
        }
        #endregion

        #region Enums
        public enum ETIQUETA { Devolução, Garantia }
        public enum EtiquetaDevolucaoCodigoBarras { CodigoBarrasDOIS, CodigoBarrasTRES }
        public enum EtiquetaDevolucaoMiolo { CodigoReferenciaUM, CodigoReferenciaDOIS, CodigoReferenciaTRES }
        #endregion

        #region Modelo Etiqueta Devolução
        /// <summary>
        ///  {0} Referência | {1} Localização | {2} Descrição | {3} Código Barras | {4} Qtde. Embalagem
        /// </summary>
        public static class EtiquetaDevolucao
        {
            #region Coluna I
            public static string PrimeiraColuna
            {
                get
                {
                    return String.Concat(
                        "^FO0,40^A0N,30,20^FR^FD{0}^FS",
                        "^FO154,42^A0N,24,20^FD{1}^FS",
                        "^FO16,68^A0N,16,16^FD{2}^FS",
                        "^FO34,82^BY2,,164^BEN,52,Y,N^FD{3}^FS",
                        "^FO016,164^A0N,8,24^FDQuant.p/emb.: {4}^FS");
                }
            }
            #endregion

            #region Coluna II
            public static string SegundaColuna
            {
                get
                {
                    return String.Concat(
                        "^FO272,40^A0N,30,20^FR^FD{0}^FS",
                        "^FO426,42^A0N,24,20^FD{1}^FS",
                        "^FO288,68^A0N,16,16^FD{2}^FS",
                        "^FO306,82^BY2,,164^BEN,52,Y,N^FD{3}^FS",
                        "^FO288,164^A0N,8,24^FDQuant.p/emb.: {4}^FS");
                }
            }
            #endregion

            #region Coluna III
            public static string TerceiraColuna
            {
                get
                {
                    return String.Concat(
                        "^FO544,40^A0N,30,20^FR^FD{0}^FS",
                        "^FO698,42^A0N,24,20^FD{1}^FS",
                        "^FO560,68^A0N,16,16^FD{2}^FS",
                        "^FO578,82^BY2,,164^BEN,52,Y,N^FD{3}^FS",
                        "^FO560,164^A0N,8,24^FDQuant.p/emb.: {4}^FS");
                }
            }
            #endregion
        }
        #endregion

        #region Modelo Etiqueta Garantia
        /// <summary>
        ///  {0} Referência | {1} N� Solicitação | {2} Descrição | {3} Código Barras | {4} Cliente | {5} Representante
        /// </summary>
        public static class EtiquetaGarantia
        {
            #region Coluna I
            public static string PrimeiraColuna
            {
                get
                {
                    return String.Concat(
                        "^FO000,49^A0N,30,25^FR^FD{0}^FS",
                        "^FO154,45^A0N,32,25^FD{1}^FS",
                        "^FO006,78^A0N,16,16^FD{2}^FS",
                        "^FO001,91^BY1,,100^BCN,73,N,N^FD{3}{1}^FS",
                        "^FO006,165^A0N,15,24^FDC.{4} R.{5}^FS");
                }
            }
            #endregion

            #region Coluna II
            public static string SegundaColuna
            {
                get
                {
                    return String.Concat(
                        "^FO272,49^A0N,30,25^FR^FD{0}^FS",
                        "^FO426,45^A0N,32,25^FD{1}^FS",
                        "^FO278,78^A0N,16,16^FD{2}^FS",
                        "^FO272,91^BY1,,100^BCN,73,N,N^FD{3}{1}^FS",
                        "^FO278,165^A0N,15,24^FDC.{4} R.{5}^FS");
                }
            }
            #endregion

            #region Coluna III
            public static string TerceiraColuna
            {
                get
                {
                    return String.Concat(
                        "^FO544,49^A0N,30,25^FR^FD{0}^FS",
                        "^FO698,45^A0N,32,25^FD{1}^FS",
                        "^FO550,78^A0N,16,16^FD{2}^FS",
                        "^FO544,91^BY1,,100^BCN,73,N,N^FD{3}{1}^FS",
                        "^FO550,165^A0N,15,24^FDC.{4} R.{5}^FS");
                }
            }
            #endregion
        }
        #endregion
    }
}