using AutoMapper;
using FWLog.AspNet.Identity;
using FWLog.Data;
using FWLog.Data.EnumsAndConsts;
using FWLog.Data.Models.FilterCtx;
using FWLog.Data.Models.GeneralCtx;
using FWLog.Services.Services;
using FWLog.Web.Backoffice.EnumsAndConsts;
using FWLog.Web.Backoffice.Helpers;
using FWLog.Web.Backoffice.Models.BOAccountCtx;
using FWLog.Web.Backoffice.Models.CommonCtx;
using DartDigital.Library.Web.Security.Backoffice;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Res = Resources.BOAccountStrings;

namespace FWLog.Web.Backoffice.Controllers
{
    public class BOAccountController : BOBaseController
    {
        UnitOfWork _uow;
        BOAccountService _boService;
        BOLogSystemService _boLogSystemService;
        PasswordService _passwordService;

        public BOAccountController(UnitOfWork uow, BOAccountService boService, BOLogSystemService boLogSystemService, PasswordService passwordService)
        {
            _uow = uow;
            _boService = boService;
            _boLogSystemService = boLogSystemService;
            _passwordService = passwordService;
        }

        [ApplicationAuthorize(Permissions = Permissions.BOAccount.List)]
        public ActionResult Index()
        {
            return View(new BOAccountListViewModel());
        }

        [ApplicationAuthorize(Permissions = Permissions.BOAccount.List)]
        public ActionResult PageData(DataTableFilter<BOAccountFilterViewModel> model)
        {
            int totalRecords = UserManager.Users.Count();

            IEnumerable<BOAccountListItemViewModel> query = UserManager.Users.Select(x => new BOAccountListItemViewModel
            {
                UserName = x.UserName,
                Email = x.Email
            });

            if (!String.IsNullOrEmpty(model.CustomFilter.UserName))
            {
                query = query.Where(x => x.UserName.Contains(model.CustomFilter.UserName));
            }

            if (!String.IsNullOrEmpty(model.CustomFilter.Email))
            {
                query = query.Where(x => x.Email.Contains(model.CustomFilter.Email));
            }

            int recordsFiltered = query.Count();

            query = query
                .OrderBy(model.OrderByColumn, model.OrderByDirection)
                .Skip(model.Start)
                .Take(model.Length);

            return DataTableResult.FromModel(new DataTableResponseModel
            {
                Draw = model.Draw,
                RecordsTotal = totalRecords,
                RecordsFiltered = recordsFiltered,
                Data = query.ToList()
            });
        }

        [ApplicationAuthorize(Permissions = Permissions.BOAccount.Create)]
        public ActionResult Create()
        {
            List<ApplicationRole> groups = RoleManager.Roles.OrderBy(x => x.Name).ToList();

            var model = new BOAccountCreateViewModel
            {
                Groups = Mapper.Map<List<GroupItemViewModel>>(groups)
            };

            return View(model);
        }


        [HttpPost]
        [ApplicationAuthorize(Permissions = Permissions.BOAccount.Create)]
        public async Task<ActionResult> Create(BOAccountCreateViewModel model)
        {
            Func<string, ViewResult> errorView = (error) =>
            {
                if (error != null)
                {
                    Notify.Error(Resources.CommonStrings.RequestUnexpectedErrorMessage);
                }

                List<ApplicationRole> groups = RoleManager.Roles.OrderBy(x => x.Name).ToList();
                model.Groups = Mapper.Map<List<GroupItemViewModel>>(groups);
                return View(model);
            };

            if (!ModelState.IsValid)
            {
                return errorView(null);
            }

            var existingUserByName = await UserManager.FindByNameAsync(model.UserName);

            if (existingUserByName != null)
            {
                ModelState.AddModelError(nameof(model.UserName), Res.UserNameAlreadyExistsMessage);
            }

            var existsUserByEmail = await UserManager.FindByEmailAsync(model.Email);

            if (existsUserByEmail != null)
            {
                ModelState.AddModelError(nameof(model.Email), Res.UserEmailAlreadyExistsMessage);
            }

            if (!ModelState.IsValid)
            {
                return errorView(null);
            }

            var user = new ApplicationUser { UserName = model.UserName, Email = model.Email };
            var result = await UserManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                throw new InvalidOperationException(Resources.CommonStrings.RequestUnexpectedErrorMessage);
            }

            IEnumerable<string> selectedRoles = model.Groups.Where(x => x.IsSelected).Select(x => x.Name);

            if (selectedRoles.Any())
            {
                result = await UserManager.AddToRolesAsync(user.Id, selectedRoles.ToArray());

                if (!result.Succeeded)
                {
                    Notify.Error(Resources.CommonStrings.RequestUnexpectedErrorMessage);
                    List<ApplicationRole> groups = RoleManager.Roles.OrderBy(x => x.Name).ToList();
                    model.Groups = Mapper.Map<List<GroupItemViewModel>>(groups);
                    return View(model);
                }
            }

            ///TODO Ajustar Log
            //var userInfo = new BackOfficeUserInfo();
            //_boLogSystemService.Add(new BOLogSystemCreation
            //{
            //    ActionType = ActionTypeNames.Add,
            //    IP = userInfo.IP,
            //    UserId = userInfo.UserId,
            //    EntityName = nameof(AspNetUsers),
            //    NewEntity = new AspNetUsersLogSerializeModel(user.UserName)
            //});

            Notify.Success(Resources.CommonStrings.RegisterCreatedSuccessMessage);
            return RedirectToAction("Index");
        }

        [ApplicationAuthorize(Permissions = Permissions.BOAccount.Edit)]
        public async Task<ActionResult> Edit(string id)
        {
            ApplicationUser user = await UserManager.FindByNameAsync(id);

            if (user == null)
            {
                throw new HttpException(404, "Not found");
            }

            IEnumerable<ApplicationRole> groups = RoleManager.Roles.OrderBy(x => x.Name);//aqui
            var model = Mapper.Map<BOAccountEditViewModel>(user);

            model.Groups = Mapper.Map<List<GroupItemViewModel>>(groups);

            IList<string> selectedRoles = await UserManager.GetRolesAsync(user.Id);

            foreach (GroupItemViewModel group in model.Groups)
            {
                if (selectedRoles.Contains(group.Name))
                    group.IsSelected = true;
            }

            return View(model);
        }

        [HttpPost]
        [ApplicationAuthorize(Permissions = Permissions.BOAccount.Edit)]
        public async Task<ActionResult> Edit(BOAccountEditViewModel model)
        {
            IEnumerable<string> selectedRoles = model.Groups.Where(x => x.IsSelected).Select(x => x.Name);

            Func<ViewResult> errorView = () =>
            {
                IEnumerable<ApplicationRole> groups = RoleManager.Roles.OrderBy(x => x.Name);
                model.Groups = Mapper.Map<List<GroupItemViewModel>>(groups);

                foreach (GroupItemViewModel group in model.Groups)
                {
                    if (selectedRoles.Contains(group.Name))
                        group.IsSelected = true;
                }

                return View(model);
            };

            if (!ModelState.IsValid)
            {
                return errorView();
            }

            ApplicationUser user = await UserManager.FindByNameAsync(model.UserName);

            if (user == null)
            {
                throw new HttpException(404, "Not found");
            }

            var existingUserByEmail = await UserManager.FindByEmailAsync(model.Email);

            if (existingUserByEmail != null && existingUserByEmail.Id != user.Id)
            {
                ModelState.AddModelError(nameof(model.Email), Res.UserEmailAlreadyExistsMessage);
                return errorView();
            }

            ApplicationUser oldUser = Mapper.Map<ApplicationUser>(user);
            user.Email = model.Email;
            IdentityResult result = await UserManager.UpdateAsync(user, selectedRoles, CompanyId);

            if (!result.Succeeded)
            {
                throw new InvalidOperationException(Resources.CommonStrings.RequestUnexpectedErrorMessage);
            }

            ///TODO Ajustar Log
            //var userInfo = new BackOfficeUserInfo();
            //_boLogSystemService.Add(new BOLogSystemCreation
            //{
            //    ActionType = ActionTypeNames.Edit,
            //    IP = userInfo.IP,
            //    UserId = userInfo.UserId,
            //    EntityName = nameof(AspNetUsers),
            //    OldEntity = new AspNetUsersLogSerializeModel(oldUser.UserName),
            //    NewEntity = new AspNetUsersLogSerializeModel(user.UserName)
            //});

            Notify.Success(Resources.CommonStrings.RegisterEditedSuccessMessage);
            return RedirectToAction("Index");
        }

        [ApplicationAuthorize(Permissions = Permissions.BOAccount.Edit)]
        public async Task<ActionResult> Details(string id)
        {
            ApplicationUser user = await UserManager.FindByNameAsync(id);

            if (user == null)
            {
                throw new HttpException(404, "Not found");
            }

            var model = Mapper.Map<BOAccountDetailsViewModel>(user);

            IEnumerable<ApplicationRole> groups = RoleManager.Roles.OrderBy(x => x.Name);

            model.Groups = Mapper.Map<List<GroupItemViewModel>>(groups);

            IEnumerable<string> selectedRoles = await UserManager.GetRolesAsync(user.Id);

            foreach (GroupItemViewModel group in model.Groups)
            {
                if (selectedRoles.Contains(group.Name))
                    group.IsSelected = true;
            }

            return View(model);
        }

        [HttpPost]
        [ApplicationAuthorize(Permissions = Permissions.BOAccount.Delete)]
        public async Task<JsonResult> AjaxDelete(string id)
        {
            try
            {
                ApplicationUser user = await UserManager.FindByNameAsync(id);

                if (user == null)
                {
                    throw new HttpException(404, "Not found");
                }

                IdentityResult result = await UserManager.DeleteAsync(user);

                if (!result.Succeeded)
                {
                    throw new InvalidOperationException(result.Errors.FirstOrDefault());
                }

                var userInfo = new BackOfficeUserInfo();
                _boLogSystemService.Add(new BOLogSystemCreation
                {
                    ActionType = ActionTypeNames.Delete,
                    IP = userInfo.IP,
                    UserId = userInfo.UserId,
                    EntityName = nameof(AspNetUsers),
                    NewEntity = new AspNetUsersLogSerializeModel(user.UserName)
                });

                return Json(new AjaxGenericResultModel
                {
                    Success = true,
                    Message = Resources.CommonStrings.RegisterDeletedSuccessMessage
                }, JsonRequestBehavior.DenyGet);
            }
            catch
            {
                return Json(new AjaxGenericResultModel
                {
                    Success = false,
                    Message = Resources.CommonStrings.RegisterHasRelationshipsErrorMessage
                }, JsonRequestBehavior.DenyGet);
            }
        }

        [HttpPost]
        [ApplicationAuthorize(Permissions = Permissions.BOAccount.Edit)]
        public async Task<JsonResult> AjaxResetPassword(string id)
        {
            try
            {
                ApplicationUser user = await UserManager.FindByNameAsync(id);

                if (user == null)
                {
                    throw new HttpException(404, "Not found");
                }

                ApplicationUser oldUser = Mapper.Map<ApplicationUser>(user);
                string randomPassword = _passwordService.GenerateRandomPassword();
                string resetToken = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                IdentityResult result = await UserManager.ResetPasswordAsync(user.Id, resetToken, randomPassword);

                if (!result.Succeeded)
                {
                    throw new InvalidOperationException(Resources.CommonStrings.RequestUnexpectedErrorMessage);
                }

                ///TODO Ajustar Log
                //var userInfo = new BackOfficeUserInfo();
                //_boLogSystemService.Add(new BOLogSystemCreation
                //{
                //    ActionType = ActionTypeNames.Edit,
                //    IP = userInfo.IP,
                //    UserId = userInfo.UserId,
                //    EntityName = nameof(AspNetUsers),
                //    OldEntity = new AspNetUsersLogSerializeModel(oldUser.UserName),
                //    NewEntity = new AspNetUsersLogSerializeModel(user.UserName)
                //});

                return Json(new AjaxGenericResultModel
                {
                    Success = true,
                    Message = String.Format(Res.UserPasswordResetSuccessMessage, user.UserName, randomPassword)
                }, JsonRequestBehavior.DenyGet);
            }
            catch
            {
                return Json(new AjaxGenericResultModel
                {
                    Success = false,
                    Message = Res.UserPasswordResetErrorMessage
                }, JsonRequestBehavior.DenyGet);
            }
        }

        [HttpPost]
        [ApplicationAuthorize(Permissions = Permissions.BOAccount.Delete)]
        public async Task<JsonResult> AjaxMassDelete(List<string> userNameList)
        {
            try
            {
                if (userNameList == null)
                {
                    throw new HttpException(400, "Bad request");
                }

                foreach (string userName in userNameList)
                {
                    ApplicationUser user = await UserManager.FindByNameAsync(userName);

                    if (user == null)
                    {
                        throw new Exception();
                    }

                    IdentityResult result = await UserManager.DeleteAsync(user);

                    if (!result.Succeeded)
                    {
                        throw new Exception();
                    }
                }

                return Json(new AjaxGenericResultModel
                {
                    Success = true,
                    Message = Resources.CommonStrings.RegisterMassDeleteSuccessMessage
                }, JsonRequestBehavior.DenyGet);
            }
            catch
            {
                return Json(new AjaxGenericResultModel
                {
                    Success = false,
                    Message = Resources.CommonStrings.RegisterHasRelationshipsErrorMessage
                }, JsonRequestBehavior.DenyGet);
            }
        }

        [ApplicationAuthorize]
        public ActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [ApplicationAuthorize]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            ApplicationUser user = await UserManager.FindByNameAsync(User.Identity.Name);
            IdentityResult result = await UserManager.ChangePasswordAsync(user.Id, model.OldPassword, model.NewPassword);

            if (!result.Succeeded)
            {
                ModelState.AddModelError(nameof(model.OldPassword), Res.CurrentPasswordInvalidMessage);
                return View(model);
            }

            Notify.Success(Res.PasswordChangedSuccessMessage);
            return RedirectToAction("Index", "BOHome");
        }

        public ActionResult LogOff()
        {
            ApplicationUser applicationUser = UserManager.FindByName(User.Identity.Name);

            if (applicationUser != null)
            {
                if (applicationUser.IdApplicationSession.HasValue)
                {
                    ApplicationSession applicationSession = _uow.ApplicationSessionRepository.GetById(applicationUser.IdApplicationSession.Value);

                    applicationSession.DataUltimaAcao = DateTime.Now;
                    applicationSession.DataLogout = DateTime.Now;

                    _uow.ApplicationSessionRepository.Update(applicationSession);
                    _uow.SaveChanges();

                    applicationUser.IdApplicationSession = null;
                    UserManager.Update(applicationUser);
                }
            }

            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("LogOn", "BOAccountBase");
        }
    }
}
