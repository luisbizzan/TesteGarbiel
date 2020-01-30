using FWLog.Data;
using FWLog.Data.EnumsAndConsts;
using FWLog.Data.Models;
using FWLog.Services.Integracao;
using FWLog.Services.Model.IntegracaoSankhya;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FWLog.Services.Services
{
    public class ProdutoService : BaseService
    {
        private UnitOfWork _uow;

        public ProdutoService(UnitOfWork uow)
        {
            _uow = uow;
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

            List<Model.IntegracaoSankhya.ProdutoIntegracao> produtosIntegracao = await IntegracaoSankhya.Instance.PreExecutarQuery<Model.IntegracaoSankhya.ProdutoIntegracao>(where: where.ToString());

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



                    bool atualizacaoOK = await IntegracaoSankhya.Instance.AtualizarInformacaoIntegracao("Produto", "CODPROD", produto.CodigoIntegracao, "AD_INTEGRARFWLOG", "0");

                    if (!atualizacaoOK)
                    {
                        throw new Exception("A atualização de Produto no Sankhya não terminou com sucesso.");
                    }

                    if (produtoNovo)
                    {
                        _uow.ProdutoRepository.Add(produto);

                        List<Empresa> empresas = _uow.EmpresaRepository.Tabela().ToList();

                        foreach (Empresa empresa in empresas)
                        {
                            var produtoEstoque = new Data.Models.ProdutoEstoque
                            {
                                IdProduto = produto.IdProduto,
                                IdEmpresa = empresa.IdEmpresa,
                                Saldo = 0
                            };

                            _uow.ProdutoEstoqueRepository.Add(produtoEstoque);
                        }
                    }

                    _uow.SaveChanges();
                }
                catch (Exception ex)
                {
                    var applicationLogService = new ApplicationLogService(_uow);
                    applicationLogService.Error(ApplicationEnum.Api, ex, string.Format("Erro na integração do Produto: {0}.", produtoInt.CodigoIntegracao));

                    continue;
                }
            }
        }

        public async Task ConsultarProdutoPrazoEntrega()
        {
            //string where = "WHERE AD_INTEGRARFWLOG = '1' ";

            List <ProdutoPrazoEntregaIntegracao> produtoPrazoEntregaIntegracao = await IntegracaoSankhya.Instance.PreExecutarQuery<ProdutoPrazoEntregaIntegracao>();

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

                    Data.Models.ProdutoEstoque produtoEstoque = _uow.ProdutoEstoqueRepository.ObterPorProdutoEmpresa(produto.IdProduto, empresa.IdEmpresa);

                    if (produtoEstoque == null)
                    {
                        produtoEstoqueNovo = true;
                        produtoEstoque = new Data.Models.ProdutoEstoque();

                        produtoEstoque.IdEmpresa = empresa.IdEmpresa;
                        produtoEstoque.IdProduto = produto.IdProduto;
                    }

                    produtoEstoque.DiasPrazoEntrega = Convert.ToInt32(produtoInt.DiasPrazoEntrega);

                    if (produtoEstoqueNovo)
                    {
                        _uow.ProdutoEstoqueRepository.Add(produtoEstoque);
                    }

                    await _uow.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    var applicationLogService = new ApplicationLogService(_uow);
                    applicationLogService.Error(ApplicationEnum.Api, ex, string.Format("Erro gerado na integração do Prazo de Entrega do Produto {0} Empresa: {1}.", produtoInt.CodigoIntegracaoProduto, produtoInt.CodigoIntegracaoEmpresa));

                    continue;
                }
            }
        }
    }
}
