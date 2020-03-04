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
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using FWLog.Data.EnumsAndConsts;

namespace FWLog.Web.Backoffice.Controllers
{
    public class GarantiaController : BOBaseController
    {
        UnitOfWork _uow;
        GarantiaService _garantiaService;
        NotaFiscalService _notaFiscalService;
        ApplicationLogService _applicationLogService;

        public GarantiaController(UnitOfWork uow, GarantiaService garantiaService,NotaFiscalService notaFiscalService, ApplicationLogService applicationLogService)
        {
            _uow = uow;
            _garantiaService = garantiaService;
            _notaFiscalService = notaFiscalService;
            _applicationLogService = applicationLogService;
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
            var garantia = _uow.GarantiaRepository.PesquisarGarantiaPorIdNotaFiscal(id);
            var notafiscal = _uow.NotaFiscalRepository.GetById(id);

            if (garantia != null || notafiscal.IdNotaFiscalStatus != NotaFiscalStatusEnum.AguardandoRecebimento)
            {
                return Json(new AjaxGenericResultModel
                {
                    Success = false,
                    Message = "A nota fiscal já foi recebida por outro usuário, verifique antes de continuar.",
                });
            }

            //ImpressaoItem impressaoItem = _uow.ImpressaoItemRepository.Obter(9);

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

        [HttpPost]
        [ApplicationAuthorize(Permissions = Permissions.Garantia.RegistrarRecebimento)]
        public JsonResult ValidarNotaFiscalRegistro(string chaveAcesso, long idNotaFiscal, long? numeroNF)
        {
            var notafiscal = _uow.NotaFiscalRepository.GetById(idNotaFiscal);

            if (notafiscal.ChaveAcesso != chaveAcesso )
            {
                return Json(new AjaxGenericResultModel
                {
                    Success = false,
                    Message = "A Chave de Acesso não condiz com a chave cadastrada da nota fiscal cadastrada."
                });
            }
            else if(notafiscal.Numero != numeroNF)
            {
                return Json(new AjaxGenericResultModel
                {
                    Success = false,
                    Message = "O número da nota não condiz com o número da nota fiscal cadastrada."
                });
            }

            ////var lote = _uow.LoteRepository.PesquisarLotePorNotaFiscal(idNotaFiscal);

            ////if (lote != null)
            ////{
            ////    return Json(new AjaxGenericResultModel
            ////    {
            ////        Success = false,
            ////        Message = "Recebimento da mecadoria já efetivado no sistema."
            ////    });
            ////}

            return Json(new AjaxGenericResultModel
            {
                Success = true,
            });
        }

        [HttpPost]
        [ApplicationAuthorize(Permissions = Permissions.Garantia.RegistrarRecebimento)]
        public async Task<JsonResult> RegistrarRecebimentoNota(long idNotaFiscal, string observacao, string informacaoTransportadora)
        {
            var garantia = _uow.GarantiaRepository.PesquisarGarantiaPorIdNotaFiscal(idNotaFiscal);
            var notafiscal = _uow.NotaFiscalRepository.GetById(idNotaFiscal);

            if (garantia != null || notafiscal.IdNotaFiscalStatus != NotaFiscalStatusEnum.AguardandoRecebimento)
            {
                return Json(new AjaxGenericResultModel
                {
                    Success = false,
                    Message = "A nota fiscal já foi recebida por outro usuário, verifique antes de continuar.",
                });
            }

            if (!(idNotaFiscal > 0))
            {
                return Json(new AjaxGenericResultModel
                {
                    Success = false,
                    Message = "Selecione a nota fiscal."
                });
            }

            try
            {
                await _notaFiscalService.RegistrarRecebimentoNotaFiscalGarantia(idNotaFiscal, User.Identity.GetUserId(), observacao, informacaoTransportadora).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                _applicationLogService.Error(ApplicationEnum.BackOffice, e);

                return Json(new AjaxGenericResultModel
                {
                    Success = false,
                    Message = "Não foi possível atualizar o status da Nota Fiscal no Sankhya. Tente novamente."
                });
            }

            return Json(new AjaxGenericResultModel
            {
                Success = true,
                Message = "Recebimento da nota fiscal registrado com sucesso. Garantira gerada"
            });
        }
    }
}
