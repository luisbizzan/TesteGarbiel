using FWLog.Data.Models;
using FWLog.Data.Repository.CommonCtx;
using Oracle.ManagedDataAccess.Client;
using System.Linq;

namespace FWLog.Data.Repository.GeneralCtx
{
    public class ProdutoEstoqueRepository : GenericRepository<ProdutoEstoque>
    {
        public ProdutoEstoqueRepository(Entities entities) : base(entities) { }

        public ProdutoEstoque ObterPorProdutoEmpresa(long idProduto, long idEmpresa)
        {
            return Entities.ProdutoEstoque.FirstOrDefault(f => f.IdProduto == idProduto && f.IdEmpresa == idEmpresa);
        }

        public void AtualizarSaldoArmazenagem(long idProduto, long idEmpresa, int saldo)
        {
            string sql = "UPDATE \"ProdutoEstoque\" SET \"Saldo\" = \"Saldo\" + :SALDO WHERE \"IdProduto\" = :IDPRODUTO AND \"IdEmpresa\" = :IDEMPRESA ";
            Entities.Database.ExecuteSqlCommand(sql, new OracleParameter(":SALDO", saldo), new OracleParameter(":IDPRODUTO", idProduto), new OracleParameter(":IDEMPRESA", idEmpresa));
        }
    }
}
