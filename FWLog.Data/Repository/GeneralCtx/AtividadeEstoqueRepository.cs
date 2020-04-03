﻿using FWLog.Data.Models;
using FWLog.Data.Models.DataTablesCtx;
using FWLog.Data.Repository.CommonCtx;
using System.Collections.Generic;
using System.Linq;

namespace FWLog.Data.Repository.GeneralCtx
{
    public class AtividadeEstoqueRepository : GenericRepository<AtividadeEstoque>
    {
        public AtividadeEstoqueRepository(Entities entities) : base(entities) { }

        public AtividadeEstoque Pesquisar(long idEmpresa, AtividadeEstoqueTipoEnum idAtividadeEstoqueTipo, long idEnderecoArmazenagem, long idProduto, bool finalizado)
        {
            return Entities.AtividadeEstoque.Where(x => x.IdEmpresa == idEmpresa && x.IdAtividadeEstoqueTipo == idAtividadeEstoqueTipo && x.IdEnderecoArmazenagem == idEnderecoArmazenagem &&
            x.IdProduto == idProduto && x.Finalizado == finalizado).FirstOrDefault();
        }

        public List<AtividadeEstoqueListaLinhaTabela> PesquisarAtividade(long idEmpresa)
        {
            var query = (from a in Entities.AtividadeEstoque
                         join e in Entities.EnderecoArmazenagem on a.IdEnderecoArmazenagem equals e.IdEnderecoArmazenagem
                         join p in Entities.Produto on a.IdProduto equals p.IdProduto
                         where a.IdEmpresa == idEmpresa && !a.Finalizado
                         orderby e.Codigo, e.Horizontal, e.Vertical, e.Divisao
                         select new AtividadeEstoqueListaLinhaTabela
                         {
                             IdAtividadeEstoque = a.IdAtividadeEstoque,
                             IdAtividadeEstoqueTipo = (int)a.AtividadeEstoqueTipo.IdAtividadeEstoqueTipo,
                             DescricaoAtividadeEstoqueTipo = a.AtividadeEstoqueTipo.Descricao,
                             IdEnderecoArmazenagem = e.IdEnderecoArmazenagem,
                             IdProduto = p.IdProduto,
                             Referencia = p.Referencia,
                             CodigoEndereco = e.Codigo,
                             Corredor = e.Corredor
                         });

            return query.ToList();
        }

        public List<AtividadeEstoque> PesquisarProdutosPendentes(long idEmpresa, AtividadeEstoqueTipoEnum idAtividadeEstoqueTipo, long idProduto)
        {
            return Entities.AtividadeEstoque.Where(x => x.IdEmpresa == idEmpresa &&
                                                            x.IdAtividadeEstoqueTipo == idAtividadeEstoqueTipo &&
                                                            x.IdProduto == idProduto &&
                                                            x.Finalizado == false).ToList();
        }
    }
}