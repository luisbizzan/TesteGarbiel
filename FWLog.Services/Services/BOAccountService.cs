using DartDigital.Library.Mail;
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

        public void EditUsuarioEmpresas(IEnumerable<EmpresaSelectedItem> empresasUserOn, List<UsuarioEmpresaUpdateFields> empresasUserEdit, string userId, long perfilUsuarioId)
        {
            var empOld = _uow.UsuarioEmpresaRepository.GetAllEmpresasByUserId(userId);

            empOld = empresasUserOn.Where(w => empOld.Contains(w.IdEmpresa)).Select(s => s.IdEmpresa).ToList();

            List<UsuarioEmpresaUpdateFields> empresasAdd = empresasUserEdit.Where(x => !empOld.Any(y => y == x.IdEmpresa)).ToList();
            List<long> empRem = empOld.Where(x => !empresasUserEdit.Any(y => y.IdEmpresa == x)).ToList();
            var empEdit = empOld.Where(x => !empresasAdd.Any(y => y.IdEmpresa == x) && !empRem.Any(y => y == x));

            foreach (var empresa in empresasAdd)
            {
                var newUsuarioEmpresa = new UsuarioEmpresa 
                { 
                    UserId = userId, 
                    IdEmpresa = empresa.IdEmpresa, 
                    PerfilUsuarioId = perfilUsuarioId, 
                    IdPerfilImpressoraPadrao = empresa.IdPerfilImpressoraPadrao,
                    CorredorEstoqueInicio = empresa.CorredorEstoqueInicio,
                    CorredorEstoqueFim = empresa.CorredorEstoqueFim,
                    CorredorSeparacaoInicio = empresa.CorredorSeparacaoInicio,
                    CorredorSeparacaoFim = empresa.CorredorSeparacaoFim,
                };
                _uow.UsuarioEmpresaRepository.Add(newUsuarioEmpresa);
            }

            empEdit.ForEach(x =>
            {
                var usuarioEmpresa = _uow.UsuarioEmpresaRepository.Tabela().FirstOrDefault(y => y.IdEmpresa == x && y.UserId == userId);
                usuarioEmpresa.IdPerfilImpressoraPadrao = empresasUserEdit.FirstOrDefault(y => y.IdEmpresa == x)?.IdPerfilImpressoraPadrao;
                usuarioEmpresa.CorredorEstoqueInicio = empresasUserEdit.FirstOrDefault(y => y.IdEmpresa == x)?.CorredorEstoqueInicio;
                usuarioEmpresa.CorredorEstoqueFim = empresasUserEdit.FirstOrDefault(y => y.IdEmpresa == x)?.CorredorEstoqueFim;
                usuarioEmpresa.CorredorSeparacaoInicio = empresasUserEdit.FirstOrDefault(y => y.IdEmpresa == x)?.CorredorSeparacaoInicio;
                usuarioEmpresa.CorredorSeparacaoFim = empresasUserEdit.FirstOrDefault(y => y.IdEmpresa == x)?.CorredorSeparacaoFim;

                _uow.UsuarioEmpresaRepository.Update(usuarioEmpresa);
            });

            empRem.ForEach(x => _uow.UsuarioEmpresaRepository.DeleteByUserId(userId, x));

            _uow.SaveChanges();
        }

        public class UsuarioEmpresaUpdateFields
        {
            public long IdEmpresa { get; set; }

            public long? IdPerfilImpressoraPadrao { get; set; }

            public int? CorredorEstoqueInicio { get; set; }

            public int? CorredorEstoqueFim { get; set; }

            public int? CorredorSeparacaoInicio { get; set; }

            public int? CorredorSeparacaoFim { get; set; }
        }
    }
}
