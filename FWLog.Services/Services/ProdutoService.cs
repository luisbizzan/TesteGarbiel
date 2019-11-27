using FWLog.Data;
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
            //if (!Convert.ToBoolean(ConfigurationManager.AppSettings["IntegracaoSankhya_Habilitar"]))
            //{
            //    return;
            //}

            StringBuilder where = new StringBuilder();

            //where.Append(string.Format(" WHERE DTALTER > to_date('{0}', 'dd-mm-yyyy hh24:mi:ss')", DateTime.UtcNow.ToString("dd-MM-yyyy hh:mm:ss")));
            where.Append("WHERE ROWNUM <= 10 ");//Temporário
            where.Append("AND DESCRPROD IS NOT NULL ");
            where.Append("AND CODPROD IS NOT NULL AND CODPROD <> 0 ");

            List<ProdutoIntegracao> produtosIntegracao = await IntegracaoSankhya.Instance.PreExecutarQueryGenerico<ProdutoIntegracao>(where: where.ToString());

            var unidadesMedida = _uow.UnidadeMedidaRepository.GetAll();

            foreach (var produtoInt in produtosIntegracao)
            {
                ValidarProdutoIntegracao(produtoInt);

                bool produtoNovo = false;

                var codProd = Convert.ToInt64(produtoInt.CODPROD);
                Produto produto = _uow.ProdutoRepository.ConsultarPorCodigoIntegracao(codProd);

                if (produto == null)
                {
                    produtoNovo = true;
                    produto = new Produto();
                }

                produto.Altura = produtoInt.ALTURA == null ? (decimal?)null : Convert.ToDecimal(produtoInt.ALTURA);
                produto.Ativo = produtoInt.ATIVO == "S" ? true: false;
                produto.CodigoFabricante = produtoInt.CODFAB == null ? (long?)null : Convert.ToInt64(produtoInt.CODFAB);
                produto.CodigoIntegracao = codProd;
                produto.CodigoProdutoNFE = Convert.ToInt32(produtoInt.PRODUTONFE);
                produto.Comprimento = produtoInt.ESPESSURA == null ? (decimal?)null : Convert.ToDecimal(produtoInt.ESPESSURA);
                produto.Descricao = produtoInt.DESCRPROD;
                produto.DescricaoNFE = produtoInt.DESCRPRODNFE;
                produto.EnderecoImagem = produtoInt.ENDIMAGEM;
                produto.Largura = produtoInt.LARGURA == null ? (decimal?)null : Convert.ToDecimal(produtoInt.LARGURA);
                produto.MetroCubico = produtoInt.M3 == null ? (decimal?)null : Convert.ToDecimal(produtoInt.M3);
                produto.MultiploVenda = produtoInt.MULTIPVENDA == null ? (decimal?)null : Convert.ToDecimal(produtoInt.MULTIPVENDA);
                produto.NomeFabricante = produtoInt.FABRICANTE;
                produto.PesoBruto = Convert.ToDecimal(produtoInt.PESOBRUTO);
                produto.PesoLiquido = Convert.ToDecimal(produtoInt.PESOLIQ);
                produto.Referencia = produtoInt.REFERENCIA;
                produto.ReferenciaFornecedor = produtoInt.REFFORN;
                //produto.SKU

                if (produtoNovo)
                {
                    _uow.ProdutoRepository.Add(produto);
                }

                await _uow.SaveChangesAsync();
            }
        }

        public void ValidarProdutoIntegracao(ProdutoIntegracao produtoIntegracao)
        {
            ValidarCampo(produtoIntegracao.CODPROD, nameof(produtoIntegracao.CODPROD));
            ValidarCampo(produtoIntegracao.PRODUTONFE, nameof(produtoIntegracao.PRODUTONFE));
            ValidarCampo(produtoIntegracao.PESOBRUTO, nameof(produtoIntegracao.PESOBRUTO));
            ValidarCampo(produtoIntegracao.PESOLIQ, nameof(produtoIntegracao.PESOLIQ));
            ValidarCampo(produtoIntegracao.DESCRPROD, nameof(produtoIntegracao.DESCRPROD));
            ValidarCampo(produtoIntegracao.ATIVO, nameof(produtoIntegracao.ATIVO));
        }
    }
}
