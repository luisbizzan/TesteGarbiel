using Dapper;
using FWLog.Data.Models;
using FWLog.Data.Repository.CommonCtx;
using Oracle.ManagedDataAccess.Client;
using System.Collections.Generic;
using System.Linq;

namespace FWLog.Data.Repository.GeneralCtx
{
    public class LoteRepository : GenericRepository<Lote>
    {
        public LoteRepository(Entities entities) : base(entities) { }

        public Lote PesquisarLotePorNotaFiscal(long idNotaFiscal)
        {
            return Entities.Lote.FirstOrDefault(f => f.IdNotaFiscal == idNotaFiscal);
        }

        public IEnumerable<Lote> Obter(long idEmpresa, NotaFiscalTipoEnum idNotafiscalTipo)
        {
            IEnumerable<Lote> lote = null;

            using (var conn = new OracleConnection(Entities.Database.Connection.ConnectionString))
            {
                conn.Open();

                if (conn.State == System.Data.ConnectionState.Open)
                {
                    lote = conn.Query<Lote, NotaFiscal, Fornecedor, FreteTipo, LoteStatus, AspNetUsers, NotaFiscalStatus, Lote>(
                          "SELECT " +
                              "A.\"IdLote\", " +
                              "A.\"DataRecebimento\", " +
                              "A.\"QuantidadeVolume\", " +
                              "B.\"IdNotaFiscal\", " +
                              "B.\"Numero\", " +
                              "B.\"Serie\", " +
                              "B.\"ValorTotal\", " +
                              "B.\"ValorFrete\", " +
                              "B.\"NumeroConhecimento\", " +
                              "B.\"PesoBruto\", " +
                              "B.\"Especie\", " +
                              "B.\"Quantidade\", " +
                              "B.\"ChaveAcesso\", " +
                              "B.\"CodigoIntegracao\", " +
                              "B.\"DataEmissao\", " +
                              "B.\"PrazoEntregaFornecedor\", " +
                              "B.\"IdEmpresa\", " +                              
                              "C.*, " +
                              "D.\"IdFreteTipo\", " +
                              "D.\"Sigla\", " +
                              "E.*, " +
                              "F.*, " +
                              "G.* " +                              
                           "FROM " +
                              "\"Lote\" A " +
                              "RIGHT JOIN \"NotaFiscal\" B ON B.\"IdNotaFiscal\" = A.\"IdNotaFiscal\" " +
                              "INNER JOIN \"Fornecedor\" C ON C.\"IdFornecedor\" = B.\"IdFornecedor\" " +
                              "INNER JOIN \"FreteTipo\" D ON D.\"IdFreteTipo\" = B.\"IdFreteTipo\" " +
                              "LEFT JOIN \"LoteStatus\" E ON (E.\"IdLoteStatus\" = CASE WHEN A.\"IdLoteStatus\" IS NULL THEN 1 ELSE A.\"IdLoteStatus\" END) " +
                              "LEFT JOIN \"AspNetUsers\" F ON F.\"Id\" = A.\"IdUsuarioRecebimento\" " +
                              "INNER JOIN \"NotaFiscalStatus\" G ON G.\"IdNotaFiscalStatus\" = B.\"IdNotaFiscalStatus\" " +                             
                            "WHERE (B.\"IdNotaFiscalStatus\" <> 0 AND B.\"IdNotaFiscalStatus\" IS NOT NULL) AND B.\"IdEmpresa\" =  " + idEmpresa +
                            "AND B.\"IdNotaFiscalTipo\" = " + idNotafiscalTipo.GetHashCode(),
                          map: (l, nf, f, ft, ls, u, nfs) =>
                          {
                              l.NotaFiscal = nf;
                              l.NotaFiscal.Fornecedor = f;
                              l.NotaFiscal.FreteTipo = ft;
                              l.LoteStatus = ls;
                              l.UsuarioRecebimento = u;
                              l.NotaFiscal.NotaFiscalStatus = nfs;                            
                              return l;
                          },
                          splitOn: "IdLote, IdNotaFiscal, IdFornecedor, IdFreteTipo, IdLoteStatus, Id, IdNotaFiscalStatus"
                          );
                }
            }

            return lote;
        }

        public Lote ObterLoteNota(long idNotaFiscal)
        {
            return Entities.Lote.Where(w => w.IdNotaFiscal == idNotaFiscal).FirstOrDefault();
        }
    }
}
