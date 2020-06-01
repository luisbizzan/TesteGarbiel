using Dapper;
using FWLog.Data.Models;
using FWLog.Data.Repository.CommonCtx;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace FWLog.Data.Repository.GeneralCtx
{
    public class GarantiaEtiquetaRepository : GenericRepository<Garantia>
    {
        #region Contexto
        public GarantiaEtiquetaRepository(Entities entities) : base(entities) { }
        #endregion

        #region Imprimir Etiqueta
        private static void ImprimirEtiqueta(GarantiaEtiqueta.DocumentoImpressao Impressao)
        {
            try
            {
                #region Validações
                if (String.IsNullOrEmpty(Impressao.EnderecoIP)) throw new Exception("Endereço de IP não pode estar em branco!");
                if (Impressao.PortaConexao.Equals(0)) throw new Exception("Porta de Conexão da Impressora não pode ser Zero (0)!");
                if (Impressao == null) throw new Exception("Documento de impressão inválido!");
                if (String.IsNullOrEmpty(Impressao.ConteudoImpressao.ToString())) throw new Exception("Etiqueta não pode estar em branco!");
                if (!Enum.IsDefined(typeof(GarantiaEtiqueta.ETIQUETA), Impressao.TipoEtiqueta)) throw new Exception("Tipo de etiqueta não é válido!");
                #endregion

                #region Processar Impressão
                using (var ping = new Ping())
                {
                    if (ping.Send(Impressao.EnderecoIP).Status == IPStatus.Success)
                    {
                        using (Socket comandoImpressao = new Socket(SocketType.Stream, ProtocolType.Tcp))
                        {
                            Byte[] bytesConteudoImpressao = Encoding.ASCII.GetBytes(Impressao.ConteudoImpressao.ToString());
                            IPAddress IP = IPAddress.Parse(Impressao.EnderecoIP);
                            IPEndPoint IPComPorta = new IPEndPoint(IP, Impressao.PortaConexao);
                            comandoImpressao.Connect(IPComPorta);
                            comandoImpressao.Send(bytesConteudoImpressao);
                            // Delay de 5 segundos antes de fechar o comando permitindo que a impressora receba todo conteúdo enviado.
                            Thread.Sleep(5000);
                            comandoImpressao.Shutdown(SocketShutdown.Both);
                            comandoImpressao.Close();
                        }
                    }
                    else
                        throw new Exception(String.Format("Impressora {0}:{1} Offline!", Impressao.EnderecoIP, Impressao.PortaConexao));
                }
                #endregion
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
                #region Definição do Comando SQL
                if (String.Join(",", dadosImpressao.IdsEtiquetasImprimir).Contains(";"))
                {
                    var sqlWhere = string.Empty;
                    dadosImpressao.IdsEtiquetasImprimir.ForEach(delegate (string i)
                    {
                        sqlWhere += String.Format("(si.refx = '{0}' AND s.id = {1}) OR ", i.Split(';')[0].ToString(), i.Split(';')[1].ToString());
                    });
                    dadosImpressao.ComandoSQL = String.Format(GarantiaEtiqueta.SQL.ConsultaEtiquetas, String.Format("WHERE {0}", sqlWhere.Substring(0, sqlWhere.Length - 4)));
                }
                else
                    dadosImpressao.ComandoSQL = String.Format(GarantiaEtiqueta.SQL.ConsultaEtiquetas, String.Format("WHERE si.id IN ({0})", String.Join(",", dadosImpressao.IdsEtiquetasImprimir)));
                #endregion

                #region Consultar
                using (var conn = new OracleConnection(Entities.Database.Connection.ConnectionString))
                {
                    conn.Open();
                    if (conn.State == ConnectionState.Open)
                    {
                        dadosImpressao.ListaImprimir = new List<GarantiaEtiqueta.ItemEtiqueta>(conn.Query<GarantiaEtiqueta.ItemEtiqueta>(dadosImpressao.ComandoSQL).ToList());
                    }
                    conn.Close();
                }
                #endregion

                #region Processamento
                if (!dadosImpressao.ListaImprimir.Count.Equals(0))
                {
                    int coluna = 1;
                    int i = 1;
                    dadosImpressao.ConteudoImpressao = new StringBuilder();
                    dadosImpressao.ListaImprimir.ForEach(delegate (GarantiaEtiqueta.ItemEtiqueta item)
                    {
                        for (int vai = 1; vai <= item.QtdeEmbalagem; vai++)
                        {
                            #region Coluna 1
                            if (coluna.Equals(1))
                            {
                                #region Cabeçalho Etiqueta
                                dadosImpressao.ConteudoImpressao.AppendLine("^XA");
                                dadosImpressao.ConteudoImpressao.AppendLine("^LH32,0");
                                dadosImpressao.ConteudoImpressao.AppendLine("^PRA^FS");
                                #endregion

                                #region Registro
                                if (item.TipoEtiqueta == GarantiaEtiqueta.ETIQUETA.Devolução)
                                {
                                    dadosImpressao.ConteudoImpressao.AppendLine("^FO0,36^GB146,26,30^FS");
                                    dadosImpressao.ConteudoImpressao.AppendLine(String.Format(GarantiaEtiqueta.EtiquetaDevolucao.PrimeiraColuna, item.Refx, item.Localizacao, item.Descricao, item.CodigoBarras, item.QtdeEmbalagem));
                                }

                                if (item.TipoEtiqueta == GarantiaEtiqueta.ETIQUETA.Garantia)
                                {
                                    dadosImpressao.ConteudoImpressao.AppendLine("^PQ1^FS");
                                    dadosImpressao.ConteudoImpressao.AppendLine("^FO000,45^GB146,26,30^FS");
                                    dadosImpressao.ConteudoImpressao.AppendLine(String.Format(GarantiaEtiqueta.EtiquetaGarantia.PrimeiraColuna, item.Refx, item.CodigoSolicitacao, item.Descricao, item.CodigoBarras, item.Cliente, item.Representante));
                                }
                                #endregion
                            }
                            #endregion

                            #region Coluna 2
                            if (coluna.Equals(2))
                            {
                                #region Registro
                                if (item.TipoEtiqueta == GarantiaEtiqueta.ETIQUETA.Devolução)
                                {
                                    dadosImpressao.ConteudoImpressao.AppendLine("^FO272,36^GB146,26,30^FS");
                                    dadosImpressao.ConteudoImpressao.AppendLine(String.Format(GarantiaEtiqueta.EtiquetaDevolucao.SegundaColuna, item.Refx, item.Localizacao, item.Descricao, item.CodigoBarras, item.QtdeEmbalagem));
                                }

                                if (item.TipoEtiqueta == GarantiaEtiqueta.ETIQUETA.Garantia)
                                {
                                    dadosImpressao.ConteudoImpressao.AppendLine("^FO272,45^GB146,26,30^FS");
                                    dadosImpressao.ConteudoImpressao.AppendLine(String.Format(GarantiaEtiqueta.EtiquetaGarantia.SegundaColuna, item.Refx, item.CodigoSolicitacao, item.Descricao, item.CodigoBarras, item.Cliente, item.Representante));
                                }
                                #endregion
                            }
                            #endregion

                            #region Coluna 3
                            if (coluna.Equals(3))
                            {
                                #region Registro
                                if (item.TipoEtiqueta == GarantiaEtiqueta.ETIQUETA.Devolução)
                                {
                                    dadosImpressao.ConteudoImpressao.AppendLine("^FO544,36^GB146,26,30^FS");
                                    dadosImpressao.ConteudoImpressao.AppendLine(String.Format(GarantiaEtiqueta.EtiquetaDevolucao.TerceiraColuna, item.Refx, item.Localizacao, item.Descricao, item.CodigoBarras, item.QtdeEmbalagem));
                                }

                                if (item.TipoEtiqueta == GarantiaEtiqueta.ETIQUETA.Garantia)
                                {
                                    dadosImpressao.ConteudoImpressao.AppendLine("^FO544,45^GB146,26,30^FS");
                                    dadosImpressao.ConteudoImpressao.AppendLine(String.Format(GarantiaEtiqueta.EtiquetaGarantia.TerceiraColuna, item.Refx, item.CodigoSolicitacao, item.Descricao, item.CodigoBarras, item.Cliente, item.Representante));
                                }
                                #endregion
                            }
                            #endregion

                            if (i == dadosImpressao.ListaImprimir.Sum(c => c.QtdeEmbalagem) || coluna == 3)
                            {
                                #region Rodapé Etiqueta
                                dadosImpressao.ConteudoImpressao.AppendLine("^XZ");
                                dadosImpressao.ConteudoImpressao.AppendLine(" ");
                                #endregion                              

                                coluna = 0;
                            }

                            i++;
                            coluna++;
                        }
                    });
                    #region Enviar Impressão
                    ImprimirEtiqueta(dadosImpressao);
                    #endregion
                }
                else
                    throw new Exception("Nenhuma Etiqueta selecionada para Impressão!");
                #endregion
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region Impressoras do Usuário
        public List<GarantiaEtiqueta.Impressora> ImpressoraUsuario(int IdEmpresa, int IdPerfilImpressora)
        {
            try
            {
                var retorno = new List<GarantiaEtiqueta.Impressora>();

                using (var conn = new OracleConnection(Entities.Database.Connection.ConnectionString))
                {
                    conn.Open();
                    if (conn.State == ConnectionState.Open)
                    {
                        retorno = conn.Query<GarantiaEtiqueta.Impressora>(GarantiaEtiqueta.SQL.ConsultaImpressorasUsuario, new { IdEmpresa, IdPerfilImpressora }).ToList();
                    }
                    conn.Close();
                }

                return retorno;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

    }
}
