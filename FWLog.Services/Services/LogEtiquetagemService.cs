using AutoMapper;
using FWLog.Data;
using FWLog.Services.Model.LogEtiquetagem;
using System;

namespace FWLog.Services.Services
{
    public class LogEtiquetagemService
    {
        private readonly UnitOfWork _unitOfWork;

        public LogEtiquetagemService(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void Registrar(LogEtiquetagem logEtiquetagem)
        {
            logEtiquetagem.DataHora = DateTime.Now;

            var log = new Data.Models.LogEtiquetagem()
            {
                IdEmpresa = logEtiquetagem.IdEmpresa,
                IdProduto = logEtiquetagem.IdProduto,
                IdTipoEtiquetagem = (Data.Models.TipoEtiquetagemEnum)logEtiquetagem.IdTipoEtiquetagem,
                DataHora = DateTime.Now,
                IdUsuario = logEtiquetagem.IdUsuario,
                Quantidade = logEtiquetagem.Quantidade                
            };

            _unitOfWork.LogEtiquetagemRepository.Add(log);

            _unitOfWork.SaveChanges();
        }
    }
}
