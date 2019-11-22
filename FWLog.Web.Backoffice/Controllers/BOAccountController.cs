using AutoMapper;
using ExtensionMethods;
using FWLog.AspNet.Identity;
using FWLog.Data;
using FWLog.Data.EnumsAndConsts;
using FWLog.Data.Models.FilterCtx;
using FWLog.Data.Models.GeneralCtx;
using FWLog.Services.Services;
using FWLog.Web.Backoffice.EnumsAndConsts.LOVs;
using FWLog.Web.Backoffice.Helpers;
using FWLog.Web.Backoffice.Models.BOAccountCtx;
using FWLog.Web.Backoffice.Models.CommonCtx;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Res = Resources.BOAccountStrings;

namespace FWLog.Web.Backoffice.Controllers
{
    public class BOAccountController : BOBaseController
    {
        private UnitOfWork _uow;
        private BOAccountService _boService;
        private BOLogSystemService _boLogSystemService;
        private PasswordService _passwordService;

        private SelectList _Empresas
        {
            get
            {
                if (empresas == null)
                {
                    empresas = new SelectList(Empresas, "IdEmpresa", "Nome");
                }

                return empresas;
            }
        }
        private SelectList empresas;

        private SelectList Status
        {
            get
            {
                if (status == null)
                {
                    status = new SelectList(new NaoSimLOV().Items, "Value", "Text");
                }

                return status;
            }
        }
        private SelectList status;

        private void setViewBags()
        {
            ViewBag.Empresas = _Empresas;
            ViewBag.Status = Status;
        }

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
            setViewBags();

            return View(new BOAccountListViewModel());
        }

        [ApplicationAuthorize(Permissions = Permissions.BOAccount.List)]
        public ActionResult PageData(DataTableFilter<BOAccountFilterViewModel> filter)
        {
            IQueryable<PerfilUsuario> allusers = _uow.PerfilUsuarioRepository.All();

            int totalRecords = allusers.Count();

            IQueryable<PerfilUsuario> query = allusers.WhereIf(!string.IsNullOrEmpty(filter.CustomFilter.UserName), x => x.Usuario.UserName.Contains(filter.CustomFilter.UserName));
            query = query.WhereIf(!string.IsNullOrEmpty(filter.CustomFilter.Email), x => x.Usuario.Email.Contains(filter.CustomFilter.Email));
            query = query.WhereIf(!string.IsNullOrEmpty(filter.CustomFilter.Nome), x => x.Nome.Contains(filter.CustomFilter.Nome));
            query = query.WhereIf(filter.CustomFilter.IdEmpresa.HasValue, x => x.EmpresaId == filter.CustomFilter.IdEmpresa);
            query = query.WhereIf(filter.CustomFilter.Ativo.HasValue, x => x.Ativo == filter.CustomFilter.Ativo);

            IQueryable<BOAccountListItemViewModel> select = query.Select(x => new BOAccountListItemViewModel
            {
                UserName = x.Usuario.UserName,
                Email = x.Usuario.Email,
                Nome = x.Nome,
                _Ativo = x.Ativo
            });

            List<BOAccountListItemViewModel> list = select.ToList();

            return DataTableResult.FromModel(new DataTableResponseModel
            {
                Draw = filter.Draw,
                RecordsTotal = totalRecords,
                RecordsFiltered = list.Count,
                Data = list.PaginationResult(filter)
            });
        }

        [ApplicationAuthorize(Permissions = Permissions.BOAccount.Create)]
        public ActionResult Create()
        {
            setViewBags();

            return View(new BOAccountCreateViewModel());
        }

        public ActionResult AdicionarEmpresa(long idEmpresa)
        {
            List<ApplicationRole> groups = RoleManager.Roles.OrderBy(x => x.Name).ToList();

            var model = new BOAccountCreateViewModel();
            var empresasGrupos = new EmpresaGrupoViewModel
            {
                IdEmpresa = idEmpresa,
                Nome = Empresas.First(f => f.IdEmpresa == idEmpresa).Nome,
                Grupos = Mapper.Map<List<GroupItemViewModel>>(groups)
            };

            model.EmpresasGrupos.Add(empresasGrupos);

            return PartialView("_EmpresaGrupo", model.EmpresasGrupos);
        }

        [HttpPost]
        [ApplicationAuthorize(Permissions = Permissions.BOAccount.Create)]
        public async Task<ActionResult> Create(BOAccountCreateViewModel model)
        {
            setViewBags();

            Func<string, ViewResult> errorView = (error) =>
            {
                if (error != null)
                {
                    Notify.Error(Resources.CommonStrings.RequestUnexpectedErrorMessage);
                }
                return View(model);
            };

            if (!ModelState.IsValid)
            {
                return errorView(null);
            }

            var existingUserByName = await UserManager.FindByNameAsync(model.UserName).ConfigureAwait(false);

            if (existingUserByName != null)
            {
                ModelState.AddModelError(nameof(model.UserName), Res.UserNameAlreadyExistsMessage);
            }

            var existsUserByEmail = await UserManager.FindByEmailAsync(model.Email).ConfigureAwait(false);

            if (existsUserByEmail != null)
            {
                ModelState.AddModelError(nameof(model.Email), Res.UserEmailAlreadyExistsMessage);
            }

            if (model.EmpresasGrupos.NullOrEmpty())
            {
                ModelState.AddModelError(nameof(model.EmpresasGrupos), Res.RequiredOnlyCompany);
                return errorView(null);
            }

            if (!model.EmpresasGrupos.Where(w => w.Grupos.Any(a => a.IsSelected)).Any())
            {
                ModelState.AddModelError(nameof(model.EmpresasGrupos), Res.RequiredOnlyGroup);
                return errorView(null);
            }

            if (!ModelState.IsValid)
            {
                return errorView(null);
            }

            var user = new ApplicationUser { UserName = model.UserName, Email = model.Email };
            user.Id = Guid.NewGuid().ToString();
            var result = await UserManager.CreateAsync(user, model.Password).ConfigureAwait(false);

            if (!result.Succeeded)
            {
                throw new InvalidOperationException(Resources.CommonStrings.RequestUnexpectedErrorMessage);
            }

            model.PerfilUsuario.UsuarioId = user.Id;
            _uow.PerfilUsuarioRepository.Add(model.PerfilUsuario);
            _uow.SaveChanges();

            model.EmpresasGrupos.ForEach(f => _uow.UsuarioEmpresaRepository.Add(new UsuarioEmpresa(user.Id, f.IdEmpresa)));
            _uow.SaveChanges();

            var empresasGruposNew = new StringBuilder();

            foreach (var item in model.EmpresasGrupos.Where(x => x.Grupos.Any(y => y.IsSelected)))
            {
                IEnumerable<string> selectedRoles = item.Grupos.Select(x => x.Name);

                empresasGruposNew.AppendLine(string.Format("{0}: {1}", item.Name, string.Join(", ", selectedRoles.ToArray())));
                empresasGruposNew.AppendLine(" || ");

                result = UserManager.AddToRolesByCompany(user, selectedRoles.ToArray(), item.CompanyId);

                if (!result.Succeeded)
                {
                    Notify.Error(Resources.CommonStrings.RequestUnexpectedErrorMessage);

                    return View(model);
                }
            }

            var userInfo = new BackOfficeUserInfo();
            _boLogSystemService.Add(new BOLogSystemCreation
            {
                ActionType = ActionTypeNames.Add,
                IP = userInfo.IP,
                UserId = userInfo.UserId,
                EntityName = nameof(AspNetUsers),
                NewEntity = new AspNetUsersLogSerializeModel(user.UserName, model.PerfilUsuario, empresasGruposNew.ToString())
            });

            Notify.Success(Resources.CommonStrings.RegisterCreatedSuccessMessage);
            return RedirectToAction("Index");
        }

        [ApplicationAuthorize(Permissions = Permissions.BOAccount.Edit)]
        public async Task<ActionResult> Edit(string id)
        {
            setViewBags();

            ApplicationUser user = await UserManager.FindByNameAsync(id).ConfigureAwait(false);

            if (user == null)
            {
                throw new HttpException(404, "Not found");
            }

            var empresas = _uow.UsuarioEmpresaRepository.GetAllEmpresasByUserId(user.Id);

            empresas = Empresas.Where(w => empresas.Contains(w.IdEmpresa)).Select(s => s.IdEmpresa).ToList();

            var perfil = _uow.PerfilUsuarioRepository.GetByUserId(user.Id.ToString());

            var model = Mapper.Map<BOAccountEditViewModel>(user);
            model.PerfilUsuario = perfil;

            IEnumerable<ApplicationRole> groups = RoleManager.Roles.OrderBy(x => x.Name);

            foreach (long empresa in empresas)
            {
                var empGrupos = new EmpresaGrupoViewModel
                {
                    IdEmpresa = empresa,
                    Nome = Empresas.First(f => f.IdEmpresa == empresa).Nome,
                    Grupos = Mapper.Map<List<GroupItemViewModel>>(groups)
                };

                IList<string> selectedRoles = await UserManager.GetUserRolesByIdEmpresa(user.Id, empresa).ConfigureAwait(false);

                foreach (GroupItemViewModel group in empGrupos.Grupos.Where(x => selectedRoles.Contains(x.Name)))
                {
                    group.IsSelected = true;
                }

                model.EmpresasGrupos.Add(empGrupos);
            }

            return View(model);
        }

        [HttpPost]
        [ApplicationAuthorize(Permissions = Permissions.BOAccount.Edit)]
        public async Task<ActionResult> Edit(BOAccountEditViewModel model)
        {
            setViewBags();

            Func<ViewResult> errorView = () =>
            {
                return View(model);
            };

            if (!ModelState.IsValid)
            {
                return errorView();
            }

            ApplicationUser user = await UserManager.FindByNameAsync(model.UserName).ConfigureAwait(false);

            if (user == null)
            {
                throw new HttpException(404, "Not found");
            }

            var existingUserByEmail = await UserManager.FindByEmailAsync(model.Email).ConfigureAwait(false);

            if (existingUserByEmail != null && existingUserByEmail.Id != user.Id)
            {
                ModelState.AddModelError(nameof(model.Email), Res.UserEmailAlreadyExistsMessage);
                return errorView();
            }

            if (model.EmpresasGrupos.NullOrEmpty())
            {
                ModelState.AddModelError(nameof(model.EmpresasGrupos), Res.RequiredOnlyCompany);
                return errorView();
            }

            if (!model.EmpresasGrupos.Where(w => w.Grupos.Any(a => a.IsSelected)).Any())
            {
                ModelState.AddModelError(nameof(model.EmpresasGrupos), Res.RequiredOnlyGroup);
                return errorView();
            }

            ApplicationUser oldUser = Mapper.Map<ApplicationUser>(user);
            user.Email = model.Email;

            var empresasGruposNew = new StringBuilder();
            var empresasGruposOld = new StringBuilder();

            var companiesUser = _uow.UserCompanyRepository.GetAllCompaniesByUserId(user.Id);
            var companies = Companies.Where(w => companiesUser.Contains(w.CompanyId)).ToList();

            foreach (var company in companies)
            {
                IList<string> selectedRoles = await UserManager.GetUserRolesByIdEmpresa(user.Id, empresa.IdEmpresa).ConfigureAwait(false);

                if (selectedRoles.Any())
                {
                    empresasGruposOld.AppendLine(string.Format("{0}: {1}", empresa.Nome, string.Join(", ", selectedRoles.ToArray())));
                    empresasGruposOld.AppendLine(" || ");
                }
            }

            foreach (var item in model.EmpresasGrupos.Where(x => x.Grupos.Any(y => y.IsSelected)))
            {
                IEnumerable<string> selectedRoles = item.Grupos.Select(x => x.Name);

                empresasGruposNew.AppendLine(string.Format("{0}: {1}", item.Name, string.Join(", ", selectedRoles.ToArray())));
                empresasGruposNew.AppendLine(" || ");

                IdentityResult result = await UserManager.UpdateAsync(user, selectedRoles, item.CompanyId).ConfigureAwait(false);

                if (!result.Succeeded)
                {
                    throw new InvalidOperationException(Resources.CommonStrings.RequestUnexpectedErrorMessage);
                }
            }

            var oldPerfil = _uow.PerfilUsuarioRepository.GetById(model.PerfilUsuario.PerfilUsuarioId);
            var log = new AspNetUsersLogSerializeModel(oldUser.UserName, oldPerfil, empresasGruposOld.ToString());

            _boService.EditPerfilUsuario(model.PerfilUsuario);

            var empresasGrupos = model.EmpresasGrupos.Where(w => w.Grupos.Any(a => a.IsSelected)).Select(s => s.IdEmpresa).ToList();
            _boService.EditUsuarioEmpresas(Empresas, empresasGrupos, user.Id.ToString());

            var userInfo = new BackOfficeUserInfo();
            _boLogSystemService.Add(new BOLogSystemCreation
            {
                ActionType = ActionTypeNames.Edit,
                IP = userInfo.IP,
                UserId = userInfo.UserId,
                EntityName = nameof(AspNetUsers),
                OldEntity = log,
                NewEntity = new AspNetUsersLogSerializeModel(user.UserName, model.PerfilUsuario, empresasGruposNew.ToString())
            });

            Notify.Success(Resources.CommonStrings.RegisterEditedSuccessMessage);
            return RedirectToAction("Index");
        }

        [ApplicationAuthorize(Permissions = Permissions.BOAccount.Edit)]
        public async Task<ActionResult> Details(string id)
        {
            ApplicationUser user = await UserManager.FindByNameAsync(id).ConfigureAwait(false);

            if (user == null)
            {
                throw new HttpException(404, "Not found");
            }

            var model = Mapper.Map<BOAccountDetailsViewModel>(user);

            IEnumerable<ApplicationRole> groups = RoleManager.Roles.OrderBy(x => x.Name);

            model.Groups = Mapper.Map<List<GroupItemViewModel>>(groups);

            IEnumerable<string> selectedRoles = await UserManager.GetRolesAsync(user.Id).ConfigureAwait(false);

            foreach (GroupItemViewModel group in model.Groups.Where(x => selectedRoles.Contains(x.Name)))
            {
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
                ApplicationUser user = await UserManager.FindByNameAsync(id).ConfigureAwait(false);

                if (user == null)
                {
                    throw new HttpException(404, "Not found");
                }

                IdentityResult result = await UserManager.DeleteAsync(user).ConfigureAwait(false);

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
                    NewEntity = new AspNetUsersLogSerializeModel(user.UserName, null, string.Empty)
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
                ApplicationUser user = await UserManager.FindByNameAsync(id).ConfigureAwait(false);

                if (user == null)
                {
                    throw new HttpException(404, "Not found");
                }

                ApplicationUser oldUser = Mapper.Map<ApplicationUser>(user);
                string randomPassword = _passwordService.GenerateRandomPassword();
                string resetToken = await UserManager.GeneratePasswordResetTokenAsync(user.Id).ConfigureAwait(false);
                IdentityResult result = await UserManager.ResetPasswordAsync(user.Id, resetToken, randomPassword).ConfigureAwait(false);

                if (!result.Succeeded)
                {
                    throw new InvalidOperationException(Resources.CommonStrings.RequestUnexpectedErrorMessage);
                }

                var userInfo = new BackOfficeUserInfo();
                _boLogSystemService.Add(new BOLogSystemCreation
                {
                    ActionType = ActionTypeNames.Edit,
                    IP = userInfo.IP,
                    UserId = userInfo.UserId,
                    EntityName = nameof(AspNetUsers),
                    OldEntity = new AspNetUsersLogSerializeModel(oldUser.UserName, null, string.Empty),
                    NewEntity = new AspNetUsersLogSerializeModel(user.UserName, null, string.Empty)
                });

                return Json(new AjaxGenericResultModel
                {
                    Success = true,
                    Message = string.Format(Res.UserPasswordResetSuccessMessage, user.UserName, randomPassword)
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
                    ApplicationUser user = await UserManager.FindByNameAsync(userName).ConfigureAwait(false);

                    if (user == null)
                    {
                        throw new Exception();
                    }

                    IdentityResult result = await UserManager.DeleteAsync(user).ConfigureAwait(false);

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

            ApplicationUser user = await UserManager.FindByNameAsync(User.Identity.Name).ConfigureAwait(false);
            IdentityResult result = await UserManager.ChangePasswordAsync(user.Id, model.OldPassword, model.NewPassword).ConfigureAwait(false);

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

            CookieSalvarEmpresa(0, applicationUser.Id, true);

            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("LogOn", "BOAccountBase");
        }

        //[ApplicationAuthorize]
        public ActionResult SearchModal()
        {
            return View(new BOPerfilUsuarioSearchModalViewModel());
        }

        //[ApplicationAuthorize]
        public ActionResult SearchModalPageData(DataTableFilter<BOPerfilUsuarioSearchModalFilterViewModel> filter)
        {
            var query = _uow.PerfilUsuarioRepository.All();

            int totalRecords = query.Count();

            if (!string.IsNullOrEmpty(filter.CustomFilter.UserName))
                query = query.Where(x => x.Usuario.UserName.Contains(filter.CustomFilter.UserName));

            if (!string.IsNullOrEmpty(filter.CustomFilter.Departamento))
                query = query.Where(x => x.Departamento.Contains(filter.CustomFilter.Departamento));

            if (!string.IsNullOrEmpty(filter.CustomFilter.Cargo))
                query = query.Where(x => x.Cargo.Contains(filter.CustomFilter.Cargo));

            List<BOPerfilUsuarioSearchModalItemViewModel> boPerfilUsuarioSearchModalFilterViewModel =
                query.Select(x => new BOPerfilUsuarioSearchModalItemViewModel()
                {
                    UsuarioId = x.UsuarioId,
                    UserName = x.Usuario.UserName,
                    Cargo = x.Cargo,
                    Departamento = x.Departamento
                }).ToList();

            var result = boPerfilUsuarioSearchModalFilterViewModel.PaginationResult(filter);

            return DataTableResult.FromModel(new DataTableResponseModel
            {
                Draw = filter.Draw,
                RecordsTotal = totalRecords,
                RecordsFiltered = boPerfilUsuarioSearchModalFilterViewModel.Count,
                Data = result
            });
        }
    }
}
