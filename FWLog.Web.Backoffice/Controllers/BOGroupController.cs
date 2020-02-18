using AutoMapper;
using FWLog.Data;
using FWLog.Data.Models.FilterCtx;
using FWLog.Data.Models;
using FWLog.Data.Models.GeneralCtx;
using FWLog.Data.Models.DataTablesCtx;
using FWLog.Services.Services;
using FWLog.Web.Backoffice.Helpers;
using FWLog.Web.Backoffice.Models.BOGroupCtx;
using FWLog.Web.Backoffice.Models.CommonCtx;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Web.Mvc;
using System.Web.Security;
using Res = Resources.BOGroupStrings;
using System.Web;
using FWLog.AspNet.Identity;
using FWLog.Web.Backoffice.EnumsAndConsts;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using FWLog.Data.EnumsAndConsts;

namespace FWLog.Web.Backoffice.Controllers
{
    public class BOGroupController : BOBaseController
    {
        UnitOfWork _uow;
        BOLogSystemService _boLogSystemService;

        public BOGroupController(UnitOfWork uow, BOLogSystemService boLogSystemService)
        {
            _uow = uow;
            _boLogSystemService = boLogSystemService;
        }

        [ApplicationAuthorize(Permissions = Permissions.Role.List)]
        public ActionResult Index(int page = 1, string groupName = null)
        {
            return View(new BOGroupListViewModel());
        }

        [ApplicationAuthorize(Permissions = Permissions.Role.List)]
        public ActionResult PageData(DataTableFilter<BOGroupFilterViewModel> model)
        {
            int totalRecords = RoleManager.Roles.Count();
            var filter = Mapper.Map<DataTableFilter<BOGroupFilter>>(model);

            var query = RoleManager.Roles.Select(x => new BOGroupTableRow
            {
                Id = x.Id,
                Name = x.Name
            });

            if (!String.IsNullOrEmpty(filter.CustomFilter.Name))
            {
                query = query.Where(x => x.Name.Contains(filter.CustomFilter.Name));
            }

            // Quantidade total de registros com filtros aplicados, sem Skip() e Take().
            int recordsFiltered = query.Count();

            var result = query
                .OrderBy(filter.OrderByColumn, filter.OrderByDirection)
                .Skip(filter.Start)
                .Take(filter.Length)
                .ToList();

            return DataTableResult.FromModel(new DataTableResponseModel
            {
                Draw = model.Draw,
                RecordsTotal = totalRecords,
                RecordsFiltered = recordsFiltered,
                Data = Mapper.Map<IEnumerable<BOGroupListItemViewModel>>(result)
            });
        }

        [ApplicationAuthorize(Permissions = Permissions.Role.Create)]
        public ActionResult Create()
        {
            var model = new BOGroupCreateViewModel
            {
                PermissionGroups = Mapper.Map<List<PermissionGroupViewModel>>(PermissionManager.Groups)
            };

            return View(model);
        }

        [HttpPost]
        [ApplicationAuthorize(Permissions = Permissions.Role.Create)]
        public async Task<ActionResult> Create(BOGroupCreateViewModel model)
        {
            Func<ViewResult> errorView = () =>
            {
                model.PermissionGroups = Mapper.Map<List<PermissionGroupViewModel>>(PermissionManager.Groups);
                return View(model);
            };

            if (!ModelState.IsValid)
            {
                return errorView();
            }

            ApplicationRole existingRole = await RoleManager.FindByNameAsync(model.Name);

            if (existingRole != null)
            {
                ModelState.AddModelError(nameof(model.Name), Res.GroupAlreadyExistsMessage);
                return errorView();
            }

            var role = Mapper.Map<ApplicationRole>(model);
            IEnumerable<string> permissions = model.PermissionGroups
                .SelectMany(x => x.Permissions.Where(y => y.IsSelected)).Select(x => x.Name);

            IdentityResult result = await RoleManager.CreateAsync(role, permissions,IdEmpresa,IdUsuario);

            if (!result.Succeeded)
            {
                throw new Exception(result.Errors.FirstOrDefault());
            }

            var userInfo = new BackOfficeUserInfo();
            _boLogSystemService.Add(new BOLogSystemCreation
            {
                ActionType = ActionTypeNames.Add,
                IP = userInfo.IP,
                UserId = userInfo.UserId,
                EntityName = nameof(AspNetRoles),
                NewEntity = new AspNetRolesLogSerializeModel(role.Name)
            });

            Notify.Success(Resources.CommonStrings.RegisterCreatedSuccessMessage);
            return RedirectToAction("Index");
        }

        [ApplicationAuthorize(Permissions = Permissions.Role.List)]
        public async Task<ActionResult> Details(string id)
        {
            ApplicationRole role = await RoleManager.FindByIdAsync(id);

            if (role == null)
            {
                throw new HttpException(404, "Not found");
            }

            IEnumerable<ApplicationPermission> permissions = role.RolePermissions.Select(x => x.Permission).ToList();

            var model = Mapper.Map<BOGroupDetailsViewModel>(role);
            model.RoleGroups = Mapper.Map<List<PermissionGroupViewModel>>(PermissionManager.Groups);
            model.SetSelectedPermissions(permissions.Select(x => x.Name));

            return View(model);
        }

        [ApplicationAuthorize(Permissions = Permissions.Role.Edit)]
        public async Task<ActionResult> Edit(string id)
        {
            ApplicationRole role = await RoleManager.FindByIdAsync(id);

            if (role == null)
            {
                throw new HttpException(404, "Not found");
            }

            IEnumerable<ApplicationPermission> permissions = role.RolePermissions.Select(x => x.Permission).ToList();

            var model = Mapper.Map<BOGroupCreateViewModel>(role);
            model.PermissionGroups = Mapper.Map<List<PermissionGroupViewModel>>(PermissionManager.Groups);
            model.SetSelectedPermissions(permissions.Select(x => x.Name));

            return View(model);
        }

        [HttpPost]
        [ApplicationAuthorize(Permissions = Permissions.Role.Edit)]
        public async Task<ActionResult> Edit(BOGroupCreateViewModel model)
        {
            IEnumerable<string> selectedPermissions = model.PermissionGroups
                .SelectMany(x => x.Permissions.Where(y => y.IsSelected)).Select(x => x.Name);

            if (!ModelState.IsValid)
            {
                model.PermissionGroups = Mapper.Map<List<PermissionGroupViewModel>>(PermissionManager.Groups);
                model.SetSelectedPermissions(selectedPermissions);
                return View(model);
            }

            ApplicationRole role = await RoleManager.FindByIdAsync(model.Id);

            if (role == null)
            {
                throw new HttpException(404, "Not found");
            }

            ApplicationRole existingRoleByName = await RoleManager.FindByNameAsync(model.Name);

            if (existingRoleByName != null && existingRoleByName.Id != model.Id)
            {
                ModelState.AddModelError(nameof(model.Name), Res.GroupAlreadyExistsMessage);
                model.PermissionGroups = Mapper.Map<List<PermissionGroupViewModel>>(PermissionManager.Groups);
                model.SetSelectedPermissions(selectedPermissions);
                return View(model);
            }

            ApplicationRole oldRole = Mapper.Map<ApplicationRole>(role);

            role.Name = model.Name;
            IdentityResult result = await RoleManager.UpdateAsync(role, selectedPermissions);

            if (!result.Succeeded)
            {
                throw new Exception(result.Errors.FirstOrDefault());
            }

            var userInfo = new BackOfficeUserInfo();
            _boLogSystemService.Add(new BOLogSystemCreation
            {
                ActionType = ActionTypeNames.Edit,
                IP = userInfo.IP,
                UserId = userInfo.UserId,
                EntityName = nameof(AspNetRoles),
                OldEntity = new AspNetRolesLogSerializeModel(oldRole.Name),
                NewEntity = new AspNetRolesLogSerializeModel(role.Name)
            });

            Notify.Success(Resources.CommonStrings.RegisterEditedSuccessMessage);
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ApplicationAuthorize(Permissions = Permissions.Role.Delete)]
        public async Task<JsonResult> AjaxDelete(string id)
        {
            ApplicationRole role = await RoleManager.FindByIdAsync(id);

            if (role == null)
            {
                throw new HttpException(404, "Not found");
            }

            try
            {
                IdentityResult result = await RoleManager.DeleteAsync(role);

                if (!result.Succeeded)
                {
                    throw new InvalidOperationException(Resources.CommonStrings.RequestUnexpectedErrorMessage);
                }


                //var userInfo = new BackOfficeUserInfo();
                //_boLogSystemService.Add(new BOLogSystemCreation
                //{
                //    ActionType = ActionTypeNames.Delete,
                //    IP = userInfo.IP,
                //    UserId = userInfo.UserId,
                //    EntityName = nameof(AspNetRoles),
                //    NewEntity = new AspNetRolesLogSerializeModel(role.Name)
                //});

                return Json(new AjaxGenericResultModel
                {
                    Success = true,
                    Message = Resources.CommonStrings.RegisterDeletedSuccessMessage
                }, JsonRequestBehavior.DenyGet);
            }
            catch (Exception e)
            {
                return Json(new AjaxGenericResultModel
                {
                    Success = false,
                    Message = Resources.CommonStrings.RegisterHasRelationshipsErrorMessage
                }, JsonRequestBehavior.DenyGet);
            }
        }
    }
}