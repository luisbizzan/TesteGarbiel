﻿using FWLog.Data;
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
    public class ProdutoEmpresaService : BaseService
    {
        private UnitOfWork _uow;

        public ProdutoEmpresaService(UnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task ConsultarProdutoEmpresaIntegracao()
        {
            if (!Convert.ToBoolean(ConfigurationManager.AppSettings["IntegracaoSankhya_Habilitar"]))
            {
                return;
            }

            List<ProdutoEmpresaIntegracao> produtoEmpresaIntegracao = await IntegracaoSankhya.Instance.PreExecutarQuery<ProdutoEmpresaIntegracao>();

            foreach (var produtoEmpresaInt in produtoEmpresaIntegracao)
            {
                try
                {
                    var empresa = !String.IsNullOrEmpty(produtoEmpresaInt.CodigoIntegracaoEmpresa) ? _uow.EmpresaRepository.ConsultaPorCodigoIntegracao(Convert.ToInt32(produtoEmpresaInt.CodigoIntegracaoEmpresa)) : null;
                    var produto = !String.IsNullOrEmpty(produtoEmpresaInt.CodigoIntegracaoProduto) ? _uow.ProdutoRepository.ConsultarPorCodigoIntegracao(Convert.ToInt32(produtoEmpresaInt.CodigoIntegracaoProduto)) : null;

                    if (empresa != null && produto != null)
                    {
                        bool produtoEmpresaNovo = false;

                        ProdutoEmpresa produtoEmpresa = _uow.ProdutoEmpresaRepository.ConsultarPorProdutoEmpresa(produto.IdProduto, empresa.IdEmpresa);

                        if (produtoEmpresa == null)
                        {
                            produtoEmpresaNovo = true;
                            produtoEmpresa = new ProdutoEmpresa();
                        }

                        produtoEmpresa.IdEmpresa = empresa.IdEmpresa;
                        produtoEmpresa.IdProduto = produto.IdProduto;
                        produtoEmpresa.Ativo = produtoEmpresaInt.Ativo == "S" ? true : false;

                        if (produtoEmpresaNovo)
                        {
                            _uow.ProdutoEmpresaRepository.Add(produtoEmpresa);
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
