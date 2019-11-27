using System.Threading.Tasks;
using FWLog.Services.Integracao;
using FWLog.Services.Model;
using System.Collections.Generic;
using FWLog.Data.Models;
using System;
using System.Linq;
using FWLog.Data;
using System.Transactions;
using System.Xml.Linq;
using FWLog.Data.EnumsAndConsts;
using System.Configuration;
using FWLog.Services.Model.IntegracaoSankhya;

namespace FWLog.Services.Services
{
    //TODO Integração de Notas e Itens estão com comentários devido a falta de dados consistentes no Sankhya
    //TODO Integração de Notas e Itens estão com validações duplicadas devido a falta de dados consistentes no Sankhya
    public class NotaFiscalService : BaseService
    {
        private UnitOfWork _uow;

        public NotaFiscalService(UnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task ConsultaNotaFiscalCompra()
        {
            if (!Convert.ToBoolean(ConfigurationManager.AppSettings["IntegracaoSankhya_Habilitar"]))
            {
                return;
            }

            var where = " WHERE TGFCAB.TIPMOV = 'C' AND TGFCAB.STATUSNOTA <> 'L' AND (TGFCAB.AD_STATUSREC = 0 OR TGFCAB.AD_STATUSREC IS NULL)";
            var inner = "INNER JOIN TGFITE ON TGFCAB.NUNOTA = TGFITE.NUNOTA";
            List<NotaFiscalIntegracao> notasIntegracao = await IntegracaoSankhya.Instance.PreExecutarQueryComplexa<NotaFiscalIntegracao>(where, inner);

            List<FreteTipo> tiposFrete = _uow.FreteTipoRepository.RetornarTodos();
            IQueryable<Empresa> empresas = _uow.EmpresaRepository.Todos();

            Dictionary<string, List<NotaFiscalIntegracao>> notasIntegracaoGrp = notasIntegracao.GroupBy(g => g.NUNOTA).ToDictionary(d => d.Key, d => d.ToList());

            foreach (var notasInt in notasIntegracaoGrp)
            {
                var notafiscalIntegracao = notasInt.Value.First();

                ValidarNotaFiscalIntegracao(notafiscalIntegracao);

                bool notaNova = true;

                var codNota = Convert.ToInt64(notafiscalIntegracao.NUNOTA);
                NotaFiscal notafiscal = _uow.NotaFiscalRepository.PegarNotaFiscal(codNota);

                if (notafiscal != null)
                {
                    notaNova = false;
                    notafiscal.IdNotaFiscalStatus = NotaFiscalStatusEnum.AguardandoRecebimento.GetHashCode();

                    var lote = _uow.LoteRepository.PesquisarLotePorNotaFiscal(notafiscal.IdNotaFiscal);

                    if (lote != null)
                    {
                        throw new Exception("Já existe um lote aberto para esta nota fiscal, portanto não é possível integra-la novamente");
                    }
                }
                else
                {
                    notafiscal = new NotaFiscal();
                    notafiscal.Numero = notafiscalIntegracao.NUMNOTA == null ? 0 : Convert.ToInt32(notafiscalIntegracao.NUMNOTA);
                    notafiscal.Serie = notafiscalIntegracao.SERIENOTA == null ? (int?)null : Convert.ToInt32(notafiscalIntegracao.SERIENOTA);
                    notafiscal.CodigoIntegracao = codNota;
                    notafiscal.DANFE = notafiscalIntegracao.DANFE;
                    notafiscal.ValorTotal = notafiscalIntegracao.VLRNOTA == null ? 0 : Convert.ToDecimal(notafiscalIntegracao.VLRNOTA);
                    notafiscal.ValorFrete = notafiscalIntegracao.VLRFRETE == null ? 0 : Convert.ToDecimal(notafiscalIntegracao.VLRFRETE);
                    notafiscal.NumeroConhecimento = notafiscalIntegracao.NUMCF == null ? (long?)null : Convert.ToInt64(notafiscalIntegracao.NUMCF);
                    notafiscal.PesoBruto = notafiscalIntegracao.PESOBRUTO == null ? (decimal?)null : Convert.ToDecimal(notafiscalIntegracao.PESOBRUTO);
                    notafiscal.Quantidade = notafiscalIntegracao.QTDVOL == null ? 0 : Convert.ToInt32(notafiscalIntegracao.QTDVOL);
                    notafiscal.Especie = notafiscalIntegracao.VOLUME;
                    notafiscal.StatusIntegracao = notafiscalIntegracao.STATUSNOTA;
                    notafiscal.IdNotaFiscalStatus = NotaFiscalStatusEnum.AguardandoRecebimento.GetHashCode();
                    notafiscal.Chave = notafiscalIntegracao.CHAVENFE;
                    notafiscal.IdFornecedor = 41;//TODO Fazer integração de Fornecedor e ajustar vinculo notaInt.CODPARC;
                    notafiscal.DataEmissao = notafiscalIntegracao.DHEMISSEPEC == null ? DateTime.Now : Convert.ToDateTime(notafiscalIntegracao.DHEMISSEPEC); //TODO validar campo geovane;

                    FreteTipo freteTipo = tiposFrete.FirstOrDefault(f => f.Sigla == notafiscalIntegracao.CIF_FOB);
                    if (freteTipo == null)
                    {
                        throw new Exception("Tipo de frete (CIF_FOB) inválido.");
                    }
                    else
                    {
                        notafiscal.IdFreteTipo = freteTipo.IdFreteTipo;
                    }

                    Empresa empresa = empresas.FirstOrDefault(f => f.CodigoIntegracao.ToString() == notafiscalIntegracao.CODEMP);
                    if (empresa == null)
                    {
                        throw new Exception("Código da Empresa (CODEMP) inválido");
                    }
                    else
                    {
                        notafiscal.CompanyId = empresa.IdEmpresa;
                    }

                    Transportadora transportadora = _uow.TransportadoraRepository.Todos().FirstOrDefault(f => f.CodigoIntegracao.ToString() == notafiscalIntegracao.CODPARCTRANSP);
                    if (empresa == null)
                    {
                        throw new Exception("Código da Transportadora (CODPARCTRANSP) inválido");
                    }
                    else
                    {
                        notafiscal.IdTransportadora = transportadora.IdTransportadora;
                    }

                    //Fornecedor fornecedor = _uow.FornecedorRepository.Todos().FirstOrDefault(f => f.Codigo.ToString() == notafiscalIntegracao.CODPARC);
                    //if (empresa == null)
                    //{
                    //    throw new Exception("Código da Transportadora (CODPARC) inválido");
                    //}
                    //else
                    //{
                    //    notafiscal.IdFornecedor = fornecedor.IdFornecedor;
                    //}
                }

                try
                {
                    await AtualizarStatusNota(notafiscal, NotaFiscalStatusEnum.AguardandoRecebimento);

                    if (notaNova)
                    {
                        _uow.NotaFiscalRepository.Add(notafiscal);
                    }

                    await _uow.SaveChangesAsync();

                }
                catch (Exception ex)
                {
                    var applicationLogService = new ApplicationLogService(_uow);
                    applicationLogService.Error(ApplicationEnum.BackOffice, ex);

                    continue;
                }

               // await ConsultaNotaFiscalItemCompra(codNota);
            }
        }

        private async Task ConsultaNotaFiscalItemCompra(long codigoIntegracao)
        {
            if (!Convert.ToBoolean(ConfigurationManager.AppSettings["IntegracaoSankhya_Habilitar"]))
            {
                return;
            }

            var where = string.Format(" WHERE NUNOTA = {0}", codigoIntegracao.ToString());
            List<NotaFiscalItemIntegracao> itensIntegracao = await IntegracaoSankhya.Instance.PreExecutarQueryGenerico<NotaFiscalItemIntegracao>(where);
            List<NotaFiscalItem> itemsNotaFsical = new List<NotaFiscalItem>();

            var unidades = _uow.UnidadeMedidaRepository.RetornarTodos();

            var idNotaFiscal = _uow.NotaFiscalRepository.PegarNotaFiscal(codigoIntegracao).IdNotaFiscal;

            foreach (var itemInt in itensIntegracao)
            {
                //ValidarNotaFiscalItemIntegracao(itemInt);

                bool itemNovo = false;
                var codNota = Convert.ToInt64(itemInt.NUNOTA);
                var codProduto = itemInt.CODPROD == null ? 0 : Convert.ToInt64(itemInt.CODPROD);

                codProduto = 46;//Temporário

                NotaFiscalItem item = _uow.NotaFiscalItemRepository.PegarNotaFiscal(codNota, codProduto);

                if (item == null)
                {
                    item = new NotaFiscalItem();
                    itemNovo = true;
                }

                item.IdProduto = codProduto;//TODO Fazer vinculado com cadastro de produto após integraç
                item.IdUnidadeMedida = unidades.First(f => f.Descricao == itemInt.CODVOL).IdUnidadeMedida;
                item.Quantidade = itemInt.QTDNEG == null ? 0 : Convert.ToInt32(itemInt.QTDNEG);
                item.ValorUnitario = itemInt.VLRUNIT == null ? 0 : Convert.ToDecimal(itemInt.VLRUNIT);
                item.ValorTotal = itemInt.VLRTOT == null ? 0 : Convert.ToDecimal(itemInt.VLRTOT);
                item.IdNotaFiscal = idNotaFiscal;
                item.CodigoNotaFiscal = codNota;

                if (itemNovo)
                {
                    itemsNotaFsical.Add(item);
                }
            }

            _uow.NotaFiscalItemRepository.AddRange(itemsNotaFsical);

            await _uow.SaveChangesAsync();
        }

        public async Task<bool> AtualizarStatusNota(NotaFiscal notaFiscal, NotaFiscalStatusEnum status)
        {
            if (!Convert.ToBoolean(ConfigurationManager.AppSettings["IntegracaoSankhya_Habilitar"]))
            {
                return false;
            }

            XElement dataRow = new XElement("dataRow", new XElement("localFields", new XElement("STATUSNOTA", status.GetHashCode())));
            dataRow.Add(new XElement("key", new XElement("NUNOTA", notaFiscal.CodigoIntegracao)));

            XAttribute[] attArray = {
                new XAttribute("rootEntity", "CabecalhoNota"),
                new XAttribute("includePresentationFields", "S"),
            };

            var entity = new XElement("entity", new XAttribute("path", ""));
            entity.Add(new XElement("fieldset", new XAttribute("list", "*")));

            XElement datset = new XElement("dataSet", attArray);
            datset.Add(entity);
            datset.Add(dataRow);

            XElement serviceRequest = new XElement("serviceRequest", new XAttribute("serviceName", "CRUDServiceProvider.saveRecord"));
            serviceRequest.Add(new XElement("requestBody", datset));

            await IntegracaoSankhya.Instance.ExecutarSaveRecord(serviceRequest);

            return true;
        }

        public void ValidarNotaFiscalIntegracao(NotaFiscalIntegracao notafiscal)
        {
            ValidarCampo(notafiscal.NUNOTA, nameof(notafiscal.NUNOTA));
            ValidarCampo(notafiscal.NUMNOTA, nameof(notafiscal.NUMNOTA));
            ValidarCampo(notafiscal.CODEMP, nameof(notafiscal.CODEMP));
            ValidarCampo(notafiscal.VLRNOTA, nameof(notafiscal.VLRNOTA));
            ValidarCampo(notafiscal.CIF_FOB, nameof(notafiscal.CIF_FOB));
            ValidarCampo(notafiscal.CODPARC, nameof(notafiscal.CODPARC));
            ValidarCampo(notafiscal.CODPARCTRANSP, nameof(notafiscal.CODPARCTRANSP));
            ValidarCampo(notafiscal.VLRFRETE, nameof(notafiscal.VLRFRETE));
            //ValidarCampo(notafiscal.DHEMISSEPEC, nameof(notafiscal.DHEMISSEPEC));    
            //ValidarCampo(itemInt.NUNOTA, nameof(itemInt.NUNOTA));
            //ValidarCampo(itemInt.CODVOL, nameof(itemInt.CODVOL));
            //ValidarCampo(itemInt.QTDNEG, nameof(itemInt.QTDNEG));
            //ValidarCampo(itemInt.VLRUNIT, nameof(itemInt.VLRUNIT));
            //ValidarCampo(itemInt.VLRTOT, nameof(itemInt.VLRTOT));
            //ValidarCampo(itemInt.CODPROD, nameof(itemInt.CODPROD));
        }

       
    }
}
