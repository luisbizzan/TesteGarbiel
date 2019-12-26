using Dapper;
using FWLog.Data.Models;
using FWLog.Data.Repository.CommonCtx;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

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
                              "CASE WHEN A.\"IdLote\" IS NULL THEN B.\"Quantidade\" ELSE A.\"QuantidadeVolume\" END \"QuantidadeVolume\", " +
                              "CASE WHEN A.\"IdLote\" IS NULL THEN (SELECT SUM(\"Quantidade\") FROM \"NotaFiscalItem\" WHERE \"IdNotaFiscal\" = B.\"IdNotaFiscal\") ELSE A.\"QuantidadePeca\" END \"QuantidadePeca\", " +
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
                            "WHERE (B.\"IdNotaFiscalStatus\" <> 0 AND B.\"IdNotaFiscalStatus\" IS NOT NULL) AND B.\"IdEmpresa\" = " + idEmpresa +
                            " AND B.\"IdNotaFiscalTipo\" = " + idNotafiscalTipo.GetHashCode(),
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

        public bool Existe(Expression<Func<Lote, bool>> predicate)
        {
            return Entities.Lote.Any(predicate);
        }

        public IQueryable<Lote> Todos()
        {
            return Entities.Lote;
        }

        public class RelatorioResumoProducaoRecebimentoListItem
        {
            public string Nome { get; set; }

            public long NOTASRECEBIDASUSUARIO { get; set; }

            public long VOLUMESRECEBIDOSUSUARIO { get; set; }

            public long NOTASRECEBIDAS { get; set; }

            public long VOLUMESRECEBIDOS { get; set; }

            public decimal PERCENTUAL { get; set; }

            public long RANKING { get; set; }
        }

        public class RelatorioResumoProducaoRecebimentoRequest
        {
            public string UserId { get; set; }
            public long IdEmpresa { get; set; }
            public DateTime DateMin { get; set; }
            public DateTime? DateMax { get; set; }
        }

        public List<RelatorioResumoProducaoRecebimentoListItem> ResumoProducaoRecebimento(RelatorioResumoProducaoRecebimentoRequest request)
        {
            string stringQuery = "SELECT rel.*, ROWNUM Ranking FROM( SELECT perfilUsu.\"Nome\", metricasUsu.NotasRecebidasUsuario, metricasUsu.VolumesRecebidosUsuario, totalLote.NotasRecebidas, totalLote.VolumesRecebidos, TRUNC((metricasUsu.VolumesRecebidosUsuario / totalLote.VolumesRecebidos) * 100, 3) Percentual FROM ( SELECT l2.\"IdUsuarioRecebimento\" UsuarioId, COUNT(DISTINCT(l2.\"IdNotaFiscal\")) NotasRecebidasUsuario, SUM(l2.\"QuantidadeVolume\") VolumesRecebidosUsuario FROM \"Lote\" l2, \"NotaFiscal\" n WHERE n.\"IdNotaFiscal\" = l2.\"IdNotaFiscal\" AND n.\"IdEmpresa\" = :ID_EMP AND l2.\"DataRecebimento\" >= :DATA_MIN AND (:DATA_MAX IS NULL OR l2.\"DataRecebimento\" <= :DATA_MAX) AND l2.\"IdUsuarioRecebimento\" = :ID_USU GROUP BY l2.\"IdUsuarioRecebimento\") metricasUsu, ( SELECT COUNT(l3.\"IdNotaFiscal\") NotasRecebidas, SUM(l3.\"QuantidadeVolume\") VolumesRecebidos FROM \"Lote\" l3, \"NotaFiscal\" n WHERE n.\"IdNotaFiscal\" = l3.\"IdNotaFiscal\" AND n.\"IdEmpresa\" = :ID_EMP AND l3.\"DataRecebimento\" >= :DATA_MIN AND (:DATA_MAX IS NULL OR l3.\"DataRecebimento\" <= :DATA_MAX)) totalLote, \"PerfilUsuario\" perfilUsu WHERE perfilUsu.\"UsuarioId\" = metricasUsu.UsuarioId ORDER BY Percentual DESC) rel";

            var param = new
            {
                ID_USU = request.UserId,
                DATA_MIN = request.DateMin,
                DATA_MAX = request.DateMax,
                ID_EMP = request.IdEmpresa
            };

            var list = Entities.Database.Connection.Query<RelatorioResumoProducaoRecebimentoListItem>(stringQuery, param).ToList();

            return list;
        }
    }
}
