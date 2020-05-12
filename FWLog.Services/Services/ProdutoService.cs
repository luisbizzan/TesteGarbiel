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

        public async Task LimparIntegracao()
        {
            if (!Convert.ToBoolean(ConfigurationManager.AppSettings["IntegracaoSankhya_Habilitar"]))
            {
                return;
            }

            StringBuilder where = new StringBuilder();

            where.Append("WHERE DESCRPROD IS NOT NULL ");
            where.Append("AND CODPROD IS NOT NULL AND CODPROD <> 0 ");
            where.Append("AND AD_INTEGRARFWLOG = '0' ");
            where.Append("ORDER BY CODPROD OFFSET 0 ROWS FETCH NEXT 5000 ROWS ONLY ");

            List<Model.IntegracaoSankhya.ProdutoIntegracao> produtosIntegracao = await IntegracaoSankhya.Instance.PreExecutarQuery<Model.IntegracaoSankhya.ProdutoIntegracao>(where: where.ToString());

            foreach (var produtoInt in produtosIntegracao)
            {
                Dictionary<string, string> campoChave = new Dictionary<string, string> { { "CODPROD", produtoInt.CodigoIntegracao.ToString() } };

                await IntegracaoSankhya.Instance.AtualizarInformacaoIntegracao("Produto", campoChave, "AD_INTEGRARFWLOG", "1");
            }
        }

        public async Task ConsultarProdutoIntegracao()
        {
            if (!Convert.ToBoolean(ConfigurationManager.AppSettings["IntegracaoSankhya_Habilitar"]))
            {
                return;
            }

            StringBuilder where = new StringBuilder();

            where.Append("WHERE DESCRPROD IS NOT NULL ");
            where.Append("AND CODPROD IS NOT NULL AND CODPROD <> 0 ");
            where.Append("AND AD_INTEGRARFWLOG = '1' ");

            int quantidadeRegistro = 4999;
            int quantidadeChamada = 0;

            List<ProdutoQuantidadeRegistroIntegracao> produtoQuantidadeRegistroIntegracao = await IntegracaoSankhya.Instance.PreExecutarQuery<Model.IntegracaoSankhya.ProdutoQuantidadeRegistroIntegracao>(where: where.ToString());

            if (produtoQuantidadeRegistroIntegracao != null)
            {
                try
                {
                    quantidadeRegistro = Convert.ToInt32(produtoQuantidadeRegistroIntegracao[0].QuantidadeRegistro);
                }
                catch { }
            }

            quantidadeChamada = quantidadeRegistro / 4999;

            where.Append("ORDER BY CODPROD OFFSET 0 ROWS FETCH NEXT 5000 ROWS ONLY ");

            for (int i = 0; i < quantidadeChamada; i++)
            {
                List<ProdutoIntegracao> produtosIntegracao = await IntegracaoSankhya.Instance.PreExecutarQuery<Model.IntegracaoSankhya.ProdutoIntegracao>(where: where.ToString());

                var unidadesMedida = _uow.UnidadeMedidaRepository.RetornarTodos();

                foreach (var produtoInt in produtosIntegracao)
                {
                    try
                    {
                        ValidarDadosIntegracao(produtoInt);

                        var unidade = unidadesMedida.FirstOrDefault(f => f.Sigla == produtoInt.UnidadeMedidaSigla);

                        if (unidade == null)
                        {
                            throw new Exception("Código da Unidade de Medida (CODVOL) inválido");
                        }

                        bool produtoNovo = false;

                        var codProd = Convert.ToInt64(produtoInt.CodigoIntegracao);
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
                        produto.MetroCubico = produtoInt.MetroCubico == null ? (decimal?)null : Convert.ToDecimal(produtoInt.MetroCubico.Replace(".", ","));
                        produto.MultiploVenda = Convert.ToDecimal(produtoInt.MultiploVenda.Replace(".", ","));
                        produto.NomeFabricante = produtoInt.NomeFabricante;
                        produto.PesoBruto = Convert.ToDecimal(produtoInt.PesoBruto.Replace(".", ",")) / 1000;
                        produto.PesoLiquido = Convert.ToDecimal(produtoInt.PesoLiquido.Replace(".", ",")) / 1000;
                        produto.Referencia = produtoInt.Referencia;
                        produto.ReferenciaFornecedor = produtoInt.ReferenciaFornecedor;
                        produto.CodigoBarras = produtoInt.CodigoBarras;
                        produto.IdUnidadeMedida = unidade.IdUnidadeMedida;
                        produto.CodigoBarras2 = produtoInt.CodigoBarras2;

                        Dictionary<string, string> campoChave = new Dictionary<string, string> { { "CODPROD", produto.CodigoIntegracao.ToString() } };

                        await IntegracaoSankhya.Instance.AtualizarInformacaoIntegracao("Produto", campoChave, "AD_INTEGRARFWLOG", "0");

                        if (produtoNovo)
                        {
                            _uow.ProdutoRepository.Add(produto);

                            List<Empresa> empresas = _uow.EmpresaRepository.Tabela().Where(emp => !string.IsNullOrEmpty(emp.Sigla)).ToList();

                            foreach (Empresa empresa in empresas)
                            {
                                var produtoEstoque = new ProdutoEstoque
                                {
                                    IdProduto = produto.IdProduto,
                                    IdEmpresa = empresa.IdEmpresa,
                                    Saldo = 0,
                                    IdProdutoEstoqueStatus = ProdutoEstoqueStatusEnum.Ativo,
                                    DiasPrazoEntrega = 10
                                };

                                _uow.ProdutoEstoqueRepository.Add(produtoEstoque);
                            }
                        }
                        else
                        {
                            _uow.ProdutoRepository.Update(produto);
                        }

                        _uow.SaveChanges();

                    }
                    catch (Exception ex)
                    {
                        _log.Error(string.Format("Erro na integração do Produto: {0}.", produtoInt.CodigoIntegracao), ex);
                    }
                }

                i++;
            }
        }

        public async Task LimparIntegracaoPrazoEntrega()
        {
            if (!Convert.ToBoolean(ConfigurationManager.AppSettings["IntegracaoSankhya_Habilitar"]))
            {
                return;
            }

            string where = "WHERE AD_INTEGRARFWLOG = '0' ";

            List<ProdutoEstoqueIntegracao> produtoPrazoEntregaIntegracao = await IntegracaoSankhya.Instance.PreExecutarQuery<ProdutoEstoqueIntegracao>(where);

            foreach (var produtoInt in produtoPrazoEntregaIntegracao)
            {
                Dictionary<string, string> chaves = new Dictionary<string, string> { { "CODPROD", produtoInt.CodigoIntegracaoProduto.ToString() }, { "CODEMP", produtoInt.CodigoIntegracaoEmpresa.ToString() } };

                await IntegracaoSankhya.Instance.AtualizarInformacaoIntegracao("EmpresaProdutoImpostos", chaves, "AD_INTEGRARFWLOG", "0");
            }
        }

        public async Task ConsultarProdutoPrazoEntrega()
        {
            string where = "WHERE AD_INTEGRARFWLOG = '1' ";

            List<ProdutoEstoqueIntegracao> produtoPrazoEntregaIntegracao = await IntegracaoSankhya.Instance.PreExecutarQuery<ProdutoEstoqueIntegracao>(where);

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
                        produtoEstoque = new ProdutoEstoque();

                        produtoEstoque.IdEmpresa = empresa.IdEmpresa;
                        produtoEstoque.IdProduto = produto.IdProduto;
                    }

                    produtoEstoque.DiasPrazoEntrega = Convert.ToInt32(produtoInt.DiasPrazoEntrega);
                    produtoEstoque.IdProdutoEstoqueStatus = (ProdutoEstoqueStatusEnum)Convert.ToInt32(produtoInt.Status);

                    if (produtoEstoqueNovo)
                    {
                        _uow.ProdutoEstoqueRepository.Add(produtoEstoque);
                    }

                    await _uow.SaveChangesAsync();

                    Dictionary<string, string> chaves = new Dictionary<string, string> { { "CODPROD", produto.CodigoIntegracao.ToString() }, { "CODEMP", empresa.CodigoIntegracao.ToString() } };

                    await IntegracaoSankhya.Instance.AtualizarInformacaoIntegracao("EmpresaProdutoImpostos", chaves, "AD_INTEGRARFWLOG", "0");
                }
                catch (Exception ex)
                {
                    _log.Error(string.Format("Erro gerado na integração do Prazo de Entrega do Produto {0} Empresa: {1}.", produtoInt.CodigoIntegracaoProduto, produtoInt.CodigoIntegracaoEmpresa), ex);

                    continue;
                }
            }
        }

        public async Task LimparIntegracaoMediaVenda()
        {
            if (!Convert.ToBoolean(ConfigurationManager.AppSettings["IntegracaoSankhya_Habilitar"]))
            {
                return;
            }

            string where = "WHERE AD_INTEGRARFWLOG = '0' ";

            List<ProdutoEstoqueIntegracao> produtoPrazoEntregaIntegracao = await IntegracaoSankhya.Instance.PreExecutarQuery<ProdutoEstoqueIntegracao>(where);

            foreach (var produtoInt in produtoPrazoEntregaIntegracao)
            {
                Dictionary<string, string> chaves = new Dictionary<string, string> { { "CODPROD", produtoInt.CodigoIntegracaoProduto.ToString() }, { "CODEMP", produtoInt.CodigoIntegracaoEmpresa.ToString() } };

                await IntegracaoSankhya.Instance.AtualizarInformacaoIntegracao("EmpresaProdutoImpostos", chaves, "AD_INTEGRARFWLOG", "1");
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