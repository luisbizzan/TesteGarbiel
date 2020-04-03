using FWLog.Data.Models;
using FWLog.Data.Models.DataTablesCtx;
using FWLog.Data.Models.FilterCtx;
using FWLog.Data.Repository.CommonCtx;
using System.Collections.Generic;
using System.Linq;

namespace FWLog.Data.Repository.GeneralCtx
{
    public class LoteProdutoEnderecoRepository : GenericRepository<LoteProdutoEndereco>
    {
        public LoteProdutoEnderecoRepository(Entities entities) : base(entities) { }

        public List<LoteProdutoEndereco> PesquisarPorEmpresa(long idEmpresa)
        {
            return Entities.LoteProdutoEndereco.Include("ProdutoEstoque").Where(w => w.IdEmpresa == idEmpresa).ToList();
        }

        public List<LoteProdutoEndereco> PesquisarPorLoteProduto(long idLote, long idProduto)
        {
            return Entities.LoteProdutoEndereco.Where(w => w.IdLote == idLote && w.IdProduto == idProduto).ToList();
        }

        public List<LoteProdutoEndereco> PesquisarPorEnderecos(List<long> idsEnderecosArmazenagem)
        {
            return Entities.LoteProdutoEndereco.Where(w => idsEnderecosArmazenagem.Contains(w.IdEnderecoArmazenagem)).ToList();
        }

        public LoteProdutoEndereco PesquisarPorEndereco(long idEnderecoArmazenagem)
        {
            return Entities.LoteProdutoEndereco.Where(w => w.IdEnderecoArmazenagem == idEnderecoArmazenagem).FirstOrDefault();
        }

        public IEnumerable<LoteProdutoEndereco> PesquisarRegistrosPorEndereco(long idEnderecoArmazenagem)
        {
            return Entities.LoteProdutoEndereco.Where(w => w.IdEnderecoArmazenagem == idEnderecoArmazenagem).ToList();
        }

        public LoteProdutoEndereco PesquisarPorEnderecoLoteProdutoEmpresa(long idEnderecoArmazenagem, long idLote, long idProduto, long IdEmpresa)
        {
            return Entities.LoteProdutoEndereco.Where(w => w.IdEnderecoArmazenagem == idEnderecoArmazenagem &&
                                                             w.IdLote == idLote &&
                                                             w.IdProduto == idProduto &&
                                                             w.IdEmpresa == IdEmpresa).FirstOrDefault();
        }

        public List<LoteProdutoEndereco> PesquisarPorProdutoComLote(long idProduto)
        {
            return Entities.LoteProdutoEndereco.Where(loteProdutoEndereco => loteProdutoEndereco.IdProduto == idProduto && loteProdutoEndereco.IdLote != null).ToList();
        }

        public IEnumerable<EnderecoArmazenagemTotalPorAlasLinhaTabela> BuscarDados(DataTableFilter<RelatorioTotalizacaoAlasListaFiltro> model, out int totalRecordsFiltered, out int totalRecords)
        {
            IQueryable<EnderecoArmazenagemTotalPorAlasLinhaTabela> query;

            if (model.CustomFilter.ImprimirVazia)
            {
                totalRecords = Entities.EnderecoArmazenagem
                .Where(x => x.IdEmpresa == model.CustomFilter.IdEmpresa &&
                      model.CustomFilter.IdNivelArmazenagem == x.IdNivelArmazenagem &&
                      model.CustomFilter.IdPontoArmazenagem == x.IdPontoArmazenagem).Count();

                query = (from end in Entities.EnderecoArmazenagem
                         join lpe in Entities.LoteProdutoEndereco on end.IdEnderecoArmazenagem equals lpe.IdEnderecoArmazenagem into a
                         from lpe in a.DefaultIfEmpty()
                         where model.CustomFilter.IdNivelArmazenagem == end.IdNivelArmazenagem &&
                               model.CustomFilter.IdPontoArmazenagem == end.IdPontoArmazenagem
                         select new EnderecoArmazenagemTotalPorAlasLinhaTabela
                         {
                             IdEnderecoArmazenagem = end.IdEnderecoArmazenagem,
                             CodigoEndereco = end.Codigo,
                             Corredor = end.Corredor,
                             DataInstalacao = lpe.DataHoraInstalacao,
                             IdLote = lpe.IdLote,
                             IdUsuarioInstalacao = lpe.IdUsuarioInstalacao,
                             PesoProduto = lpe.Produto.PesoBruto,
                             PesoTotalDeProduto = lpe.PesoTotal,
                             QuantidadeProdutoPorEndereco = lpe.Quantidade,
                             ReferenciaProduto = lpe.Produto.Referencia
                         });
            }
            else
            {
                var enderecoArmazenagemIds = model.CustomFilter.ListaEnderecoArmazenagem.Select(x => x.IdEnderecoArmazenagem).ToList();

                totalRecords = Entities.LoteProdutoEndereco
                    .Where(x => x.IdEmpresa == model.CustomFilter.IdEmpresa &&
                          enderecoArmazenagemIds.Contains(x.IdEnderecoArmazenagem) &&
                          model.CustomFilter.IdNivelArmazenagem == x.EnderecoArmazenagem.IdNivelArmazenagem &&
                          model.CustomFilter.IdPontoArmazenagem == x.EnderecoArmazenagem.IdPontoArmazenagem).Count();

                query =
                    Entities.LoteProdutoEndereco.AsNoTracking().Where(
                        lpe => lpe.IdEmpresa == model.CustomFilter.IdEmpresa &&
                        enderecoArmazenagemIds.Contains(lpe.IdEnderecoArmazenagem) &&
                        model.CustomFilter.IdNivelArmazenagem == lpe.EnderecoArmazenagem.IdNivelArmazenagem &&
                        model.CustomFilter.IdPontoArmazenagem == lpe.EnderecoArmazenagem.IdPontoArmazenagem).ToList()
                     .Select(s => new EnderecoArmazenagemTotalPorAlasLinhaTabela
                     {
                         IdEnderecoArmazenagem = s.IdEnderecoArmazenagem,
                         CodigoEndereco = s.EnderecoArmazenagem.Codigo,
                         DataInstalacao = s.DataHoraInstalacao,
                         IdUsuarioInstalacao = s.IdUsuarioInstalacao,
                         PesoProduto = s.Produto.PesoBruto,
                         IdLote = s.Lote.IdLote,
                         PesoTotalDeProduto = s.PesoTotal,
                         QuantidadeProdutoPorEndereco = s.Quantidade,
                         ReferenciaProduto = s.Produto.Referencia,
                         Corredor = s.EnderecoArmazenagem.Corredor
                     }).OrderBy(x => x.CodigoEndereco).AsQueryable();

            }

            if (model.CustomFilter.CorredorInicial > 0 && model.CustomFilter.CorredorFinal > 0)
            {
                var range = Enumerable.Range(model.CustomFilter.CorredorInicial, model.CustomFilter.CorredorFinal);

                query = query.Where(y => range.Contains(y.Corredor));
            }

            totalRecordsFiltered = query.Count();

            query = query
                .OrderBy(model.OrderByColumn, model.OrderByDirection)
                .Skip(model.Start)
                .Take(model.Length);

            return query.ToList();
        }
    }
}