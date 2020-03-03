using Dapper;
using ExtensionMethods.String;
using FWLog.Data.Models;
using FWLog.Data.Models.DataTablesCtx;
using FWLog.Data.Models.FilterCtx;
using FWLog.Data.Repository.CommonCtx;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FWLog.Data.Repository.GeneralCtx
{
    public class GarantiaRepository : GenericRepository<Garantia>
    {
        public GarantiaRepository(Entities entities) : base(entities)
        {
        }

        public IList<GarantiaTableRow> SearchForDataTable(DataTableFilter<GarantiaFilter> filter, out int totalRecordsFiltered, out int totalRecords)
        {
            IEnumerable<Garantia> garantia = null;

            using (var conn = new OracleConnection(Entities.Database.Connection.ConnectionString))
            {
                conn.Open();

                if (conn.State == System.Data.ConnectionState.Open)
                {
                    garantia = conn.Query<Garantia, NotaFiscal, Cliente, GarantiaStatus, AspNetUsers, NotaFiscalStatus, Garantia>(
                        "SELECT " +
                            "garantia.\"IdGarantia\"," +
                            "garantia.\"DataRecebimento\",	" +
                            "nota.\"IdNotaFiscal\", " +
                            "nota.\"Numero\"," +
                            "nota.\"Serie\"," +
                            "nota.\"ValorTotal\"," +
                            "nota.\"ValorFrete\"," +
                            "nota.\"NumeroConhecimento\"," +
                            "nota.\"PesoBruto\"," +
                            "nota.\"Especie\"," +
                            "nota.\"Quantidade\"," +
                            "nota.\"ChaveAcesso\"," +
                            "nota.\"CodigoIntegracao\"," +
                            "nota.\"DataEmissao\"," +
                            "nota.\"PrazoEntregaFornecedor\"," +
                            "nota.\"IdEmpresa\"," +
                            "nota.\"IdTransportadora\"," +
                            "nota.\"IdFornecedor\"," +
                            "cliente.*,	" +
                            "garantiastatus.*," +
                            "userr.*," +
                            "nfstatus.* " +
                        "FROM \"Garantia\" garantia " +
                        "RIGHT JOIN \"NotaFiscal\" nota ON nota.\"IdNotaFiscal\" = garantia.\"IdNotaFiscal\" " +
                        "INNER JOIN \"Cliente\" cliente ON cliente.\"IdCliente\" = nota.\"IdCliente\" " +
                        "LEFT JOIN \"GarantiaStatus\" garantiastatus ON (garantiastatus.\"IdGarantiaStatus\" = CASE WHEN garantia.\"IdGarantiaStatus\" IS NULL THEN 1 ELSE garantia.\"IdGarantiaStatus\" END) " +
                        "LEFT JOIN \"AspNetUsers\" userr ON userr.\"Id\" = garantia.\"IdUsuarioConferente\" " +
                        "INNER JOIN \"NotaFiscalStatus\" nfstatus ON nfstatus.\"IdNotaFiscalStatus\" = nota.\"IdNotaFiscalStatus\" " +
                        "WHERE (nota.\"IdNotaFiscalStatus\" <> 0 AND nota.\"IdNotaFiscalStatus\" IS NOT NULL) AND nota.\"IdEmpresa\" = " + filter.CustomFilter.IdEmpresa +
                        "AND nota.\"IdNotaFiscalTipo\" = " + NotaFiscalTipoEnum.Garantia.GetHashCode(),
                        map: (g, nf, f, gs, u, nfs) =>
                        {
                            g.NotaFiscal = nf;
                            g.NotaFiscal.Cliente = f;
                            g.GarantiaStatus = gs;
                            g.UsuarioConferente = u;
                            g.NotaFiscal.NotaFiscalStatus = nfs;
                            return g;
                        },
                        splitOn: "IdGarantia, IdNotaFiscal, IdCliente, IdGarantiaStatus, Id, IdNotaFiscalStatus"
                        );
                }

                conn.Close();
            }

            totalRecords = garantia.Count();

            var query = garantia.Where(x =>
                (filter.CustomFilter.IdGarantia.HasValue == false || x.IdGarantia == filter.CustomFilter.IdGarantia.Value) &&
                (filter.CustomFilter.IdCliente.HasValue == false || x.NotaFiscal.IdCliente == filter.CustomFilter.IdCliente.Value) &&
                (filter.CustomFilter.IdTransportadora.HasValue == false || x.NotaFiscal.IdTransportadora == filter.CustomFilter.IdTransportadora.Value) &&
                (filter.CustomFilter.IdFornecedor.HasValue == false || x.NotaFiscal.IdFornecedor == filter.CustomFilter.IdFornecedor.Value) &&
                (filter.CustomFilter.NumeroNF.HasValue == false || x.NotaFiscal.Numero == filter.CustomFilter.NumeroNF.Value) &&
                (string.IsNullOrEmpty(filter.CustomFilter.NumeroFicticioNF) == true || x.NotaFiscal.NumeroFicticioNF.Contains(filter.CustomFilter.NumeroFicticioNF)) &&
                (string.IsNullOrEmpty(filter.CustomFilter.ChaveAcesso) == true || x.NotaFiscal.ChaveAcesso.Contains(filter.CustomFilter.ChaveAcesso)) &&
                (filter.CustomFilter.IdGarantiaStatus.HasValue == false || (long)x.IdGarantiaStatus == filter.CustomFilter.IdGarantiaStatus.Value)
            );

            if (filter.CustomFilter.DataEmissaoInicial.HasValue)
            {
                DateTime dataInicial = new DateTime(filter.CustomFilter.DataEmissaoInicial.Value.Year, filter.CustomFilter.DataEmissaoInicial.Value.Month, filter.CustomFilter.DataEmissaoInicial.Value.Day, 00, 00, 00);
                query = query.Where(x => x.NotaFiscal.DataEmissao >= dataInicial);
            }

            if (filter.CustomFilter.DataEmissaoFinal.HasValue)
            {
                DateTime dataFinal = new DateTime(filter.CustomFilter.DataEmissaoFinal.Value.Year, filter.CustomFilter.DataEmissaoFinal.Value.Month, filter.CustomFilter.DataEmissaoFinal.Value.Day, 23, 59, 59);
                query = query.Where(x => x.NotaFiscal.DataEmissao <= dataFinal);
            }

            if (filter.CustomFilter.DataRecebimentoInicial.HasValue)
            {
                DateTime dataInicial = new DateTime(filter.CustomFilter.DataRecebimentoInicial.Value.Year, filter.CustomFilter.DataRecebimentoInicial.Value.Month, filter.CustomFilter.DataRecebimentoInicial.Value.Day, 23, 59, 59);
                query = query.Where(x => x.DataRecebimento <= dataInicial);
            }

            if (filter.CustomFilter.DataRecebimentoFinal.HasValue)
            {
                DateTime dataFinal = new DateTime(filter.CustomFilter.DataRecebimentoFinal.Value.Year, filter.CustomFilter.DataRecebimentoFinal.Value.Month, filter.CustomFilter.DataRecebimentoFinal.Value.Day, 00, 00, 00);
                query = query.Where(x => x.DataRecebimento >= dataFinal);
            }

            if (!filter.CustomFilter.IdUsuarioConferente.NullOrEmpty())
            {
                query = query.Where(x => x.UsuarioConferente?.Id == filter.CustomFilter.IdUsuarioConferente);
            }

            IEnumerable<GarantiaTableRow> queryResult = query.Select(e => new GarantiaTableRow
            {
                IdGarantia = e.IdGarantia == 0 ? (long?)null : e.IdGarantia,
                Cliente = string.Concat(e.NotaFiscal.Cliente.IdCliente, "-", e.NotaFiscal.Cliente.RazaoSocial),
                CNPJCliente = e.NotaFiscal.Cliente.CNPJCPF.CnpjOuCpf(),
                Transportadora = e.NotaFiscal.Transportadora == null ? string.Empty : string.Concat(e.NotaFiscal.Transportadora.IdTransportadora, "-", e.NotaFiscal.Transportadora.NomeFantasia),
                Fornecedor = e.NotaFiscal.Fornecedor == null ? string.Empty : string.Concat(e.NotaFiscal.Fornecedor.IdFornecedor, "-", e.NotaFiscal.Fornecedor.NomeFantasia),
                IdEmpresa = e.NotaFiscal.IdEmpresa,
                NumeroNF = e.NotaFiscal.Numero,
                NumeroFicticioNF = e.NotaFiscal.NumeroFicticioNF,
                DataEmissao = e.NotaFiscal.DataEmissao,
                //Tratar quando for MinValue
                DataRecebimento = e.DataRecebimento != DateTime.MinValue ? DateTime.Parse(e.DataRecebimento.ToString("dd//MM/yyyy")) : (DateTime?)null,                
                GarantiaStatus = e.GarantiaStatus.Descricao,
                IdNotaFiscal = e.NotaFiscal.IdNotaFiscal
            });

            totalRecordsFiltered = queryResult.Count();

            queryResult = queryResult
                .OrderBy(filter.OrderByColumn, filter.OrderByDirection)
                .Skip(filter.Start)
                .Take(filter.Length);

            return queryResult.ToList();
        }

        public long BuscarNotaPeloIdGarantia(int idGarantia)
        {
            return Entities.Garantia.Where(x => x.IdGarantia == idGarantia).Select(x => x.IdNotaFiscal).FirstOrDefault();
        }
    }
}
