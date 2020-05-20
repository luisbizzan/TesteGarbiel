﻿using DartDigital.Library.Exceptions;
using FWLog.Data;
using FWLog.Data.Models;
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

        public ExpedicaoService(UnitOfWork unitOfWork, ILog log, ColetorHistoricoService coletorHistoricoService, NotaFiscalService notaFiscalService, PedidoService pedidoService, RelatorioService relatorioService, TransportadoraService transportadoraService)
        {
            _unitOfWork = unitOfWork;
            _log = log;
            _coletorHistoricoService = coletorHistoricoService;
            _notaFiscalService = notaFiscalService;
            _pedidoService = pedidoService;
            _relatorioService = relatorioService;
            _transportadoraService = transportadoraService;
        }

        public void IniciarExpedicaoPedidoVenda(long idPedidoVenda, long idPedidoVendaVolume, string idUsuario, long idEmpresa)
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

            if (pedidoVenda.IdPedidoVendaStatus != PedidoVendaStatusEnum.SeparacaoConcluidaComSucesso && pedidoVenda.IdPedidoVendaStatus != PedidoVendaStatusEnum.InstalandoVolumeTransportadora)
            {
                throw new BusinessException("A separação do volume não está finalizada.");
            }

            var pedidoVendaVolume = pedidoVenda.PedidoVendaVolumes.First(f => f.IdPedidoVendaVolume == idPedidoVendaVolume);
            if (pedidoVendaVolume == null)
            {
                throw new BusinessException("O volume não foi encontrado.");
            }

            //Comentado porque na aparesentação do dia 15/05 Roberto disse que essa verificação não deve ser feita na instalação do volume.
            //if (Convert.ToBoolean(ConfigurationManager.AppSettings["IntegracaoSankhya_Habilitar"]))
            //{
            //    if (!pedidoVenda.Pedido.CodigoIntegracaoNotaFiscal.HasValue)
            //    {
            //        throw new BusinessException("NF não está emitida.");
            //    }
            //}

            using (var transaction = _unitOfWork.CreateTransactionScope())
            {
                pedidoVendaVolume.DataHoraInstalTransportadora = DateTime.Now;
                pedidoVendaVolume.IdUsuarioInstalTransportadora = idUsuario;

                _unitOfWork.PedidoVendaVolumeRepository.Update(pedidoVendaVolume);
                _unitOfWork.SaveChanges();
                transaction.Complete();
            }
        }

        private void BuscaEValidaDadosPorReferenciaPedido(string referenciaPedido, out int numeroPedido, out long idTransportadora, out int numeroVolume)
        {
            if (referenciaPedido.NullOrEmpty())
            {
                throw new BusinessException("Código de barras do pedido deve ser infomado.");
            }

            if (referenciaPedido.Length < 7)
            {
                throw new BusinessException("Código de Barras de pedido inválido.");
            }

            var numeroPedidoString = referenciaPedido.Substring(0, referenciaPedido.Length - 6);

            if (!int.TryParse(numeroPedidoString, out numeroPedido))
            {
                throw new BusinessException("Código de Barras de pedido inválido.");
            }

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
            BuscaEValidaDadosPorReferenciaPedido(referenciaPedido, out int numeroPedido, out long idTransportadora, out int numeroVolume);

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

                        if (long.TryParse(dadosNotaFiscal.NumeroNotaFiscal, out long numeroNotaFiscal))
                        {
                            pedido.CodigoIntegracaoNotaFiscal = numeroNotaFiscal;

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

                        if (salvaPedido)
                        {
                            await _unitOfWork.SaveChangesAsync();
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

            if (!(long.TryParse(dadosNotaFiscal.NumeroNotaFiscal, out long numeroNotaFiscal) && pedido.CodigoIntegracaoNotaFiscal == numeroNotaFiscal))
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

            var where = $"WHERE TGFVAR.NUNOTAORIG = {pedido.CodigoIntegracao} AND TGFCAB.STATUSNFE = 'A' AND TGFCAN.NUNOTA IS NULL";
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

                if (pedidoVendaVolume.IdPedidoVendaStatus != PedidoVendaStatusEnum.SeparacaoConcluidaComSucesso)
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

                    pedidoVendaVolume.IdPedidoVendaStatus = PedidoVendaStatusEnum.VolumeInstaladoTransportadora;

                    pedidoVendaVolume.PedidoVenda.IdPedidoVendaStatus = PedidoVendaStatusEnum.InstalandoVolumeTransportadora;

                    await _unitOfWork.SaveChangesAsync();

                    listaVolumesString.Add(pedidoVendaVolume.EtiquetaVolume);
                }

                var pedidoVenda = _unitOfWork.PedidoVendaRepository.GetById(listaPedidoVendaVolume.First().IdPedidoVenda);

                if (pedidoVenda.PedidoVendaVolumes.All(pvv => pvv.IdPedidoVendaStatus == PedidoVendaStatusEnum.VolumeInstaladoTransportadora))
                {
                    pedidoVenda.IdPedidoVendaStatus = PedidoVendaStatusEnum.VolumeInstaladoTransportadora;

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
            BuscaEValidaDadosPorReferenciaPedido(referenciaPedido, out int numeroPedido, out long idTransportadora, out int numeroVolume);

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
                    if (volume.DataHoraInstalacaoDOCA != null)
                    {
                        throw new BusinessException($"Volume {volume.EtiquetaVolume} ja foi lido ou está em duplicidade.");
                    }

                    if (volume.IdPedidoVendaStatus != PedidoVendaStatusEnum.MovendoDOCA && volume.IdPedidoVendaStatus != PedidoVendaStatusEnum.VolumeInstaladoTransportadora)
                    {
                        throw new BusinessException($"Volume { volume.EtiquetaVolume } não pode ser intalado na doca.");
                    }

                    if (Convert.ToBoolean(ConfigurationManager.AppSettings["IntegracaoSankhya_Habilitar"]))
                    {
                        if (volume.PedidoVenda.Pedido.CodigoIntegracaoNotaFiscal == null && !volume.PedidoVenda.Pedido.IsRequisicao)
                        {
                            throw new BusinessException($"Volume { volume.EtiquetaVolume } não tem nota fiscal faturada.");
                        }
                    }

                    if (volume.PedidoVenda.IdTransportadora != idTransportadora)
                    {
                        throw new BusinessException($"Volume { volume.EtiquetaVolume } não pertence a transportadora: {transportadora.NomeFantasia}.");
                    }

                    volume.IdUsuarioInstalacaoDOCA = idUsuario;
                    volume.DataHoraInstalacaoDOCA = dataProcessamento;
                    volume.IdPedidoVendaStatus = PedidoVendaStatusEnum.MovidoDOCA;

                    if (!volume.PedidoVenda.PedidoVendaVolumes.Any(a => a.IdPedidoVendaStatus != PedidoVendaStatusEnum.MovidoDOCA))
                    {
                        volume.PedidoVenda.IdPedidoVendaStatus = PedidoVendaStatusEnum.MovidoDOCA;
                    }
                    else
                    {
                        volume.PedidoVenda.IdPedidoVendaStatus = PedidoVendaStatusEnum.MovendoDOCA;
                    }

                    _unitOfWork.SaveChanges();
                }

                transacao.Complete();
            }

            string stringVolumes;

            if (pedidoVolumes.Count > 1)
            {
                var ultimoVolume = pedidoVolumes.Last();

                stringVolumes = string.Join(", ", pedidoVolumes.Where(w => w.IdPedidoVendaVolume != ultimoVolume.IdPedidoVendaVolume).Select(s => s.EtiquetaVolume).ToArray());

                stringVolumes = string.Concat(stringVolumes, $" e { ultimoVolume.EtiquetaVolume }");
            }
            else
            {
                stringVolumes = pedidoVolumes.First().EtiquetaVolume;
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
                DataHoraEmissaoRomaneio = DateTime.Now,
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
                // Romaneio
                var romaneio = new Romaneio();
                romaneio.IdTransportadora = idTransportadora;
                romaneio.IdEmpresa = idEmpresa;
                romaneio.NroRomaneio = GetNextNroRomaneioByEmpresa(idEmpresa);

                _unitOfWork.RomaneioRepository.Add(romaneio);
                _unitOfWork.SaveChanges();

                foreach (var chaveAcesso in chavesAcessos)
                {
                    // validações
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

                    // Notas do Romaneio
                    var romaneioNotaFiscal = new RomaneioNotaFiscal();
                    romaneioNotaFiscal.IdRomaneio = romaneio.IdRomaneio;
                    romaneioNotaFiscal.IdPedidoVenda = pedidoVenda.IdPedidoVenda;
                    romaneioNotaFiscal.NroNotaFiscal = Convert.ToInt32(pedido.CodigoIntegracaoNotaFiscal);
                    romaneioNotaFiscal.NroVolumes = pedidoVenda.NroVolumes;
                    romaneioNotaFiscal.IdCliente = pedidoVenda.IdCliente;

                    // tratamento especial pra empresas k1 e k2
                    var empresa = _unitOfWork.EmpresaRepository.GetById(idEmpresa);
                    if (empresa.Sigla == "K1" || empresa.Sigla == "K2")
                    {
                        var somaDosVolumes = pedidoVenda.PedidoVendaVolumes.Aggregate(default(decimal), (x, y) => x + y.PesoVolume);
                        romaneioNotaFiscal.TotalPesoLiquidoVolumes = somaDosVolumes;
                        romaneioNotaFiscal.TotalPesoBrutoVolumes = somaDosVolumes;
                    }

                    _unitOfWork.RomaneioNotaFiscalRepository.Add(romaneioNotaFiscal);
                    _unitOfWork.SaveChanges();

                    pedidoVenda.IdUsuarioRomaneio = idUsuario;
                    var dataHoraEmissaoRomaneio = DateTime.Now;
                    pedidoVenda.DataHoraRomaneio = dataHoraEmissaoRomaneio;

                    // atualiza status no Sankhya e nas entidades do pedido
                    await _pedidoService.AtualizarStatusPedido(pedidoVenda.Pedido, PedidoVendaStatusEnum.RomaneioImpresso);

                    pedido.IdPedidoVendaStatus = PedidoVendaStatusEnum.RomaneioImpresso;
                    _unitOfWork.PedidoRepository.Update(pedido);

                    pedidoVenda.IdPedidoVendaStatus = PedidoVendaStatusEnum.RomaneioImpresso;
                    _unitOfWork.PedidoVendaRepository.Update(pedidoVenda);

                    foreach (var pedidoVendaVolume in pedidoVenda.PedidoVendaVolumes)
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
            int? ultimoRomaneio;

            ultimoRomaneio = _unitOfWork.RomaneioRepository.BuscaUltimoNroRomaneioPorEmpresa(idEmpresa);

            if (ultimoRomaneio.HasValue)
                ultimoRomaneio = +1;
            else
                ultimoRomaneio = 1;

            return ultimoRomaneio.Value;
        }

        public void ReimprimirRomaneio(long idRomaneio, long idImpressora, long idEmpresa, string idUsuario)
        {
            var romaneio = ValidarIdRomaneio(idRomaneio, idEmpresa);

            var impressora = ValidarIdImpressora(idImpressora);

            ImprimirRomaneio(romaneio.IdRomaneio, impressora.Id, false, idEmpresa, idUsuario);

            _coletorHistoricoService.GravarHistoricoColetor(new GravarHistoricoColetorRequisicao
            {
                IdColetorAplicacao = ColetorAplicacaoEnum.Expedicao,
                IdColetorHistoricoTipo = ColetorHistoricoTipoEnum.ReimpressaoRomaneio,
                Descricao = $"Romaneio {romaneio.NroRomaneio} da transportadora {romaneio.Transportadora.NomeFantasia} reimpresso.",
                IdEmpresa = idEmpresa,
                IdUsuario = idUsuario
            });
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
            BuscaEValidaDadosPorReferenciaPedido(referenciaPedido, out int numeroPedido, out long idTransportadora, out int numeroVolume);

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
                pedidoVendaVolume.IdPedidoVendaStatus = PedidoVendaStatusEnum.VolumeInstaladoTransportadora;
                pedidoVendaVolume.DataHoraRemocaoVolume = DateTime.Now;
                pedidoVendaVolume.IdUsuarioInstalacaoDOCA = null;
                pedidoVendaVolume.DataHoraInstalacaoDOCA = null;

                _unitOfWork.SaveChanges();

                if (pedidoVenda.PedidoVendaVolumes.All(x => x.IdPedidoVendaStatus == PedidoVendaStatusEnum.VolumeInstaladoTransportadora))
                {
                    pedidoVenda.IdPedidoVendaStatus = PedidoVendaStatusEnum.VolumeInstaladoTransportadora;
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
    }
}