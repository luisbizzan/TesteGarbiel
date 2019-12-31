using FWLog.Data.Models;
using FWLog.Data.Models.DataTablesCtx;
using FWLog.Data.Models.FilterCtx;
using FWLog.Data.Repository.CommonCtx;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FWLog.Data.Repository.GeneralCtx
{
    public class LogEtiquetagemRepository : GenericRepository<LogEtiquetagem>
    {
        public LogEtiquetagemRepository(Entities entities) : base(entities) { }

        public List<LogEtiquetagemListaLinhaTabela> BuscarLista(DataTableFilter<LogEtiquetagemListaFiltro> model, out int totalRecordsFiltered, out int totalRecords, long idEmpresa)
        {
            totalRecords = Entities.LogEtiquetagem.Count(x=> x.IdEmpresa == idEmpresa);

            var query =
                Entities.LogEtiquetagem.Where(w =>
                (w.IdEmpresa == idEmpresa) &&
                (model.CustomFilter.IdProduto.HasValue == false || w.IdProduto == model.CustomFilter.IdProduto.Value) &&
                (model.CustomFilter.QuantidadeInicial.HasValue == false || w.Quantidade >= model.CustomFilter.QuantidadeInicial.Value) &&
                (model.CustomFilter.QuantidadeFinal.HasValue == false || w.Quantidade <= model.CustomFilter.QuantidadeFinal.Value) &&
                (string.IsNullOrEmpty(model.CustomFilter.IdUsuarioEtiquetagem) || w.IdUsuario.Contains(model.CustomFilter.IdUsuarioEtiquetagem)) &&
                (model.CustomFilter.DataInicial.HasValue == false || w.DataHora >= model.CustomFilter.DataInicial.Value) &&
                (model.CustomFilter.DataFinal.HasValue == false || w.DataHora <= model.CustomFilter.DataFinal.Value))
                .Select(s => new LogEtiquetagemListaLinhaTabela
                {
                    IdLogEtiquetagem = s.IdLogEtiquetagem,
                    Referencia = s.Produto.Referencia,
                    Descricao = s.Produto.Descricao,
                    TipoEtiquetagem = s.TipoEtiquetagem.Descricao,
                    Quantidade = s.Quantidade,
                    DataHora = s.DataHora,
                    Usuario = s.UsuarioEtiquetagem.Id
                });

            totalRecordsFiltered = query.Count();

            query = query
                .OrderBy(model.OrderByColumn, model.OrderByDirection)
                .Skip(model.Start)
                .Take(model.Length);

            return query.ToList();
        }
    }
}
