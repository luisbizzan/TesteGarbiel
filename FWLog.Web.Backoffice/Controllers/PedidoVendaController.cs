using DartDigital.Library.Exceptions;
using FWLog.AspNet.Identity;
using FWLog.Services.Services;
using FWLog.Web.Backoffice.Helpers;
using FWLog.Web.Backoffice.Models.CommonCtx;
using log4net;
using System;
using System.Web.Mvc;

namespace FWLog.Web.Backoffice.Controllers
{
    public class PedidoVendaController : BOBaseController
    {
        private readonly ExpedicaoService _expedicaoService;
        private readonly ILog _log;

        public PedidoVendaController(ILog log, ExpedicaoService expedicaoService)
        {
            _log = log;
            _expedicaoService = expedicaoService;
        }

        [HttpPost]
        [ApplicationAuthorize(Permissions = Permissions.RelatoriosExpedicao.RelatorioPedidosCadastrarVolume)]
        public ActionResult ValidarPedidoVenda(int id)
        {
            try
            {
                var result = _expedicaoService.GerenciarVolumesValidacaoPedido(id, IdEmpresa);

                return Json(new AjaxGenericResultModel
                {
                    Success = true,
                    Message = ""                    
                });
            }
            catch (Exception e)
            {
                _log.Error(e.Message, e);

                return Json(new AjaxGenericResultModel
                {
                    Success = false,
                    Message = e is BusinessException ? e.Message : "Ocorreu um erro na consulta dos dados do pedido."
                }, JsonRequestBehavior.DenyGet);
            }
        }
    }
}

