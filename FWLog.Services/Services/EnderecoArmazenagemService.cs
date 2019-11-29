using FWLog.Data;
using FWLog.Data.Models;

namespace FWLog.Services.Services
{
    public class EnderecoArmazenagemService
    {
        private readonly UnitOfWork _unitOfWork;

        public EnderecoArmazenagemService(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public EnderecoArmazenagem Cadastrar(EnderecoArmazenagem enderecoArmazenagem)
        {
            enderecoArmazenagem.Codigo = enderecoArmazenagem.Codigo.ToUpper();

            string[] endereco = enderecoArmazenagem.Codigo.Split('.');
            enderecoArmazenagem.Corredor = int.Parse(endereco[0]);
            enderecoArmazenagem.Horizontal = endereco[1];
            enderecoArmazenagem.Vertical = int.Parse(endereco[2]);
            enderecoArmazenagem.Divisao = int.Parse(endereco[3]);

            _unitOfWork.EnderecoArmazenagemRepository.Add(enderecoArmazenagem);
            _unitOfWork.SaveChanges();

            return enderecoArmazenagem;
        }

        public void Excluir(long idEnderecoArmazenagem)
        {
            EnderecoArmazenagem enderecoArmazenagem = _unitOfWork.EnderecoArmazenagemRepository.GetById(idEnderecoArmazenagem);

            _unitOfWork.EnderecoArmazenagemRepository.Delete(enderecoArmazenagem);
            _unitOfWork.SaveChanges();
        }

        public void Editar(EnderecoArmazenagem enderecoArmazenagem)
        {
            enderecoArmazenagem.Codigo = enderecoArmazenagem.Codigo.ToUpper();

            string[] endereco = enderecoArmazenagem.Codigo.Split('.');
            enderecoArmazenagem.Corredor = int.Parse(endereco[0]);
            enderecoArmazenagem.Horizontal = endereco[1];
            enderecoArmazenagem.Vertical = int.Parse(endereco[2]);
            enderecoArmazenagem.Divisao = int.Parse(endereco[3]);

            _unitOfWork.EnderecoArmazenagemRepository.Update(enderecoArmazenagem);
            _unitOfWork.SaveChanges();
        }
    }
}
