using DartDigital.Library.Mail;
using FWLog.Data;
using FWLog.Data.Models.GeneralCtx;
using FWLog.Services.Interfaces;
using FWLog.Services.Templates;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Res = FWLog.Services.GlobalResources.General.GeneralStrings;

namespace FWLog.Services.Services
{
    /// <summary>
    /// Serviço responsável pelo gerenciamento de usuários do backoffice.
    /// </summary>
    public class BOAccountService
    {
        private IBOAccountContentProvider _boAccountContentProvider;
        private UnitOfWork _uow;

        public BOAccountService(UnitOfWork uow, IBOAccountContentProvider contentProvider)
        {
            _uow = uow;
            _boAccountContentProvider = contentProvider;
        }

        public void SendRecoverPasswordMail(string emailFrom, string emailTo, string url)
        {
            var template = new RecoverPasswordMailTemplate(nome: emailTo, link: url, logoUrl: _boAccountContentProvider.GetLogoUrl());

            var client = new MailClient();

            var mailParams = new SendMailParams
            {
                Subject = Res.RecoverPasswordEmailSubject,
                Body = template.GetHtml(),
                IsBodyHtml = true,
                EmailFrom = emailFrom,
                EmailsTo = emailTo,
                Attachments = null,
                BodyEncoding = Encoding.Default
            };

            client.SendMail(mailParams);
        }

        public void EditPerfilUsuario(PerfilUsuario perfilModel)
        {
            var perfil = _uow.PerfilUsuarioRepository.GetById(perfilModel.PerfilUsuarioId);

            perfil.Departamento = perfilModel.Departamento;
            perfil.Cargo = perfilModel.Cargo;
            perfil.DataNascimento = perfilModel.DataNascimento;
            perfil.EmpresaId = perfilModel.EmpresaId;
            perfil.Nome = perfilModel.Nome;
            perfil.Ativo = perfilModel.Ativo;

            _uow.SaveChanges();
        }

        public void EditUsuarioEmpresas(IEnumerable<CompanySelectedItem> empresasUserOn, List<long> empresasUserEdit, string userId)
        {
            var empOld = _uow.UserCompanyRepository.GetAllCompaniesByUserId(userId);

            empOld = empresasUserOn.Where(w => empOld.Contains(w.CompanyId)).Select(s => s.CompanyId).ToList();

            List<long> empAdd = empresasUserEdit.Where(x => !empOld.Any(y => y == x)).ToList();
            List<long> empRem = empOld.Where(x => !empresasUserEdit.Any(y => y == x)).ToList();

            empAdd.ForEach(x => _uow.UserCompanyRepository.Add(new UserCompany(userId, x)));
            empRem.ForEach(x => _uow.UserCompanyRepository.DeleteByUserId(userId, x));

            _uow.SaveChanges();
        }
    }
}
