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

        public List<LogEtiquetagem> Relatorio(LogEtiquetagemListaFiltro filtro, long idEmpresa)
        {
            var query =
                Entities.LogEtiquetagem.Where(w =>
                (w.IdEmpresa == idEmpresa) &&
                (filtro.IdProduto.HasValue == false || w.IdProduto == filtro.IdProduto.Value) &&
                (filtro.QuantidadeInicial.HasValue == false || w.Quantidade >= filtro.QuantidadeInicial.Value) &&
                (filtro.QuantidadeFinal.HasValue == false || w.Quantidade <= filtro.QuantidadeFinal.Value) &&
                (string.IsNullOrEmpty(filtro.IdUsuarioEtiquetagem) || w.IdUsuario.Contains(filtro.IdUsuarioEtiquetagem)) &&
                (filtro.DataInicial.HasValue == false || w.DataHora >= filtro.DataInicial.Value) &&
                (filtro.DataFinal.HasValue == false || w.DataHora <= filtro.DataFinal.Value));

            return query.ToList();
        }

        public List<LogEtiquetagemListaLinhaTabela> BuscarLista(DataTableFilter<LogEtiquetagemListaFiltro> model, out int totalRecordsFiltered, out int totalRecords, long idEmpresa)
        {
            totalRecords = Entities.LogEtiquetagem.Count(x => x.IdEmpresa == idEmpresa);

            var query =
                Entities.LogEtiquetagem.Where(w =>
                (w.IdEmpresa == idEmpresa) &&
                (model.CustomFilter.IdProduto.HasValue == false || w.IdProduto == model.CustomFilter.IdProduto.Value) &&
                (model.CustomFilter.QuantidadeInicial.HasValue == false || w.Quantidade >= model.CustomFilter.QuantidadeInicial.Value) &&
                (model.CustomFilter.QuantidadeFinal.HasValue == false || w.Quantidade <= model.CustomFilter.QuantidadeFinal.Value) &&
                (string.IsNullOrEmpty(model.CustomFilter.IdUsuarioEtiquetagem) || w.IdUsuario.Contains(model.CustomFilter.IdUsuarioEtiquetagem)) &&
                (model.CustomFilter.DataInicial.HasValue == false || w.DataHora >= model.CustomFilter.DataInicial.Value.Date) &&
                (model.CustomFilter.DataFinal.HasValue == false || w.DataHora <= model.CustomFilter.DataFinal.Value.AddDays(1).Date))
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
