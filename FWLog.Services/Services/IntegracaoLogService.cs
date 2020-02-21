using AutoMapper;
using FWLog.Data;
using FWLog.Data.Models;
using FWLog.Services.Model.LogEtiquetagem;
using System;

namespace FWLog.Services.Services
{
    public class IntegracaoLogService
    {
        private readonly UnitOfWork _unitOfWork;

        public IntegracaoLogService(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void Registrar(IntegracaoLog integracaoLog)
        {
            _unitOfWork.IntegracaoLogRepository.Add(integracaoLog);

            _unitOfWork.SaveChanges();
        }
    }
}
