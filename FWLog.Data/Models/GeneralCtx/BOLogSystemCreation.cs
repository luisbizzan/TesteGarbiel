using FWLog.Data.EnumsAndConsts;
using System;

namespace FWLog.Data.Models.GeneralCtx
{
    public class BOLogSystemCreation
    {
        /// <summary>
        /// Quem executou a ação que gerou o log.
        /// </summary>
        public object UserId { get; set; }
        public string IP { get; set; }
        public ActionTypeNames ActionType { get; set; }

        public string EntityName { get; set; }
        public object OldEntity { get; set; }
        public object NewEntity { get; set; }
    }

    public class AspNetUsersLogSerializeModel
    {
        public string UserName { get; set; }
        public string EmpresaId { get; set; }
        public string Departamento { get; set; }
        public string Cargo { get; set; }
        public string DataNascimento { get; set; }
        public string Empresa { get; set; }
        public string Nome { get; set; }

        public AspNetUsersLogSerializeModel(string userName, PerfilUsuario perfil, string grupoEmpresas)
        {
            UserName = userName;

            if (perfil != null)
            {
                EmpresaId = perfil.EmpresaId.ToString();
                Departamento = perfil.Departamento;
                Cargo = perfil.Cargo;
                DataNascimento = perfil.DataNascimento.ToString("dd/MM/yyyy");
                Nome = perfil.Nome;
            }

            if (!grupoEmpresas.NullOrEmpty())
            {
                Empresa = grupoEmpresas.Remove(grupoEmpresas.Length - 5);
            }
        }
    }

    public class AspNetRolesLogSerializeModel
    {
        public string Name { get; set; }

        public AspNetRolesLogSerializeModel(string name)
        {
            Name = name;
        }
    }
}
