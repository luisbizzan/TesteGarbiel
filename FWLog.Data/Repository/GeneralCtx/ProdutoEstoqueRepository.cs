﻿using FWLog.Data.Models;
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

        public ProdutoEstoque ConsultarPorProduto(long idProduto)
        {
            return Entities.ProdutoEstoque.Where(x => x.IdProduto == idProduto).FirstOrDefault();
        }
    }
}
