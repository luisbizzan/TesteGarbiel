using FWLog.Data;
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
            _unitOfWork.SaveChanges();

            return pontoArmazenagem;
        }

        public PontoArmazenagem Editar(PontoArmazenagem pontoArmazenagem)
        {
            _unitOfWork.PontoArmazenagemRepository.Update(pontoArmazenagem);
            _unitOfWork.SaveChanges();

            return pontoArmazenagem;
        }

        public void Excluir(long idPontoArmazenagem)
        {
            PontoArmazenagem pontoArmazenagem = _unitOfWork.PontoArmazenagemRepository.GetById(idPontoArmazenagem);

            _unitOfWork.PontoArmazenagemRepository.Delete(pontoArmazenagem);
            _unitOfWork.SaveChanges();
        }
    }
}
