using FWLog.Data;
using FWLog.Data.Models;
using FWLog.Services.Model.Conferencia;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FWLog.Services.Services
{
    public class ConferenciaService 
    {
        private readonly UnitOfWork _uow;
        private readonly LoteService _loteService;
        public ConferenciaResponse conferenciaResponse { get; set; }

        public ConferenciaService(UnitOfWork uow, LoteService loteService)
        {
            _uow = uow;
            _loteService = loteService;
            conferenciaResponse = new ConferenciaResponse();
        }

        public ConferenciaResponse ValidarProduto(long idLote, string codigoBarrasOuReferencia, long idEmpresa)
        {
            conferenciaResponse.Sucesso = true;

            //Validar lote
            var lote = _uow.LoteRepository.GetById(idLote);

            if (lote == null)
            {
                conferenciaResponse.Sucesso = false;
                conferenciaResponse.Mensagem = "Lote não encontrado. Por favor, tente novamente!";

                return conferenciaResponse;
            }

            //Validar se o lote já foi conferido durante o processo de conferência.
            if (lote.IdLoteStatus != LoteStatusEnum.Recebido && lote.IdLoteStatus != LoteStatusEnum.Conferencia)
            {
                conferenciaResponse.Sucesso = false;
                conferenciaResponse.Mensagem = $"A conferência do lote: {lote.IdLote} já foi finalizada.";

                return conferenciaResponse;
            }

            //Valida se o código de barras ou referência é vazio ou nulo.
            if (string.IsNullOrEmpty(codigoBarrasOuReferencia))
            {
                conferenciaResponse.Sucesso = false;
                conferenciaResponse.Mensagem = "Referência inválida. Por favor, tente novamente!";

                return conferenciaResponse;
            }

            //Valida se foi encontrado um produto através do código de barras, código de barras 2 ou da referência.
            var produto = _uow.ProdutoRepository.ConsultarPorCodigoBarrasOuReferencia(codigoBarrasOuReferencia);

            if (produto == null)
            {
                conferenciaResponse.Sucesso = false;
                conferenciaResponse.Mensagem = "Referência sem código de barras. Por favor, tente novamente!";

                return conferenciaResponse;
            }

            //Valida se as configurações da empresa existem.
            var empresaConfig = _uow.EmpresaConfigRepository.ConsultarPorIdEmpresa(idEmpresa);

            if (empresaConfig == null)
            {
                conferenciaResponse.Sucesso = false;
                conferenciaResponse.Mensagem = "As configurações da empresa não foram encontradas. Por favor, tente novamente!";

                return conferenciaResponse;
            }

            //Valida se o produto está fora de linha (fornecedor 400)
            var produtoEstoque = _uow.ProdutoEstoqueRepository.ConsultarPorProduto(produto.IdProduto, idEmpresa);

            if (produtoEstoque.IdProdutoEstoqueStatus == ProdutoEstoqueStatusEnum.ForaLinha)
            {
                conferenciaResponse.Sucesso = false;
                conferenciaResponse.Mensagem = "A referência informada está fora de linha. Por favor, tente novamente!";

                return conferenciaResponse;
            }

            //Valida se a largura, altura e comprimento do produto.
            if (!(produto.Largura.HasValue || produto.Altura.HasValue || produto.Comprimento.HasValue))
            {
                conferenciaResponse.Sucesso = false;
                conferenciaResponse.Mensagem = "Referência sem cubicagem. Por favor, tente novamente!";

                return conferenciaResponse;
            }

            //Valida se o múltiplo é menor ou igual a 0.
            if (produto.MultiploVenda <= 0)
            {
                conferenciaResponse.Sucesso = false;
                conferenciaResponse.Mensagem = "Referência sem múltiplo. Por favor, tente novamente!";

                return conferenciaResponse;
            }

            //Valida se a unidade de medida é KT, MT OU CT.
            if (produto.UnidadeMedida.Sigla == "KT" || produto.UnidadeMedida.Sigla == "MT" || produto.UnidadeMedida.Sigla == "CT")
                conferenciaResponse.Mensagem = "Atenção! Verificar o tipo da peça ou quantidade antes da conferência.";

            conferenciaResponse.Lote = lote;
            conferenciaResponse.Produto = produto;
            conferenciaResponse.EmpresaConfig = empresaConfig;
            conferenciaResponse.ProdutoEstoque = _uow.ProdutoEstoqueRepository.ObterPorProdutoEmpresa(produto.IdProduto, idEmpresa);

            return conferenciaResponse;
        }

        public ConferenciaResponse ValidarConferencia(long idLote, string codigoBarrasOuReferencia, long idEmpresa, int idTipoConferencia, int quantidadePorCaixa, int quantidadeCaixa, decimal multiplo)
        {
            conferenciaResponse.Sucesso = true;

            //Faz as validações inicias do produto.
            var conferenciaProduto = ValidarProduto(idLote, codigoBarrasOuReferencia, idEmpresa);

            var tipoConferencia = (TipoConferenciaEnum)idTipoConferencia;

            //Valida se o campo quantidade de caixa é igual a 1 quando o tipo da conferência é 100%.
            //Isso é feito porque na conferência 100%, a quantidade de caixa não pode ser maior que 1.
            if (tipoConferencia == TipoConferenciaEnum.ConferenciaCemPorcento)
            {
                if (quantidadeCaixa != 1)
                {
                    conferenciaResponse.Sucesso = false;
                    conferenciaResponse.Mensagem = "Neste tipo de conferência, não é permitido um valor diferente de 1 no campo quantidade de caixa. Por favor, tente novamente!";

                    return conferenciaResponse;
                }

                //Valida se a quantidae por caixa é menor que 1.
                if (quantidadePorCaixa < 1)
                {
                    conferenciaResponse.Sucesso = false;
                    conferenciaResponse.Mensagem = "Neste tipo de conferência, não é permitido um valor menor que 1 no campo quantidade por caixa. Por favor, tente novamente!";

                    return conferenciaResponse;
                }
            }

            //Valida se a quantidade por caixa e quantidade de caixa é igual a 0.
            if (quantidadePorCaixa == 0 || quantidadeCaixa == 0)
            {
                conferenciaResponse.Sucesso = false;
                conferenciaResponse.Mensagem = "Os campos quantidade por caixa e quantidade de caixa não podem ser 0. Por favor, tente novamente!";

                return conferenciaResponse;
            }

            //Valida se o múltilo é igual ou menor 0.
            if (multiplo <= 0)
            {
                conferenciaResponse.Sucesso = false;
                conferenciaResponse.Mensagem = "Múltiplo inválido. Por favor, tente novamente!";

                return conferenciaResponse;
            }

            if (quantidadePorCaixa < 0)
            {
                var totalConferido = _uow.LoteConferenciaRepository.ObterPorProduto(conferenciaProduto.Lote.IdLote, conferenciaProduto.Produto.IdProduto).Sum(x => x.Quantidade);

                if (totalConferido - quantidadePorCaixa * -1 < 0)
                {
                    conferenciaResponse.Sucesso = false;
                    conferenciaResponse.Mensagem = "A quantidade por caixa não pode ser menor que a quantidade conferida. Por favor, tente novamente!";

                    return conferenciaResponse;
                }
            }

            conferenciaResponse.Lote = conferenciaProduto.Lote;
            conferenciaResponse.Produto = conferenciaProduto.Produto;

            return conferenciaResponse;
        }

        public async Task<ConferenciaResponse> RegistrarConferencia(Lote lote, Produto produto, string idUsuario, string inicioConferencia, int idTipoConferencia, int quantidadePorCaixa, int quantidadeCaixa)
        {
            try
            {
                if (lote.IdLoteStatus != LoteStatusEnum.Conferencia)
                {
                    lote.IdLoteStatus = LoteStatusEnum.Conferencia;
                    _uow.LoteRepository.Update(lote);

                    await _loteService.AtualizarNotaFiscalIntegracao(lote.NotaFiscal, lote.IdLoteStatus);
                }

                if (!DateTime.TryParse(inicioConferencia, out DateTime dataHoraInicioConferencia))
                {
                    dataHoraInicioConferencia = DateTime.Now;
                }

                DateTime dataHoraFimConferencia = DateTime.Now;
                TimeSpan _tempoConferencia = dataHoraFimConferencia - dataHoraInicioConferencia;
                DateTime tempoConferencia = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, _tempoConferencia.Hours, _tempoConferencia.Minutes, _tempoConferencia.Seconds);

                LoteConferencia loteConferencia = new LoteConferencia()
                {
                    IdLote = lote.IdLote,
                    IdTipoConferencia = (TipoConferenciaEnum)idTipoConferencia,
                    IdProduto = produto.IdProduto,
                    Quantidade = quantidadePorCaixa * quantidadeCaixa,
                    DataHoraInicio = dataHoraInicioConferencia,
                    DataHoraFim = dataHoraFimConferencia,
                    Tempo = tempoConferencia,
                    IdUsuarioConferente = idUsuario
                };

                _uow.LoteConferenciaRepository.Add(loteConferencia);

                _uow.SaveChanges();

                conferenciaResponse.Lote = lote;
                conferenciaResponse.Produto = produto;
            }
            catch (Exception)
            {
                throw;
            }

            return conferenciaResponse;
        }

        public void ConsultarQuantidadeConferidaENaoConferida(Lote lote, Produto produto, ref int quantidadeConferida, ref int quantidadeNaoConferida)
        {
            int quantidadeNota = 0;

            //NotaFiscalItem do produto.
            var referenciaNota = _uow.NotaFiscalItemRepository.ObterPorItem(lote.IdNotaFiscal, produto.IdProduto);

            //Lote Conferência do produto.
            var referenciaConferencia = _uow.LoteConferenciaRepository.ObterPorProduto(lote.IdLote, produto.IdProduto);

            //Captura a quantidade do produto da nota.
            if (referenciaNota.Any())
                quantidadeNota = referenciaNota.Sum(x => x.Quantidade);

            //Captura a quantidade conferida do produto.
            if (referenciaConferencia.Any())
                quantidadeConferida = referenciaConferencia.Sum(x => x.Quantidade);

            //Calcula a quantidade não conferida (quantidade da nota - quantidade conferida)
            if (quantidadeNota > quantidadeConferida)
                quantidadeNaoConferida = quantidadeNota - quantidadeConferida;
        }
    }
}
