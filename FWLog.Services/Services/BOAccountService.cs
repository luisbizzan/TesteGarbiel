﻿using DartDigital.Library.Mail;
using FWLog.Data;
using FWLog.Data.Models;
using FWLog.Data.Models.GeneralCtx;
using FWLog.Services.Interfaces;
using FWLog.Services.Model;
using FWLog.Services.Model.Usuario;
using FWLog.Services.Templates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Res = FWLog.Services.GlobalResources.General.GeneralStrings;

namespace FWLog.Services.Services
{
    /// <summary>
    /// Serviço responsável pelo gerenciamento de usuários do backoffice.
    /// </summary>
    public class BOAccountService
    {
        private readonly IBOAccountContentProvider _boAccountContentProvider;
        private readonly UnitOfWork _uow;
        private readonly AccountService _accountService;

        public BOAccountService(UnitOfWork uow, 
            IBOAccountContentProvider contentProvider,
            AccountService accountService)
        {
            _uow = uow;
            _boAccountContentProvider = contentProvider;
            _accountService = accountService;
        }

        public void SendRecoverPasswordMail(string emailFrom, string emailTo, string url)
        {
            var template = new RecoverPasswordMailTemplate(nome: emailTo, link: url, logoUrl: _boAccountContentProvider.GetLogoUrl());

            var client = new MailClient();

            var mailParams = new SendMailParams
            {
                Subject = "Recuperação de Senha",
                Body = template.GetHtml(),
                IsBodyHtml = true,
                EmailFrom = "FwLog Sistema <" + emailFrom + ">",
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

        public void EditUsuarioEmpresas(IEnumerable<EmpresaSelectedItem> empresasUserOn, List<impressorapadrao> empresasUserEdit, string userId, long perfilUsuarioId)
        {
            var empOld = _uow.UsuarioEmpresaRepository.GetAllEmpresasByUserId(userId);

            empOld = empresasUserOn.Where(w => empOld.Contains(w.IdEmpresa)).Select(s => s.IdEmpresa).ToList();

            List<impressorapadrao> empAdd = empresasUserEdit.Where(x => !empOld.Any(y => y == x.IdEmpresa)).ToList();
            List<long> empRem = empOld.Where(x => !empresasUserEdit.Any(y => y.IdEmpresa == x)).ToList();
            var empEdit = empOld.Where(x => !empAdd.Any(y => y.IdEmpresa == x) && !empRem.Any(y => y == x));

            empAdd.ForEach(x => _uow.UsuarioEmpresaRepository.Add(new UsuarioEmpresa { UserId = userId, IdEmpresa = x.IdEmpresa, PerfilUsuarioId = perfilUsuarioId, IdPerfilImpressoraPadrao = x.IdPerfilImpressoraPadrao }));
            empEdit.ForEach(x =>
            {
                var usuarioEmpresa = _uow.UsuarioEmpresaRepository.Tabela().FirstOrDefault(y => y.IdEmpresa == x && y.UserId == userId);
                usuarioEmpresa.IdPerfilImpressoraPadrao = empresasUserEdit.FirstOrDefault(y => y.IdEmpresa == x)?.IdPerfilImpressoraPadrao;
                _uow.UsuarioEmpresaRepository.Update(usuarioEmpresa);
            });

            empRem.ForEach(x => _uow.UsuarioEmpresaRepository.DeleteByUserId(userId, x));

            _uow.SaveChanges();
        }

        public class impressorapadrao
        {
            public long IdEmpresa { get; set; }

            public long? IdPerfilImpressoraPadrao { get; set; }
        }
    }
}
