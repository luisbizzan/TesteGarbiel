using FWLog.Data.Models;
using FWLog.Data.Repository.CommonCtx;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace FWLog.Data.Repository.GeneralCtx
{
    public class NotaFiscalRepository : GenericRepository<NotaFiscal>
    {
        public NotaFiscalRepository(Entities entities) : base(entities) { }
               
        public NotaFiscal ObterPorCodigoIntegracao(long codigoIntegracao)
        {
            return Entities.NotaFiscal.FirstOrDefault(f => f.CodigoIntegracao == codigoIntegracao);
        }
              
        public NotaFiscal ObterPorChave(string chaveAcesso)
        {
            return Entities.NotaFiscal.FirstOrDefault(f => f.ChaveAcesso == chaveAcesso);
        }

        public Task<List<NotaFiscal>> ConsultarProcessamentoAutomatico()
        {
            return Entities.NotaFiscal
                .Where(w => w.IdNotaFiscalStatus == NotaFiscalStatusEnum.AguardandoRecebimento && 
                       w.Empresa.Ativo && 
                       w.Empresa.EmpresaConfig.CNPJConferenciaAutomatica == w.Fornecedor.CNPJ)
                .ToListAsync();
        }
    }
}
