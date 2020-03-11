using FWLog.Data;
using FWLog.Data.Models;
using System;

namespace FWLog.Services.Services
{
    public class GarantiaService
    {
        UnitOfWork _uow;

        public GarantiaService(UnitOfWork uow)
        {
            _uow = uow;
        }

        public void Add(Garantia garantia)
        {
            // Adicionar validações se necessário...

            _uow.GarantiaRepository.Add(garantia);
            _uow.SaveChanges();
        }

        public void Edit(Garantia garantia)
        {
            // Adicionar validações se necessário...

            _uow.GarantiaRepository.Update(garantia);
            _uow.SaveChanges();
        }

        public void Delete(Garantia garantia)
        {
            // Adicionar validações se necessário...

            _uow.GarantiaRepository.Delete(garantia);
            _uow.SaveChanges();
        }

        public void CriarRecebimentoGarantia(long idNotaFiscal, string userId, string observacao, string informacaoTransportadora)
        {
            var garantia = new Garantia
            {
                IdGarantiaStatus = GarantiaStatusEnum.Recebido,
                IdNotaFiscal = idNotaFiscal,
                DataRecebimento = DateTime.Now,
                IdUsuarioConferente = userId,
                Observacao = observacao,
                InformacaoTransporte = informacaoTransportadora
            };

            _uow.GarantiaRepository.Add(garantia);
            _uow.SaveChanges();
        }
    }
}
