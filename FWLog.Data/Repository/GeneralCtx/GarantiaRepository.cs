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

        public long ImportarSolicitacaoNfCartaManual(GarSolicitacao item, string Codigo_Postagem)
        {
            string sQuery = "";
            bool podeImportar = false;
            long Id_Solicitacao = 0;

            using (var conn = new OracleConnection(Entities.Database.Connection.ConnectionString))
            {
                conn.Open();
                if (conn.State == ConnectionState.Open)
                {
                    //VERIFICA SE JA FOI IMPORTADO
                    if (item.Id_Tipo_Doc == 31)
                    {
                        sQuery = @"SELECT COUNT(*) FROM gar_solicitacao WHERE Nota_Fiscal = :Nota_Fiscal AND Cli_Cnpj = :Cli_Cnpj AND Serie = :Serie";
                        podeImportar = conn.Query<int>(sQuery, new { item.Nota_Fiscal, item.Cli_Cnpj, item.Serie }).SingleOrDefault() == 0;
                    }
                    else if (item.Id_Tipo_Doc == 32)
                    {
                        sQuery = @"SELECT COUNT(*) FROM gar_solicitacao WHERE Nota_Fiscal = :Nota_Fiscal";
                        podeImportar = conn.Query<int>(sQuery, new { item.Nota_Fiscal }).SingleOrDefault() == 0;
                    }

                    if (!podeImportar)
                        return 0;

                    var solicitacao = new GarSolicitacao();

                    if (item.Id_Tipo_Doc == 31)
                    {
                        //NF MANUAL
                        sQuery = @"
                            SELECT
                                remetente_id AS filial,
                                SYSDATE AS dt_criacao,
                                Decode(tipo_ocorrencia,'D',17,18) AS id_tipo,
                                cnpj AS cli_cnpj,
                                22 AS id_status,
                                :Id_Usr AS id_usr,
                                Motivo_id AS legenda,
                                id AS id_sav,
                                nota_fiscal,
                                serie,
                                31 AS id_tipo_doc
                            FROM
                                fdv_devgar@furacaophp
                            WHERE
                                cnpj = :Cli_Cnpj
                                AND nota_fiscal = :Nota_Fiscal
                                AND serie = :Serie
                                AND finalizado NOT IN (0,5)
                            ";
                        solicitacao = conn.Query<GarSolicitacao>(sQuery, new { item.Id_Usr, item.Cli_Cnpj, item.Nota_Fiscal, item.Serie }).SingleOrDefault();
                    }
                    else if (item.Id_Tipo_Doc == 32)
                    {
                        //NF CARTA
                        sQuery = @"
                            SELECT
                                remetente_id AS filial,
                                SYSDATE AS dt_criacao,
                                Decode(tipo_ocorrencia,'D',17,18) AS id_tipo,
                                cnpj AS cli_cnpj,
                                22 AS id_status,
                                :Id_Usr AS id_usr,
                                Motivo_id AS legenda,
                                id AS id_sav,
                                nota_fiscal,
                                serie,
                                32 AS id_tipo_doc
                            FROM
                                fdv_devgar@furacaophp
                            WHERE
                                nota_fiscal = :Nota_Fiscal
                                AND finalizado NOT IN (0,5)
                            ";
                        solicitacao = conn.Query<GarSolicitacao>(sQuery, new { item.Id_Usr, item.Nota_Fiscal }).SingleOrDefault();
                    }

                    if (solicitacao == null)
                        return 0;

                    long Id_DevGar = solicitacao.Id_Sav;

                    //ATUALIZA CODIGO RASTREIO DO SAV
                    sQuery = @"UPDATE fdv_devgar@furacaophp SET correios = :Codigo_Postagem WHERE Id = :Id_DevGar";
                    conn.Query<GarConferencia>(sQuery, new { Codigo_Postagem, Id_DevGar });

                    var param = new DynamicParameters();
                    param.Add(name: "Filial", value: solicitacao.Filial, direction: ParameterDirection.Input);
                    param.Add(name: "Dt_Criacao", value: solicitacao.Dt_Criacao, direction: ParameterDirection.Input);
                    param.Add(name: "Id_Tipo", value: solicitacao.Id_Tipo, direction: ParameterDirection.Input);
                    param.Add(name: "Cli_Cnpj", value: solicitacao.Cli_Cnpj, direction: ParameterDirection.Input);
                    param.Add(name: "Id_Status", value: solicitacao.Id_Status, direction: ParameterDirection.Input);
                    param.Add(name: "Legenda", value: solicitacao.Legenda, direction: ParameterDirection.Input);
                    param.Add(name: "Id_Usr", value: solicitacao.Id_Usr, direction: ParameterDirection.Input);
                    param.Add(name: "Id_Sav", value: solicitacao.Id_Sav, direction: ParameterDirection.Input);
                    param.Add(name: "Nota_Fiscal", value: solicitacao.Nota_Fiscal, direction: ParameterDirection.Input);
                    param.Add(name: "Serie", value: solicitacao.Serie, direction: ParameterDirection.Input);
                    param.Add(name: "Id_Tipo_Doc", value: item.Id_Tipo_Doc, direction: ParameterDirection.Input);
                    param.Add(name: "Id", dbType: DbType.Int32, direction: ParameterDirection.Output);

                    conn.Execute(@"INSERT INTO gar_solicitacao (Filial, Dt_Criacao, Id_Tipo, Cli_Cnpj,Id_Status,Legenda,Id_Usr,Id_Sav,Nota_Fiscal,Serie,Id_Tipo_Doc)
                            VALUES ( :Filial, :Dt_Criacao, :Id_Tipo, :Cli_Cnpj,:Id_Status,:Legenda,:Id_Usr,:Id_Sav,:Nota_Fiscal,:Serie,:Id_Tipo_Doc )
                            returning Id into :Id", param);

                    Id_Solicitacao = param.Get<int>("Id");

                    if (Id_Solicitacao != 0)
                    {
                        //ITENS DA NF
                        sQuery = @"
                            INSERT INTO gar_solicitacao_item (id_solicitacao, id_item_nf, refx, cod_fornecedor,quant,valor)
                            SELECT
                                :Id_Solicitacao AS id_solicitacao,
                                num_origem_nf AS id_item_nf,
                                cod_produto AS refx,
                                (SELECT codparcforn FROM tgfpro@sankhya WHERE ad_refx = cod_produto) AS cod_fornecedor,
                                quantidade AS quant,
                                vlr_unitario AS valor
                            FROM
                                fdv_devgar_itens@furacaophp
                            WHERE
                                devgar_id = :Id_DevGar
                            ";
                        conn.Query<GarConferenciaHist>(sQuery, new
                        {
                            Id_Solicitacao,
                            Id_DevGar
                        });
                    }
                }
                conn.Close();
            }

            return Id_Solicitacao;
        }

        public void EstornarSolicitacao(GarSolicitacao item)
        {
            using (var conn = new OracleConnection(Entities.Database.Connection.ConnectionString))
            {
                string sQuery = "";
                conn.Open();
                if (conn.State == ConnectionState.Open)
                {
                    //Deletar a solicitação
                    sQuery = @"DELETE FROM gar_solicitacao WHERE Id = :Id AND Id_Status != 24";
                    conn.Query<GarConferencia>(sQuery, new { item.Id });

                    //Deletar a conferencia
                    sQuery = @"DELETE FROM gar_conferencia WHERE Id_Solicitacao = :Id";
                    conn.Query<GarConferencia>(sQuery, new { item.Id });
                }
                conn.Close();
            }
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
                        GSI.Id,
                        GSI.Id_Solicitacao,
                        GSI.Id_Item_Nf,
                        GSI.Refx,
                        GSI.Cod_Fornecedor,
                        GSI.Quant,
                        TP.descrprod AS descricao,
                        GSI.Valor,
                        ROUND(GSI.Quant * GSI.Valor,2) AS Valor_Total
                    FROM
                        gar_solicitacao_item GSI
                        INNER JOIN tgfpro@sankhya TP ON TP.ad_refx = GSI.refx
                    WHERE
                        GSI.Id_Solicitacao = :Id_Solicitacao
                    ";
                    retorno = conn.Query<GarSolicitacaoItem>(sQuery, new { Id_Solicitacao }).ToList();
                }
                conn.Close();
            }

            return retorno;
        }

        public List<GarMotivoLaudo> ListarMotivoLaudo(long Id_Tipo)
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
                    WHERE
                        GML.Id_Tipo = :Id_Tipo
                    ORDER BY GT.Descricao, GML.Descricao

                    ";
                    retorno = conn.Query<GarMotivoLaudo>(sQuery, new { Id_Tipo }).ToList();
                }
                conn.Close();
            }

            return retorno;
        }

        public List<GarSolicitacaoItemLaudo> ListarConferenciaSolicitacaoLaudo(long Id_Conferencia)
        {
            List<GarSolicitacaoItemLaudo> retorno = new List<GarSolicitacaoItemLaudo>();

            using (var conn = new OracleConnection(Entities.Database.Connection.ConnectionString))
            {
                conn.Open();
                if (conn.State == ConnectionState.Open)
                {
                    string sQuery = @"
                    SELECT Id_Conf, Refx, Quant, Tem_No_Excesso, (Quant_Laudo + Quant_Laudo_Automatico) AS Quant_Laudo  FROM(
                        SELECT
                            GCI.Id_Conf,
                            GCI.Refx,
                            CASE WHEN  (Quant_Conferida - Quant) < 0 THEN (Quant_Conferida - Quant) * -1 ELSE (Quant_Conferida - Quant) END AS Quant_Laudo_Automatico,
                            NVL((SELECT SUM(GSIL.quant) FROM gar_solicitacao_item_laudo GSIL INNER JOIN gar_solicitacao_item GSI ON GSI.id = GSIL.id_item
                            WHERE GSI.id_solicitacao = GC.id_solicitacao AND GSI.refx = GCI.refx  ),0) AS Quant_Laudo,
                            GCI.Quant,
                            0 AS Tem_No_Excesso
                        FROM
                            gar_conferencia_item GCI
                            INNER JOIN gar_conferencia GC ON GC.id = GCI.Id_Conf
                        WHERE
                            GCI.Id_Conf = :Id_Conferencia
                        UNION
                        SELECT
                            Id_Conf,
                            Refx,
                            Quant_Conferida AS Quant_Laudo_Automatico,
                            0 AS Quant_Laudo,
                            Quant_Conferida AS Quant,
                            1 AS Tem_No_Excesso
                        FROM
                            gar_conferencia_excesso
                        WHERE
                            Id_Conf = :Id_Conferencia
                    )

                    ";
                    retorno = conn.Query<GarSolicitacaoItemLaudo>(sQuery, new { Id_Conferencia }).ToList();
                }
                conn.Close();
            }

            return retorno;
        }

        public List<GarSolicitacaoItemLaudo> ListarConferenciaSolicitacaoLaudoDetalhe(long Id_Conferencia, string Refx)
        {
            List<GarSolicitacaoItemLaudo> retorno = new List<GarSolicitacaoItemLaudo>();

            using (var conn = new OracleConnection(Entities.Database.Connection.ConnectionString))
            {
                conn.Open();
                if (conn.State == ConnectionState.Open)
                {
                    string sQuery = @"
                    SELECT
                        L.Id,
                        GML.descricao AS Motivo,
                        L.Id_Item_Nf,
                        L.Quant_Laudo
                    FROM(
                        SELECT
                            0 AS Id,
                            GCI.Refx,
                            (Quant_Conferida - Quant) AS Quant_Laudo,
                            0 AS Id_Item_Nf,
                            CASE WHEN  (Quant_Conferida - Quant) < 0 THEN 3 ELSE 2 END AS Id_Motivo
                        FROM
                            gar_conferencia_item GCI
                            INNER JOIN gar_conferencia GC ON GC.id = GCI.Id_Conf
                        WHERE
                            GCI.Id_Conf = :Id_Conferencia
                            AND GCI.Refx = :Refx
                            AND (Quant_Conferida - Quant) != 0
                        UNION
                            SELECT
                                GSIL.Id,
                                GSI.Refx,
                                GSIL.Quant,
                                GSI.Id_Item_Nf,
                                GSIL.Id_Motivo
                            FROM
                                gar_conferencia GC
                                INNER JOIN gar_solicitacao_item GSI ON GSI.id_solicitacao = GC.id_solicitacao
                                INNER JOIN gar_solicitacao_item_laudo GSIL ON GSIL.id_item = GSI.id
                            WHERE
                                GC.Id = :Id_Conferencia
                                AND GSI.Refx = :Refx
                    ) L INNER JOIN gar_motivo_laudo GML ON L.Id_Motivo = GML.id
                    ";
                    retorno = conn.Query<GarSolicitacaoItemLaudo>(sQuery, new { Id_Conferencia, Refx }).ToList();
                }
                conn.Close();
            }

            return retorno;
        }

        public void FinalizarConferenciaEntrada(GarConferencia item)
        {
            using (var conn = new OracleConnection(Entities.Database.Connection.ConnectionString))
            {
                string sQuery = "";
                conn.Open();
                if (conn.State == ConnectionState.Open)
                {
                    //Grava itens a mais e a menos na tebela de excesso
                    sQuery = @"
                    INSERT INTO gar_conferencia_excesso (Id_Conf, Id_Usr, Dt_Conf, Id_Item, Quant_Conferida, Refx, Id_Tipo)
                    SELECT
                        :Id AS Id_Conf,
                        :Id_Usr AS Id_Usr,
                        SYSDATE AS Dt_Conf,
                        Id_Item,
                        Quant AS Quant_Conferida,
                        Refx,
                        (CASE WHEN Id_Motivo = 3 THEN 35 ELSE 34 END) AS Id_Tipo
                    FROM(
                        SELECT
                            GCI.Refx,
                            (Quant_Conferida - Quant) AS Quant,
                            0 AS Id_Item,
                            CASE WHEN  (Quant_Conferida - Quant) < 0 THEN 3 ELSE 2 END AS Id_Motivo
                        FROM
                            gar_conferencia_item GCI
                            INNER JOIN gar_conferencia GC ON GC.id = GCI.Id_Conf
                        WHERE
                            GCI.Id_Conf = :Id
                            AND (Quant_Conferida - Quant) != 0
                    )
                    ";
                    conn.Query<GarConferencia>(sQuery, new { item.Id, item.Id_Usr });

                    //Tipo Solicitação => 17 = Devolução | 18 = Garantia
                    if (item.Id_Tipo_Solicitacao == 18)
                    {
                        //Se for Garantia Abastece Estoque Laudo e Garantia
                        sQuery = @"
                        INSERT INTO gar_movimentacao ( Id_Item, Valor, Id_Tipo_Estoque, Id_Doc_Laudo, Id_Tipo, Id_Tipo_Movimentacao, Quant)
                        SELECT * FROM(
                            SELECT
                                GSI.Id AS Id_Item,
                                GSI.Valor,
                                15 Id_Tipo_Estoque,
                                0 AS Id_Doc_Laudo,
                                13 AS Id_Tipo,
                                27 AS Id_Tipo_Movimentacao,
                                CASE WHEN  GSIL.Id IS NULL THEN  GSI.Quant ELSE ( GSI.Quant - GSIL.Quant) END AS Quant
                            FROM
                                gar_conferencia GC
                                INNER JOIN gar_solicitacao_item GSI ON GSI.id_solicitacao = GC.id_solicitacao
                                LEFT JOIN gar_solicitacao_item_laudo GSIL ON GSIL.id_item = GSI.id
                            WHERE
                                GC.Id = :Id
                            UNION ALL
                            SELECT
                                GSI.Id AS Id_Item,
                                GSI.Valor,
                                14 Id_Tipo_Estoque,
                                GSIL.Id AS Id_Doc_Laudo,
                                13 AS Id_Tipo,
                                27 AS Id_Tipo_Movimentacao,
                                GSIL.Quant
                            FROM
                                gar_conferencia GC
                                INNER JOIN gar_solicitacao_item GSI ON GSI.id_solicitacao = GC.id_solicitacao
                                INNER JOIN gar_solicitacao_item_laudo GSIL ON GSIL.id_item = GSI.id
                            WHERE
                                GC.Id = :Id
                        ) WHERE Quant != 0
                        ";
                        conn.Query<GarConferencia>(sQuery, new { item.Id });
                    }

                    //Fecha a conferencia
                    sQuery = @"UPDATE gar_conferencia SET Ativo = 0 WHERE Id = :Id";
                    conn.Query<GarConferencia>(sQuery, new { item.Id });

                    //Fecha a solicitação
                    sQuery = @"UPDATE gar_solicitacao SET Id_Status = 24 WHERE Id = :Id_Solicitacao";
                    conn.Query<GarConferencia>(sQuery, new { item.Id_Solicitacao });
                }
                conn.Close();
            }
        }

        public void CriarLaudo(GarSolicitacaoItemLaudo item)
        {
            using (var conn = new OracleConnection(Entities.Database.Connection.ConnectionString))
            {
                string sQuery = "";
                conn.Open();
                if (conn.State == ConnectionState.Open)
                {
                    sQuery = @"
                    INSERT INTO gar_solicitacao_item_laudo
	                ( Id_Item, Id_Motivo, Quant, Id_Tipo_Retorno )
                    VALUES
	                ( :Id_Item, :Id_Motivo, :Quant, :Id_Tipo_Retorno )";
                    conn.Query<GarConferenciaHist>(sQuery, new
                    {
                        item.Id_Item,
                        item.Id_Motivo,
                        item.Quant,
                        item.Id_Tipo_Retorno,
                    });
                }
                conn.Close();
            }
        }

        public void ExcluirLaudo(long Id)
        {
            using (var conn = new OracleConnection(Entities.Database.Connection.ConnectionString))
            {
                string sQuery = "";
                conn.Open();
                if (conn.State == ConnectionState.Open)
                {
                    sQuery = @"DELETE  FROM gar_solicitacao_item_laudo WHERE Id = :Id";
                    conn.Query<GarConferenciaItem>(sQuery, new
                    {
                        Id
                    });
                }
                conn.Close();
            }
        }

        public List<GarSolicitacaoItemLaudo> ListarLaudoItem(long Id_Conferencia, string Refx)
        {
            List<GarSolicitacaoItemLaudo> retorno = new List<GarSolicitacaoItemLaudo>();

            using (var conn = new OracleConnection(Entities.Database.Connection.ConnectionString))
            {
                conn.Open();
                if (conn.State == ConnectionState.Open)
                {
                    string sQuery = @"
                    SELECT
                        GSI.Id_Item_Nf,
                        GSI.Quant,
                        GSI.Id
                    FROM
                        gar_conferencia GC
                        INNER JOIN gar_solicitacao_item GSI ON GSI.id_solicitacao = GC.id_solicitacao
                    WHERE
                        GSI.Refx = :Refx
                        AND GC.Id = :Id_Conferencia
                    ";
                    retorno = conn.Query<GarSolicitacaoItemLaudo>(sQuery, new { Refx, Id_Conferencia }).ToList();
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
                        GC.Id_Solicitacao,
                        GS.Id_Tipo AS Id_Tipo_Solicitacao
                    FROM
                        gar_conferencia GC
                        INNER JOIN geral_tipo GT ON GT.id = GC.Id_Tipo_Conf AND GT.tabela = 'GAR_CONFERENCIA' AND GT.coluna = 'ID_TIPO_CONF'
                        LEFT JOIN gar_solicitacao GS ON GS.id = GC.Id_Solicitacao
                    WHERE
                        GC.Id = :Id_Conferencia
                    ";
                    retorno = conn.Query<GarConferencia>(sQuery, new { Id_Conferencia }).SingleOrDefault();
                }
                conn.Close();
            }
            return retorno;
        }

        public GarConferenciaItem SelecionaConferenciaItem(long Id_Conferencia, string Refx)
        {
            GarConferenciaItem retorno = new GarConferenciaItem();

            using (var conn = new OracleConnection(Entities.Database.Connection.ConnectionString))
            {
                conn.Open();
                if (conn.State == ConnectionState.Open)
                {
                    string sQuery = @"
                    SELECT
                        Id,
                        Quant,
                        Quant_Conferida,
                        Id_Item,
                        GREATEST(Quant_Conferida,Quant) - (Quant_Laudo + Quant_Laudo_Automatico) AS Quant_Max
                    FROM(
                        SELECT
                            GCI.Id,
                            GCI.Quant,
                            GCI.Quant_Conferida,
                            (SELECT MAX(Id) FROM gar_solicitacao_item GSI WHERE GSI.id_solicitacao = GC.id_solicitacao AND GSI.refx = GCI.refx ) AS Id_Item,
                            CASE WHEN  (Quant_Conferida - Quant) < 0 THEN (Quant_Conferida - Quant) * -1 ELSE (Quant_Conferida - Quant) END AS Quant_Laudo_Automatico,
                            NVL((SELECT SUM(GSIL.quant) FROM gar_solicitacao_item_laudo GSIL INNER JOIN gar_solicitacao_item GSI ON GSI.id = GSIL.id_item
                            WHERE GSI.id_solicitacao = GC.id_solicitacao AND GSI.refx = GCI.refx  ),0) AS Quant_Laudo
                        FROM
                            gar_conferencia_item GCI
                            INNER JOIN gar_conferencia GC ON GC.id = GCI.Id_Conf
                        WHERE
                            GCI.Id_Conf = :Id_Conferencia
                            AND GCI.Refx = :Refx
                    )

                    ";
                    retorno = conn.Query<GarConferenciaItem>(sQuery, new { Id_Conferencia, Refx }).SingleOrDefault();
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
                        GCH.Id,
                        GCH.Id_Conf,
                        GCH.Refx,
                        U.""UserName"" AS Usr,
                        GCH.Volume,
                        GCH.Dt_Conf,
                        GCH.Quant_Conferida
                    FROM
                        gar_conferencia_hist GCH
                        INNER JOIN ""AspNetUsers"" U ON U.""Id"" =  GCH.id_usr
                    WHERE
                        GCH.Id_Conf = :Id_Conferencia
                    ORDER BY
                        GCH.Id DESC
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
                        GC.Id,
                        GC.Id_Conf,
                        GC.Refx,
                        GC.Divergencia,
                        GC.Quant,
                        TP.descrprod AS Descricao,
                        GC.Tem_No_Excesso,
                        GC.Quant_Conferida
                    FROM(
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
                    ) GC INNER JOIN tgfpro@sankhya TP ON TP.ad_refx = GC.refx
                    ";
                    retorno = conn.Query<GarConferenciaItem>(sQuery, new { Id_Conferencia }).ToList();
                }
                conn.Close();
            }

            return retorno;
        }
    }
}