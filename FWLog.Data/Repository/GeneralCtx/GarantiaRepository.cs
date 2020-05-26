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

        public List<GarSolicitacao> ListarSolicitacao(DataTableFilter<GarantiaSolicitacaoFilter> filter, out int totalRecordsFiltered, out int totalRecords)
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

                    if (filter.CustomFilter.Id_Empresa.HasValue)
                        sQuery.AppendFormat(@" AND GS.Id_Empresa = {0}", filter.CustomFilter.Id_Empresa);

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

        public List<GarRemessa> ListarRemessa(DataTableFilter<GarantiaRemessaFilter> filter, out int totalRecordsFiltered, out int totalRecords)
        {
            List<GarRemessa> retorno = new List<GarRemessa>();
            var sQuery = new StringBuilder();

            using (var conn = new OracleConnection(Entities.Database.Connection.ConnectionString))
            {
                conn.Open();
                if (conn.State == ConnectionState.Open)
                {
                    sQuery.AppendFormat(@"
                    SELECT
                        COUNT(*) OVER() AS Qtde,
                        GR.Cod_Fornecedor,
                        GR.Id,
                        GR.Dt_Criacao,
                        GR.Id_Tipo,
                        GT1.descricao AS Tipo,
                        (SELECT ""Sigla"" FROM ""Empresa"" WHERE ""IdEmpresa"" = GR.Id_Empresa ) AS empresa,
                        GR.Id_Status,
                        GT2.descricao AS Status
                    FROM
                        gar_remessa GR
                        LEFT JOIN geral_tipo GT1 ON GT1.Id = GR.Id_Tipo
                        LEFT JOIN geral_tipo GT2 ON GT2.Id = GR.Id_Status
                    WHERE
                        GR.id != 0
                    ");

                    //TODO VERIFICAR FILTROS
                    if (filter.CustomFilter.Id.HasValue)
                        sQuery.AppendFormat(@" AND GR.id = {0}", filter.CustomFilter.Id);

                    if (filter.CustomFilter.Id_Empresa.HasValue)
                        sQuery.AppendFormat(@" AND GR.Id_Empresa = {0}", filter.CustomFilter.Id_Empresa);

                    if (filter.CustomFilter.Id_Status.HasValue)
                        sQuery.AppendFormat(@" AND GT2.id = {0}", filter.CustomFilter.Id_Status);

                    if (!string.IsNullOrEmpty(filter.CustomFilter.Cod_Fornecedor))
                        sQuery.AppendFormat(@" AND GR.Cod_Fornecedor LIKE '%{0}%' ", filter.CustomFilter.Cod_Fornecedor);

                    if (filter.CustomFilter.Data_Inicial.HasValue && filter.CustomFilter.Data_Final.HasValue)
                        sQuery.AppendFormat(@" AND GR.Dt_Criacao BETWEEN TO_DATE('{0}','DD/MM/YYYY') AND TO_DATE('{1}','DD/MM/YYYY') ", String.Format("{0:dd/MM/yyyy}", filter.CustomFilter.Data_Inicial), String.Format("{0:dd/MM/yyyy}", filter.CustomFilter.Data_Final));

                    sQuery.AppendFormat(" ORDER BY {0} {1}", filter.OrderByColumn, filter.OrderByDirection);

                    sQuery.AppendFormat(" OFFSET {0} ROWS FETCH NEXT {1} ROWS ONLY", filter.Start, filter.Length);

                    retorno = conn.Query<GarRemessa>(sQuery.ToString(), new { }).ToList();
                }
                conn.Close();
            }

            totalRecordsFiltered = retorno.Count();
            totalRecords = totalRecordsFiltered > 0 ? retorno.FirstOrDefault().Qtde : 0;

            return retorno;
        }

        public List<GarConferenciaItem> ListarRemessaSolicitacao(long Id_Conferencia, string Refx)
        {
            List<GarConferenciaItem> retorno = new List<GarConferenciaItem>();

            using (var conn = new OracleConnection(Entities.Database.Connection.ConnectionString))
            {
                conn.Open();
                if (conn.State == ConnectionState.Open)
                {
                    string sQuery = @"
                    SELECT
                        DISTINCT GCI.id_solicitacao
                    FROM
                        gar_conferencia GC
                        INNER JOIN gar_conferencia_item GCI ON GC.id = GCI.id_conf
                    WHERE
                        GC.Id = :Id_Conferencia
                        AND GCI.Refx = :Refx
                    ";
                    retorno = conn.Query<GarConferenciaItem>(sQuery, new { Id_Conferencia, Refx }).ToList();
                }
                conn.Close();
            }

            return retorno;
        }

        public List<GarConferenciaItem> ListarRemessaRefx(long Id_Conferencia)
        {
            List<GarConferenciaItem> retorno = new List<GarConferenciaItem>();

            using (var conn = new OracleConnection(Entities.Database.Connection.ConnectionString))
            {
                conn.Open();
                if (conn.State == ConnectionState.Open)
                {
                    string sQuery = @"
                    SELECT
                        DISTINCT GCI.refx
                    FROM
                        gar_conferencia GC
                        INNER JOIN gar_conferencia_item GCI ON GC.id = GCI.id_conf
                    WHERE
                        GC.Id = :Id_Conferencia
                    ";
                    retorno = conn.Query<GarConferenciaItem>(sQuery, new { Id_Conferencia }).ToList();
                }
                conn.Close();
            }

            return retorno;
        }

        public List<GarConferenciaItem> ListarRemessaItem(long Id_Remessa)
        {
            List<GarConferenciaItem> retorno = new List<GarConferenciaItem>();

            using (var conn = new OracleConnection(Entities.Database.Connection.ConnectionString))
            {
                conn.Open();
                if (conn.State == ConnectionState.Open)
                {
                    string sQuery = @"
                    SELECT
                        GCI.Refx,
                        TP.descrprod AS Descricao,
                        SUM(GCI.Quant) AS Quant
                    FROM
                        gar_conferencia GC
                        INNER JOIN gar_conferencia_item GCI ON GCI.id_conf = GC.id
                        LEFT JOIN tgfpro@sankhya TP ON TP.ad_refx = GCI.refx
                    WHERE
                        GC.Id_Remessa = :Id_Remessa
                    GROUP BY
                        GCI.Refx,
                        TP.descrprod
                    ";
                    retorno = conn.Query<GarConferenciaItem>(sQuery, new { Id_Remessa }).ToList();
                }
                conn.Close();
            }

            return retorno;
        }

        public List<GarConferenciaItem> ListarRemessaItemDetalhado(long Id_Remessa)
        {
            List<GarConferenciaItem> retorno = new List<GarConferenciaItem>();

            using (var conn = new OracleConnection(Entities.Database.Connection.ConnectionString))
            {
                conn.Open();
                if (conn.State == ConnectionState.Open)
                {
                    string sQuery = @"
                    SELECT
                        GCI.Refx,
                        GCI.Id_Solicitacao,
                        TP.descrprod AS Descricao,
                        SUM(GCI.Quant) AS Quant
                    FROM
                        gar_conferencia GC
                        INNER JOIN gar_conferencia_item GCI ON GCI.id_conf = GC.id
                        LEFT JOIN tgfpro@sankhya TP ON TP.ad_refx = GCI.refx
                    WHERE
                        GC.Id_Remessa = :Id_Remessa
                    GROUP BY
                        GCI.Refx,
                        GCI.Id_Solicitacao,
                        TP.descrprod
                    ";
                    retorno = conn.Query<GarConferenciaItem>(sQuery, new { Id_Remessa }).ToList();
                }
                conn.Close();
            }

            return retorno;
        }

        public DmlStatus CriarRemessa(GarRemessa item)
        {
            string sQuery = "";
            var Id_Remessa = 0;

            using (var conn = new OracleConnection(Entities.Database.Connection.ConnectionString))
            {
                conn.Open();
                if (conn.State == ConnectionState.Open)
                {
                    string codFornPai = "";
                    sQuery = @"SELECT cod_forn_pai FROM gar_forn_grupo WHERE cod_forn_filho = :Cod_Fornecedor";
                    codFornPai = conn.Query<string>(sQuery, new { item.Cod_Fornecedor }).SingleOrDefault();

                    if (!string.IsNullOrEmpty(codFornPai))
                        return new DmlStatus { Sucesso = false, Mensagem = string.Format("Esse é um fornecedor filho, busque pelo fornecedor pai <strong>{0}</strong>.", codFornPai) };

                    //VERIFICA SE TEM REMESSA ATIVA PARA ESSE FORNECEDOR
                    bool temRemessa = false;
                    sQuery = @"SELECT COUNT(*) FROM gar_remessa WHERE Cod_Fornecedor = :Cod_Fornecedor AND id_status != 39 ";
                    temRemessa = conn.Query<long>(sQuery, new { item.Cod_Fornecedor }).SingleOrDefault() > 0;

                    if (temRemessa)
                        return new DmlStatus { Sucesso = false, Mensagem = string.Format("Já existe uma remessa ativa para o fornecedor <strong>{0}</strong>.", item.Cod_Fornecedor) };

                    //VERIFICA SE TEM ALGUNS ITEM PARA A REMESSA
                    //NÃO PEGA ITENS QUE ESTÃO EM CONFERENCIA ABERTA E QUE JA FORAM TRATADOS EM REMESSAS ANTERIORES
                    bool temItens = false;
                    sQuery = @"
                    SELECT COUNT(*) FROM(
                       WITH cte_itens AS
                        (
                            SELECT
                               GSI.refx,
                               GSI.id_solicitacao,
                               SUM(GM.quant - NVL(GRC.Quant_Enviado,0) ) AS quant
                            FROM
                                gar_movimentacao GM
                                INNER JOIN gar_solicitacao_item GSI ON GSI.id = GM.id_item
                                INNER JOIN gar_solicitacao GS ON GS.id = GSI.id_solicitacao
                                LEFT JOIN gar_remessa_controle GRC ON GRC.id_movimentacao = GM.id
                            WHERE
                                (
                                    GSI.cod_fornecedor = :Cod_Fornecedor
                                    OR GSI.cod_fornecedor IN(SELECT cod_forn_filho FROM gar_forn_grupo WHERE cod_forn_pai = :Cod_Fornecedor)
                                )
                                AND GM.id_tipo_estoque = 15
                                AND GM.id_tipo_movimentacao = 27
                                AND GS.id_empresa = :Id_Empresa
                            GROUP BY
                                GSI.refx,
                                GSI.id_solicitacao
                            HAVING (SELECT COUNT(*) FROM gar_conferencia_item GCI INNER JOIN gar_conferencia GC ON GCI.id_conf = GC.id AND GC.ativo = 1
                                WHERE GCI.id_solicitacao = GSI.id_solicitacao AND GCI.refx = GSI.refx ) = 0
                        )
                        SELECT
                            CI.refx, CI.id_solicitacao, CI.quant
                        FROM
                            cte_itens CI
                        WHERE
                            CI.quant > 0
                    )";
                    temItens = conn.Query<int>(sQuery, new { item.Cod_Fornecedor, item.Id_Empresa }).SingleOrDefault() > 0;

                    if (!temItens)
                        return new DmlStatus { Sucesso = false, Mensagem = string.Format("Não foram encontrados itens para esse fornecedor.") };

                    //CRIA REMESSA
                    var param = new DynamicParameters();
                    param.Add(name: "Id_Empresa", value: item.Id_Empresa, direction: ParameterDirection.Input);
                    param.Add(name: "Cod_Fornecedor", value: item.Cod_Fornecedor, direction: ParameterDirection.Input);
                    param.Add(name: "Id_Tipo", value: item.Id_Tipo, direction: ParameterDirection.Input);
                    param.Add(name: "Id_Status", value: item.Id_Status, direction: ParameterDirection.Input);
                    param.Add(name: "Id_Usr", value: item.Id_Usr, direction: ParameterDirection.Input);
                    param.Add(name: "Id", dbType: DbType.Int32, direction: ParameterDirection.Output);
                    conn.Execute(@"INSERT INTO gar_remessa (Id_Empresa, Cod_Fornecedor, Id_Tipo, Id_Status, Id_Usr, Dt_Criacao)
                    VALUES (:Id_Empresa, :Cod_Fornecedor, :Id_Tipo, :Id_Status, :Id_Usr, SYSDATE) returning Id into :Id", param);

                    Id_Remessa = param.Get<int>("Id");
                }
                conn.Close();
            }
            return new DmlStatus { Sucesso = true, Mensagem = "Remessa criada com sucesso.", Id = Id_Remessa };
        }

        public GarRemessa SelecionaRemessa(long Id_Remessa)
        {
            GarRemessa retorno = new GarRemessa();

            using (var conn = new OracleConnection(Entities.Database.Connection.ConnectionString))
            {
                conn.Open();
                if (conn.State == ConnectionState.Open)
                {
                    string sQuery = @"
                    SELECT
                        GS.Id,
                        (SELECT ""Sigla"" FROM ""Empresa"" WHERE ""IdEmpresa"" = GS.Id_Empresa ) AS empresa,
                        GS.Dt_Criacao,
                        GT1.descricao AS Tipo,
                        GS.Cod_Fornecedor
                    FROM
                        gar_remessa GS
                        LEFT JOIN geral_tipo GT1 ON GT1.Id = GS.Id_Tipo
                    WHERE
                        GS.Id = :Id_Remessa
                    ";
                    retorno = conn.Query<GarRemessa>(sQuery, new { Id_Remessa }).SingleOrDefault();
                }
                conn.Close();
            }
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

        public DmlStatus ImportarSolicitacaoNfCartaManual(GarSolicitacao item)
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
                        return new DmlStatus { Sucesso = false, Mensagem = "A solicitação já foi importada." };

                    //VERIFICA SE EMPRESA FAZ GARANTIA
                    sQuery = @"SELECT nvl(ad_fazgarantia,'N') FROM tgfemp@sankhya WHERE codemp = (SELECT codemp FROM tsiemp@sankhya WHERE ad_filial = (SELECT ""Sigla"" FROM ""Empresa"" WHERE ""IdEmpresa""  = :Id_Empresa))";
                    bool empresaFazGarantia = conn.Query<string>(sQuery, new { item.Id_Empresa }).SingleOrDefault() == "S";

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
                        return new DmlStatus { Sucesso = false, Mensagem = "Erro ao carregar solicitação." };

                    if (solicitacao.Id_Tipo == 18 && !empresaFazGarantia)
                        return new DmlStatus { Sucesso = false, Mensagem = "Essa empresa não faz garantia." };

                    //VERIFICA SE A EMPRESA TEM PARAMETRO DE GARANTIA
                    sQuery = @"SELECT E.""IdEmpresa"" FROM tgfemp@sankhya TE INNER JOIN ""Empresa"" E ON E.""CodigoIntegracao"" =  TE.ad_codempgarantia
                    WHERE codemp = (SELECT codemp FROM tsiemp @sankhya WHERE ad_filial = :Filial";
                    var dadosEmp = conn.Query<Empresa>(sQuery, new { solicitacao.Filial }).SingleOrDefault();

                    if (solicitacao.Id_Tipo == 18 && dadosEmp == null)
                        return new DmlStatus { Sucesso = false, Mensagem = string.Format("Garantia é da Filial '{0}', esta filial não tem parametro de garantia, favor parametrizar no cadastro de empresa do Sankhya.", solicitacao.Filial) };

                    if (solicitacao.Id_Tipo == 18 && dadosEmp.IdEmpresa != item.Id_Empresa)
                        return new DmlStatus { Sucesso = false, Mensagem = string.Format("Empresa Não Autorizada a Fazer Garantia, a Filial '{0}' Tem que fazer garantia pela empresa '{1}'.", solicitacao.Filial, dadosEmp.Sigla) };

                    long Id_DevGar = solicitacao.Id_Sav;

                    //ATUALIZA CODIGO RASTREIO DO SAV
                    sQuery = @"UPDATE fdv_devgar@furacaophp SET correios = :Codigo_Postagem WHERE Id = :Id_DevGar";
                    conn.Query<GarConferencia>(sQuery, new { item.Codigo_Postagem, Id_DevGar });

                    var param = new DynamicParameters();
                    param.Add(name: "Filial", value: solicitacao.Filial, direction: ParameterDirection.Input);
                    param.Add(name: "Id_Empresa", value: solicitacao.Id_Empresa, direction: ParameterDirection.Input);
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

                    conn.Execute(@"INSERT INTO gar_solicitacao (Filial, Id_Filial, Id_Empresa, Dt_Criacao, Id_Tipo, Cli_Cnpj,Id_Status,Legenda,Id_Usr,Id_Sav,Nota_Fiscal,Serie,Id_Tipo_Doc)
                            VALUES ( :Filial, (SELECT ""IdEmpresa"" FROM ""Empresa"" WHERE ""Sigla"" = :Filial ), :Dt_Criacao, :Id_Tipo, :Cli_Cnpj,:Id_Status,:Legenda,:Id_Usr,:Id_Sav,:Nota_Fiscal,:Serie,:Id_Tipo_Doc )
                            returning Id into :Id", param);

                    Id_Solicitacao = param.Get<int>("Id");

                    if (Id_Solicitacao != 0)
                    {
                        //ITENS DA NF
                        sQuery = @"
                            INSERT INTO gar_solicitacao_item (id_solicitacao, id_item_nf, refx, cod_fornecedor,quant,valor)
                            SELECT
                                :Id_Solicitacao AS id_solicitacao,
                                rownum AS id_item_nf,
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

            return new DmlStatus { Sucesso = true, Mensagem = "Solicitação importada com sucesso.", Id = Id_Solicitacao };
        }

        public DmlStatus ImportarSolicitacaoNfEletronicaPedido(GarSolicitacao item)
        {
            string sQuery = "";
            bool podeImportar = false;
            long Id_Solicitacao = 0;

            using (var conn = new OracleConnection(Entities.Database.Connection.ConnectionString))
            {
                conn.Open();
                if (conn.State == ConnectionState.Open)
                {
                    var Nunota = 0;

                    //VERIFICA SE JA FOI IMPORTADO
                    if (item.Id_Tipo_Doc == 30)
                    {
                        sQuery = @"SELECT nunota FROM tgfCab@Sankhya WHERE ChaveNfe = :Chave_Acesso";
                        Nunota = conn.Query<int>(sQuery, new { item.Chave_Acesso }).SingleOrDefault();

                        if (Nunota == 0)
                            return new DmlStatus { Sucesso = false, Mensagem = "Nota não encontrada no Sankya." };

                        sQuery = @"SELECT COUNT(*) FROM gar_solicitacao WHERE Nota_Fiscal = SUBSTR(:Chave_Acesso,26,9) AND Cli_Cnpj = SUBSTR(:Chave_Acesso,7,14) AND Serie = SUBSTR(:Chave_Acesso,23,3)";
                        podeImportar = conn.Query<int>(sQuery, new { item.Chave_Acesso }).SingleOrDefault() == 0;
                    }
                    else if (item.Id_Tipo_Doc == 21)
                    {
                        sQuery = @"SELECT nunota FROM tgfCab@Sankhya WHERE nunota = :Id_Sav";
                        Nunota = conn.Query<int>(sQuery, new { item.Id_Sav }).SingleOrDefault();

                        if (Nunota == 0)
                            return new DmlStatus { Sucesso = false, Mensagem = "Nota não encontrada no Sankya." };

                        sQuery = @"SELECT COUNT(*) FROM gar_solicitacao WHERE Id_Sav = :Id_Sav ";
                        podeImportar = conn.Query<int>(sQuery, new { item.Id_Sav }).SingleOrDefault() == 0;
                    }

                    if (!podeImportar)
                        return new DmlStatus { Sucesso = false, Mensagem = "A solicitação já foi importada." };

                    //VERIFICA SE EMPRESA FAZ GARANTIA
                    sQuery = @"SELECT nvl(ad_fazgarantia,'N') FROM tgfemp@sankhya WHERE codemp = (SELECT codemp FROM tsiemp@sankhya WHERE ad_filial = (SELECT ""Sigla"" FROM ""Empresa"" WHERE ""IdEmpresa""  = :Id_Empresa))";
                    bool empresaFazGarantia = conn.Query<string>(sQuery, new { item.Id_Empresa }).SingleOrDefault() == "S";

                    //VERIFICA SE TEM NOTA SANKYA
                    var solicitacao = new GarSolicitacao();

                    if (item.Id_Tipo_Doc == 30)
                    {
                        //NF ELETRONICA
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
                            30 AS id_tipo_doc
                        FROM
                            fdv_devgar@furacaophp
                        WHERE
                            cnpj = (SELECT cgc_cpf FROM tgfpar@sankhya WHERE codparc = (SELECT CODPARC  FROM tgfCab@Sankhya WHERE ChaveNfe = :Chave_Acesso))
                            AND nota_fiscal = SUBSTR(:Chave_Acesso,26,9)
                            AND serie = SUBSTR(:Chave_Acesso,23,3)
                            AND finalizado NOT IN(0,5)
                        ";
                        //tirar pra funcionar AND finalizado NOT IN(0,5)
                        solicitacao = conn.Query<GarSolicitacao>(sQuery, new { item.Id_Usr, item.Chave_Acesso }).SingleOrDefault();
                    }
                    else if (item.Id_Tipo_Doc == 21)
                    {
                        //PEDIDO
                        sQuery = @"
                        SELECT
                        (SELECT ad_filial FROM tsiemp@sankhya s WHERE  s.codemp = n.codemp) AS filial,
                            SYSDATE AS dt_criacao,
                            17 AS id_tipo,
                            (SELECT cgc_cpf FROM tgfpar@sankhya p WHERE n.codparc = p.codparc) AS cli_cnpj,
                            22 AS id_status,
                            :Id_Usr AS id_usr,
                            3 AS legenda,
                            0 AS id_sav,
                            lpad(nunota,9,'0') AS Nota_fiscal,
                            ' ' as serie,
                            21 AS id_tipo_doc
                        FROM
                            tgfCab@Sankhya n
                        WHERE
                            nunota = :Nunota
                            AND codtipoper IN (SELECT CODTIPOPER  FROM tgftop@sankhya WHERE tipmov = 'P')
                         ";
                        solicitacao = conn.Query<GarSolicitacao>(sQuery, new { item.Id_Usr, Nunota }).SingleOrDefault();
                    }

                    if (solicitacao == null)
                        return new DmlStatus { Sucesso = false, Mensagem = "Erro ao carregar solicitação." };

                    if (solicitacao.Id_Tipo == 18 && !empresaFazGarantia)
                        return new DmlStatus { Sucesso = false, Mensagem = "Essa empresa não faz garantia." };

                    //VERIFICA SE A EMPRESA TEM PARAMETRO DE GARANTIA
                    sQuery = @"SELECT E.""IdEmpresa"" FROM tgfemp@sankhya TE INNER JOIN ""Empresa"" E ON E.""CodigoIntegracao"" =  TE.ad_codempgarantia
                    WHERE codemp = (SELECT codemp FROM tsiemp @sankhya WHERE ad_filial = :Filial";
                    var dadosEmp = conn.Query<Empresa>(sQuery, new { solicitacao.Filial }).SingleOrDefault();

                    if (solicitacao.Id_Tipo == 18 && dadosEmp == null)
                        return new DmlStatus { Sucesso = false, Mensagem = string.Format("Garantia é da Filial '{0}', esta filial não tem parametro de garantia, favor parametrizar no cadastro de empresa do Sankhya.", solicitacao.Filial) };

                    if (solicitacao.Id_Tipo == 18 && dadosEmp.IdEmpresa != item.Id_Empresa)
                        return new DmlStatus { Sucesso = false, Mensagem = string.Format("Empresa Não Autorizada a Fazer Garantia, a Filial '{0}' Tem que fazer garantia pela empresa '{1}'.", solicitacao.Filial, dadosEmp.Sigla) };

                    long Id_DevGar = solicitacao.Id_Sav;

                    //ATUALIZA CODIGO RASTREIO DO SAV
                    if (Id_DevGar != 0)
                    {
                        sQuery = @"UPDATE fdv_devgar@furacaophp SET correios = :Codigo_Postagem WHERE Id = :Id_DevGar";
                        conn.Query<GarConferencia>(sQuery, new { item.Codigo_Postagem, Id_DevGar });
                    }

                    var param = new DynamicParameters();
                    param.Add(name: "Filial", value: solicitacao.Filial, direction: ParameterDirection.Input);
                    param.Add(name: "Id_Empresa", value: solicitacao.Id_Empresa, direction: ParameterDirection.Input);
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

                    conn.Execute(@"INSERT INTO gar_solicitacao (Filial, Id_Filial, Id_Empresa, Dt_Criacao, Id_Tipo, Cli_Cnpj,Id_Status,Legenda,Id_Usr,Id_Sav,Nota_Fiscal,Serie,Id_Tipo_Doc)
                            VALUES ( :Filial, (SELECT ""IdEmpresa"" FROM ""Empresa"" WHERE ""Sigla"" = :Filial ), :Dt_Criacao, :Id_Tipo, :Cli_Cnpj,:Id_Status,:Legenda,:Id_Usr,:Id_Sav,:Nota_Fiscal,:Serie,:Id_Tipo_Doc )
                            returning Id into :Id", param);

                    Id_Solicitacao = param.Get<int>("Id");

                    if (Id_Solicitacao != 0)
                    {
                        //ITENS DA NF
                        sQuery = @"
                        INSERT INTO gar_solicitacao_item (id_solicitacao, id_item_nf, refx, cod_fornecedor,quant,valor)
                        SELECT
                            :Id_Solicitacao AS id_solicitacao,
                            sequencia AS id_item_nf,
                            (SELECT AD_REFX FROM tgfpro@sankhya p WHERE i.codprod = p.codprod) AS refx,
                            (SELECT codparcforn FROM tgfpro@sankhya p WHERE i.codprod = p.codprod) AS cod_fornecedor,
                            qtdneg AS quant,
                            vlrunit AS valor
                        FROM
                            tgfite@sankhya i
                        WHERE
                            i.nunota = :Nunota
                        ";
                        conn.Query<GarConferenciaHist>(sQuery, new
                        {
                            Id_Solicitacao,
                            Nunota
                        });
                    }
                }
                conn.Close();
            }

            return new DmlStatus { Sucesso = true, Mensagem = "Solicitação importada com sucesso.", Id = Id_Solicitacao };
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
                    SELECT GCI.Id_Conf, TP.descrprod AS Descricao, GCI.Refx, GCI.Id_Solicitacao, GCI.Quant, GCI.Tem_No_Excesso, (GCI.Quant_Laudo + GCI.Quant_Laudo_Automatico) AS Quant_Laudo  FROM(
                        SELECT
                            GCI.Id_Conf,
                            GCI.Refx,
                            GCI.Id_Solicitacao,
                            CASE WHEN  (Quant_Conferida - Quant) < 0 THEN (Quant_Conferida - Quant) * -1 ELSE (Quant_Conferida - Quant) END AS Quant_Laudo_Automatico,
                            NVL((SELECT SUM(GSIL.quant) FROM gar_solicitacao_item_laudo GSIL INNER JOIN gar_solicitacao_item GSI ON GSI.id = GSIL.id_item
                            WHERE GSI.id_solicitacao = GCI.id_solicitacao AND GSI.refx = GCI.refx  ),0) AS Quant_Laudo,
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
                            0 AS Id_Solicitacao,
                            Quant_Conferida AS Quant_Laudo_Automatico,
                            0 AS Quant_Laudo,
                            Quant_Conferida AS Quant,
                            1 AS Tem_No_Excesso
                        FROM
                            gar_conferencia_excesso
                        WHERE
                            Id_Conf = :Id_Conferencia
                    ) GCI INNER JOIN tgfpro@sankhya TP ON TP.ad_refx = GCI.refx

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
                            0 AS Id_Item_Nf,
                            (Quant_Conferida - Quant) AS Quant_Laudo,
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
                                GSI.Id_Item_Nf,
                                GSIL.Quant,
                                GSIL.Id_Motivo
                            FROM
                                gar_conferencia GC
                                INNER JOIN gar_conferencia_item GCI ON GCI.id_conf = GC.id
                                INNER JOIN gar_solicitacao_item GSI ON GSI.id_solicitacao = GCI.id_solicitacao AND GSI.refx = GCI.refx
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
                                12 AS Id_Tipo,
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

        public void FinalizarConferenciaRemessa(GarConferencia item)
        {
            using (var conn = new OracleConnection(Entities.Database.Connection.ConnectionString))
            {
                string sQuery = "";
                conn.Open();
                if (conn.State == ConnectionState.Open)
                {
                    //Se for Garantia Abastece Estoque Laudo e Garantia
                    sQuery = @"
                    INSERT INTO gar_movimentacao ( Id_Item, Valor, Id_Tipo_Estoque, Id_Doc_Laudo, Id_Tipo, Id_Tipo_Movimentacao, Quant)
                    WITH cte_laudos AS
                    (
                        SELECT
                            GSI.Refx,
                            GSI.id,
                            GSIL.Quant
                        FROM
                            gar_conferencia_item GCI
                            INNER JOIN gar_solicitacao_item GSI ON GSI.id_solicitacao = GCI.id_solicitacao AND GSI.refx = GCI.refx
                            INNER JOIN gar_solicitacao_item_laudo GSIL ON GSIL.id_item = GSI.id
                        WHERE
                            GCI.id_conf = :Id
                    ),
                    cte_conferidos AS
                    (
                        SELECT refx, id, SUM(quant) AS quant FROM(
                            SELECT
                                    GCI.refx, GSI.id, GSI.Quant AS quant
                            FROM
                                gar_conferencia_item GCI
                                INNER JOIN gar_solicitacao_item GSI ON GSI.id_solicitacao = GCI.id_solicitacao AND GSI.refx = GCI.refx
                            WHERE
                                GCI.id_conf = :Id
                                AND GCI.Quant_Conferida > 0
                                AND (GCI.Quant - GCI.Quant_Conferida) >= 0
                            UNION ALL
                            SELECT
                                    refx, id, quant * -1 AS quant
                            FROM
                                cte_laudos
                        ) GROUP BY refx, id HAVING SUM(quant) > 0
                    ),
                    cte_itens AS
                    (
                        SELECT
                            refx,
                            id,
                            Quant,
                            15 Id_Tipo_Estoque,
                            16 AS Id_Tipo_Movimentacao
                        FROM
                            cte_conferidos
                        UNION
                        SELECT
                            refx,
                            id,
                            Quant,
                            14 Id_Tipo_Estoque,
                            27 AS Id_Tipo_Movimentacao
                        FROM
                            cte_laudos
                    )
                    SELECT
                        CI.id, GSI.valor, CI.Id_Tipo_Estoque, 0 AS Id_Doc_Laudo, 11 AS Id_Tipo, CI.Id_Tipo_Movimentacao, CI.Quant
                    FROM
                        cte_itens CI
                        INNER JOIN gar_solicitacao_item GSI ON GSI.id = CI.id
                    ";
                    conn.Query<GarConferencia>(sQuery, new { item.Id });

                    //gravar tudo que foi conferido na tabela gar_remessa_controle
                    sQuery = @"
                    INSERT INTO gar_remessa_controle (id_remessa, id_movimentacao, quant_enviado)
                    SELECT
                         :Id_Remessa, GM.id AS id_movimentacao, GSI.Quant AS quant_enviado
                    FROM
                        gar_conferencia_item GCI
                        INNER JOIN gar_solicitacao_item GSI ON GSI.id_solicitacao = GCI.id_solicitacao AND GSI.refx = GCI.refx
                        INNER JOIN gar_movimentacao GM  ON GSI.id = GM.id_item   AND GM.id_tipo_estoque = 15 AND GM.id_tipo_movimentacao = 27
                    WHERE
                        GCI.id_conf = :Id
                        AND GCI.Quant_Conferida > 0
                        AND (GCI.Quant - GCI.Quant_Conferida) >= 0
                    ";
                    conn.Query<GarConferencia>(sQuery, new { item.Id_Remessa, item.Id });

                    //Fecha a conferencia
                    sQuery = @"UPDATE gar_conferencia SET Ativo = 0 WHERE Id = :Id";
                    conn.Query<GarConferencia>(sQuery, new { item.Id });

                    //Fecha a remessa
                    sQuery = @"UPDATE gar_remessa SET Id_Status = 39 WHERE Id = :Id_Remessa";
                    conn.Query<GarConferencia>(sQuery, new { item.Id_Remessa });
                }
                conn.Close();
            }
        }

        public void AtualizaStatusConferenciaRemessa(GarConferencia item)
        {
            //ATUALIZA O STATUS PARA AGUARDANDO A NF DO SANKYA
            using (var conn = new OracleConnection(Entities.Database.Connection.ConnectionString))
            {
                string sQuery = "";
                conn.Open();
                if (conn.State == ConnectionState.Open)
                {
                    //Fecha a remessa
                    sQuery = @"UPDATE gar_remessa SET Id_Status = 37 WHERE Id = :Id_Remessa";
                    conn.Query<GarConferencia>(sQuery, new { item.Id_Remessa });
                }
                conn.Close();
            }
        }

        public DmlStatus VerificarConferenciaRemessa(GarConferencia item)
        {
            string sQuery = "";

            using (var conn = new OracleConnection(Entities.Database.Connection.ConnectionString))
            {
                conn.Open();
                if (conn.State == ConnectionState.Open)
                {
                    //VERIFICA SE TEM ITENS A MAIS
                    decimal qtdItensAmais = 0;
                    sQuery = @"
                    SELECT
                        COUNT(*) AS total
                    FROM
                        gar_conferencia_item
                    WHERE
                        Id_Conf = :Id
                        AND (Quant_Conferida - Quant) > 0";
                    qtdItensAmais = conn.Query<decimal>(sQuery, new { item.Id }).SingleOrDefault();

                    if (qtdItensAmais > 0)
                        return new DmlStatus { Sucesso = false, Mensagem = string.Format("Verifique as divergências, <strong>{0}</strong> itens a mais.", qtdItensAmais) };

                    bool aceitaParcial = false;
                    sQuery = @"
                    SELECT COUNT(*) FROM
                        gar_remessa GR
                        INNER JOIN gar_remessa_config GRC ON GRC.cod_fornecedor = GR.cod_fornecedor AND GRC.id_empresa = GR.id_empresa
                    WHERE GR.Id = :Id_Remessa AND GRC.total = 0";
                    aceitaParcial = conn.Query<decimal>(sQuery, new { item.Id_Remessa }).SingleOrDefault() > 0;

                    if (!aceitaParcial)
                    {
                        //FORNECEDOR TOTAL
                        //VERIFICAR SE TEM DIVERGENCIA (ITENS A MAIS OU A MENOS)
                        decimal qtdItensAmenos = 0;
                        sQuery = @"
                        SELECT
                            COUNT(*) AS total
                        FROM
                            gar_conferencia_item
                        WHERE
                            Id_Conf = :Id
                            AND (Quant_Conferida - Quant) < 0";
                        qtdItensAmenos = conn.Query<decimal>(sQuery, new { item.Id }).SingleOrDefault();

                        if (qtdItensAmenos > 0)
                            return new DmlStatus { Sucesso = false, Mensagem = string.Format("Fornecedor só aceita entrega total, verifique as divergências, <strong>{0}</strong> itens a menos.", qtdItensAmenos) };
                    }
                }
                conn.Close();
            }
            return new DmlStatus { Sucesso = true, Mensagem = "Ok, pode continuar.", Id = 0 };
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

        public bool ItemExiste(string Refx)
        {
            bool retorno = false;
            string sQuery = "";

            using (var conn = new OracleConnection(Entities.Database.Connection.ConnectionString))
            {
                conn.Open();
                if (conn.State == ConnectionState.Open)
                {
                    sQuery = @"SELECT COUNT(*) FROM tgfpro@sankhya WHERE ad_refx = :Refx";
                    retorno = conn.Query<int>(sQuery, new { Refx }).SingleOrDefault() > 0;
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
                            (SELECT MAX(Id) FROM gar_solicitacao_item GSI WHERE GSI.id_solicitacao = GCI.id_solicitacao AND GSI.refx = GCI.refx ) AS Id_Item,
                            CASE WHEN  (Quant_Conferida - Quant) < 0 THEN (Quant_Conferida - Quant) * -1 ELSE (Quant_Conferida - Quant) END AS Quant_Laudo_Automatico,
                            NVL((SELECT SUM(GSIL.quant) FROM gar_solicitacao_item_laudo GSIL INNER JOIN gar_solicitacao_item GSI ON GSI.id = GSIL.id_item
                            WHERE GSI.id_solicitacao = GCI.id_solicitacao AND GSI.refx = GCI.refx  ),0) AS Quant_Laudo
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

        public void AtualizarItemConferenciaRemessa(GarConferenciaItem item)
        {
            using (var conn = new OracleConnection(Entities.Database.Connection.ConnectionString))
            {
                string sQuery = "";
                conn.Open();
                if (conn.State == ConnectionState.Open)
                {
                    sQuery = @"
                    SELECT
                        (SELECT COUNT(*) FROM gar_conferencia_item WHERE id_conf = GC.id AND refx = :Refx AND Id_Solicitacao = :Id_Solicitacao ) AS tem_na_nota
                    FROM
                        gar_conferencia GC
                    WHERE
                        GC.id = :Id_Conf
                    ";
                    var retorno = conn.Query<GarConferenciaItem>(sQuery, new { item.Refx, item.Id_Solicitacao, item.Id_Conf }).SingleOrDefault();

                    if (retorno.Tem_Na_Nota == 1)
                    {
                        sQuery = @"
                        UPDATE gar_conferencia_item SET Quant_Conferida = Quant_Conferida + :Quant_Conferida, Id_Usr = :Id_Usr, Dt_Conf = SYSDATE
                        WHERE Id_Conf = :Id_Conf AND Refx = :Refx AND Id_Solicitacao = :Id_Solicitacao
                        ";
                        conn.Query<GarConferenciaItem>(sQuery, new
                        {
                            item.Quant_Conferida,
                            item.Id_Usr,
                            item.Id_Conf,
                            item.Refx,
                            item.Id_Solicitacao
                        });
                    }
                }
                conn.Close();
            }
        }

        public void CriarConferenciaEntrada(GarConferencia item)
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
                        INSERT INTO gar_conferencia_item (refx, id_solicitacao, id_conf, dt_conf, id_usr,quant)
                        SELECT
                            refx,
                            id_solicitacao,
                            :Id_Conferencia,
                            SYSDATE AS dt_conf,
                            :id_usr,
                            SUM(quant) AS quant
                        FROM
                            gar_solicitacao_item
                        WHERE
                            id_solicitacao = :Id_Solicitacao
                        GROUP BY
                            refx,
                            id_solicitacao
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

        public long CriarConferenciaRemessa(GarConferencia item)
        {
            string sQuery = "";
            var Id_Conferencia = 0;
            using (var conn = new OracleConnection(Entities.Database.Connection.ConnectionString))
            {
                conn.Open();
                if (conn.State == ConnectionState.Open)
                {
                    sQuery = @"SELECT COUNT(*) FROM gar_conferencia WHERE Id_Remessa = :Id_Remessa AND ativo = 1";
                    var retorno = conn.Query<int>(sQuery, new { item.Id_Remessa }).SingleOrDefault();

                    if (retorno == 0)
                    {
                        var param = new DynamicParameters();
                        param.Add(name: "Id_Remessa", value: item.Id_Remessa, direction: ParameterDirection.Input);
                        param.Add(name: "Id_Tipo_Conf", value: item.Id_Tipo_Conf, direction: ParameterDirection.Input);
                        param.Add(name: "Id_Usr", value: item.Id_Usr, direction: ParameterDirection.Input);
                        param.Add(name: "Id", dbType: DbType.Int32, direction: ParameterDirection.Output);

                        conn.Execute(@"INSERT INTO gar_conferencia (Id_Remessa, Id_Tipo_Conf, Id_Usr, Dt_Conf)
                        VALUES (:Id_Remessa, :Id_Tipo_Conf, :Id_Usr, SYSDATE) returning Id into :Id", param);
                        Id_Conferencia = param.Get<int>("Id");
                    }
                }
                conn.Close();
            }
            return Id_Conferencia;
        }

        public void AdicionarConferenciaRemessaItem(GarConferencia item)
        {
            //MESMO SELECT USADO AO CRIAR REMESSA E AO ATUALIZAR ITENS
            //QUANTIDADE DA NOTA - QUANTIDADE CONFERIDA SE O ITEM JA ESTA NA CONFERENCIA
            //SE O ITENS AINDA NÃO TIVER NA CONFERENCIA PEGA SÓ QUANTIDADE DISPONIVEL
            //NÃO PEGA SE O REFX E ID_SOLICITACAO JA EXISTIR EM OUTRA CONFERENCIA ATIVA

            using (var conn = new OracleConnection(Entities.Database.Connection.ConnectionString))
            {
                string sQuery = "";
                conn.Open();
                if (conn.State == ConnectionState.Open)
                {
                    sQuery = @"SELECT GR.cod_fornecedor FROM gar_conferencia GC INNER JOIN gar_remessa GR ON GR.id = GC.id_remessa
                    WHERE GC.Id = :Id AND GC.ativo = 1";

                    var fornecedor = conn.Query<string>(sQuery, new { item.Id }).SingleOrDefault();

                    if (!string.IsNullOrEmpty(fornecedor))
                    {
                        sQuery = @"
                        INSERT INTO gar_conferencia_item (refx, id_solicitacao, id_conf, dt_conf, id_usr,quant)
                        WITH cte_itens AS
                        (
                            SELECT
                               GSI.refx,
                               GSI.id_solicitacao,
                               SUM(GM.quant - NVL(GRC.Quant_Enviado,0) ) AS quant
                            FROM
                                gar_movimentacao GM
                                INNER JOIN gar_solicitacao_item GSI ON GSI.id = GM.id_item
                                INNER JOIN gar_solicitacao GS ON GS.id = GSI.id_solicitacao
                                LEFT JOIN gar_remessa_controle GRC ON GRC.id_movimentacao = GM.id
                            WHERE
                                (
                                    GSI.cod_fornecedor = :fornecedor
                                    OR GSI.cod_fornecedor IN(SELECT cod_forn_filho FROM gar_forn_grupo WHERE cod_forn_pai = :fornecedor)
                                )
                                AND GM.id_tipo_estoque = 15
                                AND GM.id_tipo_movimentacao = 27
                                AND GS.id_empresa = :Id_Empresa
                            GROUP BY
                                GSI.refx,
                                GSI.id_solicitacao
                            HAVING (SELECT COUNT(*) FROM gar_conferencia_item GCI INNER JOIN gar_conferencia GC ON GCI.id_conf = GC.id AND GC.ativo = 1
                                WHERE GCI.id_solicitacao = GSI.id_solicitacao AND GCI.refx = GSI.refx ) = 0
                        )
                        SELECT
                            CI.refx, CI.id_solicitacao, :Id, SYSDATE AS dt_conf, :Id_Usr,  CI.quant
                        FROM
                            cte_itens CI
                            LEFT JOIN gar_conferencia_item GCI ON GCI.id_solicitacao = CI.id_solicitacao AND GCI.refx = CI.refx AND GCI.id_conf = :Id
                            LEFT JOIN gar_conferencia GC ON GCI.id_conf = GC.id AND GC.id_remessa != 0
                        WHERE
                        (
                            ( GC.id = :Id AND (CI.quant - NVL(GCI.quant,0)) > 0)
                            OR ( GC.id IS NULL AND CI.quant > 0 )
                        )
                        ";
                        conn.Query<GarConferenciaHist>(sQuery, new
                        {
                            item.Id,
                            item.Id_Usr,
                            fornecedor,
                            item.Id_Empresa
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
	                    ( Id_Conf, Refx, Id_Solicitacao, Volume, Id_Usr, Quant_Conferida, Dt_Conf )
                    VALUES
	                    ( :Id_Conf, :Refx, :Id_Solicitacao, :Volume, :Id_Usr, :Quant_Conferida, SYSDATE )
                    ";
                    conn.Query<GarConferenciaHist>(sQuery, new
                    {
                        item.Id_Conf,
                        item.Refx,
                        item.Id_Solicitacao,
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
                        TP.descrprod AS Descricao,
                        GCH.Id_Solicitacao,
                        U.""UserName"" AS Usr,
                        GCH.Volume,
                        GCH.Dt_Conf,
                        GCH.Quant_Conferida
                    FROM
                        gar_conferencia_hist GCH
                        INNER JOIN ""AspNetUsers"" U ON U.""Id"" =  GCH.id_usr
                        LEFT JOIN tgfpro@sankhya TP ON TP.ad_refx = GCH.refx
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
                        TP.descrprod AS Descricao,
                        GCI.Id_Solicitacao,
                        GCI.Refx
                    FROM
                        gar_conferencia_item GCI
                        LEFT JOIN tgfpro@sankhya TP ON TP.ad_refx = GCI.refx
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
                        GC.Id_Solicitacao,
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
                            Id_Solicitacao,
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
                            0 AS Id_Solicitacao,
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