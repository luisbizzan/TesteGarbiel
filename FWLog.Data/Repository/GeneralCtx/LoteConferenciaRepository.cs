using Dapper;
using ExtensionMethods.List;
using FWLog.Data.Models;
using FWLog.Data.Models.DataTablesCtx;
using FWLog.Data.Models.FilterCtx;
using FWLog.Data.Repository.CommonCtx;
using System.Collections.Generic;
using System.Linq;

namespace FWLog.Data.Repository.GeneralCtx
{
    public class LoteConferenciaRepository : GenericRepository<LoteConferencia>
    {
        public LoteConferenciaRepository(Entities entities) : base(entities) { }

        public List<LoteConferencia> Obter(long idLote)
        {
            return Entities.LoteConferencia.Where(w => w.IdLote == idLote).ToList();
        }

        public List<LoteConferencia> ObterPorId(long idLote)
        {
            return Entities.LoteConferencia.Where(w => w.IdLote == idLote).ToList();
        }

        public List<LoteConferencia> ObterPorProduto(long idLote, long idProduto)
        {
            return Entities.LoteConferencia.Where(w => w.IdLote == idLote && w.IdProduto == idProduto).ToList();
        }

        public IQueryable<RelatorioRastreioPecaListaLinhaTabela> RastreioPeca(IRelatorioRastreioPecaListaFiltro filter)
        {
            var query = (from lc in Entities.LoteConferencia
                         join l in Entities.Lote on lc.IdLote equals l.IdLote
                         join n in Entities.NotaFiscal on l.IdNotaFiscal equals n.IdNotaFiscal
                         join p in Entities.Produto on lc.IdProduto equals p.IdProduto
                         where n.IdEmpresa == filter.IdEmpresa
                         && l.IdLoteStatus != LoteStatusEnum.AguardandoRecebimento
                         && l.IdLoteStatus != LoteStatusEnum.Recebido
                         && l.IdLoteStatus != LoteStatusEnum.Conferencia
                         select new RelatorioRastreioPecaListaLinhaTabela
                         {
                             IdLote = l.IdLote,
                             IdEmpresa = n.IdEmpresa,
                             Empresa = n.Empresa.NomeFantasia,
                             NroNota = n.Numero,
                             ReferenciaPronduto = p.Referencia,
                             DataCompra = n.DataEmissao,
                             DataRecebimento = lc.DataHoraFim,
                             QtdCompra = n.Quantidade,
                             QtdRecebida = lc.Quantidade
                         });

            query = query.AsEnumerable().GroupBy(x => new { x.IdLote, x.NroNota, x.IdEmpresa, x.ReferenciaPronduto }, (key, group) => new RelatorioRastreioPecaListaLinhaTabela
            {
                IdLote = key.IdLote,
                IdEmpresa = key.IdEmpresa,
                NroNota = key.NroNota,
                ReferenciaPronduto = key.ReferenciaPronduto,
                Empresa = group.FirstOrDefault().Empresa,
                DataCompra = group.Min(y => y.DataCompra),
                DataRecebimento = group.Max(y => y.DataRecebimento),
                QtdCompra = group.Sum(y => y.QtdCompra),
                QtdRecebida = group.Sum(y => y.QtdRecebida)
            }).AsQueryable();

            query = query.WhereIf(!string.IsNullOrEmpty(filter.ReferenciaPronduto), x => x.ReferenciaPronduto.ToUpper().Contains(filter.ReferenciaPronduto.ToUpper()));
            query = query.WhereIf(filter.IdLote.HasValue, x => x.IdLote == filter.IdLote);
            query = query.WhereIf(filter.NroNota.HasValue, x => x.NroNota == filter.NroNota);

            query = query.WhereIf(filter.DataCompraMinima.HasValue, x => x.DataCompra >= filter.DataCompraMinima);
            query = query.WhereIf(filter.DataCompraMaxima.HasValue, x => x.DataCompra <= filter.DataCompraMaxima);

            query = query.WhereIf(filter.DataRecebimentoMinima.HasValue, x => x.DataRecebimento >= filter.DataRecebimentoMinima);
            query = query.WhereIf(filter.DataRecebimentoMaxima.HasValue, x => x.DataRecebimento <= filter.DataCompraMaxima);

            query = query.WhereIf(filter.QtdCompraMinima.HasValue, x => x.QtdCompra >= filter.QtdCompraMinima);
            query = query.WhereIf(filter.QtdCompraMaxima.HasValue, x => x.QtdCompra <= filter.QtdCompraMaxima);

            query = query.WhereIf(filter.QtdRecebidaMinima.HasValue, x => x.QtdRecebida >= filter.QtdRecebidaMinima);
            query = query.WhereIf(filter.QtdRecebidaMaxima.HasValue, x => x.QtdRecebida <= filter.QtdRecebidaMaxima);

            return query;
        }

        public bool ExisteConferencia(long idLote)
        {
            return Entities.LoteConferencia.Any(a => a.IdLote == idLote);
        }

        public IQueryable<LoteConferencia> Todos()
        {
            return Entities.LoteConferencia;
        }

        public IEnumerable<RelatorioResumoProducaoConferenciaListRow> ResumoProducaoConferencia(RelatorioResumoProducaoFilter request)
        {
            string stringQuery = "SELECT rel.*, ROWNUM Ranking FROM( SELECT perfilUsu.\"Nome\", perfilUsu.\"UsuarioId\", metricasUsu.LotesRecebidasUsuario, metricasUsu.PecasRecebidasUsuario, totalLote.LotesRecebidos, totalLote.PecasRecebidas, TRUNC((metricasUsu.PecasRecebidasUsuario / totalLote.PecasRecebidas) * 100, 3) Percentual FROM ( SELECT lc.\"IdUsuarioConferente\" UsuarioId, COUNT(DISTINCT(lc.\"IdLote\")) LotesRecebidasUsuario, SUM(lc.\"Quantidade\") PecasRecebidasUsuario FROM \"LoteConferencia\" lc, \"Lote\" l, \"NotaFiscal\" n WHERE lc.\"IdLote\" = l.\"IdLote\" AND l.\"IdNotaFiscal\" = n.\"IdNotaFiscal\" AND n.\"IdEmpresa\" = :ID_EMP AND lc.\"DataHoraInicio\" >= :DATA_MIN AND (:DATA_MAX IS NULL OR lc.\"DataHoraInicio\" <= :DATA_MAX) GROUP BY lc.\"IdUsuarioConferente\") metricasUsu, ( SELECT COUNT(DISTINCT(lc.\"IdLote\")) LotesRecebidos, SUM(lc.\"Quantidade\") PecasRecebidas FROM \"LoteConferencia\" lc, \"Lote\" l, \"NotaFiscal\" n WHERE lc.\"IdLote\" = l.\"IdLote\" AND l.\"IdNotaFiscal\" = n.\"IdNotaFiscal\" AND n.\"IdEmpresa\" = :ID_EMP AND lc.\"DataHoraInicio\" >= :DATA_MIN AND (:DATA_MAX IS NULL OR lc.\"DataHoraInicio\" <= :DATA_MAX)) totalLote, \"PerfilUsuario\" perfilUsu WHERE perfilUsu.\"UsuarioId\" = metricasUsu.UsuarioId ORDER BY Percentual DESC) rel";

            var param = new
            {
                DATA_MIN = request.DateMin,
                DATA_MAX = request.DateMax,
                ID_EMP = request.IdEmpresa
            };

            var list = Entities.Database.Connection.Query<RelatorioResumoProducaoConferenciaListRow>(stringQuery, param);

            if (request.UserId != null)
            {
                list = list.Where(x => x.UsuarioId == request.UserId);
            }

            return list;
        }
    }
}
