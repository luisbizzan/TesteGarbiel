using FWLog.Data.Models;
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
    }
}