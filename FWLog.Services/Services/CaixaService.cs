using FWLog.Data;
using FWLog.Data.Models;
using FWLog.Data.Models.DataTablesCtx;
using FWLog.Data.Models.FilterCtx;
using System;
using System.Collections.Generic;

namespace FWLog.Services.Services
{
    public class CaixaService
    {
        private readonly UnitOfWork _unitOfWork;

        public CaixaService(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Caixa Cadastrar(Caixa caixa)
        {
            _unitOfWork.CaixaRepository.Add(caixa);
            _unitOfWork.SaveChanges();

            return caixa;
        }

        public List<CaixaListaTabela> BuscarLista(DataTableFilter<CaixaListaFiltro> filtro, out int registrosFiltrados, out int totalRegistros)
        {
            return _unitOfWork.CaixaRepository.BuscarLista(filtro, out registrosFiltrados, out totalRegistros);
        }

        //public void Excluir(long idCaixa)
        //{
        //    Caixa Caixa = _unitOfWork.CaixaRepository.GetById(idCaixa);

        //    _unitOfWork.CaixaRepository.Delete(Caixa);
        //    _unitOfWork.SaveChanges();
        //}

        //public void Editar(Caixa Caixa)
        //{
        //    _unitOfWork.CaixaRepository.Update(Caixa);
        //    _unitOfWork.SaveChanges();
        //}
    }
}