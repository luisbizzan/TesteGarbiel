using FWLog.Data;
using FWLog.Data.Models;
using System.Linq;

namespace FWLog.Services.Services
{
    public class PerfilUsuarioService
    {
        private UnitOfWork _unitiOfWork;

        public PerfilUsuarioService(UnitOfWork unitiOfWork)
        {
            _unitiOfWork = unitiOfWork;
        }

        public PerfilUsuario ObterPorUsuario(string idUsuario)
        {
            return _unitiOfWork.PerfilUsuarioRepository.Todos().Where(x => x.UsuarioId == idUsuario).FirstOrDefault();
        }

        public void CadastrarPerfilAdministrador(string idUsuario)
        {
            var empresa = _unitiOfWork.EmpresaConfigRepository.Todos().Where(x => x.IdEmpresaTipo == EmpresaTipoEnum.Matriz).FirstOrDefault();
            
            if (empresa != null)
            {
                var perfilUsuario = new PerfilUsuario()
                {
                    UsuarioId = idUsuario,
                    EmpresaId = empresa.IdEmpresa,
                    Nome = "Administrador",
                    DataNascimento = null,
                    Departamento = "",
                    Cargo = "",
                    Ativo = true
                };

                _unitiOfWork.PerfilUsuarioRepository.Add(perfilUsuario);

                _unitiOfWork.CommitWithoutLog();
            }

            var empresas = _unitiOfWork.EmpresaConfigRepository.Todos().ToList();
            var empresasUsuario = _unitiOfWork.UsuarioEmpresaRepository.GetAllEmpresasByUserId(idUsuario).ToList();
            var perfilUsuarioId = _unitiOfWork.PerfilUsuarioRepository.Todos().Where(x => x.UsuarioId == idUsuario).FirstOrDefault().PerfilUsuarioId;
            UsuarioEmpresa usuarioEmpresa;

            foreach (var item in empresas)
            {
                if (!empresasUsuario.Any(x=> x == item.IdEmpresa))
                {
                    usuarioEmpresa = new UsuarioEmpresa()
                    {
                        UserId = idUsuario,
                        IdEmpresa = item.IdEmpresa,
                        PerfilUsuarioId = perfilUsuarioId,
                        IdPerfilImpressoraPadrao = null
                    };

                    _unitiOfWork.UsuarioEmpresaRepository.Add(usuarioEmpresa);

                    _unitiOfWork.CommitWithoutLog();
                }
            }
            
        }
    }
}
