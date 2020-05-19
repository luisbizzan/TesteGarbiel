using Dapper;
using ExtensionMethods.String;
using FWLog.Data.Models;
using FWLog.Data.Models.DataTablesCtx;
using FWLog.Data.Models.FilterCtx;
using FWLog.Data.Repository.CommonCtx;
using Microsoft.Win32.SafeHandles;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;

namespace FWLog.Data.Repository.GeneralCtx
{
    public class GarantiaEtiquetaRepository : GenericRepository<Garantia>
    {
        #region Contexto
        public GarantiaEtiquetaRepository(Entities entities) : base(entities) { }
        #endregion

        #region Imprimir Etiqueta
        private static Boolean ImprimirEtiqueta(GarantiaEtiqueta.DocumentoImpressao Impressao)
        {
            try
            {
                #region Validações
                Impressao.EnderecoIP = String.IsNullOrEmpty(Impressao.EnderecoIP) ? "10.201.0.155" : Impressao.EnderecoIP;
                if (String.IsNullOrEmpty(Impressao.EnderecoIP)) throw new Exception("Endereço de IP não pode estar em branco!");
                if (Impressao == null) throw new Exception("Documento de impressão inválido!");
                if (String.IsNullOrEmpty(Impressao.ConteudoImpressao.ToString())) throw new Exception("Etiqueta não pode estar em branco!");
                if (!Enum.IsDefined(typeof(GarantiaEtiqueta.ETIQUETA), Impressao.TipoEtiqueta)) throw new Exception("Tipo de etiqueta não é válido!");
                #endregion

                #region Processar Impressão
                var iRetorno = 0;
                using (var ping = new Ping())
                {
                    if (ping.Send(Impressao.EnderecoIP).Status == IPStatus.Success)
                        using (var comandoImpressao = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
                        {
                            comandoImpressao.NoDelay = true;
                            var bytesConteudoImpressao = Encoding.ASCII.GetBytes(Impressao.ConteudoImpressao.ToString());
                            var IP = IPAddress.Parse(Impressao.EnderecoIP);
                            var IPComPorta = new IPEndPoint(IP, Impressao.PortaConexao.Equals(0) ? 9100 : Impressao.PortaConexao);
                            comandoImpressao.Connect(IPComPorta);
                            iRetorno = comandoImpressao.Send(bytesConteudoImpressao);
                        }
                    else
                        throw new Exception(String.Format("Impressora {0} Offline!", Impressao.EnderecoIP));
                }
                #endregion

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region Gerar Etiqueta
        public void ProcessarImpressaoEtiqueta(GarantiaEtiqueta.DocumentoImpressao dadosImpressao)
        {
            try
            {
                dadosImpressao.ConteudoImpressao = new StringBuilder();

                #region Inicio
                dadosImpressao.ConteudoImpressao.AppendLine("^XA");
                dadosImpressao.ConteudoImpressao.AppendLine("^LH32,0");
                dadosImpressao.ConteudoImpressao.AppendLine("^PRA^FS");
                #endregion

                #region Devolução
                if (dadosImpressao.TipoEtiqueta == GarantiaEtiqueta.ETIQUETA.Devolucao)
                {
                    //Codigo Barras 1
                    dadosImpressao.ConteudoImpressao.AppendLine("^FO0,36^GB146,26,30^FS");
                    //Codigo Barras 2
                    dadosImpressao.ConteudoImpressao.AppendLine("^FO272,66^GB146,26,30^FS");
                    //Codigo Barras 3
                    dadosImpressao.ConteudoImpressao.AppendLine("^FO544,66^GB146,26,30^FS");

                    //Coluna 1
                    dadosImpressao.ConteudoImpressao.AppendLine(String.Format(GarantiaEtiqueta.EtiquetaDevolucao.PrimeiraColuna, "ZEN501", "01.A.30.01", "PEDAL DA PARAFUSETA", "12545152452582", "12"));

                    //Coluna 2
                    dadosImpressao.ConteudoImpressao.AppendLine(String.Format(GarantiaEtiqueta.EtiquetaDevolucao.SegundaColuna, "ZEN501", "01.A.30.01", "PEDAL DA PARAFUSETA", "12545152452582", "12"));

                    //Coluna 3
                    dadosImpressao.ConteudoImpressao.AppendLine(String.Format(GarantiaEtiqueta.EtiquetaDevolucao.TerceiraColuna, "ZEN501", "01.A.30.01", "PEDAL DA PARAFUSETA", "12545152452582", "12"));
                }
                #endregion

                #region Garantia
                if (dadosImpressao.TipoEtiqueta == GarantiaEtiqueta.ETIQUETA.Garantia)
                {
                    //Codigo Barras 1
                    dadosImpressao.ConteudoImpressao.AppendLine("^FO000,45^GB146,26,30^FS");
                    //Codigo Barras 2
                    dadosImpressao.ConteudoImpressao.AppendLine("^FO272,45^GB146,26,30^FS");
                    //Codigo Barras 3
                    dadosImpressao.ConteudoImpressao.AppendLine("^FO544,45^GB146,26,30^FS");

                    //Coluna 1
                    dadosImpressao.ConteudoImpressao.AppendLine(String.Format(GarantiaEtiqueta.EtiquetaGarantia.PrimeiraColuna, "ZEN501", "01.A.30.01", "PEDAL DA PARAFUSETA", "12545152452582", "12"));

                    //Coluna 2
                    dadosImpressao.ConteudoImpressao.AppendLine(String.Format(GarantiaEtiqueta.EtiquetaGarantia.SegundaColuna, "ZEN501", "01.A.30.01", "PEDAL DA PARAFUSETA", "12545152452582", "12"));

                    //Coluna 3
                    dadosImpressao.ConteudoImpressao.AppendLine(String.Format(GarantiaEtiqueta.EtiquetaGarantia.TerceiraColuna, "ZEN501", "01.A.30.01", "PEDAL DA PARAFUSETA", "12545152452582", "12"));
                }
                #endregion

                #region Final
                dadosImpressao.ConteudoImpressao.AppendLine("^XZ");
                dadosImpressao.ConteudoImpressao.AppendLine(" ");
                #endregion

                var retornoProcessamento = ImprimirEtiqueta(dadosImpressao);
            }
            catch (Exception erro)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine(erro.Message);
            }
        }
        #endregion

    }
}
