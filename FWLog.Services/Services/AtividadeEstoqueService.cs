using FWLog.Data;
using FWLog.Data.Models.DataTablesCtx;
using System.Collections.Generic;

namespace FWLog.Services.Services
{
    public class AtividadeEstoqueService : BaseService
    {
        private readonly UnitOfWork _unitOfWork;

        public AtividadeEstoqueService(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public List<AtividadeEstoqueListaLinhaTabela> PesquisarAtividade(long idEmpresa)
        {
            return _unitOfWork.AtividadeEstoqueRepository.PesquisarAtividade(idEmpresa);
        }
    }
}
