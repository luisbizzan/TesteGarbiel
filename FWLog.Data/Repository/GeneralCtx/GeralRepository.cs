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
    }
}