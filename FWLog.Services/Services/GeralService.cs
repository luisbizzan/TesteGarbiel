﻿using FWLog.Data;
using FWLog.Data.Models;
using System.Collections.Generic;

namespace FWLog.Services.Services
{
    public class GeralService
    {
        private readonly UnitOfWork _uow;

        public GeralService(UnitOfWork uow)
        {
            _uow = uow;
        }

        public void InserirHistorico(GeralHistorico item)
        {
            _uow.GeralRepository.InserirHistorico(item);
        }

        public List<GeralHistorico> TodosHistoricosDaCategoria(long Id_Categoria, long Id_Ref)
        {
            return _uow.GeralRepository.TodosHistoricosDaCategoria(Id_Categoria, Id_Ref);
        }
    }
}