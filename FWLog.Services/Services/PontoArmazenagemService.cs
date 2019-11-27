﻿using FWLog.Data;
using FWLog.Data.Models;

namespace FWLog.Services.Services
{
    public class PontoArmazenagemService
    {
        private readonly UnitOfWork _unitOfWork;

        public PontoArmazenagemService(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public PontoArmazenagem Cadastrar(PontoArmazenagem pontoArmazenagem)
        {
            _unitOfWork.PontoArmazenagemRepository.Add(pontoArmazenagem);

            return pontoArmazenagem;
        }
    }
}
