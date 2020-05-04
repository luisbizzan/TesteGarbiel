using DartDigital.Library.Exceptions;
using FWLog.Data;
using FWLog.Data.Models;
using FWLog.Services.Integracao;
using FWLog.Services.Model.Coletor;
using FWLog.Services.Model.IntegracaoSankhya;
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
        private ILog _log;

        public SeparacaoPedidoService(UnitOfWork unitOfWork, ColetorHistoricoService coletorHistoricoService, ILog log)
        {
            _unitOfWork = unitOfWork;
            _coletorHistoricoService = coletorHistoricoService;
            _log = log;
        }

        public List<long> ConsultaPedidoVendaEmSeparacao(string idUsuario, long idEmpresa)
        {
            //var ids = _unitOfWork.PedidoVendaRepository.PesquisarIdsEmSeparacao(idUsuario, idEmpresa);
            //return ids;
            throw new NotImplementedException();
        }

        public async Task AtualizarStatusPedidoVenda(Pedido pedido, PedidoVendaStatusEnum statusPedidoVenda)
        {
            if (!Convert.ToBoolean(ConfigurationManager.AppSettings["IntegracaoSankhya_Habilitar"]))
            {
                return;
            }

            try
            {
                Dictionary<string, string> campoChave = new Dictionary<string, string> { { "NUNOTA", pedido.CodigoIntegracao.ToString() } };

                await IntegracaoSankhya.Instance.AtualizarInformacaoIntegracao("CabecalhoNota", campoChave, "AD_STATUSSEP", statusPedidoVenda.GetHashCode());
            }
            catch (Exception ex)
            {
                var errorMessage = string.Format("Erro na atualização do pedido de venda: {0}.", pedido.CodigoIntegracao);
                _log.Error(errorMessage, ex);
                throw new BusinessException(errorMessage);
            }
        }

        public BuscarPedidoVendaResposta BuscarPedidoVenda(string codigoBarrasPedido, long idEmpresa)
        {
            if (codigoBarrasPedido.NullOrEmpty())
            {
                throw new BusinessException("Código de barras do pedido deve ser infomado.");
            }

            if (codigoBarrasPedido.Length < 7)
            {
                throw new BusinessException("Código de Barras de pedido inválido.");
            }

            var numeroPedidoString = codigoBarrasPedido.Substring(0, codigoBarrasPedido.Length - 6);

            if (!int.TryParse(numeroPedidoString, out int numeroPedido))
            {
                throw new BusinessException("Código de Barras de pedido inválido.");
            }

            var numeroVolumeString = codigoBarrasPedido.Substring(codigoBarrasPedido.Length - 3);

            if (!int.TryParse(numeroVolumeString, out int numeroVolume))
            {
                throw new BusinessException("Código de Barras de pedido inválido.");
            }

            var pedidoVenda = _unitOfWork.PedidoVendaRepository.ObterPorNroPedidoEEmpresa(numeroPedido, idEmpresa);

            ValidarPedidoVenda(pedidoVenda, idEmpresa);

            var pedidoVendaVolume = pedidoVenda.PedidoVendaVolumes.FirstOrDefault(volume => volume.NroVolume == numeroVolume);

            if (pedidoVendaVolume == null)
            {
                throw new BusinessException("Volume fornecido não encontrado.");
            }

            var model = new BuscarPedidoVendaResposta();

            model.IdPedidoVenda = pedidoVenda.IdPedidoVenda;
            model.NroPedidoVenda = pedidoVenda.NroPedidoVenda;
            model.SeparacaoIniciada = pedidoVenda.IdPedidoVendaStatus == PedidoVendaStatusEnum.ProcessandoSeparacao;
            model.IdPedidoVendaVolume = pedidoVendaVolume.IdPedidoVendaVolume;
            model.NroVolume = pedidoVendaVolume.NroVolume;

            var consultaProdutos = (from pedidoVendaProduto in pedidoVendaVolume.PedidoVendaProdutos
                                    let enderecoArmazenagem = pedidoVendaProduto.EnderecoArmazenagem
                                    from grupoCorredorArmazenagem in _unitOfWork.GrupoCorredorArmazenagemRepository.Todos().Where(gca =>
                                                   gca.IdPontoArmazenagem == enderecoArmazenagem.IdPontoArmazenagem &&
                                                   enderecoArmazenagem.Corredor >= gca.CorredorInicial &&
                                                   enderecoArmazenagem.Corredor <= gca.CorredorFinal)
                                    where
                                        pedidoVendaProduto.QtdSeparada.GetValueOrDefault() < pedidoVendaProduto.QtdSeparar
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
                    IdPontoArmazenagem = item.pedidoVendaProduto.EnderecoArmazenagem.IdPontoArmazenagem
                }).ToList()
            }).ToList();

            model.ListaCorredoresSeparacao = model.ListaCorredoresSeparacao.OrderBy(order => order.CorredorInicial).ThenBy(order => order.CorredorFinal).ToList();

            foreach (var item in model.ListaCorredoresSeparacao)
            {
                item.ListaEnderecosArmazenagem = item.ListaEnderecosArmazenagem.OrderBy(order => order.Corredor).ThenBy(order => order.Codigo).ToList();
            }

            return model;
        }

        private void ValidarPedidoVenda(PedidoVenda pedidoVenda, long idEmpresa)
        {
            if (pedidoVenda == null)
            {
                throw new BusinessException("O pedido informado não foi encontrado.");
            }

            if (pedidoVenda.IdEmpresa != idEmpresa)
            {
                throw new BusinessException("O pedido informado não pertence a empresa do usuário logado.");
            }
        }

        private void ValidarPedidoVendaEmSeparacao(PedidoVenda pedidoVenda, long idEmpresa)
        {
            ValidarPedidoVenda(pedidoVenda, idEmpresa);

            if (pedidoVenda.IdPedidoVendaStatus == PedidoVendaStatusEnum.EnviadoSeparacao)
            {
                throw new BusinessException("O pedido informado ainda não está liberado para a separação.");
            }

            if (pedidoVenda.IdPedidoVendaStatus == PedidoVendaStatusEnum.ConcluidaComSucesso)
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

            if (pedidoVendaVolume.IdPedidoVendaStatus == PedidoVendaStatusEnum.ConcluidaComSucesso)
            {
                throw new BusinessException("O volume informado já foi separado.");
            }

            if (pedidoVendaVolume.IdPedidoVendaStatus == PedidoVendaStatusEnum.PendenteCancelamento || pedidoVendaVolume.IdPedidoVendaStatus == PedidoVendaStatusEnum.Cancelado)
            {
                throw new BusinessException("O volume informado teve a separação cancelada.");
            }
        }

        //TODO: Falta definir os status e adicionar IdPontoArmazenagemSeparacao na UsuarioEmpresa
        //public void ValidarPedidoVendaVolumePorUsuario(string idUsuario, long idEmpresa, List<ProdutoEstoque> produtoEstoque)
        //{
        //    var usuarioEmpresa = _unitOfWork.UsuarioEmpresaRepository.Obter(idEmpresa, idUsuario);
        //    var range = Enumerable.Range(usuarioEmpresa.CorredorEstoqueInicio.Value, usuarioEmpresa.CorredorSeparacaoFim.Value);

        //    foreach (var item in produtoEstoque)
        //    {
        //        if (!range.Contains(item.EnderecoArmazenagem.Corredor))
        //    }
        //}

        private async Task AjustarQuantidadeVolume(long idEnderecoArmazenagem, long idProduto, int quantidadeAdicionar, long idEmpresa, string idUsuarioAjuste)
        {
            var loteProdutoEndereco = _unitOfWork.LoteProdutoEnderecoRepository.PesquisarPorEnderecoProdutoEmpresa(idEnderecoArmazenagem, idProduto, idEmpresa);

            var produto = _unitOfWork.ProdutoRepository.GetById(idProduto);

            var quantidadeAnterior = loteProdutoEndereco.Quantidade;

            var novaQuantidade = loteProdutoEndereco.Quantidade += quantidadeAdicionar;

            var pesoInstalacao = produto.PesoLiquido / produto.MultiploVenda * novaQuantidade;

            loteProdutoEndereco.Quantidade = novaQuantidade;
            loteProdutoEndereco.PesoTotal = pesoInstalacao;

            _unitOfWork.LoteProdutoEnderecoRepository.Update(loteProdutoEndereco);
            await _unitOfWork.SaveChangesAsync();

            var textoLogLote = string.Empty;

            if (loteProdutoEndereco.IdLote.HasValue)
            {
                var loteMovimentacao = new LoteMovimentacao
                {
                    IdEmpresa = idEmpresa,
                    IdLote = loteProdutoEndereco.IdLote.Value,
                    IdProduto = loteProdutoEndereco.IdProduto,
                    IdEnderecoArmazenagem = loteProdutoEndereco.IdEnderecoArmazenagem,
                    IdUsuarioMovimentacao = idUsuarioAjuste,
                    Quantidade = loteProdutoEndereco.Quantidade,
                    IdLoteMovimentacaoTipo = LoteMovimentacaoTipoEnum.Ajuste,
                    DataHora = DateTime.Now
                };

                _unitOfWork.LoteMovimentacaoRepository.Add(loteMovimentacao);
                await _unitOfWork.SaveChangesAsync();

                textoLogLote = $" do lote {loteProdutoEndereco.IdLote}";
            }

            var gravarHistoricoColetorRequisicao = new GravarHistoricoColetorRequisicao
            {
                IdColetorAplicacao = ColetorAplicacaoEnum.Separacao,
                IdColetorHistoricoTipo = ColetorHistoricoTipoEnum.AjustarQuantidade,
                Descricao = $"Ajustou a quantidade de {quantidadeAnterior} para {loteProdutoEndereco.Quantidade} unidade(s) do produto {loteProdutoEndereco.Produto.Referencia} do{textoLogLote} endereço {loteProdutoEndereco.EnderecoArmazenagem.Codigo}",
                IdEmpresa = idEmpresa,
                IdUsuario = idUsuarioAjuste
            };

            _coletorHistoricoService.GravarHistoricoColetor(gravarHistoricoColetorRequisicao);
        }

        public async Task CancelarPedidoSeparacao(long idPedidoVenda, string usuarioPermissaoCancelamento, string idUsuarioOperacao, long idEmpresa)
        {
            if (idPedidoVenda <= 0)
            {
                throw new BusinessException("O pedido deve ser informado.");
            }
            var pedidoVenda = _unitOfWork.PedidoVendaRepository.GetById(idPedidoVenda);

            if (pedidoVenda == null)
            {
                throw new BusinessException("O pedido não foi encontrado.");
            }

            if (pedidoVenda.IdEmpresa != idEmpresa)
            {
                throw new BusinessException("O pedido não pertence a empresa do usuário logado.");

            }

            //if (pedidoVenda.IdUsuarioSeparacao != idUsuarioOperacao)
            //{
            //    throw new BusinessException("O pedido não pertence ao usuário logado.");
            //}

            if (pedidoVenda.IdPedidoVendaStatus != PedidoVendaStatusEnum.ProcessandoSeparacao)
            {
                throw new BusinessException("O pedido não está em separação.");
            }

            if (usuarioPermissaoCancelamento.NullOrEmpty())
            {
                throw new BusinessException("O usuário da permissão deve ser informado.");
            }

            using (var transacao = _unitOfWork.CreateTransactionScope())
            {
                var novoStatusSeparacao = PedidoVendaStatusEnum.EnviadoSeparacao;

                pedidoVenda.IdPedidoVendaStatus = novoStatusSeparacao;
                pedidoVenda.DataHoraInicioSeparacao = null;
                pedidoVenda.DataHoraFimSeparacao = null;

                _unitOfWork.SaveChanges();


                foreach (var pedidoVendaVolume in pedidoVenda.PedidoVendaVolumes.ToList())
                {
                    pedidoVendaVolume.IdPedidoVendaStatus = novoStatusSeparacao;
                    pedidoVendaVolume.DataHoraInicioSeparacao = null;
                    pedidoVendaVolume.DataHoraFimSeparacao = null;
                    pedidoVendaVolume.IdCaixaVolume = null;

                    foreach (var pedidoVendaProduto in pedidoVendaVolume.PedidoVendaProdutos.ToList())
                    {
                        if (pedidoVendaProduto.QtdSeparada.HasValue)
                        {
                            await AjustarQuantidadeVolume(pedidoVendaProduto.IdEnderecoArmazenagem, pedidoVendaProduto.IdProduto, pedidoVendaProduto.QtdSeparada.Value, idEmpresa, idUsuarioOperacao);
                        }

                        pedidoVendaProduto.IdPedidoVendaStatus = novoStatusSeparacao;
                        pedidoVendaProduto.IdUsuarioSeparacao = null;
                        pedidoVendaProduto.DataHoraInicioSeparacao = null;
                        pedidoVendaProduto.DataHoraFimSeparacao = null;
                        pedidoVendaProduto.IdUsuarioAutorizacaoZerar = idUsuarioOperacao;
                        pedidoVendaProduto.DataHoraAutorizacaoZerarPedido = DateTime.Now;

                        pedidoVendaProduto.QtdSeparada = 0;
                    }

                    _unitOfWork.SaveChanges();
                }

                await AtualizarStatusPedidoVenda(pedidoVenda.Pedido, novoStatusSeparacao);

                var gravarHistoricoColetorRequisicao = new GravarHistoricoColetorRequisicao
                {
                    IdColetorAplicacao = ColetorAplicacaoEnum.Separacao,
                    IdColetorHistoricoTipo = ColetorHistoricoTipoEnum.CancelamentoSeparacao,
                    Descricao = $"Cancelou a separação do pedido {idPedidoVenda} com permissão do usuário {usuarioPermissaoCancelamento}",
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
                throw new BusinessException("O pedido deve ser informado.");
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

            if (pedidoVenda.IdPedidoVendaStatus != PedidoVendaStatusEnum.EnviadoSeparacao &&
                pedidoVenda.IdPedidoVendaStatus != PedidoVendaStatusEnum.ProcessandoSeparacao)
            {
                throw new BusinessException("O pedido de venda não está liberado para separação.");
            }

            //if (pedidoVenda.IdPedidoVendaStatus == PedidoVendaStatusEnum.ProcessandoSeparacao /*&& pedidoVenda.IdUsuarioSeparacao != idUsuarioOperacao*/)
            //{
            //    throw new BusinessException("O pedido já está sendo separado por outro usuário.");
            //}

            var dataProcessamento = DateTime.Now;

            if (pedidoVenda.IdPedidoVendaStatus == PedidoVendaStatusEnum.EnviadoSeparacao)
            {
                await AtualizarStatusPedidoVenda(pedidoVenda.Pedido, PedidoVendaStatusEnum.ProcessandoSeparacao);

                using (var transaction = _unitOfWork.CreateTransactionScope())
                {
                    pedidoVenda.IdPedidoVendaStatus = PedidoVendaStatusEnum.ProcessandoSeparacao;
                    pedidoVenda.DataHoraInicioSeparacao = dataProcessamento;
                    pedidoVenda.Pedido.IdPedidoVendaStatus = PedidoVendaStatusEnum.ProcessandoSeparacao;

                    _unitOfWork.PedidoVendaRepository.Update(pedidoVenda);

                    _unitOfWork.SaveChanges();

                    transaction.Complete();
                }
            }

            var pedidoVendaVolume = pedidoVenda.PedidoVendaVolumes.First(f => f.IdPedidoVendaVolume == idPedidoVendaVolume);

            if (pedidoVendaVolume.IdPedidoVendaStatus == PedidoVendaStatusEnum.EnviadoSeparacao)
            {
                pedidoVendaVolume.DataHoraInicioSeparacao = dataProcessamento;
                pedidoVendaVolume.IdPedidoVendaStatus = PedidoVendaStatusEnum.ProcessandoSeparacao;

                _unitOfWork.PedidoVendaVolumeRepository.Update(pedidoVendaVolume);

                _unitOfWork.SaveChanges();
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

            if (pedidoVendaVolume.PedidoVendaProdutos.Any(pedidoVendaProduto => pedidoVendaProduto.QtdSeparada.GetValueOrDefault() < pedidoVendaProduto.QtdSeparar))
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
                var novoStatusSeparacao = PedidoVendaStatusEnum.ConcluidaComSucesso;
                var dataProcessamento = DateTime.Now;

                pedidoVendaVolume.IdPedidoVendaStatus = novoStatusSeparacao;
                pedidoVendaVolume.DataHoraFimSeparacao = dataProcessamento;
                pedidoVendaVolume.IdCaixaVolume = idCaixa;

                _unitOfWork.SaveChanges();

                var todosProdutosVenda = _unitOfWork.PedidoVendaProdutoRepository.ObterPorIdPedidoVenda(idPedidoVenda);

                var finalizouPedidoVenda = false;

                if (!todosProdutosVenda.Any(produtoVendaProduto => produtoVendaProduto.QtdSeparada != produtoVendaProduto.QtdSeparar))
                {
                    pedidoVenda.IdPedidoVendaStatus = novoStatusSeparacao;
                    pedidoVenda.Pedido.IdPedidoVendaStatus = novoStatusSeparacao;
                    pedidoVenda.DataHoraFimSeparacao = dataProcessamento;

                    _unitOfWork.SaveChanges();

                    await AtualizarQtdConferidaIntegracao(pedidoVenda);

                    await AtualizarStatusPedidoVenda(pedidoVenda.Pedido, novoStatusSeparacao);

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

        public async Task<SalvarSeparacaoProdutoResposta> SalvarSeparacaoProduto(long idPedidoVenda, long idProduto, long idProdutoSeparacao, string idUsuario, long idEmpresa)
        {
            var pedidoVenda = _unitOfWork.PedidoVendaRepository.ObterPorIdPedidoVendaEIdEmpresa(idPedidoVenda, idEmpresa);

            ValidarPedidoVendaEmSeparacao(pedidoVenda, idEmpresa);

            ValidarProdutoPorPedidoVenda(idPedidoVenda, idProdutoSeparacao);

            var pedidoVendaProduto = _unitOfWork.PedidoVendaProdutoRepository.ObterPorIdProduto(idProdutoSeparacao);

            ValidarPedidoVendaVolumeConcluidoCancelado(pedidoVendaProduto.PedidoVendaVolume);

            var loteProdutoEndereco = _unitOfWork.LoteProdutoEnderecoRepository.PesquisarPorEnderecoProdutoEmpresa(pedidoVendaProduto.IdEnderecoArmazenagem, pedidoVendaProduto.IdProduto, idEmpresa);

            ValidarLoteProdutoEndereco(loteProdutoEndereco);

            if (idProduto != idProdutoSeparacao)
            {
                throw new BusinessException("O produto informado é inválido para separação.");
            }

            var salvarSeparacaoProdutoResposta = new SalvarSeparacaoProdutoResposta
            {
                IdPedidoVenda = pedidoVendaProduto.IdPedidoVenda,
                IdProduto = pedidoVendaProduto.IdProduto,
                Multiplo = pedidoVendaProduto.Produto.MultiploVenda,
                Referencia = pedidoVendaProduto.Produto.Referencia,
                QtdSeparar = pedidoVendaProduto.QtdSeparar,
            };

            using (var transacao = _unitOfWork.CreateTransactionScope())
            {
                var quantidadeIncrementada = (int)pedidoVendaProduto.Produto.MultiploVenda;

                var qtdSeparada = pedidoVendaProduto.QtdSeparada.GetValueOrDefault() + quantidadeIncrementada;

                if (qtdSeparada > pedidoVendaProduto.QtdSeparar)
                {
                    throw new BusinessException("A quantidade separada é maior que o pedido.");
                }

                var dataProcessamento = DateTime.Now;

               

                if (pedidoVendaProduto.QtdSeparada.GetValueOrDefault() == 0)
                {
                    pedidoVendaProduto.DataHoraInicioSeparacao = dataProcessamento;
                    pedidoVendaProduto.IdUsuarioSeparacao = idUsuario;
                    pedidoVendaProduto.IdPedidoVendaStatus = PedidoVendaStatusEnum.ProcessandoSeparacao;
                }
                else if (qtdSeparada == pedidoVendaProduto.QtdSeparar)
                {
                    pedidoVendaProduto.DataHoraFimSeparacao = dataProcessamento;
                    pedidoVendaProduto.IdPedidoVendaStatus = PedidoVendaStatusEnum.ConcluidaComSucesso;

                    salvarSeparacaoProdutoResposta.ProdutoSeparado = true;
                }

                pedidoVendaProduto.QtdSeparada = salvarSeparacaoProdutoResposta.QtdSeparada = qtdSeparada;

                salvarSeparacaoProdutoResposta.VolumeSeparado = !pedidoVendaProduto.PedidoVendaVolume.PedidoVendaProdutos.Any(pvv => pvv.QtdSeparada != pvv.QtdSeparar);

                _unitOfWork.PedidoVendaProdutoRepository.Update(pedidoVendaProduto);
                await _unitOfWork.SaveChangesAsync();

                await AjustarQuantidadeVolume(pedidoVendaProduto.IdEnderecoArmazenagem, pedidoVendaProduto.IdProduto, quantidadeIncrementada * -1, idEmpresa, idUsuario);

                transacao.Complete();
            }

            return salvarSeparacaoProdutoResposta;
        }

        //TODO: Será utilizado no momento que precisar validar PedidosVenda em aberto para separação por usuário
        //public void ValidarPedidoVendaPorUsuario(string idUsuario, long idEmpresa, PedidoVenda pedidoVenda)
        //{
        //    var pedidoVendaPorUsuario = _unitOfWork.PedidoVendaRepository.ObterPorIdUsuarioEIdEmpresa(idUsuario, idEmpresa);

        //    if (pedidoVendaPorUsuario.Any(x => x.PedidoVendaStatus.IdPedidoVendaStatus == PedidoVendaStatusEnum.ProcessandoSeparacao && x.IdPedidoVenda != pedidoVenda.IdPedidoVenda))
        //    {
        //        throw new BusinessException("Existe um pedido em separação pelo usuário logado que não foi concluído.");
        //    }
        //}

        public void ValidarProdutoPorPedidoVenda(long idPedidoVenda, long idProduto)
        {
            var pedidoVendaProdutos = _unitOfWork.PedidoVendaProdutoRepository.ObterPorIdPedidoVenda(idPedidoVenda);

            if (!pedidoVendaProdutos.Any(pvp => pvp.IdProduto == idProduto))
            {
                throw new BusinessException("O produto informado não faz parte do pedido em separação.");
            }
        }

        public void ValidarLoteProdutoEndereco(LoteProdutoEndereco loteProdutoEndereco)
        {
            if (loteProdutoEndereco == null)
            {
                throw new BusinessException("Não foi encontrado lote produto endereço associado ao produto.");
            }
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
    }
}