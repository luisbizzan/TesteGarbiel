using FWLog.Data;

namespace FWLog.Services.Services
{
    public class EnderecoArmazenagemService
    {
        private readonly UnitOfWork _unitOfWork;

        public EnderecoArmazenagemService(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
    }
}
