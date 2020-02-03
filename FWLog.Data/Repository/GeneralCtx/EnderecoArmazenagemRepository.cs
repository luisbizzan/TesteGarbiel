using FWLog.Data.Models;
using FWLog.Data.Models.DataTablesCtx;
using FWLog.Data.Models.FilterCtx;
using FWLog.Data.Repository.CommonCtx;
using System.Collections.Generic;
using System.Linq;

namespace FWLog.Data.Repository.GeneralCtx
{
    public class EnderecoArmazenagemRepository : GenericRepository<EnderecoArmazenagem>
    {
        public EnderecoArmazenagemRepository(Entities entities) : base(entities) { }

        public List<EnderecoArmazenagemListaLinhaTabela> BuscarLista(DataTableFilter<EnderecoArmazenagemListaFiltro> model, out int totalRecordsFiltered, out int totalRecords)
        {
            totalRecords = Entities.EnderecoArmazenagem.Where(w => w.IdEmpresa == model.CustomFilter.IdEmpresa).Count();

            IQueryable<EnderecoArmazenagemListaLinhaTabela> query =
                Entities.EnderecoArmazenagem.AsNoTracking().Where(w => w.IdEmpresa == model.CustomFilter.IdEmpresa &&
                    (model.CustomFilter.Codigo.Equals(string.Empty) || w.Codigo.Contains(model.CustomFilter.Codigo)) &&
                    (model.CustomFilter.IdNivelArmazenagem.HasValue == false || w.IdNivelArmazenagem == model.CustomFilter.IdNivelArmazenagem.Value) &&
                    (model.CustomFilter.IdPontoArmazenagem.HasValue == false || w.IdPontoArmazenagem == model.CustomFilter.IdPontoArmazenagem.Value) &&
                    (model.CustomFilter.Status.HasValue == false || w.Ativo == model.CustomFilter.Status))
                .Select(s => new EnderecoArmazenagemListaLinhaTabela
                {
                    IdEnderecoArmazenagem = s.IdEnderecoArmazenagem,
                    NivelArmazenagem = s.NivelArmazenagem.Descricao,
                    PontoArmazenagem = s.PontoArmazenagem.Descricao,
                    Codigo = s.Codigo,
                    Fifo = s.IsFifo ? "Sim" : "Não",
                    PontoSeparacao = s.IsPontoSeparacao ? "Sim" : "Não",
                    EstoqueMinimo = s.EstoqueMinimo ?? 0,
                    Status = s.Ativo ? "Ativo" : "Inativo"
                });

            totalRecordsFiltered = query.Count();

            query = query
                .OrderBy(model.OrderByColumn, model.OrderByDirection)
                .Skip(model.Start)
                .Take(model.Length);

            return query.ToList();
        }

        public EnderecoArmazenagem ConsultarPorNivelEPontoArmazenagem(long? nivel, long? pontoarmazenagem, long idEmpresa)
        {
            return Entities.EnderecoArmazenagem.FirstOrDefault(f => f.IdNivelArmazenagem == nivel && f.IdPontoArmazenagem == pontoarmazenagem && f.IdEmpresa == idEmpresa);
        }
    }
}
