using DartDigital.Library.Exceptions;
using FWLog.Data;
using FWLog.Data.Models;
using System.Linq;

namespace FWLog.Services.Services
{
    public class TransportadoraEnderecoService : BaseService
    {
        private readonly UnitOfWork _unitOfWork;

        public TransportadoraEnderecoService(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void Cadastrar(TransportadoraEndereco transportadoraEndereco, long idEmpresa)
        {
            var endereco = _unitOfWork.EnderecoArmazenagemRepository.BuscarPorIdEPorEmpresa(transportadoraEndereco.IdEnderecoArmazenagem, idEmpresa);
            var transportadora = _unitOfWork.TransportadoraRepository.GetById(transportadoraEndereco.IdTransportadora);
            var transportadoraEnderecos = _unitOfWork.TransportadoraEnderecoRepository.ObterPorIdEmpresa(idEmpresa);

            var transportadoraComEnderecoVinculado = transportadoraEnderecos.FirstOrDefault(x => x.IdEnderecoArmazenagem == transportadoraEndereco.IdEnderecoArmazenagem);

            if (transportadoraComEnderecoVinculado != null)
                throw new BusinessException($"O endereço já se encontra vinculado a transportadora {transportadoraComEnderecoVinculado.Transportadora.RazaoSocial}");

            transportadoraEndereco.EnderecoArmazenagem = endereco ?? throw new BusinessException($"O endereço informado não foi localizado.");
            transportadoraEndereco.Transportadora = transportadora ?? throw new BusinessException($"A transportadora informada não foi localizada.");

            _unitOfWork.TransportadoraEnderecoRepository.Add(transportadoraEndereco);
            _unitOfWork.SaveChanges();
        }

        public void Editar(TransportadoraEndereco transportadoraEndereco, long idEmpresa)
        {
            var transportadoraEnderecoAntigo = _unitOfWork.TransportadoraEnderecoRepository.GetById(transportadoraEndereco.IdTransportadoraEndereco);

            if (transportadoraEnderecoAntigo == null)
            {
                throw new BusinessException("Transportadora x Endereço não encontrado");
            }

            if (transportadoraEnderecoAntigo.EnderecoArmazenagem.IdEmpresa != idEmpresa)
            {
                throw new BusinessException("Usuário não tem permissão para editar o Transportadora x Endereço");
            }

            var endereco = _unitOfWork.EnderecoArmazenagemRepository.BuscarPorIdEPorEmpresa(transportadoraEndereco.IdEnderecoArmazenagem, idEmpresa);
            var transportadora = _unitOfWork.TransportadoraRepository.GetById(transportadoraEndereco.IdTransportadora);
            var transportadoraEnderecos = _unitOfWork.TransportadoraEnderecoRepository.ObterPorIdEmpresa(idEmpresa);

            var transportadoraComEnderecoVinculado = transportadoraEnderecos.FirstOrDefault(x => x.IdEnderecoArmazenagem == transportadoraEndereco.IdEnderecoArmazenagem);

            if (transportadoraComEnderecoVinculado != null)
                throw new BusinessException($"O endereço já se encontra vinculado a transportadora {transportadoraComEnderecoVinculado.Transportadora.RazaoSocial}");

            transportadoraEnderecoAntigo.EnderecoArmazenagem = endereco ?? throw new BusinessException($"O endereço informado não foi localizado.");
            transportadoraEnderecoAntigo.Transportadora = transportadora ?? throw new BusinessException($"A transportadora informada não foi localizada.");
            transportadoraEnderecoAntigo.IdEnderecoArmazenagem = transportadoraEndereco.IdEnderecoArmazenagem;
            transportadoraEnderecoAntigo.IdTransportadora = transportadoraEndereco.IdTransportadora;

            _unitOfWork.TransportadoraEnderecoRepository.Update(transportadoraEnderecoAntigo);
            _unitOfWork.SaveChanges();
        }
    }
}
