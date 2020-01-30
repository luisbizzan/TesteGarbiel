using FWLog.Data;
using FWLog.Data.EnumsAndConsts;
using FWLog.Data.Models;
using FWLog.Services.Integracao;
using FWLog.Services.Model.IntegracaoSankhya;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;

namespace FWLog.Services.Services
{
    public class ProdutoEstoqueService : BaseService
    {
        private UnitOfWork _uow;

        public ProdutoEstoqueService(UnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task ConsultarProdutoEmpresaIntegracao()
        {
            if (!Convert.ToBoolean(ConfigurationManager.AppSettings["IntegracaoSankhya_Habilitar"]))
            {
                return;
            }

            List<ProdutoEstoqueIntegracao> produtoEmpresaIntegracao = await IntegracaoSankhya.Instance.PreExecutarQuery<ProdutoEstoqueIntegracao>();

            foreach (var produtoEmpresaInt in produtoEmpresaIntegracao)
            {
                try
                {
                    var empresa = !String.IsNullOrEmpty(produtoEmpresaInt.CodigoIntegracaoEmpresa) ? _uow.EmpresaRepository.ConsultaPorCodigoIntegracao(Convert.ToInt32(produtoEmpresaInt.CodigoIntegracaoEmpresa)) : null;
                    var produto = !String.IsNullOrEmpty(produtoEmpresaInt.CodigoIntegracaoProduto) ? _uow.ProdutoRepository.ConsultarPorCodigoIntegracao(Convert.ToInt32(produtoEmpresaInt.CodigoIntegracaoProduto)) : null;

                    if (empresa != null && produto != null)
                    {
                        bool produtoEstoqueNovo = false;

                        ProdutoEstoque produtoEstoque = _uow.ProdutoEstoqueRepository.ObterPorProdutoEmpresa(produto.IdProduto, empresa.IdEmpresa);

                        if (produtoEstoque == null)
                        {
                            produtoEstoqueNovo = true;
                            produtoEstoque = new ProdutoEstoque();
                        }

                        produtoEstoque.IdEmpresa = empresa.IdEmpresa;
                        produtoEstoque.IdProduto = produto.IdProduto;
                        produtoEstoque.IdProdutoEnderedoStatus = Convert.ToInt32(produtoEmpresaInt.Status);

                        if (produtoEstoqueNovo)
                        {
                            _uow.ProdutoEstoqueRepository.Add(produtoEstoque);
                        }

                        await _uow.SaveChangesAsync();
                    }
                }
                catch (Exception ex)
                {
                    var applicationLogService = new ApplicationLogService(_uow);
                    applicationLogService.Error(ApplicationEnum.Api, ex, string.Format("Erro gerado na integração do Produto {0} Empresa: {1}.", produtoEmpresaInt.CodigoIntegracaoProduto, produtoEmpresaInt.CodigoIntegracaoEmpresa));

                    continue;
                }
            }
        }
    }
}
