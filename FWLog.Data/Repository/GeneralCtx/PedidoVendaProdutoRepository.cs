using FWLog.Data.Models;
using FWLog.Data.Models.DataTablesCtx;
using FWLog.Data.Models.FilterCtx;
using FWLog.Data.Repository.CommonCtx;
using System.Collections.Generic;
using System.Linq;

namespace FWLog.Data.Repository.GeneralCtx
{
    public class PedidoVendaProdutoRepository : GenericRepository<PedidoVendaProduto>
    {
        public PedidoVendaProdutoRepository(Entities entities) : base(entities)
        {
        }

        public List<PedidoVendaProduto> ObterPorIdPedidoVenda(long idPedidoVenda)
        {
            return Entities.PedidoVendaProduto.Where(x => x.IdPedidoVenda == idPedidoVenda).ToList();
        }

        public PedidoVendaProduto ObterPorIdProduto(long idProduto)
        {
            return Entities.PedidoVendaProduto.FirstOrDefault(x => x.IdProduto == idProduto);
        }

        public PedidoVendaProduto ObterPorIdPedidoVendaVolumeEIdProduto(long idPedidoVendaVolume, long idProduto)
        {
            return Entities.PedidoVendaProduto.FirstOrDefault(pvp => pvp.IdPedidoVendaVolume == idPedidoVendaVolume && pvp.IdProduto == idProduto);
        }

        public List<ProdutoPesquisaModalListaLinhaTabela> BuscarLista(DataTableFilter<ProdutoPesquisaModalFiltro> model, out int totalRecordsFiltered, out int totalRecords)
        {
            totalRecords = Entities.PedidoVendaProduto.Where(w => w.IdLote == model.CustomFilter.IdLote).Count();

            IQueryable<ProdutoPesquisaModalListaLinhaTabela> query = Entities.PedidoVendaProduto.AsNoTracking()
               .Where(w => w.IdLote.HasValue == false &&
                 w.IdPedidoVendaVolume == model.CustomFilter.IdPedidoVendaVolume.Value &&                
                (w.PedidoVenda.IdPedidoVendaStatus == PedidoVendaStatusEnum.EnviadoSeparacao ||
                 w.PedidoVenda.IdPedidoVendaStatus == PedidoVendaStatusEnum.ProcessandoSeparacao) &&
                (w.PedidoVendaVolume.IdPedidoVendaStatus == PedidoVendaStatusEnum.EnviadoSeparacao ||
                 w.PedidoVendaVolume.IdPedidoVendaStatus == PedidoVendaStatusEnum.ProcessandoSeparacao ||
                 w.PedidoVendaVolume.IdPedidoVendaStatus == PedidoVendaStatusEnum.SeparacaoConcluidaComSucesso) &&
                (w.IdPedidoVendaStatus == PedidoVendaStatusEnum.EnviadoSeparacao ||
                 w.IdPedidoVendaStatus == PedidoVendaStatusEnum.ProcessandoSeparacao ||
                 w.IdPedidoVendaStatus == PedidoVendaStatusEnum.SeparacaoConcluidaComSucesso) &&
                (model.CustomFilter.Referencia.Equals(string.Empty) || w.Produto.Referencia.Contains(model.CustomFilter.Referencia)) &&
                (model.CustomFilter.Descricao.Equals(string.Empty) || w.Produto.Descricao.Contains(model.CustomFilter.Descricao)) &&
                (model.CustomFilter.Status.HasValue == false || w.Produto.Ativo == model.CustomFilter.Status))
               .Select(s => new ProdutoPesquisaModalListaLinhaTabela
               {
                   IdProduto = s.IdProduto,
                   Referencia = s.Produto.Referencia,
                   Descricao = s.Produto.Descricao,
                   Status = s.Produto.Ativo ? "Ativo" : "Inativo"
               });

            totalRecordsFiltered = query.Count();

            query = query
                .OrderBy(model.OrderByColumn, model.OrderByDirection)
                .Skip(model.Start)
                .Take(model.Length);

            return query.ToList();
        }
    }
}