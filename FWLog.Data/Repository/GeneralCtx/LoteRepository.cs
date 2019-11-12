using Dapper;
using FWLog.Data.Models;
using FWLog.Data.Repository.CommonCtx;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FWLog.Data.Repository.GeneralCtx
{
    public class LoteRepository : GenericRepository<Lote>
    {
        public LoteRepository(Entities entities) : base(entities)
        {

        }
        public IEnumerable<Lote> ObterLote()
        {
            IEnumerable<Lote> lote = null;

            using (var conn = new OracleConnection(Entities.Database.Connection.ConnectionString))
            {
                conn.Open();

                if (conn.State == System.Data.ConnectionState.Open)
                {
                    lote = conn.Query<Lote, NotaFiscal, Fornecedor, FreteTipo, LoteStatus, AspNetUsers, Lote>(
                        "SELECT " +
                            "A.\"IdLote\", " +
                            "A.\"DataRecebimento\", " +
                            "A.\"QuantidadePeca\", " +
                            "A.\"QuantidadeVolume\", " +
                            "B.\"IdNotaFiscal\", " +
                            "B.\"Numero\", " +
                            "B.\"Serie\", " +
                            "B.\"DANFE\", " +
                            "B.\"ValorTotal\", " +
                            "B.\"ValorFrete\", " +
                            "B.\"NumeroConhecimento\", " +
                            "B.\"PesoBruto\", " +
                            "B.\"PesoLiquido\", " +
                            "B.\"Especie\", " +
                            "B.\"Quantidade\", " +
                            "B.\"Status\", " +
                            "B.\"Chave\", " +
                            "B.\"CodigoNotaFiscal\", " +
                            "B.\"DataEmissao\", " +
                            "C.*, " +
                            "D.\"IdFreteTipo\", " +
                            "D.\"Sigla\", " +
                            "E.*, " +
                            "F.* " +
                         "FROM " +
                            "\"Lote\" A " +
                            "RIGHT JOIN \"NotaFiscal\" B ON B.\"IdNotaFiscal\" = A.\"IdNotaFiscal\" " +
                            "INNER JOIN \"Fornecedor\" C ON C.\"IdFornecedor\" = B.\"IdFornecedor\" " +
                            "INNER JOIN \"FreteTipo\" D ON D.\"IdFreteTipo\" = B.\"IdFreteTipo\" " +
                            "LEFT JOIN \"LoteStatus\" E ON (E.\"IdLoteStatus\" = CASE WHEN A.\"IdLoteStatus\" IS NULL THEN 1 ELSE A.\"IdLoteStatus\" END) " +
                            "LEFT JOIN \"AspNetUsers\" F ON F.\"Id\" = A.\"IdUsuarioRecebimento\" ",
                        map: (l, nf, f, ft, ls, u) =>
                        {
                            l.NotaFiscal = nf;
                            l.NotaFiscal.Fornecedor = f;
                            l.NotaFiscal.FreteTipo = ft;
                            l.LoteStatus = ls;
                            l.UsuarioRecebimento = u;
                           
                            return l;
                        },
                        splitOn: "IdLote, IdNotaFiscal, IdFornecedor, IdFreteTipo, IdLoteStatus, Id"
                        );
                }
            }

            return lote;
        }
    }
}
