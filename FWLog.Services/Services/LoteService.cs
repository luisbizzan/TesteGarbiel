using FWLog.Data;
using FWLog.Data.EnumsAndConsts;
using FWLog.Data.Models;
using FWLog.Services.Integracao;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;

namespace FWLog.Services.Services
{
    public class LoteService
    {
        private UnitOfWork _uow;

        public LoteService(UnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task RegistrarRecebimentoNotaFiscal(long idNotaFiscal, string userId, DateTime dataRecebimento, int qtdVolumes)
        {
            Lote lote = _uow.LoteRepository.GetById(idNotaFiscal);

            if (lote != null)
            {
                return;
            }

            var nota = _uow.NotaFiscalRepository.GetById(idNotaFiscal);

            if (Convert.ToBoolean(ConfigurationManager.AppSettings["IntegracaoSankhya_Habilitar"]))//TODO Temporário
            {
                await AtualizarNotaFiscalIntegracao(nota, LoteStatusEnum.Recebido);
            }

            lote = new Lote();

            lote.IdLoteStatus = LoteStatusEnum.Recebido.GetHashCode();
            lote.IdNotaFiscal = idNotaFiscal;
            lote.DataRecebimento = dataRecebimento;
            lote.IdUsuarioRecebimento = userId;
            lote.QuantidadeVolume = qtdVolumes;

            _uow.LoteRepository.Add(lote);

            nota.IdNotaFiscalStatus = NotaFiscalStatusEnum.Recebida;

            _uow.SaveChanges();
        }

        public async Task FinalizarConferencia(Lote lote)
        {
            NotaFiscal notafiscal = _uow.NotaFiscalRepository.GetById(lote.IdNotaFiscal);
            List<LoteConferencia> loteConferencia = _uow.LoteConferenciaRepository.Obter(lote.IdLote);
            List<LoteDivergencia> loteDivergencia = new List<LoteDivergencia>();

            foreach (var nfItem in notafiscal.NotaFiscalItens)
            {
                LoteDivergencia divergencia = null;

                var conferencia = loteConferencia.Where(x => x.IdProduto == nfItem.IdProduto).ToList();

                if (conferencia.NullOrEmpty())
                {
                    divergencia = new LoteDivergencia();
                    divergencia.QuantidadeConferenciaMenos = nfItem.Quantidade;
                    divergencia.QuantidadeConferencia = 0;
                    divergencia.QuantidadeConferenciaMais = 0;
                }
                else
                {
                    var qtdConferida = loteConferencia.Sum(s => s.Quantidade);

                    if (qtdConferida == nfItem.Quantidade)
                    {
                        continue;
                    }

                    divergencia = new LoteDivergencia();
                    divergencia.QuantidadeConferencia = qtdConferida;
                    divergencia.QuantidadeConferenciaMais = qtdConferida > nfItem.Quantidade ? qtdConferida - nfItem.Quantidade : 0;
                    divergencia.QuantidadeConferenciaMenos = qtdConferida < nfItem.Quantidade ? nfItem.Quantidade - qtdConferida : 0;
                }

                if (divergencia == null)
                {
                    continue;
                }

                divergencia.IdProduto = nfItem.IdProduto;
                divergencia.IdLote = lote.IdLote;
                divergencia.IdNotaFiscal = lote.IdNotaFiscal;
                divergencia.DataTratamentoDivergencia = DateTime.UtcNow;
                divergencia.IdLoteDivergenciaStatus = LoteDivergenciaStatusEnum.AguardandoTratativa;

                loteDivergencia.Add(divergencia);
            };

            var conferenciaMais = loteConferencia.Where(s => !notafiscal.NotaFiscalItens.Any(w => w.IdProduto == s.IdProduto)).GroupBy(g => g.IdProduto).ToDictionary(g => g.Key, g => g.ToList());

            foreach (var item in conferenciaMais)
            {
                var qtdConferida = item.Value.Sum(s => s.Quantidade);

                LoteDivergencia divergencia = new LoteDivergencia();
                divergencia.QuantidadeConferenciaMais = qtdConferida;
                divergencia.QuantidadeConferenciaMenos = 0;
                divergencia.QuantidadeConferencia = qtdConferida;
                divergencia.IdProduto = item.Key;
                divergencia.IdLote = lote.IdLote;
                divergencia.IdNotaFiscal = lote.IdNotaFiscal;
                divergencia.DataTratamentoDivergencia = DateTime.UtcNow;
                divergencia.IdLoteDivergenciaStatus = LoteDivergenciaStatusEnum.AguardandoTratativa;

                loteDivergencia.Add(divergencia);
            }

            using (TransactionScope transactionScope = _uow.CreateTransactionScope())
            {
                _uow.LoteDivergenciaRepository.DeletePorId(lote.IdLote);
                _uow.LoteDivergenciaRepository.AddRange(loteDivergencia);
                _uow.SaveChanges();

                transactionScope.Complete();
            }


            //TODO verifcar necessidade de criar status para erro no sankhya
            if (loteDivergencia.NullOrEmpty())
            {
                if (Convert.ToBoolean(ConfigurationManager.AppSettings["IntegracaoSankhya_Habilitar"]))//TODO Temporário
                {
                    await AtualizarNotaFiscalIntegracao(notafiscal, LoteStatusEnum.Finalizado);

                    if (notafiscal.IdNotaFiscalStatus == NotaFiscalStatusEnum.Confirmada)
                    {
                        await ConfirmarNotaFiscalIntegracao(notafiscal);
                    }
                }

                lote.IdLoteStatus = LoteStatusEnum.Finalizado.GetHashCode();
                notafiscal.IdNotaFiscalStatus = NotaFiscalStatusEnum.Confirmada;

                //TODO ganho de saldo
            }
            else
            {
                if (Convert.ToBoolean(ConfigurationManager.AppSettings["IntegracaoSankhya_Habilitar"]))//TODO Temporário
                {
                    await AtualizarNotaFiscalIntegracao(notafiscal, LoteStatusEnum.ConferidoDivergencia);
                }

                lote.IdLoteStatus = LoteStatusEnum.ConferidoDivergencia.GetHashCode();
                notafiscal.IdNotaFiscalStatus = NotaFiscalStatusEnum.ConferidaDivergencia;
            }

            //TODO verificar qual metodo vai chamar a finalização para tratar a exception como no RegistrarRecebimentoNotaFiscal

            _uow.SaveChanges();


        }

        public async Task FinalizarTratativaDivergencia(Lote lote)
        {

        }

        private static async Task ConfirmarNotaFiscalIntegracao(NotaFiscal notafiscal)
        {
            var atualizacaoOK = await IntegracaoSankhya.Instance.ConfirmarNotaFiscal(notafiscal.CodigoIntegracao);

            if (!atualizacaoOK)
            {
                throw new Exception("A atualização de nota fiscal no Sankhya não terminou com sucesso.");
            }
        }

        private static async Task AtualizarNotaFiscalIntegracao(NotaFiscal notafiscal, LoteStatusEnum loteStatusEnum)
        {
            bool atualizacaoOK = await IntegracaoSankhya.Instance.AtualizarInformacaoIntegracao("CabecalhoNota", "NUNOTA", notafiscal.CodigoIntegracao, "AD_STATUSREC", loteStatusEnum.GetHashCode());

            if (!atualizacaoOK)
            {
                throw new Exception("A atualização de nota fiscal no Sankhya não terminou com sucesso.");
            }
        }
    }
}
