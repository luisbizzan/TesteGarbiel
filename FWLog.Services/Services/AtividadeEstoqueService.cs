using FWLog.Data;

namespace FWLog.Services.Services
{
    public class AtividadeEstoqueService : BaseService
    {
        private readonly UnitOfWork _unitOfWork;

        public AtividadeEstoqueService(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
    }
}
