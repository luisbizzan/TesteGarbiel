using DartDigital.Library.Exceptions;
using FWLog.Data;
using FWLog.Data.Models;
using FWLog.Services.Integracao;
using FWLog.Services.Model.Caixa;
using FWLog.Services.Model.Coletor;
using FWLog.Services.Model.Etiquetas;
using FWLog.Services.Model.GrupoCorredorArmazenagem;
using FWLog.Services.Model.IntegracaoSankhya;
using FWLog.Services.Model.Produto;
using FWLog.Services.Model.ProdutoEstoque;
using FWLog.Services.Model.SeparacaoPedido;
using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace FWLog.Services.Services
{
    public class SeparacaoPedidoService
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly ColetorHistoricoService _coletorHistoricoService;
        private readonly ILog _log;
        private List<CaixaViewModel> listaRankingCaixas;
        private readonly PedidoVendaService _pedidoVendaService;
        private readonly PedidoService _pedidoService;
        private readonly PedidoVendaProdutoService _pedidoVendaProdutoService;
        private readonly PedidoVendaVolumeService _pedidoVendaVolumeService;
        private readonly EtiquetaService _etiquetaService;
        private readonly CaixaService _caixaService;

        public SeparacaoPedidoService(UnitOfWork unitOfWork, ColetorHistoricoService coletorHistoricoService, ILog log, PedidoService pedidoService, PedidoVendaService pedidoVendaService,
            PedidoVendaProdutoService pedidoVendaProdutoService, PedidoVendaVolumeService pedidoVendaVolumeService, EtiquetaService etiquetaService, CaixaService caixaService)
        {
            _unitOfWork = unitOfWork;
            _coletorHistoricoService = coletorHistoricoService;
            _log = log;
            _pedidoService = pedidoService;
            _pedidoVendaService = pedidoVendaService;
            _pedidoVendaProdutoService = pedidoVendaProdutoService;
            _pedidoVendaVolumeService = pedidoVendaVolumeService;
            _etiquetaService = etiquetaService;
            _caixaService = caixaService;
        }

        public List<PedidoVendaVolumeEmSeparacaoViewModel> ConsultaPedidoVendaEmSeparacao(string idUsuario, long idEmpresa)
        {
            var pedidoVendaVolumes = _unitOfWork.PedidoVendaVolumeRepository.PesquisarIdsEmSeparacao(idUsuario, idEmpresa);

            var result = pedidoVendaVolumes.Select(x => new PedidoVendaVolumeEmSeparacaoViewModel
            {
                IdPedidoVenda = x.IdPedidoVenda,
                IdPedidoVendaVolume = x.IdPedidoVendaVolume,
                EtiquetaVolume = $"{x.PedidoVenda.NroPedidoVenda.ToString().PadLeft(7, '0')}{x.PedidoVenda.Transportadora.IdTransportadora.ToString().PadLeft(3, '0')}{x.NroVolume.ToString().PadLeft(3, '0')}"
            }).ToList();

            return result;
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

        public BuscarPedidoVendaResposta BuscarPedidoVenda(string referenciaPedido, long idEmpresa, string idUsuario, bool temPermissaoF7)
        {
            BuscaEValidaDadosPorReferenciaPedido(referenciaPedido, out int numeroPedido, out long idTransportadora, out int numeroVolume);

            var pedidoVenda = _unitOfWork.PedidoVendaRepository.ObterPorNroPedidoEEmpresa(numeroPedido, idEmpresa);

            ValidarPedidoVenda(pedidoVenda, idEmpresa);

            var pedidoVendaVolume = pedidoVenda.PedidoVendaVolumes.FirstOrDefault(volume => volume.NroVolume == numeroVolume);

            ValidaPedidoVendaVolumeNaBuscaPedidoVenda(pedidoVendaVolume);

            var usuarioEmpresa = _unitOfWork.UsuarioEmpresaRepository.Obter(idEmpresa, idUsuario);
            IEnumerable<int> rangeDeCorredoresDoUsuario = null;

            if (!usuarioEmpresa.CorredorSeparacaoInicio.HasValue && !usuarioEmpresa.CorredorSeparacaoFim.HasValue && !temPermissaoF7)
            {
                throw new BusinessException("O Usuário não possui corredor de separação vinculado no cadastro e não possui acesso a função F7.");
            }

            if (usuarioEmpresa.CorredorSeparacaoInicio.HasValue && usuarioEmpresa.CorredorSeparacaoFim.HasValue)
            {
                rangeDeCorredoresDoUsuario = Enumerable.Range(usuarioEmpresa.CorredorSeparacaoInicio.Value, (usuarioEmpresa.CorredorSeparacaoFim.Value - usuarioEmpresa.CorredorSeparacaoInicio.Value) + 1);

                var corredoresDosProdutos = pedidoVendaVolume.PedidoVendaProdutos.Select(x => x.EnderecoArmazenagem.Corredor).Distinct();

                if (rangeDeCorredoresDoUsuario.Intersect(corredoresDosProdutos).Count() == 0 && !temPermissaoF7)
                {
                    throw new BusinessException("Nenhum produto para separar.");
                }
            }

            var model = new BuscarPedidoVendaResposta();

            model.IdPedidoVenda = pedidoVenda.IdPedidoVenda;
            model.NroPedidoVenda = pedidoVenda.NroPedidoVenda;
            model.SeparacaoIniciada = pedidoVenda.IdPedidoVendaStatus == PedidoVendaStatusEnum.ProcessandoSeparacao;
            model.IdPedidoVendaVolume = pedidoVendaVolume.IdPedidoVendaVolume;
            model.IdCaixaVolume = pedidoVendaVolume.IdCaixaCubagem;
            model.NroVolume = pedidoVendaVolume.NroVolume;

            var statusRetorno = new List<PedidoVendaStatusEnum>()
            {
                PedidoVendaStatusEnum.EnviadoSeparacao, PedidoVendaStatusEnum.ProcessandoSeparacao
            };

            var consultaProdutos = (from pedidoVendaProduto in pedidoVendaVolume.PedidoVendaProdutos
                                    let enderecoArmazenagem = pedidoVendaProduto.EnderecoArmazenagem
                                    from grupoCorredorArmazenagem in _unitOfWork.GrupoCorredorArmazenagemRepository.Todos().Where(gca =>
                                                   gca.IdPontoArmazenagem == enderecoArmazenagem.IdPontoArmazenagem &&
                                                   enderecoArmazenagem.Corredor >= gca.CorredorInicial &&
                                                   enderecoArmazenagem.Corredor <= gca.CorredorFinal)
                                    where
                                        pedidoVendaProduto.QtdSeparada.GetValueOrDefault() < pedidoVendaProduto.QtdSeparar &&
                                        statusRetorno.Contains(pedidoVendaProduto.IdPedidoVendaStatus) &&
                                        (temPermissaoF7 == true || rangeDeCorredoresDoUsuario.Contains(pedidoVendaProduto.EnderecoArmazenagem.Corredor))
                                    select new
                                    {
                                        GrupoCorredorArmazenagem = new
                                        {
                                            grupoCorredorArmazenagem.IdGrupoCorredorArmazenagem,
                                            grupoCorredorArmazenagem.CorredorInicial,
                                            grupoCorredorArmazenagem.CorredorFinal,
                                        },
                                        pedidoVendaProduto
                                    }).ToList();

            model.ListaCorredoresSeparacao = consultaProdutos.GroupBy(g => g.GrupoCorredorArmazenagem).Select(g => new BuscarPedidoVendaGrupoCorredorResposta
            {
                IdGrupoCorredorArmazenagem = g.Key.IdGrupoCorredorArmazenagem,
                CorredorInicial = g.Key.CorredorInicial,
                CorredorFinal = g.Key.CorredorFinal,
                ListaEnderecosArmazenagem = g.Select(item => new BuscarPedidoVendaGrupoCorredorEnderecoProdutoResposta
                {
                    Corredor = item.pedidoVendaProduto.EnderecoArmazenagem.Corredor,
                    Codigo = item.pedidoVendaProduto.EnderecoArmazenagem.Codigo,
                    PontoArmazenagem = item.pedidoVendaProduto.EnderecoArmazenagem.PontoArmazenagem.Descricao,
                    IdProduto = item.pedidoVendaProduto.IdProduto,
                    ReferenciaProduto = item.pedidoVendaProduto.Produto.Referencia,
                    MultiploProduto = item.pedidoVendaProduto.Produto.MultiploVenda,
                    QtdePedido = item.pedidoVendaProduto.QtdSeparar,
                    QtdSeparada = item.pedidoVendaProduto.QtdSeparada.GetValueOrDefault(),
                    IdPontoArmazenagem = item.pedidoVendaProduto.EnderecoArmazenagem.IdPontoArmazenagem,
                    Picking = item.pedidoVendaProduto.IdLote == null
                }).ToList()
            }).ToList();

            model.ListaCorredoresSeparacao = model.ListaCorredoresSeparacao.OrderBy(order => order.CorredorInicial).ThenBy(order => order.CorredorFinal).ToList();

            foreach (var item in model.ListaCorredoresSeparacao)
            {
                item.ListaEnderecosArmazenagem = item.ListaEnderecosArmazenagem.OrderBy(order => order.Corredor).ThenBy(order => order.Codigo).ToList();
            }

            return model;
        }

        private static void ValidaPedidoVendaVolumeNaBuscaPedidoVenda(PedidoVendaVolume pedidoVendaVolume)
        {
            if (pedidoVendaVolume == null)
            {
                throw new BusinessException("Volume não encontrado.");
            }

            if (pedidoVendaVolume.IdPedidoVendaStatus == PedidoVendaStatusEnum.Cancelado || pedidoVendaVolume.IdPedidoVendaStatus == PedidoVendaStatusEnum.PendenteCancelamento)
            {
                throw new BusinessException("Volume cancelado.");
            }

            if (pedidoVendaVolume.IdPedidoVendaStatus == PedidoVendaStatusEnum.PendenteSeparacao || pedidoVendaVolume.IdPedidoVendaStatus == PedidoVendaStatusEnum.ProcessandoIntegracao)
            {
                throw new BusinessException("Volume não liberado para separação.");
            }

            if (pedidoVendaVolume.IdPedidoVendaStatus != PedidoVendaStatusEnum.EnviadoSeparacao && pedidoVendaVolume.IdPedidoVendaStatus != PedidoVendaStatusEnum.ProcessandoSeparacao)
            {
                throw new BusinessException("Volume já separado.");
            }
        }

        private void ValidarPedidoVenda(PedidoVenda pedidoVenda, long idEmpresa)
        {
            if (pedidoVenda == null)
            {
                throw new BusinessException("Pedido não foi encontrado.");
            }

            if (pedidoVenda.IdEmpresa != idEmpresa)
            {
                throw new BusinessException("Pedido não pertence a empresa do usuário logado.");
            }
        }

        private void ValidarPedidoVendaEmSeparacao(PedidoVenda pedidoVenda, long idEmpresa)
        {
            ValidarPedidoVenda(pedidoVenda, idEmpresa);

            if (pedidoVenda.IdPedidoVendaStatus == PedidoVendaStatusEnum.EnviadoSeparacao)
            {
                throw new BusinessException("O pedido informado ainda não está liberado para a separação.");
            }

            if (pedidoVenda.IdPedidoVendaStatus == PedidoVendaStatusEnum.SeparacaoConcluidaComSucesso)
            {
                throw new BusinessException("O pedido informado já foi separado.");
            }

            if (pedidoVenda.IdPedidoVendaStatus == PedidoVendaStatusEnum.PendenteCancelamento || pedidoVenda.IdPedidoVendaStatus == PedidoVendaStatusEnum.Cancelado)
            {
                throw new BusinessException("O pedido informado teve a separação cancelada.");
            }
        }

        private void ValidarPedidoVendaVolumeConcluidoCancelado(PedidoVendaVolume pedidoVendaVolume)
        {
            if (pedidoVendaVolume == null)
            {
                throw new BusinessException("O volume informado não foi encontrado.");
            }

            if (pedidoVendaVolume.IdPedidoVendaStatus == PedidoVendaStatusEnum.SeparacaoConcluidaComSucesso)
            {
                throw new BusinessException("O volume informado já foi separado.");
            }

            if (pedidoVendaVolume.IdPedidoVendaStatus == PedidoVendaStatusEnum.PendenteCancelamento || pedidoVendaVolume.IdPedidoVendaStatus == PedidoVendaStatusEnum.Cancelado)
            {
                throw new BusinessException("O volume informado teve a separação cancelada.");
            }
        }

        private async Task AjustarQuantidadeVolume(PedidoVendaProduto pedidoVendaProduto, int quantidadeAdicionar, long idEmpresa, string idUsuarioAjuste)
        {
            LoteProdutoEndereco loteProdutoEndereco;

            if (pedidoVendaProduto.IdLote == null)
            {
                loteProdutoEndereco = _unitOfWork.LoteProdutoEnderecoRepository.PesquisarPorEnderecoProdutoEmpresaPicking(pedidoVendaProduto.IdEnderecoArmazenagem, pedidoVendaProduto.IdProduto, idEmpresa);
            }
            else
            {
                loteProdutoEndereco = _unitOfWork.LoteProdutoEnderecoRepository.PesquisarPorEnderecoLoteProdutoEmpresa(pedidoVendaProduto.IdEnderecoArmazenagem, pedidoVendaProduto.IdLote.Value, pedidoVendaProduto.IdProduto, idEmpresa);
            }

            if (loteProdutoEndereco == null)
            {
                throw new BusinessException("Não foi encontrado lote produto endereço associado ao produto.");
            }

            var quantidadeAnterior = loteProdutoEndereco.Quantidade;

            var novaQuantidade = loteProdutoEndereco.Quantidade += quantidadeAdicionar;

            var produto = loteProdutoEndereco.Produto;

            var novoPesoInstalacao = produto.PesoLiquido / produto.MultiploVenda * novaQuantidade;

            var pesoAnterior = loteProdutoEndereco.PesoTotal;

            loteProdutoEndereco.Quantidade = novaQuantidade;
            loteProdutoEndereco.PesoTotal = novoPesoInstalacao;

            _unitOfWork.LoteProdutoEnderecoRepository.Update(loteProdutoEndereco);
            await _unitOfWork.SaveChangesAsync();

            var idColetorHistoricoTipo = ColetorHistoricoTipoEnum.AjustarQuantidade;

            string descricaoColetorHistorico;

            if (loteProdutoEndereco.IdLote.HasValue)
            {
                if (loteProdutoEndereco.QuantidadeCaixas.HasValue)
                {
                    loteProdutoEndereco.QuantidadeCaixas = loteProdutoEndereco.QuantidadeCaixas + (quantidadeAdicionar < 0 ? -1 : +1);
                }

                var loteProduto = _unitOfWork.LoteProdutoRepository.PesquisarProdutoNoLote(idEmpresa, pedidoVendaProduto.IdLote.Value, pedidoVendaProduto.IdProduto);

                loteProduto.Saldo += quantidadeAdicionar;

                await _unitOfWork.SaveChangesAsync();

                var idLoteMovimentacaoTipo = LoteMovimentacaoTipoEnum.Ajuste;

                var idLote = loteProdutoEndereco.IdLote.Value;
                var idProduto = loteProdutoEndereco.IdProduto;
                var idEnderecoArmazenagem = loteProdutoEndereco.IdEnderecoArmazenagem;

                if (loteProdutoEndereco.Quantidade == 0)
                {
                    idColetorHistoricoTipo = ColetorHistoricoTipoEnum.RetirarProduto;

                    idLoteMovimentacaoTipo = LoteMovimentacaoTipoEnum.Saida;

                    descricaoColetorHistorico = $"Retirou o produto {loteProdutoEndereco.Produto.Referencia} quantidade {quantidadeAnterior} peso {pesoAnterior} do lote {loteProdutoEndereco.IdLote} do endereço {loteProdutoEndereco.EnderecoArmazenagem.Codigo} devido a separação";

                    _unitOfWork.LoteProdutoEnderecoRepository.Delete(loteProdutoEndereco);
                    await _unitOfWork.SaveChangesAsync();
                }
                else
                {
                    descricaoColetorHistorico = $"Ajustou a quantidade de {quantidadeAnterior} para {loteProdutoEndereco.Quantidade} unidade(s) do produto {loteProdutoEndereco.Produto.Referencia} do lote {loteProdutoEndereco.IdLote} do endereço {loteProdutoEndereco.EnderecoArmazenagem.Codigo} devido a separação";
                }

                var loteMovimentacao = new LoteMovimentacao
                {
                    IdEmpresa = idEmpresa,
                    IdLote = idLote,
                    IdProduto = idProduto,
                    IdEnderecoArmazenagem = idEnderecoArmazenagem,
                    IdUsuarioMovimentacao = idUsuarioAjuste,
                    Quantidade = novaQuantidade,
                    IdLoteMovimentacaoTipo = idLoteMovimentacaoTipo,
                    DataHora = DateTime.Now
                };

                _unitOfWork.LoteMovimentacaoRepository.Add(loteMovimentacao);
                await _unitOfWork.SaveChangesAsync();
            }
            else
            {
                descricaoColetorHistorico = $"Ajustou a quantidade de {quantidadeAnterior} para {loteProdutoEndereco.Quantidade} unidade(s) do produto {loteProdutoEndereco.Produto.Referencia} do endereço {loteProdutoEndereco.EnderecoArmazenagem.Codigo} devido a separação";
            }

            var gravarHistoricoColetorRequisicao = new GravarHistoricoColetorRequisicao
            {
                IdColetorAplicacao = ColetorAplicacaoEnum.Separacao,
                IdColetorHistoricoTipo = idColetorHistoricoTipo,
                Descricao = descricaoColetorHistorico,
                IdEmpresa = idEmpresa,
                IdUsuario = idUsuarioAjuste
            };

            _coletorHistoricoService.GravarHistoricoColetor(gravarHistoricoColetorRequisicao);
        }

        public async Task CancelarPedidoSeparacao(long idPedidoVendaVolume, string usuarioPermissaoCancelamento, bool usuarioTemPermissao, string idUsuarioPermissao, string idUsuarioOperacao, long idEmpresa)
        {
            if (idPedidoVendaVolume <= 0)
            {
                throw new BusinessException("Volume deve ser informado.");
            }

            var pedidoVendaVolume = _unitOfWork.PedidoVendaVolumeRepository.GetById(idPedidoVendaVolume);

            if (pedidoVendaVolume == null)
            {
                throw new BusinessException("Volume não encontrado.");
            }

            if (pedidoVendaVolume.PedidoVenda.IdEmpresa != idEmpresa)
            {
                throw new BusinessException("Volume não pertence a empresa do usuário.");

            }

            if (pedidoVendaVolume.IdPedidoVendaStatus != PedidoVendaStatusEnum.ProcessandoSeparacao)
            {
                throw new BusinessException("Volume não está em separação.");
            }

            if (usuarioPermissaoCancelamento.NullOrEmpty())
            {
                throw new BusinessException("Usuário deve ser informado.");
            }

            if (usuarioTemPermissao == false || idUsuarioPermissao.NullOrEmpty())
            {
                throw new BusinessException("Usuário não tem permissão para cancelamento.");
            }

            using (var transacao = _unitOfWork.CreateTransactionScope())
            {
                var novoStatusSeparacao = PedidoVendaStatusEnum.EnviadoSeparacao;

                pedidoVendaVolume.IdPedidoVendaStatus = novoStatusSeparacao;
                pedidoVendaVolume.DataHoraInicioSeparacao = null;
                pedidoVendaVolume.DataHoraFimSeparacao = null;
                pedidoVendaVolume.IdCaixaVolume = null;
                pedidoVendaVolume.IdUsuarioSeparacaoAndamento = null;

                foreach (var pedidoVendaProduto in pedidoVendaVolume.PedidoVendaProdutos.ToList())
                {
                    if (pedidoVendaProduto.QtdSeparada.GetValueOrDefault() > 0)
                    {
                        await AjustarQuantidadeVolume(pedidoVendaProduto, pedidoVendaProduto.QtdSeparada.Value, idEmpresa, idUsuarioOperacao);
                    }

                    pedidoVendaProduto.QtdSeparada = 0;
                    pedidoVendaProduto.IdPedidoVendaStatus = novoStatusSeparacao;
                    pedidoVendaProduto.IdUsuarioSeparacao = null;
                    pedidoVendaProduto.DataHoraInicioSeparacao = null;
                    pedidoVendaProduto.DataHoraFimSeparacao = null;
                    pedidoVendaProduto.IdUsuarioAutorizacaoZerar = null;
                    pedidoVendaProduto.DataHoraAutorizacaoZerarPedido = null;
                }

                _unitOfWork.SaveChanges();

                if (!pedidoVendaVolume.PedidoVenda.PedidoVendaVolumes.Any(pvv => pvv.IdPedidoVendaVolume != idPedidoVendaVolume && (pvv.IdPedidoVendaStatus == PedidoVendaStatusEnum.ProcessandoSeparacao || pvv.IdPedidoVendaStatus == PedidoVendaStatusEnum.SeparacaoConcluidaComSucesso)))
                {
                    pedidoVendaVolume.PedidoVenda.IdPedidoVendaStatus = novoStatusSeparacao;
                    pedidoVendaVolume.PedidoVenda.DataHoraInicioSeparacao = null;
                    pedidoVendaVolume.PedidoVenda.DataHoraFimSeparacao = null;

                    _unitOfWork.SaveChanges();
                }

                var gravarHistoricoColetorRequisicao = new GravarHistoricoColetorRequisicao
                {
                    IdColetorAplicacao = ColetorAplicacaoEnum.Separacao,
                    IdColetorHistoricoTipo = ColetorHistoricoTipoEnum.CancelamentoSeparacao,
                    Descricao = $"Cancelou a separação do volume {pedidoVendaVolume.NroVolume} do pedidoVenda {pedidoVendaVolume.IdPedidoVenda} com permissão do usuário {usuarioPermissaoCancelamento}",
                    IdEmpresa = idEmpresa,
                    IdUsuario = idUsuarioOperacao
                };

                _coletorHistoricoService.GravarHistoricoColetor(gravarHistoricoColetorRequisicao);

                transacao.Complete();
            }
        }

        public async Task IniciarSeparacaoPedidoVenda(long idPedidoVenda, string idUsuarioOperacao, long idEmpresa, long idPedidoVendaVolume)
        {
            // validações
            if (idPedidoVenda <= 0)
            {
                throw new BusinessException("Pedido deve ser informado.");
            }

            var pedidoVenda = _unitOfWork.PedidoVendaRepository.ObterPorIdPedidoVendaEIdEmpresa(idPedidoVenda, idEmpresa);

            if (pedidoVenda == null)
            {
                throw new BusinessException("Pedido não encontrado.");
            }

            if (pedidoVenda.IdEmpresa != idEmpresa)
            {
                throw new BusinessException("Pedido não pertence a empresa do usuário.");
            }

            if (pedidoVenda.IdPedidoVendaStatus != PedidoVendaStatusEnum.EnviadoSeparacao && pedidoVenda.IdPedidoVendaStatus != PedidoVendaStatusEnum.ProcessandoSeparacao)
            {
                throw new BusinessException("Pedido não liberado para separação.");
            }

            var atualizaPedidoVenda = pedidoVenda.IdPedidoVendaStatus == PedidoVendaStatusEnum.EnviadoSeparacao;

            var pedidoVendaVolume = pedidoVenda.PedidoVendaVolumes.First(f => f.IdPedidoVendaVolume == idPedidoVendaVolume);

            var atualizaPedidoVendaVolume = pedidoVendaVolume.IdPedidoVendaStatus == PedidoVendaStatusEnum.EnviadoSeparacao;

            if (atualizaPedidoVenda || atualizaPedidoVendaVolume)
            {
                var dataProcessamento = DateTime.Now;

                using (var transaction = _unitOfWork.CreateTransactionScope())
                {
                    if (atualizaPedidoVenda)
                    {
                        await _pedidoService.AtualizarStatusPedido(pedidoVenda.Pedido, PedidoVendaStatusEnum.ProcessandoSeparacao);

                        pedidoVenda.IdPedidoVendaStatus = PedidoVendaStatusEnum.ProcessandoSeparacao;
                        pedidoVenda.DataHoraInicioSeparacao = dataProcessamento;
                        pedidoVenda.Pedido.IdPedidoVendaStatus = PedidoVendaStatusEnum.ProcessandoSeparacao;

                        _unitOfWork.PedidoVendaRepository.Update(pedidoVenda);

                        _unitOfWork.SaveChanges();
                    }

                    if (atualizaPedidoVendaVolume)
                    {
                        pedidoVendaVolume.DataHoraInicioSeparacao = dataProcessamento;
                        pedidoVendaVolume.IdPedidoVendaStatus = PedidoVendaStatusEnum.ProcessandoSeparacao;
                        pedidoVendaVolume.IdUsuarioSeparacaoAndamento = idUsuarioOperacao;

                        _unitOfWork.PedidoVendaVolumeRepository.Update(pedidoVendaVolume);

                        _unitOfWork.SaveChanges();
                    }

                    transaction.Complete();
                }
            }
        }

        public async Task FinalizarSeparacaoVolume(long idPedidoVenda, long idPedidoVendaVolume, long idCaixa, string idUsuarioOperacao, long idEmpresa)
        {
            if (idPedidoVenda <= 0)
            {
                throw new BusinessException("O pedido deve ser informado.");
            }

            if (idPedidoVendaVolume <= 0)
            {
                throw new BusinessException("O volume deve ser informado.");
            }

            if (idCaixa <= 0)
            {
                throw new BusinessException("A caixa deve ser informada.");
            }

            var pedidoVenda = _unitOfWork.PedidoVendaRepository.GetById(idPedidoVenda);

            ValidarPedidoVendaEmSeparacao(pedidoVenda, idEmpresa);

            var pedidoVendaVolume = _unitOfWork.PedidoVendaVolumeRepository.GetById(idPedidoVendaVolume);

            ValidarPedidoVendaVolumeConcluidoCancelado(pedidoVendaVolume);

            if (pedidoVendaVolume.IdPedidoVendaStatus == PedidoVendaStatusEnum.EnviadoSeparacao)
            {
                throw new BusinessException("O volume informado ainda não está liberado para a separação.");
            }

            if (pedidoVendaVolume.PedidoVendaProdutos.Any(pedidoVendaProduto => pedidoVendaProduto.IdUsuarioAutorizacaoZerar.NullOrEmpty() && pedidoVendaProduto.QtdSeparada.GetValueOrDefault() < pedidoVendaProduto.QtdSeparar))
            {
                throw new BusinessException("Ainda existem itens a serem separados no pedido.");
            }

            var caixaSeparacao = _unitOfWork.CaixaRepository.BuscarCaixaAtivaPorEmpresa(idCaixa, idEmpresa, CaixaTipoEnum.Separacao);

            if (caixaSeparacao == null)
            {
                throw new BusinessException("Caixa de separação não encontrada.");
            }

            using (var transacao = _unitOfWork.CreateTransactionScope())
            {
                var novoStatusSeparacao = PedidoVendaStatusEnum.SeparacaoConcluidaComSucesso;
                var dataProcessamento = DateTime.Now;

                pedidoVendaVolume.IdPedidoVendaStatus = novoStatusSeparacao;
                pedidoVendaVolume.DataHoraFimSeparacao = dataProcessamento;
                pedidoVendaVolume.IdCaixaVolume = idCaixa;
                pedidoVendaVolume.IdUsuarioSeparacaoAndamento = null;

                _unitOfWork.SaveChanges();

                var todosProdutosVenda = _unitOfWork.PedidoVendaProdutoRepository.ObterPorIdPedidoVenda(idPedidoVenda);

                var finalizouPedidoVenda = false;

                if (!todosProdutosVenda.Any(produtoVendaProduto => produtoVendaProduto.IdUsuarioAutorizacaoZerar.NullOrEmpty() && produtoVendaProduto.QtdSeparada != produtoVendaProduto.QtdSeparar))
                {
                    pedidoVenda.IdPedidoVendaStatus = novoStatusSeparacao;
                    pedidoVenda.Pedido.IdPedidoVendaStatus = novoStatusSeparacao;
                    pedidoVenda.DataHoraFimSeparacao = dataProcessamento;

                    _unitOfWork.SaveChanges();

                    await AtualizarQtdConferidaIntegracao(pedidoVenda);

                    await _pedidoService.AtualizarStatusPedido(pedidoVenda.Pedido, novoStatusSeparacao);

                    finalizouPedidoVenda = true;
                }

                var gravarHistoricoColetorRequisicao = new GravarHistoricoColetorRequisicao
                {
                    IdColetorAplicacao = ColetorAplicacaoEnum.Separacao,
                    IdColetorHistoricoTipo = ColetorHistoricoTipoEnum.FinalizacaoSeparacao,
                    Descricao = $"Finalizou a separação do volume {idPedidoVendaVolume} {(finalizouPedidoVenda ? "e a separação " : "")}do pedido {idPedidoVenda} com a caixa {caixaSeparacao.Nome}",
                    IdEmpresa = idEmpresa,
                    IdUsuario = idUsuarioOperacao
                };

                _coletorHistoricoService.GravarHistoricoColetor(gravarHistoricoColetorRequisicao);

                transacao.Complete();
            }
        }

        public async Task<SalvarSeparacaoProdutoResposta> SalvarSeparacaoProduto(long idPedidoVendaVolume, long idProduto, long? idProdutoSeparacao, string idUsuario, long idEmpresa, int? quantidadeAjuste, bool temPermissaoF7, string idUsuarioAutorizacaoZerarPedido, bool temPermissaoF8)
        {
            var pedidoVendaVolume = _unitOfWork.PedidoVendaVolumeRepository.ObterPedidoVendaVolumePorIdPorEmpresa(idPedidoVendaVolume, idEmpresa);

            ValidarPedidoVendaVolumeConcluidoCancelado(pedidoVendaVolume);

            var pedidoVenda = pedidoVendaVolume.PedidoVenda;

            ValidarPedidoVendaEmSeparacao(pedidoVenda, idEmpresa);

            var pedidoVendaProduto = _unitOfWork.PedidoVendaProdutoRepository.ObterPorIdPedidoVendaVolumeEIdProduto(idPedidoVendaVolume, idProdutoSeparacao ?? idProduto);

            if (pedidoVendaProduto == null)
            {
                throw new BusinessException("O produto informado não faz parte do pedido em separação.");
            }

            ValidarPedidoVendaVolumeConcluidoCancelado(pedidoVendaVolume);

            if (pedidoVendaProduto.IdPedidoVendaStatus != PedidoVendaStatusEnum.PendenteSeparacao && pedidoVendaProduto.IdPedidoVendaStatus != PedidoVendaStatusEnum.EnviadoSeparacao && pedidoVendaProduto.IdPedidoVendaStatus != PedidoVendaStatusEnum.ProcessandoSeparacao)
            {
                throw new BusinessException("Produto com status inválido para separação.");
            }

            var salvarSeparacaoProdutoResposta = new SalvarSeparacaoProdutoResposta
            {
                IdPedidoVenda = pedidoVendaProduto.IdPedidoVenda,
                IdProduto = pedidoVendaProduto.IdProduto,
                Multiplo = pedidoVendaProduto.Produto.MultiploVenda,
                Referencia = pedidoVendaProduto.Produto.Referencia,
                QtdSeparar = pedidoVendaProduto.QtdSeparar,
            };

            if (pedidoVendaProduto.EnderecoArmazenagem.IsPicking && !temPermissaoF7)
            {
                throw new BusinessException("Você não tem permissão para separar pedidos fora do picking.");
            }

            var multiploProduto = pedidoVendaProduto.Produto.MultiploVenda;
            var quantidadeJaSeparada = pedidoVendaProduto.QtdSeparada.GetValueOrDefault();
            int qtdSeparada = quantidadeJaSeparada, quantidadeIncrementada = 0;

            var zerarPedido = !idUsuarioAutorizacaoZerarPedido.NullOrEmpty();
            var isPicking = !pedidoVendaProduto.IdLote.HasValue;

            if (zerarPedido == false)
            {
                if (idProdutoSeparacao == null)
                {
                    throw new BusinessException("Produto deve ser informado.");
                }

                if (idProduto != idProdutoSeparacao)
                {
                    throw new BusinessException("Produto informado inválido para separação.");
                }

                if (quantidadeAjuste.HasValue)
                {
                    if (quantidadeAjuste <= 0)
                    {
                        throw new BusinessException("Quantidade ajuste deve ser informada.");
                    }

                    if (quantidadeAjuste < multiploProduto || (quantidadeAjuste.Value % pedidoVendaProduto.Produto.MultiploVenda) != 0)
                    {
                        throw new BusinessException("Quantidade ajuste está fora do múltiplo.");
                    }

                    if (!isPicking && quantidadeAjuste != pedidoVendaProduto.QtdSeparar)
                    {
                        throw new BusinessException("Quantidade ajuste deve ser igual a quantidade total.");
                    }

                    quantidadeIncrementada = quantidadeAjuste.Value;
                }
                else if (!isPicking)
                {
                    throw new BusinessException("Só é permitida separação por F7 para itens fora do picking.");
                }
                else
                {
                    quantidadeIncrementada = (int)multiploProduto;
                }

                qtdSeparada += quantidadeIncrementada;

                if (qtdSeparada > pedidoVendaProduto.QtdSeparar)
                {
                    throw new BusinessException("A quantidade separada é maior que o pedido.");
                }
            }
            else if (!temPermissaoF8)
            {
                throw new BusinessException("Usuário informado não tem permissão para zerar.");
            }

            using (var transacao = _unitOfWork.CreateTransactionScope())
            {
                var dataProcessamento = DateTime.Now;

                if (quantidadeJaSeparada == 0)
                {
                    pedidoVendaProduto.DataHoraInicioSeparacao = dataProcessamento;
                    pedidoVendaProduto.IdUsuarioSeparacao = idUsuario;
                    pedidoVendaProduto.IdPedidoVendaStatus = PedidoVendaStatusEnum.ProcessandoSeparacao;
                }

                if (qtdSeparada == pedidoVendaProduto.QtdSeparar || zerarPedido)
                {
                    pedidoVendaProduto.DataHoraFimSeparacao = dataProcessamento;
                    pedidoVendaProduto.IdPedidoVendaStatus = PedidoVendaStatusEnum.SeparacaoConcluidaComSucesso;

                    salvarSeparacaoProdutoResposta.ProdutoSeparado = true;
                }

                pedidoVendaProduto.QtdSeparada = salvarSeparacaoProdutoResposta.QtdSeparada = qtdSeparada;

                if (salvarSeparacaoProdutoResposta.ProdutoSeparado)
                {
                    salvarSeparacaoProdutoResposta.VolumeSeparado = !pedidoVendaProduto.PedidoVendaVolume.PedidoVendaProdutos.Any(pvv => pvv.IdPedidoVendaProduto != pedidoVendaProduto.IdPedidoVendaProduto &&
                        (
                            pvv.IdUsuarioAutorizacaoZerar.NullOrEmpty() &&
                            pvv.QtdSeparada != pvv.QtdSeparar
                        )
                    );
                }

                if (zerarPedido)
                {
                    pedidoVendaProduto.IdUsuarioAutorizacaoZerar = idUsuarioAutorizacaoZerarPedido;
                    pedidoVendaProduto.DataHoraAutorizacaoZerarPedido = dataProcessamento;
                }

                _unitOfWork.PedidoVendaProdutoRepository.Update(pedidoVendaProduto);
                await _unitOfWork.SaveChangesAsync();

                var numeroVolume = pedidoVendaVolume.NroVolume.ToString().PadLeft(3, '0');
                var numeroPedido = pedidoVenda.Pedido.NroPedido;

                if (zerarPedido == false)
                {
                    await AjustarQuantidadeVolume(pedidoVendaProduto, quantidadeIncrementada * -1, idEmpresa, idUsuario);

                    if (quantidadeAjuste.HasValue)
                    {
                        var gravarHistoricoColetorRequisicao = new GravarHistoricoColetorRequisicao
                        {
                            IdColetorAplicacao = ColetorAplicacaoEnum.Separacao,
                            IdColetorHistoricoTipo = ColetorHistoricoTipoEnum.FinalizacaoSeparacao,
                            Descricao = $"Ajustou manualmente a quantidade do produto {pedidoVendaProduto.Produto.Referencia} na separação do volume {numeroVolume} no pedido {numeroPedido}, quantidade ajuste {quantidadeAjuste}, valor passou de {quantidadeJaSeparada} para {qtdSeparada}.",
                            IdEmpresa = idEmpresa,
                            IdUsuario = idUsuario
                        };

                        _coletorHistoricoService.GravarHistoricoColetor(gravarHistoricoColetorRequisicao);
                    }
                }
                else
                {
                    var gravarHistoricoColetorRequisicao = new GravarHistoricoColetorRequisicao
                    {
                        IdColetorAplicacao = ColetorAplicacaoEnum.Separacao,
                        IdColetorHistoricoTipo = ColetorHistoricoTipoEnum.FinalizacaoSeparacao,
                        Descricao = $"Zerou manualmente o produto {pedidoVendaProduto.Produto.Referencia} na separação do volume {numeroVolume} no pedido {numeroPedido}, quantidade permaneceu em {quantidadeJaSeparada}, usuário autorização {idUsuarioAutorizacaoZerarPedido}",
                        IdEmpresa = idEmpresa,
                        IdUsuario = idUsuario
                    };

                    _coletorHistoricoService.GravarHistoricoColetor(gravarHistoricoColetorRequisicao);
                }

                transacao.Complete();
            }

            return salvarSeparacaoProdutoResposta;
        }

        public void ValdarPedidoVendaVolume(PedidoVendaVolume pedidoVendaVolume)
        {
            if (pedidoVendaVolume == null)
            {
                throw new BusinessException("O Volume não foi encontrado.");
            }
        }

        private async Task AtualizarQtdConferidaIntegracao(PedidoVenda pedidoVenda)
        {
            if (!Convert.ToBoolean(ConfigurationManager.AppSettings["IntegracaoSankhya_Habilitar"]))
            {
                return;
            }

            var itensIntegracao = new List<PedidoItemIntegracao>();

            var vendaProdutos = pedidoVenda.PedidoVendaProdutos.Where(w => w.QtdSeparada.HasValue).GroupBy(g => g.IdProduto).ToDictionary(d => d.Key, d => d.ToList());

            foreach (var vendaProduto in vendaProdutos)
            {
                var pedidoItens = pedidoVenda.Pedido.PedidoItens.Where(w => w.IdProduto == vendaProduto.Key).OrderBy(o => o.Sequencia).ToList();

                var qtdSeparada = vendaProduto.Value.Sum(s => s.QtdSeparada).Value;

                if (pedidoItens.NullOrEmpty())
                {
                    throw new BusinessException("Não foi possível encontrar os itens da nota fiscal para atualizar o pedido no Sankhya.");
                }

                if (pedidoItens.Count() == 1)
                {
                    var itemIntegracao = new PedidoItemIntegracao()
                    {
                        QtdSeparada = qtdSeparada,
                        Sequencia = pedidoItens.First().Sequencia,
                        IdProduto = pedidoItens.First().IdProduto
                    };

                    itensIntegracao.Add(itemIntegracao);
                }
                else
                {
                    foreach (var item in pedidoItens)
                    {
                        int qtdAlocada = 0;
                        int qtdPendente = 0;

                        if (itensIntegracao.Any(s => s.IdProduto == item.IdProduto))
                        {
                            qtdAlocada = itensIntegracao.Where(s => s.IdProduto == item.IdProduto).Sum(s => s.QtdSeparada);
                        }

                        if (qtdSeparada > qtdAlocada)
                        {
                            qtdPendente = qtdSeparada - qtdAlocada;
                        }

                        var itemDevolucao = new PedidoItemIntegracao()
                        {
                            QtdSeparada = item.QtdPedido <= qtdPendente ? item.QtdPedido : qtdPendente,
                            Sequencia = item.Sequencia,
                            IdProduto = item.IdProduto
                        };

                        itensIntegracao.Add(itemDevolucao);

                        if (itensIntegracao.Where(s => s.IdProduto == item.IdProduto).Sum(s => s.QtdSeparada) == qtdSeparada)
                        {
                            break;
                        }
                    }
                }
            }

            foreach (var item in itensIntegracao)
            {
                try
                {
                    Dictionary<string, string> campoChave = new Dictionary<string, string> { { "NUNOTA", pedidoVenda.Pedido.CodigoIntegracao.ToString() }, { "SEQUENCIA", item.Sequencia.ToString() } };

                    await IntegracaoSankhya.Instance.AtualizarInformacaoIntegracao("ItemNota", campoChave, "QTDCONFERIDA", item.QtdSeparada);
                }
                catch (Exception ex)
                {
                    var errorMessage = string.Format($"Erro na atualização da quantidade conferida do pedido de venda: {pedidoVenda.Pedido.CodigoIntegracao}.");
                    _log.Error(errorMessage, ex);
                    throw new BusinessException(errorMessage);
                }
            }
        }

        public async Task DividirPedido(long idEmpresa)
        {
            if (idEmpresa == 0)
                throw new BusinessException("Empresa inválida");

            //Captura os corredores por empresa e ordena por corredor inicial.
            var grupoCorredorArmazenagem = _unitOfWork.GrupoCorredorArmazenagemRepository.Todos().Where(x => x.IdEmpresa == idEmpresa).OrderBy(x => x.CorredorInicial).ToList();

            //Captura os pedidos por empresa e status pendente separação.
            var listaPedidos = _unitOfWork.PedidoRepository.PesquisarPendenteSeparacao(idEmpresa);

            foreach (var pedido in listaPedidos) //Percorre a lista de pedidos.
            {
                try
                {
                    var idPedidoVenda = await _pedidoVendaService.Salvar(pedido);

                    if (idPedidoVenda == 0)
                        continue;

                    //Agrupa os itens do pedido por produto. 
                    var listaItensDoPedido = await AgruparItensDoPedidoPorProduto(pedido.IdPedido);

                    /*
                     * Usamos o foreach abaixo para capturar e atualizar o IdGrupoCorredorArmazenagem e IdEnderecoArmazenagem de cada item.
                     */
                    foreach (var pedidoItem in listaItensDoPedido)
                    {
                        //Captura o endereço de picking do produto.
                        //Posteriormente a lógica deverá ser alterada por ponto de separação.
                        var produtoEstoqueRepository = _unitOfWork.ProdutoEstoqueRepository.ObterPorProdutoEmpresaPicking(pedidoItem.Produto.IdProduto, idEmpresa);

                        if (produtoEstoqueRepository == null)
                            continue;

                        var enderecoArmazenagemProduto = new ProdutoEstoqueViewModel()
                        {
                            IdProduto = produtoEstoqueRepository.IdProduto,
                            IdEmpresa = produtoEstoqueRepository.IdEmpresa,
                            IdEnderecoArmazenagem = produtoEstoqueRepository.IdEnderecoArmazenagem,
                            Saldo = produtoEstoqueRepository.Saldo,
                            IdProdutoEstoqueStatus = produtoEstoqueRepository.IdProdutoEstoqueStatus,
                            EnderecoArmazenagem = produtoEstoqueRepository.EnderecoArmazenagem
                        };

                        //Captura o grupo de corredores do item do pedido.
                        var grupoCorredorArmazenagemItemPedido = await BuscarGrupoCorredorArmazenagemItemPedido(enderecoArmazenagemProduto.EnderecoArmazenagem.Corredor, grupoCorredorArmazenagem);

                        if (grupoCorredorArmazenagemItemPedido == null)
                            continue;

                        //Captura o indice do item na lista e atualizo os dados.
                        int index = listaItensDoPedido.IndexOf(pedidoItem);
                        listaItensDoPedido[index].GrupoCorredorArmazenagem = grupoCorredorArmazenagemItemPedido;
                        listaItensDoPedido[index].EnderecoSeparacao = enderecoArmazenagemProduto;
                    }

                    int quantidadeVolume = 0; //Variável utilizada para saber o número e a quantidade de volumes do pedido.

                    /*
                     * No foreach abaixo, capturamos quais e a quantidade de caixas (volumes) que serão utilizados.
                     * Além disso, através do método Cubicagem, saberemos a caixa de cada produto. 
                     * É importante saber que o processo é feito por corredor.
                     */
                    foreach (var itemCorredorArmazenagem in grupoCorredorArmazenagem)
                    {
                        //Captura o corredor do item.
                        var listaItensDoPedidoPorCorredor = listaItensDoPedido.Where(x => x.GrupoCorredorArmazenagem.IdGrupoCorredorArmazenagem == itemCorredorArmazenagem.IdGrupoCorredorArmazenagem).ToList();

                        //Se não houver nenhum item para o corredor, vai para o próximo.
                        if (listaItensDoPedidoPorCorredor.Count != 0)
                        {
                            //Captura os itens do pedido com as caixas em que cada um deve ir.
                            //A partir do método cubicagem, existem chamadas para vários outros.
                            var listaItensDoPedidoDividido = await Cubagem(listaItensDoPedidoPorCorredor, idEmpresa);

                            if (listaItensDoPedidoDividido.Count > 0)
                            {
                                //Busca os volumes que serão utilizados.
                                var listaVolumes = await BuscarCubagemVolumes(pedido.IdEmpresa, listaItensDoPedidoDividido);

                                foreach (var itemVolume in listaVolumes)
                                {
                                    quantidadeVolume++;

                                    var grupoCorredorItem = await BuscarGrupoCorredorArmazenagemItemPedido(itemVolume.ListaItensDoPedido[0].EnderecoSeparacao.EnderecoArmazenagem.Corredor, grupoCorredorArmazenagem);

                                    using (var transacao = _unitOfWork.CreateTransactionScope())
                                    {
                                        var pedidoVendaVolume = _pedidoVendaVolumeService.RetornarParaSalvar(idPedidoVenda, itemVolume.Caixa, grupoCorredorItem, quantidadeVolume, pedido.IdEmpresa, itemVolume.Peso, itemVolume.Cubagem);

                                        var pedidoVendaProdutos = new List<PedidoVendaProduto>();

                                        foreach (var item in itemVolume.ListaItensDoPedido)
                                        {
                                            pedidoVendaProdutos.Add(new PedidoVendaProduto()
                                            {
                                                IdPedidoVenda = idPedidoVenda,
                                                IdProduto = item.Produto.IdProduto,
                                                IdEnderecoArmazenagem = item.EnderecoSeparacao.IdEnderecoArmazenagem.Value,
                                                IdPedidoVendaStatus = PedidoVendaStatusEnum.EnviadoSeparacao,
                                                QtdSeparar = item.Quantidade,
                                                QtdSeparada = null,
                                                CubagemProduto = item.Produto.CubagemProduto.Value,
                                                PesoProduto = item.Produto.PesoBruto,
                                                DataHoraInicioSeparacao = null,
                                                DataHoraFimSeparacao = null,
                                                IdLote = item.IdLote
                                            });
                                        }

                                        pedidoVendaVolume.PedidoVendaProdutos = pedidoVendaProdutos;

                                        _unitOfWork.PedidoVendaVolumeRepository.Add(pedidoVendaVolume);

                                        //Captura o primeiro corredor de separação.
                                        int corredorInicioSeparacao = listaItensDoPedidoDividido.Min(x => x.EnderecoSeparacao.EnderecoArmazenagem.Corredor);

                                        //Atualiza a quantidade de volumes na PedidoVenda.
                                        var pedidoVenda = _unitOfWork.PedidoVendaRepository.GetById(idPedidoVenda);

                                        pedidoVenda.NroVolumes = pedidoVenda.NroVolumes + 1;

                                        _unitOfWork.PedidoVendaRepository.Update(pedidoVenda);
                                        //await _pedidoVendaService.AtualizarQuantidadeVolume(idPedidoVenda, quantidadeVolume);

                                        _unitOfWork.SaveChanges();

                                        //throw new Exception("");

                                        //Imprime a etiqueta de separação.
                                        await ImprimirEtiquetaVolumeSeparacao(itemVolume, quantidadeVolume, grupoCorredorItem, pedido, pedidoVendaVolume.IdPedidoVendaVolume, corredorInicioSeparacao);

                                        transacao.Complete();
                                    }
                                }
                            }
                        }
                    }

                    //Atualiza o status do PedidoVenda para Enviado Separação.
                    await _pedidoVendaService.AtualizarStatus(idPedidoVenda, PedidoVendaStatusEnum.EnviadoSeparacao);

                    //Atualiza o status do Pedido para Enviado Separação.
                    await _pedidoService.AtualizarStatus(pedido.IdPedido, PedidoVendaStatusEnum.EnviadoSeparacao);
                }
                catch (Exception exception)
                {
                    _log.Error($"Erro dividindo pedido id {pedido.IdPedido} para separação", exception);
                }
            }
        }

        public async Task ImprimirEtiquetaVolumeSeparacao(VolumeViewModel volume, int numeroVolume, GrupoCorredorArmazenagemViewModel grupoCorredorArmazenagem, Pedido pedido, long idPedidoVendaVolume, int corredorInicioSeparacao)
        {
            var pedidoVendaVolume = _unitOfWork.PedidoVendaVolumeRepository.GetById(idPedidoVendaVolume);

            _etiquetaService.ImprimirEtiquetaVolumeSeparacao(new ImprimirEtiquetaVolumeSeparacaoRequest()
            {
                ClienteNomeFantasia = pedido.Cliente.NomeFantasia,
                ClienteEndereco = pedido.Cliente.Endereco,
                ClienteEnderecoNumero = pedido.Cliente.Numero,
                ClienteCEP = pedido.Cliente.CEP,
                ClienteCidade = pedido.Cliente.Cidade,
                ClienteUF = pedido.Cliente.UF,
                ClienteTelefone = pedido.Cliente.Telefone,
                ClienteCodigo = pedido.Cliente.IdCliente.ToString(),
                RepresentanteCodigo = pedido.Representante.IdRepresentante.ToString(),
                PedidoCodigo = pedido.NroPedido.ToString(),
                PedidoDataCriacao = pedido.DataCriacao,
                PedidoIsRequisicao = pedido.IsRequisicao,
                PedidoPagamentoCodigoIntegracao = pedido.PagamentoCodigoIntegracao,
                PedidoPagamentoIsDebito = pedido.PagamentoIsDebitoIntegracao,
                PedidoPagamentoIsCredito = pedido.PagamentoIsCreditoIntegracao,
                PedidoPagamentoIsDinheiro = pedido.PagamentoIsDinheiroIntegracao,
                Centena = pedidoVendaVolume.NroCentena.ToString(),
                TransportadoraSigla = pedido.Transportadora.CodigoTransportadora,
                TransportadoraCodigo = pedido.Transportadora.IdTransportadora.ToString(),
                TransportadoraNome = pedido.Transportadora.RazaoSocial,
                CorredoresInicio = grupoCorredorArmazenagem.CorredorInicial.ToString(),
                CorredoresFim = grupoCorredorArmazenagem.CorredorFinal.ToString(),
                CaixaTextoEtiqueta = volume.Caixa.TextoEtiqueta,
                Volume = numeroVolume.ToString(),
                IdImpressora = grupoCorredorArmazenagem.IdImpressora
            },
            pedido.IdEmpresa);
        }

        public async Task<GrupoCorredorArmazenagemViewModel> BuscarGrupoCorredorArmazenagemItemPedido(int corredor, List<GrupoCorredorArmazenagem> listaGrupoCorredorArmazenagem)
        {
            GrupoCorredorArmazenagemViewModel grupoArmazenagem = null;

            foreach (var item in listaGrupoCorredorArmazenagem)
            {
                if (corredor >= item.CorredorInicial && corredor <= item.CorredorFinal)
                {
                    grupoArmazenagem = new GrupoCorredorArmazenagemViewModel()
                    {
                        IdGrupoCorredorArmazenagem = item.IdGrupoCorredorArmazenagem,
                        IdPontoArmazenagem = item.IdPontoArmazenagem,
                        IdEmpresa = item.IdEmpresa,
                        IdImpressora = item.IdImpressora,
                        CorredorInicial = item.CorredorInicial,
                        CorredorFinal = item.CorredorFinal,
                        Ativo = item.Ativo
                    };

                    return grupoArmazenagem;
                }
            }

            return grupoArmazenagem;
        }

        public async Task<List<PedidoItemViewModel>> AgruparItensDoPedidoPorProduto(long idPedido)
        {
            List<PedidoItemViewModel> listaItensDoPedidoAgrupada = new List<PedidoItemViewModel>();

            //Captura os itens do pedido.
            var listaItensDoPedido = _unitOfWork.PedidoItemRepository.BuscarPorIdPedido(idPedido);

            //Devido a mais de uma linha do mesmo produto, fazemos o agrupamento por produto.
            listaItensDoPedidoAgrupada = listaItensDoPedido.GroupBy(x => new { x.IdPedido, x.Produto }).Select(s => new PedidoItemViewModel()
            {
                Produto = new ProdutoViewModel()
                {
                    IdProduto = s.Key.Produto.IdProduto,
                    IdUnidadeMedida = s.Key.Produto.IdUnidadeMedida,
                    Descricao = s.Key.Produto.Descricao,
                    Referencia = s.Key.Produto.Referencia,
                    PesoBruto = s.Key.Produto.PesoBruto,
                    PesoLiquido = s.Key.Produto.PesoLiquido,
                    Largura = s.Key.Produto.Largura,
                    Altura = s.Key.Produto.Altura,
                    Comprimento = s.Key.Produto.Comprimento,
                    MetroCubico = s.Key.Produto.MetroCubico,
                    MultiploVenda = s.Key.Produto.MultiploVenda,
                    Ativo = s.Key.Produto.Ativo
                },
                Quantidade = s.Sum(x => x.QtdPedido)
            }).ToList();

            return listaItensDoPedidoAgrupada;
        }

        public async Task<List<PedidoItemViewModel>> Cubagem(List<PedidoItemViewModel> listaItensDoPedido, long idEmpresa) //Cubicagem
        {
            try
            {
                List<PedidoItemViewModel> listaItensDoPedidoRetorno = new List<PedidoItemViewModel>();

                List<CaixaViewModel> listaCaixas = new List<CaixaViewModel>();

                //Lista das caixas ATIVAS cadastradas no sistema.
                var caixaRepository = _unitOfWork.CaixaRepository.BuscarTodos(idEmpresa).Where(x => x.Ativo == true).ToList();

                foreach (var item in caixaRepository)
                {
                    listaCaixas.Add(new CaixaViewModel()
                    {
                        IdCaixa = item.IdCaixa,
                        IdCaixaTipo = item.IdCaixaTipo,
                        Nome = item.Nome,
                        TextoEtiqueta = item.TextoEtiqueta,
                        Largura = item.Largura,
                        Altura = item.Altura,
                        Comprimento = item.Comprimento,
                        Cubagem = item.Cubagem,
                        PesoCaixa = item.PesoCaixa,
                        PesoMaximo = item.PesoMaximo,
                        Sobra = item.Sobra,
                        Ativo = item.Ativo
                    });
                }

                //Inicializa a lista do ranking de caixas.
                listaRankingCaixas = new List<CaixaViewModel>();

                listaItensDoPedidoRetorno = await BuscarPesoCubagemItensPedido(listaItensDoPedido, listaCaixas, idEmpresa);

                if (listaItensDoPedidoRetorno.Count > 0)
                {
                    listaItensDoPedidoRetorno = await CalcularCubagemVolume(listaItensDoPedidoRetorno);

                    //Agrupa os itens por caixa (removido) e agrupamento.
                    listaItensDoPedidoRetorno = listaItensDoPedidoRetorno.OrderBy(x => x.Agrupador).ToList();
                }

                return listaItensDoPedidoRetorno;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Este método retorna as informações que serão utilizadas na cubicagem dos volumes.
        /// Através da listaPedidoAgrupadaPorProduto o método sabe quais os itens que serão cubicados.
        /// </summary>
        /// <param name="idPedido"></param>
        /// <param name="listaPedidoAgrupado"></param>
        /// <param name="listaEnderecosSeparacao"></param>
        /// <returns></returns>
        public async Task<List<PedidoItemViewModel>> BuscarPesoCubagemItensPedido(List<PedidoItemViewModel> listaItensDoPedido, List<CaixaViewModel> listaCaixas, long idEmpresa) //getInfoIt
        {
            var listaItensDoPedidoCubicados = new List<PedidoItemViewModel>();

            for (int i = 0; i < listaItensDoPedido.Count; i++)
            {
                var produtosSeparados = await BuscarLocalProdutoSeparacao(listaItensDoPedido[i], idEmpresa);

                decimal? larguraProduto = listaItensDoPedido[i].Produto.Largura;
                decimal? comprimentoProduto = listaItensDoPedido[i].Produto.Comprimento;
                decimal? alturaProduto = listaItensDoPedido[i].Produto.Altura;

                var caixasQuePodemSerUsadas = await CaixasQuePodemSerUtilizadas(listaItensDoPedido[i].Produto, listaCaixas, idEmpresa);

                foreach (var item in produtosSeparados)
                {
                    listaItensDoPedidoCubicados.Add(new PedidoItemViewModel()
                    {
                        Produto = new ProdutoViewModel()
                        {
                            IdProduto = listaItensDoPedido[i].Produto.IdProduto,
                            IdEnderecoArmazenagem = item.EnderecoSeparacao.IdEnderecoArmazenagem,
                            CodigoEnderecoArmazenagem = item.EnderecoSeparacao.EnderecoArmazenagem.Codigo,
                            CubagemProduto = (larguraProduto * comprimentoProduto) * alturaProduto,
                            PesoBruto = listaItensDoPedido[i].Produto.PesoBruto,
                            MultiploVenda = listaItensDoPedido[i].Produto.MultiploVenda,
                            Altura = alturaProduto,
                            Largura = larguraProduto,
                            Comprimento = comprimentoProduto,
                            IsEmbalagemFornecedor = listaItensDoPedido[i].Produto.IsEmbalagemFornecedor,
                            IsEmbalagemFornecedorVolume = listaItensDoPedido[i].Produto.IsEmbalagemFornecedorVolume,
                        },
                        Agrupador = 0,
                        CaixaEscolhida = null,
                        Quantidade = item.Quantidade,
                        Caixa = caixasQuePodemSerUsadas,
                        EnderecoSeparacao = item.EnderecoSeparacao,
                        GrupoCorredorArmazenagem = listaItensDoPedido[i].GrupoCorredorArmazenagem,
                        IsSeparacaoNoPikcing = item.IsSeparacaoNoPikcing,
                        IdLote = item.IdLote
                    });
                }
            }

            //Classifica a listaRankingCaixas por Quantidade
            listaRankingCaixas = listaRankingCaixas.OrderByDescending(x => x.QuantidadeRanking).ToList();

            return listaItensDoPedidoCubicados;
        }

        public async Task<List<LoteProdutoEnderecoViewModel>> BuscarLocalProdutoSeparacao(PedidoItemViewModel item, long idEmpresa)
        {
            int quantidadePedido = item.Quantidade;
            var enderecoPicking = _unitOfWork.ProdutoEstoqueRepository.ObterPorProdutoEmpresaPicking(item.Produto.IdProduto, idEmpresa);
            LoteProdutoEndereco pontoSeparacaoEscolhido;
            bool encontrou = false;
            List<LoteProdutoEnderecoViewModel> listaProdutoEndereco = new List<LoteProdutoEnderecoViewModel>();

            //Aqui é feito uma verificação para ignorar itens que estão sem localização.
            //O motivo é que foi dito na reunião com Veronezzi e Beatriz que a chance disso acontecer é pequena, pois ao cadastrar um produto será informado um endereço de picking para ele.
            //Mantive a verificação comentada pois, se tiver algum erro perante a isso basta descomentar.
            //if (enderecoPicking == null)
            //    continue;

            var listaEnderecoSeparacao = _unitOfWork.LoteProdutoEnderecoRepository.PesquisarPorProdutoComLote(item.Produto.IdProduto, idEmpresa)
                .Where(x => x.Quantidade > 0).OrderBy(x => x.DataHoraInstalacao).ToList();

            //Verifica se o endereço de Picking é controlado por FIFO.
            if (!enderecoPicking.EnderecoArmazenagem.IsFifo)
            {
                //Primeiro caso: se a quantidade do pedido for igual a quantidade de pelo menos um endereço de separação.
                if (listaEnderecoSeparacao.Any(x => quantidadePedido == (x.Quantidade / x.QuantidadeCaixas)))
                {
                    pontoSeparacaoEscolhido = listaEnderecoSeparacao.Where(x => x.Quantidade == quantidadePedido).OrderBy(o => o.DataHoraInstalacao).FirstOrDefault();

                    if (pontoSeparacaoEscolhido != null)
                    {
                        listaProdutoEndereco.Add(new LoteProdutoEnderecoViewModel()
                        {
                            Quantidade = quantidadePedido,
                            IdLote = pontoSeparacaoEscolhido.IdLote,
                            EnderecoSeparacao = new ProdutoEstoqueViewModel()
                            {
                                EnderecoArmazenagem = pontoSeparacaoEscolhido.EnderecoArmazenagem,
                                IdEmpresa = pontoSeparacaoEscolhido.IdEmpresa,
                                IdEnderecoArmazenagem = pontoSeparacaoEscolhido.IdEnderecoArmazenagem,
                                IdProduto = pontoSeparacaoEscolhido.IdProduto
                            },
                            IsSeparacaoNoPikcing = false
                        });

                        encontrou = true;
                    }
                }

                List<LoteProdutoEnderecoViewModel> listaQuantidadeEnderecoSeparacao = new List<LoteProdutoEnderecoViewModel>();

                foreach (var pontoSeparacao in listaEnderecoSeparacao)
                {
                    int quantidade = (pontoSeparacao.Quantidade / pontoSeparacao.QuantidadeCaixas).Value;

                    for (int i = 0; i < pontoSeparacao.QuantidadeCaixas; i++)
                    {
                        listaQuantidadeEnderecoSeparacao.Add(new LoteProdutoEnderecoViewModel()
                        {
                            Id = i,
                            IdLote = pontoSeparacao.IdLote,
                            EnderecoSeparacao = new ProdutoEstoqueViewModel()
                            {
                                EnderecoArmazenagem = pontoSeparacao.EnderecoArmazenagem,
                                IdEmpresa = pontoSeparacao.IdEmpresa,
                                IdEnderecoArmazenagem = pontoSeparacao.IdEnderecoArmazenagem,
                                IdProduto = pontoSeparacao.IdProduto
                            },
                            Quantidade = quantidade
                        });
                    }
                }

                //Segundo caso: se a quantidade do pedido for igual a SOMA das quantidades dos endereços separação.
                if (encontrou == false)
                {
                    List<string> listaCombinacoes = new List<string>();

                    foreach (string s in ObterCombinacoes(listaQuantidadeEnderecoSeparacao, quantidadePedido, ""))
                    {
                        listaCombinacoes.Add(s);
                    }

                    if (listaCombinacoes.Count > 0)
                    {
                        string[] idsPontosEscolhidos = listaCombinacoes[0].Split(',');

                        for (int i = 0; i < idsPontosEscolhidos.Length - 1; i++)
                        {
                            var pontoEscolhido = listaQuantidadeEnderecoSeparacao[Convert.ToInt32(idsPontosEscolhidos[i])];

                            listaProdutoEndereco.Add(new LoteProdutoEnderecoViewModel()
                            {
                                Quantidade = pontoEscolhido.Quantidade,
                                IdLote = pontoEscolhido.IdLote,
                                EnderecoSeparacao = pontoEscolhido.EnderecoSeparacao,
                                IsSeparacaoNoPikcing = false
                            });
                        }

                        encontrou = true;
                    }
                }

                //Terceiro caso: se a quantidade do pedido for diferente da soma de N endereços, captura o maior número de endereços de separação.
                if (encontrou == false)
                {
                    List<string> listaCombinacoes = new List<string>();

                    for (int i = quantidadePedido; i > 0; i--)
                    {
                        foreach (string s in ObterCombinacoes(listaQuantidadeEnderecoSeparacao, i, ""))
                        {
                            listaCombinacoes.Add(s);
                        }

                        if (listaCombinacoes.Count > 0)
                            break;
                    }

                    if (listaCombinacoes.Count > 0)
                    {
                        string[] idsPontosEscolhidos = listaCombinacoes[0].Split(',');

                        for (int i = 0; i < idsPontosEscolhidos.Length - 1; i++)
                        {
                            var pontoEscolhido = listaQuantidadeEnderecoSeparacao[Convert.ToInt32(idsPontosEscolhidos[i])];

                            listaProdutoEndereco.Add(new LoteProdutoEnderecoViewModel()
                            {
                                Quantidade = pontoEscolhido.Quantidade,
                                IdLote = pontoEscolhido.IdLote,
                                EnderecoSeparacao = pontoEscolhido.EnderecoSeparacao,
                                IsSeparacaoNoPikcing = false
                            });
                        }

                        encontrou = true;
                    }
                }

                var quantidadeFaltante = quantidadePedido - listaProdutoEndereco.Sum(x => x.Quantidade);

                //Captura o restante do picking.
                if (quantidadeFaltante > 0)
                {
                    listaProdutoEndereco.Add(new LoteProdutoEnderecoViewModel()
                    {
                        Quantidade = quantidadeFaltante,
                        IdLote = null,
                        EnderecoSeparacao = new ProdutoEstoqueViewModel()
                        {
                            EnderecoArmazenagem = enderecoPicking.EnderecoArmazenagem,
                            IdEmpresa = enderecoPicking.IdEmpresa,
                            IdEnderecoArmazenagem = enderecoPicking.IdEnderecoArmazenagem,
                            IdProduto = enderecoPicking.IdProduto,
                            IdProdutoEstoqueStatus = enderecoPicking.IdProdutoEstoqueStatus,
                            Saldo = enderecoPicking.Saldo
                        },
                        IsSeparacaoNoPikcing = true
                    });
                }
            }
            else
            {
                //Primeiro caso: se a quantidade do pedido for igual a quantidade de pelo menos um endereço de separação.
                if (listaEnderecoSeparacao.Any(x => x.Quantidade == quantidadePedido))
                {
                    pontoSeparacaoEscolhido = listaEnderecoSeparacao.Where(x => x.Quantidade == quantidadePedido).OrderBy(o => o.DataHoraInstalacao).FirstOrDefault();

                    if (pontoSeparacaoEscolhido != null)
                    {
                        listaProdutoEndereco.Add(new LoteProdutoEnderecoViewModel()
                        {
                            Quantidade = quantidadePedido,
                            IdLote = pontoSeparacaoEscolhido.IdLote,
                            EnderecoSeparacao = new ProdutoEstoqueViewModel()
                            {
                                EnderecoArmazenagem = pontoSeparacaoEscolhido.EnderecoArmazenagem,
                                IdEmpresa = pontoSeparacaoEscolhido.IdEmpresa,
                                IdEnderecoArmazenagem = pontoSeparacaoEscolhido.IdEnderecoArmazenagem,
                                IdProduto = pontoSeparacaoEscolhido.IdProduto
                            },
                            IsSeparacaoNoPikcing = false
                        });
                    }
                }
                else
                {
                    listaProdutoEndereco.Add(new LoteProdutoEnderecoViewModel()
                    {
                        Quantidade = quantidadePedido,
                        IdLote = null,
                        EnderecoSeparacao = new ProdutoEstoqueViewModel()
                        {
                            EnderecoArmazenagem = enderecoPicking.EnderecoArmazenagem,
                            IdEmpresa = enderecoPicking.IdEmpresa,
                            IdEnderecoArmazenagem = enderecoPicking.IdEnderecoArmazenagem,
                            IdProduto = enderecoPicking.IdProduto,
                            IdProdutoEstoqueStatus = enderecoPicking.IdProdutoEstoqueStatus,
                            Saldo = enderecoPicking.Saldo
                        },
                        IsSeparacaoNoPikcing = true
                    });
                }
            }

            return listaProdutoEndereco;
        }

        public IEnumerable<string> ObterCombinacoes(List<LoteProdutoEnderecoViewModel> dados, int quantidadePedido, string values)
        {
            for (int i = 0; i < dados.Count; i++)
            {
                int resto = quantidadePedido - dados[i].Quantidade;
                string vals = dados[i].Id + "," + values;
                if (resto == 0)
                {
                    yield return vals;
                }
                else
                {
                    List<LoteProdutoEnderecoViewModel> dadosPossiveis = dados.Take(i).Where(n => n.Quantidade <= quantidadePedido).ToList();
                    if (dadosPossiveis.Count > 0)
                    {
                        foreach (string s in ObterCombinacoes(dadosPossiveis, resto, vals))
                        {
                            yield return s;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Analisa as caixas de recusa e inverte o resultado para agregar aos itens as caixas que PODEM ser utilizadas
        /// </summary>
        /// <param name="referencia"></param>
        /// <param name="ListaCaixas"></param>
        /// <param name="LarguraAlturaComprimento"></param>
        /// <returns></returns>
        public async Task<List<CaixaViewModel>> CaixasQuePodemSerUtilizadas(ProdutoViewModel produto, List<CaixaViewModel> listaCaixas, long idEmpresa) //getCaixasOK
        {
            List<CaixaViewModel> listaCaixasQuePodemSerUtilizadas = new List<CaixaViewModel>();

            var caixasRecusa = _unitOfWork.CaixaRecusaRepository.BuscarPorEmpresaProduto(idEmpresa, produto.IdProduto).GroupBy(x => x.IdCaixa);

            foreach (var item in caixasRecusa)
            {
                var caixa = listaCaixas.Where(x => x.IdCaixa == item.Key).FirstOrDefault();

                if (caixa != null)
                    listaCaixas.Remove(caixa);
            }

            for (int i = 0; i < listaCaixas.Count; i++)
            {
                //Valida a cubagem entre a caixa e o produto.
                if (await CalcularCubagemEntreCaixaProduto(produto, listaCaixas[i]))
                {
                    //Se passou na validação acima, adiciona a caixa na listaCaixasQuePodemSerUtilizadas.
                    listaCaixasQuePodemSerUtilizadas.Add(listaCaixas[i]);

                    var caixaRanking = listaRankingCaixas.FirstOrDefault(x => x.IdCaixa == listaCaixas[i].IdCaixa);

                    /*
                     * Verifica se a caixa existe no ranking.
                     * Se existir, atualiza o campo quantidadeRanking, ou seja, mais uma caixa que pode ser usada pelo item.
                     * Caso contrário cadastra a caixa na listaRankingCaixas.
                     */

                    if (caixaRanking != null)
                    {
                        int index = listaRankingCaixas.IndexOf(caixaRanking);
                        listaRankingCaixas[index].QuantidadeRanking++;
                    }
                    else
                    {
                        listaCaixas[i].QuantidadeRanking = 1;
                        listaRankingCaixas.Add(listaCaixas[i]);
                    }
                }
            }

            return listaCaixasQuePodemSerUtilizadas;
        }

        /// <summary>
        /// Método que cruza as informações entre a cubagem da caixa e do item do pedido.
        /// Serão utilizados os valores altura, largura e comprimento
        /// </summary>
        /// <param name="produto"></param>
        /// <param name="caixa"></param>
        /// <returns></returns>
        public async Task<bool> CalcularCubagemEntreCaixaProduto(ProdutoViewModel produto, CaixaViewModel caixa) //chkCubIt2
        {
            bool retorno = false;

            //Valida se a largura, comprimento e peso bruto do produto é diferente de 0.
            if (produto.Largura.HasValue == false || produto.Largura == 0 || produto.Comprimento.HasValue == false || produto.Comprimento == 0 || produto.PesoBruto == 0)
                return false;

            //Valida se a largura, comprimento e e peso bruto do produto é menor os valores da caixa.
            if (produto.Altura <= caixa.Altura && produto.Largura <= caixa.Largura && produto.Comprimento <= caixa.Comprimento)
                retorno = true;

            return retorno;
        }

        /// <summary>
        /// O método abaixo rotina faz o cálculo da cubagem dos volumes.
        /// </summary>
        /// <param name="listaItensDoPedido"></param>
        /// <returns></returns>
        public async Task<List<PedidoItemViewModel>> CalcularCubagemVolume(List<PedidoItemViewModel> listaItensDoPedido) //calcCub
        {
            try
            {
                //Declaração das variáveis que serão utilizadas.
                bool caixaAnteriorEhMelhorQueAtual = false; //Indica que a caixa anterior é melhor que a atual.
                bool encontrouCaixaCorreta = false; //Indica que a caixa correta foi encontrada.
                bool usarCaixaEncontrada = false; //Indica que os itens receberão a caixa encontrada.
                CaixaViewModel caixaMaior = null; //Caixa corrente.
                CaixaViewModel caixaAnterior = null; //Controle da caixa anterior.
                CaixaViewModel proximaCaixa = null; //Próxima caixa na escala de grandeza.
                decimal contadorCubagem2 = 0;
                bool usouAgrupamento; //Indica se o agrupamento foi utilizado.
                decimal contadorCubagem; //Acumulador (auxiliar) para cubagem.
                decimal contadorPeso; //Acumulador (auxiliar) para peso.
                bool sair = false;

                //Captura a lista de caixas mais comum.
                var listaCaixasMaisComum = await BuscarCaixaMaisComum(listaItensDoPedido);

                //Valida se a lista de caixas mais comum é nula.
                if (listaCaixasMaisComum == null)
                    return listaItensDoPedido;

                int agrupador = 1;

                for (int i = 0; i < listaItensDoPedido.Count; i++)
                {
                    //Para cada item do pedido, se não existir caixa ou for embalagem do fornecedor, vai adicionando o agrupador.
                    if ((listaItensDoPedido[i].Agrupador == 0 && listaItensDoPedido[i].Caixa == null) || listaItensDoPedido[i].Produto.IsEmbalagemFornecedorVolume || listaItensDoPedido[i].IsSeparacaoNoPikcing == false)
                    {
                        listaItensDoPedido[i].Agrupador = agrupador;
                        agrupador++;
                    }
                }

                caixaMaior = await BuscarMaiorCaixa(listaCaixasMaisComum);

                do
                {
                    usouAgrupamento = false;

                    contadorCubagem = 0;
                    contadorPeso = 0;

                    //Primeira triagem dos itens - não há consideração pelos multiplos
                    for (int i = 0; i < listaItensDoPedido.Count; i++)
                    {
                        /*
                         * Se o item já estiver agrupado ou as caixas do item for nulo 
                         * ou se o item for um volume do fornecedor (IsEmbalagemFornecedorVolume).
                         */
                        if (listaItensDoPedido[i].Agrupador != 0 || listaItensDoPedido[i].Caixa == null || listaItensDoPedido[i].Produto.IsEmbalagemFornecedorVolume || listaItensDoPedido[i].IsSeparacaoNoPikcing == false)
                            continue;

                        //Verifica se o item cabe na caixa indicada.
                        if (!await CalcularCubagemEntreCaixaProduto(listaItensDoPedido[i].Produto, caixaMaior))
                            continue;

                        //Calcula a cubagem do item (produto) do pedido.
                        var cubagemPedidoItem = await CalcularCubagemPedidoItem(listaItensDoPedido[i]);

                        //Calcula o peso do item (produto) do pedido.
                        var pesoPedidoItem = await CalcularPesoItemPedido(listaItensDoPedido[i]);
                        decimal valor = 1.05M; //Variavel utilizada no calculo da sobra da caixa. Existe uma margem de 5% por conta do plástico bolha.

                        //Verifica se a cubagem total (contadorCubagem + cubagemPedidoItem) é menor ou igual a cubagem da caixa.
                        //Verifica se o peso total (contadorCubagem + cubagemPedidoItem) é menor ou igual ao peso maximo da caixa.
                        if ((contadorCubagem + cubagemPedidoItem.Value) <= (caixaMaior.Cubagem * ((100 - caixaMaior.Sobra) / 100)) &&
                            (contadorPeso + pesoPedidoItem.Value) <= (caixaMaior.PesoMaximo * valor))
                        {
                            //Verifica se a caixa identificada está na lista de caixas dos itens do pedido;
                            if (listaItensDoPedido[i].Caixa.Any(x => x == caixaMaior))
                            {
                                //Soma a cubagem do item do pedido ao contador.
                                contadorCubagem += cubagemPedidoItem.Value;

                                //Soma o peso do item do pedido ao contador.
                                contadorPeso += pesoPedidoItem.Value;

                                listaItensDoPedido[i].Agrupador = agrupador;
                                usouAgrupamento = true;
                            }
                        }
                    }

                    usarCaixaEncontrada = true;

                    if (!encontrouCaixaCorreta)
                    {
                        //Analisa a sobra da caixa
                        if (contadorCubagem < (caixaMaior.Cubagem * ((100 - caixaMaior.Sobra) / 100)))
                        {
                            if (caixaAnteriorEhMelhorQueAtual && contadorCubagem != contadorCubagem2)
                            {
                                encontrouCaixaCorreta = true;

                                caixaMaior = caixaAnterior;
                                caixaAnteriorEhMelhorQueAtual = false;

                                await ZerarAgrupamento(agrupador, listaItensDoPedido);

                                usarCaixaEncontrada = false;
                                usouAgrupamento = true;
                            }
                            else
                            {
                                proximaCaixa = await CalcularProximaCaixaMaior(caixaMaior, listaCaixasMaisComum);

                                if (caixaMaior == proximaCaixa)
                                    encontrouCaixaCorreta = true;

                                caixaAnterior = caixaMaior;

                                if (proximaCaixa != null)
                                    caixaMaior = proximaCaixa;
                                else
                                {
                                    caixaMaior = caixaAnterior;
                                    encontrouCaixaCorreta = true;
                                }

                                contadorCubagem2 = contadorCubagem;
                                caixaAnteriorEhMelhorQueAtual = true;

                                await ZerarAgrupamento(agrupador, listaItensDoPedido);

                                usarCaixaEncontrada = false;
                                usouAgrupamento = true;
                            }
                        }
                        else
                        {
                            proximaCaixa = await CalcularProximaCaixaMaior(caixaMaior, listaCaixasMaisComum);

                            if (caixaMaior == proximaCaixa)
                                encontrouCaixaCorreta = true;

                            caixaAnterior = caixaMaior;

                            if (proximaCaixa != null)
                                caixaMaior = proximaCaixa;
                            else
                            {
                                caixaMaior = caixaAnterior;
                                encontrouCaixaCorreta = true;
                            }

                            contadorCubagem2 = contadorCubagem;
                            caixaAnteriorEhMelhorQueAtual = true;

                            await ZerarAgrupamento(agrupador, listaItensDoPedido);

                            usarCaixaEncontrada = false;
                            usouAgrupamento = true;
                        }
                    }

                    if (usarCaixaEncontrada)
                    {
                        if (contadorCubagem > 0)
                        {
                            for (int i = 0; i < listaItensDoPedido.Count; i++)
                            {
                                if (listaItensDoPedido[i].Agrupador == 0 || listaItensDoPedido[i].Caixa == null || listaItensDoPedido[i].Produto.IsEmbalagemFornecedorVolume || listaItensDoPedido[i].IsSeparacaoNoPikcing == false)
                                    continue;

                                if (listaItensDoPedido[i].Agrupador == agrupador)
                                {
                                    listaItensDoPedido[i].CaixaEscolhida = caixaMaior;

                                    if (!usouAgrupamento)
                                        usouAgrupamento = true;
                                }
                            }
                        }

                        caixaMaior = await BuscarMaiorCaixa(listaCaixasMaisComum);

                        caixaAnterior = null;
                        contadorCubagem2 = 0;

                        caixaAnteriorEhMelhorQueAtual = false;
                        encontrouCaixaCorreta = false;
                    }

                    if (!usouAgrupamento)
                        sair = true;
                    else
                        agrupador++;
                } while (!sair);

                return await BuscaItensNaoCubicadosSemFrancionamento(agrupador, listaItensDoPedido, listaCaixasMaisComum);
            }
            catch (Exception)
            { 
                throw;
            }
        }

        /// <summary>
        /// Rotina que calcula as opções de caixas mais comuns entre os itens contidos no volume passado via parâmetro.
        /// </summary>
        /// <returns></returns>
        public async Task<List<CaixaViewModel>> BuscarCaixaMaisComum(List<PedidoItemViewModel> listaItensDoPedido)
        {
            List<CaixaViewModel> listaCaixasMaisComum = new List<CaixaViewModel>();

            //Valida se a listaRankingCaixas é nula.
            if (listaRankingCaixas == null)
                return listaCaixasMaisComum;

            //Captura a quantidade do maior da listaRankingCaixas.
            decimal quantidadeDoMaiorDaListaRankingCaixas = listaRankingCaixas.Max(x => x.QuantidadeRanking);

            /*
             * A condição abaixo pega a caixa melhor posicionada na listaRankingCaixas e
	         * enquanto houver outras caixas com a mesma quantidade que
	         * ela, estas caixas serão adicionadas na listaCaixasMaisComum;
	         * Saiba que tem uma tolerância de 10% do valor da caixa melhor ranqueado; isso ajuda na margem.
             */
            for (int i = 0; i < listaRankingCaixas.Count &&
                listaRankingCaixas[i].QuantidadeRanking <= quantidadeDoMaiorDaListaRankingCaixas &&
                listaRankingCaixas[i].QuantidadeRanking >= (quantidadeDoMaiorDaListaRankingCaixas - Math.Round(Convert.ToDecimal((quantidadeDoMaiorDaListaRankingCaixas * 10) / 100), 0))
                ; i++)
            {
                listaCaixasMaisComum.Add(listaRankingCaixas[i]);
            }

            //Percorre cada item do pedido.
            for (int i = 0; i < listaItensDoPedido.Count; i++)
            {
                if (listaItensDoPedido[i].Agrupador == 0)
                {
                    bool encontrou = false;

                    //Percorre a lista de caixas mais comum enquanto a variável encontrou for igual a false.
                    for (int j = 0; j < listaCaixasMaisComum.Count && !encontrou; j++)
                    {
                        //Percorre as caixas dos itens do pedido.
                        for (int k = 0; k < listaItensDoPedido[i].Caixa.Count; k++)
                        {
                            //Verifica se a caixa do item do pedido é igual a caixa de lista mais comum.
                            if (listaItensDoPedido[i].Caixa[k] == listaCaixasMaisComum[j])
                            {
                                encontrou = true;
                                break;
                            }
                        }
                    }

                    //Se não encontrar, busca na listaCaixasMaisComum as caixas da listaItensDoPedido e se não existir, adiciona.
                    if (!encontrou)
                    {
                        for (int y = 0; y < listaItensDoPedido[i].Caixa.Count; y++)
                        {
                            CaixaViewModel caixa = new CaixaViewModel();

                            if (listaCaixasMaisComum.Count > 0)
                                caixa = listaCaixasMaisComum.Where(x => x == listaItensDoPedido[i].Caixa[y]).FirstOrDefault();

                            if (caixa == null)
                                listaCaixasMaisComum.Add(listaItensDoPedido[i].Caixa[y]);
                        }
                    }
                }
            }

            //Ordena a lista de caixas mais comum por quantidade.
            if (listaCaixasMaisComum.Count > 0)
                listaCaixasMaisComum = listaCaixasMaisComum.OrderByDescending(x => x.QuantidadeRanking).ToList();

            return listaCaixasMaisComum;
        }

        /// <summary>
        /// Procura a maior caixa cadastrada da listaCaixasMaisComum.
        /// </summary>
        /// <param name="listaCaixasMaisComum"></param>
        /// <returns></returns>
        public async Task<CaixaViewModel> BuscarMaiorCaixa(List<CaixaViewModel> listaCaixasMaisComum)
        {
            CaixaViewModel caixaMaior = new CaixaViewModel();

            for (int i = 0; i < listaCaixasMaisComum.Count; i++)
            {
                //Verifica se é a primeira caixaMaior ou se a cubagem da listaCaixasMaisComum 
                //é maior que a caixaMaior.
                if (caixaMaior == null || listaCaixasMaisComum[i].Cubagem > caixaMaior.Cubagem)
                {
                    caixaMaior = listaCaixasMaisComum[i];
                }
            }

            return caixaMaior;
        }

        /// <summary>
        /// Este método calcula a cubagem do item. O cálculo foi separado da rotina principal para facilitar a manutenção.
        /// 
        /// </summary>
        /// <param name="pedidoItem"></param>
        /// <returns></returns>
        public async Task<decimal?> CalcularCubagemPedidoItem(PedidoItemViewModel pedidoItem)
        {
            var cubagemPedidoItem = (pedidoItem.Produto.CubagemProduto * pedidoItem.Quantidade) / pedidoItem.Produto.MultiploVenda;

            return cubagemPedidoItem;
        }

        /// <summary>
        /// Este método calcula a cubicagem do item 
        /// O cálculo foi separado da rotina principal para facilitar a manutenção.
        /// 
        /// </summary>
        /// <param name="pedidoItem"></param>
        /// <returns></returns>
        public async Task<decimal?> CalcularPesoItemPedido(PedidoItemViewModel pedidoItem)
        {
            var pesoItem = (pedidoItem.Produto.PesoBruto * pedidoItem.Quantidade) / pedidoItem.Produto.MultiploVenda;

            return pesoItem;
        }

        /// <summary>
        /// Procura a 'próxima' maior caixa cadastrada.
        /// </summary>
        /// <param name="maiorCaixa"></param>
        /// <param name="listaCaixasMaisComum"></param>
        /// <returns></returns>
        public async Task<CaixaViewModel> CalcularProximaCaixaMaior(CaixaViewModel maiorCaixa, List<CaixaViewModel> listaCaixasMaisComum) //getPrxMaior
        {
            CaixaViewModel proximaMaiorCaixa = null;
            CaixaViewModel retornoProximaMaiorCaixa = null;

            for (int i = 0; i < listaCaixasMaisComum.Count; i++)
            {
                if ((proximaMaiorCaixa == null || listaCaixasMaisComum[i].Cubagem > proximaMaiorCaixa.Cubagem) && listaCaixasMaisComum[i].Cubagem < maiorCaixa.Cubagem)
                {
                    proximaMaiorCaixa = listaCaixasMaisComum[i];
                    retornoProximaMaiorCaixa = listaCaixasMaisComum[i];
                }
            }

            return retornoProximaMaiorCaixa;
        }

        public async Task ZerarAgrupamento(int agrupador, List<PedidoItemViewModel> listaItensDoPedido)
        {
            for (int i = 0; i < listaItensDoPedido.Count; i++)
            {
                if (listaItensDoPedido[i].Agrupador == 0 || listaItensDoPedido[i].Caixa == null || listaItensDoPedido[i].Produto.IsEmbalagemFornecedorVolume || listaItensDoPedido[i].IsSeparacaoNoPikcing == false)
                    continue;

                if (listaItensDoPedido[i].Agrupador == agrupador)
                    listaItensDoPedido[i].Agrupador = 0;
            }
        }

        /// <summary>
        /// Verifica os itens que estão com uma quantidade grande e não puderam ser cubicadas sem fracionamento (o mesmo se aplica ao peso) - aplica
        /// uma regra muito parecida com o método CalcularCubagemVolume, porém, trabalha a divisão ou fracionamento dos itens.
        /// </summary>
        /// <param name="agrupador"></param>
        /// <param name="listaItensDoPedido"></param>
        /// <param name="listaCaixasMaisComum"></param>
        /// <returns></returns>
        public async Task<List<PedidoItemViewModel>> BuscaItensNaoCubicadosSemFrancionamento(int agrupador, List<PedidoItemViewModel> listaItensDoPedido, List<CaixaViewModel> listaCaixasMaisComum) //calcFracVol
        {
            try
            {
                //Declaração das variáveis que serão utilizadas.
                CaixaViewModel caixaCorrente = null; // Caixa corrente.
                CaixaViewModel proximaCaixa = null; //Próxima caixa na escala de grandeza.
                CaixaViewModel caixaAnterior = null; //Controle da caixa anterior.
                bool caixaAnteriorEhMelhorQueAtual = false; //Indica que a caixa anterior é melhor que a atual.
                decimal? nAux2Cub;
                bool encontrouCaixaCorreta; //Indica que a caixa correta foi encontrada.
                bool sair;
                int nAuxQtde = 0;
                decimal? contadorCubagem; //Acumulador (auxiliar) para cubagem.
                decimal contadorPeso; //Acumulador (auxiliar) para peso.
                bool usarCaixaEncontrada; //Indica que os itens receberão a caixa encontrada.
                bool usouAgrupamento = false; //Indica se o agrupamento atual tem alguma peça
                List<PedidoItemViewModel> listaItensDoPedidoRetorno = new List<PedidoItemViewModel>();
                bool sairWhile = false;

                //Pra garantir que o agrupamento a ser usado não é o mesmo
                agrupador++;

                //Controle das peças que estão sem agrupamento
                do
                {
                    usouAgrupamento = false;

                    for (int i = 0; i < listaItensDoPedido.Count; i++)
                    {
                        if (listaItensDoPedido[i].Agrupador != 0 || listaItensDoPedido[i].Caixa == null || listaItensDoPedido[i].Produto.IsEmbalagemFornecedorVolume || listaItensDoPedido[i].IsSeparacaoNoPikcing == false)
                            continue;

                        caixaCorrente = await BuscarMaiorCaixa(listaCaixasMaisComum);
                        caixaAnterior = null;
                        nAux2Cub = 0;

                        encontrouCaixaCorreta = false;
                        sair = false;

                        //Laço para controle da caixa
                        do
                        {
                            var cubagemProduto = await CalcularCubagemEntreCaixaProduto(listaItensDoPedido[i].Produto, caixaCorrente);

                            if (!cubagemProduto || listaItensDoPedido[i].Caixa.Any(x => x.IdCaixa == caixaCorrente.IdCaixa) == false)
                            {
                                proximaCaixa = caixaCorrente != null ? await CalcularProximaCaixaMaior(caixaCorrente, listaCaixasMaisComum) : null;

                                if (proximaCaixa == null)
                                {
                                    if (!caixaAnteriorEhMelhorQueAtual)
                                        sairWhile = true;
                                }
                                else
                                {
                                    caixaCorrente = proximaCaixa;
                                    continue;
                                }
                            }

                            //Laço que coloca os itens dentro da caixa
                            nAuxQtde = 0;

                            decimal valor = 1.05M;

                            do
                            {
                                nAuxQtde += listaItensDoPedido[i].Produto.MultiploVenda <= 0 ? 1 : Convert.ToInt32(listaItensDoPedido[i].Produto.MultiploVenda);
                                contadorCubagem = (listaItensDoPedido[i].Produto.CubagemProduto * nAuxQtde) / listaItensDoPedido[i].Produto.MultiploVenda;
                                contadorPeso = (listaItensDoPedido[i].Produto.PesoBruto * nAuxQtde) / listaItensDoPedido[i].Produto.MultiploVenda;
                            } while (
                                (nAuxQtde + (listaItensDoPedido[i].Produto.MultiploVenda <= 0 ? 1 : listaItensDoPedido[i].Produto.MultiploVenda)) <= listaItensDoPedido[i].Quantidade &&
                                (((listaItensDoPedido[i].Produto.CubagemProduto * (nAuxQtde + (listaItensDoPedido[i].Produto.MultiploVenda <= 0 ? 1 : listaItensDoPedido[i].Produto.MultiploVenda))) / listaItensDoPedido[i].Produto.MultiploVenda) <= (caixaCorrente.Cubagem * ((100 - caixaCorrente.Sobra) / 100)) &&
                                ((listaItensDoPedido[i].Produto.PesoBruto * (nAuxQtde + (listaItensDoPedido[i].Produto.MultiploVenda <= 0 ? 1 : listaItensDoPedido[i].Produto.MultiploVenda)) <= (caixaCorrente.PesoMaximo * valor))))
                            );

                            usarCaixaEncontrada = true;

                            if (!encontrouCaixaCorreta)
                            {
                                if (contadorCubagem.Value != 0 && contadorCubagem < (caixaCorrente.Cubagem * ((100 - caixaCorrente.Sobra) / 100)) &&
                                    listaItensDoPedido[i].Caixa != null)
                                {
                                    if (caixaAnteriorEhMelhorQueAtual && contadorCubagem != nAux2Cub)
                                    {
                                        encontrouCaixaCorreta = true;

                                        caixaCorrente = caixaAnterior;
                                        caixaAnteriorEhMelhorQueAtual = false;

                                        usarCaixaEncontrada = false;
                                        usouAgrupamento = true;
                                    }
                                    else
                                    {
                                        proximaCaixa = await CalcularProximaCaixaMaior(caixaCorrente, listaCaixasMaisComum);

                                        if (caixaCorrente == proximaCaixa)
                                            encontrouCaixaCorreta = true;

                                        caixaAnterior = caixaCorrente;

                                        if (proximaCaixa != null && await CalcularCubagemEntreCaixaProduto(listaItensDoPedido[i].Produto, proximaCaixa))
                                            caixaCorrente = proximaCaixa;
                                        else
                                        {
                                            caixaCorrente = caixaAnterior;
                                            encontrouCaixaCorreta = true;
                                        }

                                        nAux2Cub = contadorCubagem;
                                        caixaAnteriorEhMelhorQueAtual = true;

                                        usarCaixaEncontrada = false;
                                        usouAgrupamento = true;
                                    }
                                }
                            }

                            if (usarCaixaEncontrada && nAuxQtde > 0)
                            {
                                if (nAuxQtde != listaItensDoPedido[i].Quantidade)
                                {
                                    int quantidadePedido = listaItensDoPedido[i].Quantidade;

                                    listaItensDoPedido.Add(new PedidoItemViewModel()
                                    {
                                        Produto = listaItensDoPedido[i].Produto,
                                        Agrupador = agrupador,
                                        CaixaEscolhida = caixaCorrente,
                                        Quantidade = nAuxQtde,
                                        Caixa = listaItensDoPedido[i].Caixa,
                                        EnderecoSeparacao = listaItensDoPedido[i].EnderecoSeparacao,
                                        GrupoCorredorArmazenagem = listaItensDoPedido[i].GrupoCorredorArmazenagem,
                                        IsSeparacaoNoPikcing = listaItensDoPedido[i].IsSeparacaoNoPikcing

                                    });

                                    usouAgrupamento = true;

                                    listaItensDoPedido[i].Quantidade = quantidadePedido - nAuxQtde;
                                }
                                else
                                {
                                    listaItensDoPedido[i].Agrupador = agrupador;
                                    listaItensDoPedido[i].CaixaEscolhida = caixaCorrente;

                                    usouAgrupamento = true;
                                }

                                caixaAnterior = null;
                                nAux2Cub = 0;
                                nAuxQtde = 0;

                                caixaAnteriorEhMelhorQueAtual = false;
                                encontrouCaixaCorreta = false;
                            }
                            else
                            {
                                if (usarCaixaEncontrada)
                                {
                                    listaItensDoPedido[i].Agrupador = agrupador;
                                    usouAgrupamento = true;
                                }
                            }

                            if (!usouAgrupamento || nAuxQtde == 0)
                                sair = true;
                            else
                            {
                                agrupador++;
                                usouAgrupamento = false;
                            }
                        } while (!sair);
                    }

                    if (!usouAgrupamento)
                        sairWhile = true;
                    else
                    {
                        agrupador++;
                        usouAgrupamento = false;
                    }

                } while (!sairWhile);

                return listaItensDoPedido;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<VolumeViewModel>> BuscarCubagemVolumes(long idEmpresa, List<PedidoItemViewModel> listaItensDoPedido)
        {
            List<VolumeViewModel> listaVolumes = new List<VolumeViewModel>();
            int nAuxAgrup = 0;
            long nAuxNrCx = 0;
            CaixaViewModel nAuxCX = null;
            CaixaViewModel caixaFornecedor = null;

            var caixaRepository = _caixaService.BuscarCaixaFornecedor(idEmpresa);

            if (caixaRepository != null)
            {
                caixaFornecedor = new CaixaViewModel()
                {
                    IdCaixa = caixaRepository.IdCaixa,
                    IdCaixaTipo = caixaRepository.IdCaixaTipo,
                    Nome = caixaRepository.Nome,
                    TextoEtiqueta = caixaRepository.TextoEtiqueta,
                    Largura = caixaRepository.Largura,
                    Altura = caixaRepository.Altura,
                    Comprimento = caixaRepository.Comprimento,
                    Cubagem = caixaRepository.Cubagem,
                    PesoCaixa = caixaRepository.PesoCaixa,
                    PesoMaximo = caixaRepository.PesoMaximo,
                    Sobra = caixaRepository.Sobra,
                    Ativo = caixaRepository.Ativo
                };
            }

            var volume = new VolumeViewModel();

            for (int i = 0; i < listaItensDoPedido.Count; i++)
            {
                if (listaItensDoPedido[i].CaixaEscolhida == null && listaItensDoPedido[i].Produto.IsEmbalagemFornecedorVolume == true)
                {
                    var condicao = listaItensDoPedido[i].Quantidade / listaItensDoPedido[i].Produto.MultiploVenda;

                    for (int l = 0; l < condicao; l++)
                    {
                        volume = new VolumeViewModel()
                        {
                            IsCaixaFornecedor = true,
                            Peso = +listaItensDoPedido[i].Produto.PesoBruto * listaItensDoPedido[i].Quantidade,
                            Cubagem = +listaItensDoPedido[i].Produto.CubagemProduto.Value * listaItensDoPedido[i].Quantidade,
                            Caixa = caixaFornecedor
                        };

                        listaVolumes.Add(volume);

                        volume.ListaItensDoPedido.Add(new PedidoItemViewModel()
                        {
                            Agrupador = listaItensDoPedido[i].Agrupador,
                            Caixa = listaItensDoPedido[i].Caixa,
                            CaixaEscolhida = listaItensDoPedido[i].CaixaEscolhida,
                            Produto = listaItensDoPedido[i].Produto,
                            EnderecoSeparacao = listaItensDoPedido[i].EnderecoSeparacao,
                            GrupoCorredorArmazenagem = listaItensDoPedido[i].GrupoCorredorArmazenagem,
                            IsSeparacaoNoPikcing = listaItensDoPedido[i].IsSeparacaoNoPikcing,
                            Quantidade = Convert.ToInt32(listaItensDoPedido[i].Produto.MultiploVenda),
                            IdLote = listaItensDoPedido[i].IdLote
                        });
                    }
                }
                else if (listaItensDoPedido[i].CaixaEscolhida == null && listaItensDoPedido[i].Produto.IsEmbalagemFornecedorVolume == false)
                {
                    volume = new VolumeViewModel()
                    {
                        IsCaixaFornecedor = true,
                        Peso = +listaItensDoPedido[i].Produto.PesoBruto * listaItensDoPedido[i].Quantidade,
                        Cubagem = +listaItensDoPedido[i].Produto.CubagemProduto.Value * listaItensDoPedido[i].Quantidade,
                        Caixa = caixaFornecedor
                    };

                    listaVolumes.Add(volume);

                    volume.ListaItensDoPedido.Add(listaItensDoPedido[i]);
                }
                else if (listaItensDoPedido[i].Agrupador != nAuxAgrup || listaItensDoPedido[i].CaixaEscolhida.IdCaixa != nAuxNrCx)
                {
                    nAuxCX = listaItensDoPedido[i].Caixa.Where(x => x.IdCaixa == listaItensDoPedido[i].CaixaEscolhida.IdCaixa).FirstOrDefault();

                    if (nAuxCX != null)
                    {
                        volume = new VolumeViewModel()
                        {
                            Caixa = nAuxCX,
                            Peso = +listaItensDoPedido[i].Produto.PesoBruto * listaItensDoPedido[i].Quantidade,
                            Cubagem = +listaItensDoPedido[i].Produto.CubagemProduto.Value * listaItensDoPedido[i].Quantidade
                        };

                        listaVolumes.Add(volume);

                        nAuxAgrup = listaItensDoPedido[i].Agrupador;
                        nAuxNrCx = listaItensDoPedido[i].CaixaEscolhida.IdCaixa;
                    }
                    else
                    {
                        volume = new VolumeViewModel()
                        {
                            IsCaixaFornecedor = true,
                            Peso = +listaItensDoPedido[i].Produto.PesoBruto * listaItensDoPedido[i].Quantidade,
                            Cubagem = +listaItensDoPedido[i].Produto.CubagemProduto.Value * listaItensDoPedido[i].Quantidade,
                            Caixa = caixaFornecedor
                        };

                        listaVolumes.Add(volume);
                    }
                }

                if (listaItensDoPedido[i].CaixaEscolhida != null)
                    volume.ListaItensDoPedido.Add(listaItensDoPedido[i]);
            }

            return listaVolumes;
        }

        private string PreparaStatusParaColetor(PedidoVendaStatusEnum status)
        {
            switch (status)
            {
                case PedidoVendaStatusEnum.ProcessandoIntegracao:
                    return "Preparando Ped.";
                case PedidoVendaStatusEnum.PendenteSeparacao:
                    return "Ped. Pendente";
                case PedidoVendaStatusEnum.EnviadoSeparacao:
                    return "Enviado Sep.";
                case PedidoVendaStatusEnum.ProcessandoSeparacao:
                    return "Separando";
                case PedidoVendaStatusEnum.SeparacaoConcluidaComSucesso:
                    return "Separado";
                case PedidoVendaStatusEnum.PendenteCancelamento:
                    return "Cancelando";
                case PedidoVendaStatusEnum.Cancelado:
                    return "Cancelado";
                case PedidoVendaStatusEnum.InstalandoVolumeTransportadora:
                    return "Instalando Vol.";
                case PedidoVendaStatusEnum.VolumeInstaladoTransportadora:
                    return "Vol. Instalado";
                case PedidoVendaStatusEnum.MovendoDOCA:
                    return "Movendo DOCA";
                case PedidoVendaStatusEnum.MovidoDOCA:
                    return "Movido DOCA";
                case PedidoVendaStatusEnum.DespachandoNF:
                    return "Despachando NF.";
                case PedidoVendaStatusEnum.NFDespachada:
                    return "NF. Despachada";
                case PedidoVendaStatusEnum.RomaneioImpresso:
                    return "Env. Transp.";
                default:
                    return null;
            }
        }

        public DetalhePedidoVendaResposta ConsultarDetalhesPedidoVenda(string referenciaOuNumeroPedido, long idEmpresa)
        {
            PedidoVenda pedidoVenda = null;

            if (int.TryParse(referenciaOuNumeroPedido, out int numeroPedido))
            {
                pedidoVenda = _unitOfWork.PedidoVendaRepository.ObterPorNroPedidoEEmpresa(numeroPedido, idEmpresa);
            }

            if (pedidoVenda == null)
            {
                BuscaEValidaDadosPorReferenciaPedido(referenciaOuNumeroPedido, out numeroPedido, out long _idTransportadora, out int _numeroVolume);

                pedidoVenda = _unitOfWork.PedidoVendaRepository.ObterPorNroPedidoEEmpresa(numeroPedido, idEmpresa);
            }

            ValidarPedidoVenda(pedidoVenda, idEmpresa);

            var retorno = new DetalhePedidoVendaResposta()
            {
                IdPedidoVenda = pedidoVenda.IdPedidoVenda,
                NroPedidoVenda = pedidoVenda.NroPedidoVenda,
                DataCriacao = pedidoVenda.Pedido.DataCriacao,
                Status = PreparaStatusParaColetor(pedidoVenda.IdPedidoVendaStatus),
                PesoTotal = pedidoVenda.PedidoVendaVolumes.Sum(pvv => pvv.PesoVolume),
                ListaVolumes = pedidoVenda.PedidoVendaVolumes.Select(pvv => new DetalhePedidoVendaVolumeResposta()
                {
                    IdPedidoVendaVolume = pvv.IdPedidoVendaVolume,
                    Numero = pvv.NroVolume,
                    Status = PreparaStatusParaColetor(pvv.IdPedidoVendaStatus),
                    ListaProdutos = pvv.PedidoVendaProdutos.Select(pvp => new DetalhePedidoVendaVolumeProdutoResposta()
                    {
                        IdPedidoVendaProduto = pvp.IdPedidoVendaProduto,
                        IdProduto = pvp.IdProduto,
                        ReferenciaProduto = pvp.Produto.Referencia,
                        CodigoEndereco = pvp.EnderecoArmazenagem.Codigo,
                        DescricaoPontoArmazenagem = pvp.EnderecoArmazenagem.PontoArmazenagem.Descricao,
                        QuantidadeSeparar = pvp.QtdSeparar,
                        QuantidadeSeparada = pvp.QtdSeparada.GetValueOrDefault(),
                        Corredor = 2,
                        UsuarioSeparacao = pvp.IdUsuarioSeparacao,
                        DataHoraSeparacao = pvp.DataHoraFimSeparacao,
                        Peso = pvp.PesoProduto
                    }).ToList()
                }).ToList()
            };

            retorno.ListaVolumes.SelectMany(v => v.ListaProdutos).ToList().ForEach(pvp =>
            {
                pvp.Corredor = _unitOfWork.ProdutoEstoqueRepository.ObterPorProdutoEmpresa(pvp.IdProduto, idEmpresa)?.EnderecoArmazenagem?.Corredor;

                if (!pvp.UsuarioSeparacao.NullOrEmpty())
                {
                    var perfilUsuario = _unitOfWork.PerfilUsuarioRepository.GetByUserId(pvp.UsuarioSeparacao);

                    if (perfilUsuario != null)
                    {
                        pvp.UsuarioSeparacao = $"{perfilUsuario.Usuario.UserName}";
                    }
                }
            });

            return retorno;
        }
    }
}