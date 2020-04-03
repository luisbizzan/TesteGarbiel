using FWLog.Data.Models;
using FWLog.Data.Repository.CommonCtx;
using System.Linq;

namespace FWLog.Data.Repository.GeneralCtx
{
    public class AtividadeEstoqueRepository : GenericRepository<AtividadeEstoque>
    {
        public AtividadeEstoqueRepository(Entities entities) : base(entities) { }

        public AtividadeEstoque Pesquisar(long idEmpresa, AtividadeEstoqueTipoEnum idAtividadeEstoqueTipo, long idEnderecoArmazenagem, long idProduto, bool finalizado )
        {
            return Entities.AtividadeEstoque.Where(x => x.IdEmpresa == idEmpresa && x.IdAtividadeEstoqueTipo == idAtividadeEstoqueTipo && x.IdEnderecoArmazenagem == x.IdEnderecoArmazenagem &&
            x.IdProduto == idProduto && x.Finalizado == finalizado).FirstOrDefault();
        }
    }
}
