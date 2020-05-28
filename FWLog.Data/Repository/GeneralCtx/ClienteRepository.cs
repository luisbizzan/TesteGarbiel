using ExtensionMethods.List;
using FWLog.Data.Models;
using FWLog.Data.Models.DataTablesCtx;
using FWLog.Data.Models.FilterCtx;
using FWLog.Data.Repository.CommonCtx;
using System.Collections.Generic;
using System.Linq;

namespace FWLog.Data.Repository.GeneralCtx
{
    public class ClienteRepository : GenericRepository<Cliente>
    {
        public ClienteRepository(Entities entities) : base(entities) { }

        public Cliente ConsultarPorCodigoIntegracao(long codigoIntegracao)
        {
            return Entities.Cliente.FirstOrDefault(f => f.CodigoIntegracao == codigoIntegracao);
        }

        public IEnumerable<ClientePesquisaModalLinhaTabela> ObterDadosParaDataTable(DataTableFilter<ClientePesquisaModalFiltro> filter, out int totalRecordsFiltered, out int totalRecords)
        {
            totalRecords = Entities.Cliente.Count();

            IQueryable<ClientePesquisaModalLinhaTabela> query = Entities.Cliente.AsNoTracking()
                .Where(x => (filter.CustomFilter.IdCliente.HasValue == false || x.IdCliente == filter.CustomFilter.IdCliente) &&
                (filter.CustomFilter.RazaoSocial.Equals(string.Empty) || x.RazaoSocial.Contains(filter.CustomFilter.RazaoSocial)) &&
                (filter.CustomFilter.CNPJCPF.Equals(string.Empty) || x.CNPJCPF.Contains(filter.CustomFilter.CNPJCPF.Replace(".", "").Replace("/", "").Replace("-", ""))))
                .Select(e => new ClientePesquisaModalLinhaTabela
                {
                    IdCliente = e.IdCliente,
                    CNPJCPF = e.CNPJCPF,
                    Status = e.Ativo ? "Ativo" : "Inativo",
                    CodigoIntegracao = e.CodigoIntegracao,
                    Classificacao = e.Classificacao,
                    NomeFantasia = e.NomeFantasia,
                    RazaoSocial = e.RazaoSocial
                }) ;

            totalRecordsFiltered = query.Count();

            return query.PaginationResult(filter);
        }
    }
}
