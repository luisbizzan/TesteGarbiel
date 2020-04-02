using FWLog.Data.Models;
using FWLog.Data.Models.FilterCtx;
using FWLog.Data.Repository.CommonCtx;
using System.Collections.Generic;
using System.Linq;

namespace FWLog.Data.Repository.GeneralCtx
{
    public class LoteProdutoEnderecoRepository : GenericRepository<LoteProdutoEndereco>
    {
        public LoteProdutoEnderecoRepository(Entities entities) : base(entities) { }

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

        public IEnumerable<LoteProdutoEndereco> Teste(DataTableFilter<RelatorioTotalizacaoAlasListaFiltro> model, out int totalRecordsFiltered, out int totalRecords)
        {
            totalRecords = Entities.LoteProdutoEndereco.Where(x => x.IdEmpresa == model.CustomFilter.IdEmpresa && model.CustomFilter.ListaIdEnderecoArmazenagem.Contains(x.IdEnderecoArmazenagem)).Count();

            IQueryable<LoteProdutoEndereco> query =
                Entities.LoteProdutoEndereco.AsNoTracking().Where(
                    w => w.IdEmpresa == model.CustomFilter.IdEmpresa &&
                    model.CustomFilter.ListaIdEnderecoArmazenagem.Contains(w.IdEnderecoArmazenagem));


            if (!model.CustomFilter.ImprimirVazia)
            {
                query = query.Where(x => x.Quantidade > 0);
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