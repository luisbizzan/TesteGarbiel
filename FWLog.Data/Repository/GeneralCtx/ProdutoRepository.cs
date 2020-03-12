﻿using FWLog.Data.Models;
using FWLog.Data.Models.DataTablesCtx;
using FWLog.Data.Models.FilterCtx;
using FWLog.Data.Repository.CommonCtx;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

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
    }
}
