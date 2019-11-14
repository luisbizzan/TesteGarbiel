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

namespace FWLog.Services.Services
{
    //TODO Integração de Notas e Itens estão com comentários devido a falta de dados consistentes no Sankhya
    //TODO Integração de Notas e Itens estão com validações duplicadas devido a falta de dados consistentes no Sankhya
    public class NotaFiscalService
    {
        private UnitOfWork _uow;

        public NotaFiscalService(UnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task ConsultaNotaFiscalCompra()
        {
            var where = "TGFCAB.TIPMOV = 'C' AND TGFCAB.STATUSNOTA <> 'L' AND TGFCAB.AD_STATUSREC <> 0";
            List<NotaFiscalIntegracao> notasIntegracao = await IntegracaoSankhya.Instance.PreExecuteQuery<NotaFiscalIntegracao>(where);

            var tiposFrete = _uow.FreteTipoRepository.GetAll();

            foreach (var notaInt in notasIntegracao)
            {
                ValidarNotaFiscalIntegracao(notaInt);

                bool notaNova = true;
                var codNota = notaInt.NUNOTA == null ? 0 : Convert.ToInt64(notaInt.NUNOTA);

                NotaFiscal nota = _uow.NotaFiscalRepository.PegarNotaFiscal(codNota);

                if (nota != null)
                {
                    notaNova = false;
                    nota.IdNotaFiscalStatus = NotaFiscalStatusEnum.AguardandoRecebimento.GetHashCode();

                    //TODO verificar lote aberto
                }
                else
                {
                    nota.Numero = notaInt.NUMNOTA == null ? 0 : Convert.ToInt32(notaInt.NUMNOTA);
                    nota.Serie = notaInt.SERIENOTA == null ? 0 : Convert.ToInt32(notaInt.SERIENOTA);
                    nota.CodigoIntegracao = codNota;                    
                    nota.DANFE = notaInt.DANFE;
                    nota.ValorTotal = notaInt.VLRNOTA == null ? 0 : Convert.ToDecimal(notaInt.VLRNOTA);
                    nota.ValorFrete = notaInt.VLRFRETE == null ? 0 : Convert.ToDecimal(notaInt.VLRFRETE);
                    nota.NumeroConhecimento = notaInt.NUMCF == null ? 0 : Convert.ToInt64(notaInt.NUMCF);
                    nota.PesoBruto = notaInt.PESOBRUTO == null ? 0 : Convert.ToDecimal(notaInt.PESOBRUTO);
                    nota.Quantidade = notaInt.QTDVOL == null ? 0 : Convert.ToInt32(notaInt.QTDVOL);
                    nota.IdFreteTipo = tiposFrete.FirstOrDefault(f => f.Sigla == notaInt.CIF_FOB).IdFreteTipo;
                    nota.Especie = notaInt.VOLUME;
                    nota.StatusIntegracao = notaInt.STATUSNOTA == null ? "0" : notaInt.STATUSNOTA;
                    nota.IdNotaFiscalStatus = NotaFiscalStatusEnum.AguardandoRecebimento.GetHashCode();
                    nota.Chave = notaInt.CHAVENFE == null ? "0" : notaInt.CHAVENFE;
                    nota.IdTransportadora = 41;//TODO Fazer integração de Transportadora e ajustar vinculo - notaInt.CODPARCTRANSP;
                    nota.IdFornecedor = 41;//TODO Fazer integração de Fornecedor e ajustar vinculo notaInt.CODPARC;
                    nota.CompanyId = 1;//TODO Fazer integração de Empresa e ajustar vinculo notaInt.CODEMP;
                    nota.DataEmissao = notaInt.DHEMISSEPEC == null ? DateTime.Now : Convert.ToDateTime(notaInt.DHEMISSEPEC); //TODO validar campo geovane;
                }

                if (notaNova)
                {
                    _uow.NotaFiscalRepository.Add(nota);
                }

                await _uow.SaveChangesAsync();

                await ConsultaNotaFiscalItemCompra(codNota);

                //await AtualizarStatusNota(nota);
            }
        }

        private async Task ConsultaNotaFiscalItemCompra(long codigoIntegracao)
        {
            var where = string.Format(" WHERE NUNOTA = {0}", codigoIntegracao.ToString());
            List<NotaFiscalItemIntegracao> itensIntegracao = await IntegracaoSankhya.Instance.PreExecuteQuery<NotaFiscalItemIntegracao>(where);
            List<NotaFiscalItem> itemsNotaFsical = new List<NotaFiscalItem>();

            var unidades = _uow.UnidadeMedidaRepository.GetAll();

            var idNotaFiscal = _uow.NotaFiscalRepository.PegarNotaFiscal(codigoIntegracao).IdNotaFiscal;

            foreach (var itemInt in itensIntegracao)
            {
                ValidarNotaFiscalItemIntegracao(itemInt);
                
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

        public async Task AtualizarStatusNota(NotaFiscal notaFiscal)
        {
            XElement dataRow = new XElement("dataRow", new XElement("localFields", new XElement("STATUSNOTA", notaFiscal.IdNotaFiscalStatus)));
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
        }

        public void ValidarNotaFiscalItemIntegracao(NotaFiscalItemIntegracao itemInt)
        {
            ValidarCampo(itemInt.NUNOTA);
            ValidarCampo(itemInt.CODVOL);
            ValidarCampo(itemInt.QTDNEG);
            ValidarCampo(itemInt.VLRUNIT);
            ValidarCampo(itemInt.VLRTOT);
            ValidarCampo(itemInt.CODPROD);
        }

        public void ValidarNotaFiscalIntegracao(NotaFiscalIntegracao notafiscal)
        {
            ValidarCampo(notafiscal.NUNOTA);
            ValidarCampo(notafiscal.NUMNOTA);
            ValidarCampo(notafiscal.SERIENOTA);
            ValidarCampo(notafiscal.CODEMP);
            ValidarCampo(notafiscal.DANFE);
            ValidarCampo(notafiscal.CHAVENFE);
            ValidarCampo(notafiscal.VLRNOTA);
            ValidarCampo(notafiscal.CIF_FOB);
            ValidarCampo(notafiscal.CODPARC);
            ValidarCampo(notafiscal.STATUSNOTA);
            ValidarCampo(notafiscal.CODPARCTRANSP);
            ValidarCampo(notafiscal.VLRFRETE);
            ValidarCampo(notafiscal.NUMCF);
            ValidarCampo(notafiscal.PESOBRUTO);
            ValidarCampo(notafiscal.VOLUME);
            ValidarCampo(notafiscal.QTDVOL);
            ValidarCampo(notafiscal.DHEMISSEPEC);          
        }

        public void ValidarCampo(string campo)
        {
            throw new NullReferenceException(campo);
        }
    }
}
