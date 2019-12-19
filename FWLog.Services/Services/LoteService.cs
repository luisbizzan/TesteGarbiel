using FWLog.Data;
using FWLog.Data.EnumsAndConsts;
using FWLog.Data.Models;
using FWLog.Services.Integracao;
using FWLog.Services.Model.Lote;
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
            var nota = _uow.NotaFiscalRepository.GetById(idNotaFiscal);

            await VerificarNotaFiscalCancelada(nota);

            Lote lote = _uow.LoteRepository.GetById(idNotaFiscal);

            if (lote != null)
            {
                return;
            }

            if (Convert.ToBoolean(ConfigurationManager.AppSettings["IntegracaoSankhya_Habilitar"]))//TODO Temporário
            {
                await AtualizarNotaFiscalIntegracao(nota, LoteStatusEnum.Recebido);
            }

            lote = new Lote();

            lote.IdLoteStatus = LoteStatusEnum.Recebido;
            lote.IdNotaFiscal = idNotaFiscal;
            lote.DataRecebimento = dataRecebimento;
            lote.IdUsuarioRecebimento = userId;
            lote.QuantidadeVolume = qtdVolumes;

            _uow.LoteRepository.Add(lote);

            nota.IdNotaFiscalStatus = NotaFiscalStatusEnum.Recebida;

            _uow.SaveChanges();
        }

        public async Task FinalizarConferencia(long idlote, string userId, long idEmpresa)
        {
            Lote lote = _uow.LoteRepository.GetById(idlote);
            NotaFiscal notafiscal = _uow.NotaFiscalRepository.GetById(lote.IdNotaFiscal);
            List<LoteConferencia> loteConferencias = _uow.LoteConferenciaRepository.Obter(lote.IdLote);
            List<LoteDivergencia> loteDivergencias = new List<LoteDivergencia>();
            List<LoteConferencia> loteNaoConferido = new List<LoteConferencia>();

            var empresaConfig = _uow.EmpresaConfigRepository.ConsultarPorIdEmpresa(idEmpresa);

            var nfItensAgrupado = notafiscal.NotaFiscalItens.GroupBy(g => g.IdProduto).ToDictionary(g => g.Key, g => g.ToList());

            foreach (var nfItem in nfItensAgrupado)
            {
                LoteDivergencia divergencia = null;

                var qtdOriginal = nfItem.Value.Sum(s => s.Quantidade);
                var conferencia = loteConferencias.Where(x => x.IdProduto == nfItem.Key).ToList();

                if (conferencia.NullOrEmpty())
                {
                    divergencia = new LoteDivergencia();
                    divergencia.QuantidadeConferenciaMenos = qtdOriginal;
                    divergencia.QuantidadeConferencia = 0;
                    divergencia.QuantidadeConferenciaMais = 0;

                    var loteConferencia = new LoteConferencia()
                    {
                        IdLote = lote.IdLote,
                        IdTipoConferencia = empresaConfig.IdTipoConferencia.Value,
                        IdProduto = nfItem.Key,
                        Quantidade = 0,
                        DataHoraInicio = DateTime.Now,//TODO VErificar
                        DataHoraFim = DateTime.Now,
                        Tempo = DateTime.Now,
                        IdUsuarioConferente = userId
                    };

                    loteNaoConferido.Add(loteConferencia);
                }
                else
                {
                    var qtdConferida = conferencia.Sum(s => s.Quantidade);

                    if (qtdConferida == qtdOriginal)
                    {
                        continue;
                    }

                    divergencia = new LoteDivergencia();
                    divergencia.QuantidadeConferencia = qtdConferida;
                    divergencia.QuantidadeConferenciaMais = qtdConferida > qtdOriginal ? qtdConferida - qtdOriginal : 0;
                    divergencia.QuantidadeConferenciaMenos = qtdConferida < qtdOriginal ? qtdOriginal - qtdConferida : 0;
                }

                if (divergencia == null)
                {
                    continue;
                }

                divergencia.IdProduto = nfItem.Key;
                divergencia.IdLote = lote.IdLote;
                divergencia.IdNotaFiscal = lote.IdNotaFiscal;
                divergencia.IdLoteDivergenciaStatus = LoteDivergenciaStatusEnum.AguardandoTratativa;

                loteDivergencias.Add(divergencia);
            };

            var conferenciaMais = loteConferencias.Where(s => !nfItensAgrupado.Any(w => w.Key == s.IdProduto)).GroupBy(g => g.IdProduto).ToDictionary(g => g.Key, g => g.ToList());

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
                divergencia.IdLoteDivergenciaStatus = LoteDivergenciaStatusEnum.AguardandoTratativa;

                loteDivergencias.Add(divergencia);
            }

            NotaFiscalStatusEnum notaStatus;
            LoteStatusEnum loteStatus;

            if (loteDivergencias.NullOrEmpty())
            {
                loteStatus = LoteStatusEnum.Finalizado;
                notaStatus = NotaFiscalStatusEnum.Confirmada;

                await AtualizarNotaFiscalIntegracao(notafiscal, loteStatus);
                await ConfirmarNotaFiscalIntegracao(notafiscal, lote);
            }
            else
            {
                loteStatus = LoteStatusEnum.ConferidoDivergencia;
                notaStatus = NotaFiscalStatusEnum.ConferidaDivergencia;

                await AtualizarNotaFiscalIntegracao(notafiscal, loteStatus);
            }

            using (TransactionScope transactionScope = _uow.CreateTransactionScope())
            {
                lote.IdLoteStatus = loteStatus;
                notafiscal.IdNotaFiscalStatus = notaStatus;

                _uow.LoteConferenciaRepository.AddRange(loteNaoConferido);
                _uow.LoteDivergenciaRepository.AddRange(loteDivergencias);
                _uow.SaveChanges();

                if (notaStatus == NotaFiscalStatusEnum.Confirmada)
                {
                    GravarGanhoEstoque();
                }

                transactionScope.Complete();
            }

            //TODO verificar qual metodo vai chamar a finalização para tratar a exception como no RegistrarRecebimentoNotaFiscal
        }

        public async Task TratarDivergencia(TratarDivergenciaRequest request)
        {
            GravarTratamentoDivergencia(request);

            NotaFiscal notafiscal = _uow.NotaFiscalRepository.GetById(request.IdNotaFiscal);
            Lote lote = _uow.LoteRepository.ObterLoteNota(request.IdNotaFiscal);

            List<LoteDivergencia> loteDivergencias = null;

            if (notafiscal.IdNotaFiscalStatus != NotaFiscalStatusEnum.Confirmada)
            {
                loteDivergencias = _uow.LoteDivergenciaRepository.RetornarPorNotaFiscal(lote.IdNotaFiscal);

                loteDivergencias.ForEach(f => f.IdLoteDivergenciaStatus = LoteDivergenciaStatusEnum.DivergenciaTratada);

                if (loteDivergencias.Any(a => a.QuantidadeDivergenciaMais > 0) && loteDivergencias.Any(a => a.QuantidadeDivergenciaMenos > 0))
                {
                    lote.IdLoteStatus = LoteStatusEnum.FinalizadoDivergenciaTodas;
                }
                else if (loteDivergencias.Any(a => a.QuantidadeDivergenciaMais > 0))
                {
                    lote.IdLoteStatus = LoteStatusEnum.FinalizadoDivergenciaPositiva;
                }
                else if (loteDivergencias.Any(a => a.QuantidadeDivergenciaMenos > 0))
                {
                    lote.IdLoteStatus = LoteStatusEnum.FinalizadoDivergenciaNegativa;
                }
                else
                {
                    lote.IdLoteStatus = LoteStatusEnum.Finalizado;
                }

                await AtualizarNotaFiscalIntegracao(notafiscal, lote.IdLoteStatus);
                await ConfirmarNotaFiscalIntegracao(notafiscal, lote);

                notafiscal.IdNotaFiscalStatus = NotaFiscalStatusEnum.Confirmada;
                _uow.SaveChanges();
            }

            // if (!lote.GanhoEstoqueEfetuado)
            {
                GravarGanhoEstoque();
            }

            if (lote.IdLoteStatus == LoteStatusEnum.FinalizadoDivergenciaPositiva || lote.IdLoteStatus == LoteStatusEnum.FinalizadoDivergenciaTodas)
            {
                GravarQuaretena(loteDivergencias, lote);
            }

            if (lote.IdLoteStatus == LoteStatusEnum.FinalizadoDivergenciaTodas || lote.IdLoteStatus == LoteStatusEnum.FinalizadoDivergenciaNegativa)
            {
                NotaFiscal notafiscalDevolucao = null;

                if (notafiscal.CodigoIntegracaoNFDevolucao == null)
                {
                    notafiscalDevolucao = await CriarNotaDevolucao();
                    notafiscalDevolucao.IdNotaFiscalStatus = NotaFiscalStatusEnum.NotaDevolucaoCriada;
                    notafiscal.CodigoIntegracaoNFDevolucao = notafiscalDevolucao.CodigoIntegracao;
                    _uow.SaveChanges();
                }
                else
                {
                    notafiscalDevolucao = _uow.NotaFiscalRepository.ObterPorCodigoIntegracao(notafiscal.CodigoIntegracaoNFDevolucao.Value);

                }

                if (notafiscalDevolucao == null)
                {
                    throw new Exception();//verificar
                }

                if (notafiscalDevolucao.IdNotaFiscalStatus == NotaFiscalStatusEnum.NotaDevolucaoCriada)
                {
                    await ConfirmarNotaFiscalIntegracao(notafiscalDevolucao, null);
                    notafiscalDevolucao.IdNotaFiscalStatus = NotaFiscalStatusEnum.Confirmada;
                    _uow.SaveChanges();
                }

                if (notafiscalDevolucao.IdNotaFiscalStatus == NotaFiscalStatusEnum.Confirmada && notafiscalDevolucao.IdNotaFiscalStatus != NotaFiscalStatusEnum.NotaDevolucaoAutorizada)
                {
                    await VerificarNotaFiscalAutorizada(notafiscalDevolucao);
                    notafiscalDevolucao.IdNotaFiscalStatus = NotaFiscalStatusEnum.NotaDevolucaoAutorizada;
                    _uow.SaveChanges();
                }
            }
        }

        private void GravarTratamentoDivergencia(TratarDivergenciaRequest request)
        {
            foreach (TratarDivergenciaItemRequest divergencia in request.Divergencias)
            {
                LoteDivergencia loteDivergencia = _uow.LoteDivergenciaRepository.GetById(divergencia.IdLoteDivergencia);

                loteDivergencia.QuantidadeDivergenciaMais = divergencia.QuantidadeMaisTratado ?? 0;
                loteDivergencia.QuantidadeDivergenciaMenos = divergencia.QuantidadeMenosTratado ?? 0;
                loteDivergencia.IdUsuarioDivergencia = request.IdUsuario;
                loteDivergencia.IdLoteDivergenciaStatus = LoteDivergenciaStatusEnum.DivergenciaTratada;
                loteDivergencia.DataTratamentoDivergencia = DateTime.Now;

                _uow.LoteDivergenciaRepository.Update(loteDivergencia);
                _uow.SaveChanges();
            }
        }

        private async Task<NotaFiscal> CriarNotaDevolucao()
        {
            return new NotaFiscal();
            //TODO ganho de saldo
        }

        private void GravarGanhoEstoque()
        {
            //TODO ganho de saldo
        }

        private void GravarQuaretena(List<LoteDivergencia> loteDivergencias, Lote lote)
        {
            if (loteDivergencias.NullOrEmpty() || _uow.QuarentenaRepository.ExisteQuarentena(lote.IdLote))
            {
                return;
            }

            loteDivergencias = loteDivergencias.Where(w => w.QuantidadeDivergenciaMais > 0).ToList();

            if (loteDivergencias.NullOrEmpty())
            {
                return;
            }

            Quarentena quarentena = new Quarentena()
            {
                DataAbertura = DateTime.UtcNow,
                IdLote = lote.IdLote,
                IdQuarentenaStatus = QuarentenaStatusEnum.Aberto
            };

            _uow.QuarentenaRepository.Add(quarentena);
            _uow.SaveChanges();
        }

        private async Task ConfirmarNotaFiscalIntegracao(NotaFiscal notafiscal, Lote lote)
        {
            if (!(Convert.ToBoolean(ConfigurationManager.AppSettings["IntegracaoSankhya_Habilitar"])))//TODO Temporário
            {
                return;
            }

            if (notafiscal.IdNotaFiscalStatus == NotaFiscalStatusEnum.Confirmada)
            {
                return;
            }

            var atualizacaoOK = await IntegracaoSankhya.Instance.ConfirmarNotaFiscal(notafiscal.CodigoIntegracao);

            if (!atualizacaoOK)
            {
                throw new Exception("A atualização de nota fiscal no Sankhya não terminou com sucesso.");
            }
        }

        private async Task AtualizarNotaFiscalIntegracao(NotaFiscal notafiscal, LoteStatusEnum loteStatusEnum)
        {
            if (!(Convert.ToBoolean(ConfigurationManager.AppSettings["IntegracaoSankhya_Habilitar"])))//TODO Temporário
            {
                return;
            }

            bool atualizacaoOK = await IntegracaoSankhya.Instance.AtualizarInformacaoIntegracao("CabecalhoNota", "NUNOTA", notafiscal.CodigoIntegracao, "AD_STATUSREC", loteStatusEnum.GetHashCode());

            if (!atualizacaoOK)
            {
                throw new Exception("A atualização de nota fiscal no Sankhya não terminou com sucesso.");
            }
        }

        private async Task VerificarNotaFiscalCancelada(NotaFiscal notafiscal)
        {
            if (!(Convert.ToBoolean(ConfigurationManager.AppSettings["IntegracaoSankhya_Habilitar"])))//TODO Temporário
            {
                return;
            }

            var notaFiscalService = new NotaFiscalService(_uow);

            bool atualizacaoOK = await notaFiscalService.VerificarNotaFiscalCancelada(notafiscal.CodigoIntegracao);

            if (atualizacaoOK)
            {
                throw new Exception(string.Format("A nota fiscal {0} foi cancelada no Sankhya.", notafiscal.CodigoIntegracao));
            }
        }

        private async Task VerificarNotaFiscalAutorizada(NotaFiscal notafiscal)
        {
            if (!(Convert.ToBoolean(ConfigurationManager.AppSettings["IntegracaoSankhya_Habilitar"])))//TODO Temporário
            {
                return;
            }

            var notaFiscalService = new NotaFiscalService(_uow);

            bool atualizacaoOK = await notaFiscalService.VerificarNotaFiscalAutorizada(notafiscal.CodigoIntegracao);

            if (!atualizacaoOK)
            {
                throw new Exception(string.Format("A nota fiscal {0} não autorizada no Sankhya.", notafiscal.CodigoIntegracao));
            }
        }
    }
}
