using System.Threading.Tasks;
using FWLog.Services.Integracao;
using FWLog.Services.Model;
using System.Collections.Generic;
using FWLog.Data.Models;
using System;
using System.Linq;
using FWLog.Data;
using System.Transactions;

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
            List<NotaFiscalIntegracao> notasIntegracao = await IntegracaoSankhya.Instance.PreExecuteQuery<NotaFiscalIntegracao>();

            var tiposFrete = _uow.FreteTipoRepository.GetAll();

            foreach (var notaInt in notasIntegracao)
            {
                if (notaInt.NUMNOTA == null)
                {
                    //throw new NullReferenceException("NUMNOTA");
                }

                if (notaInt.SERIENOTA == null)
                {
                    //throw new NullReferenceException("SERIENOTA");
                }

                if (notaInt.VLRNOTA == null)
                {
                    //throw new NullReferenceException("VLRNOTA");
                }

                if (notaInt.VLRFRETE == null)
                {
                    // throw new NullReferenceException("VLRFRETE");
                }

                if (notaInt.NUMCF == null)
                {
                    //throw new NullReferenceException("NUMCF");
                }

                if (notaInt.PESOBRUTO == null)
                {
                    //throw new NullReferenceException("PESOBRUTO");
                }

                if (notaInt.QTDVOL == null)
                {
                    //throw new NullReferenceException("QTDVOL");
                }

                bool notaNova = false;
                var codNota = notaInt.NUNOTA == null ? 0 : Convert.ToInt64(notaInt.NUNOTA);

                NotaFiscal nota = _uow.NotaFiscalRepository.PegarNotaFiscal(codNota);

                if (nota == null)
                {
                    notaNova = true;

                    nota = new NotaFiscal();
                    nota.Numero = notaInt.NUMNOTA == null ? 0 : Convert.ToInt32(notaInt.NUMNOTA);
                    nota.Serie = notaInt.SERIENOTA == null ? 0 : Convert.ToInt32(notaInt.SERIENOTA);
                    nota.CodigoIntegracao = codNota;

                    nota.IdFornecedor = 41;//TODO Fazer integração de Fornecedor e ajustar vinculo notaInt.CODEMP;
                }

                nota.DANFE = notaInt.DANFE;
                nota.ValorTotal = notaInt.VLRNOTA == null ? 0 : Convert.ToDecimal(notaInt.VLRNOTA);
                nota.ValorFrete = notaInt.VLRFRETE == null ? 0 : Convert.ToDecimal(notaInt.VLRFRETE);
                nota.NumeroConhecimento = notaInt.NUMCF == null ? 0 : Convert.ToInt64(notaInt.NUMCF);
                nota.PesoBruto = notaInt.PESOBRUTO == null ? 0 : Convert.ToDecimal(notaInt.PESOBRUTO);
                nota.Quantidade = notaInt.QTDVOL == null ? 0 : Convert.ToInt32(notaInt.QTDVOL);
                nota.IdFreteTipo = tiposFrete.FirstOrDefault(f => f.Sigla == notaInt.CIF_FOB).IdFreteTipo;
                nota.Especie = "Volumes";//TODO Não desconhecido
                nota.Status = notaInt.STATUSNOTA == null ? "0" : notaInt.STATUSNOTA;
                nota.Chave = notaInt.CHAVENFE == null ? "0" : notaInt.CHAVENFE;
                nota.IdTransportadora = 41;//TODO Fazer integração de Transportadora e ajustar vinculo - notaInt.CODPARCTRANSPFINAL;
                //TODO baixar campo de emissão
                //TODO prazo do fornecedor
                if (notaNova)
                {
                    _uow.NotaFiscalRepository.Add(nota);
                }

                await _uow.SaveChangesAsync();

                await ConsultaNotaFiscalItemCompra(codNota);
            }

            //TODO atualizar status de notas consultadas.
        }

        private async Task ConsultaNotaFiscalItemCompra(long codigoNotaFiscal)
        {
            var where = string.Format(" WHERE NUNOTA = {0}", codigoNotaFiscal.ToString());
            List<NotaFiscalItemIntegracao> itensIntegracao = await IntegracaoSankhya.Instance.PreExecuteQuery<NotaFiscalItemIntegracao>(where);
            List<NotaFiscalItem> itemsNotaFsical = new List<NotaFiscalItem>();

            var unidades = _uow.UnidadeMedidaRepository.GetAll();

            var idNotaFiscal = _uow.NotaFiscalRepository.PegarNotaFiscal(codigoNotaFiscal).IdNotaFiscal;

            foreach (var itemInt in itensIntegracao)
            {
                if (itemInt.NUNOTA == null)
                {
                    //throw new NullReferenceException("NUMNOTA");
                }

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
    }
}
