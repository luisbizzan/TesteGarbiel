using Dapper;
using FWLog.Data.Models;
using FWLog.Data.Models.DataTablesCtx;
using FWLog.Data.Models.FilterCtx;
using FWLog.Data.Repository.CommonCtx;
using Oracle.ManagedDataAccess.Client;
using System.Collections.Generic;
using System.Linq;

namespace FWLog.Data.Repository.GeneralCtx
{
    public class ProdutoRepository : GenericRepository<Produto>
    {
        public ProdutoRepository(Entities entities) : base(entities)
        {
        }

        public Produto ConsultarPorCodigoIntegracao(long codigoIntegracao)
        {
            return Entities.Produto.FirstOrDefault(f => f.CodigoIntegracao == codigoIntegracao);
        }

        public Produto ConsultarPorCodigoBarrasOuReferencia(string codigoBarrasOuReferencia)
        {
            return Entities.Produto.FirstOrDefault(f => f.CodigoBarras == codigoBarrasOuReferencia
                || f.Referencia == codigoBarrasOuReferencia
                || f.CodigoBarras2 == codigoBarrasOuReferencia);
        }

        public Produto ConsultarPorCodigoBarrasOuReferenciaGarantia(string codigoBarrasOuReferenciaOuGarantiaEtiqueta, long idGarantia)
        {
            var idGarantiaLenght = idGarantia.ToString().Length;

            var etiquetaGarantiaFormatada = codigoBarrasOuReferenciaOuGarantiaEtiqueta.Substring(idGarantiaLenght);

            return Entities.Produto.FirstOrDefault(f => f.CodigoBarras == codigoBarrasOuReferenciaOuGarantiaEtiqueta
                 || f.Referencia == codigoBarrasOuReferenciaOuGarantiaEtiqueta
                 || f.CodigoBarras2 == codigoBarrasOuReferenciaOuGarantiaEtiqueta
                 || f.CodigoBarras == etiquetaGarantiaFormatada);
        }

        public IQueryable<Produto> Todos()
        {
            return Entities.Produto;
        }

        public List<ProdutoPesquisaModalListaLinhaTabela> BuscarLista(DataTableFilter<ProdutoPesquisaModalFiltro> model, out int totalRecordsFiltered, out int totalRecords)
        {
            totalRecords = Entities.Produto.Count();

            IQueryable<ProdutoPesquisaModalListaLinhaTabela> query =
                Entities.Produto.AsNoTracking().Where(w =>
                    (model.CustomFilter.Referencia.Equals(string.Empty) || w.Referencia.Contains(model.CustomFilter.Referencia)) &&
                    (model.CustomFilter.Descricao.Equals(string.Empty) || w.Descricao.Contains(model.CustomFilter.Descricao)) &&
                    (model.CustomFilter.Status.HasValue == false || w.Ativo == model.CustomFilter.Status))
                    .Select(s => new ProdutoPesquisaModalListaLinhaTabela
                    {
                        IdProduto = s.IdProduto,
                        Referencia = s.Referencia,
                        Descricao = s.Descricao,
                        Status = s.Ativo ? "Ativo" : "Inativo"
                    });

            totalRecordsFiltered = query.Count();

            query = query
                .OrderBy(model.OrderByColumn, model.OrderByDirection)
                .Skip(model.Start)
                .Take(model.Length);

            return query.ToList();
        }

        public Produto PesquisarPorCodigoBarras(string codigoBarras)
        {
            return Entities.Produto.Where(w => w.CodigoBarras.Equals(codigoBarras) || w.CodigoBarras.Equals(codigoBarras)).FirstOrDefault();
        }

        public Produto PesquisarPorReferencia(string referencia)
        {
            return Entities.Produto.Where(w => w.Referencia.Equals(referencia)).FirstOrDefault();
        }

        //public IEnumerable<ProdutoListaLinhaTabela> ObterDadosParaDataTable(DataTableFilter<ProdutoListaFiltro> filter, out int totalRecordsFiltered, out int totalRecords)
        //{
        //    IEnumerable<ProdutoEstoque> produto = null;

        //    using (var conn = new OracleConnection(Entities.Database.Connection.ConnectionString))
        //    {
        //        conn.Open();

        //        if (conn.State == System.Data.ConnectionState.Open)
        //        {
        //            produto = conn.Query<ProdutoEstoque, Produto, UnidadeMedida, EnderecoArmazenagem, ProdutoEstoqueStatus, ProdutoEstoque>(
        //            "SELECT " +
        //                "produtoestoque.\"IdProduto\"," +
        //                "produto.\"Descricao\",	" +
        //                "produto.\"Referencia\",	" +
        //                "produto.\"PesoBruto\", " +
        //                "produto.\"Largura\"," +
        //                "produto.\"Altura\"," +
        //                "produto.\"Comprimento\"," +
        //                "unidademedida.\"IdUnidadeMedida\"," +
        //                "unidademedida.\"Sigla\"," +
        //                "enderecoarmazenagem.\"IdEnderecoArmazenagem\"," +
        //                "enderecoarmazenagem.\"Codigo\", " +
        //                "produtoestoquestatus.\"IdProdutoEstoqueStatus\"," +
        //                "produtoestoquestatus.\"Descricao\"" +
        //            "FROM \"ProdutoEstoque\" produtoestoque " +
        //            "INNER JOIN \"Produto\" produto ON produto.\"IdProduto\" = produtoestoque.\"IdProduto\" " +
        //            "INNER JOIN \"UnidadeMedida\" unidademedida ON unidademedida.\"IdUnidadeMedida\" = produto.\"IdUnidadeMedida\" " +
        //            "LEFT JOIN \"EnderecoArmazenagem\" enderecoarmazenagem ON enderecoarmazenagem.\"IdEnderecoArmazenagem\" = produtoestoque.\"IdEnderecoArmazenagem\" " +
        //            "LEFT JOIN \"ProdutoEstoqueStatus\" produtoestoquestatus ON produtoestoquestatus.\"IdProdutoEstoqueStatus\" = produtoestoque.\"IdProdutoEstoqueStatus\" " +
        //            "WHERE produtoestoque.\"IdEmpresa\" = " + filter.CustomFilter.IdEmpresa,
        //            map: (pe, p, un, ea, pes) =>
        //            {
        //                pe.Produto = p;
        //                pe.Produto.UnidadeMedida = un;
        //                pe.EnderecoArmazenagem = ea;
        //                pe.IdProdutoEstoqueStatus = pes.IdProdutoEstoqueStatus;
        //                return pe;
        //            },
        //            splitOn: "IdProduto,Descricao,IdUnidadeMedida,IdEnderecoArmazenagem,IdProdutoEstoqueStatus"
        //            );
        //        }

        //        conn.Close();
        //    }

        //    totalRecords = produto.Count();

        //    var query = produto.Where(x =>
        //       (string.IsNullOrEmpty(filter.CustomFilter.Referencia) == true || x.Produto.Referencia == filter.CustomFilter.Referencia) &&
        //       (string.IsNullOrEmpty(filter.CustomFilter.Descricao) == true || x.Produto.Descricao == filter.CustomFilter.Descricao) &&
        //       (string.IsNullOrEmpty(filter.CustomFilter.CodigoDeBarras) == true || x.Produto.CodigoBarras == filter.CustomFilter.CodigoDeBarras)
        //    );

        //    if (filter.CustomFilter.ProdutoStatus.HasValue)
        //    {

        //        //Sem Locação
        //        if (filter.CustomFilter.ProdutoStatus == 2)
        //        {
        //            query = query.Where(x => x.EnderecoArmazenagem == null);
        //        }
        //        //Ativo
        //        else if (filter.CustomFilter.ProdutoStatus == 1)
        //        {
        //            query = query.Where(x => x.IdProdutoEstoqueStatus == ProdutoEstoqueStatusEnum.Ativo);
        //        }
        //        //Todos(Inativo)
        //        else
        //        {
        //            query = query.Where(x => x.IdProdutoEstoqueStatus != ProdutoEstoqueStatusEnum.Ativo);
        //        }
        //    }

        //    IEnumerable<ProdutoListaLinhaTabela> queryResult = query.Select(e => new ProdutoListaLinhaTabela
        //    {
        //        IdProduto = e.IdProduto == 0 ? (long?)null : e.IdProduto,
        //        Referencia = e.Produto.Referencia,
        //        Descricao = e.Produto.Descricao,
        //        Peso = e.Produto.PesoBruto.ToString("n2"),
        //        Largura = e.Produto.Largura?.ToString("n2"),
        //        Altura = e.Produto.Altura?.ToString("n2"),
        //        Comprimento = e.Produto.Comprimento?.ToString("n2"),
        //        Unidade = e.Produto.UnidadeMedida.Sigla,
        //        Endereco = e.EnderecoArmazenagem?.Codigo,
        //        Multiplo = e.Produto.MultiploVenda.ToString(),
        //        Status = e.IdProdutoEstoqueStatus.ToString()
        //    });

        //    totalRecordsFiltered = queryResult.Count();

        //    queryResult = queryResult
        //        .OrderBy(filter.OrderByColumn, filter.OrderByDirection)
        //        .Skip(filter.Start)
        //        .Take(filter.Length);

        //    return queryResult.ToList();
        //}

        public IEnumerable<ProdutoListaLinhaTabela> FormatarDadosParaDataTable(DataTableFilter<ProdutoListaFiltro> filter, out int totalRecordsFiltered, out int totalRecords, IEnumerable<ProdutoEstoque> produtoEstoque)
        {
            totalRecords = produtoEstoque.Count();

            var query = produtoEstoque.Where(x =>
               (string.IsNullOrEmpty(filter.CustomFilter.Referencia) == true || x.Produto.Referencia == filter.CustomFilter.Referencia) &&
               (string.IsNullOrEmpty(filter.CustomFilter.Descricao) == true || x.Produto.Descricao == filter.CustomFilter.Descricao) &&
               (string.IsNullOrEmpty(filter.CustomFilter.CodigoDeBarras) == true || x.Produto.CodigoBarras == filter.CustomFilter.CodigoDeBarras)
            );

            if (filter.CustomFilter.ProdutoStatus.HasValue)
            {

                //Sem Locação
                if (filter.CustomFilter.ProdutoStatus == 2)
                {
                    query = query.Where(x => x.EnderecoArmazenagem == null);
                }
                //Ativo
                else if (filter.CustomFilter.ProdutoStatus == 1)
                {
                    query = query.Where(x => x.IdProdutoEstoqueStatus == ProdutoEstoqueStatusEnum.Ativo);
                }
                //Todos(Inativo)
                else
                {
                    query = query.Where(x => x.IdProdutoEstoqueStatus != ProdutoEstoqueStatusEnum.Ativo);
                }
            }

            IEnumerable<ProdutoListaLinhaTabela> queryResult = query.Select(e => new ProdutoListaLinhaTabela
            {
                IdProduto = e.IdProduto == 0 ? (long?)null : e.IdProduto,
                Referencia = e.Produto.Referencia,
                Descricao = e.Produto.Descricao,
                Peso = e.Produto.PesoBruto.ToString("n2"),
                Largura = e.Produto.Largura?.ToString("n2"),
                Altura = e.Produto.Altura?.ToString("n2"),
                Comprimento = e.Produto.Comprimento?.ToString("n2"),
                Unidade = e.Produto.UnidadeMedida.Sigla,
                Endereco = e.EnderecoArmazenagem?.Codigo,
                Multiplo = e.Produto.MultiploVenda.ToString(),
                Status = e.IdProdutoEstoqueStatus.ToString()
            });

            totalRecordsFiltered = queryResult.Count();

            queryResult = queryResult
                .OrderBy(filter.OrderByColumn, filter.OrderByDirection)
                .Skip(filter.Start)
                .Take(filter.Length);

            return queryResult.ToList();
        }
    }
}
