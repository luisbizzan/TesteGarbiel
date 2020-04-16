using Dapper;
using ExtensionMethods.String;
using FWLog.Data.Models;
using FWLog.Data.Models.DataTablesCtx;
using FWLog.Data.Models.FilterCtx;
using FWLog.Data.Repository.CommonCtx;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace FWLog.Data.Repository.GeneralCtx
{
    public class GarantiaRepository : GenericRepository<Garantia>
    {
        public GarantiaRepository(Entities entities) : base(entities)
        {
        }

        public List<GarSolicitacao> ListarSolicitacao(DataTableFilter<GarantiaFilter> filter, out int totalRecordsFiltered, out int totalRecords)
        {
            List<GarSolicitacao> retorno = new List<GarSolicitacao>();
            var sQuery = new StringBuilder();

            using (var conn = new OracleConnection(Entities.Database.Connection.ConnectionString))
            {
                conn.Open();
                if (conn.State == System.Data.ConnectionState.Open)
                {
                    sQuery.AppendFormat(@"
                    SELECT
                        COUNT(*) OVER() AS Qtde,
                        GS.Cli_Cnpj,
                        GS.Id,
                        GS.Dt_Criacao,
                        GS.Id_Tipo,
                        GT1.descricao AS Tipo,
                        GS.Id_Status,
                        GT2.descricao AS Status
                    FROM
                        gar_solicitacao GS
                        LEFT JOIN geral_tipo GT1 ON GT1.Id = GS.Id_Tipo
                        LEFT JOIN geral_tipo GT2 ON GT2.Id = GS.Id_Status
                    WHERE
                        GS.id != 0
                    ");

                    if (filter.CustomFilter.Id.HasValue)
                        sQuery.AppendFormat(@" AND GS.id = {0}", filter.CustomFilter.Id);

                    if (filter.CustomFilter.Id_Status.HasValue)
                        sQuery.AppendFormat(@" AND GT2.id = {0}", filter.CustomFilter.Id_Status);

                    if (filter.CustomFilter.Id_Tipo.HasValue)
                        sQuery.AppendFormat(@" AND GT1.id = {0}", filter.CustomFilter.Id_Tipo);

                    if (!string.IsNullOrEmpty(filter.CustomFilter.Cli_Cnpj))
                        sQuery.AppendFormat(@" AND GS.Cli_Cnpj LIKE '%{0}%' ", filter.CustomFilter.Cli_Cnpj);

                    if (!string.IsNullOrEmpty(filter.CustomFilter.Nota_Fiscal))
                        sQuery.AppendFormat(@" AND GS.Nota_Fiscal LIKE '%{0}%' ", filter.CustomFilter.Nota_Fiscal);

                    if (!string.IsNullOrEmpty(filter.CustomFilter.Serie))
                        sQuery.AppendFormat(@" AND GS.Nota_Fiscal LIKE '%{0}%' ", filter.CustomFilter.Serie);

                    if (filter.CustomFilter.Data_Inicial.HasValue && filter.CustomFilter.Data_Final.HasValue)
                        sQuery.AppendFormat(@" AND GS.Dt_Criacao BETWEEN TO_DATE('{0}','DD/MM/YYYY') AND TO_DATE('{1}','DD/MM/YYYY') ", String.Format("{0:dd/MM/yyyy}", filter.CustomFilter.Data_Inicial), String.Format("{0:dd/MM/yyyy}", filter.CustomFilter.Data_Final));

                    sQuery.AppendFormat(" ORDER BY {0} {1}", filter.OrderByColumn, filter.OrderByDirection);

                    sQuery.AppendFormat(" OFFSET {0} ROWS FETCH NEXT {1} ROWS ONLY", filter.Start, filter.Length);

                    retorno = conn.Query<GarSolicitacao>(sQuery.ToString(), new { }).ToList();
                }
                conn.Close();
            }

            totalRecordsFiltered = retorno.Count();
            totalRecords = totalRecordsFiltered > 0 ? retorno.FirstOrDefault().Qtde : 0;

            return retorno;
        }

        public GarSolicitacao SelecionaSolicitacao(long Id_Solicitacao)
        {
            GarSolicitacao retorno = new GarSolicitacao();

            using (var conn = new OracleConnection(Entities.Database.Connection.ConnectionString))
            {
                conn.Open();
                if (conn.State == System.Data.ConnectionState.Open)
                {
                    string sQuery = @"
                    SELECT
                        GS.Id,
                        GS.Nota_Fiscal,
                        GS.Dt_Criacao,
                        GT1.descricao AS Tipo,
                        GS.Cli_Cnpj
                    FROM
                        gar_solicitacao GS
                        LEFT JOIN geral_tipo GT1 ON GT1.Id = GS.Id_Tipo
                    WHERE
                        GS.Id = :Id_Solicitacao
                    ";
                    retorno = conn.Query<GarSolicitacao>(sQuery, new { Id_Solicitacao }).SingleOrDefault();
                }
                conn.Close();
            }
            return retorno;
        }

        public List<GarSolicitacaoItem> ListarSolicitacaoItem(long Id_Solicitacao)
        {
            List<GarSolicitacaoItem> retorno = new List<GarSolicitacaoItem>();

            using (var conn = new OracleConnection(Entities.Database.Connection.ConnectionString))
            {
                conn.Open();
                if (conn.State == System.Data.ConnectionState.Open)
                {
                    string sQuery = @"
                    SELECT
                        Id,
                        Id_Solicitacao,
                        Id_Item_Nf,
                        Refx,
                        Cod_Fornecedor,
                        Quant,
                        Valor,
                        ROUND(Quant * Valor,2) AS Valor_Total
                    FROM
                        gar_solicitacao_item
                    WHERE
                        Id_Solicitacao = :Id_Solicitacao
                    ";
                    retorno = conn.Query<GarSolicitacaoItem>(sQuery, new { Id_Solicitacao }).ToList();
                }
                conn.Close();
            }

            return retorno;
        }
    }
}