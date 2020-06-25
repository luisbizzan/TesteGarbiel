﻿using DartDigital.Library.Exceptions;
using FWLog.Data;
using FWLog.Data.Models;
using FWLog.Data.Models.DataTablesCtx;
using FWLog.Data.Models.FilterCtx;
using FWLog.Services.Integracao;
using FWLog.Services.Model.Coletor;
using FWLog.Services.Model.Expedicao;
using FWLog.Services.Model.IntegracaoSankhya;
using FWLog.Services.Model.Relatorios;
using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FWLog.Services.Services
{
    public class ExpedicaoService
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly ILog _log;
        private readonly ColetorHistoricoService _coletorHistoricoService;
        private readonly NotaFiscalService _notaFiscalService;
        private readonly PedidoService _pedidoService;
        private readonly RelatorioService _relatorioService;
        private readonly TransportadoraService _transportadoraService;
        private readonly RomaneioService _romaneioService;
        private readonly PedidoVendaVolumeService _pedidoVendaVolumeService;
        private readonly SeparacaoPedidoService _separacaoPedidoService;

        public ExpedicaoService(
            UnitOfWork unitOfWork,
            ILog log,
            ColetorHistoricoService coletorHistoricoService,
            NotaFiscalService notaFiscalService,
            PedidoService pedidoService,
            RelatorioService relatorioService,
            TransportadoraService transportadoraService,
            RomaneioService romaneioService,
            PedidoVendaVolumeService pedidoVendaVolumeService,
            SeparacaoPedidoService separacaoPedidoService)
        {
            _unitOfWork = unitOfWork;
            _log = log;
            _coletorHistoricoService = coletorHistoricoService;
            _notaFiscalService = notaFiscalService;
            _pedidoService = pedidoService;
            _relatorioService = relatorioService;
            _transportadoraService = transportadoraService;
            _romaneioService = romaneioService;
            _pedidoVendaVolumeService = pedidoVendaVolumeService;
            _separacaoPedidoService = separacaoPedidoService;
        }

        public void IniciarExpedicaoPedidoVenda(long idPedidoVenda, long idPedidoVendaVolume, long idEmpresa)
        {
            // validações
            if (idPedidoVenda <= 0)
            {
                throw new BusinessException("O pedido de venda deve ser informado.");
            }

            var pedidoVenda = _unitOfWork.PedidoVendaRepository.ObterPorIdPedidoVendaEIdEmpresa(idPedidoVenda, idEmpresa);

            if (pedidoVenda == null)
            {
                throw new BusinessException("O pedido não foi encontrado.");
            }

            if (pedidoVenda.IdEmpresa != idEmpresa)
            {
                throw new BusinessException("O pedido não pertence a empresa do usuário logado.");
            }

            if (pedidoVenda.IdPedidoVendaStatus == PedidoVendaStatusEnum.VolumeExcluido)
            {
                throw new BusinessException("O pedido foi excluído.");
            }

            if (pedidoVenda.IdPedidoVendaStatus == PedidoVendaStatusEnum.AguardandoRetirada)
            {
                throw new BusinessException("O pedido está aguardando retirada.");
            }

            if (pedidoVenda.IdPedidoVendaStatus != PedidoVendaStatusEnum.SeparacaoConcluidaComSucesso &&
                pedidoVenda.IdPedidoVendaStatus != PedidoVendaStatusEnum.InstalandoVolumeTransportadora &&
                pedidoVenda.IdPedidoVendaStatus != PedidoVendaStatusEnum.MovendoDOCA)
            {
                throw new BusinessException("A separação do volume não está finalizada.");
            }

            var pedidoVendaVolume = pedidoVenda.PedidoVendaVolumes.First(f => f.IdPedidoVendaVolume == idPedidoVendaVolume);

            if (pedidoVendaVolume == null)
            {
                throw new BusinessException("O volume não foi encontrado.");
            }

            if (pedidoVendaVolume.IdPedidoVendaStatus == PedidoVendaStatusEnum.VolumeExcluido)
            {
                throw new BusinessException("O volume foi excluído.");
            }

            if (pedidoVendaVolume.IdPedidoVendaStatus != PedidoVendaStatusEnum.SeparacaoConcluidaComSucesso &&
                pedidoVendaVolume.IdPedidoVendaStatus != PedidoVendaStatusEnum.InstalandoVolumeTransportadora &&
                pedidoVendaVolume.IdPedidoVendaStatus != PedidoVendaStatusEnum.RemovidoDOCA)
            {
                throw new BusinessException("A separação do volume não está finalizada.");
            }
        }

        private void BuscaEValidaDadosPorReferenciaPedido(string referenciaPedido, out string numeroPedido, out long idTransportadora, out int numeroVolume)
        {
            if (referenciaPedido.NullOrEmpty())
            {
                throw new BusinessException("Código de barras do pedido deve ser infomado.");
            }

            if (referenciaPedido.Length < 7)
            {
                throw new BusinessException("Código de Barras de pedido inválido.");
            }

            numeroPedido = referenciaPedido.Substring(0, referenciaPedido.Length - 6);

            var idTransportadoraString = referenciaPedido.Substring(referenciaPedido.Length - 6, 3);

            if (!long.TryParse(idTransportadoraString, out idTransportadora))
            {
                throw new BusinessException("Código de Barras de pedido inválido.");
            }

            var numeroVolumeString = referenciaPedido.Substring(referenciaPedido.Length - 3);

            if (!int.TryParse(numeroVolumeString, out numeroVolume))
            {
                throw new BusinessException("Código de Barras de pedido inválido.");
            }
        }

        public PedidoVendaVolumeResposta BuscaPedidoVendaVolume(string referenciaPedido, long idEmpresa)
        {
            BuscaEValidaDadosPorReferenciaPedido(referenciaPedido, out string numeroPedido, out long idTransportadora, out int numeroVolume);

            var pedidoVenda = _unitOfWork.PedidoVendaRepository.ObterPorNroPedidoEEmpresa(numeroPedido, idEmpresa);

            if (pedidoVenda == null)
            {
                throw new BusinessException("Pedido não encontrado.");
            }

            var pedidoVendaVolume = pedidoVenda.PedidoVendaVolumes.FirstOrDefault(volume => volume.NroVolume == numeroVolume);

            if (pedidoVendaVolume == null)
            {
                throw new BusinessException("Volume fornecido não encontrado.");
            }

            if (pedidoVendaVolume.IdEnderecoArmazTransportadora != null)
            {
                throw new BusinessException($"Volume já instalado em: {pedidoVendaVolume.EnderecoTransportadora.Codigo}");
            }

            if (pedidoVenda.Transportadora.IdTransportadora != Convert.ToInt32(idTransportadora))
            {
                throw new BusinessException("Transportadora da referência está incorreta");
            }

            if (pedidoVenda.Transportadora.Enderecos.NullOrEmpty())
            {
                throw new BusinessException("Endereço da transportadora não cadastrado!");
            }

            var resposta = new PedidoVendaVolumeResposta()
            {
                IdPedidoVenda = pedidoVenda.IdPedidoVenda,
                IdTransportadora = pedidoVenda.IdTransportadora,
                IdPedidoVendaVolume = pedidoVendaVolume.IdPedidoVendaVolume
            };

            return resposta;
        }

        public async Task AtualizaNotasFiscaisPedidos()
        {
            if (!Convert.ToBoolean(ConfigurationManager.AppSettings["IntegracaoSankhya_Habilitar"]))
            {
                return;
            }

            var pedidosSemNota = _unitOfWork.PedidoRepository.ObterPedidosSemNotaFiscal();

            foreach (var pedido in pedidosSemNota)
            {
                try
                {
                    var dadosNotaFiscal = await ConsultarSituacaoNFVenda(pedido);

                    if (dadosNotaFiscal != null)
                    {
                        var salvaPedido = false;

                        if (long.TryParse(dadosNotaFiscal.CodigoIntegracaoNotaFiscal, out long codigoIntegracaoNotaFiscal))
                        {
                            pedido.CodigoIntegracaoNotaFiscal = codigoIntegracaoNotaFiscal;

                            salvaPedido = true;
                        }

                        if (int.TryParse(dadosNotaFiscal.NumeroNotaFiscal, out int numeroNotaFiscal) && !string.IsNullOrWhiteSpace(dadosNotaFiscal.SerieNotaFiscal))
                        {
                            pedido.NumeroNotaFiscal = numeroNotaFiscal;
                            pedido.SerieNotaFiscal = dadosNotaFiscal.SerieNotaFiscal;

                            salvaPedido = true;
                        }

                        if (!string.IsNullOrWhiteSpace(dadosNotaFiscal.TipoFrete) && !string.Equals(pedido.CodigoIntegracaoTipoFrete, dadosNotaFiscal.TipoFrete))
                        {
                            pedido.CodigoIntegracaoTipoFrete = dadosNotaFiscal.TipoFrete;

                            salvaPedido = true;
                        }

                        if (string.IsNullOrWhiteSpace(dadosNotaFiscal.ChaveAcesso))
                        {
                            throw new BusinessException($"Erro ao atualizar a nota fiscal do pedido {pedido.IdPedido} por não haver o número da DANFE");
                        }
                        else if (!string.Equals(pedido.ChaveAcessoNotaFiscal, dadosNotaFiscal.ChaveAcesso))
                        {
                            pedido.ChaveAcessoNotaFiscal = dadosNotaFiscal.ChaveAcesso;

                            salvaPedido = true;
                        }

                        if (!salvaPedido)
                        {
                            return;
                        }

                        var pedidoVenda = _unitOfWork.PedidoVendaRepository.ObterPorIdPedido(pedido.IdPedido);

                        if (pedidoVenda == null)
                        {
                            throw new BusinessException($"Não foi encontrado pedido de venda para o pedido. Código Integração do Pedido: {pedido.CodigoIntegracao}");
                        }

                        if (pedidoVenda.PedidoVendaVolumes == null)
                        {
                            throw new BusinessException($"O pedido de venda não tem volumes. Código Integração do Pedido: {pedido.CodigoIntegracao}");
                        }

                        if (!pedidoVenda.PedidoVendaVolumes.Any(a => a.PedidoVendaProdutos != null))
                        {
                            throw new BusinessException($"Os volumes do pedido de venda não tem produtos. Código Integração do Pedido: {pedido.CodigoIntegracao}");
                        }

                        List<PedidoVendaProduto> produtos = new List<PedidoVendaProduto>();

                        pedidoVenda.PedidoVendaVolumes.Where(w => w.IdPedidoVendaStatus != PedidoVendaStatusEnum.VolumeExcluido)
                            .ForEach(f => f.PedidoVendaProdutos.Where(w => w.IdPedidoVendaStatus != PedidoVendaStatusEnum.VolumeExcluido && w.QtdSeparada > 0)
                                .ForEach(ff => produtos.Add(ff)));

                        if (produtos == null)
                        {
                            throw new BusinessException("Produto não tem configuração de estoque.");
                        }

                        var produtosAgrupados = produtos.GroupBy(g => g.IdProduto).ToDictionary(d => d.Key, d => d.ToList());

                        foreach (var produto in produtosAgrupados)
                        {
                            var produtoEstoque = _unitOfWork.ProdutoEstoqueRepository.ConsultarPorProduto(produto.Key, pedido.IdEmpresa);

                            if (produtoEstoque == null)
                            {
                                throw new BusinessException("Produto não tem configuração de estoque.");
                            }

                            var qtdSeparadaTotal = produto.Value.Sum(s => s.QtdSeparada);

                            if ((produtoEstoque.Saldo - qtdSeparadaTotal) < 0)
                            {
                                throw new BusinessException("Produto não tem saldo suficiente.");
                            }
                        }

                        using (var transacao = _unitOfWork.CreateTransactionScope())
                        {
                            await _unitOfWork.SaveChangesAsync();

                            foreach (var produto in produtosAgrupados)
                            {
                                var produtoEstoque = _unitOfWork.ProdutoEstoqueRepository.ConsultarPorProduto(produto.Key, pedido.IdEmpresa);

                                var qtdSeparadaTotal = produto.Value.Sum(s => s.QtdSeparada);

                                produtoEstoque.Saldo -= qtdSeparadaTotal.Value;

                                _unitOfWork.ProdutoEstoqueRepository.Update(produtoEstoque);

                                await _unitOfWork.SaveChangesAsync();
                            }

                            transacao.Complete();
                        }
                    }
                }
                catch (BusinessException ex)
                {
                    _log.Error(ex.Message, ex);
                }
                catch (Exception exception)
                {
                    _log.Error(string.Format("Erro na tentativa de integração de nota fiscal do pedido {0}", pedido.IdPedido), exception);
                }
            }
        }

        public async Task<bool> ValidarSituacaoNFVenda(Pedido pedido)
        {
            if (!Convert.ToBoolean(ConfigurationManager.AppSettings["IntegracaoSankhya_Habilitar"]))
            {
                return true;
            }

            if (!pedido.CodigoIntegracaoNotaFiscal.HasValue || pedido.CodigoIntegracaoNotaFiscal <= 0)
            {
                throw new BusinessException("Pedido não tem nota fiscal emitida.");
            }

            await _notaFiscalService.VerificarNotaFiscalCancelada(pedido.CodigoIntegracaoNotaFiscal.Value);

            PedidoNumeroNotaFiscalIntegracao dadosNotaFiscal = await ConsultarSituacaoNFVenda(pedido);

            if (dadosNotaFiscal == null)
            {
                throw new BusinessException($"Nota Fiscal {pedido.CodigoIntegracaoNotaFiscal} não foi encontrada no Sankhya.");
            }

            if (!(long.TryParse(dadosNotaFiscal.CodigoIntegracaoNotaFiscal, out long numeroNotaFiscal) && pedido.CodigoIntegracaoNotaFiscal == numeroNotaFiscal))
            {
                return false;
            }

            return true;
        }

        private async Task<PedidoNumeroNotaFiscalIntegracao> ConsultarSituacaoNFVenda(Pedido pedido)
        {
            if (!Convert.ToBoolean(ConfigurationManager.AppSettings["IntegracaoSankhya_Habilitar"]))
            {
                return null;
            }

            var where = $"WHERE TGFVAR.NUNOTAORIG = {pedido.CodigoIntegracao} AND TGFCAB.STATUSNFE IN ('A','S') AND TGFCAN.NUNOTA IS NULL";
            var inner = "INNER JOIN TGFVAR ON TGFVAR.NUNOTA = TGFCAB.NUNOTA";
            inner += " LEFT JOIN TGFCAN ON TGFCAN.NUNOTA = TGFCAB.NUNOTA";

            var dadosIntegracaoSankhya = await IntegracaoSankhya.Instance.PreExecutarQuery<PedidoNumeroNotaFiscalIntegracao>(where: where, inner: inner);

            return dadosIntegracaoSankhya.FirstOrDefault();
        }

        public PedidoVendaVolume ValidarTransportadoraInstalacaoVolume(long idPedidoVendaVolume, long idtransportadora, long idEmpresa)
        {
            if (idPedidoVendaVolume <= 0)
            {
                throw new BusinessException("O volume deve ser informado.");
            }

            if (idtransportadora <= 0)
            {
                throw new BusinessException("A transportadora deve ser informada.");
            }

            var pedidoVendaVolume = _unitOfWork.PedidoVendaVolumeRepository.GetById(idPedidoVendaVolume);

            if (pedidoVendaVolume == null)
            {
                throw new BusinessException("O volume não foi encontrado.");
            }

            if (pedidoVendaVolume.PedidoVenda.IdEmpresa != idEmpresa)
            {
                throw new BusinessException("O volume informado não pertence a empresa do usuário logado.");
            }

            var transportadora = _unitOfWork.TransportadoraRepository.GetById(idtransportadora);

            if (transportadora == null)
            {
                throw new BusinessException("A transportadora informada não existe.");
            }

            if (pedidoVendaVolume.PedidoVenda.IdTransportadora != idtransportadora)
            {
                throw new BusinessException("Transportadora diferente dos volumes.");
            }

            return pedidoVendaVolume;
        }

        public void ValidarEnderecoInstalacaoVolume(long idPedidoVendaVolume, long idTransportadora, long idEnderecoArmazenagem, long idEmpresa)
        {
            var pedidoVendaVolume = ValidarTransportadoraInstalacaoVolume(idPedidoVendaVolume, idTransportadora, idEmpresa);

            var enderecoTransportadora = _unitOfWork.TransportadoraEnderecoRepository.ObterPorEnderecoTransportadoraEmpresa(idEnderecoArmazenagem, pedidoVendaVolume.PedidoVenda.IdTransportadora, idEmpresa);

            if (enderecoTransportadora == null)
            {
                throw new BusinessException("Endereço não pertence a transportadora.");
            }
        }

        public async Task SalvarInstalacaoVolumes(List<long> listaIdsVolumes, long idTransportadora, long idEnderecoArmazenagem, long idEmpresa, string idUsuario)
        {
            if (listaIdsVolumes.NullOrEmpty())
            {
                throw new BusinessException("Volumes devem ser informados.");
            }

            if (idEnderecoArmazenagem <= 0)
            {
                throw new BusinessException("Endereço deve ser informado.");
            }

            var enderecoInstalacao = _unitOfWork.EnderecoArmazenagemRepository.GetById(idEnderecoArmazenagem);

            if (enderecoInstalacao == null)
            {
                throw new BusinessException("Endereço não encontrado.");
            }

            foreach (var idPedidoVendaVolume in listaIdsVolumes)
            {
                ValidarEnderecoInstalacaoVolume(idPedidoVendaVolume, idTransportadora, idEnderecoArmazenagem, idEmpresa);
            }

            var listaPedidoVendaVolume = new List<PedidoVendaVolume>();

            foreach (var idPedidoVendaVolume in listaIdsVolumes)
            {
                var pedidoVendaVolume = _unitOfWork.PedidoVendaVolumeRepository.GetById(idPedidoVendaVolume);

                if (pedidoVendaVolume.IdPedidoVendaStatus != PedidoVendaStatusEnum.SeparacaoConcluidaComSucesso && pedidoVendaVolume.IdPedidoVendaStatus != PedidoVendaStatusEnum.RemovidoDOCA)
                {
                    throw new BusinessException($"Volume {pedidoVendaVolume.NroVolume} com status inválido.");
                }

                listaPedidoVendaVolume.Add(pedidoVendaVolume);
            }

            if (listaPedidoVendaVolume.Select(pvv => pvv.IdPedidoVenda).Distinct().Count() > 1)
            {
                throw new BusinessException("Existem volumes de diferentes pedidos.");
            }

            using (var transacao = _unitOfWork.CreateTransactionScope())
            {
                var listaVolumesString = new List<string>();

                foreach (var pedidoVendaVolume in listaPedidoVendaVolume)
                {
                    pedidoVendaVolume.IdEnderecoArmazTransportadora = idEnderecoArmazenagem;

                    pedidoVendaVolume.IdUsuarioInstalTransportadora = idUsuario;

                    pedidoVendaVolume.DataHoraInstalTransportadora = DateTime.Now;

                    pedidoVendaVolume.IdPedidoVendaStatus = PedidoVendaStatusEnum.VolumeInstaladoTransportadora;

                    pedidoVendaVolume.PedidoVenda.IdPedidoVendaStatus = PedidoVendaStatusEnum.InstalandoVolumeTransportadora;

                    await _unitOfWork.SaveChangesAsync();

                    listaVolumesString.Add(GerarReferenciaPedidoVolume(pedidoVendaVolume));
                }

                var pedidoVenda = _unitOfWork.PedidoVendaRepository.GetById(listaPedidoVendaVolume.First().IdPedidoVenda);

                if (pedidoVenda.PedidoVendaVolumes.Where(w => w.IdPedidoVendaStatus != PedidoVendaStatusEnum.VolumeExcluido).All(pvv => pvv.IdPedidoVendaStatus == PedidoVendaStatusEnum.VolumeInstaladoTransportadora))
                {
                    pedidoVenda.IdPedidoVendaStatus = PedidoVendaStatusEnum.VolumeInstaladoTransportadora;
                    pedidoVenda.Pedido.IdPedidoVendaStatus = PedidoVendaStatusEnum.VolumeInstaladoTransportadora;

                    await _unitOfWork.SaveChangesAsync();
                }

                var stringVolumes = string.Join(", ", listaVolumesString);

                _coletorHistoricoService.GravarHistoricoColetor(new GravarHistoricoColetorRequisicao
                {
                    IdColetorAplicacao = ColetorAplicacaoEnum.Expedicao,
                    IdColetorHistoricoTipo = ColetorHistoricoTipoEnum.InstalarMultiplo,
                    Descricao = $"Os volumes {stringVolumes} foram instalados no endereço {enderecoInstalacao.Codigo} para a transportadora {pedidoVenda.Transportadora.NomeFantasia}.",
                    IdEmpresa = idEmpresa,
                    IdUsuario = idUsuario
                });

                transacao.Complete();
            }
        }

        public PedidoVendaVolumeResposta ValidarVolumeDoca(string referenciaPedido, long idEmpresa)
        {
            BuscaEValidaDadosPorReferenciaPedido(referenciaPedido, out string numeroPedido, out long idTransportadora, out int numeroVolume);

            var pedidoVenda = _unitOfWork.PedidoVendaRepository.ObterPorNroPedidoEEmpresa(numeroPedido, idEmpresa);

            if (pedidoVenda == null)
            {
                throw new BusinessException("Pedido não encontrado.");
            }

            var pedidoVendaVolume = pedidoVenda.PedidoVendaVolumes.FirstOrDefault(volume => volume.NroVolume == numeroVolume);

            if (pedidoVendaVolume == null)
            {
                throw new BusinessException("Volume fornecido não encontrado.");
            }

            if (pedidoVendaVolume.IdEnderecoArmazTransportadora == null)
            {
                throw new BusinessException($"O volume não foi instalado.");
            }

            if (pedidoVenda.Transportadora.IdTransportadora != idTransportadora)
            {
                throw new BusinessException("Este volume não pertence a esta transportadora.");
            }

            if (pedidoVenda.Transportadora.Enderecos.NullOrEmpty())
            {
                throw new BusinessException("Endereço da transportadora não cadastrado.");
            }

            if (!pedidoVenda.Transportadora.Enderecos.Any(e => e.IdEnderecoArmazenagem == pedidoVendaVolume.IdEnderecoArmazTransportadora))
            {
                throw new BusinessException($"O volume não foi instalado.");
            }

            if (Convert.ToBoolean(ConfigurationManager.AppSettings["IntegracaoSankhya_Habilitar"]))
            {
                if (!pedidoVenda.Pedido.CodigoIntegracaoNotaFiscal.HasValue && !pedidoVenda.Pedido.IsRequisicao)
                {
                    throw new BusinessException("Este volume não tem uma nota fiscal faturada.");
                }
            }

            if (pedidoVendaVolume.DataHoraInstalacaoDOCA != null && pedidoVendaVolume.DataHoraInstalacaoDOCA != default)
            {
                throw new BusinessException("Este volume já foi lido, ou está em duplicidade.");
            }

            if (pedidoVendaVolume.IdPedidoVendaStatus != PedidoVendaStatusEnum.VolumeInstaladoTransportadora &&
                pedidoVendaVolume.IdPedidoVendaStatus != PedidoVendaStatusEnum.MovendoDOCA)
            {
                throw new BusinessException("Há volumes ainda não locados deste pedido.");
            }

            var resposta = new PedidoVendaVolumeResposta()
            {
                IdPedidoVenda = pedidoVenda.IdPedidoVenda,
                IdTransportadora = pedidoVenda.IdTransportadora,
                IdPedidoVendaVolume = pedidoVendaVolume.IdPedidoVendaVolume
            };

            return resposta;
        }

        public DespachoTransportadoraResposta ValidarDespachoTransportadora(long idTransportadora, long idEmpresa)
        {
            var transportadora = _transportadoraService.ValidarERetornarTransportadora(idTransportadora);

            var volumesForaDoca = _unitOfWork.PedidoVendaRepository.BuscarVolumesForaDoca(transportadora.IdTransportadora, idEmpresa);

            var resposta = new DespachoTransportadoraResposta()
            {
                IdTransportadora = transportadora.IdTransportadora,
                VolumesForaDoca = volumesForaDoca.Select(vfd => new DespachoTransportadoraVolumeResposta()
                {
                    IdPedidoVendaVolume = vfd.IdPedidoVendaVolume,
                    NumeroPedido = vfd.NumeroPedido.ToString(),
                    NumeroVolume = vfd.NumeroVolume.ToString().PadLeft(3, '0'),
                    EnderecoCodigo = vfd.EnderecoCodigo
                }).ToList()
            };

            return resposta;
        }

        public void FinalizarMovimentacaoDoca(List<long> idsVolume, long idTransportadora, string idUsuario, long idEmpresa)
        {
            if (idsVolume.NullOrEmpty())
            {
                throw new BusinessException($"Favor informar os volumes para finalização da movimentação para doca.");
            }

            var pedidoVolumes = _unitOfWork.PedidoVendaVolumeRepository.ObterPorIdsPedidoVendaVolume(idsVolume);
            var transportadora = _unitOfWork.TransportadoraRepository.GetById(idTransportadora);

            var dataProcessamento = DateTime.Now;

            using (var transacao = _unitOfWork.CreateTransactionScope())
            {
                foreach (var volume in pedidoVolumes)
                {
                    var referenciaVolume = GerarReferenciaPedidoVolume(volume);

                    if (volume.DataHoraInstalacaoDOCA != null)
                    {
                        throw new BusinessException($"Volume { referenciaVolume } ja foi lido ou está em duplicidade.");
                    }

                    if (volume.IdPedidoVendaStatus != PedidoVendaStatusEnum.MovendoDOCA && volume.IdPedidoVendaStatus != PedidoVendaStatusEnum.VolumeInstaladoTransportadora)
                    {
                        throw new BusinessException($"Volume { referenciaVolume } não pode ser instalado na doca.");
                    }

                    if (Convert.ToBoolean(ConfigurationManager.AppSettings["IntegracaoSankhya_Habilitar"]))
                    {
                        if (volume.PedidoVenda.Pedido.CodigoIntegracaoNotaFiscal == null && !volume.PedidoVenda.Pedido.IsRequisicao)
                        {
                            throw new BusinessException($"Volume { referenciaVolume } não tem nota fiscal faturada.");
                        }
                    }

                    if (volume.PedidoVenda.IdTransportadora != idTransportadora)
                    {
                        throw new BusinessException($"Volume { referenciaVolume } não pertence a transportadora: {transportadora.NomeFantasia}.");
                    }

                    volume.IdUsuarioInstalacaoDOCA = idUsuario;
                    volume.DataHoraInstalacaoDOCA = dataProcessamento;
                    volume.IdPedidoVendaStatus = PedidoVendaStatusEnum.MovidoDOCA;

                    if (!volume.PedidoVenda.PedidoVendaVolumes.Where(w => w.IdPedidoVendaStatus != PedidoVendaStatusEnum.VolumeExcluido).Any(a => a.IdPedidoVendaStatus != PedidoVendaStatusEnum.MovidoDOCA))
                    {
                        volume.PedidoVenda.IdPedidoVendaStatus = PedidoVendaStatusEnum.MovidoDOCA;
                        volume.PedidoVenda.Pedido.IdPedidoVendaStatus = PedidoVendaStatusEnum.MovidoDOCA;
                    }
                    else
                    {
                        volume.PedidoVenda.IdPedidoVendaStatus = PedidoVendaStatusEnum.MovendoDOCA;
                        volume.PedidoVenda.Pedido.IdPedidoVendaStatus = PedidoVendaStatusEnum.MovendoDOCA;
                    }

                    _unitOfWork.SaveChanges();
                }

                transacao.Complete();
            }

            string stringVolumes;

            if (pedidoVolumes.Count > 1)
            {
                var ultimoVolume = pedidoVolumes.Last();

                stringVolumes = string.Join(", ", pedidoVolumes.Where(w => w.IdPedidoVendaVolume != ultimoVolume.IdPedidoVendaVolume).Select(s => GerarReferenciaPedidoVolume(s)).ToArray());

                stringVolumes = string.Concat(stringVolumes, $" e { GerarReferenciaPedidoVolume(ultimoVolume) }");
            }
            else
            {
                stringVolumes = GerarReferenciaPedidoVolume(pedidoVolumes.First());
            }

            _coletorHistoricoService.GravarHistoricoColetor(new GravarHistoricoColetorRequisicao
            {
                IdColetorAplicacao = ColetorAplicacaoEnum.Expedicao,
                IdColetorHistoricoTipo = ColetorHistoricoTipoEnum.MoverDoca,
                Descricao = $"Os Volumes {stringVolumes} foram movidos para a DOCA da transportadora {transportadora.NomeFantasia}.",
                IdEmpresa = idEmpresa,
                IdUsuario = idUsuario
            });
        }

        public async Task FinalizarDespachoNF(long idTransportadora, string chaveAcesso, string idUsuario, long idEmpresa)
        {
            _transportadoraService.ValidarERetornarTransportadora(idTransportadora);

            ValidarChaveAcessoNF(chaveAcesso);

            Pedido pedido = _unitOfWork.PedidoRepository.PesquisaPorChaveAcesso(chaveAcesso);

            if (pedido.IdTransportadora != idTransportadora)
            {
                throw new BusinessException("Nota fiscal não pertence a transportadora informada.");
            }

            PedidoVenda pedidoVenda = _unitOfWork.PedidoVendaRepository.ObterPorIdPedido(pedido.IdPedido);

            if (pedidoVenda == null)
            {
                throw new BusinessException("Não existe pedido venda para chave de acesso informada.");
            }

            if (pedidoVenda.IdPedidoVendaStatus == PedidoVendaStatusEnum.DespachandoNF ||
                pedidoVenda.IdPedidoVendaStatus == PedidoVendaStatusEnum.NFDespachada ||
                pedidoVenda.IdPedidoVendaStatus == PedidoVendaStatusEnum.RomaneioImpresso)
            {
                throw new BusinessException("Nota fiscal já foi despachada.");
            }

            if (pedidoVenda.IdPedidoVendaStatus != PedidoVendaStatusEnum.MovidoDOCA)
            {
                throw new BusinessException("Nota fiscal não está instalada na doca.");
            }

            if (!await ValidarSituacaoNFVenda(pedido))
            {
                throw new BusinessException("Nota fiscal não está autorizada no Sankhya.");
            }

            using (var transacao = _unitOfWork.CreateTransactionScope())
            {
                pedidoVenda.IdPedidoVendaStatus = PedidoVendaStatusEnum.NFDespachada;
                pedidoVenda.DataHoraDespachoNotaFiscal = DateTime.Now;
                pedidoVenda.IdUsuarioDespachoNotaFiscal = idUsuario;

                pedido.IdPedidoVendaStatus = PedidoVendaStatusEnum.NFDespachada;

                await _pedidoService.AtualizarStatusPedido(pedidoVenda.Pedido, PedidoVendaStatusEnum.NFDespachada);

                _coletorHistoricoService.GravarHistoricoColetor(new GravarHistoricoColetorRequisicao
                {
                    IdColetorAplicacao = ColetorAplicacaoEnum.Expedicao,
                    IdColetorHistoricoTipo = ColetorHistoricoTipoEnum.DespachoNF,
                    Descricao = $"Nota fiscal {pedido.CodigoIntegracaoNotaFiscal} despachada para a transportadora {pedido.Transportadora.NomeFantasia}. Chave de Acesso: {pedido.ChaveAcessoNotaFiscal}.",
                    IdEmpresa = idEmpresa,
                    IdUsuario = idUsuario
                });

                _unitOfWork.SaveChanges();

                transacao.Complete();
            }
        }

        public void ImprimirRomaneio(long idRomaneio, long idImpressora, bool imprimeSegundaVia, long idEmpresa, string idUsuario)
        {
            if (idRomaneio <= 0)
            {
                throw new BusinessException("O romaneio deve ser informado.");
            }

            if (idImpressora <= 0)
            {
                throw new BusinessException("Impressora deve ser informada.");
            }

            var romaneio = _unitOfWork.RomaneioRepository.BuscarPorIdRomaneioEEmpresa(idRomaneio, idEmpresa);

            if (romaneio == null)
            {
                throw new BusinessException("Romaneio não encontrado.");
            }

            if (romaneio.RomaneioNotaFiscal.NullOrEmpty())
            {
                throw new BusinessException("Notas Fiscais de romaneio não encontradas.");
            }

            if (_unitOfWork.BOPrinterRepository.GetById(idImpressora) == null)
            {
                throw new BusinessException("Impressora não encontrada.");
            }

            _relatorioService.ImprimirRomaneio(new RelatorioRomaneioRequest()
            {
                Romaneio = romaneio,
                DataHoraEmissaoRomaneio = romaneio.RomaneioNotaFiscal.FirstOrDefault().PedidoVenda.DataHoraRomaneio.Value,
                IdEmpresa = idEmpresa,
                IdUsuarioExecucao = idUsuario
            }, idImpressora, imprimeSegundaVia);
        }

        public void ValidarNotaFiscalRomaneio(long idTransportadora, string chaveAcesso)
        {
            _transportadoraService.ValidarERetornarTransportadora(idTransportadora);

            ValidarChaveAcessoNF(chaveAcesso);

            Pedido pedido = _unitOfWork.PedidoRepository.PesquisaPorChaveAcesso(chaveAcesso);

            if (pedido == null)
            {
                throw new BusinessException("A nota fiscal não encontrada.");
            }

            PedidoVenda pedidoVenda = _unitOfWork.PedidoVendaRepository.ObterPorIdPedido(pedido.IdPedido);

            if (pedidoVenda == null)
            {
                throw new BusinessException("Não existe pedido venda para chave de acesso informada.");
            }

            if (pedidoVenda.IdTransportadora != idTransportadora)
            {
                throw new BusinessException("Nota fiscal não pertence a transportadora informada.");
            }

            if (pedidoVenda.IdPedidoVendaStatus != PedidoVendaStatusEnum.NFDespachada)
            {
                throw new BusinessException("Atenção, esta nota fiscal ainda não foi despachada.");
            }

            if (pedidoVenda.DataHoraRomaneio.HasValue)
            {
                throw new BusinessException("Atenção, NF pertence a outro romaneio.");
            }
        }

        public void ValidarImpressoraRomaneio(string idUsuario, long idEmpresa)
        {
            var usuarioEmpresa = _unitOfWork.UsuarioEmpresaRepository.Obter(idEmpresa, idUsuario);

            if (!usuarioEmpresa.IdPerfilImpressoraPadrao.HasValue)
            {
                throw new BusinessException("O usuário não possui impressora configurada nessa empresa.");
            }

            var perfilImpressoras = _unitOfWork.PerfilImpressoraItemRepository.ObterPorIdPerfilImpressora(usuarioEmpresa.IdPerfilImpressoraPadrao.Value);

            if (!perfilImpressoras.Any(p => p.IdImpressaoItem == ImpressaoItemEnum.RelatorioA4))
            {
                throw new BusinessException("Usuário não possui impressora configurada para Romaneio.");
            }
        }

        private void ValidarChaveAcessoNF(string chaveAcesso)
        {
            if (string.IsNullOrEmpty(chaveAcesso))
            {
                throw new BusinessException("Favor informar a chave de acesso da NF.");
            }

            if (chaveAcesso.Length != 44)
            {
                throw new BusinessException("Chave de acesso inválida.");
            }

            if (!Regex.IsMatch(chaveAcesso, @"^\d+$"))
            {
                throw new BusinessException("Leitura inválida! Efetue a leitura do código de barras novamente.");
            }
        }

        private Transportadora ValidarERetornarTransportadora(long idTransportadora)
        {
            if (idTransportadora <= 0)
            {
                throw new BusinessException("Informar a transportadora.");
            }

            var transportadora = _unitOfWork.TransportadoraRepository.GetById(idTransportadora);

            if (transportadora == null)
            {
                throw new BusinessException("Transportadora não encontrada.");
            }

            if (!transportadora.Ativo)
            {
                throw new BusinessException("Transportadora não está ativa.");
            }

            return transportadora;
        }

        private Romaneio ValidarNroRomaneio(int nroRomaneio, long idEmpresa)
        {
            if (nroRomaneio <= 0)
            {
                throw new BusinessException("Favor informar o número do romaneio.");
            }

            Romaneio romaneio = _unitOfWork.RomaneioRepository.BuscarPorNumeroRomaneioEEmpresa(nroRomaneio, idEmpresa);

            if (romaneio == null)
            {
                throw new BusinessException("Romaneio não encontrado.");
            }

            return romaneio;
        }

        private Romaneio ValidarIdRomaneio(long idRomaneio, long idEmpresa)
        {
            if (idRomaneio <= 0)
            {
                throw new BusinessException("Favor informar o romaneio.");
            }

            Romaneio romaneio = _unitOfWork.RomaneioRepository.GetById(idRomaneio);

            if (romaneio == null)
            {
                throw new BusinessException("Romaneio não encontrado.");
            }

            if (romaneio.IdEmpresa != idEmpresa)
            {
                throw new BusinessException("Romaneio não pertence a empresa.");
            }

            return romaneio;
        }

        private Printer ValidarIdImpressora(long idImpressora)
        {
            if (idImpressora <= 0)
            {
                throw new BusinessException("Favor informar a impressora.");
            }

            Printer impressora = _unitOfWork.BOPrinterRepository.GetById(idImpressora);

            if (impressora == null)
            {
                throw new BusinessException("Impressora não encontrada.");
            }

            return impressora;
        }

        public RomaneioTransportadoraResposta ValidarRomaneioTransportadora(long idTrasnportadora, long idEmpresa)
        {
            var transportadora = _transportadoraService.ValidarERetornarTransportadora(idTrasnportadora);

            Empresa empresa = _unitOfWork.EmpresaRepository.GetById(idEmpresa);

            string grupoBat = ConfigurationManager.AppSettings["IntegracaoSankhya_Empresa_GrupoBat"];

            if (grupoBat != null && grupoBat.Split(',').Contains(empresa.Sigla))
            {
                if (!empresa.EmpresaConfig.IdDiasDaSemana.HasValue)
                {
                    throw new BusinessException("Não existe configuração de dia coleta para a empresa.");
                }

                if (empresa.EmpresaConfig.IdDiasDaSemana.GetHashCode() != DateTime.Now.DayOfWeek.GetHashCode())
                {
                    throw new BusinessException("Atenção, não haverá coleta para esta cidade.");
                }

                if (empresa.EmpresaConfig.IdTransportadora != transportadora.IdTransportadora)
                {
                    throw new BusinessException("Transportadora não está configurada para coleta.");
                }
            }

            var pedidos = _unitOfWork.PedidoVendaRepository.ObterPorIdTransportadoraRomaneio(transportadora.IdTransportadora, idEmpresa);

            if (pedidos.NullOrEmpty())
            {
                throw new BusinessException("Atenção, não há nenhuma nota despachada para esta transportadora.");
            }

            var resposta = new RomaneioTransportadoraResposta()
            {
                IdTransportadora = transportadora.IdTransportadora,
                ChavesAcessos = pedidos.Select(x => x.Pedido.ChaveAcessoNotaFiscal).ToList()
            };

            return resposta;
        }

        public RomaneioResposta BuscarRomaneio(int nroRomaneio, long idEmpresa)
        {
            var romaneio = ValidarNroRomaneio(nroRomaneio, idEmpresa);

            var resposta = new RomaneioResposta()
            {
                IdRomaneio = romaneio.IdRomaneio,
            };

            return resposta;
        }

        public async Task<RomaneioFinalizarNFResposta> FinalizarRomaneioNF(long idTransportadora, List<string> chavesAcessos, string idUsuario, long idEmpresa)
        {
            long idRomaneioResultado = default;

            if (!chavesAcessos.Any())
            {
                throw new BusinessException("Favor informar as chaves de acesso.");
            }

            _transportadoraService.ValidarERetornarTransportadora(idTransportadora);
            // ações
            using (var transacao = _unitOfWork.CreateTransactionScope())
            {
                var dataHoraEmissaoRomaneio = DateTime.Now;

                // Romaneio
                var romaneio = new Romaneio();
                romaneio.IdTransportadora = idTransportadora;
                romaneio.IdEmpresa = idEmpresa;
                romaneio.NroRomaneio = GetNextNroRomaneioByEmpresa(idEmpresa);
                romaneio.DataHoraCriacao = dataHoraEmissaoRomaneio;

                _unitOfWork.RomaneioRepository.Add(romaneio);
                _unitOfWork.SaveChanges();

                await _romaneioService.InserirRomaneioSankhya(romaneio.NroRomaneio, dataHoraEmissaoRomaneio);

                foreach (var chaveAcesso in chavesAcessos)
                {
                    // validações
                    ValidarChaveAcessoNF(chaveAcesso);

                    Pedido pedido = _unitOfWork.PedidoRepository.PesquisaPorChaveAcesso(chaveAcesso);

                    if (pedido.IdTransportadora != idTransportadora)
                    {
                        throw new BusinessException("Nota fiscal não pertence a transportadora informada.");
                    }

                    if (!pedido.NumeroNotaFiscal.HasValue)
                    {
                        throw new BusinessException("Não foi encontrado número da nota fiscal informada.");
                    }

                    if (pedido.SerieNotaFiscal.NullOrEmpty())
                    {
                        throw new BusinessException("Não foi encontrado número de série da nota fiscal informada.");
                    }

                    PedidoVenda pedidoVenda = _unitOfWork.PedidoVendaRepository.ObterPorIdPedido(pedido.IdPedido);

                    if (pedidoVenda == null)
                    {
                        throw new BusinessException("Não existe pedido venda para chave de acesso informada.");
                    }

                    var volumes = pedidoVenda.PedidoVendaVolumes.Where(w => w.IdPedidoVendaStatus != PedidoVendaStatusEnum.VolumeExcluido && w.IdPedidoVendaStatus != PedidoVendaStatusEnum.ProdutoZerado).ToList();

                    // Notas do Romaneio
                    var romaneioNotaFiscal = new RomaneioNotaFiscal();
                    romaneioNotaFiscal.IdRomaneio = romaneio.IdRomaneio;
                    romaneioNotaFiscal.IdPedidoVenda = pedidoVenda.IdPedidoVenda;
                    romaneioNotaFiscal.NroNotaFiscal = pedido.NumeroNotaFiscal.Value;
                    romaneioNotaFiscal.SerieNotaFiscal = pedido.SerieNotaFiscal;
                    romaneioNotaFiscal.NroVolumes = pedidoVenda.NroVolumes;
                    romaneioNotaFiscal.IdCliente = pedidoVenda.IdCliente;

                    // tratamento especial pra empresas k1 e k2
                    var empresa = _unitOfWork.EmpresaRepository.GetById(idEmpresa);
                    if (empresa.Sigla == "K1" || empresa.Sigla == "K2")
                    {
                        var somaDosVolumes = volumes.Aggregate(default(decimal), (x, y) => x + y.PesoVolume);
                        romaneioNotaFiscal.TotalPesoLiquidoVolumes = somaDosVolumes;
                        romaneioNotaFiscal.TotalPesoBrutoVolumes = somaDosVolumes;
                    }

                    _unitOfWork.RomaneioNotaFiscalRepository.Add(romaneioNotaFiscal);
                    _unitOfWork.SaveChanges();

                    pedidoVenda.IdUsuarioRomaneio = idUsuario;
                    pedidoVenda.DataHoraRomaneio = dataHoraEmissaoRomaneio;

                    // atualiza status no Sankhya e nas entidades do pedido
                    await _pedidoService.AtualizarStatusPedido(pedidoVenda.Pedido, PedidoVendaStatusEnum.RomaneioImpresso);

                    //Atualizando o numero do Romaneio no Sankhya da Nota Fiscal de Venda atraves do pedido
                    await _romaneioService.AtualizarRomaneioNotaFiscal(pedidoVenda.Pedido, romaneio.NroRomaneio);

                    pedido.IdPedidoVendaStatus = PedidoVendaStatusEnum.RomaneioImpresso;
                    _unitOfWork.PedidoRepository.Update(pedido);

                    pedidoVenda.IdPedidoVendaStatus = PedidoVendaStatusEnum.RomaneioImpresso;
                    _unitOfWork.PedidoVendaRepository.Update(pedidoVenda);

                    foreach (var pedidoVendaVolume in volumes)
                    {
                        pedidoVendaVolume.IdPedidoVendaStatus = PedidoVendaStatusEnum.RomaneioImpresso;
                        _unitOfWork.PedidoVendaVolumeRepository.Update(pedidoVendaVolume);
                    }

                    _unitOfWork.SaveChanges();

                    _coletorHistoricoService.GravarHistoricoColetor(new GravarHistoricoColetorRequisicao
                    {
                        IdColetorAplicacao = ColetorAplicacaoEnum.Expedicao,
                        IdColetorHistoricoTipo = ColetorHistoricoTipoEnum.Romaneio,
                        Descricao = $"Romaneio {romaneio.NroRomaneio} gerado para a transportadora {pedido.Transportadora.NomeFantasia}.",
                        IdEmpresa = idEmpresa,
                        IdUsuario = idUsuario
                    });

                    _unitOfWork.SaveChanges();
                }

                transacao.Complete();
                idRomaneioResultado = romaneio.IdRomaneio;
            }

            var resposta = new RomaneioFinalizarNFResposta
            {
                IdRomaneio = idRomaneioResultado
            };

            return resposta;
        }

        private int GetNextNroRomaneioByEmpresa(long idEmpresa)
        {
            int? ultimoRomaneio = _unitOfWork.RomaneioRepository.BuscaUltimoNroRomaneioPorEmpresa(idEmpresa);

            if (ultimoRomaneio.HasValue)
            {
                ultimoRomaneio++;
                return ultimoRomaneio.Value;
            }
            else
            {
                return 1;
            }
        }

        public void ReimprimirRomaneio(long idRomaneio, long idImpressora, bool imprimirSegundaVia, long idEmpresa, string idUsuario)
        {
            var romaneio = ValidarIdRomaneio(idRomaneio, idEmpresa);

            var impressora = ValidarIdImpressora(idImpressora);

            ImprimirRomaneio(romaneio.IdRomaneio, impressora.Id, imprimirSegundaVia, idEmpresa, idUsuario);

            _coletorHistoricoService.GravarHistoricoColetor(new GravarHistoricoColetorRequisicao
            {
                IdColetorAplicacao = ColetorAplicacaoEnum.Expedicao,
                IdColetorHistoricoTipo = ColetorHistoricoTipoEnum.ReimpressaoRomaneio,
                Descricao = $"Romaneio {romaneio.NroRomaneio} da transportadora {romaneio.Transportadora.NomeFantasia} reimpresso.",
                IdEmpresa = idEmpresa,
                IdUsuario = idUsuario
            });
        }

        public EnderecosPorTransportadoraResposta BuscaEnderecosPorTransportadora(long idTransportadora, long idEmpresa)
        {
            var transportadora = ValidarERetornarTransportadora(idTransportadora);

            var volumesInstalados = _unitOfWork.PedidoVendaVolumeRepository.ObterVolumesInstaladosPorTransportadoraEmpresa(transportadora.IdTransportadora, idEmpresa);

            if (volumesInstalados.NullOrEmpty())
            {
                throw new BusinessException("VAGO.");
            }

            return new EnderecosPorTransportadoraResposta()
            {
                IdTransportadora = transportadora.IdTransportadora,
                NomeTransportadora = transportadora.NomeFantasia,
                ListaEnderecos = volumesInstalados.Select(enderecoInstalado => new EnderecosPorTransportadoraVolumeResposta
                {
                    IdPedidoVendaVolume = enderecoInstalado.IdPedidoVendaVolume,
                    CodigoEndereco = enderecoInstalado.EnderecoTransportadora.Codigo
                }).ToList()
            };

        }

        public void ValidarRemoverDocaTransportadora(string idOuCodtransportadora, long idEmpresa)
        {
            Transportadora transportadora;

            if (long.TryParse(idOuCodtransportadora, out long idTransportadora))
            {
                transportadora = _unitOfWork.TransportadoraRepository.GetById(idTransportadora);
            }
            else
            {
                transportadora = _unitOfWork.TransportadoraRepository.ConsultarPorCodigoTransportadora(idOuCodtransportadora);
            }

            if (transportadora == null)
            {
                throw new BusinessException("Transportadora não encontrada.");
            }

            if (!transportadora.Ativo)
            {
                throw new BusinessException("Transportadora não está ativa.");
            }

            var temPedidoVendaMovidoDoca = _unitOfWork.PedidoVendaRepository.TemPedidoVendaMovidoDoca(transportadora.IdTransportadora, idEmpresa);

            if (!temPedidoVendaMovidoDoca)
            {
                throw new BusinessException("Não há volumes na DOCA.");
            }
        }

        public void RemoverVolumeDoca(string referenciaPedido, string idUsuario, long idEmpresa)
        {
            BuscaEValidaDadosPorReferenciaPedido(referenciaPedido, out string numeroPedido, out long idTransportadora, out int numeroVolume);

            var pedidoVenda = _unitOfWork.PedidoVendaRepository.ObterPorNroPedidoEEmpresa(numeroPedido, idEmpresa);

            if (pedidoVenda == null)
            {
                throw new BusinessException("Pedido não encontrado.");
            }

            var pedidoVendaVolume = pedidoVenda.PedidoVendaVolumes.FirstOrDefault(volume => volume.NroVolume == numeroVolume);

            if (pedidoVendaVolume == null)
            {
                throw new BusinessException("Volume fornecido não encontrado.");
            }

            var transportadora = _transportadoraService.ValidarERetornarTransportadora(idTransportadora);

            if (pedidoVendaVolume.PedidoVenda.IdTransportadora != transportadora.IdTransportadora)
            {
                throw new BusinessException("Este volume não pertence a esta transportadora.");
            }

            if (pedidoVendaVolume.IdPedidoVendaStatus == PedidoVendaStatusEnum.RomaneioImpresso)
            {
                throw new BusinessException("Volume não pode ser removido, o Romaneio está impresso.");
            }

            if (pedidoVendaVolume.IdPedidoVendaStatus != PedidoVendaStatusEnum.MovidoDOCA)
            {
                throw new BusinessException("Volume não está na DOCA.");
            }

            using (var transacao = _unitOfWork.CreateTransactionScope())
            {
                pedidoVendaVolume.IdPedidoVendaStatus = PedidoVendaStatusEnum.RemovidoDOCA;
                pedidoVendaVolume.DataHoraRemocaoVolume = DateTime.Now;
                pedidoVendaVolume.IdUsuarioInstalacaoDOCA = null;
                pedidoVendaVolume.IdUsuarioInstalTransportadora = null;
                pedidoVendaVolume.DataHoraInstalTransportadora = null;
                pedidoVendaVolume.IdEnderecoArmazTransportadora = null;
                pedidoVendaVolume.DataHoraInstalacaoDOCA = null;

                _unitOfWork.SaveChanges();

                if (pedidoVenda.PedidoVendaVolumes.Where(w => w.IdPedidoVendaStatus != PedidoVendaStatusEnum.VolumeExcluido).All(x => x.IdPedidoVendaStatus == PedidoVendaStatusEnum.RemovidoDOCA))
                {
                    pedidoVenda.IdPedidoVendaStatus = PedidoVendaStatusEnum.SeparacaoConcluidaComSucesso;
                    _unitOfWork.SaveChanges();
                }
                else if (pedidoVenda.IdPedidoVendaStatus != PedidoVendaStatusEnum.MovendoDOCA)
                {
                    pedidoVenda.IdPedidoVendaStatus = PedidoVendaStatusEnum.MovendoDOCA;
                    _unitOfWork.SaveChanges();
                }

                _coletorHistoricoService.GravarHistoricoColetor(new GravarHistoricoColetorRequisicao
                {
                    IdColetorAplicacao = ColetorAplicacaoEnum.Expedicao,
                    IdColetorHistoricoTipo = ColetorHistoricoTipoEnum.RemoverDoca,
                    Descricao = $"O pedido-volume {pedidoVendaVolume.IdPedidoVendaVolume} da transportadora {transportadora.RazaoSocial} foi removido da DOCA.",
                    IdEmpresa = idEmpresa,
                    IdUsuario = idUsuario
                });

                transacao.Complete();
            }

        }

        public List<RelatorioVolumesInstaladosTransportadoraItem> BuscarDadosVolumePorTransportadora(DataTableFilter<RelatorioVolumesInstaladosTransportadoraFiltro> filtro, out int totalRecordsFiltered, out int totalRecords)
        {
            return _unitOfWork.PedidoVendaVolumeRepository.BuscarDadosVolumePorTransportadora(filtro, out totalRecordsFiltered, out totalRecords);
        }

        public List<PedidoVendaItem> BuscarDadosPedidoVendaParaTabela(DataTableFilter<PedidoVendaFiltro> filtro, out int registrosFiltrados, out int totalRegistros)
        {
            return _unitOfWork.PedidoVendaRepository.BuscarDadosPedidoVendaParaTabela(filtro, out registrosFiltrados, out totalRegistros);
        }

        public List<MovimentacaoVolumesModel> BuscarDadosMovimentacaoVolumes(DateTime dataInicial, DateTime dataFinal, bool? cartaoCredito, bool? cartaoDebito, bool? dinheiro, bool? requisicao, bool? reposicao, long idEmpresa)
        {
            var dados = _unitOfWork.PedidoVendaVolumeRepository.BuscarDadosVolumeGrupoArmazenagem(dataInicial, dataFinal, cartaoCredito, cartaoDebito, dinheiro, requisicao, reposicao, idEmpresa);

            var dadosAgrupados = dados.GroupBy(g => new
            {
                g.IdGrupoCorredorArmazenagem,
                g.PontoArmazenagemDescricao,
                g.CorredorInicial,
                g.CorredorFinal
            }).OrderBy(o => o.Key.CorredorInicial).ThenBy(o => o.Key.CorredorFinal).ToList();

            var dadosRetorno = new List<MovimentacaoVolumesModel>();

            dadosAgrupados.ForEach(g => dadosRetorno.Add(new MovimentacaoVolumesModel
            {
                Corredores = $"{g.Key.CorredorInicial.ToString().PadLeft(2, '0')} à {g.Key.CorredorFinal.ToString().PadLeft(2, '0')}",
                IdGrupoCorredorArmazenagem = g.Key.IdGrupoCorredorArmazenagem,
                PontoArmazenagemDescricao = g.Key.PontoArmazenagemDescricao,
                EnviadoSeparacao = g.Count(v => v.IdPedidoVendaStatus == PedidoVendaStatusEnum.EnviadoSeparacao),
                EmSeparacao = g.Count(v => v.IdPedidoVendaStatus == PedidoVendaStatusEnum.ProcessandoSeparacao),
                FinalizadoSeparacao = g.Count(v => v.IdPedidoVendaStatus == PedidoVendaStatusEnum.SeparacaoConcluidaComSucesso),
                InstaladoTransportadora = g.Count(v => v.IdPedidoVendaStatus == PedidoVendaStatusEnum.VolumeInstaladoTransportadora || v.IdPedidoVendaStatus == PedidoVendaStatusEnum.MovendoDOCA),
                DOC = g.Count(v => v.NumeroNotaFiscal.HasValue && (v.IdPedidoVendaStatus == PedidoVendaStatusEnum.SeparacaoConcluidaComSucesso || v.IdPedidoVendaStatus == PedidoVendaStatusEnum.InstalandoVolumeTransportadora || v.IdPedidoVendaStatus == PedidoVendaStatusEnum.VolumeInstaladoTransportadora || v.IdPedidoVendaStatus == PedidoVendaStatusEnum.MovendoDOCA || v.IdPedidoVendaStatus == PedidoVendaStatusEnum.MovidoDOCA || v.IdPedidoVendaStatus == PedidoVendaStatusEnum.DespachandoNF || v.IdPedidoVendaStatus == PedidoVendaStatusEnum.NFDespachada)),
                EnviadoTransportadora = g.Count(v => v.IdPedidoVendaStatus == PedidoVendaStatusEnum.RomaneioImpresso),
                VolumeExcluido = g.Count(v => v.IdPedidoVendaStatus == PedidoVendaStatusEnum.VolumeExcluido),
            }));

            dadosRetorno.ForEach(dr => dr.Total = dr.EnviadoSeparacao + dr.EmSeparacao + dr.FinalizadoSeparacao + dr.InstaladoTransportadora + dr.DOC + dr.EnviadoTransportadora + dr.VolumeExcluido);

            return dadosRetorno;
        }

        public async Task<MovimentacaoVolumesIntegracoesModel> BuscarDadosMovimentacaoVolumesIntegracoes(long idEmpresa)
        {
            var dadosRetorno = new MovimentacaoVolumesIntegracoesModel();

            dadosRetorno.AguardandoIntegracao = (await _pedidoService.ConsultarPedidosPendentesSankhya(idEmpresa)).GetValueOrDefault();

            dadosRetorno.AguardandoRobo = _unitOfWork.PedidoRepository.PesquisarTotalPendenteSeparacao(idEmpresa);

            return dadosRetorno;
        }

        public List<RelatorioPedidosExpedidosLinhaTabela> BuscarDadosPedidosExpedidos(DataTableFilter<RelatorioPedidosExpedidosFilter> filtro, out int totalRecordsFiltered, out int totalRecords)
        {
            return _unitOfWork.PedidoVendaVolumeRepository.BuscarDadosPedidosExpedidos(filtro, out totalRecordsFiltered, out totalRecords);
        }

        public List<MovimentacaoVolumesDetalhesModel> BuscarDadosVolumes(DateTime dataInicial, DateTime dataFinal, long? idGrupoCorredorArmazenagem, string tipo, bool? cartaoCredito, bool? cartaoDebito, bool? dinheiro, bool? requisicao, bool? reposicao, long idEmpresa, out string statusDescricao, out string corredorArmazenagemDescricao)
        {
            var listaStatus = new List<PedidoVendaStatusEnum>();
            var nfFaturada = false;

            switch (tipo)
            {
                case "EnviadoSeparacao":
                    listaStatus.Add(PedidoVendaStatusEnum.EnviadoSeparacao);
                    statusDescricao = "Enviado Separação";
                    break;
                case "EmSeparacao":
                    listaStatus.Add(PedidoVendaStatusEnum.ProcessandoSeparacao);
                    statusDescricao = "Em Separação";
                    break;
                case "FinalizadoSeparacao":
                    listaStatus.Add(PedidoVendaStatusEnum.SeparacaoConcluidaComSucesso);
                    statusDescricao = "Finalizado Separação";
                    break;
                case "InstaladoTransportadora":
                    listaStatus.Add(PedidoVendaStatusEnum.VolumeInstaladoTransportadora);
                    listaStatus.Add(PedidoVendaStatusEnum.MovendoDOCA);
                    statusDescricao = "Instalado Transportadora	";
                    break;
                case "DOC":
                    nfFaturada = true;
                    listaStatus.Add(PedidoVendaStatusEnum.SeparacaoConcluidaComSucesso);
                    listaStatus.Add(PedidoVendaStatusEnum.InstalandoVolumeTransportadora);
                    listaStatus.Add(PedidoVendaStatusEnum.VolumeInstaladoTransportadora);
                    listaStatus.Add(PedidoVendaStatusEnum.MovendoDOCA);
                    listaStatus.Add(PedidoVendaStatusEnum.MovidoDOCA);
                    listaStatus.Add(PedidoVendaStatusEnum.DespachandoNF);
                    listaStatus.Add(PedidoVendaStatusEnum.NFDespachada);
                    statusDescricao = "DOC";
                    break;
                case "EnviadoTransportadora":
                    listaStatus.Add(PedidoVendaStatusEnum.RomaneioImpresso);
                    statusDescricao = "Enviado Transportadora";
                    break;
                case "VolumeExcluido":
                    listaStatus.Add(PedidoVendaStatusEnum.VolumeExcluido);
                    statusDescricao = "Volume Excluído";
                    break;
                default:
                    statusDescricao = "Todos";
                    break;
            }

            corredorArmazenagemDescricao = null;

            if (idGrupoCorredorArmazenagem.HasValue)
            {
                var grupoCorredorArmazenagem = _unitOfWork.GrupoCorredorArmazenagemRepository.GetById(idGrupoCorredorArmazenagem.Value);

                if (grupoCorredorArmazenagem != null)
                {
                    corredorArmazenagemDescricao = $"{grupoCorredorArmazenagem.PontoArmazenagem.Descricao}: {grupoCorredorArmazenagem.CorredorInicial.ToString().PadLeft(2, '0')} à {grupoCorredorArmazenagem.CorredorFinal.ToString().PadLeft(2, '0')}";
                }
            }

            var responseList = _unitOfWork.PedidoVendaVolumeRepository.BuscarDadosVolumes(dataInicial, dataFinal, idGrupoCorredorArmazenagem, listaStatus, cartaoCredito, cartaoDebito, dinheiro, requisicao, reposicao, nfFaturada, idEmpresa);

            responseList.ForEach(dados =>
            {
                dados.NumeroRomaneio = _unitOfWork.RomaneioNotaFiscalRepository.BuscarPorPedidoVenda(dados.IdPedidoVenda).FirstOrDefault()?.Romaneio?.NroRomaneio;

                if (!dados.UsuarioDespachoNotaFiscal.NullOrEmpty())
                {
                    dados.UsuarioDespachoNotaFiscal = _unitOfWork.PerfilUsuarioRepository.GetByUserId(dados.UsuarioDespachoNotaFiscal)?.Nome;
                }

                if (!dados.UsuarioRomaneio.NullOrEmpty())
                {
                    dados.UsuarioRomaneio = _unitOfWork.PerfilUsuarioRepository.GetByUserId(dados.UsuarioRomaneio)?.Nome;
                }
            });

            return responseList;
        }

        public List<RelatorioPedidosLinhaTabela> BuscarDadosPedidos(DataTableFilter<RelatorioPedidosFiltro> filtro, out int registrosFiltrados, out int totalRegistros)
        {
            var pedidos = _unitOfWork.PedidoVendaRepository.BuscarPedidosVolumePorEmpresa(filtro.CustomFilter.IdEmpresa);

            totalRegistros = pedidos.Count();

            if (filtro.CustomFilter.IdTransportadora.HasValue)
            {
                pedidos = pedidos.Where(pv => pv.PedidoVenda.IdTransportadora == filtro.CustomFilter.IdTransportadora.Value);
            }

            if (filtro.CustomFilter.DataInicial.HasValue)
            {
                var dataInicial = new DateTime(filtro.CustomFilter.DataInicial.Value.Year, filtro.CustomFilter.DataInicial.Value.Month, filtro.CustomFilter.DataInicial.Value.Day);

                pedidos = pedidos.Where(x => x.PedidoVenda.DataProcessamento >= dataInicial);
            }

            if (filtro.CustomFilter.DataFinal.HasValue)
            {
                var dataFinal = new DateTime(filtro.CustomFilter.DataFinal.Value.Year, filtro.CustomFilter.DataFinal.Value.Month, filtro.CustomFilter.DataFinal.Value.Day, 23, 59, 59);

                pedidos = pedidos.Where(x => x.PedidoVenda.DataProcessamento <= dataFinal);
            }

            if (!filtro.CustomFilter.NumeroPedido.NullOrEmpty())
            {
                pedidos = pedidos.Where(pv => pv.PedidoVenda.Pedido.NumeroPedido.Contains(filtro.CustomFilter.NumeroPedido));
            }

            if (filtro.CustomFilter.IdCliente.HasValue)
            {
                pedidos = pedidos.Where(pv => pv.PedidoVenda.IdCliente == filtro.CustomFilter.IdCliente.Value);
            }

            if (filtro.CustomFilter.IdStatus.HasValue)
            {
                pedidos = pedidos.Where(pv => (int)pv.IdPedidoVendaStatus == filtro.CustomFilter.IdStatus.Value);
            }

            if (filtro.CustomFilter.IdProduto.HasValue)
            {
                pedidos = pedidos.Where(pv => pv.PedidoVenda.PedidoVendaProdutos.Any(x => x.IdProduto == filtro.CustomFilter.IdProduto.Value));
            }

            var query = pedidos.Select(s => new
            {
                NroPedido = s.PedidoVenda.Pedido.NumeroPedido,
                NroVolume = s.NroVolume,
                NroCentena = s.NroCentena,
                IdPedidoVendaVolume = s.IdPedidoVendaVolume,
                DataCriacao = s.PedidoVenda.Pedido.DataCriacao,
                DataIntegracao = s.PedidoVenda.Pedido.DataIntegracao,
                DataImpressao = s.PedidoVenda.DataProcessamento,
                PedidoNumeroNotaFiscal = s.PedidoVenda.Pedido.NumeroNotaFiscal,
                PedidoSerieNotaFiscal = s.PedidoVenda.Pedido.SerieNotaFiscal,
                DataSaida = s.PedidoVenda.DataHoraRomaneio,
                Status = s.PedidoVendaStatus.Descricao,
                StatusPedido = s.PedidoVenda.PedidoVendaStatus.Descricao,
                NomeCliente = s.PedidoVenda.Cliente.RazaoSocial,
                IdStatusPedido = (int)s.PedidoVenda.IdPedidoVendaStatus,
                IdStatusVolume = (int)s.IdPedidoVendaStatus,
                s.IdUsuarioSeparacaoAndamento
            });

            registrosFiltrados = query.Count();

            query = query.OrderBy(o => o.NroCentena).ThenBy(o => o.NroPedido).ThenBy(o => o.NroVolume).Skip(filtro.Start).Take(filtro.Length);

            List<RelatorioPedidosLinhaTabela> resultado = new List<RelatorioPedidosLinhaTabela>();

            query.ToList().ForEach(s =>
            {
                resultado.Add(new RelatorioPedidosLinhaTabela
                {
                    NroPedido = $"Pedido: {s.NroPedido} - Cliente: {s.NomeCliente}",
                    NroVolume = s.NroVolume.ToString().PadLeft(3, '0'),
                    NroCentena = s.NroCentena.ToString().PadLeft(4, '0'),
                    IdPedidoVendaVolume = s.IdPedidoVendaVolume,
                    DataCriacao = s.DataCriacao.ToString("dd/MM/yyyy"),
                    DataIntegracao = s.DataIntegracao.ToString("dd/MM/yyyy HH:mm"),
                    DataImpressao = s.DataImpressao.ToString("dd/MM/yyyy HH:mm"),
                    NumeroSerieNotaFiscal = s.PedidoNumeroNotaFiscal.HasValue ? $"{s.PedidoNumeroNotaFiscal}/{s.PedidoSerieNotaFiscal}" : "Aguardando Faturamento",
                    DataExpedicao = s.DataSaida.HasValue ? s.DataSaida.Value.ToString("dd/MM/yyyy HH:mm") : string.Empty,
                    StatusVolume = s.Status,
                    StatusPedido = s.StatusPedido,
                    PermitirEditarVolume =
                    (s.IdStatusPedido == PedidoVendaStatusEnum.EnviadoSeparacao.GetHashCode() ||
                     s.IdStatusPedido == PedidoVendaStatusEnum.ProcessandoSeparacao.GetHashCode()) &&
                    (s.IdStatusVolume == PedidoVendaStatusEnum.EnviadoSeparacao.GetHashCode() ||
                     s.IdStatusVolume == PedidoVendaStatusEnum.ProcessandoSeparacao.GetHashCode() ||
                     s.IdStatusVolume == PedidoVendaStatusEnum.SeparacaoConcluidaComSucesso.GetHashCode()),
                    PodeRemoverUsuarioSeparacao = s.IdStatusPedido == PedidoVendaStatusEnum.ProcessandoSeparacao.GetHashCode() && !s.IdUsuarioSeparacaoAndamento.NullOrEmpty()
                });
            });

            return resultado;
        }

        public DetalhesPedidoVolume BuscarDadosPedidoVolume(long idPedidoVendaVolume, long idEmpresa)
        {
            var pedidoVendaVolume = _unitOfWork.PedidoVendaVolumeRepository.GetById(idPedidoVendaVolume);

            if (pedidoVendaVolume == null || pedidoVendaVolume.PedidoVenda?.IdEmpresa != idEmpresa)
            {
                return null;
            }

            var retorno = new DetalhesPedidoVolume();

            var pedido = pedidoVendaVolume.PedidoVenda.Pedido;

            retorno.PedidoNro = pedido.NumeroPedido;

            retorno.PedidoCliente = pedido.Cliente?.NomeFantasia;

            retorno.PedidoTransportadora = pedido.Transportadora?.NomeFantasia;

            retorno.PedidoCodigoIntegracao = pedido.CodigoIntegracao;

            retorno.PedidoRepresentante = pedido.Representante.Nome;

            retorno.PedidoDataCriacao = pedido.DataCriacao;

            retorno.PedidoDataIntegracao = pedido.DataIntegracao;

            retorno.PedidoDataImpressao = pedidoVendaVolume.PedidoVenda.DataProcessamento;

            retorno.PedidoIsRequisicao = pedido.IsRequisicao;

            retorno.PedidoCodigoIntegracaoNotaFiscal = pedido.CodigoIntegracaoNotaFiscal;

            retorno.PedidoVendaStatus = pedido.PedidoVendaStatus.Descricao;

            retorno.PedidoChaveAcessoNotaFiscal = pedido.ChaveAcessoNotaFiscal;

            retorno.PedidoPagamentoDescricaoIntegracao = pedido.PagamentoDescricaoIntegracao;

            retorno.PedidoNumeroNotaFiscal = pedido.NumeroNotaFiscal;

            retorno.PedidoSerieNotaFiscal = pedido.SerieNotaFiscal;

            retorno.VolumeNroVolume = pedidoVendaVolume.NroVolume;

            retorno.VolumeNroCentena = pedidoVendaVolume.NroCentena;

            retorno.VolumeCaixaCubagem = pedidoVendaVolume.CaixaCubagem?.Nome;

            retorno.VolumeCubagem = pedidoVendaVolume.CubagemVolume;

            retorno.VolumePeso = pedidoVendaVolume.PesoVolume;

            retorno.VolumeCorredorInicio = pedidoVendaVolume.CorredorInicio;

            retorno.VolumeCorredorFim = pedidoVendaVolume.CorredorFim;

            retorno.VolumeCaixaVolume = pedidoVendaVolume.CaixaVolume?.Nome;

            retorno.VolumePedidoVendaStatus = pedidoVendaVolume.PedidoVendaStatus.Descricao;

            if (!pedidoVendaVolume.IdUsuarioInstalTransportadora.NullOrEmpty())
            {
                retorno.VolumeUsuarioInstalTransportadora = _unitOfWork.PerfilUsuarioRepository.GetByUserId(pedidoVendaVolume.IdUsuarioInstalTransportadora)?.Nome;
            }

            retorno.VolumeDataHoraInstalTransportadora = pedidoVendaVolume.DataHoraInstalTransportadora;

            retorno.VolumeEnderecoArmazTransportadora = pedidoVendaVolume.EnderecoTransportadora?.Codigo;

            if (!pedidoVendaVolume.IdUsuarioInstalacaoDOCA.NullOrEmpty())
            {
                retorno.VolumeUsuarioInstalacaoDOCA = _unitOfWork.PerfilUsuarioRepository.GetByUserId(pedidoVendaVolume.IdUsuarioInstalacaoDOCA)?.Nome;
            }

            retorno.VolumeDataHoraInstalacaoDOCA = pedidoVendaVolume.DataHoraInstalacaoDOCA;

            retorno.VolumeDataHoraRemocaoVolume = pedidoVendaVolume.DataHoraRemocaoVolume;

            retorno.NroRomaneio = _unitOfWork.RomaneioNotaFiscalRepository.BuscarPorPedidoVenda(pedidoVendaVolume.IdPedidoVenda).FirstOrDefault()?.Romaneio?.NroRomaneio;

            retorno.ListaProdutos = pedidoVendaVolume.PedidoVendaProdutos.Select(pvp => new DetalhesPedidoProdutoVolume
            {
                ProdutoReferencia = pvp.Produto.Referencia,
                CodigoEnderecoPicking = _unitOfWork.ProdutoEstoqueRepository.ConsultarPorProduto(pvp.IdProduto, idEmpresa)?.EnderecoArmazenagem?.Codigo,
                QuantidadeSeparar = pvp.QtdSeparar,
                QuantidadeSeparada = pvp.QtdSeparada.GetValueOrDefault(),
                DataHoraInicioSeparacao = pvp.DataHoraInicioSeparacao,
                DataHoraFimSeparacao = pvp.DataHoraFimSeparacao,
                UsuarioSeparacao = !pvp.IdUsuarioSeparacao.NullOrEmpty() ? $"{_unitOfWork.PerfilUsuarioRepository.GetByUserId(pvp.IdUsuarioSeparacao)?.Nome}" : null
            }).ToList();

            return retorno;
        }

        public string GerarReferenciaPedidoVolume(PedidoVendaVolume volume)
        {
            return $"{volume.PedidoVenda.Pedido.NumeroPedido}{volume.PedidoVenda.Transportadora.IdTransportadora.ToString().PadLeft(3, '0')}{volume.NroVolume.ToString().PadLeft(3, '0')}";
        }

        public async Task<bool> GerenciarVolumes(string nroPedido, long? idPedidoVendaVolume, List<GerenciarVolumeItem> listaVolumesProdutos, long idEmpresa, long idGrupoCorredorArmazenagem, string idUsuario)
        {
            if (listaVolumesProdutos.NullOrEmpty())
            {
                throw new BusinessException("Nenhum volume adicionado.");
            }

            List<PedidoVendaProduto> listaVolumeProdutosOrigem = new List<PedidoVendaProduto>();
            List<PedidoVendaProduto> listaVolumeProdutosAlterar = new List<PedidoVendaProduto>();

            decimal pesoTotal = 0;
            bool excedeuPeso = false;

            PedidoVenda pedidoVenda = GerenciarVolumesValidacaoPedido(nroPedido, idEmpresa);

            var grupoCorredorArmazenagem = _unitOfWork.GrupoCorredorArmazenagemRepository.GetById(idGrupoCorredorArmazenagem);
            if (grupoCorredorArmazenagem == null)
            {
                throw new BusinessException("Não foi encontrado o grupo de corredores.");
            }

            PedidoVendaVolume pedidoVendaVolume = await GerenciamentoVolumesGerarVolume(idPedidoVendaVolume, idEmpresa, idGrupoCorredorArmazenagem, pedidoVenda, grupoCorredorArmazenagem);

            var listaVolumesProdutosAgrupados = listaVolumesProdutos.GroupBy(g => g.IdProduto).ToDictionary(d => d.Key, d => d.ToList());

            foreach (var volumeProdutoAgrupado in listaVolumesProdutosAgrupados)
            {
                PedidoVendaProduto pedidoVendaProdutoOrigem = null;

                var produto = _unitOfWork.ProdutoRepository.GetById(volumeProdutoAgrupado.Key);

                foreach (var volumeProduto in volumeProdutoAgrupado.Value)
                {
                    pedidoVendaProdutoOrigem = _unitOfWork.PedidoVendaProdutoRepository.ObterPorIdPedidoVendaVolumeEIdProduto(volumeProduto.IdPedidoVendaVolumeOrigem, volumeProdutoAgrupado.Key);

                    if (produto == null)
                    {
                        throw new BusinessException("Um dos produtos da lista não foi encontrado;");
                    }

                    if (!(volumeProduto.Quantidade > 0))
                    {
                        throw new BusinessException($"A quantidade digitada do produto {produto.Referencia} deve ser maior que zero.");
                    }

                    if (volumeProduto.Quantidade < produto.MultiploVenda || (volumeProduto.Quantidade % produto.MultiploVenda) != 0)
                    {
                        throw new BusinessException($"O produto {produto.Referencia} está fora do múltiplo.");
                    }

                    GerneciamentoVolumesValidacaoVolume(idGrupoCorredorArmazenagem, pedidoVenda, grupoCorredorArmazenagem, volumeProduto, produto, pedidoVendaProdutoOrigem);

                    pedidoVendaProdutoOrigem.QtdSeparar -= volumeProduto.Quantidade;

                    listaVolumeProdutosOrigem.Add(pedidoVendaProdutoOrigem);
                }

                var qtdTransferencia = volumeProdutoAgrupado.Value.Sum(s => s.Quantidade);
                var pesoProduto = produto.PesoBruto * qtdTransferencia;

                PedidoVendaProduto pedidoVendaProduto = pedidoVendaVolume.PedidoVendaProdutos.Where(w => w.IdProduto == volumeProdutoAgrupado.Key).FirstOrDefault();

                if (pedidoVendaProduto != null)
                {
                    pesoTotal += pedidoVendaProduto.PesoProduto + produto.PesoBruto;

                    pedidoVendaProduto.PesoProduto = pedidoVendaProduto.PesoProduto + produto.PesoBruto;
                    pedidoVendaProduto.QtdSeparar += qtdTransferencia;
                }
                else
                {
                    var largura = produto.Largura.HasValue ? produto.Largura.Value : 0;
                    var comprimento = produto.Comprimento.HasValue ? produto.Comprimento.Value : 0;
                    var altura = produto.Altura.HasValue ? produto.Altura.Value : 0;

                    pedidoVendaProduto = new PedidoVendaProduto()
                    {
                        IdPedidoVenda = pedidoVenda.IdPedidoVenda,
                        IdProduto = volumeProdutoAgrupado.Key,
                        IdEnderecoArmazenagem = pedidoVendaProdutoOrigem.IdEnderecoArmazenagem,
                        IdPedidoVendaStatus = PedidoVendaStatusEnum.EnviadoSeparacao,
                        QtdSeparar = qtdTransferencia,
                        QtdSeparada = null,
                        CubagemProduto = largura * comprimento * altura,
                        PesoProduto = produto.PesoBruto,
                        DataHoraInicioSeparacao = null,
                        DataHoraFimSeparacao = null
                    };

                    pedidoVendaVolume.PedidoVendaProdutos.Add(pedidoVendaProduto);
                    pesoTotal += pesoProduto;
                }

                listaVolumeProdutosAlterar.Add(pedidoVendaProduto);
            }

            if (pesoTotal > 22)
            {
                excedeuPeso = true;
            }

            foreach (var volumeProduto in listaVolumeProdutosAlterar)
            {
                pedidoVendaVolume.CubagemVolume += volumeProduto.CubagemProduto * volumeProduto.QtdSeparar;
                pedidoVendaVolume.PesoVolume += volumeProduto.PesoProduto * volumeProduto.QtdSeparar;
                pedidoVendaVolume.PedidoVendaProdutos.Add(volumeProduto);
            }

            var volumesOrigemAgrupados = listaVolumeProdutosOrigem.GroupBy(g => g.IdPedidoVendaVolume).ToDictionary(d => d.Key, d => d.ToList());

            using (var transacao = _unitOfWork.CreateTransactionScope())
            {
                if (!idPedidoVendaVolume.HasValue)
                {
                    _unitOfWork.PedidoVendaVolumeRepository.Add(pedidoVendaVolume);
                }
                else
                {
                    _unitOfWork.PedidoVendaVolumeRepository.Update(pedidoVendaVolume);
                }

                await _unitOfWork.SaveChangesAsync();

                foreach (var volumeOrigem in volumesOrigemAgrupados)
                {
                    foreach (var volumeProduto in volumeOrigem.Value)
                    {
                        if (volumeProduto.IdPedidoVendaProduto == 0)
                        {
                            _unitOfWork.PedidoVendaProdutoRepository.Add(volumeProduto);

                            await _unitOfWork.SaveChangesAsync();

                            continue;
                        }

                        if (volumeProduto.QtdSeparar == 0)
                        {
                            volumeProduto.IdPedidoVendaStatus = PedidoVendaStatusEnum.VolumeExcluido;
                            volumeProduto.DataHoraAutorizacaoZerarPedido = DateTime.Now;
                        }
                        else if (volumeProduto.IdPedidoVendaStatus != PedidoVendaStatusEnum.EnviadoSeparacao)
                        {
                            volumeProduto.DataHoraInicioSeparacao = null;
                            volumeProduto.DataHoraFimSeparacao = null;
                            volumeProduto.IdUsuarioSeparacao = null;
                            volumeProduto.IdPedidoVendaStatus = PedidoVendaStatusEnum.EnviadoSeparacao;

                            if (volumeProduto.QtdSeparada.GetValueOrDefault() > 0)
                            {
                                await _separacaoPedidoService.AjustarQuantidadeVolume(volumeProduto, volumeProduto.QtdSeparada.Value, idEmpresa, idUsuario);
                            }
                        }

                        _unitOfWork.PedidoVendaProdutoRepository.Update(volumeProduto);

                        await _unitOfWork.SaveChangesAsync();
                    }

                    var volume = _unitOfWork.PedidoVendaVolumeRepository.GetById(volumeOrigem.Key);

                    volume.PesoVolume = volume.PedidoVendaProdutos.Sum(s => s.PesoProduto * s.QtdSeparar);
                    volume.CubagemVolume = volume.PedidoVendaProdutos.Sum(s => s.CubagemProduto * s.QtdSeparar);

                    if (!volume.PedidoVendaProdutos.Any(a => a.IdPedidoVendaStatus != PedidoVendaStatusEnum.VolumeExcluido))
                    {
                        volume.IdPedidoVendaStatus = PedidoVendaStatusEnum.VolumeExcluido;
                    }
                    else if (!volume.PedidoVendaProdutos.Any(a => a.IdPedidoVendaStatus == PedidoVendaStatusEnum.ProcessandoSeparacao || a.IdPedidoVendaStatus == PedidoVendaStatusEnum.SeparacaoConcluidaComSucesso))
                    {
                        volume.DataHoraInicioSeparacao = null;
                        volume.DataHoraFimSeparacao = null;
                        volume.IdUsuarioSeparacaoAndamento = null;
                        volume.IdPedidoVendaStatus = PedidoVendaStatusEnum.EnviadoSeparacao;
                    }

                    _unitOfWork.PedidoVendaVolumeRepository.Update(volume);
                }

                if (!pedidoVenda.PedidoVendaVolumes.Any(a => a.IdPedidoVendaStatus == PedidoVendaStatusEnum.ProcessandoSeparacao || a.IdPedidoVendaStatus == PedidoVendaStatusEnum.SeparacaoConcluidaComSucesso))
                {
                    pedidoVenda.DataHoraInicioSeparacao = null;
                    pedidoVenda.DataHoraFimSeparacao = null;
                    pedidoVenda.IdPedidoVendaStatus = PedidoVendaStatusEnum.EnviadoSeparacao;
                    pedidoVenda.Pedido.IdPedidoVendaStatus = PedidoVendaStatusEnum.EnviadoSeparacao;
                }

                var pedidoVendaChecagem = _unitOfWork.PedidoVendaRepository.GetById(pedidoVenda.IdPedidoVenda);
                if (pedidoVendaChecagem.IdPedidoVendaStatus == PedidoVendaStatusEnum.SeparacaoConcluidaComSucesso)
                {
                    throw new BusinessException("A separação do pedido está concluída, o mesmo não pode ser alterado.");
                }

                _unitOfWork.PedidoVendaRepository.Update(pedidoVenda);

                await _unitOfWork.SaveChangesAsync();

                transacao.Complete();
            }

            return excedeuPeso;
        }

        private async Task<PedidoVendaVolume> GerenciamentoVolumesGerarVolume(long? idPedidoVendaVolume, long idEmpresa, long idGrupoCorredorArmazenagem, PedidoVenda pedidoVenda, GrupoCorredorArmazenagem grupoCorredorArmazenagem)
        {
            PedidoVendaVolume pedidoVendaVolume;

            if (idPedidoVendaVolume.HasValue)
            {
                pedidoVendaVolume = _unitOfWork.PedidoVendaVolumeRepository.GetById(idPedidoVendaVolume.Value);
            }
            else
            {
                PedidoVendaVolume ultimoVolume = _unitOfWork.PedidoVendaVolumeRepository.ObterPorIdPedidoVenda(pedidoVenda.IdPedidoVenda).OrderBy(o => o.NroVolume).Last();
                Caixa caixa = _unitOfWork.CaixaRepository.BuscarTodos(idEmpresa).Where(w => w.Ativo).OrderBy(o => o.Cubagem).LastOrDefault();

                if (caixa == null)
                {
                    throw new BusinessException("Nenhuma caixa ativa foi encontrada para a empresa.");
                }

                pedidoVendaVolume = new PedidoVendaVolume()
                {
                    IdPedidoVenda = pedidoVenda.IdPedidoVenda,
                    NroCentena = ultimoVolume.NroCentena,
                    NroVolume = ultimoVolume.NroVolume + 1,
                    IdCaixaCubagem = caixa.IdCaixa,
                    EtiquetaVolume = caixa.TextoEtiqueta,
                    IdGrupoCorredorArmazenagem = idGrupoCorredorArmazenagem,
                    DataHoraInicioSeparacao = null,
                    DataHoraFimSeparacao = null,
                    IdPedidoVendaStatus = PedidoVendaStatusEnum.EnviadoSeparacao,
                    CorredorInicio = grupoCorredorArmazenagem.CorredorInicial,
                    CorredorFim = grupoCorredorArmazenagem.CorredorFinal,
                    IdImpressora = grupoCorredorArmazenagem.IdImpressora
                };

                pedidoVenda.NroVolumes += 1;
            }

            return pedidoVendaVolume;
        }

        private void GerneciamentoVolumesValidacaoVolume(long idGrupoCorredorArmazenagem, PedidoVenda pedidoVenda, GrupoCorredorArmazenagem grupoCorredorArmazenagem, GerenciarVolumeItem volume, Produto produto, PedidoVendaProduto pedidoVendaProdutoOrigem)
        {
            var nroVolume = pedidoVendaProdutoOrigem.PedidoVendaVolume.NroVolume.ToString().PadLeft(3, '0');

            if (pedidoVendaProdutoOrigem == null)
            {
                throw new BusinessException($"Ocorreu erro ao encontrar o volume do produto {produto.Referencia}");
            }

            if (pedidoVendaProdutoOrigem.IdPedidoVenda != pedidoVenda.IdPedidoVenda)
            {
                throw new BusinessException($"O volume {nroVolume} não pertence ao pedido.");
            }

            if (idGrupoCorredorArmazenagem != pedidoVendaProdutoOrigem.PedidoVendaVolume.IdGrupoCorredorArmazenagem)
            {
                throw new BusinessException($"O produto {produto.Referencia} do volume {nroVolume} não pertence ao intervalo de Corredor: {grupoCorredorArmazenagem.CorredorInicial} até {grupoCorredorArmazenagem.CorredorFinal}.");
            }

            if (pedidoVendaProdutoOrigem.QtdSeparar < volume.Quantidade)
            {
                throw new BusinessException($"A quantidade digitada excede a quanto total produto {produto.Referencia} do volume {nroVolume}");
            }

            if (pedidoVendaProdutoOrigem.IdPedidoVendaStatus != PedidoVendaStatusEnum.EnviadoSeparacao &&
                pedidoVendaProdutoOrigem.IdPedidoVendaStatus != PedidoVendaStatusEnum.ProcessandoSeparacao &&
                pedidoVendaProdutoOrigem.IdPedidoVendaStatus != PedidoVendaStatusEnum.SeparacaoConcluidaComSucesso)
            {
                throw new BusinessException($"Status do volume {nroVolume} está inválido para alteração");
            }

            if (pedidoVendaProdutoOrigem.PedidoVendaVolume.EtiquetaVolume == "F")
            {
                throw new BusinessException($"O produto {pedidoVendaProdutoOrigem.Produto.Referencia} do volume {nroVolume} não pode ser incluído no volume. No volume atual do produto, a caixa do produto é do fornecedor.");
            }

            if (pedidoVendaProdutoOrigem.IdLote.HasValue)
            {
                throw new BusinessException($"O produto {pedidoVendaProdutoOrigem.Produto.Referencia} do volume {nroVolume} não pode ser incluído no volume. No volume atual do produto, a separação do produto é fora do picking.");
            }
        }

        public PedidoVenda GerenciarVolumesValidacaoPedido(string nroPedido, long idEmpresa)
        {
            var pedidoVenda = _unitOfWork.PedidoVendaRepository.ObterPorNroPedidoEEmpresa(nroPedido, idEmpresa);
            if (pedidoVenda == null)
            {
                throw new BusinessException("O pedido não foi encontrado.");
            }

            if (pedidoVenda.IdPedidoVendaStatus == PedidoVendaStatusEnum.SeparacaoConcluidaComSucesso)
            {
                throw new BusinessException("A separação do pedido está concluída, o mesmo não pode ser alterado.");
            }

            if (pedidoVenda.IdPedidoVendaStatus != PedidoVendaStatusEnum.EnviadoSeparacao && pedidoVenda.IdPedidoVendaStatus != PedidoVendaStatusEnum.ProcessandoSeparacao)
            {
                throw new BusinessException("O pedido está com status inválido para alteração.");
            }

            return pedidoVenda;
        }

        public PedidoVenda GerenciarVolumesValidacaoVolume(long idEmpresa, long idPedidoVendaVolume)
        {
            var pedidoVendaVolume = _unitOfWork.PedidoVendaVolumeRepository.GetById(idPedidoVendaVolume);
            if (pedidoVendaVolume == null)
            {
                throw new BusinessException("O Volume não foi encontrado.");
            }

            var pedidoVenda = GerenciarVolumesValidacaoPedido(pedidoVendaVolume.PedidoVenda.Pedido.NumeroPedido, idEmpresa);

            if (pedidoVendaVolume.IdPedidoVenda != pedidoVenda.IdPedidoVenda)
            {
                throw new BusinessException("O volume e o pedido estão divergêntes.");
            }

            if (!(pedidoVendaVolume.IdPedidoVendaStatus == PedidoVendaStatusEnum.EnviadoSeparacao ||
                pedidoVendaVolume.IdPedidoVendaStatus == PedidoVendaStatusEnum.SeparacaoConcluidaComSucesso ||
                pedidoVendaVolume.IdPedidoVendaStatus == PedidoVendaStatusEnum.ProcessandoSeparacao))
            {
                throw new BusinessException("O volume está com status inválido para alteração.");
            }

            return pedidoVenda;
        }

        public bool GerenciarVolumesValidarPeso(long? idPedidoVendaVolume, List<GerenciarVolumeItem> listaVolumes)
        {
            decimal pesoTotal = 0;

            if (idPedidoVendaVolume.HasValue)
            {
                var volume = _unitOfWork.PedidoVendaVolumeRepository.GetById(idPedidoVendaVolume.Value);
                if (volume == null)
                {
                    throw new BusinessException("Volume não encontrado para validar o peso.");
                }

                pesoTotal += volume.PedidoVendaProdutos.Sum(s => s.PesoProduto * s.QtdSeparar);
            }

            foreach (var item in listaVolumes)
            {
                var produto = _unitOfWork.ProdutoRepository.GetById(item.IdProduto);
                if (produto == null)
                {
                    throw new BusinessException("Produto não encontrado para validar o peso.");
                }

                pesoTotal += produto.PesoBruto * item.Quantidade;
            }

            if (pesoTotal > 22)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task RemoverUsuarioSeparacao(long idPedidoVendaVolume)
        {
            var pedidoVendaVolume = _unitOfWork.PedidoVendaVolumeRepository.GetById(idPedidoVendaVolume);

            if (pedidoVendaVolume == null)
            {
                throw new BusinessException("Volume não encontrado");
            }

            if (pedidoVendaVolume.IdPedidoVendaStatus != PedidoVendaStatusEnum.ProcessandoSeparacao)
            {
                throw new BusinessException("Volume com status inválido");
            }

            if (pedidoVendaVolume.IdUsuarioSeparacaoAndamento.NullOrEmpty())
            {
                throw new BusinessException("Não existe usuário associado ao volume");
            }

            pedidoVendaVolume.IdUsuarioSeparacaoAndamento = null;

            await _unitOfWork.SaveChangesAsync();
        }
    }
}