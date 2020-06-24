using DartDigital.Library.Exceptions;
using FWLog.Data;
using FWLog.Data.Models;
using FWLog.Services.Integracao;
using FWLog.Services.Model.IntegracaoSankhya;
using FWLog.Services.Model.Produto;
using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FWLog.Services.Services
{
    public class ProdutoService : BaseService
    {
        private UnitOfWork _uow;
        private ILog _log;

        public ProdutoService(UnitOfWork uow, ILog log)
        {
            _uow = uow;
            _log = log;
        }

        public async Task ConsultarProdutoIntegracao(bool somenteNovos)
        {
            if (!Convert.ToBoolean(ConfigurationManager.AppSettings["IntegracaoSankhya_Habilitar"]))
            {
                return;
            }

            var where = new StringBuilder();

            if (somenteNovos)
            {
                where.Append("WHERE AD_INTEGRARFWLOG = '1' ");
            }
            else
            {
                where.Append("WHERE AD_INTEGRARFWLOG = '0' ");
            }

            int quantidadeChamadas = 0;

            var produtoQuantidadeRegistroIntegracao = await IntegracaoSankhya.Instance.PreExecutarQuery<ProdutoQuantidadeRegistroIntegracao>(where: where.ToString());

            if (produtoQuantidadeRegistroIntegracao != null)
            {
                try
                {
                    decimal contadorRegistros = Convert.ToInt32(produtoQuantidadeRegistroIntegracao[0].QuantidadeRegistro);

                    if (contadorRegistros < 4999)
                    {
                        quantidadeChamadas = 1;
                    }
                    else
                    {
                        decimal div = contadorRegistros / 4999;
                        quantidadeChamadas = Convert.ToInt32(Math.Ceiling(div));
                    }

                }
                catch (Exception ex)
                {
                    _log.Error("Erro na integração de Produtos", ex);
                    return;
                }
            }

            int offsetRows = 0;
            var produtosIntegracao = new List<ProdutoIntegracao>();
            var unidadesMedida = _uow.UnidadeMedidaRepository.RetornarTodos();
            List<Empresa> empresas = _uow.EmpresaRepository.Tabela().Where(emp => !string.IsNullOrEmpty(emp.Sigla)).ToList();

            for (int i = 0; i < quantidadeChamadas; i++)
            {
                where = new StringBuilder();

                if (somenteNovos)
                {
                    where.Append("WHERE AD_INTEGRARFWLOG = '1' ");
                }
                else
                {
                    where.Append("WHERE AD_INTEGRARFWLOG = '0' ");
                }

                where.Append("ORDER BY CODPROD OFFSET " + offsetRows + " ROWS FETCH NEXT 4999 ROWS ONLY ");

                produtosIntegracao.AddRange(await IntegracaoSankhya.Instance.PreExecutarQuery<ProdutoIntegracao>(where: where.ToString()));

                offsetRows += 4999;
            }

            foreach (var produtoInt in produtosIntegracao)
            {
                try
                {
                    ValidarDadosIntegracao(produtoInt);

                    var unidade = unidadesMedida.FirstOrDefault(f => f.Sigla == produtoInt.UnidadeMedidaSigla);
                    if (unidade == null)
                    {
                        string whereUnidade = "WHERE TGFVOL.CODVOL = '" + produtoInt.UnidadeMedidaSigla + "' ";
                        var unidadeMedidaIntegrar = await IntegracaoSankhya.Instance.PreExecutarQuery<UnidadeMedidaIntegracao>(where: whereUnidade);

                        if (unidadeMedidaIntegrar != null && unidadeMedidaIntegrar.Count > 0)
                        {
                            var unidadeMedida = new UnidadeMedida
                            {
                                Sigla = unidadeMedidaIntegrar[0].Sigla,
                                Descricao = unidadeMedidaIntegrar[0].Descricao
                            };

                            _uow.UnidadeMedidaRepository.Add(unidadeMedida);
                            _uow.SaveChanges();
                        }
                        else
                        {
                            throw new Exception("Código da Unidade de Medida (CODVOL: " + produtoInt.UnidadeMedidaSigla + ") inválido.");
                        }
                    }

                    bool produtoNovo = false;

                    var codProd = Convert.ToInt64(produtoInt.CodigoIntegracao);
                    if (codProd == 0)
                    {
                        throw new Exception("Código da Produto (CODPROD: 0) inválido.");
                    }

                    Produto produto = _uow.ProdutoRepository.ConsultarPorCodigoIntegracao(codProd);

                    if (produto == null)
                    {
                        produtoNovo = true;
                        produto = new Produto();
                    }

                    produto.Altura = produtoInt.Altura == null ? (decimal?)null : Convert.ToDecimal(produtoInt.Altura.Replace(".", ","));
                    produto.Ativo = produtoInt.Ativo == "S" ? true : false;
                    produto.CodigoFabricante = produtoInt.CodigoFabricante == null ? (long?)null : Convert.ToInt64(produtoInt.CodigoFabricante);
                    produto.CodigoIntegracao = codProd;
                    produto.CodigoProdutoNFE = Convert.ToInt32(produtoInt.CodigoProdutoNFE);
                    produto.Comprimento = produtoInt.Comprimento == null ? (decimal?)null : Convert.ToDecimal(produtoInt.Comprimento.Replace(".", ","));
                    produto.Descricao = produtoInt.Descricao;
                    produto.EnderecoImagem = produtoInt.EnderecoImagem;
                    produto.Largura = produtoInt.Largura == null ? (decimal?)null : Convert.ToDecimal(produtoInt.Largura.Replace(".", ","));
                    produto.MultiploVenda = Convert.ToDecimal(produtoInt.MultiploVenda.Replace(".", ","));
                    produto.NomeFabricante = produtoInt.NomeFabricante;
                    produto.PesoBruto = Convert.ToDecimal(produtoInt.PesoBruto.Replace(".", ","));
                    produto.PesoLiquido = Convert.ToDecimal(produtoInt.PesoLiquido.Replace(".", ","));
                    produto.Referencia = produtoInt.Referencia;
                    produto.ReferenciaFornecedor = produtoInt.ReferenciaFornecedor;
                    produto.CodigoBarras = produtoInt.CodigoBarras;
                    produto.IdUnidadeMedida = unidade.IdUnidadeMedida;
                    produto.CodigoBarras2 = produtoInt.CodigoBarras2;
                    produto.IsEmbalagemFornecedor = produtoInt.IsEmbalagemFornecedor == "S" ? true : false;
                    produto.IsEmbalagemFornecedorVolume = produtoInt.IsEmbalagemFornecedorVolume == "S" ? true : false;

                    if (produtoInt.MetroCubico == null)
                    {
                        var largura = produto.Largura.HasValue ? produto.Largura.Value : 0;
                        var comprimento = produto.Comprimento.HasValue ? produto.Comprimento.Value : 0;
                        var altura = produto.Altura.HasValue ? produto.Altura.Value : 0;

                        produto.MetroCubico = largura * comprimento * altura;
                    }
                    else
                    {
                        produto.MetroCubico = Convert.ToDecimal(produtoInt.MetroCubico.Replace(".", ","));
                    }

                    if (produtoNovo)
                    {
                        _uow.ProdutoRepository.Add(produto);

                        foreach (Empresa empresa in empresas)
                        {
                            var produtoEstoque = new ProdutoEstoque
                            {
                                IdProduto = produto.IdProduto,
                                IdEmpresa = empresa.IdEmpresa,
                                Saldo = 0,
                                IdProdutoEstoqueStatus = ProdutoEstoqueStatusEnum.Ativo,
                                DiasPrazoEntrega = 0
                            };

                            _uow.ProdutoEstoqueRepository.Add(produtoEstoque);
                        }
                    }
                    else
                    {
                        _uow.ProdutoRepository.Update(produto);
                    }

                    _uow.SaveChanges();

                    if (somenteNovos)
                    {
                        var campoChave = new Dictionary<string, string> { { "CODPROD", produto.CodigoIntegracao.ToString() } };
                        await IntegracaoSankhya.Instance.AtualizarInformacaoIntegracao("Produto", campoChave, "AD_INTEGRARFWLOG", "0");
                    }
                }
                catch (Exception ex)
                {
                    _log.Error(string.Format("Erro na integração do Produto: {0}.", produtoInt.CodigoIntegracao), ex);
                }
            }
        }

        public async Task ConsultarProdutoPrazoEntrega(bool somenteNovos)
        {
            if (!Convert.ToBoolean(ConfigurationManager.AppSettings["IntegracaoSankhya_Habilitar"]))
            {
                return;
            }

            var where = new StringBuilder();

            if (somenteNovos)
            {
                where.Append("WHERE AD_INTEGRARFWLOG = '1' ");
            }
            else
            {
                where.Append("WHERE AD_INTEGRARFWLOG = '0' ");
            }

            int quantidadeChamadas = 0;

            var produtoEstoqueQuantidadeRegistroIntegracao = await IntegracaoSankhya.Instance.PreExecutarQuery<ProdutoEstoqueQuantidadeIntegracao>(where: where.ToString());

            if (produtoEstoqueQuantidadeRegistroIntegracao != null)
            {
                try
                {
                    decimal contadorRegistros = Convert.ToInt32(produtoEstoqueQuantidadeRegistroIntegracao[0].QuantidadeRegistro);

                    if (contadorRegistros < 4999)
                    {
                        quantidadeChamadas = 1;
                    }
                    else
                    {
                        decimal div = contadorRegistros / 4999;
                        quantidadeChamadas = Convert.ToInt32(Math.Ceiling(div));
                    }

                }
                catch (Exception ex)
                {
                    _log.Error("Erro na integração de prazo de entrega", ex);
                    return;
                }
            }

            int offsetRows = 0;
            var produtoPrazoEntregaIntegracao = new List<ProdutoEstoqueIntegracao>();

            for (int i = 0; i < quantidadeChamadas; i++)
            {
                where = new StringBuilder();

                if (somenteNovos)
                {
                    where.Append("WHERE AD_INTEGRARFWLOG = '1' ");
                }
                else
                {
                    where.Append("WHERE AD_INTEGRARFWLOG = '0' ");
                }

                where.Append("ORDER BY CODPROD OFFSET " + offsetRows + " ROWS FETCH NEXT 4999 ROWS ONLY ");

                produtoPrazoEntregaIntegracao.AddRange(await IntegracaoSankhya.Instance.PreExecutarQuery<ProdutoEstoqueIntegracao>(where: where.ToString()));

                offsetRows += 4999;
            }

            foreach (var produtoInt in produtoPrazoEntregaIntegracao)
            {
                try
                {
                    ValidarDadosIntegracao(produtoInt);

                    Empresa empresa = _uow.EmpresaRepository.ConsultaPorCodigoIntegracao(Convert.ToInt32(produtoInt.CodigoIntegracaoEmpresa));
                    if (empresa == null)
                    {
                        throw new Exception(string.Format("Código da Empresa (CODEMP: {0}) inválido", produtoInt.CodigoIntegracaoEmpresa));
                    }

                    Produto produto = _uow.ProdutoRepository.ConsultarPorCodigoIntegracao(Convert.ToInt32(produtoInt.CodigoIntegracaoProduto));
                    if (produto == null)
                    {
                        throw new Exception(string.Format("Código da Produto (CODPROD: {0}) inválido", produtoInt.CodigoIntegracaoProduto));
                    }

                    bool produtoEstoqueNovo = false;

                    ProdutoEstoque produtoEstoque = _uow.ProdutoEstoqueRepository.ObterPorProdutoEmpresa(produto.IdProduto, empresa.IdEmpresa);

                    if (produtoEstoque == null)
                    {
                        produtoEstoqueNovo = true;
                        produtoEstoque = new ProdutoEstoque
                        {
                            IdEmpresa = empresa.IdEmpresa,
                            IdProduto = produto.IdProduto
                        };
                    }

                    produtoEstoque.DiasPrazoEntrega = Convert.ToInt32(produtoInt.DiasPrazoEntrega);
                    produtoEstoque.IdProdutoEstoqueStatus = (ProdutoEstoqueStatusEnum)Convert.ToInt32(produtoInt.Status);

                    if (produtoEstoqueNovo)
                    {
                        _uow.ProdutoEstoqueRepository.Add(produtoEstoque);
                    }

                    await _uow.SaveChangesAsync();

                    if (somenteNovos)
                    {
                        var chaves = new Dictionary<string, string> { { "CODPROD", produto.CodigoIntegracao.ToString() }, { "CODEMP", empresa.CodigoIntegracao.ToString() } };
                        await IntegracaoSankhya.Instance.AtualizarInformacaoIntegracao("EmpresaProdutoImpostos", chaves, "AD_INTEGRARFWLOG", "0");
                    }
                }
                catch (Exception ex)
                {
                    _log.Error(string.Format("Erro integração Prazo de Entrega Produto {0} Empresa: {1}.", produtoInt.CodigoIntegracaoProduto, produtoInt.CodigoIntegracaoEmpresa), ex);
                }
            }
        }

        public async Task ConsultarMediaVenda()
        {
            string where = "WHERE AD_INTEGRARFWLOG = '1' ";

            List<ProdutoMediaVendaIntegracao> produtoMediaVenda = await IntegracaoSankhya.Instance.PreExecutarQuery<ProdutoMediaVendaIntegracao>(where);

            foreach (var produtoInt in produtoMediaVenda)
            {
                try
                {
                    ValidarDadosIntegracao(produtoInt);

                    Empresa empresa = _uow.EmpresaRepository.ConsultaPorCodigoIntegracao(Convert.ToInt32(produtoInt.CodigoIntegracaoEmpresa));

                    if (empresa == null)
                    {
                        throw new Exception(string.Format("Código da Empresa (CODEMP: {0}) inválido", produtoInt.CodigoIntegracaoEmpresa));
                    }

                    Produto produto = _uow.ProdutoRepository.ConsultarPorCodigoIntegracao(Convert.ToInt32(produtoInt.CodigoIntegracaoProduto));

                    if (produto == null)
                    {
                        throw new Exception(string.Format("Código da Produto (CODPROD: {0}) inválido", produtoInt.CodigoIntegracaoProduto));
                    }

                    ProdutoEstoque produtoEstoque = _uow.ProdutoEstoqueRepository.ObterPorProdutoEmpresa(produto.IdProduto, empresa.IdEmpresa);

                    if (produtoEstoque != null)
                    {
                        if (!String.IsNullOrEmpty(produtoInt.MediaVenda))
                        {
                            produtoEstoque.MediaVenda = double.Parse(produtoInt.MediaVenda, CultureInfo.InvariantCulture);
                        }
                        else
                            produtoEstoque.MediaVenda = null;
                    }

                    await _uow.SaveChangesAsync();

                    Dictionary<string, string> chaves = new Dictionary<string, string> { { "CODPROD", produto.CodigoIntegracao.ToString() }, { "CODEMP", empresa.CodigoIntegracao.ToString() } };

                    await IntegracaoSankhya.Instance.AtualizarInformacaoIntegracao("AD_MEDVENDPRO", chaves, "AD_INTEGRARFWLOG", "0");
                }
                catch (Exception ex)
                {
                    _log.Error(string.Format("Erro gerado na integração da Média de Venda do Produto {0} Empresa: {1}.", produtoInt.CodigoIntegracaoProduto, produtoInt.CodigoIntegracaoEmpresa), ex);
                }
            }
        }

        public async Task<int> ConsultarQuantidadeReservada(long idProduto, long idEmpresa)
        {
            if (!Convert.ToBoolean(ConfigurationManager.AppSettings["IntegracaoSankhya_Habilitar"]))
            {
                return 0;
            }

            int quantidadeReservada = 0;

            Empresa empresa = _uow.EmpresaRepository.ConsultaPorId(idEmpresa);
            Produto produto = _uow.ProdutoRepository.GetById(idProduto);

            if (empresa != null && produto != null)
            {
                string where = "WHERE CODEMP = '" + empresa.CodigoIntegracao + "' AND CODPROD = '" + produto.CodigoIntegracao + "' ";

                List<ProdutoReservadoIntegracao> produtoReservado = await IntegracaoSankhya.Instance.PreExecutarQuery<ProdutoReservadoIntegracao>(where);

                foreach (var produtoInt in produtoReservado)
                {
                    try
                    {
                        ValidarDadosIntegracao(produtoInt);

                        quantidadeReservada = Convert.ToInt32(produtoInt.Reservado);
                    }
                    catch (Exception ex)
                    {
                        _log.Error(string.Format("Erro gerado na integração da Reserva do Produto {0} Empresa: {1}.", produtoInt.CodigoIntegracaoProduto, produtoInt.CodigoIntegracaoEmpresa), ex);
                    }
                }
            }

            return quantidadeReservada;
        }

        public async Task<ProdutoEstoqueResposta> ConsultarProdutoEstoque(long idProduto, long idEmpresa)
        {
            var produto = _uow.ProdutoRepository.GetById(idProduto);
            if (produto == null)
            {
                throw new BusinessException("Produto não encontrado.");
            }

            var produtoEstoque = _uow.ProdutoEstoqueRepository.ObterPorProdutoEmpresa(idProduto, idEmpresa);
            if (produtoEstoque == null)
            {
                throw new BusinessException("Produto não configurado para a empresa selecionada.");
            }

            var qtdreservada = await ConsultarQuantidadeReservada(produto.IdProduto, idEmpresa);

            var resposta = new ProdutoEstoqueResposta()
            {
                Referencia = produto.Referencia,
                IdProduto = produto.IdProduto,
                QtdEstoque = produtoEstoque.Saldo,
                QtdReservada = qtdreservada,
                Saldo = produtoEstoque.Saldo - qtdreservada,
                PontosArmazenagem = new List<PontoArmazenagemResposta>()
            };

            var loteProdutoEndereco = _uow.LoteProdutoEnderecoRepository.PesquisarPorProduto(idProduto, idEmpresa);
            if (loteProdutoEndereco != null)
            {
                var pontosArmazenagem = loteProdutoEndereco.GroupBy(g => g.EnderecoArmazenagem.IdPontoArmazenagem).ToDictionary(g => g.Key, g => g.ToList());

                foreach (var item in pontosArmazenagem)
                {
                    resposta.PontosArmazenagem.Add(new PontoArmazenagemResposta
                    {
                        Descricao = item.Value.First().EnderecoArmazenagem.PontoArmazenagem.Descricao,
                        IdPontoArmazenagem = item.Key,
                        Saldo = item.Value.Sum(s => s.Quantidade)
                    });
                }
            }

            return resposta;
        }

        public EntradasProdutoResposta ConsultarEntradasProduto(long idProduto, long idEmpresa)
        {
            if (idProduto <= 0)
            {
                throw new BusinessException("Produto deve ser informado.");
            }

            var produto = _uow.ProdutoRepository.GetById(idProduto);

            if (produto == null)
            {
                throw new BusinessException("Produto não encontrado.");
            }

            var entradasProduto = _uow.ProdutoRepository.ConsultarEntradasProduto(idProduto, idEmpresa);

            var dadosAgrupados = entradasProduto.GroupBy(g => g.DataFinalConferenciaLote.Date).OrderBy(g => g.Key).ToList();

            var resultado = new EntradasProdutoResposta
            {
                IdProduto = produto.IdProduto,
                ReferenciaProduto = produto.Referencia
            };

            resultado.ListaEntradas = dadosAgrupados.Select(da => new EntradasProdutoItemResposta
            {
                DataEntrada = da.Key,
                QuantidadeEntrada = da.Sum(i => i.QuantidadeRecebidaLoteProduto)
            }).ToList();

            return resultado;
        }
    }
}