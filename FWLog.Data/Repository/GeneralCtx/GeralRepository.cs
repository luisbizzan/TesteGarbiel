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

namespace FWLog.Data.Repository.GeneralCtx
{
    public class GeralRepository : GenericRepository<Garantia>
    {
        public GeralRepository(Entities entities) : base(entities)
        {
        }

        public List<GeralHistorico> TodosHistoricosDaCategoria(long Id_Categoria, long Id_Ref)
        {
            List<GeralHistorico> retorno = new List<GeralHistorico>();

            using (var conn = new OracleConnection(Entities.Database.Connection.ConnectionString))
            {
                conn.Open();
                if (conn.State == System.Data.ConnectionState.Open)
                {
                    string sQuery = @"
                    SELECT
                        GH.id,
                        GH.id_categoria,
                        GH.id_ref,
                        GH.id_usr,
                        GH.dt_cad,
                        U.""UserName"" AS usuario,
                        GH.historico
                    FROM
                        geral_historico GH
                        INNER JOIN ""AspNetUsers"" U ON U.""Id"" =  GH.id_usr
                    WHERE
                        GH.id_ref = :Id_Ref
                        AND GH.id_categoria = :Id_Categoria
                    ";
                    retorno = conn.Query<GeralHistorico>(sQuery, new { Id_Ref, Id_Categoria }).ToList();
                }
                conn.Close();
            }

            return retorno;
        }

        public void InserirHistorico(GeralHistorico item)
        {
            using (var conn = new OracleConnection(Entities.Database.Connection.ConnectionString))
            {
                conn.Open();
                if (conn.State == System.Data.ConnectionState.Open)
                {
                    string sQuery = @"
                    INSERT INTO geral_historico
	                    ( Id_Categoria, Id_Ref, Id_Usr, Historico, Dt_Cad )
                    VALUES
	                    ( :Id_Categoria, :Id_Ref, :Id_Usr, :Historico, SYSDATE )
                    ";
                    conn.Query<GeralHistorico>(sQuery, new
                    {
                        item.Id_Categoria,
                        item.Id_Ref,
                        item.Id_Usr,
                        item.Historico
                    });
                }
                conn.Close();
            }
        }

        public List<GeralUpload> TodosUploadsDaCategoria(long Id_Categoria, long Id_Ref)
        {
            List<GeralUpload> retorno = new List<GeralUpload>();

            using (var conn = new OracleConnection(Entities.Database.Connection.ConnectionString))
            {
                conn.Open();
                if (conn.State == System.Data.ConnectionState.Open)
                {
                    string sQuery = @"
                    SELECT
                        GU.id,
                        GU.id_categoria,
                        GU.id_ref,
                        GU.id_usr,
                        GU.dt_cad,
                        U.""UserName"" AS usuario,
                        GU.arquivo,
                        GU.arquivo_tipo
                    FROM
                        geral_upload GU
                        INNER JOIN ""AspNetUsers"" U ON U.""Id"" =  GU.id_usr
                    WHERE
                        GU.id_ref = :Id_Ref
                        AND GU.id_categoria = :Id_Categoria
                    ";
                    retorno = conn.Query<GeralUpload>(sQuery, new { Id_Ref, Id_Categoria }).ToList();
                }
                conn.Close();
            }

            return retorno;
        }

        public GeralUploadCategoria SelecionaUploadCategoria(long Id_Categoria)
        {
            GeralUploadCategoria retorno = new GeralUploadCategoria();

            using (var conn = new OracleConnection(Entities.Database.Connection.ConnectionString))
            {
                conn.Open();
                if (conn.State == System.Data.ConnectionState.Open)
                {
                    string sQuery = @"
                    SELECT
                        tabela,
                        categoria,
                        formatos
                    FROM
                        geral_upload_categoria
                    WHERE
                        id = :Id_Categoria
                    ";
                    retorno = conn.Query<GeralUploadCategoria>(sQuery, new { Id_Categoria }).SingleOrDefault();
                }
                conn.Close();
            }

            return retorno;
        }

        public void InserirUpload(GeralUpload item)
        {
            using (var conn = new OracleConnection(Entities.Database.Connection.ConnectionString))
            {
                conn.Open();
                if (conn.State == System.Data.ConnectionState.Open)
                {
                    string sQuery = @"
                    INSERT INTO geral_upload
	                    ( Id_Categoria, Id_Ref, Id_Usr, Arquivo, Arquivo_Tipo, Dt_Cad )
                    VALUES
	                    ( :Id_Categoria, :Id_Ref, :Id_Usr, :Arquivo, :Arquivo_Tipo, SYSDATE )
                    ";
                    conn.Query<GeralUpload>(sQuery, new
                    {
                        item.Id_Categoria,
                        item.Id_Ref,
                        item.Id_Usr,
                        item.Arquivo,
                        item.Arquivo_Tipo,
                    });
                }
                conn.Close();
            }
        }

        public void ExcluirUpload(long id)
        {
            using (var conn = new OracleConnection(Entities.Database.Connection.ConnectionString))
            {
                conn.Open();
                if (conn.State == System.Data.ConnectionState.Open)
                {
                    string sQuery = @"
                    DELETE FROM geral_upload WHERE id = :id
                    ";
                    conn.Query<GeralUpload>(sQuery, new { id });
                }
                conn.Close();
            }
        }
    }
}