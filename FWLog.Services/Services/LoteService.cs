using DartDigital.Library.Exceptions;
using FWLog.Data;
using FWLog.Data.EnumsAndConsts;
using FWLog.Data.Models;
using FWLog.Services.Integracao;
using FWLog.Services.Model.IntegracaoSankhya;
using FWLog.Services.Model.Lote;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using log4net;

namespace FWLog.Services.Services
{
    public class LoteService
    {
        private UnitOfWork _uow;
        private NotaFiscalService notaFiscalService;
        private ILog _log;

        public LoteService(UnitOfWork uow, ILog log)
        {
            _uow = uow;
            notaFiscalService = new NotaFiscalService(_uow, log);
            _log = log;
        }

        public void CriarLoteRecebimento(NotaFiscal notaFiscal, string userId, DateTime dataRecebimento, int? qtdVolumes = null)
        {
            var lote = new Lote
            {
                IdLoteStatus = LoteStatusEnum.Recebido,
                IdNotaFiscal = notaFiscal.IdNotaFiscal,
                DataRecebimento = dataRecebimento,
                IdUsuarioRecebimento = userId,
                QuantidadeVolume = qtdVolumes == null ? notaFiscal.Quantidade : qtdVolumes.Value,
                QuantidadePeca = notaFiscal.NotaFiscalItens.Sum(s => s.Quantidade)
            };

            _uow.LoteRepository.Add(lote);
            _uow.SaveChanges();
        }

        public async Task FinalizarConferencia(long idlote, string userId)
        {
            Lote lote = _uow.LoteRepository.GetById(idlote);
            NotaFiscal notafiscal = _uow.NotaFiscalRepository.GetById(lote.IdNotaFiscal);
            List<LoteConferencia> loteConferencias = _uow.LoteConferenciaRepository.Obter(lote.IdLote);
            List<LoteDivergencia> loteDivergencias = new List<LoteDivergencia>();
            List<LoteConferencia> loteNaoConferido = new List<LoteConferencia>();

            var nfItens = notafiscal.NotaFiscalItens.GroupBy(g => g.IdProduto).ToDictionary(g => g.Key, g => g.ToList());

            foreach (var nfItem in nfItens)
            {
                LoteDivergencia divergencia = null;

                var qtdOriginal = nfItem.Value.Sum(s => s.Quantidade);
                var qtdDevolucao = nfItem.Value.Sum(s => s.QuantidadeDevolucao);
                var conferencia = loteConferencias.Where(x => x.IdProduto == nfItem.Key).ToList();

                if (conferencia.NullOrEmpty())
                {
                    divergencia = new LoteDivergencia
                    {
                        QuantidadeConferenciaMenos = qtdOriginal,
                        QuantidadeDevolucao = qtdDevolucao,
                        QuantidadeConferencia = 0,
                        QuantidadeConferenciaMais = 0
                    };

                    var loteConferencia = new LoteConferencia()
                    {
                        IdLote = lote.IdLote,
                        IdTipoConferencia = notafiscal.Empresa.EmpresaConfig.IdTipoConferencia.Value,
                        IdProduto = nfItem.Key,
                        Quantidade = 0,
                        QuantidadeDevolucao = qtdDevolucao,
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
                        QuantidadeDevolucao = qtdDevolucao,
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
                var qtdDevolucao = item.Value.Sum(s => s.QuantidadeDevolucao);

                LoteDivergencia divergencia = new LoteDivergencia
                {
                    QuantidadeConferenciaMais = qtdConferida,
                    QuantidadeConferenciaMenos = 0,
                    QuantidadeConferencia = qtdConferida,
                    QuantidadeDevolucao = qtdDevolucao,
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
                await ConfirmarNotaFiscalIntegracao(notafiscal.CodigoIntegracao);
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
                    //Registrar quantidade recebida na entidade LoteProduto.
                    RegistrarLoteProduto(nfItens, lote, null);

                    AtualizarSaldoArmazenagem(nfItens, notafiscal, null);
                }

                transactionScope.Complete();
            }
        }

        public async Task<ProcessamentoTratativaDivergencia> DevolucaoTotal(long idlote, string userId)
        {
            Lote lote = _uow.LoteRepository.GetById(idlote);
            NotaFiscal notafiscal = _uow.NotaFiscalRepository.GetById(lote.IdNotaFiscal);
            List<LoteDivergencia> loteDivergencias = new List<LoteDivergencia>();
            List<LoteConferencia> loteConferencias = new List<LoteConferencia>();

            var nfItens = notafiscal.NotaFiscalItens.GroupBy(g => g.IdProduto).ToDictionary(g => g.Key, g => g.ToList());

            foreach (var nfItem in nfItens)
            {
                LoteConferencia conferencia = null;
                LoteDivergencia divergencia = null;

                var qtdOriginal = nfItem.Value.Sum(s => s.Quantidade);
                var qtdDevolucao = nfItem.Value.Sum(s => s.QuantidadeDevolucao);

                var loteConferencia = new LoteConferencia()
                {
                    IdLote = lote.IdLote,
                    IdTipoConferencia = notafiscal.Empresa.EmpresaConfig.IdTipoConferencia.Value,
                    IdProduto = nfItem.Key,
                    Quantidade = 0,
                    QuantidadeDevolucao = qtdDevolucao,
                    DataHoraInicio = DateTime.Now,
                    DataHoraFim = DateTime.Now,
                    Tempo = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0),
                    IdUsuarioConferente = userId
                };

                var loteDivergencia = new LoteDivergencia()
                {
                    IdLote = lote.IdLote,
                    IdProduto = nfItem.Key,
                    IdNotaFiscal = lote.IdNotaFiscal,
                    IdLoteDivergenciaStatus = LoteDivergenciaStatusEnum.AguardandoTratativa,
                    IdUsuarioDivergencia = userId,
                    QuantidadeConferencia = qtdOriginal,
                    QuantidadeConferenciaMais = 0,
                    QuantidadeConferenciaMenos = 0,
                    QuantidadeDivergenciaMais = 0,
                    QuantidadeDivergenciaMenos = 0,
                    QuantidadeDevolucao = qtdDevolucao,
                    DataTratamentoDivergencia = DateTime.Now
                };

                loteConferencias.Add(loteConferencia);
                loteDivergencias.Add(loteDivergencia);
            };

            var tempoTotal = new TimeSpan(0, 0, 0);
            loteConferencias.ForEach(f => { tempoTotal = tempoTotal.Add(new TimeSpan(f.Tempo.Hour, f.Tempo.Minute, f.Tempo.Second)); });
            lote.TempoTotalConferencia = Convert.ToInt64(tempoTotal.TotalSeconds);

            NotaFiscalStatusEnum notaStatus;
            LoteStatusEnum loteStatus;

            loteStatus = LoteStatusEnum.Finalizado;
            notaStatus = NotaFiscalStatusEnum.Confirmada;

            await AtualizarNotaFiscalIntegracao(notafiscal, loteStatus);
            await ConfirmarNotaFiscalIntegracao(notafiscal.CodigoIntegracao);

            using (TransactionScope transactionScope = _uow.CreateTransactionScope())
            {
                lote.IdLoteStatus = loteStatus;
                lote.DataFinalConferencia = DateTime.Now;
                notafiscal.IdNotaFiscalStatus = notaStatus;

                _uow.LoteConferenciaRepository.AddRange(loteConferencias
                    );
                _uow.LoteDivergenciaRepository.AddRange(loteDivergencias);
                _uow.SaveChanges();

                if (notaStatus == NotaFiscalStatusEnum.Confirmada)
                {
                    //Registrar quantidade recebida na entidade LoteProduto.
                    RegistrarLoteProduto(nfItens, lote, null);

                    AtualizarSaldoArmazenagem(nfItens, notafiscal, null);
                }

                transactionScope.Complete();
            }

            ///    Novo Finalizar
            ProcessamentoTratativaDivergencia processamento = new ProcessamentoTratativaDivergencia()
            {
                AtualizacaoNFCompra = true,
                ConfirmacaoNFCompra = true,
                AtualizacaoEstoque = true,
                ProcessamentoErro = false
            };

            try
            {

                //Quarentena
                if (!Convert.ToBoolean(ConfigurationManager.AppSettings["IntegracaoSankhya_Habilitar"]))//TODO Temporário
                {
                    // Este trecho de código está assim para finalizar a tratativa de divergencia quando nao estava comunicando com sankhya
                    lote.IdLoteStatus = LoteStatusEnum.FinalizadoDevolucaoTotal;
                    CriarQuarentena(lote, userId);

                    _uow.SaveChanges();

                    return processamento;
                }

                //Nota de Devolução
                if (!lote.NotaFiscal.CodigoIntegracaoNFDevolucao.HasValue)
                {
                    lote.NotaFiscal.CodigoIntegracaoNFDevolucao = await CarregarNotaFiscalDevolucao(lote);
                    lote.IdLoteStatus = LoteStatusEnum.AguardandoConfirmacaoNFDevolucao;
                    _uow.SaveChanges();

                    await AtualizarNotaFiscalIntegracao(lote.NotaFiscal, lote.IdLoteStatus);
                }

                if (!lote.NotaFiscal.CodigoIntegracaoNFDevolucao.HasValue)
                {
                    throw new BusinessException("Não possível encontrar o Código de Integração da Nota Fiscal de Devolução");
                }

                processamento.CriacaoNFDevolucao = true;

                if (!lote.NotaFiscal.NFDevolucaoConfirmada)
                {
                    await ConfirmarNotaFiscalIntegracao(lote.NotaFiscal.CodigoIntegracaoNFDevolucao.Value);
                    lote.NotaFiscal.NFDevolucaoConfirmada = true;
                    lote.IdLoteStatus = LoteStatusEnum.AguardandoAutorizacaoSefaz;
                    _uow.SaveChanges();

                    await AtualizarNotaFiscalIntegracao(lote.NotaFiscal, lote.IdLoteStatus);
                }

                processamento.ConfirmacaoNFDevolucao = true;

                await VerificarNotaFiscalAutorizada(lote.NotaFiscal.CodigoIntegracaoNFDevolucao.Value);
                processamento.AutorizaçãoNFDevolucaoSefaz = true;


                lote.IdLoteStatus = LoteStatusEnum.FinalizadoDevolucaoTotal;
                CriarQuarentena(lote, userId);

                _uow.SaveChanges();

                await AtualizarNotaFiscalIntegracao(lote.NotaFiscal, lote.IdLoteStatus);
            }
            catch (Exception e)
            {
                _log.Error(e.Message, e);

                processamento.ProcessamentoErro = true;
            }

            return processamento;
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
            await ConfirmarNotaFiscalIntegracao(notafiscal.CodigoIntegracao);

            Dictionary<long, List<NotaFiscalItem>> nfItens = notafiscal.NotaFiscalItens.GroupBy(g => g.IdProduto).ToDictionary(g => g.Key, g => g.ToList());

            using (TransactionScope transactionScope = _uow.CreateTransactionScope())
            {
                notafiscal.IdNotaFiscalStatus = NotaFiscalStatusEnum.Confirmada;
                _uow.SaveChanges();

                List<LoteDivergencia> loteDivergenciasMenos = GravarTratamentoDivergencia(request);

                //Registrar quantidade recebida na entidade LoteProduto.
                RegistrarLoteProduto(nfItens, lote, loteDivergenciasMenos);

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

        public ResumoFinalizarConferenciaResponse ResumoFinalizarConferencia(long idLote, long idEmpresa, string userId)
        {
            EmpresaConfig empresaConfig = _uow.EmpresaConfigRepository.ConsultarPorIdEmpresa(idEmpresa);
            Lote lote = _uow.LoteRepository.GetById(idLote);
            var itensNotaFiscal = _uow.NotaFiscalItemRepository.ObterItens(lote.IdNotaFiscal).GroupBy(g => g.IdProduto);
            List<LoteConferencia> itensConferidos = _uow.LoteConferenciaRepository.ObterPorId(idLote);
            var usuario = _uow.PerfilUsuarioRepository.GetByUserId(userId);

            var response = new ResumoFinalizarConferenciaResponse
            {
                DataRecebimento = lote.DataRecebimento.ToString("dd/MM/yyyy hh:mm:ss"),
                IdLote = lote.IdLote,
                IdNotaFiscal = lote.IdNotaFiscal,
                NumeroNotaFiscal = string.Concat(lote.NotaFiscal.Numero, " - ", lote.NotaFiscal.Serie),
                QuantidadeVolume = lote.QuantidadeVolume,
                RazaoSocialFornecedor = lote.NotaFiscal.Fornecedor.NomeFantasia,
                TipoConferencia = empresaConfig.TipoConferencia.Descricao,
                NomeConferente = usuario.Nome
            };

            foreach (var itemNota in itensNotaFiscal)
            {
                var itensConferencia = itensConferidos.Where(x => x.IdProduto == itemNota.Key);
                int quantidadeNota = itemNota.Sum(s => s.Quantidade);
                int quantidadeDevolucao = itemNota.Sum(s => s.QuantidadeDevolucao);
                string referencia = itemNota.First().Produto.Referencia;
                string descricao = itemNota.First().Produto.Descricao;

                if (itensConferencia.Any())
                {
                    int quantidadeConferido = itensConferencia.Sum(s => s.Quantidade);
                    int diferencaNotaConferido = quantidadeNota - quantidadeConferido;

                    var item = new ResumoFinalizarConferenciaItemResponse
                    {
                        Referencia = referencia,
                        DescricaoProduto = descricao,
                        QuantidadeConferido = quantidadeConferido,
                        QuantidadeDevolucao = quantidadeDevolucao,
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
                        DescricaoProduto = descricao,
                        QuantidadeConferido = 0,
                        QuantidadeDevolucao = quantidadeDevolucao,
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
                int? quantidadeDevolucao = itemForaNota.Sum(s => s.QuantidadeDevolucao);

                var item = new ResumoFinalizarConferenciaItemResponse
                {
                    Referencia = itemForaNota.First().Produto.Referencia,
                    DescricaoProduto = itemForaNota.First().Produto.Descricao,
                    QuantidadeConferido = quantidadeConferido,
                    QuantidadeDevolucao = quantidadeDevolucao ?? 0,
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
                List<LoteDivergencia> loteDivergencias = _uow.LoteDivergenciaRepository.RetornarPorNotaFiscal(lote.IdNotaFiscal);

                if (!Convert.ToBoolean(ConfigurationManager.AppSettings["IntegracaoSankhya_Habilitar"]))//TODO Temporário
                {
                    // Este trecho de código está assim para finalizar a tratativa de divergencia quando nao estava comunicando com sankhya

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

                    return processamento;
                }

                if (!lote.NotaFiscal.CodigoIntegracaoNFDevolucao.HasValue)
                {
                    lote.NotaFiscal.CodigoIntegracaoNFDevolucao = await CarregarNotaFiscalDevolucao(lote);
                    lote.IdLoteStatus = LoteStatusEnum.AguardandoConfirmacaoNFDevolucao;
                    _uow.SaveChanges();

                    await AtualizarNotaFiscalIntegracao(lote.NotaFiscal, lote.IdLoteStatus);
                }

                if (!lote.NotaFiscal.CodigoIntegracaoNFDevolucao.HasValue)
                {
                    throw new BusinessException("Não possível encontrar o Código de Integração da Nota Fiscal de Devolução");
                }

                processamento.CriacaoNFDevolucao = true;

                if (!lote.NotaFiscal.NFDevolucaoConfirmada)
                {
                    await ConfirmarNotaFiscalIntegracao(lote.NotaFiscal.CodigoIntegracaoNFDevolucao.Value);
                    lote.NotaFiscal.NFDevolucaoConfirmada = true;
                    lote.IdLoteStatus = LoteStatusEnum.AguardandoAutorizacaoSefaz;
                    _uow.SaveChanges();

                    await AtualizarNotaFiscalIntegracao(lote.NotaFiscal, lote.IdLoteStatus);
                }

                processamento.ConfirmacaoNFDevolucao = true;

                await VerificarNotaFiscalAutorizada(lote.NotaFiscal.CodigoIntegracaoNFDevolucao.Value);

                processamento.AutorizaçãoNFDevolucaoSefaz = true;

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

                await AtualizarNotaFiscalIntegracao(lote.NotaFiscal, lote.IdLoteStatus);
            }
            catch (Exception e)
            {
                _log.Error(e.Message, e);

                processamento.ProcessamentoErro = true;
                processamento.ProcessamentoErroMensagem = e.Message;
            }

            return processamento;
        }

        private async Task<long> CarregarNotaFiscalDevolucao(Lote lote)
        {
            List<ElementoItemDetalhes> itensDevolucao = new List<ElementoItemDetalhes>();
            List<LoteDivergencia> divergencias = _uow.LoteDivergenciaRepository.RetornarPorNotaFiscal(lote.IdNotaFiscal).Where(w => w.QuantidadeDivergenciaMenos > 0).ToList();
            List<NotaFiscalItem> nfItens = _uow.NotaFiscalItemRepository.ObterItens(lote.IdNotaFiscal);

            foreach (LoteDivergencia divergencia in divergencias)
            {
                var nfProduto = nfItens.Where(w => w.IdProduto == divergencia.IdProduto).OrderBy(o => o.Sequencia).ToList();

                if (nfProduto.NullOrEmpty())
                {
                    throw new BusinessException("Não foi possível encontrar os itens da nota fiscal para tratar a divergência.");
                }

                if (nfProduto.Count() == 1)
                {
                    var itemDevolucao = new ElementoItemDetalhes()
                    {
                        Quantidade = divergencia.QuantidadeDivergenciaMenos.Value,
                        Sequencia = nfProduto.First().Sequencia.ToString(),
                        IdProduto = nfProduto.First().IdProduto
                    };

                    itensDevolucao.Add(itemDevolucao);
                }
                else
                {
                    foreach (NotaFiscalItem nfItem in nfProduto)
                    {
                        int qtdAlocada = 0;
                        int qtdPendente = 0;

                        if (itensDevolucao.Any(s => s.IdProduto == nfItem.IdProduto))
                        {
                            qtdAlocada = itensDevolucao.Where(s => s.IdProduto == nfItem.IdProduto).Sum(s => s.Quantidade);
                        }

                        if (divergencia.QuantidadeDivergenciaMenos > qtdAlocada)
                        {
                            qtdPendente = divergencia.QuantidadeDivergenciaMenos.Value - qtdAlocada;
                        }

                        var itemDevolucao = new ElementoItemDetalhes()
                        {
                            Quantidade = nfItem.Quantidade <= qtdPendente ? nfItem.Quantidade : qtdPendente,
                            Sequencia = nfItem.Sequencia.ToString(),
                            IdProduto = nfItem.IdProduto
                        };

                        itensDevolucao.Add(itemDevolucao);

                        if (itensDevolucao.Where(s => s.IdProduto == nfItem.IdProduto).Sum(s => s.Quantidade) == divergencia.QuantidadeDivergenciaMenos)
                        {
                            break;
                        }
                    }
                }
            }

            long CodigoIntegracaoNFDevolucao = await IntegracaoSankhya.Instance.GerarNotaFiscalDevolucao(lote.NotaFiscal.CodigoIntegracao, itensDevolucao);
            if (CodigoIntegracaoNFDevolucao <= 0)
            {
                throw new BusinessException(string.Format("O serviço de confirmação da nota fiscal respondeu com sucesso, porém a nota fiscal {0} não está confirmada no Sankhya.", lote.NotaFiscal.CodigoIntegracao));
            }

            return CodigoIntegracaoNFDevolucao;
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

        private void RegistrarLoteProduto(Dictionary<long, List<NotaFiscalItem>> nfItens, Lote lote, List<LoteDivergencia> loteDivergenciasMenos)
        {
            LoteProduto loteProduto;

            foreach (var nfItem in nfItens)
            {
                LoteDivergencia divergencia = null;

                //Verifica se houve divergência a menos do produto no lote.
                if (!loteDivergenciasMenos.NullOrEmpty())
                {
                    divergencia = loteDivergenciasMenos.FirstOrDefault(w => w.IdProduto == nfItem.Key); //Captura a divergência do produto.
                }

                //Captura a quantidade total do produto na nota.
                int qtdNF = nfItem.Value.Sum(s => s.Quantidade);

                if (divergencia != null && divergencia.QuantidadeDivergenciaMenos > 0)
                {
                    //Subtrai a quantidade de peças a menos da quantidade total do produto.
                    qtdNF = qtdNF - divergencia.QuantidadeDivergenciaMenos.Value;
                }

                loteProduto = new LoteProduto();

                loteProduto.IdEmpresa = lote.NotaFiscal.IdEmpresa;
                loteProduto.IdLote = lote.IdLote;
                loteProduto.IdProduto = nfItem.Key;
                loteProduto.QuantidadeRecebida = qtdNF;
                loteProduto.Saldo = qtdNF;

                _uow.LoteProdutoRepository.Add(loteProduto);
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

        private async Task ConfirmarNotaFiscalIntegracao(long codigoIntegracao)
        {
            if (!(Convert.ToBoolean(ConfigurationManager.AppSettings["IntegracaoSankhya_Habilitar"])))//TODO Temporário
            {
                return;
            }

            var notaConfirmada = await notaFiscalService.VerificarNotaFiscalConfirmada(codigoIntegracao);

            if (!notaConfirmada)
                await IntegracaoSankhya.Instance.ConfirmarNotaFiscal(codigoIntegracao);

            bool atualizacaoOK = await notaFiscalService.VerificarNotaFiscalConfirmada(codigoIntegracao);
            if (!atualizacaoOK)
            {
                throw new BusinessException(string.Format("O serviço de confirmação da nota fiscal respondeu com sucesso, porém a nota fiscal {0} não está confirmada no Sankhya.", codigoIntegracao));
            }
        }

        public async Task AtualizarNotaFiscalIntegracao(NotaFiscal notafiscal, LoteStatusEnum loteStatusEnum)
        {
            if (!(Convert.ToBoolean(ConfigurationManager.AppSettings["IntegracaoSankhya_Habilitar"])))//TODO Temporário
            {
                return;
            }

            await notaFiscalService.AtualizarNotaFiscalIntegracao(notafiscal, loteStatusEnum);
        }

        private async Task VerificarNotaFiscalAutorizada(long codigoIntegracao)
        {
            if (!(Convert.ToBoolean(ConfigurationManager.AppSettings["IntegracaoSankhya_Habilitar"])))//TODO Temporário
            {
                return;
            }

            bool atualizacaoOK = await notaFiscalService.VerificarNotaFiscalAutorizada(codigoIntegracao);

            if (!atualizacaoOK)
            {
                throw new BusinessException(string.Format("A nota fiscal {0} ainda não está autorizada no Sankhya.", codigoIntegracao));
            }
        }

        public async Task ConferirLoteAutomatico(string userId)
        {
            List<Lote> lotes = await _uow.LoteRepository.ConsultarProcessamentoAutomatico();

            foreach (Lote lote in lotes)
            {
                try
                {
                    await RegistrarConferenciaAutomatico(lote, userId);
                }
                catch (Exception ex)
                {
                    _log.Error(string.Format("Erro no conferência automática do lote - IdNotaFiscal: {0}.", lote.IdLote), ex);
                }
            }
        }

        public async Task RegistrarConferenciaAutomatico(Lote lote, string userId)
        {
            lote.IdLoteStatus = LoteStatusEnum.Conferencia;
            lote.DataInicioConferencia = DateTime.Now;

            await AtualizarNotaFiscalIntegracao(lote.NotaFiscal, lote.IdLoteStatus);

            _uow.LoteConferenciaRepository.DeletePorIdLote(lote.IdLote);

            _uow.SaveChanges();

            foreach (var item in lote.NotaFiscal.NotaFiscalItens)
            {
                LoteConferencia loteConf = new LoteConferencia()
                {
                    IdLote = lote.IdLote,
                    IdTipoConferencia = TipoConferenciaEnum.PorQuantidade,
                    IdProduto = item.IdProduto,
                    Quantidade = item.Quantidade,
                    DataHoraInicio = DateTime.Now,
                    DataHoraFim = DateTime.Now,
                    Tempo = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0),
                    IdUsuarioConferente = userId
                };

                _uow.LoteConferenciaRepository.Add(loteConf);
            }

            _uow.SaveChanges();

            await FinalizarConferencia(lote.IdLote, userId);
        }

        public LoteProduto BuscaProdutoNoLote(long idLote, long idProduto)
        {
            if (idLote <= 0)
            {
                throw new BusinessException("O lote deve ser informado.");
            }

            var lote = _uow.LoteRepository.GetById(idLote);

            if (lote == null)
            {
                throw new BusinessException("O lote não foi encontrado.");
            }

            if (idProduto <= 0)
            {
                throw new BusinessException("O produto deve ser informado.");
            }

            var produto = _uow.ProdutoRepository.GetById(idProduto);

            if (produto == null)
            {
                throw new BusinessException("O produto não foi encontrado.");
            }

            var produtoLote = _uow.LoteProdutoRepository.ConsultarPorLoteProduto(idLote, idProduto);

            if (produtoLote == null)
            {
                throw new BusinessException("O produto não foi encontrado no lote.");
            }

            return produtoLote;
        }
    }
}