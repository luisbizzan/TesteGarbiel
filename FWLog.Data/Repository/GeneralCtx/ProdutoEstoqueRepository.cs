using Dapper;
using FWLog.Data.Models;
using FWLog.Data.Repository.CommonCtx;
using Oracle.ManagedDataAccess.Client;
using System.Collections.Generic;
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

        public int ObterDiasPrazoEntrega(long idEmpresa , List<long> listIdProdutos)
        {
            return Entities.ProdutoEstoque.Where(w => w.IdEmpresa == idEmpresa && listIdProdutos.Contains(w.IdProduto)).Max(m => m.DiasPrazoEntrega);
        }

        public ProdutoEstoque ConsultarPorProduto(long idProduto, long idEmpresa)
        {
            return Entities.ProdutoEstoque.Where(x => x.IdProduto == idProduto && x.IdEmpresa == idEmpresa).FirstOrDefault();
        }

        public IEnumerable<ProdutoEstoque> ObterProdutoEstoquePorEmpresa(long IdEmpresa)
        {
            IEnumerable<ProdutoEstoque> produtoEstoque = null;

            using (var conn = new OracleConnection(Entities.Database.Connection.ConnectionString))
            {
                conn.Open();

                if (conn.State == System.Data.ConnectionState.Open)
                {
                    produtoEstoque = conn.Query<ProdutoEstoque, Produto, UnidadeMedida, EnderecoArmazenagem, ProdutoEstoqueStatus, ProdutoEstoque>(
                    "SELECT " +
                        "produtoestoque.\"IdProduto\"," +
                        "produto.\"Descricao\",	" +
                        "produto.\"Referencia\",	" +
                        "produto.\"PesoBruto\", " +
                        "produto.\"Largura\"," +
                        "produto.\"Altura\"," +
                        "produto.\"Comprimento\"," +
                        "unidademedida.\"IdUnidadeMedida\"," +
                        "unidademedida.\"Sigla\"," +
                        "enderecoarmazenagem.\"IdEnderecoArmazenagem\"," +
                        "enderecoarmazenagem.\"Codigo\", " +
                        "produtoestoquestatus.\"IdProdutoEstoqueStatus\"," +
                        "produtoestoquestatus.\"Descricao\"" +
                    "FROM \"ProdutoEstoque\" produtoestoque " +
                    "INNER JOIN \"Produto\" produto ON produto.\"IdProduto\" = produtoestoque.\"IdProduto\" " +
                    "INNER JOIN \"UnidadeMedida\" unidademedida ON unidademedida.\"IdUnidadeMedida\" = produto.\"IdUnidadeMedida\" " +
                    "LEFT JOIN \"EnderecoArmazenagem\" enderecoarmazenagem ON enderecoarmazenagem.\"IdEnderecoArmazenagem\" = produtoestoque.\"IdEnderecoArmazenagem\" " +
                    "LEFT JOIN \"ProdutoEstoqueStatus\" produtoestoquestatus ON produtoestoquestatus.\"IdProdutoEstoqueStatus\" = produtoestoque.\"IdProdutoEstoqueStatus\" " +
                    "WHERE produtoestoque.\"IdEmpresa\" = " + IdEmpresa,
                    map: (pe, p, un, ea, pes) =>
                    {
                        pe.Produto = p;
                        pe.Produto.UnidadeMedida = un;
                        pe.EnderecoArmazenagem = ea;
                        pe.IdProdutoEstoqueStatus = pes.IdProdutoEstoqueStatus;
                        return pe;
                    },
                    splitOn: "IdProduto,Descricao,IdUnidadeMedida,IdEnderecoArmazenagem,IdProdutoEstoqueStatus"
                    );
                }

                conn.Close();
            }

            return produtoEstoque.OrderByDescending(x => x.IdProduto);
        }




    }
}
