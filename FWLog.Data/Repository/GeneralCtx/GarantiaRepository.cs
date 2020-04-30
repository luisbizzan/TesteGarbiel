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
using System.Data;
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
                if (conn.State == ConnectionState.Open)
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
                if (conn.State == ConnectionState.Open)
                {
                    string sQuery = @"
                    SELECT
                        GS.Id,
                        GS.Nota_Fiscal,
                        GS.Filial,
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
                if (conn.State == ConnectionState.Open)
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

        public List<GarMotivoLaudo> ListarMotivoLaudo()
        {
            List<GarMotivoLaudo> retorno = new List<GarMotivoLaudo>();

            using (var conn = new OracleConnection(Entities.Database.Connection.ConnectionString))
            {
                conn.Open();
                if (conn.State == ConnectionState.Open)
                {
                    string sQuery = @"
                    SELECT
                        GML.Id,
                        GML.Descricao,
                        GML.Id_Tipo,
                        GT.Descricao AS Tipo
                    FROM
                        gar_motivo_laudo GML
                        INNER JOIN geral_tipo GT ON GT.id = GML.Id_Tipo AND GT.tabela = 'GAR_MOTIVO_LAUDO' AND GT.coluna = 'ID_TIPO'
                    ORDER BY GT.Descricao, GML.Descricao

                    ";
                    retorno = conn.Query<GarMotivoLaudo>(sQuery, new { }).ToList();
                }
                conn.Close();
            }

            return retorno;
        }

        public long PegaIdUltimaConferenciaAtiva(string Origem, long Id)
        {
            long retorno = 0;
            string sQuery = "";

            using (var conn = new OracleConnection(Entities.Database.Connection.ConnectionString))
            {
                conn.Open();
                if (conn.State == ConnectionState.Open)
                {
                    if (Origem == "solicitacao")
                        sQuery = @"SELECT id FROM gar_conferencia WHERE Ativo = 1 AND Id_Solicitacao = :Id ORDER BY id DESC FETCH FIRST 1 ROWS ONLY";
                    else
                        sQuery = @"SELECT id FROM gar_conferencia WHERE Ativo = 1 AND Id_Remessa = :Id ORDER BY id DESC FETCH FIRST 1 ROWS ONLY";

                    retorno = conn.Query<long>(sQuery, new { Id }).SingleOrDefault();
                }
                conn.Close();
            }
            return retorno;
        }

        public GarConferencia SelecionaConferencia(long Id_Conferencia)
        {
            GarConferencia retorno = new GarConferencia();

            using (var conn = new OracleConnection(Entities.Database.Connection.ConnectionString))
            {
                conn.Open();
                if (conn.State == ConnectionState.Open)
                {
                    string sQuery = @"
                    SELECT
                        GC.Id,
                        GT.Descricao AS Tipo_Conf,
                        GC.Id_Tipo_Conf,
                        GC.Id_Remessa,
                        GC.Id_Solicitacao
                    FROM
                        gar_conferencia GC
                        INNER JOIN geral_tipo GT ON GT.id = GC.Id_Tipo_Conf AND GT.tabela = 'GAR_CONFERENCIA' AND GT.coluna = 'ID_TIPO_CONF'
                    WHERE
                        GC.Id = :Id_Conferencia
                    ";
                    retorno = conn.Query<GarConferencia>(sQuery, new { Id_Conferencia }).SingleOrDefault();
                }
                conn.Close();
            }
            return retorno;
        }

        public void AtualizarItemConferencia(GarConferenciaItem item)
        {
            using (var conn = new OracleConnection(Entities.Database.Connection.ConnectionString))
            {
                string sQuery = "";
                conn.Open();
                if (conn.State == ConnectionState.Open)
                {
                    sQuery = @"
                    SELECT
                        (SELECT COUNT(*) FROM gar_conferencia_item WHERE id_conf = GC.id AND refx = :Refx ) AS tem_na_nota,
                        (SELECT COUNT(*) FROM gar_conferencia_excesso WHERE id_conf = GC.id AND refx = :Refx ) AS tem_no_excesso
                    FROM
                        gar_conferencia GC
                    WHERE
                        GC.id = :Id_Conf
                    ";
                    var retorno = conn.Query<GarConferenciaItem>(sQuery, new { item.Refx, item.Id_Conf }).SingleOrDefault();

                    if (retorno.Tem_Na_Nota == 1)
                    {
                        sQuery = @"
                        UPDATE gar_conferencia_item SET Quant_Conferida = Quant_Conferida + :Quant_Conferida, Id_Usr = :Id_Usr, Dt_Conf = SYSDATE
                        WHERE Id_Conf = :Id_Conf AND Refx = :Refx
                        ";
                        conn.Query<GarConferenciaItem>(sQuery, new
                        {
                            item.Quant_Conferida,
                            item.Id_Usr,
                            item.Id_Conf,
                            item.Refx
                        });
                    }
                    else if (retorno.Tem_No_Excesso == 0)
                    {
                        sQuery = @"
                        INSERT INTO gar_conferencia_excesso
                        ( Quant_Conferida, Id_Conf, Refx, Id_Usr, Dt_Conf )
                        VALUES
                        ( :Quant_Conferida, :Id_Conf, :Refx, :Id_Usr, SYSDATE )
                        ";
                        conn.Query<GarConferenciaItem>(sQuery, new
                        {
                            item.Quant_Conferida,
                            item.Id_Conf,
                            item.Refx,
                            item.Id_Usr
                        });
                    }
                    else if (retorno.Tem_No_Excesso == 1)
                    {
                        sQuery = @"
                         UPDATE gar_conferencia_excesso SET Quant_Conferida = Quant_Conferida + :Quant_Conferida, Id_Usr = :Id_Usr, Dt_Conf = SYSDATE
                        WHERE Id_Conf = :Id_Conf AND Refx = :Refx
                        ";
                        conn.Query<GarConferenciaItem>(sQuery, new
                        {
                            item.Quant_Conferida,
                            item.Id_Usr,
                            item.Id_Conf,
                            item.Refx
                        });

                        sQuery = @"DELETE  FROM gar_conferencia_excesso WHERE Id_Conf = :Id_Conf AND Refx = :Refx AND Quant_Conferida <= 0";
                        conn.Query<GarConferenciaItem>(sQuery, new
                        {
                            item.Id_Conf,
                            item.Refx
                        });
                    }
                }
                conn.Close();
            }
        }

        public void CriarConferencia(GarConferencia item)
        {
            using (var conn = new OracleConnection(Entities.Database.Connection.ConnectionString))
            {
                string sQuery = "";
                conn.Open();
                if (conn.State == ConnectionState.Open)
                {
                    sQuery = @"
                    SELECT COUNT(*) FROM gar_conferencia WHERE id_solicitacao = :Id_Solicitacao AND ativo = 1
                    ";
                    var retorno = conn.Query<int>(sQuery, new { item.Id_Solicitacao }).SingleOrDefault();

                    if (retorno == 0)
                    {
                        var param = new DynamicParameters();

                        param.Add(name: "Id_Solicitacao", value: item.Id_Solicitacao, direction: ParameterDirection.Input);
                        param.Add(name: "Id_Tipo_Conf", value: item.Id_Tipo_Conf, direction: ParameterDirection.Input);
                        param.Add(name: "Id_Usr", value: item.Id_Usr, direction: ParameterDirection.Input);
                        param.Add(name: "Id", dbType: DbType.Int32, direction: ParameterDirection.Output);

                        conn.Execute("INSERT INTO gar_conferencia (Id_Solicitacao, Id_Tipo_Conf, Id_Usr, Dt_Conf) VALUES (:Id_Solicitacao, :Id_Tipo_Conf, :Id_Usr, SYSDATE) returning Id into :Id", param);

                        var Id_Conferencia = param.Get<int>("Id");

                        sQuery = @"
                        INSERT INTO gar_conferencia_item (refx, id_conf, dt_conf, id_usr,quant)
                        SELECT
                            refx,
                            :Id_Conferencia,
                            SYSDATE AS dt_conf,
                            :id_usr,
                            SUM(quant) AS quant
                        FROM
                            gar_solicitacao_item
                        WHERE
                            id_solicitacao = :Id_Solicitacao
                        GROUP BY
                            refx
                        ";
                        conn.Query<GarConferenciaHist>(sQuery, new
                        {
                            Id_Conferencia,
                            item.Id_Usr,
                            item.Id_Solicitacao
                        });
                    }
                }
                conn.Close();
            }
        }

        #region Conferencia Historico

        public void InserirConferenciaHistorico(GarConferenciaHist item)
        {
            //TODO VERIFICAR SE ITEM EXISTE NA TABELA DE ESTOQUE
            using (var conn = new OracleConnection(Entities.Database.Connection.ConnectionString))
            {
                conn.Open();
                if (conn.State == ConnectionState.Open)
                {
                    string sQuery = @"
                    INSERT INTO gar_conferencia_hist
	                    ( Id_Conf, Refx, Volume, Id_Usr, Quant_Conferida, Dt_Conf )
                    VALUES
	                    ( :Id_Conf, :Refx, :Volume, :Id_Usr, :Quant_Conferida, SYSDATE )
                    ";
                    conn.Query<GarConferenciaHist>(sQuery, new
                    {
                        item.Id_Conf,
                        item.Refx,
                        item.Volume,
                        item.Id_Usr,
                        item.Quant_Conferida
                    });
                }
                conn.Close();
            }
        }

        public List<GarConferenciaHist> ListarConferenciaHistorico(long Id_Conferencia)
        {
            List<GarConferenciaHist> retorno = new List<GarConferenciaHist>();

            using (var conn = new OracleConnection(Entities.Database.Connection.ConnectionString))
            {
                conn.Open();
                if (conn.State == ConnectionState.Open)
                {
                    string sQuery = @"
                    SELECT
                        Id,
                        Id_Conf,
                        Refx,
                        Volume,
                        Dt_Conf,
                        Quant_Conferida
                    FROM
                        gar_conferencia_hist
                    WHERE
                        Id_Conf = :Id_Conferencia
                    ORDER BY
                        Id DESC
                    ";
                    retorno = conn.Query<GarConferenciaHist>(sQuery, new { Id_Conferencia }).ToList();
                }
                conn.Close();
            }

            return retorno;
        }

        public List<GarConferenciaHist> ListarConferenciaItemPendente(long Id_Conferencia)
        {
            List<GarConferenciaHist> retorno = new List<GarConferenciaHist>();

            using (var conn = new OracleConnection(Entities.Database.Connection.ConnectionString))
            {
                conn.Open();
                if (conn.State == ConnectionState.Open)
                {
                    string sQuery = @"
                    SELECT
                        GCI.Id,
                        GCI.Id_Conf,
                        GCI.Refx
                    FROM
                        gar_conferencia_item GCI
                    WHERE
                        GCI.Id_Conf = :Id_Conferencia
                        AND GCI.Refx NOT IN(SELECT GCH.refx FROM gar_conferencia_hist GCH WHERE GCH.Id_Conf = GCI.Id_Conf)
                    ";
                    retorno = conn.Query<GarConferenciaHist>(sQuery, new { Id_Conferencia }).ToList();
                }
                conn.Close();
            }

            return retorno;
        }

        #endregion Conferencia Historico

        public List<GarConferenciaItem> ListarConferenciaItem(long Id_Conferencia)
        {
            List<GarConferenciaItem> retorno = new List<GarConferenciaItem>();

            using (var conn = new OracleConnection(Entities.Database.Connection.ConnectionString))
            {
                conn.Open();
                if (conn.State == ConnectionState.Open)
                {
                    string sQuery = @"
                    SELECT
                        Id,
                        Id_Conf,
                        Refx,
                        (Quant_Conferida - Quant) AS Divergencia,
                        Quant,
                        0 AS Tem_No_Excesso,
                        Quant_Conferida
                    FROM
                        gar_conferencia_item
                    WHERE
                        Id_Conf = :Id_Conferencia
                    UNION
                    SELECT
                        Id,
                        Id_Conf,
                        Refx,
                        0 AS Divergencia,
                        0 AS Quant,
                        1 AS Tem_No_Excesso,
                        Quant_Conferida
                    FROM
                        gar_conferencia_excesso
                    WHERE
                        Id_Conf = :Id_Conferencia
                    ";
                    retorno = conn.Query<GarConferenciaItem>(sQuery, new { Id_Conferencia }).ToList();
                }
                conn.Close();
            }

            return retorno;
        }
    }
}