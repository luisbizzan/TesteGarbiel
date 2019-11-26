using FWLog.Data.Models;
using FWLog.Data.Models.DataTablesCtx;
using FWLog.Data.Models.FilterCtx;
using FWLog.Data.Repository.CommonCtx;
using System.Collections.Generic;
using System.Linq;

namespace FWLog.Data.Repository.GeneralCtx
{
    public class PontoArmazenagemRepository : GenericRepository<PontoArmazenagem>
    {
        public PontoArmazenagemRepository(Entities entities) : base(entities) { }

        public List<PontoArmazenagemListaLinhaTabela> BuscarLista(DataTableFilter<PontoArmazenagemListaFiltro> model, out int totalRecordsFiltered, out int totalRecords)
        {
            totalRecords = Entities.PontoArmazenagem.Where(w => w.IdEmpresa == model.CustomFilter.IdEmpresa).Count();
            
            IQueryable<PontoArmazenagemListaLinhaTabela> query = 
                from pontoArmazenagem in Entities.PontoArmazenagem.AsNoTracking().Where(w => w.IdEmpresa == model.CustomFilter.IdEmpresa)
                select new PontoArmazenagemListaLinhaTabela
                {
                    IdPontoArmazenagem = pontoArmazenagem.IdPontoArmazenagem,
                    Descricao = pontoArmazenagem.Descricao,
                    LimitePesoVertical = pontoArmazenagem.LimitePesoVertical ?? 0,
                    Status = pontoArmazenagem.Ativo ? "Ativo" : "Inativo",
                    NivelArmazenagem = pontoArmazenagem.NivelArmazenagem.Descricao,
                    TipoArmazenagem = pontoArmazenagem.TipoArmazenagem.Descricao,
                    TipoMovimentacao = pontoArmazenagem.TipoMovimentacao.Descricao
                };

            if (!string.IsNullOrEmpty(model.CustomFilter.Descricao))
            {
                query = query.Where(x => x.Descricao.Contains(model.CustomFilter.Descricao));
            }

            totalRecordsFiltered = query.Count();

            query = query
                .OrderBy(model.OrderByColumn, model.OrderByDirection)
                .Skip(model.Start)
                .Take(model.Length);

            return query.ToList();
        }
    }
}
