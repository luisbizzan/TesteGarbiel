using ExtensionMethods.List;
using FWLog.Data.Models;
using FWLog.Data.Models.DataTablesCtx;
using FWLog.Data.Models.FilterCtx;
using FWLog.Data.Repository.CommonCtx;
using System.Collections.Generic;
using System.Linq;

namespace FWLog.Data.Repository.GeneralCtx
{
    public class TransportadoraRepository : GenericRepository<Transportadora>
    {
        public TransportadoraRepository(Entities entities) : base(entities)
        {

        }

        public Transportadora ConsultarPorCodigoIntegracao(long codigoIntegracao)
        {
            return Entities.Transportadora.FirstOrDefault(f => f.CodigoIntegracao == codigoIntegracao);
        }

        public IQueryable<Transportadora> Todos()
        {
            return Entities.Transportadora;
        }

        public IEnumerable<TransportadoraPesquisaModalLinhaTabela> ObterDadosParaDataTable(DataTableFilter<TransportadoraPesquisaModalFiltro> filter, out int totalRecordsFiltered, out int totalRecords)
        {
            totalRecords = Entities.Transportadora.Count();

            IQueryable<TransportadoraPesquisaModalLinhaTabela> query = Entities.Transportadora.AsNoTracking()
                .Where(x => (filter.CustomFilter.IdTransportadora.HasValue == false || x.IdTransportadora == filter.CustomFilter.IdTransportadora) &&
                (filter.CustomFilter.NomeFantasia.Equals(string.Empty) || x.NomeFantasia.Contains(filter.CustomFilter.NomeFantasia)) &&
                (filter.CustomFilter.CNPJ.Equals(string.Empty) || x.CNPJ.Contains(filter.CustomFilter.CNPJ.Replace(".", "").Replace("/", "").Replace("-", ""))))
                .Select(e => new TransportadoraPesquisaModalLinhaTabela
                {
                    IdTransportadora = e.IdTransportadora,
                    CNPJ = e.CNPJ,
                    Status = e.Ativo ? "Ativo" : "Inativo",
                    CodigoIntegracao = e.CodigoIntegracao,
                    NomeFantasia = e.NomeFantasia,
                    RazaoSocial = e.RazaoSocial
                });

            totalRecordsFiltered = query.Count();

            return query.PaginationResult(filter);
        }

        public Transportadora ConsultarPorCodigoTransportadora(string codigoTransportadora)
        {
            return Entities.Transportadora.FirstOrDefault(f => f.CodigoTransportadora == codigoTransportadora);
        }
    }
}