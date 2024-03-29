﻿using DartDigital.Library.Web.ActionFilters;
using FWLog.AspNet.Identity;
using FWLog.Data;
using FWLog.Data.Models;
using FWLog.Services.Services;
using FWLog.Web.Backoffice.Helpers;
using FWLog.Web.Backoffice.Models.BOAccountBaseCtx;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Configuration;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using Res = Resources.BOAccountBaseStrings;

namespace FWLog.Web.Backoffice.Controllers
{
    [AllowAnonymous]
    public class BOAccountBaseController : BOBaseController
    {
        private UnitOfWork _uow;
        private BOAccountService _boAccountService;

        public BOAccountBaseController(UnitOfWork uow, BOAccountService boAccountService)
        {
            _uow = uow;
            _boAccountService = boAccountService;
        }

        [HttpGet]
        [AnonymousOnly]
        public ActionResult LogOn(string l = null)
        {
            if (!string.IsNullOrEmpty(l))
            {
                CultureManager.SetCulture(Thread.CurrentThread, HttpContext, l);
            }

            var model = new LogOnViewModel
            {
                LanguageSelectList = GetLanguageSelectList(),
                CurrentLanguage = CultureManager.GetCulture(Thread.CurrentThread).Name
            };

            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> LogOn(LogOnViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                model.LanguageSelectList = GetLanguageSelectList();
                model.CurrentLanguage = CultureManager.GetCulture(Thread.CurrentThread).Name;
                return View(model);
            }

            DateTime loginAttempUtc = DateTime.UtcNow;
            var result = await SignInManager.PasswordSignInAsync(model.UserName, model.Password, false, shouldLockout: true).ConfigureAwait(false);

            switch (result)
            {
                case SignInStatus.Success:
                    ApplicationUser applicationUser = await UserManager.FindByNameAsync(model.UserName).ConfigureAwait(false);
                    PerfilUsuario perfilUsuario = _uow.PerfilUsuarioRepository.GetByUserId(applicationUser.Id);

                    if (perfilUsuario.Ativo)
                    {
                        if (_uow.EmpresaRepository.RetornarEmpresaPrincipal(applicationUser.Id) > 0)
                        {
                            await CreateApplicationSession(model.UserName).ConfigureAwait(false);
                            return RedirectToLocal(returnUrl);
                        }
                        else
                        {
                            model.ErrorMessage = Res.UserEmpresaError;
                        }
                    }
                    else
                    {
                        model.ErrorMessage = "O usuário informado não está ativo no sistema.";
                    }
                    
                    break;
                case SignInStatus.LockedOut:
                    model.ErrorMessage = await GetLogOnLockoutMessageAsync(loginAttempUtc, model.UserName).ConfigureAwait(false);
                    break;
                case SignInStatus.RequiresVerification:
                    model.ErrorMessage = Res.SignInRequiresVerificationMessage;
                    break;
                case SignInStatus.Failure:
                default:
                    model.ErrorMessage = Res.InvalidUserMessage;
                    break;
            }

            model.LanguageSelectList = GetLanguageSelectList();
            model.CurrentLanguage = CultureManager.GetCulture(Thread.CurrentThread).Name;
            model.Password = string.Empty;

            return View(model);
        }

        [HttpGet]
        [AnonymousOnly]
        public ActionResult RecoverPassword()
        {
            return View();
        }

        [HttpPost]
        [AnonymousOnly]
        public async Task<ActionResult> RecoverPassword(RecoverPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            ApplicationUser user = await UserManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                ModelState.AddModelError(nameof(model.Email), Res.EmailNotRegisteredMessage);
                return View(model);
            }

            string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
            string url = Url.Action("RedefinePassword", "BOAccountBase", new { userId = user.Id, token = code }, this.Request.Url.Scheme);

            string mailFrom = ConfigurationManager.AppSettings["EmailFromRecoverPassword"];
            _boAccountService.SendRecoverPasswordMail(mailFrom, user.Email, url);

            model.SuccessMessage = Res.RecoverPasswordMailSentMessage;
            return View(model);
        }

        [HttpGet]
        [AnonymousOnly]
        public async Task<ActionResult> RedefinePassword(string userId, string token)
        {
            bool valid = await UserManager.VerifyUserTokenAsync(userId, "ResetPassword", token);

            if (!valid)
            {
                Notify.Error(Res.PasswordRecoveryLinkInvalidMessage);
                return Redirect("LogOn");
            }

            var model = new RedefinePasswordViewModel()
            {
                Token = token,
                UserId = userId

            };

            return View(model);
        }

        [HttpPost]
        [AnonymousOnly]
        public async Task<ActionResult> RedefinePassword(RedefinePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            bool valid = await UserManager.VerifyUserTokenAsync(model.UserId, "ResetPassword", model.Token);

            if (!valid)
            {
                Notify.Error(Res.PasswordRecoveryLinkInvalidMessage);
                return Redirect("LogOn");
            }

            IdentityResult result = await UserManager.ResetPasswordAsync(model.UserId, model.Token, model.NewPassword);

            if (!result.Succeeded)
            {
                Notify.Error(Resources.CommonStrings.RequestUnexpectedErrorMessage);
                return View(model);
            }

            model.SuccessMessage = Res.PasswordChangeSuccessMessage;
            return View(model);
        }

        private SelectList GetLanguageSelectList()
        {
            return new SelectList(
                _uow.ApplicationLanguageRepository.GetAllActive().Select(x => new SelectListItem
                {
                    Value = x.CultureName,
                    Text = x.DisplayName,
                }), "Value", "Text"
            );

        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "BOHome");
        }

        private async Task<string> GetLogOnLockoutMessageAsync(DateTime loginAttemptUtc, string userName)
        {
            ApplicationUser user = await UserManager.FindByNameAsync(userName);
            DateTimeOffset lockoutEnd = await UserManager.GetLockoutEndDateAsync(user.Id);

            TimeSpan timeLeft = lockoutEnd.ToUniversalTime() - loginAttemptUtc;
            int totalSeconds = (int)timeLeft.TotalSeconds;
            bool isExactMinute = totalSeconds % 60 == 0;
            int totalMinutes = (int)timeLeft.TotalMinutes;

            int displayMinutes = isExactMinute ? totalMinutes : (totalMinutes + 1);
            string message = string.Format(Res.SignInLockedOutMessage, displayMinutes);
            return message;

        }

        private async Task CreateApplicationSession(string userName)
        {
            ApplicationUser applicationUser = await UserManager.FindByNameAsync(userName);

            if (applicationUser == null)
            {
                return;
            }

            long idEmpresa = _uow.EmpresaRepository.RetornarEmpresaPrincipal(applicationUser.Id);

            var applicationSession = new ApplicationSession
            {
                IdAspNetUsers = applicationUser.Id,
                IdApplication = applicationUser.ApplicationId,
                DataLogin = DateTime.Now,
                DataUltimaAcao = DateTime.Now,
                IdEmpresa = idEmpresa
            };

            _uow.ApplicationSessionRepository.Add(applicationSession);
            _uow.SaveChanges();

            applicationUser.IdApplicationSession = applicationSession.IdApplicationSession;

            UserManager.Update(applicationUser);
        }
    }
}
