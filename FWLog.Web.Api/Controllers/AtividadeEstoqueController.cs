using FWLog.Data;
using FWLog.Services.Services;

namespace FWLog.Web.Api.Controllers
{
    public class AtividadeEstoqueController : ApiBaseController
    {
        private AtividadeEstoqueService _atividadeEstoqueService;
        private UnitOfWork _unitOfWork;
        public AtividadeEstoqueController(UnitOfWork unitOfWork, AtividadeEstoqueService atividadeEstoqueService)
        {
            _unitOfWork = unitOfWork;
            _atividadeEstoqueService = atividadeEstoqueService;
        }
    }
}