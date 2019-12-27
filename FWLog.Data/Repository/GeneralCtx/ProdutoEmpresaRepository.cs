using FWLog.Data.Models;
using FWLog.Data.Repository.CommonCtx;
using Oracle.ManagedDataAccess.Client;
using System.Linq;

namespace FWLog.Data.Repository.GeneralCtx
{
    public class ProdutoEmpresaRepository : GenericRepository<ProdutoEmpresa>
    {
        public ProdutoEmpresaRepository(Entities entities) : base(entities)
        {

        }

        public ProdutoEmpresa ObterPorProdutoEmpresa(long idProduto, long idEmpresa)
        {
            return Entities.ProdutoEmpresa.FirstOrDefault(f => f.IdProduto == idProduto && f.IdEmpresa == idEmpresa);
        }

        public void AtualizarSaldoArmazenagem(long idProduto, long idEmpresa, int saldoArmazenagem)
        {
            string sql = "UPDATE \"ProdutoEmpresa\" SET \"SaldoArmazenagem\" = \"SaldoArmazenagem\" + :SALDOARMAZENAGEM WHERE \"IdProduto\" = :IDPRODUTO AND \"IdEmpresa\" = :IDEMPRESA ";

            Entities.Database.ExecuteSqlCommand(sql, new OracleParameter(":SALDOARMAZENAGEM", saldoArmazenagem), new OracleParameter(":IDPRODUTO", idProduto), new OracleParameter(":IDEMPRESA", idEmpresa));
        }
    }
}
