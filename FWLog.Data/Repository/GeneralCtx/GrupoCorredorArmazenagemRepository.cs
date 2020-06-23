using FWLog.Data.Models;
using FWLog.Data.Models.DataTablesCtx;
using FWLog.Data.Models.FilterCtx;
using FWLog.Data.Repository.CommonCtx;
using System.Collections.Generic;
using System.Linq;

namespace FWLog.Data.Repository.GeneralCtx
{
    public class GrupoCorredorArmazenagemRepository : GenericRepository<GrupoCorredorArmazenagem>
    {
        public GrupoCorredorArmazenagemRepository(Entities entities) : base(entities)
        {
        }

        public List<GrupoCorredorArmazenagem> Todos()
        {
            return Entities.GrupoCorredorArmazenagem.ToList();
        }

        public GrupoCorredorArmazenagem BuscarPorCorredor(long idEmpresa, int corredorInicial, int corredorFinal, long idPontoArmazenagem)
        {
            return Entities.GrupoCorredorArmazenagem.Where(x => x.IdEmpresa == idEmpresa && x.CorredorInicial == corredorInicial && x.CorredorFinal == corredorFinal && x.IdPontoArmazenagem == idPontoArmazenagem).FirstOrDefault();
        }

        public GrupoCorredorArmazenagem BuscarPorImpressora(long idEmpresa, int corredorInicial, int corredorFinal, long idImpressora, long idImpressoraPedidoFilial)
        {
            return Entities.GrupoCorredorArmazenagem.Where(x => x.IdEmpresa == idEmpresa && x.CorredorInicial == corredorInicial && x.CorredorFinal == corredorFinal && x.IdImpressora == idImpressora && x.IdImpressoraPedidoFilial == idImpressoraPedidoFilial).FirstOrDefault();
        }

        public List<CorredorImpressoraListaTabela> BuscarLista(DataTableFilter<CorredorImpressoraListaFiltro> model, out int totalRecordsFiltered, out int totalRecords)
        {
            totalRecords = Entities.GrupoCorredorArmazenagem.Where(w => w.IdEmpresa == model.CustomFilter.IdEmpresa).Count();

            var query =
                Entities.GrupoCorredorArmazenagem.AsNoTracking().Where(w => w.IdEmpresa == model.CustomFilter.IdEmpresa &&
                    (model.CustomFilter.IdPontoArmazenagem.HasValue == false || w.IdPontoArmazenagem == model.CustomFilter.IdPontoArmazenagem.Value) &&
                    (model.CustomFilter.CorredorInicial.HasValue == false || w.CorredorInicial >= model.CustomFilter.CorredorInicial.Value) &&
                    (model.CustomFilter.CorredorFinal.HasValue == false || w.CorredorFinal <= model.CustomFilter.CorredorFinal.Value) &&
                    (model.CustomFilter.IdImpressora.HasValue == false || w.IdImpressora == model.CustomFilter.IdImpressora.Value) &&
                    (model.CustomFilter.Status.HasValue == false || w.Ativo == model.CustomFilter.Status.Value))
                .Select(s => new
                {
                    s.IdEmpresa,
                    s.IdGrupoCorredorArmazenagem,
                    DescricaoPontoArmazenagem = s.PontoArmazenagem.Descricao,
                    s.CorredorInicial,
                    s.CorredorFinal,
                    Impressora = s.Impressora.Name,
                    ImpressoraPedidoFilial = s.ImpressoraPedidoFilial.Name,
                    Status = s.Ativo
                });

            totalRecordsFiltered = query.Count();

            query = query
                .OrderBy(model.OrderByColumn, model.OrderByDirection)
                .Skip(model.Start)
                .Take(model.Length);

            var list = query.ToList();

            return list.Select(q => new CorredorImpressoraListaTabela
            {
                IdEmpresa = q.IdEmpresa,
                IdGrupoCorredorArmazenagem = q.IdGrupoCorredorArmazenagem,
                DescricaoPontoArmazenagem = q.DescricaoPontoArmazenagem,
                CorredorInicial = q.CorredorInicial.ToString().PadLeft(2, '0'),
                CorredorFinal = q.CorredorFinal.ToString().PadLeft(2, '0'),
                Impressora = q.Impressora,
                ImpressoraPedidoFilial = q.ImpressoraPedidoFilial,
                Status = q.Status ? "Sim" : "Não"
            }).ToList();
        }

        public List<GrupoCorredorArmazenagem> BuscarPorEmpresaEPontoArmazenagem(long idEmpresa, long idPontoArmazenagem)
        {
            return Entities.GrupoCorredorArmazenagem.Where(x => x.IdEmpresa == idEmpresa && x.IdPontoArmazenagem == idPontoArmazenagem).ToList();
        }
    }
}