using FWLog.Data;
using FWLog.Data.EnumsAndConsts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FWLog.Services.Services
{
    public class LoteService
    {
        private UnitOfWork _uow;

        public LoteService(UnitOfWork uow)
        {
            _uow = uow;
        }

        public void RegistrarRecebimentoNotaFiscal(long idLote, string userId)
        {
            var lote = _uow.LoteRepository.GetById(idLote);

            lote.IdLoteStatus = LoteStatusEnum.Recebido.GetHashCode();

            _uow.SaveChanges();


            //TODO Chamar Sankhya
        }
    }
}
