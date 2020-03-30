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
    public class ColetorHistoricoRepository : GenericRepository<ColetorHistorico>
    {
        public ColetorHistoricoRepository(Entities entities) : base(entities) { }

        public IEnumerable<HistoricoAcaoUsuarioLinhaTabela> ObterDadosParaDataTable(DataTableFilter<HistoricoAcaoUsuarioFilter> filter, out int totalRecordsFiltered, out int totalRecords)
        {
            totalRecords = Entities.ColetorHistorico.Count();

            IQueryable<HistoricoAcaoUsuarioLinhaTabela> query = Entities.ColetorHistorico.AsNoTracking()
                .Where(x => x.IdEmpresa == filter.CustomFilter.IdEmpresa &&
                (string.IsNullOrEmpty(filter.CustomFilter.IdUsuario) == true || x.IdUsuario == filter.CustomFilter.IdUsuario)).ToList()
                .Select(e => new HistoricoAcaoUsuarioLinhaTabela
                {
                    ColetorAplicacaoDescricao = e.ColetorAplicacao.Descricao,
                    DataHora = e.DataHora,
                    Descricao = e.Descricao,
                    HistoricoColetorTipoDescricao = e.ColetorHistoricoTipo.Descricao,
                    Usuario = e.Usuario.Email,
                    IdColetorAplicacao = e.IdColetorAplicacao.GetHashCode(),
                    IdHistoricoColetorTipo = e.IdColetorHistoricoTipo.GetHashCode()
                }).AsQueryable();

            if (filter.CustomFilter.IdColetorAplicacao.HasValue)
            {
                query = query.Where(x => x.IdColetorAplicacao == filter.CustomFilter.IdColetorAplicacao);
            }

            if (filter.CustomFilter.IdHistoricoColetorTipo.HasValue)
            {
                query = query.Where(x => x.IdHistoricoColetorTipo == filter.CustomFilter.IdHistoricoColetorTipo);
            }

            DateTime dataInicial = new DateTime(filter.CustomFilter.DataInicial.Year, filter.CustomFilter.DataInicial.Month, filter.CustomFilter.DataInicial.Day, 00, 00, 00);
            query = query.Where(x => x.DataHora >= dataInicial);

            DateTime dataFinal = new DateTime(filter.CustomFilter.DataFinal.Year, filter.CustomFilter.DataFinal.Month, filter.CustomFilter.DataFinal.Day, 23, 59, 59);
            query = query.Where(x => x.DataHora <= dataFinal);

            totalRecordsFiltered = query.Count();

            return query.PaginationResult(filter);
        }
    }
}
