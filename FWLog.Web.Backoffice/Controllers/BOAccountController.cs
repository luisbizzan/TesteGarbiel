using AutoMapper;
using ExtensionMethods.List;
using FWLog.AspNet.Identity;
using FWLog.Data;
using FWLog.Data.EnumsAndConsts;
using FWLog.Data.Models;
using FWLog.Data.Models.DataTablesCtx;
using FWLog.Data.Models.FilterCtx;
using FWLog.Data.Models.GeneralCtx;
using FWLog.Services.Services;
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
        private readonly UnitOfWork _unitOfWork;
        private readonly BOAccountService _boService;
        private readonly BOLogSystemService _boLogSystemService;
        private readonly PasswordService _passwordService;

        public BOAccountController(
            UnitOfWork uow,
            BOAccountService boService,
            BOLogSystemService boLogSystemService,
            PasswordService passwordService)
        {
            _unitOfWork = uow;
            _boService = boService;
            _boLogSystemService = boLogSystemService;
            _passwordService = passwordService;
        }

        [HttpGet]
        [ApplicationAuthorize(Permissions = Permissions.BOAccount.List)]
        public ActionResult Index()
        {
            ViewBag.Empresas = _Empresas;

            var viewModel = new BOAccountListViewModel
            {
                Status = new SelectList(new List<SelectListItem>
                {
                    new SelectListItem { Text = "Ativo", Value = "true" },
                    new SelectListItem { Text = "Inativo", Value = "false" }
                }, "Value", "Text")
            };

            viewModel.Filter.Ativo = true;

            return View(viewModel);
        }

        [HttpPost]
        [ApplicationAuthorize(Permissions = Permissions.BOAccount.List)]
        public ActionResult PageData(DataTableFilter<BOAccountFilterViewModel> model)
        {

            int totalRecords = UserManager.Users.Count();
            var filtros = Mapper.Map<DataTableFilter<UsuarioListaFiltro>>(model);
            filtros.CustomFilter.IdEmpresa = IdEmpresa;

            var query = UserManager.Users.Select(x => new UsuarioListaLinhaTabela
            {
                UserName = x.UserName,
                Email = x.Email
            });

            if (!String.IsNullOrEmpty(filtros.CustomFilter.UserName))
            {
                query = query.Where(x => x.UserName.Contains(filtros.CustomFilter.UserName));
            }

            // Quantidade total de registros com filtros aplicados, sem Skip() e Take().
            int recordsFiltered = query.Count();

            List<UsuarioListaLinhaTabela> result = _unitOfWork.PerfilUsuarioRepository.PesquisarLista(filtros);

            return DataTableResult.FromModel(new DataTableResponseModel
            {
                Draw = model.Draw,
                RecordsTotal = totalRecords,
                RecordsFiltered = recordsFiltered,
                Data = Mapper.Map<IEnumerable<BOAccountListItemViewModel>>(result)
            });
        }

        [HttpGet]
        [ApplicationAuthorize(Permissions = Permissions.BOAccount.Create)]
        public ActionResult Create()
        {
            ViewBag.Empresas = _Empresas;

            var viewModel = new BOAccountCreateViewModel
            {
                Ativo = true
            };

            return View(viewModel);
        }

        [HttpPost]
        [ApplicationAuthorize(Permissions = Permissions.BOAccount.Create)]
        public async Task<ActionResult> AdicionarEmpresa(long id)
        {
            List<GroupItemViewModel> gruposPermissoesUsuario = (await UserManager.GetUserRolesByIdEmpresa(User.Identity.GetUserId(), id).ConfigureAwait(false))
                .OrderBy(x => x).Select(x => new GroupItemViewModel { IsSelected = false, Name = x }).ToList();

            if (gruposPermissoesUsuario.Any(x => x.Name == "Administrador"))
            {
                gruposPermissoesUsuario = RoleManager.Roles.OrderBy(x => x.Name).Select(x => new GroupItemViewModel { IsSelected = false, Name = x.Name }).ToList();
            }

            var perfilImpressoras = PerfilImpressorasList(id);

            var empresasGrupos = new EmpresaGrupoViewModel
            {
                IdEmpresa = id,
                Nome = Empresas.First(f => f.IdEmpresa == id).Nome,
                Grupos = gruposPermissoesUsuario,
                PerfilImpressora = perfilImpressoras
            };

            var list = new List<EmpresaGrupoViewModel>
            {
                empresasGrupos
            };

            return PartialView("_EmpresaGrupo", list);
        }

        [HttpPost]
        [ApplicationAuthorize(Permissions = Permissions.BOAccount.Create)]
        public async Task<ActionResult> Create(BOAccountCreateViewModel model)
        {
            ViewBag.Empresas = _Empresas;

            for (int i = 0; i < model.EmpresasGrupos.Count; i++)
            {
                model.EmpresasGrupos[i].PerfilImpressora = PerfilImpressorasList(model.EmpresasGrupos[i].IdEmpresa, idPerfilImpressora: model.EmpresasGrupos[i].IdPerfilImpressoraPadrao);
            }

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

            if (model.EmpresasGrupos.Any(x => x.IdPerfilImpressoraPadrao == null))
            {
                ModelState.AddModelError(nameof(model.EmpresasGrupos), "Selecione um Perfil de Impressão Padrão em todas as empresas.");
            }

            var existingUserByName = await UserManager.FindByNameAsync(model.UserName).ConfigureAwait(false);

            if (existingUserByName != null)
            {
                ModelState.AddModelError(nameof(model.UserName), "O código de usuário informado já existe");
            }

            var existsUserByEmail = await UserManager.FindByEmailAsync(model.Email).ConfigureAwait(false);

            if (existsUserByEmail != null)
            {
                ModelState.AddModelError(nameof(model.Email), "E-mail informado já utilizado por outro usuário");
            }

            if (model.EmpresasGrupos.NullOrEmpty())
            {
                ModelState.AddModelError(nameof(model.EmpresasGrupos), Res.RequiredOnlyCompany);
                return errorView(null);
            }

            if (!model.EmpresasGrupos.All(w => w.Grupos.Any(a => a.IsSelected)))
            {
                ModelState.AddModelError(nameof(model.EmpresasGrupos), Res.RequiredOnlyGroup);
                return errorView(null);
            }

            if (!model.EmpresasGrupos.Where(w => w.IsEmpresaPrincipal).Any())
            {
                ModelState.AddModelError(nameof(model.EmpresasGrupos), "Selecione a empresa principal do usuário");
                return errorView(null);
            }

            for (int i = 0; i < model.EmpresasGrupos.Count; i++)
            {
                if (model.EmpresasGrupos[0].CorredorEstoqueInicio == null && model.EmpresasGrupos[0].CorredorEstoqueFim != null)
                {
                    ModelState.AddModelError($"[{i}].CorredorEstoque", "Preencha o valor inicial do Corredor Estoque");
                }

                if (model.EmpresasGrupos[0].CorredorEstoqueInicio != null && model.EmpresasGrupos[0].CorredorEstoqueFim == null)
                {
                    ModelState.AddModelError($"[{i}].CorredorEstoque", "Preencha o valor final do Corredor Estoque");
                }

                if (model.EmpresasGrupos[0].CorredorEstoqueInicio.HasValue && model.EmpresasGrupos[0].CorredorEstoqueFim.HasValue)
                {
                    if (model.EmpresasGrupos[0].CorredorEstoqueInicio.Value > model.EmpresasGrupos[0].CorredorEstoqueFim.Value)
                    {
                        ModelState.AddModelError($"[{i}].CorredorEstoque", "O valor inicial do Corredor Estoque não pode ser maior do que o final");
                    }
                }

                if (model.EmpresasGrupos[0].CorredorSeparacaoInicio == null && model.EmpresasGrupos[0].CorredorSeparacaoFim != null)
                {
                    ModelState.AddModelError($"[{i}].CorredorSeparacao", "Preencha o valor inicial do Corredor Separação");
                }

                if (model.EmpresasGrupos[0].CorredorSeparacaoInicio != null && model.EmpresasGrupos[0].CorredorSeparacaoFim == null)
                {
                    ModelState.AddModelError($"[{i}].CorredorSeparacao", "Preencha o valor final do Corredor Separação");
                }

                if (model.EmpresasGrupos[0].CorredorSeparacaoInicio.HasValue && model.EmpresasGrupos[0].CorredorSeparacaoFim.HasValue)
                {
                    if (model.EmpresasGrupos[0].CorredorSeparacaoInicio.Value > model.EmpresasGrupos[0].CorredorSeparacaoFim.Value)
                    {
                        ModelState.AddModelError($"[{i}].CorredorSeparacao", "O valor inicial do Corredor Separação não pode ser maior do que o final");
                    }
                }
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

            var perfilUsuario = new PerfilUsuario
            {
                Ativo = model.Ativo,
                Cargo = model.Cargo,
                DataNascimento = model.DataNascimento,
                Departamento = model.Departamento,
                Nome = model.Nome,
                UsuarioId = user.Id,
                EmpresaId = model.EmpresasGrupos.Where(w => w.IsEmpresaPrincipal).First().IdEmpresa
            };

            _unitOfWork.PerfilUsuarioRepository.Add(perfilUsuario);
            _unitOfWork.SaveChanges();

            foreach (var f in model.EmpresasGrupos)
            {
                var newUsuarioEmpresa = new UsuarioEmpresa
                {
                    UserId = user.Id,
                    IdEmpresa = f.IdEmpresa,
                    PerfilUsuarioId = perfilUsuario.PerfilUsuarioId,
                    IdPerfilImpressoraPadrao = f.IdPerfilImpressoraPadrao,
                    CorredorEstoqueInicio = f.CorredorEstoqueInicio,
                    CorredorEstoqueFim = f.CorredorEstoqueFim,
                    CorredorSeparacaoInicio = f.CorredorSeparacaoInicio,
                    CorredorSeparacaoFim = f.CorredorSeparacaoFim,
                };
                _unitOfWork.UsuarioEmpresaRepository.Add(newUsuarioEmpresa);
            }

            _unitOfWork.SaveChanges();

            var empresasGruposNew = new StringBuilder();

            foreach (var item in model.EmpresasGrupos.Where(x => x.Grupos.Any(y => y.IsSelected)))
            {
                IEnumerable<string> selectedRoles = item.Grupos.Where(x => x.IsSelected).Select(x => x.Name);

                empresasGruposNew.AppendLine(string.Format("{0}: {1}", item.Nome, string.Join(", ", selectedRoles.ToArray())));
                empresasGruposNew.AppendLine(" || ");

                result = UserManager.AddToRolesByEmpresa(user, selectedRoles.ToArray(), item.IdEmpresa);

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
                NewEntity = new AspNetUsersLogSerializeModel(user.UserName, perfilUsuario, empresasGruposNew.ToString())
            });

            Notify.Success(Resources.CommonStrings.RegisterCreatedSuccessMessage);
            return RedirectToAction("Index");
        }

        [HttpGet]
        [ApplicationAuthorize(Permissions = Permissions.BOAccount.Edit)]
        public async Task<ActionResult> Edit(string id)
        {
            ViewBag.Empresas = _Empresas;

            ApplicationUser user = await UserManager.FindByNameAsync(id).ConfigureAwait(false);

            if (user == null)
            {
                throw new HttpException(404, "Not found");
            }

            var empresas = _unitOfWork.UsuarioEmpresaRepository.GetAllEmpresasByUserId(user.Id);

            empresas = Empresas.Where(w => empresas.Contains(w.IdEmpresa)).Select(s => s.IdEmpresa).ToList();

            var usuarioEmpresas = _unitOfWork.UsuarioEmpresaRepository.Tabela().Where(x => x.UserId == user.Id).ToList();

            var perfil = _unitOfWork.PerfilUsuarioRepository.GetByUserId(user.Id);

            var model = new BOAccountEditViewModel
            {
                Ativo = perfil.Ativo,
                Cargo = perfil.Cargo,
                DataNascimento = perfil.DataNascimento,
                Departamento = perfil.Departamento,
                PerfilUsuarioId = perfil.PerfilUsuarioId,
                Nome = perfil.Nome,
                Email = user.Email,
                UserName = user.UserName
            };

            foreach (long empresa in empresas)
            {
                var usuarioEmpresa = usuarioEmpresas.FirstOrDefault(x => x.IdEmpresa == empresa);

                var empGrupos = new EmpresaGrupoViewModel
                {
                    IdEmpresa = empresa,
                    Nome = Empresas.First(f => f.IdEmpresa == empresa).Nome,
                    IsEmpresaPrincipal = perfil.EmpresaId == empresa ? true : false,
                    IdPerfilImpressoraPadrao = usuarioEmpresa?.IdPerfilImpressoraPadrao,
                    PerfilImpressora = PerfilImpressorasList(empresa, usuarioEmpresa?.IdPerfilImpressoraPadrao),
                    CorredorEstoqueInicio = usuarioEmpresa?.CorredorEstoqueInicio,
                    CorredorEstoqueFim = usuarioEmpresa?.CorredorEstoqueFim,
                    CorredorSeparacaoInicio= usuarioEmpresa?.CorredorSeparacaoInicio,
                    CorredorSeparacaoFim = usuarioEmpresa?.CorredorSeparacaoFim,
                };

                List<string> gruposPermissoesUsuarioEdicao = (await UserManager.GetUserRolesByIdEmpresa(user.Id, empresa).ConfigureAwait(false)).OrderBy(x => x).ToList();
                List<string> gruposPermissoesUsuarioLogado = (await UserManager.GetUserRolesByIdEmpresa(User.Identity.GetUserId(), empresa).ConfigureAwait(false)).OrderBy(x => x).ToList();

                if (gruposPermissoesUsuarioLogado.Contains("Administrador"))
                {
                    List<string> todosGrupos = RoleManager.Roles.OrderBy(x => x.Name).Select(x => x.Name).ToList();

                    foreach (var grupoPermissaoUsuario in todosGrupos)
                    {
                        var groupItemViewModel = new GroupItemViewModel
                        {
                            IsSelected = gruposPermissoesUsuarioEdicao.Any(x => x == grupoPermissaoUsuario),
                            Name = grupoPermissaoUsuario
                        };

                        empGrupos.Grupos.Add(groupItemViewModel);
                    }
                }
                else
                {
                    var gruposPermissoesSemelhantes = gruposPermissoesUsuarioEdicao.Where(x => gruposPermissoesUsuarioLogado.Contains(x)).ToList();

                    foreach (var grupoPermissaoUsuario in gruposPermissoesSemelhantes)
                    {
                        var groupItemViewModel = new GroupItemViewModel
                        {
                            IsSelected = true,
                            Name = grupoPermissaoUsuario
                        };

                        empGrupos.Grupos.Add(groupItemViewModel);
                    }
                }

                empGrupos.Grupos.OrderBy(x => x.Name);

                model.EmpresasGrupos.Add(empGrupos);
            }

            return View(model);
        }

        [HttpPost]
        [ApplicationAuthorize(Permissions = Permissions.BOAccount.Edit)]
        public async Task<ActionResult> Edit(BOAccountEditViewModel model)
        {
            ViewBag.Empresas = _Empresas;

            for (int i = 0; i < model.EmpresasGrupos.Count; i++)
            {
                model.EmpresasGrupos[i].PerfilImpressora = PerfilImpressorasList(model.EmpresasGrupos[i].IdEmpresa, idPerfilImpressora: model.EmpresasGrupos[i].IdPerfilImpressoraPadrao);
            }

            ViewResult errorView()
            {
                return View(model);
            }

            if (!ModelState.IsValid)
            {
                return errorView();
            }

            ApplicationUser user = await UserManager.FindByNameAsync(model.UserName).ConfigureAwait(false);

            if (user == null)
            {
                throw new HttpException(404, "Not found");
            }

            if (model.EmpresasGrupos.Any(x => x.IdPerfilImpressoraPadrao == null))
            {
                ModelState.AddModelError(nameof(model.EmpresasGrupos), "Selecione um Perfil de Impressão Padrão em todas as empresas.");
                return errorView();
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

            if (!model.EmpresasGrupos.All(w => w.Grupos.Any(a => a.IsSelected)))
            {
                ModelState.AddModelError(nameof(model.EmpresasGrupos), Res.RequiredOnlyGroup);
                return errorView();
            }

            if (!model.EmpresasGrupos.Any(w => w.IsEmpresaPrincipal))
            {
                ModelState.AddModelError(nameof(model.EmpresasGrupos), "Selecione a empresa principal do usuário.");
                return errorView();
            }

            for (int i = 0; i < model.EmpresasGrupos.Count; i++)
            {
                if (model.EmpresasGrupos[0].CorredorEstoqueInicio == null && model.EmpresasGrupos[0].CorredorEstoqueFim != null)
                {
                    ModelState.AddModelError($"[{i}].CorredorEstoque", "Preencha o valor inicial do Corredor Estoque");
                }

                if (model.EmpresasGrupos[0].CorredorEstoqueInicio != null && model.EmpresasGrupos[0].CorredorEstoqueFim == null)
                {
                    ModelState.AddModelError($"[{i}].CorredorEstoque", "Preencha o valor final do Corredor Estoque");
                }

                if (model.EmpresasGrupos[0].CorredorEstoqueInicio.HasValue && model.EmpresasGrupos[0].CorredorEstoqueFim.HasValue)
                {
                    if (model.EmpresasGrupos[0].CorredorEstoqueInicio.Value > model.EmpresasGrupos[0].CorredorEstoqueFim.Value)
                    {
                        ModelState.AddModelError($"[{i}].CorredorEstoque", "O valor inicial do Corredor Estoque não pode ser maior do que o final");
                    }
                }

                if (model.EmpresasGrupos[0].CorredorSeparacaoInicio == null && model.EmpresasGrupos[0].CorredorSeparacaoFim != null)
                {
                    ModelState.AddModelError($"[{i}].CorredorSeparacao", "Preencha o valor inicial do Corredor Separação");
                }

                if (model.EmpresasGrupos[0].CorredorSeparacaoInicio != null && model.EmpresasGrupos[0].CorredorSeparacaoFim == null)
                {
                    ModelState.AddModelError($"[{i}].CorredorSeparacao", "Preencha o valor final do Corredor Separação");
                }

                if (model.EmpresasGrupos[0].CorredorSeparacaoInicio.HasValue && model.EmpresasGrupos[0].CorredorSeparacaoFim.HasValue)
                {
                    if (model.EmpresasGrupos[0].CorredorSeparacaoInicio.Value > model.EmpresasGrupos[0].CorredorSeparacaoFim.Value)
                    {
                        ModelState.AddModelError($"[{i}].CorredorSeparacao", "O valor inicial do Corredor Separação não pode ser maior do que o final");
                    }
                }
            }

            if (!ModelState.IsValid)
            {
                return errorView();
            }

            ApplicationUser oldUser = Mapper.Map<ApplicationUser>(user);
            user.Email = model.Email;

            var empresasGruposNew = new StringBuilder();
            var empresasGruposOld = new StringBuilder();

            var companiesUser = _unitOfWork.UsuarioEmpresaRepository.GetAllEmpresasByUserId(user.Id);
            var empresas = Empresas.Where(w => companiesUser.Contains(w.IdEmpresa)).ToList();

            foreach (var empresa in empresas)
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
                IEnumerable<string> selectedRoles = item.Grupos.Where(x => x.IsSelected).Select(x => x.Name);
                IList<string> rolesUsuarioEdicao = await UserManager.GetUserRolesByIdEmpresa(user.Id, item.IdEmpresa).ConfigureAwait(false);
                IList<string> rolesUsuarioLogado = await UserManager.GetUserRolesByIdEmpresa(User.Identity.GetUserId(), item.IdEmpresa).ConfigureAwait(false);
                IEnumerable<string> rolesIgnorar = rolesUsuarioEdicao.Where(x => !rolesUsuarioLogado.Any(y => y == x) && !rolesUsuarioLogado.Contains("Administrador"));


                empresasGruposNew.AppendLine(string.Format("{0}: {1}", item.Nome, string.Join(", ", selectedRoles.ToArray())));
                empresasGruposNew.AppendLine(" || ");
                IdentityResult result = await UserManager.UpdateAsync(user, selectedRoles, rolesIgnorar, item.IdEmpresa).ConfigureAwait(false);

                if (!result.Succeeded)
                {
                    throw new InvalidOperationException(Resources.CommonStrings.RequestUnexpectedErrorMessage);
                }
            }

            var oldPerfil = _unitOfWork.PerfilUsuarioRepository.GetById(model.PerfilUsuarioId);
            var log = new AspNetUsersLogSerializeModel(oldUser.UserName, oldPerfil, empresasGruposOld.ToString());

            var perfilUsuario = _unitOfWork.PerfilUsuarioRepository.GetById(model.PerfilUsuarioId);
            perfilUsuario.Nome = model.Nome;
            perfilUsuario.EmpresaId = model.EmpresasGrupos.Where(w => w.IsEmpresaPrincipal).First().IdEmpresa;
            perfilUsuario.Departamento = model.Departamento;
            perfilUsuario.DataNascimento = model.DataNascimento;
            perfilUsuario.Cargo = model.Cargo;
            perfilUsuario.Ativo = model.Ativo;

            _boService.EditPerfilUsuario(perfilUsuario);

            var empresasGrupos = model.EmpresasGrupos.Where(w => w.Grupos.Any(a => a.IsSelected))
                .Select(s => new BOAccountService.UsuarioEmpresaUpdateFields { IdEmpresa = s.IdEmpresa, IdPerfilImpressoraPadrao = s.IdPerfilImpressoraPadrao, CorredorEstoqueInicio = s.CorredorEstoqueInicio, CorredorEstoqueFim = s.CorredorEstoqueFim, CorredorSeparacaoInicio = s.CorredorSeparacaoInicio, CorredorSeparacaoFim = s.CorredorSeparacaoFim }).ToList();
            _boService.EditUsuarioEmpresas(Empresas, empresasGrupos, user.Id, perfilUsuario.PerfilUsuarioId);

            var userInfo = new BackOfficeUserInfo();
            _boLogSystemService.Add(new BOLogSystemCreation
            {
                ActionType = ActionTypeNames.Edit,
                IP = userInfo.IP,
                UserId = userInfo.UserId,
                EntityName = nameof(AspNetUsers),
                OldEntity = log,
                NewEntity = new AspNetUsersLogSerializeModel(user.UserName, perfilUsuario, empresasGruposNew.ToString())
            });

            Notify.Success(Resources.CommonStrings.RegisterEditedSuccessMessage);
            return RedirectToAction("Index");
        }

        [HttpGet]
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
                    Message = "O usuário já realizou ações no sistema e não pode ser excluído. Utilize a opção \"Inativar\" na tela de edição."
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

        [HttpGet]
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
                    ApplicationSession applicationSession = _unitOfWork.ApplicationSessionRepository.GetById(applicationUser.IdApplicationSession.Value);

                    applicationSession.DataUltimaAcao = DateTime.Now;
                    applicationSession.DataLogout = DateTime.Now;

                    _unitOfWork.ApplicationSessionRepository.Update(applicationSession);
                    _unitOfWork.SaveChanges();

                    applicationUser.IdApplicationSession = null;
                    UserManager.Update(applicationUser);
                }
            }

            CookieLogoff();

            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("LogOn", "BOAccountBase");
        }

        [HttpGet]
        [ApplicationAuthorize]
        public ActionResult SearchModal(string id)
        {
            var viewModel = new BOPerfilUsuarioSearchModalViewModel
            {
                Origem = id
            };

            return View(viewModel);
        }

        [HttpPost]
        [ApplicationAuthorize]
        public ActionResult SearchModalPageData(DataTableFilter<BOPerfilUsuarioSearchModalFilterViewModel> filter)
        {
            int totalRecords = UserManager.Users.Count();
            var filtros = Mapper.Map<DataTableFilter<BOPerfilUsuarioSearchModalFilterViewModel>>(filter);

            var query = _unitOfWork.PerfilUsuarioRepository.Tabela();
            query = query.Where(x => x.EmpresaId == IdEmpresa);

            if (!String.IsNullOrEmpty(filtros.CustomFilter.UserName))
                query = query.Where(x => x.Usuario.UserName.Contains(filtros.CustomFilter.UserName));

            if (!String.IsNullOrEmpty(filtros.CustomFilter.Nome))
                query = query.Where(x => x.Nome.Contains(filtros.CustomFilter.Nome));

            if (!String.IsNullOrEmpty(filtros.CustomFilter.Departamento))
                query = query.Where(x => x.Departamento.Contains(filtros.CustomFilter.Departamento));

            if (!String.IsNullOrEmpty(filtros.CustomFilter.Cargo))
                query = query.Where(x => x.Cargo.Contains(filtros.CustomFilter.Cargo));

            // Quantidade total de registros com filtros aplicados, sem Skip() e Take().
            int recordsFiltered = query.Count();

            List<BOPerfilUsuarioSearchModalItemViewModel> boPerfilUsuarioSearchModalFilterViewModel =
                query.Select(x => new BOPerfilUsuarioSearchModalItemViewModel()
                {
                    UsuarioId = x.UsuarioId,
                    UserName = x.Usuario.UserName,
                    Nome = x.Nome,
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

        private SelectList PerfilImpressorasList(long idEmpresa, long? idPerfilImpressora = null)
        {
            return new SelectList(
                          _unitOfWork.PerfilImpressoraRepository.RetornarAtivas().Where(x => x.IdEmpresa == idEmpresa).Select(x => new SelectListItem
                          {
                              Value = x.IdPerfilImpressora.ToString(),
                              Text = x.Nome,
                              Selected = idPerfilImpressora == x.IdPerfilImpressora
                          }).ToList(), "Value", "Text", idPerfilImpressora);
        }

        public string LabelUsuario()
        {
            PerfilUsuario usuario = _unitOfWork.PerfilUsuarioRepository.GetByUserId(User.Identity.GetUserId());

            string rtn = $"{usuario.Usuario.UserName} - {usuario.Nome}";

            return rtn;
        }
    }
}
