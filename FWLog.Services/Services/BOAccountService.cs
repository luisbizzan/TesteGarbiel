﻿using FWLog.Data;
using FWLog.Data.Repository.BackofficeCtx;
using FWLog.Services.Interfaces;
using FWLog.Services.Templates;
using DartDigital.Library.Exceptions;
using DartDigital.Library.Mail;
using System;
using System.Collections.Generic;
using System.Text;

using Res = FWLog.Services.GlobalResources.General.GeneralStrings;
using System.Linq;
using FWLog.Data.Models.GeneralCtx;

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

            _uow.SaveChanges();
        }

        public void EditUsuarioEmpresas(IEnumerable<EmpresaSelectedItem> empresasUserOn, List<long> empresasUserEdit, string userId)
        {
            var empOld = _uow.UsuarioEmpresaRepository.GetAllEmpresasByUserId(userId);

            empOld = empresasUserOn.Where(w => empOld.Contains(w.IdEmpresa)).Select(s => s.IdEmpresa).ToList();

            List<long> empAdd = empresasUserEdit.Where(x => !empOld.Any(y => y == x)).ToList();
            List<long> empRem = empOld.Where(x => !empresasUserEdit.Any(y => y == x)).ToList();

            empAdd.ForEach(x => _uow.UsuarioEmpresaRepository.Add(new UsuarioEmpresa(userId, x)));
            empRem.ForEach(x => _uow.UsuarioEmpresaRepository.DeleteByUserId(userId, x));

            _uow.SaveChanges();
        }
    }
}
