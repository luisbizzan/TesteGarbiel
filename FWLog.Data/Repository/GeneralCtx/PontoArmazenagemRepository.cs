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

        public List<PontoArmazenagem> RetornarAtivos()
        {
            return Entities.PontoArmazenagem.Where(w => w.Ativo).ToList();
        }

        public List<PontoArmazenagemListaLinhaTabela> BuscarLista(DataTableFilter<PontoArmazenagemListaFiltro> model, out int totalRecordsFiltered, out int totalRecords)
        {
            totalRecords = Entities.PontoArmazenagem.Where(w => w.IdEmpresa == model.CustomFilter.IdEmpresa).Count();

            IQueryable<PontoArmazenagemListaLinhaTabela> query =
                Entities.PontoArmazenagem.AsNoTracking().Where(w => w.IdEmpresa == model.CustomFilter.IdEmpresa &&
                    (model.CustomFilter.Descricao.Equals(string.Empty) || w.Descricao.Contains(model.CustomFilter.Descricao)) &&
                    (model.CustomFilter.IdNivelArmazenagem.HasValue == false || w.IdNivelArmazenagem == model.CustomFilter.IdNivelArmazenagem.Value) &&
                    (model.CustomFilter.IdTipoArmazenagem.HasValue == false || w.IdTipoArmazenagem == model.CustomFilter.IdTipoArmazenagem) &&
                    (model.CustomFilter.IdTipoMovimentacao.HasValue == false || w.IdTipoMovimentacao == model.CustomFilter.IdTipoMovimentacao) &&
                    (model.CustomFilter.Status.HasValue == false || w.Ativo == model.CustomFilter.Status))
                .Select(s => new PontoArmazenagemListaLinhaTabela
                {
                    IdPontoArmazenagem = s.IdPontoArmazenagem,
                    Descricao = s.Descricao,
                    LimitePesoVertical = s.LimitePesoVertical,
                    Status = s.Ativo ? "Ativo" : "Inativo",
                    NivelArmazenagem = s.NivelArmazenagem.Descricao,
                    TipoArmazenagem = s.TipoArmazenagem.Descricao,
                    TipoMovimentacao = s.TipoMovimentacao.Descricao
                });

            totalRecordsFiltered = query.Count();

            query = query
                .OrderBy(model.OrderByColumn, model.OrderByDirection)
                .Skip(model.Start)
                .Take(model.Length);

            return query.ToList();
        }

        public IList<PontoArmazenagemPesquisaModalListaLinhaTabela> BuscarListaModal(DataTableFilter<PontoArmazenagemPesquisaModalFiltro> filtros, out int registrosFiltrados, out int totalRegistros)
        {
            totalRegistros = Entities.PontoArmazenagem.Count(w => w.IdEmpresa == filtros.CustomFilter.IdEmpresa);

            var query = Entities.PontoArmazenagem
                .Where(w => w.IdEmpresa == filtros.CustomFilter.IdEmpresa &&
                (filtros.CustomFilter.IdNivelArmazenagem.HasValue == false || w.IdNivelArmazenagem == filtros.CustomFilter.IdNivelArmazenagem.Value) &&
                (filtros.CustomFilter.Descricao.Equals(string.Empty) || w.Descricao.Contains(filtros.CustomFilter.Descricao)) &&
                (filtros.CustomFilter.Status.HasValue == false || w.Ativo == filtros.CustomFilter.Status.Value))
                .Select(s => new PontoArmazenagemPesquisaModalListaLinhaTabela
                {
                    IdPontoArmazenagem = s.IdPontoArmazenagem,
                    PontoArmazenagem = s.Descricao,
                    NivelArmazenagem = s.NivelArmazenagem.Descricao,
                    Status = s.Ativo ? "Ativo" : "Inativo"
                });

            registrosFiltrados = query.Count();

            query = query
                .OrderBy(filtros.OrderByColumn, filtros.OrderByDirection)
                .Skip(filtros.Start)
                .Take(filtros.Length);

            return query.ToList();
        }

        public PontoArmazenagem BuscarPontoArmazenagemPorIdEmpresaPorPontoEPorNivel(long? idNivelArmazenagem, string descricao, long IdEmpresa)
        {
            var pontoArmazenagem = Entities.PontoArmazenagem
               .FirstOrDefault(x => x.IdNivelArmazenagem == idNivelArmazenagem
               && x.Descricao == descricao
               && x.IdEmpresa == IdEmpresa);

            return pontoArmazenagem;
        }
    }
}