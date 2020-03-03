using AutoMapper;
using FWLog.Data;
using FWLog.Data.Models.FilterCtx;
using FWLog.Data.Models;
using FWLog.Data.Models.DataTablesCtx;
using FWLog.Services.Services;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web;
using FWLog.Web.Backoffice.Helpers;
using FWLog.AspNet.Identity;
using FWLog.Web.Backoffice.Models.GarantiaCtx;
using FWLog.Web.Backoffice.Models.CommonCtx;
using System;
using System.Linq;
using ExtensionMethods.String;

namespace FWLog.Web.Backoffice.Controllers
{
    public class GarantiaController : BOBaseController
    {
        UnitOfWork _uow;
        GarantiaService _garantiaService;

        public GarantiaController(UnitOfWork uow, GarantiaService garantiaService)
        {
            _uow = uow;
            _garantiaService = garantiaService;
        }

        [ApplicationAuthorize(Permissions = Permissions.Garantia.Listar)]
        public ActionResult Index()
        {
            var model = new GarantiaListViewModel
            {
                Filter = new GarantiaFilterViewModel()
                {
                    ListaStatus = new SelectList(
                    _uow.GarantiaStatusRepository.Todos().OrderBy(o => o.IdGarantiaStatus).Select(x => new SelectListItem
                    {
                        Value = x.IdGarantiaStatus.GetHashCode().ToString(),
                        Text = x.Descricao,
                    }), "Value", "Text"
                )
                }
            };

            model.Filter.IdGarantiaStatus = GarantiaStatusEnum.AguardandoRecebimento.GetHashCode();
            model.Filter.DataEmissaoInicial = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 00, 00, 00).AddDays(-7);
            model.Filter.DataEmissaoFinal = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 00, 00, 00).AddDays(10);

            return View(model);
        }

        [ApplicationAuthorize(Permissions = Permissions.Garantia.Listar)]
        public ActionResult PageData(DataTableFilter<GarantiaFilterViewModel> model)
        {
            int recordsFiltered, totalRecords;
            var filter = Mapper.Map<DataTableFilter<GarantiaFilter>>(model);

            filter.CustomFilter.IdEmpresa = IdEmpresa;

            IEnumerable<GarantiaTableRow> result = _uow.GarantiaRepository.SearchForDataTable(filter, out recordsFiltered, out totalRecords);

            return DataTableResult.FromModel(new DataTableResponseModel
            {
                Draw = model.Draw,
                RecordsTotal = totalRecords,
                RecordsFiltered = recordsFiltered,
                Data = Mapper.Map<IEnumerable<GarantiaListItemViewModel>>(result)
            });
        }

        [ApplicationAuthorize(Permissions = Permissions.Garantia.Listar)]
        public ActionResult DetalhesEntradaConferencia(int id)
        {
            var notaFiscal = _uow.NotaFiscalRepository.GetById(id);

            var itensDaNota = _uow.NotaFiscalItemRepository.ObterItens(id);

            var valorProduto = itensDaNota.Sum(x => x.ValorUnitario);

            var model = new GarantiaDetalhesEntradaConferenciaViewModel
            {
                IdNotaFiscal = id,
                BaseICMS = notaFiscal.BaseICMS?.ToString(),
                BaseST = notaFiscal.BaseST?.ToString(),
                ChaveAcesso = notaFiscal.ChaveAcesso?.ToString(),
                CienteCNPJ = notaFiscal.Cliente.CNPJCPF.CnpjOuCpf(),
                DataEmissaoNF = notaFiscal?.DataEmissao.ToString("dd/MM/yyyy"),
                NroFicticio = notaFiscal.NumeroFicticioNF,
                NumeroNotaFiscal = string.Concat(notaFiscal?.Numero.ToString(), " - ", notaFiscal.Serie),
                RazaoSocialCliente = notaFiscal.Cliente.RazaoSocial,
                ValorFrete = notaFiscal.ValorFrete.ToString("C"),
                ValorICMS = notaFiscal.ValorICMS?.ToString("C"),
                ValorProduto = valorProduto.ToString("C"),
                ValorSeguro = notaFiscal.ValorSeguro?.ToString("C"),
                ValorST = notaFiscal.ValorST?.ToString("C"),
                ValorIPI = notaFiscal.ValorIPI?.ToString("C"),
                ValorTotal = notaFiscal?.ValorTotal.ToString("C"),
            };

            foreach (var item in itensDaNota)
            {
                var entradaConferenciaItem = new GarantiaDetalhesEntradaConferenciaItem
                {
                    Referencia = item.Produto.Referencia,
                    DescricaoProduto = item.Produto.Descricao,
                    QuantidadeProduto = item.Quantidade,
                    CFOP = item.CFOP.ToString(),
                    NumeroNotaFiscalOrigem = item.CodigoIntegracaoNFOrigem?.ToString(),
                    Unidade = item.UnidadeMedida.Sigla
                };

                model.ItensDaNota.Add(entradaConferenciaItem);
            }

            return View(model);
        }

        [ApplicationAuthorize(Permissions = Permissions.Garantia.RegistrarRecebimento)]
        public JsonResult ValidarModalRegistroRecebimento(long id)
        {
            //var lote = _uow.LoteRepository.PesquisarLotePorNotaFiscal(id);
            //var notafiscal = _uow.NotaFiscalRepository.GetById(id);

            //if (lote != null || notafiscal.IdNotaFiscalStatus != NotaFiscalStatusEnum.AguardandoRecebimento)
            //{
            //    return Json(new AjaxGenericResultModel
            //    {
            //        Success = false,
            //        Message = "Recebimento da mecadoria já se enconta efetivado no sistema.",
            //    });
            //}

            //ImpressaoItem impressaoItem = _uow.ImpressaoItemRepository.Obter(5);

            //if (!_uow.BOPrinterRepository.ObterPorPerfil(IdPerfilImpressora, impressaoItem.IdImpressaoItem).Any())
            //{
            //    return Json(new AjaxGenericResultModel
            //    {
            //        Success = false,
            //        Message = "Não há impressora configurada para Etiqueta de Recebimento.",
            //    });
            //}

            return Json(new AjaxGenericResultModel
            {
                Success = true
            });
        }

        [HttpGet]
        [ApplicationAuthorize(Permissions = Permissions.Garantia.RegistrarRecebimento)]
        public ActionResult ExibirModalRegistroRecebimento(long id)
        {
            var modal = new GarantiaRegistroRecebimentoViewModel
            {
                IdNotaFiscal = id
            };

            return PartialView("RegistroRecebimento", modal);
        }
    }
}
