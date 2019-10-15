using FWLog.Data;
using FWLog.Web.Backoffice.EnumsAndConsts;
using FWLog.Web.Backoffice.Helpers;
using FWLog.Web.Backoffice.Models.CommonCtx;
using FWLog.Web.Backoffice.Models.ExampleCtx;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace FWLog.Web.Backoffice.Controllers
{
    public class ExampleController : BOBaseController
    {
        UnitOfWork _uow;

        public ExampleController(UnitOfWork uow)
        {
            _uow = uow;
        }

        #region Upload

        public ActionResult Upload()
        {
            var model = new UploadModel()
            {
                ImageName = GetFirstFile()
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult UploadCreate(string fileName)
        {
            if (!fileName.NullOrEmpty())
            {
                // obrigatório chamar função para transferir o arquivo da pasta temporária para a pasta correta.
                FileHelper.MoveFileFromTempFolder(FileUploadDefinitions.ExampleConfig, fileName);
            }

            return RedirectToAction("Upload");
        }

        [HttpPost]
        public ActionResult UploadEdit(List<string> fileName)
        {
            var oldFileName = GetFirstFile();

            if (fileName != null && fileName.Any())
            {
                //contador é para aplicar a lógica da página de exemplo, em um cenário real ele não é necessário
                var count = 0;

                foreach (var file in fileName)
                {
                    // obrigatório chamar função para apagar o arquivo antigo e transferir o novo da pasta temporária para a pasta correta.
                    if (count == 0)
                    {
                        FileHelper.EditFile(FileUploadDefinitions.ExampleConfig, file, oldFileName);
                    }
                    else
                    {
                        FileHelper.EditFile(FileUploadDefinitions.ExampleConfig, file, null);
                    }

                    count++;
                }
            }


            return RedirectToAction("Upload");
        }

        [HttpPost]
        public ActionResult UploadDelete(string fileName)
        {
            // função apaga o arquivo na pasta
            FileHelper.DeleteFile(FileUploadDefinitions.ExampleConfig, fileName);

            return RedirectToAction("Upload");
        }

        private string GetFirstFile()
        {
            var path = string.Concat(AppDomain.CurrentDomain.BaseDirectory, "Dart");

            if (!Directory.Exists(path))
            {
                return null;
            }

            var directory = new DirectoryInfo(path);

            var file = directory.GetFiles().FirstOrDefault();

            if (file != null)
            {
                return file.Name;
            }
            else
            {
                return string.Empty;
            }
        }

        #endregion Upload

        #region MultiSelect

        public ActionResult MultiSelect()
        {
            ViewBag.MultiSelect = new List<SelectListItem>() {
                new SelectListItem(){Text = "Valor 1", Value = "1" },
                new SelectListItem(){Text = "Valor 2", Value = "2" },
                new SelectListItem(){Text = "Valor 3", Value = "3" },
                new SelectListItem(){Text = "Valor 4", Value = "4" },
                new SelectListItem(){Text = "Valor 5", Value = "5" }
            };

            return View();
        }

        [HttpPost]
        public ActionResult MultiSelect(MultiSelectModel model)
        {
            ViewBag.MultiSelect = new List<SelectListItem>() {
                new SelectListItem(){Text = "Valor 1", Value = "1" },
                new SelectListItem(){Text = "Valor 2", Value = "2" },
                new SelectListItem(){Text = "Valor 3", Value = "3" },
                new SelectListItem(){Text = "Valor 4", Value = "4" },
                new SelectListItem(){Text = "Valor 5", Value = "5" }
            };

            model.SingleValue = "2";
            model.ListValues = new List<string>() { "1", "2", "3" };

            return View(model);
        }



        #endregion MultiSelect

        #region ImageUpload

        public ActionResult ImageUpload()
        {
            return View();
        }

        #endregion

        #region AutoComplete

        public ActionResult AutoComplete()
        {
            return View();
        }

        public AutoCompleteResult SearchForAutoComplete(string query)
        {
            int takeCount = 10;
            IEnumerable<ApplicationLog> search = _uow.ApplicationLogRepository.SearchByMessage(query, takeCount);
            var suggestions = search.Select(x => new AutoCompleteSuggestionModel(value: x.Message, data: x.IdApplicationLog));
            var response = new AutoCompleteResponseModel(suggestions);

            return AutoCompleteResult.FromModel(response);
        }

        #endregion

        #region Testes Validação

        public ActionResult TesteValidacao()
        {
            return View(new TesteValidacaoViewModel());
        }

        [HttpPost]
        public ActionResult TesteValidacao(TesteValidacaoViewModel model)
        {
            if (ModelState.IsValid)
            {
                model.Message = "Modelo é válido.";
            }
            else
            {
                model.Message = "Modelo não é válido.";
            }

            return View(model);
        }

        #endregion


    }



}