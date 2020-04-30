using FWLog.Data.Models;
using FWLog.Data.Repository.CommonCtx;

namespace FWLog.Data.Repository.GeneralCtx
{
    public class TransportadoraEnderecoRepository : GenericRepository<TransportadoraEndereco>
    {
        public TransportadoraEnderecoRepository(Entities entities) : base(entities)
        {

        }
    }
}
