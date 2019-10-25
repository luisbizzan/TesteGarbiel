using AutoMapper;
using FWLog.AspNet.Identity;
using FWLog.Data;
using FWLog.Services.Services;
using FWLog.Web.Api.GlobalResources;
using FWLog.Web.Api.Models.Account;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using System.Linq;
using System;

namespace FWLog.Web.Api.Controllers
{
    public class AccountController : ApiBaseController
    {
        private AccountService _accountService;
        private UnitOfWork _unitOfWork;

        public AccountController(UnitOfWork unitOfWork, AccountService accountService)
        {
            _unitOfWork = unitOfWork;
            _accountService = accountService;
        }
                
        [AllowAnonymous]
        [HttpPost]
        [Route("api/v1/account/login")]
        public async Task<IHttpActionResult> Login(LoginModelRequest loginRequest)
        {
            if (!ModelState.IsValid)
            {
                return ApiBadRequest(ModelState);
            }

            SignInStatus signInResult = await SignInManager.PasswordSignInAsync(loginRequest.UserName, loginRequest.Password, false, shouldLockout: true);

            if (signInResult == SignInStatus.Failure)
            {
                return ApiBadRequest(AccountResource.InvalidUserOrPassword, loginRequest.UserName);
            }

            if (signInResult == SignInStatus.LockedOut)
            {
                return ApiForbidden(AccountResource.UserLockedOut, loginRequest.UserName);
            }

            ApplicationUser applicationUser = await UserManager.FindByNameAsync(loginRequest.UserName);
            IList<string> userPermissions = await UserManager.GetPermissionsAsync(applicationUser.Id, loginRequest.CompanyId);

            if (userPermissions == null || !userPermissions.Any(w => w.Equals("UserAppLogin", StringComparison.OrdinalIgnoreCase)))
            {
                return ApiForbidden(AccountResource.UserPermissionDenied, loginRequest.UserName);
            }

            var token = await _accountService.Token(loginRequest.UserName, loginRequest.Password);
            var response = Mapper.Map<LoginModelResponse>(token);

            var applicationSession = new ApplicationSession
            {
                DataLogin = DateTime.Now,
                DataUltimaAcao = DateTime.Now,
                IdApplication = 2,
                IdAspNetUsers = applicationUser.Id,
                CompanyId = loginRequest.CompanyId
            };

            _unitOfWork.ApplicationSessionRepository.Add(applicationSession);
            _unitOfWork.SaveChanges();

            applicationUser.IdApplicationSession = applicationSession.IdApplicationSession;

            UserManager.Update(applicationUser);

            return ApiOk(response);
        }

        [HttpGet]
        [Route("api/v1/account/permissions")]
        public async Task<IHttpActionResult> UserPermissions(int companyId)
        {
            IList<string> permissions = await UserManager.GetPermissionsAsync(User.Identity.GetUserId(), companyId);

            var permissionsResponse = new PermissionsModelResponse
            {
                Permissions = new List<string>(permissions)
            };

            return ApiOk(permissionsResponse);
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("api/v1/account/logout")]
        public async Task<IHttpActionResult> Logout()
        {
            ApplicationUser applicationUser = await UserManager.FindByNameAsync(User.Identity.Name);

            if (applicationUser != null)
            {
                if (applicationUser.IdApplicationSession.HasValue)
                {
                    ApplicationSession applicationSession = _unitOfWork.ApplicationSessionRepository.GetById(applicationUser.IdApplicationSession.Value);

                    applicationSession.DataLogout = DateTime.Now;
                    applicationSession.DataUltimaAcao = DateTime.Now;

                    _unitOfWork.ApplicationSessionRepository.Update(applicationSession);
                    _unitOfWork.SaveChanges();

                    applicationUser.IdApplicationSession = null;
                    UserManager.Update(applicationUser);
                }
            }

            AuthenticationManager.SignOut();
            return ApiOk();
        }
    }
}