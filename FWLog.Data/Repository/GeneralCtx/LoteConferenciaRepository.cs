using Dapper;
using ExtensionMethods.List;
using FWLog.Data.Models;
using FWLog.Data.Models.DataTablesCtx;
using FWLog.Data.Models.FilterCtx;
using FWLog.Data.Repository.CommonCtx;
using System;
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

        public List<RelatorioRastreioPecaListaLinhaTabela> RastreioPeca(IRelatorioRastreioPecaListaFiltro filter, out int totalRecordsFiltered, out int totalRecords)
        {
            var resultList = new List<RelatorioRastreioPecaListaLinhaTabela>();

            IQueryable<RelatorioRastreioPecaListaLinhaTabela> query = Entities.LoteConferencia
                .Where(x => x.Lote.NotaFiscal.IdEmpresa == filter.IdEmpresa &&
                            (x.Lote.IdLoteStatus == LoteStatusEnum.Finalizado || x.Lote.IdLoteStatus == LoteStatusEnum.FinalizadoDivergenciaNegativa
                            || x.Lote.IdLoteStatus == LoteStatusEnum.FinalizadoDivergenciaPositiva || x.Lote.IdLoteStatus == LoteStatusEnum.FinalizadoDivergenciaTodas))
                .Select(s => new RelatorioRastreioPecaListaLinhaTabela
                {
                    IdLote = s.IdLote,
                    NroNota = s.Lote.NotaFiscal.Numero,
                    IdNotaFiscal = s.Lote.IdNotaFiscal,
                    ReferenciaPronduto = s.Produto.Referencia,
                    DataCompra = s.Lote.NotaFiscal.DataEmissao,
                    DataRecebimento = s.Lote.DataRecebimento,
                    IdProduto = s.IdProduto,
                    QtdRecebida = s.Quantidade,
                    IdEmpresa = s.Lote.NotaFiscal.IdEmpresa,
                    Empresa = s.Lote.NotaFiscal.Empresa.NomeFantasia,
                    IdLoteStatus = s.Lote.IdLoteStatus
                }); ;

            query = query.WhereIf(!string.IsNullOrEmpty(filter.ReferenciaPronduto), x => x.ReferenciaPronduto.ToUpper().Contains(filter.ReferenciaPronduto.ToUpper()));
            query = query.WhereIf(filter.IdLote.HasValue, x => x.IdLote == filter.IdLote);
            query = query.WhereIf(filter.NroNota.HasValue, x => x.NroNota == filter.NroNota);

            if (filter.DataCompraMinima.HasValue)
            {
                query = query.Where(x => x.DataCompra >= filter.DataCompraMinima.Value);
            }

            if (filter.DataCompraMaxima.HasValue)
            {
                DateTime data = new DateTime(filter.DataCompraMaxima.Value.Year, filter.DataCompraMaxima.Value.Month, filter.DataCompraMaxima.Value.Day, 23, 59, 59);
                query = query.Where(x => x.DataCompra <= data);
            }

            if (filter.DataRecebimentoMinima.HasValue)
            {
                query = query.Where(x => x.DataRecebimento >= filter.DataRecebimentoMinima.Value);
            }

            if (filter.DataRecebimentoMaxima.HasValue)
            {
                DateTime data = new DateTime(filter.DataRecebimentoMaxima.Value.Year, filter.DataRecebimentoMaxima.Value.Month, filter.DataRecebimentoMaxima.Value.Day, 23, 59, 59);
                query = query.Where(x => x.DataRecebimento <= data);
            }

            var loteConferencias = query.GroupBy(g => new { g.IdLote, g.IdProduto }).ToDictionary(d => d.Key, d => d.ToList());

            totalRecords = loteConferencias.Count();

            var nfItem = Entities.NotaFiscalItem.Where(w => query.Select(s => s.IdNotaFiscal).ToList().Contains(w.IdNotaFiscal)).ToList();
            var divergencias = Entities.LoteDivergencia.Where(w => query.Select(s => s.IdLote).ToList().Contains(w.IdLote)).ToList();

            foreach (var item in loteConferencias)
            {
                var conferencia = item.Value.First();

                int qtdCompra = nfItem.Where(w => w.IdNotaFiscal == conferencia.IdNotaFiscal && w.IdProduto == item.Key.IdProduto).Sum(s => s.Quantidade);

                int qtdRecebida = qtdCompra;

                if (conferencia.IdLoteStatus == LoteStatusEnum.FinalizadoDivergenciaNegativa || conferencia.IdLoteStatus == LoteStatusEnum.FinalizadoDivergenciaTodas)
                {
                    var divergencia = divergencias.FirstOrDefault(f => f.IdProduto == item.Key.IdProduto && f.IdLote == item.Key.IdLote);

                    if (divergencia != null && divergencia.QuantidadeDivergenciaMenos.HasValue)
                    {
                        qtdRecebida = qtdRecebida - divergencia.QuantidadeDivergenciaMenos.Value;
                    }
                }

                if ((filter.QtdCompraMinima.HasValue && qtdCompra < filter.QtdCompraMinima) ||
                    (filter.QtdCompraMaxima.HasValue && qtdCompra > filter.QtdCompraMaxima) ||
                    (filter.QtdRecebidaMinima.HasValue && qtdRecebida < filter.QtdRecebidaMinima) ||
                    (filter.QtdRecebidaMaxima.HasValue && qtdRecebida > filter.QtdRecebidaMaxima))
                {
                    continue;
                }

                var linha = new RelatorioRastreioPecaListaLinhaTabela()
                {
                    IdLote = item.Key.IdLote,
                    IdEmpresa = conferencia.IdEmpresa,
                    NroNota = conferencia.NroNota,
                    ReferenciaPronduto = conferencia.ReferenciaPronduto,
                    Empresa = conferencia.Empresa,
                    DataCompra = conferencia.DataCompra,
                    DataRecebimento = conferencia.DataRecebimento,
                    QtdCompra = qtdCompra,
                    QtdRecebida = qtdRecebida
                };

                resultList.Add(linha);
            }

            totalRecordsFiltered = resultList.Count();

            return resultList;
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
            string stringQuery = "SELECT rel.*, ROWNUM Ranking FROM( SELECT perfilUsu.\"Nome\", perfilUsu.\"UsuarioId\", metricasUsu.LotesRecebidasUsuario, metricasUsu.PecasRecebidasUsuario, totalLote.LotesRecebidos, totalLote.PecasRecebidas, TRUNC((metricasUsu.PecasRecebidasUsuario / totalLote.PecasRecebidas) * 100, 3) Percentual FROM ( SELECT lc.\"IdUsuarioConferente\" UsuarioId, COUNT(DISTINCT(lc.\"IdLote\")) LotesRecebidasUsuario, SUM(lc.\"Quantidade\") PecasRecebidasUsuario FROM \"LoteConferencia\" lc, \"Lote\" l, \"NotaFiscal\" n WHERE lc.\"IdLote\" = l.\"IdLote\" AND l.\"IdNotaFiscal\" = n.\"IdNotaFiscal\" AND n.\"IdEmpresa\" = :ID_EMP AND lc.\"DataHoraInicio\" >= :DATA_MIN AND (:DATA_MAX IS NULL OR lc.\"DataHoraInicio\" <= :DATA_MAX AND lc.\"Quantidade\" > 0) GROUP BY lc.\"IdUsuarioConferente\") metricasUsu, ( SELECT COUNT(DISTINCT(lc.\"IdLote\")) LotesRecebidos, SUM(lc.\"Quantidade\") PecasRecebidas FROM \"LoteConferencia\" lc, \"Lote\" l, \"NotaFiscal\" n WHERE lc.\"IdLote\" = l.\"IdLote\" AND l.\"IdNotaFiscal\" = n.\"IdNotaFiscal\" AND n.\"IdEmpresa\" = :ID_EMP AND lc.\"DataHoraInicio\" >= :DATA_MIN AND (:DATA_MAX IS NULL OR lc.\"DataHoraInicio\" <= :DATA_MAX AND lc.\"Quantidade\" > 0)) totalLote, \"PerfilUsuario\" perfilUsu WHERE perfilUsu.\"UsuarioId\" = metricasUsu.UsuarioId AND metricasUsu.PecasRecebidasUsuario > 0 ORDER BY Percentual DESC) rel";

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

        public void DeletePorIdLote(long idLote)
        {
            var conferencias = Entities.LoteConferencia.Where(w => w.IdLote == idLote).ToList();
            Entities.LoteConferencia.RemoveRange(conferencias);
        }
    }
}
