using DartDigital.Library.Exceptions;
using FWLog.Data;
using FWLog.Data.Models;
using FWLog.Services.Integracao;
using FWLog.Services.Model.Coletor;
using FWLog.Services.Model.Expedicao;
using FWLog.Services.Model.IntegracaoSankhya;
using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace FWLog.Services.Services
{
    public class ExpedicaoService
    {
        private readonly UnitOfWork _unitOfWork;
        private ILog _log;
        private readonly ColetorHistoricoService _coletorHistoricoService;

        public ExpedicaoService(UnitOfWork unitOfWork, ILog log, ColetorHistoricoService coletorHistoricoService)
        {
            _unitOfWork = unitOfWork;
            _log = log;
            _coletorHistoricoService = coletorHistoricoService;
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

            // TODO: melhorar validação quando Sankhya criar os status de Nota e Guia:
            // - verificar se a NF do pedido foi emitida, e confirmada. Status: “A” no Sankhya e as guias foram pagas, status “Y” no Sankhya
            if (!pedidoVenda.Pedido.CodigoIntegracaoNotaFiscal.HasValue)
            {
                throw new BusinessException("NF e Guias não estão emitidas/pagas.");
            }

            using (var transaction = _unitOfWork.CreateTransactionScope())
            {
                pedidoVendaVolume.DataHoraInstalTransportadora = DateTime.Now;
                pedidoVendaVolume.IdUsuarioInstalTransportadora = idUsuario;

                _unitOfWork.PedidoVendaVolumeRepository.Update(pedidoVendaVolume);
                _unitOfWork.SaveChanges();
                transaction.Complete();
            }
        }

        public PedidoVendaVolumeResposta BuscaPedidoVendaVolume(string referenciaPedido, long idEmpresa)
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

            if (!int.TryParse(numeroPedidoString, out int numeroPedido))
            {
                throw new BusinessException("Código de Barras de pedido inválido.");
            }

            var numeroVolumeString = referenciaPedido.Substring(referenciaPedido.Length - 3);

            if (!int.TryParse(numeroVolumeString, out int numeroVolume))
            {
                throw new BusinessException("Código de Barras de pedido inválido.");
            }

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

            var codigoTransportadora = referenciaPedido.Substring(referenciaPedido.Length - 6).Replace(numeroVolumeString, "");

            if (pedidoVenda.Transportadora.CodigoTransportadora != codigoTransportadora)
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

            if (pedido.CodigoIntegracaoNotaFiscal == null)
            {
                throw new BusinessException("Pedido sem NF emitida.");
            }

            PedidoNumeroNotaFiscalIntegracao dadosNotaFiscal = await ConsultarSituacaoNFVenda(pedido);

            if (dadosNotaFiscal == null)
            {
                return false;
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

        public PedidoVendaVolume ValidaTransportadoraInstalacaoVolume(long idPedidoVendaVolume, long idtransportadora, long idEmpresa)
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

        public void ValidaEnderecoInstalacaoVolume(long idPedidoVendaVolume, long idTransportadora, long idEnderecoArmazenagem, long idEmpresa)
        {
            var pedidoVendaVolume = ValidaTransportadoraInstalacaoVolume(idPedidoVendaVolume, idTransportadora, idEmpresa);

            var enderecoTransportadora = _unitOfWork.TransportadoraEnderecoRepository.ObterPorEnderecoTransportadoraEmpresa(idEnderecoArmazenagem, pedidoVendaVolume.PedidoVenda.IdTransportadora, idEmpresa);

            if (enderecoTransportadora == null)
            {
                throw new BusinessException("Endereço não pertence a transportadora.");
            }
        }

        public async Task SalvaInstalacaoVolumes(List<long> listaIdsVolumes, long idTransportadora, long idEnderecoArmazenagem, long idEmpresa, string idUsuario)
        {
            if (listaIdsVolumes.NullOrEmpty())
            {
                throw new BusinessException("A lista de volumes deve ser informada.");
            }

            if (idEnderecoArmazenagem <= 0)
            {
                throw new BusinessException("O endereço deve ser informado.");
            }

            foreach (var idPedidoVendaVolume in listaIdsVolumes)
            {
                ValidaEnderecoInstalacaoVolume(idPedidoVendaVolume, idTransportadora, idEnderecoArmazenagem, idEmpresa);
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
                foreach (var pedidoVendaVolume in listaPedidoVendaVolume)
                {
                    pedidoVendaVolume.IdEnderecoArmazTransportadora = idEnderecoArmazenagem;

                    pedidoVendaVolume.IdPedidoVendaStatus = PedidoVendaStatusEnum.VolumeInstaladoTransportadora;

                    pedidoVendaVolume.PedidoVenda.IdPedidoVendaStatus = PedidoVendaStatusEnum.InstalandoVolumeTransportadora;

                    await _unitOfWork.SaveChangesAsync();
                }

                var pedidoVenda = _unitOfWork.PedidoVendaRepository.GetById(listaPedidoVendaVolume.First().IdPedidoVenda);

                if (pedidoVenda.PedidoVendaVolumes.All(pvv => pvv.IdPedidoVendaStatus == PedidoVendaStatusEnum.VolumeInstaladoTransportadora))
                {
                    pedidoVenda.IdPedidoVendaStatus = PedidoVendaStatusEnum.VolumeInstaladoTransportadora;

                    await _unitOfWork.SaveChangesAsync();
                }

                transacao.Complete();
            }
        }

        public EnderecosPorTransportadoraResposta BuscaEnderecosPorTransportadora(string codigoTransportadora, long idEmpresa)
        {
            if (string.IsNullOrWhiteSpace(codigoTransportadora))
            {
                throw new BusinessException("O código da transportadora deve ser informado.");
            }

            var transportadora = _unitOfWork.TransportadoraRepository.ConsultarPorCodigoTransportadora(codigoTransportadora);

            if (transportadora == null)
            {
                throw new BusinessException("A tranportadora informada não foi encontrada.");
            }

            var volumesInstalados = _unitOfWork.PedidoVendaVolumeRepository.ObterVolumesInstaladosPorTranportadoraEmpresa(transportadora.IdTransportadora, idEmpresa);

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

        public PedidoVendaVolumeResposta ValidarVolumeDoca(string referenciaPedido, string idUsuario, long idEmpresa)
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

            if (!int.TryParse(numeroPedidoString, out int numeroPedido))
            {
                throw new BusinessException("Código de Barras de pedido inválido.");
            }

            var numeroVolumeString = referenciaPedido.Substring(referenciaPedido.Length - 3);

            if (!int.TryParse(numeroVolumeString, out int numeroVolume))
            {
                throw new BusinessException("Código de Barras de pedido inválido.");
            }

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

            var codigoTransportadora = referenciaPedido.Substring(referenciaPedido.Length - 6).Replace(numeroVolumeString, "");

            if (pedidoVenda.Transportadora.CodigoTransportadora != codigoTransportadora)
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

            if (!pedidoVenda.Pedido.CodigoIntegracaoNotaFiscal.HasValue)
            {
                throw new BusinessException("Este volume não tem uma nota fiscal faturada.");
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

        public DespachoTransportadoraResposta ValidarDespachoTransportadora(string codigoTransportadora, string idUsuario, long idEmpresa)
        {
            if (string.IsNullOrWhiteSpace(codigoTransportadora))
            {
                throw new BusinessException("O código da transportadora deve ser informado.");
            }

            var transportadora = _unitOfWork.TransportadoraRepository.ConsultarPorCodigoTransportadora(codigoTransportadora);

            if (transportadora == null)
            {
                throw new BusinessException("A tranportadora informada não foi encontrada.");
            }

            if (!transportadora.Ativo)
            {
                throw new BusinessException("A tranportadora está inativa.");
            }

            var existemPedidosParaDespacho = _unitOfWork.PedidoVendaRepository.ExistemPedidosParaDespachoNaTransportadora(transportadora.IdTransportadora, idEmpresa);

            if (!existemPedidosParaDespacho)
            {
                throw new BusinessException("Não existem volumes na DOCA para esta transportadora.");
            }

            var resposta = new DespachoTransportadoraResposta()
            {
                IdTransportadora = transportadora.IdTransportadora
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

                    if (volume.PedidoVenda.Pedido.CodigoIntegracaoNotaFiscal == null)
                    {
                        throw new BusinessException($"Volume { volume.EtiquetaVolume } não tem nota fiscal faturada.");
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

        public void FinalizarDespachoNF(long idTransportadora, string chaveAcesso)
        {
            if (idTransportadora <= 0)
            {
                throw new BusinessException("Favor informar a tranportadora.");
            }

            if (chaveAcesso.NullOrEmpty())
            {
                throw new BusinessException("Favor informar a chave de acesso da NF.");
            }

            if (chaveAcesso.Length != 44)
            {
                throw new BusinessException("Chave de acesso inválida.");
            }

            Transportadora transportadora = _unitOfWork.TransportadoraRepository.GetById(idTransportadora);

            if (transportadora == null)
            {

            }
        }

        public void ImprimirRomaneio(int nroRomaneio, int nroNotaFiscal, long idImpressora, bool imprimeSegundaVia, long idEmpresa, string idUsuario)
        {
            if (nroRomaneio <= 0)
            {
                throw new BusinessException("Número romaneio deve ser informado.");
            }

            if (nroNotaFiscal <= 0)
            {
                throw new BusinessException("Número NF deve ser informado.");
            }

            if (idImpressora <= 0)
            {
                throw new BusinessException("Impressora deve ser informada.");
            }

            var romaneio = _unitOfWork.RomaneioRepository.BuscarPorNumeroRomaneioEEmpresa(nroRomaneio, idEmpresa);

            if (romaneio == null)
            {
                throw new BusinessException("Romaneio não encontrado.");
            }

            var romaneioNotaFiscal = _unitOfWork.RomaneioNotaFiscalRepository.BuscarPorRomaneioENumeroNotaFiscal(nroRomaneio, nroNotaFiscal);

            if (romaneioNotaFiscal == null)
            {
                throw new BusinessException("Nota Fiscal de romaneio não encontrada.");
            }

            var impressora = _unitOfWork.BOPrinterRepository.GetById(idImpressora);

            if (impressora == null)
            {
                throw new BusinessException("Impressora não encontrada.");
            }
        }
    }
}