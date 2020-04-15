using FWLog.Data.Models;
using FWLog.Data.Models.DataTablesCtx;
using FWLog.Data.Models.FilterCtx;
using FWLog.Data.Repository.CommonCtx;
using System;
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

        public LoteProdutoEndereco PesquisarPorEnderecoProdutoEmpresa(long idEnderecoArmazenagem, long idProduto, long IdEmpresa)
        {
            return Entities.LoteProdutoEndereco.Where(w => w.IdEnderecoArmazenagem == idEnderecoArmazenagem &&
                                                             w.IdProduto == idProduto &&
                                                             w.IdEmpresa == IdEmpresa &&
                                                             w.IdLote == null).FirstOrDefault();
        }

        public LoteProdutoEndereco PesquisarPorEnderecoLoteProdutoEmpresa(long idEnderecoArmazenagem, long idLote, long idProduto, long IdEmpresa)
        {
            return Entities.LoteProdutoEndereco.Where(w => w.IdEnderecoArmazenagem == idEnderecoArmazenagem &&
                                                             w.IdLote == idLote &&
                                                             w.IdProduto == idProduto &&
                                                             w.IdEmpresa == IdEmpresa).FirstOrDefault();
        }

        public List<LoteProdutoEndereco> PesquisarPorProdutoComLote(long idProduto, long idEmpresa)
        {
            return Entities.LoteProdutoEndereco.Where(lpe => lpe.IdProduto == idProduto && lpe.IdEmpresa == idEmpresa && lpe.IdLote != null).ToList();
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

                query = Entities.LoteProdutoEndereco.AsNoTracking().Where(
                        lpe => lpe.IdEmpresa == model.CustomFilter.IdEmpresa &&
                        enderecoArmazenagemIds.Contains(lpe.IdEnderecoArmazenagem) &&
                        model.CustomFilter.IdNivelArmazenagem == lpe.EnderecoArmazenagem.IdNivelArmazenagem &&
                        model.CustomFilter.IdPontoArmazenagem == lpe.EnderecoArmazenagem.IdPontoArmazenagem)
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
                     });

            }

            if (model.CustomFilter.CorredorInicial > 0 && model.CustomFilter.CorredorFinal > 0)
            {
                var range = Enumerable.Range(model.CustomFilter.CorredorInicial, model.CustomFilter.CorredorFinal);

                query = query.Where(y => range.Contains(y.Corredor));
            }

            totalRecordsFiltered = query.Count();

            query = query
                .OrderBy(model.OrderByColumn, model.OrderByDirection).OrderBy(x => x.Corredor).ThenBy(x => x.CodigoEndereco)
                .Skip(model.Start)
                .Take(model.Length);

            return query.ToList();
        }

        public IEnumerable<PosicaoInventarioListaLinhaTabela> BuscarDadosPosicaoInventario(DataTableFilter<RelatorioPosicaoInventarioListaFiltro> model, out int totalRecordsFiltered, out int totalRecords)
        {
            totalRecords = Entities.LoteProdutoEndereco
                .Where(x => x.IdEmpresa == model.CustomFilter.IdEmpresa &&
                      (model.CustomFilter.IdNivelArmazenagem.HasValue == false || x.EnderecoArmazenagem.IdNivelArmazenagem == model.CustomFilter.IdNivelArmazenagem) &&
                      (model.CustomFilter.IdPontoArmazenagem.HasValue == false || x.EnderecoArmazenagem.IdPontoArmazenagem == model.CustomFilter.IdPontoArmazenagem) &&
                      (model.CustomFilter.IdProduto.HasValue == false || x.IdProduto == model.CustomFilter.IdProduto.Value)).Count();


            IQueryable<PosicaoInventarioListaLinhaTabela> query = Entities.LoteProdutoEndereco.AsNoTracking()
                .Where(x => x.IdEmpresa == model.CustomFilter.IdEmpresa &&
                (model.CustomFilter.IdNivelArmazenagem.HasValue == false || x.EnderecoArmazenagem.IdNivelArmazenagem == model.CustomFilter.IdNivelArmazenagem) &&
                (model.CustomFilter.IdPontoArmazenagem.HasValue == false || x.EnderecoArmazenagem.IdPontoArmazenagem == model.CustomFilter.IdPontoArmazenagem) &&
                (model.CustomFilter.IdProduto.HasValue == false || x.IdProduto == model.CustomFilter.IdProduto.Value))
                .Select(s => new PosicaoInventarioListaLinhaTabela
                {
                    Codigo = s.EnderecoArmazenagem.Codigo,
                    Referencia = s.Produto.Referencia,
                    DescricaoProduto = s.Produto.Descricao,
                    IdLote = s.IdLote.Value,
                    QuantidadeProdutoPorEndereco = s.Quantidade,
                    IdProduto = s.IdProduto
                });

            totalRecordsFiltered = query.Count();

            query = query
               .OrderBy(model.OrderByColumn, model.OrderByDirection)
               .Skip(model.Start)
               .Take(model.Length);

            return query.ToList();
        }

        private IQueryable<RelatorioTotalizacaoLocalizacaoItem> BuscarDadosTotalizacaoLocalizacaoQuery(RelatorioTotalizacaoLocalizacaoFiltro model)
        {
            var baseQuery = Entities.LoteProdutoEndereco.AsNoTracking().Where(lpe =>
                                                               lpe.IdEmpresa == model.IdEmpresa &&
                                                               lpe.EnderecoArmazenagem.IdNivelArmazenagem == model.IdNivelArmazenagem &&
                                                               lpe.EnderecoArmazenagem.IdPontoArmazenagem == model.IdPontoArmazenagem
                                                            );

            if (model.CorredorInicial.HasValue && model.CorredorFinal.HasValue)
            {
                var range = Enumerable.Range(model.CorredorInicial.Value, model.CorredorFinal.Value).ToList();

                baseQuery = baseQuery.Where(y => range.Contains(y.EnderecoArmazenagem.Corredor));
            }

            var query = baseQuery.Select(bq => new RelatorioTotalizacaoLocalizacaoItem
            {
                CodigoEndereco = bq.EnderecoArmazenagem.Codigo,
                ReferenciaProduto = bq.Produto.Referencia,
                Unidade = bq.Produto.UnidadeMedida.Descricao,
                Quantidade = bq.Quantidade
            });

            return query;
        }

        public IEnumerable<RelatorioTotalizacaoLocalizacaoItem> BuscarDadosTotalizacaoLocalizacao(DataTableFilter<RelatorioTotalizacaoLocalizacaoFiltro> model, out int totalRecordsFiltered, out int totalRecords)
        {
            totalRecords = Entities.LoteProdutoEndereco.Count();

            var query = BuscarDadosTotalizacaoLocalizacaoQuery(model.CustomFilter);

            totalRecordsFiltered = query.Count();

            var response = query.OrderBy(model.OrderByColumn, model.OrderByDirection)
                                .Skip(model.Start)
                                .Take(model.Length);

            return response.ToList();
        }

        public List<RelatorioTotalizacaoLocalizacaoItem> BuscarDadosTotalizacaoLocalizacao(RelatorioTotalizacaoLocalizacaoFiltro model)
        {
            var query = BuscarDadosTotalizacaoLocalizacaoQuery(model);

            return query.ToList();
        }

        public IEnumerable<LoteProdutoEndereco> BuscarDadosLogisticaCorredor(DataTableFilter<RelatorioLogisticaCorredorListaFiltro> model, out int totalRecordsFiltered, out int totalRecords)
        {
            totalRecords = Entities.LoteProdutoEndereco
               .Where(x => x.IdEmpresa == model.CustomFilter.IdEmpresa).Count();

            IQueryable<LoteProdutoEndereco> query = Entities.LoteProdutoEndereco.Include("ProdutoEstoque").AsNoTracking()
               .Where(x => x.IdEmpresa == model.CustomFilter.IdEmpresa &&
               (x.EnderecoArmazenagem.IdNivelArmazenagem == model.CustomFilter.IdNivelArmazenagem) &&
               (x.EnderecoArmazenagem.IdPontoArmazenagem == model.CustomFilter.IdPontoArmazenagem));

            if (model.CustomFilter.CorredorInicial > 0 && model.CustomFilter.CorredorFinal > 0)
            {
                var range = Enumerable.Range(model.CustomFilter.CorredorInicial.Value, model.CustomFilter.CorredorFinal.Value);

                query = query.Where(y => range.Contains(y.EnderecoArmazenagem.Corredor));
            }

            if (model.CustomFilter.DataInicial.HasValue)
            {
                DateTime dataInicial = new DateTime(model.CustomFilter.DataInicial.Value.Year, model.CustomFilter.DataInicial.Value.Month, model.CustomFilter.DataInicial.Value.Day, 00, 00, 00);
                query = query.Where(x => x.DataHoraInstalacao >= dataInicial);
            }

            if (model.CustomFilter.DataFinal.HasValue)
            {
                DateTime dataFinal = new DateTime(model.CustomFilter.DataFinal.Value.Year, model.CustomFilter.DataFinal.Value.Month, model.CustomFilter.DataFinal.Value.Day, 23, 59, 59);
                query = query.Where(x => x.DataHoraInstalacao <= dataFinal);
            }

            if (model.CustomFilter.Ordenacao == 0)
            {
                query = query.OrderBy(x => x.EnderecoArmazenagem.Corredor).ThenBy(x => x.EnderecoArmazenagem.Codigo);
            }
            else if (model.CustomFilter.Ordenacao == 1)
            {
                query = query.OrderBy(x => x.ProdutoEstoque.Saldo).ThenBy(x => x.EnderecoArmazenagem.Codigo);
            }
            //else
            //{
            //    query = query.OrderBy(x => x.EnderecoArmazenagem)
            //}

            totalRecordsFiltered = query.Count();

            query = query
               .Skip(model.Start)
               .Take(model.Length);

            return query.ToList();
        }
    }
}