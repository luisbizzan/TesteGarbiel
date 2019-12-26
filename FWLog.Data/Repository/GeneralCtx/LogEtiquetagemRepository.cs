using FWLog.Data.Models;
using FWLog.Data.Models.DataTablesCtx;
using FWLog.Data.Models.FilterCtx;
using FWLog.Data.Repository.CommonCtx;
using System.Collections.Generic;
using System.Linq;

namespace FWLog.Data.Repository.GeneralCtx
{
    public class LogEtiquetagemRepository : GenericRepository<LogEtiquetagem>
    {
        public LogEtiquetagemRepository(Entities entities) : base(entities) { }

        public List<LogEtiquetagemListaLinhaTabela> BuscarLista(DataTableFilter<LogEtiquetagemListaFiltro> model, out int totalRecordsFiltered, out int totalRecords)
        {
            totalRecords = Entities.LogEtiquetagem.Where(w => w.IdTipoEtiquetagem == model.CustomFilter.IdLogEtiquetagem).Count();

            IQueryable<LogEtiquetagemListaLinhaTabela> query =
                Entities.LogEtiquetagem.AsNoTracking().Where(w => 
                    (model.CustomFilter.IdUsuarioEtiquetagem.Equals(string.Empty) || w.IdUsuario.Contains(model.CustomFilter.IdUsuarioEtiquetagem)) &&
                    (model.CustomFilter.DataInicial.HasValue == false || w.DataHora >= model.CustomFilter.DataInicial.Value) &&
                    (model.CustomFilter.DataFinal.HasValue == false || w.DataHora <= model.CustomFilter.DataFinal.Value))
                .Select(s => new LogEtiquetagemListaLinhaTabela
                {
                    IdLogEtiquetagem = s.IdLogEtiquetagem,
                    IdProduto = s.IdProduto,
                    TipoEtiquetagem = s.TipoEtiquetagem.Descricao,
                    DescricaoProduto = s.Produto.Descricao,
                    Quantidade = s.Quantidade,
                    DataHora = s.DataHora.ToString("dd/MM/yyy hh:mm:ss"),
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
