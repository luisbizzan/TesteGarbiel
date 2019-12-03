using AutoMapper;
using FWLog.AspNet.Identity;
using FWLog.Data;
using FWLog.Data.Models;
using FWLog.Services.Services;
using FWLog.Web.Backoffice.Helpers;
using FWLog.Web.Backoffice.Models.BOEmpresaCtx;
using System.Web;
using System.Web.Mvc;

namespace FWLog.Web.Backoffice.Controllers
{
    public class BOEmpresaConfigController : BOBaseController
    {
        private readonly UnitOfWork _uow;
        private readonly EmpresaConfigService _empresaConfigService;

        public BOEmpresaConfigController(UnitOfWork uow, EmpresaConfigService empresaConfigService)
        {
            _uow = uow;
            _empresaConfigService = empresaConfigService;
        }
       
        [ApplicationAuthorize(Permissions = Permissions.EmpresaConfig.Edit)]
        public ActionResult Edit()
        {
            EmpresaConfig empresaConfig = _uow.EmpresaConfigRepository.ConsultarPorIdEmpresa(IdEmpresa);

            if (empresaConfig == null)
            {
                throw new HttpException(404, "Not found");
            }

            var model = Mapper.Map<EmpresaConfigEditViewModel>(empresaConfig);

            return View(model);
        }

        [HttpPost]
        [ApplicationAuthorize(Permissions = Permissions.EmpresaConfig.Edit)]
        public ActionResult Edit(EmpresaConfigEditViewModel model)
        {
            if (model.IdEmpresa == model.IdEmpresaGarantia && !model.EmpresaFazGarantia)
            {
                ModelState.AddModelError(nameof(model.EmpresaFazGarantia), string.Format("A empresa editada não pode ser selecionada para Garantia, se não estiver marcada para fazer garantia.", model.RazaoSocialEmpresaGarantia));
            }

            var empresaConfigGarantia = _uow.EmpresaConfigRepository.ConsultarPorIdEmpresa(model.IdEmpresaGarantia);
            if (!empresaConfigGarantia.EmpresaFazGarantia)
            {
                ModelState.AddModelError(nameof(model.IdEmpresaGarantia), string.Format("A Empresa '{0}' não pode ser selecionada para Garantia, se não estiver marcada para fazer garantia.", model.RazaoSocialEmpresaGarantia));
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            EmpresaConfig empresaConfig = Mapper.Map<EmpresaConfig>(model);

            _empresaConfigService.Save(empresaConfig);

            Notify.Success(Resources.CommonStrings.RegisterEditedSuccessMessage);
            return RedirectToAction("Index");
        }
    }
}