using DartDigital.Library.Exceptions;
using FWLog.Data;
using FWLog.Data.Models;
using FWLog.Data.Models.DataTablesCtx;
using FWLog.Data.Models.FilterCtx;
using System.Collections.Generic;

namespace FWLog.Services.Services
{
    public class CaixaRecusaService : BaseService
    {
        private readonly UnitOfWork _unitOfWork;

        public CaixaRecusaService(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public List<CaixaRecusaListaTabela> BuscarLista(DataTableFilter<CaixaRecusaListaFiltro> filtro, out int registrosFiltrados, out int totalRegistros)
        {
            return _unitOfWork.CaixaRecusaRepository.BuscarLista(filtro, out registrosFiltrados, out totalRegistros);
        }

        public bool ExisteCaixaRecusa(long idEmpresa, long idCaixa)
        {
            var caixaRecusa = _unitOfWork.CaixaRecusaRepository.BuscarCaixaPorEmpresa(idEmpresa, idCaixa);

            if (caixaRecusa.Count > 0)
                return true;
            else
                return false;
        }

        public void Cadastrar(List<CaixaRecusa> caixaRecusa, long idEmpresa)
        {
            if (caixaRecusa== null)
                throw new BusinessException("Nenhum item identificado para cadastro da caixa de recusa.");

            //Capturo o id da primeira caixa (os outros são iguais).
            long idCaixa = caixaRecusa[0].IdCaixa;

            var caixaRecusaCadastrado = _unitOfWork.CaixaRecusaRepository.BuscarCaixaPorEmpresa(idEmpresa, idCaixa);

            _unitOfWork.CaixaRecusaRepository.DeleteRange(caixaRecusaCadastrado);

            _unitOfWork.CaixaRecusaRepository.AddRange(caixaRecusa);
            _unitOfWork.SaveChanges();
        }
    }
}
