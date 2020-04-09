using AutoMapper;
using FWLog.AspNet.Identity;
using FWLog.Data;
using FWLog.Data.Models;
using FWLog.Data.Models.DataTablesCtx;
using FWLog.Data.Models.FilterCtx;
using FWLog.Services.Services;
using FWLog.Web.Backoffice.Helpers;
using FWLog.Web.Backoffice.Models.CommonCtx;
using FWLog.Web.Backoffice.Models.GeralCtx;
using Microsoft.AspNet.Identity;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FWLog.Web.Backoffice.Controllers
{
    public class GeralController : BOBaseController
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly GeralService _geralService;

        public GeralController(UnitOfWork unitOfWork, GeralService geralService)
        {
            _unitOfWork = unitOfWork;
            _geralService = geralService;
        }

        public ActionResult ListarUploads(long Id_Categoria, long Id_Ref)
        {
            var categoria = _geralService.SelecionaUploadCategoria(Id_Categoria);

            if (categoria == null)
                return PartialView("_FormUploads", new GeralUploadVM());

            var model = new GeralUploadVM
            {
                Id_Ref = Id_Ref,
                Id_Categoria = Id_Categoria,
                Formatos = categoria.Formatos,
                Tabela = categoria.Tabela,
                Lista_Uploads = _geralService.TodosUploadsDaCategoria(Id_Categoria, Id_Ref)
                    .Select(x => new GeralUploadVM
                    {
                        Arquivo = x.Arquivo,
                        Arquivo_Tipo = x.Arquivo_Tipo,
                        Id_Usr = x.Id_Usr,
                        Usuario = x.Usuario,
                        Dt_Cad = x.Dt_Cad
                    }).ToList(),
            };

            return PartialView("_FormUploads", model);
        }

        [HttpPost]
        public ActionResult GravarUpload(GeralUploadVM model)
        {
            var msgArr = new List<string>();
            var isUploaded = false;

            var categoria = _geralService.SelecionaUploadCategoria(model.Id_Categoria);

            var formatosPermitidos = categoria.Formatos.Split(',').ToList();

            if (Request.Files.Count > 0)
            {
                foreach (string fileName in Request.Files)
                {
                    HttpPostedFileBase meuArquivo = Request.Files[fileName];

                    if (!formatosPermitidos.Contains(meuArquivo.ContentType))
                    {
                        msgArr.Add(string.Format("Formato do arquivo inválido. ({0})", meuArquivo.FileName));
                    }
                    else
                    {
                        var caminhoArquivo = Server.MapPath(string.Format("~/Uploads/{0}/{1}", categoria.Tabela, model.Id_Ref));

                        if (CriarDiretorioSeNecessario(caminhoArquivo))
                        {
                            try
                            {
                                var nomeArquivo = meuArquivo.FileName;
                                meuArquivo.SaveAs(Path.Combine(caminhoArquivo, nomeArquivo));

                                _geralService.InserirUpload(new GeralUpload
                                {
                                    Id_Categoria = model.Id_Categoria,
                                    Id_Ref = model.Id_Ref,
                                    Id_Usr = User.Identity.GetUserId(),
                                    Arquivo = nomeArquivo,
                                    Arquivo_Tipo = meuArquivo.ContentType
                                });
                                isUploaded = true;
                            }
                            catch (Exception e)
                            {
                                msgArr.Add(string.Format("Erro no upload. ({0})", meuArquivo.FileName));
                            }
                        }
                    }
                }
            }

            return Json(new { isUploaded = isUploaded, message = msgArr }, "text/html");
        }

        public ActionResult ListarHistoricos(long Id_Categoria, long Id_Ref)
        {
            var model = new GeralHistoricoVM
            {
                Id_Ref = Id_Ref,
                Id_Categoria = Id_Categoria,
                Lista_Historicos = _geralService.TodosHistoricosDaCategoria(Id_Categoria, Id_Ref)
                    .Select(x => new GeralHistoricoVM
                    {
                        Historico = x.Historico,
                        Id_Usr = x.Id_Usr,
                        Usuario = x.Usuario,
                        Dt_Cad = x.Dt_Cad
                    }).ToList(),
            };

            return PartialView("_FormHistoricos", model);
        }

        [HttpPost]
        public ActionResult GravarHistorico(GeralHistoricoVM model)
        {
            Func<ViewResult> errorView = () =>
            {
                return View(model);
            };

            if (!ModelState.IsValid)
            {
                var erros = ModelState.Values.Where(x => x.Errors.Count > 0)
                    .Aggregate("", (current, s) => current + (s.Errors[0].ErrorMessage + "<br />"));
                return Json(new AjaxGenericResultModel
                {
                    Success = false,
                    Message = erros
                });
            }

            try
            {
                _geralService.InserirHistorico(new GeralHistorico
                {
                    Id_Categoria = model.Id_Categoria,
                    Id_Ref = model.Id_Ref,
                    Id_Usr = User.Identity.GetUserId(),
                    Historico = model.Historico
                });
                return Json(new AjaxGenericResultModel
                {
                    Success = true,
                    Message = Resources.CommonStrings.RegisterCreatedSuccessMessage
                });
            }
            catch (Exception)
            {
                return Json(new AjaxGenericResultModel
                {
                    Success = false,
                    Message = Resources.CommonStrings.RegisterEditedErrorMessage
                });
            }
        }
    }
}