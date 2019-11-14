using FWLog.Data;
using FWLog.Data.EnumsAndConsts;
using FWLog.Data.Models;
using System;
using System.Threading.Tasks;
using System.Transactions;

namespace FWLog.Services.Services
{
    public class LoteService
    {
        private UnitOfWork _uow;

        public LoteService(UnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task RegistrarRecebimentoNotaFiscal(long idNotaFiscal, string userId, DateTime dataRecebimento, int qtdVolumes)
        {
            Lote lote = _uow.LoteRepository.GetById(idNotaFiscal);

            if (lote != null)
            {
                return;
            }

            lote = new Lote();

            lote.IdLoteStatus = LoteStatusEnum.Recebido.GetHashCode();
            lote.IdNotaFiscal = idNotaFiscal;
            lote.DataRecebimento = dataRecebimento;
            lote.IdUsuarioRecebimento = userId;
            lote.QuantidadeVolume = qtdVolumes;

            _uow.LoteRepository.Add(lote);
            _uow.SaveChanges();

            var notaService = new NotaFiscalService(_uow);

            //try
            //{
            //    await notaService.AtualizarStatusNota(idNotaFiscal, LoteStatusEnum.Recebido.GetHashCode());
            //}
            //catch (Exception e)
            //{
            //    _uow.LoteRepository.Delete(lote);
            //    _uow.SaveChanges();

            //    throw e;
            //}
        }
    }
}
