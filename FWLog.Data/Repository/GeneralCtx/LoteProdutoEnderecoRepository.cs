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

        public List<EnderecoArmazenagemTotalPorAlasLinhaTabela> TotalDeInstalados(DataTableFilter<RelatorioTotalizacaoAlasListaFiltro> model, out int totalRecordsFiltered, out int totalRecords)
        {
            var enderecoArmazenagemIds = model.CustomFilter.ListaIdEnderecoArmazenagem.Select(x => x.IdEnderecoArmazenagem).ToList();


            totalRecords = Entities.LoteProdutoEndereco.Where(x => x.IdEmpresa == model.CustomFilter.IdEmpresa).Count();

            IQueryable<EnderecoArmazenagemTotalPorAlasLinhaTabela> query =
                Entities.LoteProdutoEndereco.AsNoTracking().Where(
                    lpe => lpe.IdEmpresa == model.CustomFilter.IdEmpresa &&
                    enderecoArmazenagemIds.Contains(lpe.IdEnderecoArmazenagem) &&
                    model.CustomFilter.IdNivelArmazenagem == lpe.EnderecoArmazenagem.IdNivelArmazenagem &&
                    model.CustomFilter.IdPontoArmazenagem == lpe.EnderecoArmazenagem.IdPontoArmazenagem).ToList()
                 .Select(s => new EnderecoArmazenagemTotalPorAlasLinhaTabela
                 {
                     IdEnderecoArmazenagem = s.IdEnderecoArmazenagem,
                     CodigoEndereco = s.EnderecoArmazenagem.Codigo,
                     DataInstalacao = s.DataHoraInstalacao.ToString("dd/MM/yyyy HH:mm:ss"),
                     IdUsuarioInstalacao = s.IdUsuarioInstalacao,
                     PesoProduto = s.Produto.PesoBruto.ToString("n2"),
                     PesoTotalDeProduto = s.PesoTotal.ToString("n2"),
                     QuantidadeProdutoPorEndereco = s.Quantidade,
                     ReferenciaProduto = s.Produto.Referencia,
                     Corredor = s.EnderecoArmazenagem.Corredor
                 }).AsQueryable();


            if (model.CustomFilter.CorredorInicial > 0 && model.CustomFilter.CorredorFinal > 0)
            {
                var range = Enumerable.Range(model.CustomFilter.CorredorInicial, model.CustomFilter.CorredorFinal);

                query = query.Where(y => range.Contains(y.Corredor));
            }

            //if (model.CustomFilter.ImprimirVazia)
            //{
            //    query = (from end in Entities.EnderecoArmazenagem
            //             join lpe in Entities.LoteProdutoEndereco on end.IdEnderecoArmazenagem equals lpe.IdEnderecoArmazenagem into a
            //             from lpe in a.DefaultIfEmpty()
            //             select new EnderecoArmazenagemTotalPorAlasLinhaTabela
            //             {
            //                 IdEnderecoArmazenagem = end.IdEnderecoArmazenagem,
            //                 CodigoEndereco = end.Codigo,
            //                 Corredor = end.Corredor,
            //                 DataInstalacao = lpe.DataHoraInstalacao.ToString("dd/MM/yyyy HH:mm:ss") ?? null,
            //                 IdUsuarioInstalacao = lpe.IdUsuarioInstalacao ?? null,
            //                 PesoProduto = lpe.Produto.PesoBruto.ToString("n2") ?? null,
            //                 PesoTotalDeProduto = lpe.PesoTotal.ToString("n2") ?? null,
            //                 QuantidadeProdutoPorEndereco = lpe.Quantidade != null ? lpe.Quantidade : 0,
            //                 ReferenciaProduto = lpe.Produto.Referencia ?? "-",



            //             });
            //}


            totalRecordsFiltered = query.Count();

            query = query
                .OrderBy(model.OrderByColumn, model.OrderByDirection)
                .Skip(model.Start)
                .Take(model.Length);

            return query.ToList();
        }
    }
}