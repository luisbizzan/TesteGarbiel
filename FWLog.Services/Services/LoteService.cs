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

            lote = new Lote
            {
                IdLoteStatus = LoteStatusEnum.Recebido,
                IdNotaFiscal = idNotaFiscal,
                DataRecebimento = dataRecebimento,
                IdUsuarioRecebimento = userId,
                QuantidadeVolume = qtdVolumes,
                QuantidadePeca = nota.NotaFiscalItens.Sum(s => s.Quantidade)
            };

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

            var nfItens = notafiscal.NotaFiscalItens.GroupBy(g => g.IdProduto).ToDictionary(g => g.Key, g => g.ToList());

            foreach (var nfItem in nfItens)
            {
                LoteDivergencia divergencia = null;

                var qtdOriginal = nfItem.Value.Sum(s => s.Quantidade);
                var conferencia = loteConferencias.Where(x => x.IdProduto == nfItem.Key).ToList();

                if (conferencia.NullOrEmpty())
                {
                    divergencia = new LoteDivergencia
                    {
                        QuantidadeConferenciaMenos = qtdOriginal,
                        QuantidadeConferencia = 0,
                        QuantidadeConferenciaMais = 0
                    };

                    var loteConferencia = new LoteConferencia()
                    {
                        IdLote = lote.IdLote,
                        IdTipoConferencia = empresaConfig.IdTipoConferencia.Value,
                        IdProduto = nfItem.Key,
                        Quantidade = 0,
                        DataHoraInicio = DateTime.Now,
                        DataHoraFim = DateTime.Now,
                        Tempo = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0),
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

                    divergencia = new LoteDivergencia
                    {
                        QuantidadeConferencia = qtdConferida,
                        QuantidadeConferenciaMais = qtdConferida > qtdOriginal ? qtdConferida - qtdOriginal : 0,
                        QuantidadeConferenciaMenos = qtdConferida < qtdOriginal ? qtdOriginal - qtdConferida : 0
                    };
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

            var conferenciaMais = loteConferencias.Where(s => !nfItens.Any(w => w.Key == s.IdProduto)).GroupBy(g => g.IdProduto).ToDictionary(g => g.Key, g => g.ToList());

            foreach (var item in conferenciaMais)
            {
                var qtdConferida = item.Value.Sum(s => s.Quantidade);

                LoteDivergencia divergencia = new LoteDivergencia
                {
                    QuantidadeConferenciaMais = qtdConferida,
                    QuantidadeConferenciaMenos = 0,
                    QuantidadeConferencia = qtdConferida,
                    IdProduto = item.Key,
                    IdLote = lote.IdLote,
                    IdNotaFiscal = lote.IdNotaFiscal,
                    IdLoteDivergenciaStatus = LoteDivergenciaStatusEnum.AguardandoTratativa
                };

                loteDivergencias.Add(divergencia);
            }

            var tempoTotal = new TimeSpan(0, 0, 0);
            loteConferencias.ForEach(f => { tempoTotal = tempoTotal.Add(new TimeSpan(f.Tempo.Hour, f.Tempo.Minute, f.Tempo.Second)); });
            lote.TempoTotalConferencia = Convert.ToInt64(tempoTotal.TotalSeconds);

            NotaFiscalStatusEnum notaStatus;
            LoteStatusEnum loteStatus;

            if (loteDivergencias.NullOrEmpty())
            {
                loteStatus = LoteStatusEnum.Finalizado;
                notaStatus = NotaFiscalStatusEnum.Confirmada;

                await AtualizarNotaFiscalIntegracao(notafiscal, loteStatus);
                await ConfirmarNotaFiscalIntegracao(notafiscal);
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
                lote.DataFinalConferencia = DateTime.Now;
                notafiscal.IdNotaFiscalStatus = notaStatus;

                _uow.LoteConferenciaRepository.AddRange(loteNaoConferido);
                _uow.LoteDivergenciaRepository.AddRange(loteDivergencias);
                _uow.SaveChanges();

                if (notaStatus == NotaFiscalStatusEnum.Confirmada)
                {
                    AtualizarSaldoArmazenagem(nfItens, notafiscal, null);
                }

                transactionScope.Complete();
            }
        }

        public async Task<LoteStatusEnum> TratarDivergencia(TratarDivergenciaRequest request, string IdUsuario)
        {
            ValidarDadosDivergencia(request);

            NotaFiscal notafiscal = _uow.NotaFiscalRepository.GetById(request.IdNotaFiscal);
            Lote lote = _uow.LoteRepository.ObterLoteNota(request.IdNotaFiscal);

            lote.ObservacaoDivergencia = request.ObservacaoDivergencia;

            if ((request.Divergencias.Any(a => a.QuantidadeMaisTratado > 0) && request.Divergencias.Any(a => a.QuantidadeMenosTratado > 0)) || request.Divergencias.Any(a => a.QuantidadeMenosTratado > 0))
            {
                lote.IdLoteStatus = LoteStatusEnum.AguardandoCriacaoNFDevolucao;
            }
            else if (request.Divergencias.Any(a => a.QuantidadeMaisTratado > 0))
            {
                lote.IdLoteStatus = LoteStatusEnum.FinalizadoDivergenciaPositiva;
            }
            else
            {
                lote.IdLoteStatus = LoteStatusEnum.Finalizado;
            }

            await AtualizarNotaFiscalIntegracao(notafiscal, lote.IdLoteStatus);
            await ConfirmarNotaFiscalIntegracao(notafiscal);

            Dictionary<long, List<NotaFiscalItem>> nfItens = notafiscal.NotaFiscalItens.GroupBy(g => g.IdProduto).ToDictionary(g => g.Key, g => g.ToList());

            using (TransactionScope transactionScope = _uow.CreateTransactionScope())
            {
                notafiscal.IdNotaFiscalStatus = NotaFiscalStatusEnum.Confirmada;
                _uow.SaveChanges();

                List<LoteDivergencia> loteDivergenciasMenos = GravarTratamentoDivergencia(request);

                AtualizarSaldoArmazenagem(nfItens, notafiscal, loteDivergenciasMenos);

                if (lote.IdLoteStatus == LoteStatusEnum.FinalizadoDivergenciaPositiva && loteDivergenciasMenos.NullOrEmpty())
                {
                    CriarQuarentena(lote, IdUsuario);
                }

                transactionScope.Complete();
            }

            return lote.IdLoteStatus;
        }

        private void ValidarDadosDivergencia(TratarDivergenciaRequest request)
        {
            NotaFiscal notafiscal = _uow.NotaFiscalRepository.GetById(request.IdNotaFiscal);

            var divergenciasNegativa = request.Divergencias.Where(w => w.QuantidadeMenosTratado > 0).ToList();

            foreach (var divergenciaMenos in divergenciasNegativa)
            {
                var divergencia = _uow.LoteDivergenciaRepository.GetById(divergenciaMenos.IdLoteDivergencia);
                var qtdNF = notafiscal.NotaFiscalItens.Where(w => w.IdProduto == divergencia.IdProduto).Sum(s => s.Quantidade);

                if (qtdNF - divergenciaMenos.QuantidadeMenosTratado < 0)
                {
                    throw new BusinessException(string.Format("A quantidade A- não pode ser maior que a quantidade total do item {0} na nota fiscal.", divergencia.Produto.Referencia));
                }
            }
        }

        public ResumoFinalizarConferenciaResponse ResumoFinalizarConferencia(long idLote, long idEmpresa)
        {
            EmpresaConfig empresaConfig = _uow.EmpresaConfigRepository.ConsultarPorIdEmpresa(idEmpresa);
            Lote lote = _uow.LoteRepository.GetById(idLote);
            var itensNotaFiscal = _uow.NotaFiscalItemRepository.ObterItens(lote.IdNotaFiscal).GroupBy(g => g.IdProduto);
            List<LoteConferencia> itensConferidos = _uow.LoteConferenciaRepository.ObterPorId(idLote);

            var response = new ResumoFinalizarConferenciaResponse
            {
                DataRecebimento = lote.DataRecebimento.ToString("dd/MM/yyyy hh:mm:ss"),
                IdLote = lote.IdLote,
                IdNotaFiscal = lote.IdNotaFiscal,
                NumeroNotaFiscal = lote.NotaFiscal.Numero,
                QuantidadeVolume = lote.QuantidadeVolume,
                RazaoSocialFornecedor = lote.NotaFiscal.Fornecedor.RazaoSocial,
                TipoConferencia = empresaConfig.TipoConferencia.Descricao
            };

            foreach (var itemNota in itensNotaFiscal)
            {
                var itensConferencia = itensConferidos.Where(x => x.IdProduto == itemNota.Key);
                int quantidadeNota = itemNota.Sum(s => s.Quantidade);
                string referencia = itemNota.First().Produto.Referencia;

                if (itensConferencia.Any())
                {
                    int quantidadeConferido = itensConferencia.Sum(s => s.Quantidade);
                    int diferencaNotaConferido = quantidadeNota - quantidadeConferido;

                    var item = new ResumoFinalizarConferenciaItemResponse
                    {
                        Referencia = referencia,
                        QuantidadeConferido = quantidadeConferido,
                        QuantidadeNota = quantidadeNota,
                        DivergenciaMais = diferencaNotaConferido < 0 ? diferencaNotaConferido * -1 : 0,
                        DivergenciaMenos = diferencaNotaConferido > 0 ? diferencaNotaConferido : 0
                    };

                    response.Itens.Add(item);
                }
                else
                {
                    var item = new ResumoFinalizarConferenciaItemResponse
                    {
                        Referencia = referencia,
                        QuantidadeConferido = 0,
                        QuantidadeNota = quantidadeNota,
                        DivergenciaMais = 0,
                        DivergenciaMenos = quantidadeNota
                    };

                    response.Itens.Add(item);
                }
            }

            var itensForaNota = itensConferidos.Where(w => !itensNotaFiscal.Any(x => x.Key == w.IdProduto)).GroupBy(g => g.IdProduto);

            foreach (var itemForaNota in itensForaNota)
            {
                int quantidadeConferido = itemForaNota.Sum(s => s.Quantidade);

                var item = new ResumoFinalizarConferenciaItemResponse
                {
                    Referencia = itemForaNota.First().Produto.Referencia,
                    QuantidadeConferido = quantidadeConferido,
                    QuantidadeNota = 0,
                    DivergenciaMais = quantidadeConferido,
                    DivergenciaMenos = 0
                };

                response.Itens.Add(item);
            }

            return response;
        }

        private List<LoteDivergencia> GravarTratamentoDivergencia(TratarDivergenciaRequest request)
        {
            List<LoteDivergencia> loteDivergenciasMenos = new List<LoteDivergencia>();

            foreach (TratarDivergenciaItemRequest divergencia in request.Divergencias)
            {
                LoteDivergencia loteDivergencia = _uow.LoteDivergenciaRepository.GetById(divergencia.IdLoteDivergencia);

                loteDivergencia.QuantidadeDivergenciaMais = divergencia.QuantidadeMaisTratado ?? 0;
                loteDivergencia.QuantidadeDivergenciaMenos = divergencia.QuantidadeMenosTratado ?? 0;
                loteDivergencia.IdUsuarioDivergencia = request.IdUsuario;
                loteDivergencia.IdLoteDivergenciaStatus = LoteDivergenciaStatusEnum.DivergenciaTratada;
                loteDivergencia.DataTratamentoDivergencia = DateTime.Now;

                _uow.LoteDivergenciaRepository.Update(loteDivergencia);

                if (loteDivergencia.QuantidadeDivergenciaMenos > 0)
                {
                    loteDivergenciasMenos.Add(loteDivergencia);
                }
            }

            _uow.SaveChanges();

            return loteDivergenciasMenos;
        }

        public async Task<ProcessamentoTratativaDivergencia> FinalizarProcessamentoTratativaDivergencia(long idLote, string IdUsuario)
        {
            ProcessamentoTratativaDivergencia processamento = new ProcessamentoTratativaDivergencia()
            {
                AtualizacaoNFCompra = true,
                ConfirmacaoNFCompra = true,
                AtualizacaoEstoque = true,
                ProcessamentoErro = false
            };

            try
            {
                Lote lote = _uow.LoteRepository.GetById(idLote);
                
                //TODO Nota de Devolução
                NotaFiscal notafiscalDevolucao = null;

                if (lote.NotaFiscal.CodigoIntegracaoNFDevolucao == null)
                {
                    notafiscalDevolucao = await CriarNFDevolucao();
                    notafiscalDevolucao.IdNotaFiscalStatus = NotaFiscalStatusEnum.NotaDevolucaoCriada;
                    lote.NotaFiscal.CodigoIntegracaoNFDevolucao = notafiscalDevolucao.CodigoIntegracao;
                    lote.IdLoteStatus = LoteStatusEnum.AguardandoConfirmacaoNFDevolucao;
                    _uow.SaveChanges();
                }
                else
                {
                    notafiscalDevolucao = _uow.NotaFiscalRepository.ObterPorCodigoIntegracao(lote.NotaFiscal.CodigoIntegracaoNFDevolucao.Value);
                }

                processamento.CriacaoNFDevolucao = true;

                if (notafiscalDevolucao.IdNotaFiscalStatus != NotaFiscalStatusEnum.Confirmada)
                {
                    await ConfirmarNotaFiscalIntegracao(notafiscalDevolucao);
                    notafiscalDevolucao.IdNotaFiscalStatus = NotaFiscalStatusEnum.Confirmada;
                    lote.IdLoteStatus = LoteStatusEnum.AguardandoAutorizacaoSefaz;
                    _uow.SaveChanges();
                }

                processamento.ConfirmacaoNFDevolucao = true;

                await VerificarNotaFiscalAutorizada(notafiscalDevolucao);
                notafiscalDevolucao.IdNotaFiscalStatus = NotaFiscalStatusEnum.NotaDevolucaoAutorizada;
                _uow.SaveChanges();
                processamento.AutorizaçãoNFDevolucaoSefaz = true;

                List<LoteDivergencia> loteDivergencias = _uow.LoteDivergenciaRepository.RetornarPorNotaFiscal(lote.IdNotaFiscal);

                if (loteDivergencias.Any(a => a.QuantidadeDivergenciaMais > 0) && loteDivergencias.Any(a => a.QuantidadeDivergenciaMenos > 0))
                {
                    lote.IdLoteStatus = LoteStatusEnum.FinalizadoDivergenciaTodas;

                    CriarQuarentena(lote, IdUsuario);
                }
                else if (loteDivergencias.Any(a => a.QuantidadeDivergenciaMenos > 0))
                {
                    lote.IdLoteStatus = LoteStatusEnum.FinalizadoDivergenciaNegativa;
                }

                _uow.SaveChanges();
            }
            catch (Exception e)
            {
                ApplicationLogService log = new ApplicationLogService(_uow);
                log.Error(ApplicationEnum.BackOffice, e);

                processamento.ProcessamentoErro = true;
            }

            return processamento;
        }

        private void AtualizarSaldoArmazenagem(Dictionary<long, List<NotaFiscalItem>> nfItens, NotaFiscal notafiscal, List<LoteDivergencia> loteDivergenciasMenos)
        {
            foreach (var nfItem in nfItens)
            {
                LoteDivergencia divergencia = null;

                if (!loteDivergenciasMenos.NullOrEmpty())
                {
                    divergencia = loteDivergenciasMenos.FirstOrDefault(w => w.IdProduto == nfItem.Key);
                }

                int qtdNF = nfItem.Value.Sum(s => s.Quantidade);

                if (divergencia != null && divergencia.QuantidadeDivergenciaMenos > 0)
                {
                    qtdNF = qtdNF - divergencia.QuantidadeDivergenciaMenos.Value;
                }

                _uow.ProdutoEstoqueRepository.AtualizarSaldoArmazenagem(nfItem.Key, notafiscal.IdEmpresa, qtdNF);
            }
        }

        private void CriarQuarentena(Lote lote, string IdUsuario)
        {
            Quarentena quarentena = new Quarentena()
            {
                DataAbertura = DateTime.UtcNow,
                IdLote = lote.IdLote,
                IdQuarentenaStatus = QuarentenaStatusEnum.Aberto
            };

            _uow.QuarentenaRepository.Add(quarentena, IdUsuario);
            _uow.SaveChanges();
        }

        private async Task ConfirmarNotaFiscalIntegracao(NotaFiscal notafiscal)
        {
            if (!(Convert.ToBoolean(ConfigurationManager.AppSettings["IntegracaoSankhya_Habilitar"])))//TODO Temporário
            {
                return;
            }

            var atualizacaoOK = await IntegracaoSankhya.Instance.ConfirmarNotaFiscal(notafiscal.CodigoIntegracao);

            if (!atualizacaoOK)
            {
                throw new BusinessException("A confirmação da nota fiscal no Sankhya não terminou com sucesso.");
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
                throw new BusinessException("A atualização da nota fiscal no Sankhya não terminou com sucesso.");
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
                throw new BusinessException(string.Format("A nota fiscal {0} está cancelada no Sankhya.", notafiscal.CodigoIntegracao));
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
                throw new BusinessException(string.Format("A nota fiscal {0} ainda não está autorizada no Sankhya.", notafiscal.CodigoIntegracao));
            }
        }

        private async Task<NotaFiscal> CriarNFDevolucao()
        {
            return new NotaFiscal();
        }
    }
}
