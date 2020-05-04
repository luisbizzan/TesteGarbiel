using AutoMapper;
using DartDigital.Library.Exceptions;
using FWLog.Data;
using FWLog.Data.Models;
using FWLog.Services.Integracao;
using FWLog.Services.Model.Caixa;
using FWLog.Services.Model.Coletor;
using FWLog.Services.Model.Produto;
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
        private List<CaixaViewModel> listaRankingCaixas;
        private PedidoVendaService _pedidoVendaService;
        private PedidoVendaProdutoService _pedidoVendaProdutoService;
        private PedidoVendaVolumeService _pedidoVendaVolumeService;

        public SeparacaoPedidoService(UnitOfWork unitOfWork, ColetorHistoricoService coletorHistoricoService, ILog log, PedidoVendaService pedidoVendaService, PedidoVendaProdutoService pedidoVendaProdutoService, PedidoVendaVolumeService pedidoVendaVolumeService)
        {
            _unitOfWork = unitOfWork;
            _coletorHistoricoService = coletorHistoricoService;
            _log = log;
            _pedidoVendaService = pedidoVendaService;
            _pedidoVendaProdutoService = pedidoVendaProdutoService;
            _pedidoVendaVolumeService = pedidoVendaVolumeService;
        }

        public List<long> ConsultaPedidoVendaEmSeparacao(string idUsuario, long idEmpresa)
        {
            var ids = _unitOfWork.PedidoVendaRepository.PesquisarIdsEmSeparacao(idUsuario, idEmpresa);
            return ids;
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

        public BuscarPedidoVendaResposta BuscarPedidoVenda(string codigoBarrasPedido, string idUsuario, long idEmpresa)
        {
            var pedidoVenda = ValidarPedidoVenda(codigoBarrasPedido, idEmpresa);

            ValidarPedidoVendaPorUsuario(idUsuario, idEmpresa, pedidoVenda);

            var model = new BuscarPedidoVendaResposta();

            model.IdPedidoVenda = pedidoVenda.IdPedidoVenda;
            model.NroPedidoVenda = pedidoVenda.NroPedidoVenda;
            model.SeparacaoIniciada = pedidoVenda.IdPedidoVendaStatus == PedidoVendaStatusEnum.ProcessandoSeparacao;

            model.ListaCorredoresSeparacao = pedidoVenda.PedidoVendaVolumes.Select(pedidoVendaVolume => new BuscarPedidoVendaGrupoCorredorResposta
            {
                IdGrupoCorredorArmazenagem = pedidoVendaVolume.IdGrupoCorredorArmazenagem,
                CorredorInicial = pedidoVendaVolume.GrupoCorredorArmazenagem.CorredorInicial,
                CorredorFinal = pedidoVendaVolume.GrupoCorredorArmazenagem.CorredorFinal
            }).OrderBy(o => new { o.CorredorInicial, o.CorredorFinal }).ToList();

            //model.ListaEnderecosArmazenagem = pedidoVenda.PedidoVendaProdutos.Where(p => p.QtdSeparada < p.Qtd)
            //.Select(pedidoVendaProduto => new BuscarPedidoVendaGrupoCorredorEnderecoProdutoResposta
            //{
            //    Corredor = pedidoVendaProduto.EnderecoArmazenagem.Corredor,
            //    Codigo = pedidoVendaProduto.EnderecoArmazenagem.Codigo,
            //    PontoArmazenagem = pedidoVendaProduto.EnderecoArmazenagem.PontoArmazenagem.Descricao,
            //    ReferenciaProduto = pedidoVendaProduto.Produto.Referencia,
            //    MultiploProduto = pedidoVendaProduto.Produto.MultiploVenda,
            //    QtdePedido = pedidoVendaProduto.QtdPedido
            //}).OrderBy(o => new { o.Corredor, o.Codigo }).ToList();

            return model;
        }

        private PedidoVenda ValidarPedidoVenda(string codigoBarrasPedido, long idEmpresa)
        {
            if (codigoBarrasPedido.NullOrEmpty())
            {
                throw new BusinessException("Código de barras do pedido deve ser infomado.");
            }

            var pedidoVenda = ConsultaPedidoVenda(codigoBarrasPedido, idEmpresa);

            if (pedidoVenda == null)
            {
                throw new BusinessException("O pedido informado não foi encontrado.");
            }

            if (pedidoVenda.IdEmpresa != idEmpresa)
            {
                throw new BusinessException("O pedido informado não pertence a empresa do usuário logado.");
            }

            if (pedidoVenda.IdPedidoVendaStatus == PedidoVendaStatusEnum.ConcluidaComSucesso)
            {
                throw new BusinessException("O pedido informado já foi separado.");
            }

            if (pedidoVenda.IdPedidoVendaStatus == PedidoVendaStatusEnum.EnviadoSeparacao)
            {
                throw new BusinessException("O pedido informado ainda não está liberado para a separação.");
            }

            if (pedidoVenda.IdPedidoVendaStatus == PedidoVendaStatusEnum.PendenteCancelamento || pedidoVenda.IdPedidoVendaStatus == PedidoVendaStatusEnum.Cancelado)
            {
                throw new BusinessException("O pedido informado teve a separação cancelada.");
            }

            return pedidoVenda;
        }

        private void ValidarPedidoVendaPorUsuario(string idUsuario, long idEmpresa, PedidoVenda pedidoVenda)
        {
            var pedidoVendaPorUsuario = _unitOfWork.PedidoVendaRepository.ObterPorIdUsuarioEIdEmpresa(idUsuario, idEmpresa);

            if (pedidoVendaPorUsuario.Any(x => x.PedidoVendaStatus.IdPedidoVendaStatus == PedidoVendaStatusEnum.ProcessandoSeparacao && x.IdPedidoVenda != pedidoVenda.IdPedidoVenda))
            {
                throw new BusinessException("Existe um pedido em separação pelo usuário logado que não foi concluído.");
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

        private PedidoVenda ConsultaPedidoVenda(string codigoBarrasPedido, long idEmpresa)
        {
            if (codigoBarrasPedido.Length < 7)
            {
                throw new BusinessException("Código de Barras de pedido inválido.");
            }

            var numeroPedidoString = codigoBarrasPedido.Substring(0, codigoBarrasPedido.Length - 6);

            if (!int.TryParse(numeroPedidoString, out int numeroPedido))
            {
                throw new BusinessException("Código de Barras de pedido inválido.");
            }

            var volumeString = codigoBarrasPedido.Substring(codigoBarrasPedido.Length - 3);

            if (!int.TryParse(volumeString, out int volume))
            {
                throw new BusinessException("Código de Barras de pedido inválido.");
            }

            var pedidoVenda = _unitOfWork.PedidoVendaRepository.ObterPorNroPedidoENroVolume(numeroPedido, volume, idEmpresa);

            return pedidoVenda;
        }

        private async Task AjustarQuantidadeVolume(long idEnderecoArmazenagem, long idProduto, int quantidadeAdicionar, long idEmpresa, string idUsuarioAjuste)
        {
            var loteProdutoEndereco = _unitOfWork.LoteProdutoEnderecoRepository.PesquisarPorEnderecoProdutoEmpresa(idEnderecoArmazenagem, idProduto, idEmpresa);

            var produto = _unitOfWork.ProdutoRepository.GetById(idProduto);

            var quantidadeAnterior = loteProdutoEndereco.Quantidade;

            var novaQuantidade = loteProdutoEndereco.Quantidade += quantidadeAdicionar;

            var pesoInstalacao = produto.PesoLiquido / produto.MultiploVenda * novaQuantidade;

            using (var transacao = _unitOfWork.CreateTransactionScope())
            {
                loteProdutoEndereco.Quantidade = novaQuantidade;
                loteProdutoEndereco.PesoTotal = pesoInstalacao;

                _unitOfWork.LoteProdutoEnderecoRepository.Update(loteProdutoEndereco);
                await _unitOfWork.SaveChangesAsync();

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
                }

                transacao.Complete();

                var gravarHistoricoColetorRequisicao = new GravarHistoricoColetorRequisicao
                {
                    IdColetorAplicacao = ColetorAplicacaoEnum.Armazenagem,
                    IdColetorHistoricoTipo = ColetorHistoricoTipoEnum.AjustarQuantidade,
                    Descricao = $"Ajustou a quantidade de {quantidadeAnterior} para {loteProdutoEndereco.Quantidade} unidade(s) do produto {loteProdutoEndereco.Produto.Referencia} do lote {loteProdutoEndereco.Lote.IdLote} do endereço {loteProdutoEndereco.EnderecoArmazenagem.Codigo}",
                    IdEmpresa = idEmpresa,
                    IdUsuario = idUsuarioAjuste
                };

                _coletorHistoricoService.GravarHistoricoColetor(gravarHistoricoColetorRequisicao);
            }
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

            if (pedidoVenda.IdUsuarioSeparacao != idUsuarioOperacao)
            {
                throw new BusinessException("O pedido não pertence ao usuário logado.");
            }

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
                pedidoVenda.IdUsuarioSeparacao = null;
                pedidoVenda.DataHoraInicioSeparacao = null;
                pedidoVenda.DataHoraFimSeparacao = null;

                _unitOfWork.SaveChanges();

                foreach (var pedidoVendaProduto in pedidoVenda.PedidoVendaProdutos.ToList())
                {
                    if (pedidoVendaProduto.QtdSeparada.HasValue)
                    {
                        await AjustarQuantidadeVolume(pedidoVendaProduto.IdEnderecoArmazenagem, pedidoVendaProduto.IdProduto, pedidoVendaProduto.QtdSeparada.Value, idEmpresa, idUsuarioOperacao);
                    }

                    pedidoVendaProduto.IdPedidoVendaProdutoStatus = PedidoVendaProdutoStatusEnum.AguardandoSeparacao;
                    pedidoVendaProduto.DataHoraInicioSeparacao = null;
                    pedidoVendaProduto.DataHoraFimSeparacao = null;
                    pedidoVendaProduto.IdUsuarioAutorizacaoZerar = idUsuarioOperacao;
                    pedidoVendaProduto.QtdSeparada = 0;
                }

                _unitOfWork.SaveChanges();

                foreach (var pedidoVendaVolume in pedidoVenda.PedidoVendaVolumes.ToList())
                {
                    pedidoVendaVolume.IdPedidoVendaStatus = novoStatusSeparacao;
                    pedidoVendaVolume.DataHoraInicioSeparacao = null;
                    pedidoVendaVolume.DataHoraFimSeparacao = null;
                    pedidoVendaVolume.IdCaixaVolume = null;
                }

                _unitOfWork.SaveChanges();

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

        public async Task IniciarSeparacaoPedidoVenda(long idPedidoVenda, string idUsuarioOperacao, long idEmpresa)
        {
            // validações
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

            if (pedidoVenda.IdPedidoVendaStatus != PedidoVendaStatusEnum.EnviadoSeparacao &&
                pedidoVenda.IdPedidoVendaStatus != PedidoVendaStatusEnum.ProcessandoSeparacao)
            {
                throw new BusinessException("O pedido de venda não está liberado para separação.");
            }

            if (pedidoVenda.IdPedidoVendaStatus == PedidoVendaStatusEnum.ProcessandoSeparacao &&
                pedidoVenda.IdUsuarioSeparacao != idUsuarioOperacao)
            {
                throw new BusinessException("O pedido já está sendo separado por outro usuário.");
            }

            // update do pedido na base oracle
            using (var transaction = _unitOfWork.CreateTransactionScope())
            {
                pedidoVenda.IdPedidoVendaStatus = PedidoVendaStatusEnum.ProcessandoSeparacao;
                pedidoVenda.IdUsuarioSeparacao = idUsuarioOperacao;
                pedidoVenda.DataHoraInicioSeparacao = DateTime.Now;

                _unitOfWork.PedidoVendaRepository.Update(pedidoVenda);

                _unitOfWork.SaveChanges();

                // update no Sankhya
                await AtualizarStatusPedidoVenda(pedidoVenda.Pedido, PedidoVendaStatusEnum.ProcessandoSeparacao);

                transaction.Complete();
            }
        }

        public async Task DividirPedido(long idEmpresa)
        {
            if (idEmpresa == 0)
                throw new BusinessException("Empresa inválida");

            //Captura os corredores por empresa e ordena por corredor inicial.
            var grupoCorredorArmazenagem = _unitOfWork.GrupoCorredorArmazenagemRepository.Todos().Where(x => x.IdEmpresa == idEmpresa).OrderBy(x => x.CorredorInicial);

            //Captura os pedidos por empresa e status pendente separação.
            var listaPedidos = _unitOfWork.PedidoRepository.PesquisarPendenteSeparacao(idEmpresa);

            foreach (var pedido in listaPedidos) //Percorre a lista de pedidos.
            {
                var idPedidoVenda = await _pedidoVendaService.Salvar(pedido); 

                if (idPedidoVenda == 0)
                    break;

                //Agrupa os itens do pedido por produto. 
                var listaItensDoPedido = await AgruparItensDoPedidoPorProduto(pedido.IdPedido); 

                /*
                 * Usamos o foreach abaixo para capturar e atualizar o IdGrupoCorredorArmazenagem e IdEnderecoArmazenagem de cada item.
                 */
                foreach (var pedidoItem in listaItensDoPedido) 
                {
                    //Captura o endereço de picking do produto.
                    //Posteriormente a lógica deverá ser alterada por ponto de separação.
                    var enderecoArmazenagemProduto = _unitOfWork.ProdutoEstoqueRepository.ObterPorProdutoEmpresaPicking(pedidoItem.Produto.IdProduto, idEmpresa);

                    if (enderecoArmazenagemProduto == null)
                        break;

                    //Captura o grupo de corredores do item do pedido.
                    var grupoCorredorArmazenagemItemPedido = _unitOfWork.GrupoCorredorArmazenagemRepository.Todos().Where(
                        x => x.CorredorInicial >= enderecoArmazenagemProduto.EnderecoArmazenagem.Corredor && x.CorredorFinal <= enderecoArmazenagemProduto.EnderecoArmazenagem.Corredor).FirstOrDefault();

                    if (grupoCorredorArmazenagemItemPedido == null)
                        break;

                    //Captura o indice do item na lista e atualizo os dados.
                    int index = listaItensDoPedido.IndexOf(pedidoItem);
                    listaItensDoPedido[index].IdGrupoCorredorArmazenagem = grupoCorredorArmazenagemItemPedido.IdGrupoCorredorArmazenagem;
                    listaItensDoPedido[index].IdEnderecoArmazenagem = enderecoArmazenagemProduto.IdEnderecoArmazenagem.Value;
                }

                /*
                 * No foreach abaixo, capturamos quais e a quantidade de caixas (volumes) que serão utilizados.
                 * Além disso, através do método Cubicagem, saberemos a caixa de cada produto. 
                 * É importante saber que o processo é feito por corredor.
                 */
                foreach (var itemCorredorArmazenagem in grupoCorredorArmazenagem) 
                {
                    int contadorVolume = 1;
                    
                    //Captura o corredor do item.
                    var listaItensDoPedidoPorCorredor = listaItensDoPedido.Where(x => x.IdGrupoCorredorArmazenagem == itemCorredorArmazenagem.IdGrupoCorredorArmazenagem).ToList();

                    //Se não houver nenhum item para o corredor, vai para o próximo.
                    if (listaItensDoPedidoPorCorredor == null)
                        break;

                    //Captura os itens do pedido com as caixas em que cada um deve ir.
                    //A partir do método cubicagem, existem chamadas para vários outros.
                    var listaItensDoPedidoDividido = await Cubicagem(listaItensDoPedidoPorCorredor, idEmpresa);

                    if (listaItensDoPedidoDividido.Count > 0)
                    {
                        //Busca os volumes que serão utilizados.
                        var listaVolumes = await BuscarCubagemVolumes(listaItensDoPedidoDividido);

                        foreach (var itemVolume in listaVolumes)
                        {
                            //Chamar aqui para salvar o volume.
                            //Implementar aqui dentro do método Salvar se CaixaFornecedor então IdCaixa é nulo.
                            var idPedidoVendaVolume = await _pedidoVendaVolumeService.Salvar(idPedidoVenda, itemVolume.Caixa, itemCorredorArmazenagem, contadorVolume);

                            if (idPedidoVendaVolume == 0)
                                break;

                            contadorVolume++;

                            foreach (var item in listaItensDoPedidoDividido)
                            {
                                //Chamar aqui para salvar os produtos do volume.
                                await _pedidoVendaProdutoService.Salvar(idPedidoVenda, idPedidoVendaVolume, item);
                            }
                        }

                        //Atualizar a quantidade de volumes na PedidoVenda.
                    }
                }

                //Atualizar status da PedidoVenda.
            }
        }

        public async Task<List<PedidoItemViewModel>> AgruparItensDoPedidoPorProduto(long idPedido)
        {
            List<PedidoItemViewModel> listaItensDoPedidoAgrupada = new List<PedidoItemViewModel>();

            //Captura os itens do pedido.
            var listaItensDoPedido = _unitOfWork.PedidoItemRepository.BuscarPorIdPedido(idPedido);

            //Devido a mais de uma linha do mesmo produto, fazemos o agrupamento por produto.
            listaItensDoPedidoAgrupada = listaItensDoPedido.GroupBy(x => new { x.IdPedido, x.Produto, x.QtdPedido }).Select(s => new PedidoItemViewModel()
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
                Quantidade = s.Key.QtdPedido
            }).ToList();

            return listaItensDoPedidoAgrupada;
        }

        

        public async Task<List<PedidoItemViewModel>> Cubicagem(List<PedidoItemViewModel> listaItensDoPedido, long idEmpresa) //Cubicagem
        {
            List<PedidoItemViewModel> listaItensDoPedidoRetorno = new List<PedidoItemViewModel>();

            /*
             * Lista dos endereços e corredores com seus respectivos produtos.
             * São considerados endereços de separação aqueles com a flag Ponto de Separação (IsPontoSeparacao).
             * Inicialmente, estou capturando somente endereços de Picking pois, considerar pontos de separação será feito numa próxima versão.
             * Posteriormente, descomentar a linha listaEnderecosSeparacao(IsPicking == true).
            */
            //var listaEnderecosSeparacao = _unitOfWork.ProdutoEstoqueRepository.ObterProdutoEstoquePorEmpresa(pedido.IdEmpresa).Where(x => x.EnderecoArmazenagem.IsPontoSeparacao == true && x.EnderecoArmazenagem.Ativo == true).ToList();
            var listaEnderecosSeparacao = _unitOfWork.ProdutoEstoqueRepository.ObterProdutoEstoquePorEmpresa(idEmpresa).Where(x => x.EnderecoArmazenagem.IsPicking == true && x.EnderecoArmazenagem.Ativo == true).ToList();

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
                    Prioridade = item.Prioridade,
                    Ativo = item.Ativo
                });
            }

            //Inicializa a lista do ranking de caixas.
            listaRankingCaixas = new List<CaixaViewModel>();

            listaItensDoPedidoRetorno = await BuscarPesoCubagemItensPedido(listaItensDoPedido, listaEnderecosSeparacao, listaCaixas);

            if (listaItensDoPedidoRetorno.Count > 0)
            {
                listaItensDoPedidoRetorno = await CalcularCubagemVolume(listaItensDoPedidoRetorno);

                //Agrupa os itens por caixa e agrupamento.
                listaItensDoPedidoRetorno = listaItensDoPedidoRetorno.OrderBy(x => x.CaixaEscolhida.IdCaixa).OrderBy(y => y.Agrupador).ToList();
            }

            return listaItensDoPedidoRetorno;
        }

        /// <summary>
        /// Este método retorna as informações que serão utilizadas na cubicagem dos volumes.
        /// Através da listaPedidoAgrupadaPorProduto o método sabe quais os itens que serão cubicados.
        /// </summary>
        /// <param name="idPedido"></param>
        /// <param name="listaPedidoAgrupado"></param>
        /// <param name="listaEnderecosSeparacao"></param>
        /// <returns></returns>
        public async Task<List<PedidoItemViewModel>> BuscarPesoCubagemItensPedido(List<PedidoItemViewModel> listaItensDoPedido, List<ProdutoEstoque> listaEnderecosSeparacao, List<CaixaViewModel> listaCaixas) //getInfoIt
        {
            var listaItensDoPedidoCubicados = new List<PedidoItemViewModel>();
            
            for (int i = 0; i < listaItensDoPedido.Count; i++)
            {
                /*Busca todos os endereços de separação do produto.
                 * Como foi dito anteriormente, nessa primeira versão, vamos capturar somente o endereço de picking, deixando de ser uma lista.
                 * Posteriormente, substituir enderecoArmazenagemPicking por listaEnderecosSeparacaoDoProduto.
                 */
                //var listaEnderecosSeparacaoDoProduto = listaEnderecosSeparacao.Where(x => x.IdProduto == listaItensDoPedidoAgrupadaPorProduto[i].Produto.IdProduto).ToList();
                var enderecoArmazenagemPicking = listaEnderecosSeparacao.Where(x => x.IdProduto == listaItensDoPedido[i].Produto.IdProduto).FirstOrDefault();

                //Aqui é feito uma verificação para ignorar itens que estão sem localização.
                //O motivo é que foi dito na reunião com Veronezzi e Beatriz que a chance disso acontecer é pequena, pois ao cadastrar um produto será informado um endereço de picking para ele.
                //Mantive a verificação comentada pois, se tiver algum erro perante a isso basta descomentar.
                //if (listaEnderecosSeparacaoDoProduto == null)
                //    break;
                
                decimal? larguraProduto = listaItensDoPedido[i].Produto.Largura;
                decimal? comprimentoProduto = listaItensDoPedido[i].Produto.Comprimento;
                decimal? alturaProduto = listaItensDoPedido[i].Produto.Altura;

                listaItensDoPedidoCubicados.Add(new PedidoItemViewModel()
                {
                    Produto = new ProdutoViewModel()
                    {
                        IdProduto = listaItensDoPedido[i].Produto.IdProduto,
                        IdEnderecoArmazenagem = enderecoArmazenagemPicking.IdEnderecoArmazenagem,
                        CodigoEnderecoArmazenagem = enderecoArmazenagemPicking.EnderecoArmazenagem.Codigo,
                        CubagemProduto = (larguraProduto * comprimentoProduto) * alturaProduto,
                        PesoBruto = listaItensDoPedido[i].Produto.PesoBruto,
                        MultiploVenda = listaItensDoPedido[i].Produto.MultiploVenda,
                        Altura = alturaProduto,
                        Largura = larguraProduto,
                        Comprimento = comprimentoProduto
                        //IsEmbalagemFornecedor (posteriormente, capturar o valor da entidade produto).
                        //IsEmbalagemFornecedorVolume (posteriormente, capturar o valor da entidade produto).
                    },
                    Agrupador = 0,
                    CaixaEscolhida = null,
                    Quantidade = listaItensDoPedido[i].Quantidade,
                    Caixa = await CaixasQuePodemSerUtilizadas(listaItensDoPedido[i].Produto, listaCaixas)
                });

                //Classifica a listaRankingCaixas por Quantidade
                listaRankingCaixas = listaRankingCaixas.OrderBy(x => x.QuantidadeRanking).ToList();
            }

            return listaItensDoPedidoCubicados;
        }

        /// <summary>
        /// Analisa as caixas de recusa e inverte o resultado para agregar aos itens as caixas que PODEM ser utilizadas
        /// </summary>
        /// <param name="referencia"></param>
        /// <param name="ListaCaixas"></param>
        /// <param name="LarguraAlturaComprimento"></param>
        /// <returns></returns>
        public async Task<List<CaixaViewModel>> CaixasQuePodemSerUtilizadas(ProdutoViewModel produto, List<CaixaViewModel> listaCaixas) //getCaixasOK
        {
            List<CaixaViewModel> listaCaixasQuePodemSerUtilizadas = new List<CaixaViewModel>();

            /*
             * Aqui deverá ser implementado a verificação das caixas de recusa. Mas o que é isso?
             * A caixa de recusas é uma estrutura onde são armazenadas as caixas que um determinado produto NÃO pode utilizar.
             * Sabendo disso, será necessário executar as seguintes atividades:
             *  1. Criar a tabela CaixaProdutoRecusa no banco de dados.
             *  2. Possíveis campos da tabela: IdCaixaProdutoRecusa, IdProduto, IdCaixa
             *  3. Implementar um CRUD para cadastrar, editar e excluir essas verificações.
             *  4. Levar em consideração que o mesmo produto pode possuir várias caixas de recusa. 
             *  5. Depois de criar o CRUD, capturar as caixas de recursa do banco.
             *  6. Em seguida, remover da listaCaixas o ID das caixas que forem iguais da tabela de recusa.
             *  7. Caso a lista de recusa seja vazia ou o ID não exista na listaCaixas, vida que segue.
             * */
             
            for (int i = 0; i <listaCaixas.Count; i++)
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
            if (produto.Largura.HasValue == false || produto.Largura == 0 ||
                produto.Comprimento.HasValue == false || produto.Comprimento == 0 ||
                produto.PesoBruto == 0)
                return false;
            
            //Valida se a largura, comprimento e e peso bruto do produto é menor os valores da caixa.
            if (produto.Largura <= caixa.Largura && produto.Comprimento <= caixa.Comprimento && produto.PesoBruto <= caixa.PesoCaixa)
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
            //Declaração das variáveis que serão utilizadas.
            bool caixaAnteriorEhMelhorQueAtual = false; //Indica que a caixa anterior é melhor que a atual.
            bool encontrouCaixaCorreta = false; //Indica que a caixa correta foi encontrada.
            bool usarCaixaEncontrada = false; //Indica que os itens receberão a caixa encontrada.
            CaixaViewModel caixaMaior = new CaixaViewModel(); //Caixa corrente.
            CaixaViewModel caixaAnterior = new CaixaViewModel(); //Controle da caixa anterior.
            CaixaViewModel proximaCaixa = new CaixaViewModel(); //Próxima caixa na escala de grandeza.
            decimal contadorCubagem2 = 0;
            bool usouAgrupamento; //Indica se o agrupamento foi utilizado.
            decimal contadorCubagem; //Acumulador (auxiliar) para cubagem.
            decimal contadorPeso; //Acumulador (auxiliar) para peso.

            //Captura a lista de caixas mais comum.
            var listaCaixasMaisComum = await BuscarCaixaMaisComum(listaItensDoPedido);

            //Valida se a lista de caixas mais comum é nula.
            if (listaCaixasMaisComum == null)
                return listaItensDoPedido;

            int agrupador = 1;

            for (int i = 0; i < listaItensDoPedido.Count; i++)
            {
                //Para cada item do pedido, se não existir caixa ou for embalagem do fornecedor, vai adicionando o agrupador.
                if ((listaItensDoPedido[i].Agrupador == 0 && listaItensDoPedido[i].Caixa == null) || listaItensDoPedido[i].Produto.IsEmbalagemFornecedorVolume)
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
                    if (listaItensDoPedido[i].Agrupador != 0 || listaItensDoPedido[i].Caixa == null || listaItensDoPedido[i].Produto.IsEmbalagemFornecedorVolume)
                        break;

                    //Verifica se o item cabe na caixa indicada.
                    if (!await CalcularCubagemEntreCaixaProduto(listaItensDoPedido[i].Produto, caixaMaior))
                        break;

                    //Calcula a cubagem do item (produto) do pedido.
                    var cubagemPedidoItem = await CalcularCubagemPedidoItem(listaItensDoPedido[i]);

                    //Calcula o peso do item (produto) do pedido.
                    var pesoPedidoItem = await CalcularPesoItemPedido(listaItensDoPedido[i]);
                    decimal valor = 1.05M; //Variavel utilizada no calculo da sobra da caixa. Existe uma margem de 5% por conta do plástico bolha.

                    //Verifica se a cubagem total (contadorCubagem + cubagemPedidoItem) é menor ou igual a cubagem da caixa.
                    //Verifica se o peso total (contadorCubagem + cubagemPedidoItem) é menor ou igual ao peso maximo da caixa.
                    if ((contadorCubagem + cubagemPedidoItem.Value) <= (caixaMaior.Cubagem * ((100 - caixaMaior.Sobra)/100)) &&
                        (contadorPeso + pesoPedidoItem.Value) <= (caixaMaior.PesoMaximo * valor))
                    {
                        //Verifica se a caixa identificada está na lista de caixas dos itens do pedido;
                        if (listaItensDoPedido[i].Caixa.Any(x=> x == caixaMaior))
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
                    if (contadorCubagem < (caixaMaior.Cubagem *((100 - caixaMaior.Sobra)/100)))
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
                            if (listaItensDoPedido[i].Agrupador == 0 || listaItensDoPedido[i].Caixa == null || listaItensDoPedido[i].Produto.IsEmbalagemFornecedorVolume)
                                break;

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
                    break;
                else
                    agrupador++;
            } while (true);

            return await BuscaItensNaoCubicadosSemFrancionamento(agrupador, listaItensDoPedido, listaCaixasMaisComum);
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
                listaCaixasMaisComum = listaCaixasMaisComum.OrderBy(x => x.QuantidadeRanking).ToList();

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
            CaixaViewModel proximaMaiorCaixa = new CaixaViewModel();
            CaixaViewModel retornoProximaMaiorCaixa = new CaixaViewModel();

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
                if (listaItensDoPedido[i].Agrupador == 0 || listaItensDoPedido[i].Caixa == null || listaItensDoPedido[i].Produto.IsEmbalagemFornecedorVolume)
                    break;

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
            //Declaração das variáveis que serão utilizadas.
            CaixaViewModel caixaCorrente = new CaixaViewModel(); // Caixa corrente.
            CaixaViewModel proximaCaixa = new CaixaViewModel(); //Próxima caixa na escala de grandeza.
            CaixaViewModel caixaAnterior = new CaixaViewModel(); //Controle da caixa anterior.
            bool caixaAnteriorEhMelhorQueAtual = false; //Indica que a caixa anterior é melhor que a atual.
            decimal? nAux2Cub;
            bool encontrouCaixaCorreta; //Indica que a caixa correta foi encontrada.
            bool sair;
            decimal nAuxQtde = 0;
            decimal? nAuxCub = 0;
            decimal nAuxPeso = 0;
            bool usarCaixaEncontrada; //Indica que os itens receberão a caixa encontrada.
            bool usouAgrupamento = false; //Indica se o agrupamento foi utilizado.
            PedidoItemViewModel pedidoItem = new PedidoItemViewModel(); 
            List<PedidoItemViewModel> listaItensDoPedidoRetorno = new List<PedidoItemViewModel>();

            //Pra garantir que o agrupamento a ser usado não é o mesmo
            agrupador++;

            //Controle das peças que estão sem agrupamento
            do
            {
                for (int i = 0; i < listaItensDoPedido.Count; i++)
                {
                    usouAgrupamento = false;
                    
                    //FALTOU VERIFICAR usa embalagem fornecedor OR
                    if (listaItensDoPedido[i].Agrupador == 0 || listaItensDoPedido[i].Caixa == null)
                        break;

                    caixaCorrente = await BuscarMaiorCaixa(listaCaixasMaisComum);

                    caixaAnteriorEhMelhorQueAtual = false;
                    nAux2Cub = 0;

                    encontrouCaixaCorreta = false;
                    sair = false;

                    //Laço para controle da caixa
                    do
                    {
                        var cubagemProduto = await CalcularCubagemEntreCaixaProduto(listaItensDoPedido[i].Produto, caixaCorrente);

                        if (!(cubagemProduto) || listaItensDoPedido[i].Caixa.Any(x => x.IdCaixa == caixaCorrente.IdCaixa) == false)
                        {
                            proximaCaixa = caixaCorrente != null ? await CalcularProximaCaixaMaior(caixaCorrente, listaCaixasMaisComum) : null;

                            if (proximaCaixa == null)
                            {
                                if (!caixaAnteriorEhMelhorQueAtual)
                                    break;
                            }
                            else
                            {
                                caixaCorrente = proximaCaixa;
                                break;
                            }
                        }

                        //Laço que coloca os itens dentro da caixa
                        nAuxQtde = 0;

                        var condicao = (nAuxQtde + listaItensDoPedido[i].Produto.MultiploVenda <= 0 ? 1 : listaItensDoPedido[i].Produto.MultiploVenda) <= listaItensDoPedido[i].Quantidade &&
                            ((listaItensDoPedido[i].Produto.CubagemProduto * (nAuxQtde + (listaItensDoPedido[i].Produto.MultiploVenda <= 0 ? 1 : listaItensDoPedido[i].Produto.MultiploVenda))) / listaItensDoPedido[i].Produto.MultiploVenda <= (caixaCorrente.Cubagem * ((100 - caixaCorrente.Sobra) / 100)) &
                            ((listaItensDoPedido[i].Produto.PesoBruto * (nAuxQtde + (listaItensDoPedido[i].Produto.MultiploVenda <= 0 ? 1 : listaItensDoPedido[i].Produto.MultiploVenda)) <= (caixaCorrente.PesoMaximo * 1))));

                        do
                        {
                            nAuxQtde += listaItensDoPedido[i].Produto.MultiploVenda <= 0 ? 1 : listaItensDoPedido[i].Produto.MultiploVenda;
                            nAuxCub = (listaItensDoPedido[i].Produto.CubagemProduto * nAuxQtde) / listaItensDoPedido[i].Produto.MultiploVenda;
                            nAuxPeso = (listaItensDoPedido[i].Produto.PesoBruto * nAuxQtde) / listaItensDoPedido[i].Produto.MultiploVenda;
                        } while (condicao);

                        usarCaixaEncontrada = true;

                        if (!encontrouCaixaCorreta)
                        {
                            if (nAuxCub != 0 && nAuxCub < (caixaCorrente.Cubagem * ((100-caixaCorrente.Sobra) / 100 )) &&
                                listaItensDoPedido[i].Caixa != null)
                            {
                                if (caixaAnteriorEhMelhorQueAtual && nAuxCub != nAux2Cub)
                                {
                                    encontrouCaixaCorreta = true;

                                    caixaCorrente = caixaAnterior; //ALTERAR 
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

                                    nAux2Cub = nAuxCub;
                                    caixaAnteriorEhMelhorQueAtual = true;

                                    usarCaixaEncontrada = true;
                                    usouAgrupamento = true;
                                }
                            }
                        }

                        if (usarCaixaEncontrada && nAuxQtde > 0)
                        {
                            if (nAuxQtde != listaItensDoPedido[i].Quantidade)
                            {
                                pedidoItem = listaItensDoPedido[i];

                                pedidoItem.Quantidade = Convert.ToInt32(nAuxQtde);
                                pedidoItem.Agrupador = agrupador;
                                pedidoItem.CaixaEscolhida = caixaCorrente;

                                usouAgrupamento = true;

                                listaItensDoPedido[i].Quantidade -= Convert.ToInt32(nAuxQtde);
                                listaItensDoPedido.Add(pedidoItem);
                            }
                            else
                            {
                                listaItensDoPedido[i].Agrupador = agrupador;
                                pedidoItem.CaixaEscolhida = caixaCorrente;

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

                    if (!usouAgrupamento)
                        break;
                    else
                    {
                        agrupador++;
                        usouAgrupamento = false;
                    }
                }

            } while (true);

            return listaItensDoPedido;
        }

        public async Task<List<VolumeViewModel>> BuscarCubagemVolumes(List<PedidoItemViewModel> listaItensDoPedido)
        {
            List<VolumeViewModel> listaVolumes = new List<VolumeViewModel>(); 
            int nAuxAgrup = 0;
            long nAuxNrCx = 0;

            CaixaViewModel nAuxCX = new CaixaViewModel();

            for (int i = 0; i < listaItensDoPedido.Count; i++)
            {
                if (listaItensDoPedido[i].CaixaEscolhida != null)
                {
                    var condicao = listaItensDoPedido[i].Quantidade / listaItensDoPedido[i].Produto.MultiploVenda;
                    
                    for (int l = 0; l < condicao; l++)
                    {
                        listaVolumes.Add(new VolumeViewModel()
                        {
                            CaixaFornecedor =  true
                        });
                        //aAdd(aVolumes, { 'F', 0 } )
                    }
                } 
                else if (listaItensDoPedido[i].Agrupador != nAuxAgrup || listaItensDoPedido[i].CaixaEscolhida.IdCaixa != nAuxNrCx)
                {
                    nAuxCX = listaItensDoPedido[i].Caixa.Where(x => x.IdCaixa == listaItensDoPedido[i].CaixaEscolhida.IdCaixa).FirstOrDefault();

                    if (nAuxCX != null)
                    {
                        listaVolumes.Add(new VolumeViewModel()
                        {
                            Caixa = nAuxCX
                        }); ;
                        //aAdd(aVolumes, { aCaixas[nAuxCX, 3], ; aCaixas[nAuxCX, 1] } )
                        nAuxAgrup = listaItensDoPedido[i].Agrupador;
                        nAuxNrCx = listaItensDoPedido[i].CaixaEscolhida.IdCaixa;
                    }
                    else
                    {
                        listaVolumes.Add(new VolumeViewModel()
                        {
                            CaixaFornecedor = true
                        });
                        //aAdd(aVolumes, { 'F', 0 } )
                    }
                }
            }

            return listaVolumes;
        }
    }
}