using FWLog.Data.Models;
using FWLog.Data.Models.DataTablesCtx;
using FWLog.Data.Models.FilterCtx;
using FWLog.Data.Repository.CommonCtx;
using System.Collections.Generic;
using System.Linq;

namespace FWLog.Data.Repository.GeneralCtx
{
    public class CaixaRecusaRepository : GenericRepository<CaixaRecusa>
    {
        public CaixaRecusaRepository(Entities entities) : base(entities)
        {
        }

        public List<CaixaRecusaListaTabela> BuscarLista(DataTableFilter<CaixaRecusaListaFiltro> filtro, out int totalRecordsFiltered, out int totalRecords)
        {
            totalRecords = Entities.CaixaRecusa.Where(w => w.IdEmpresa == filtro.CustomFilter.IdEmpresa).Count();

            var query = Entities.CaixaRecusa.AsNoTracking().Where(w => w.IdEmpresa == filtro.CustomFilter.IdEmpresa);

            if (filtro.CustomFilter.IdCaixa.HasValue)
            {
                query = query.Where(caixaRecusa => caixaRecusa.IdCaixa == filtro.CustomFilter.IdCaixa.Value);
            }

            if (filtro.CustomFilter.IdProduto.HasValue)
            {
                query = query.Where(caixaRecusa => caixaRecusa.IdProduto == filtro.CustomFilter.IdProduto.Value);
            }

            var selectedQuery = query.GroupBy(x => new { x.IdEmpresa, x.IdCaixa, x.Caixa })
                .Select(caixaRecusa => new CaixaRecusaListaTabela
                {
                    IdEmpresa = caixaRecusa.Key.IdEmpresa,
                    IdCaixa = caixaRecusa.Key.IdCaixa,
                    NomeCaixa = caixaRecusa.Key.Caixa.Nome
                });

            totalRecordsFiltered = selectedQuery.Count();

            selectedQuery = selectedQuery
                .OrderBy(filtro.OrderByColumn, filtro.OrderByDirection)
                .Skip(filtro.Start)
                .Take(filtro.Length);

            return selectedQuery.ToList();
        }

        public List<CaixaRecusa> BuscarCaixaPorEmpresa(long idEmpresa, long idCaixa)
        {
            return Entities.CaixaRecusa.Where(x => x.IdEmpresa == idEmpresa && x.IdCaixa == idCaixa).ToList();
        }

        public List<CaixaRecusa> BuscarPorEmpresaProduto(long idEmpresa, long idProduto)
        {
            return Entities.CaixaRecusa.Where(x => x.IdEmpresa == idEmpresa && x.IdProduto == idProduto).ToList();
        }
    }
}
