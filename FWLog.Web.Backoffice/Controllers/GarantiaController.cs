using AutoMapper;
using FWLog.Data;
using FWLog.Data.Models.FilterCtx;
using FWLog.Data.Models;
using FWLog.Data.Models.DataTablesCtx;
using FWLog.Services.Services;
using System.Collections.Generic;
using System.Web.Mvc;
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
using System.Net;
using Newtonsoft.Json;

namespace FWLog.Web.Backoffice.Controllers
{
    public class GarantiaController : BOBaseController
    {
        private readonly UnitOfWork _uow;
        private readonly GarantiaService _garantiaService;
        private readonly NotaFiscalService _notaFiscalService;
        private readonly ApplicationLogService _applicationLogService;
        private readonly ConferenciaService _conferenciaService;

        public GarantiaController(
            UnitOfWork uow,
            GarantiaService garantiaService,
            NotaFiscalService notaFiscalService,
            ApplicationLogService applicationLogService,
            ConferenciaService conferenciaService)
        {
            _uow = uow;
            _garantiaService = garantiaService;
            _notaFiscalService = notaFiscalService;
            _applicationLogService = applicationLogService;
            _conferenciaService = conferenciaService;
        }

        [ApplicationAuthorize(Permissions = Permissions.Garantia.Listar)]
        public ActionResult Index()
        {
            //TESTE
            _uow.GarantiaRepository.TestSankya();

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
        public ActionResult DetalhesEntradaConferenciaGarantia(int id)
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
            try
            {
                var garantia = _uow.GarantiaRepository.BuscarGarantiaPorIdNotaFiscal(id);
                var notafiscal = _uow.NotaFiscalRepository.GetById(id);

                if (garantia != null || notafiscal.IdNotaFiscalStatus != NotaFiscalStatusEnum.AguardandoRecebimento)
                {
                    return Json(new AjaxGenericResultModel
                    {
                        Success = false,
                        Message = "A nota fiscal já foi recebida por outro usuário, verifique antes de continuar.",
                    });
                }

                ImpressaoItem impressaoItem = _uow.ImpressaoItemRepository.Obter(10);

                if (!_uow.BOPrinterRepository.ObterPorPerfil(IdPerfilImpressora, impressaoItem.IdImpressaoItem).Any())
                {
                    return Json(new AjaxGenericResultModel
                    {
                        Success = false,
                        Message = "Não há impressora configurada para Etiqueta de Recebimento.",
                    });
                }

                return Json(new AjaxGenericResultModel
                {
                    Success = true,
                });
            }
            catch (Exception e)
            {
                _applicationLogService.Error(ApplicationEnum.BackOffice, e);

                return Json(new AjaxGenericResultModel
                {
                    Success = false,
                    Message = "Algo inesperado ocorreu, atualize a página e tente novamente."
                });
            }
        }

        [HttpGet]
        [ApplicationAuthorize(Permissions = Permissions.Garantia.RegistrarRecebimento)]
        public ActionResult ExibirModalRegistroRecebimento(long id)
        {
            var modal = new GarantiaRegistroRecebimentoViewModel
            {
                IdNotaFiscal = id
            };

            return PartialView("RegistroRecebimentoGarantia", modal);
        }

        [HttpPost]
        [ApplicationAuthorize(Permissions = Permissions.Garantia.RegistrarRecebimento)]
        public JsonResult ValidarNotaFiscalRegistro(string chaveAcesso, long idNotaFiscal, long? numeroNF)
        {
            try
            {
                var notafiscal = _uow.NotaFiscalRepository.GetById(idNotaFiscal);

                if (notafiscal.ChaveAcesso != chaveAcesso)
                {
                    return Json(new AjaxGenericResultModel
                    {
                        Success = false,
                        Message = "A Chave de Acesso não condiz com a chave cadastrada da nota fiscal cadastrada."
                    });
                }
                else if (notafiscal.Numero != numeroNF)
                {
                    return Json(new AjaxGenericResultModel
                    {
                        Success = false,
                        Message = "O número da nota não condiz com o número da nota fiscal cadastrada."
                    });
                }

                var garantia = _uow.GarantiaRepository.BuscarGarantiaPorIdNotaFiscal(idNotaFiscal);

                if (garantia != null)
                {
                    return Json(new AjaxGenericResultModel
                    {
                        Success = false,
                        Message = "Recebimento da garantia já efetivado no sistema."
                    });
                }

                return Json(new AjaxGenericResultModel
                {
                    Success = true,
                });
            }
            catch (Exception e)
            {
                _applicationLogService.Error(ApplicationEnum.BackOffice, e);

                return Json(new AjaxGenericResultModel
                {
                    Success = false,
                    Message = "Algo inesperado ocorreu, atualize a página e tente novamente."
                });
            }
        }

        [HttpPost]
        [ApplicationAuthorize(Permissions = Permissions.Garantia.RegistrarRecebimento)]
        public async Task<JsonResult> RegistrarRecebimentoNota(long idNotaFiscal, string observacao, string informacaoTransportadora)
        {
            var garantia = _uow.GarantiaRepository.BuscarGarantiaPorIdNotaFiscal(idNotaFiscal);
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

        [HttpPost]
        [ApplicationAuthorize(Permissions = Permissions.Garantia.ConferirGarantia)]
        public async Task<JsonResult> ValidarInicioConferenciaDaGarantia(long id)
        {
            try
            {
                var garantia = _uow.GarantiaRepository.GetById(id);

                //Valida a Garantia.
                if (garantia == null)
                {
                    return Json(new AjaxGenericResultModel
                    {
                        Success = false,
                        Message = "A garantia ainda não foi recebida."
                    });
                }

                //Verifica se a garantia já foi conferido durante o processo de conferência.
                if (garantia.IdGarantiaStatus != GarantiaStatusEnum.Recebido && garantia.IdGarantiaStatus != GarantiaStatusEnum.Conferencia)
                {
                    return Json(new AjaxGenericResultModel
                    {
                        Success = false,
                        Message = $"A conferência da garantia: {garantia.IdGarantia} já foi finalizada.",
                    });
                }

                //ImpressaoItem impressaoItem = _uow.ImpressaoItemRepository.Obter(10);

                //if(impressaoItem == null)
                //{
                //    return Json(new AjaxGenericResultModel
                //    {
                //        Success = false,
                //        Message = "Não foi encontrado item configurado para impressão de Etiqueta de Garantia.",
                //    });
                //}

                //if (!_uow.BOPrinterRepository.ObterPorPerfil(IdPerfilImpressora, impressaoItem.IdImpressaoItem).Any())
                //{
                //    return Json(new AjaxGenericResultModel
                //    {
                //        Success = false,
                //        Message = "Não há impressora configurada para Etiqueta de Garantia.",
                //    });
                //}

                NotaFiscal notaFiscal = _uow.NotaFiscalRepository.GetById(garantia.IdNotaFiscal);

                //Valida a Nota Fiscal.
                if (notaFiscal == null)
                {
                    return Json(new AjaxGenericResultModel
                    {
                        Success = false,
                        Message = "Não foi possível buscar a Nota Fiscal. Por favor, tente novamente!"
                    });
                }

                var mensagem = string.Empty;

                if (User.Identity.GetUserId() != garantia.IdUsuarioConferente)
                {
                    mensagem = "A nota fiscal já está em conferência por outro usuário, verifique antes de continuar.";
                }

                return Json(new AjaxGenericResultModel
                {
                    Success = true,
                    Message = mensagem
                });
            }
            catch (Exception e)
            {
                _applicationLogService.Error(ApplicationEnum.BackOffice, e);

                return Json(new AjaxGenericResultModel
                {
                    Success = false,
                    Message = "Algo inesperado ocorreu, atualize a página e tente novamente."
                });
            }
        }

        [HttpGet]
        [ApplicationAuthorize(Permissions = Permissions.Garantia.ConferirGarantia)]
        public ActionResult EntradaConferenciaGarantia(long id)
        {
            var garantia = _uow.GarantiaRepository.GetById(id);
            var notafiscal = _uow.NotaFiscalRepository.GetById(garantia.IdNotaFiscal);

            //Valida o Garantia.
            if (garantia == null)
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "Garantia não encontrado. Por favor, tente novamente!");

            //Captura o Usuário que está iniciando a conferência.
            var usuario = _uow.PerfilUsuarioRepository.GetByUserId(User.Identity.GetUserId());

            var empresaConfig = _uow.EmpresaConfigRepository.ConsultarPorIdEmpresa(IdEmpresa);

            if (empresaConfig == null)
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "As configurações da empresa não foram encontradas. Por favor, tente novamente!");

            var model = new GarantiaEntradaConferenciaViewModel
            {
                IdNotaFiscal = garantia.NotaFiscal.IdNotaFiscal,
                IdGarantia = garantia.IdGarantia,
                NumeroNotaFiscal = string.Concat(garantia.NotaFiscal.Numero, " - ", garantia.NotaFiscal.Serie),
                IdUuarioConferente = usuario.UsuarioId,
                NomeConferente = usuario.Nome,
                DataDaSolicitacao = garantia.DataRecebimento.ToString("dd/MM/yyyy"),
                Cliente = notafiscal.Cliente.RazaoSocial,
                Fornecedor = string.Concat(notafiscal.Fornecedor.IdFornecedor, " - ", notafiscal.Fornecedor.NomeFantasia),
                Representante = notafiscal.Cliente.RepresentanteExterno.Nome ?? notafiscal.Cliente.RepresentanteInterno.Nome
                //Cliente = garantia
                //DataHoraRecebimento = lote.DataRecebimento.ToString("dd/MM/yyyy HH:mm"),
                //NomeFornecedor = lote.NotaFiscal.Fornecedor.NomeFantasia,
                //QuantidadeVolume = lote.QuantidadeVolume,
                //TipoConferencia = empresaConfig.TipoConferencia.Descricao,
                //IdTipoConferencia = empresaConfig.TipoConferencia.IdTipoConferencia.GetHashCode()
            };

            if (!garantia.DataInicioConferencia.HasValue)
            {
                garantia.DataInicioConferencia = DateTime.Now;
                _uow.SaveChanges();
            }

            ////Se o tipo da conferência for, o usuário não poderá informar a quantidade por caixa e quantidade de caixa.
            ////Sabendo disso, atribui 1 para os campos.
            //if (empresaConfig.TipoConferencia.IdTipoConferencia == TipoConferenciaEnum.ConferenciaCemPorcento)
            //{
            //    model.QuantidadePorCaixa = 1;
            //    model.QuantidadeCaixa = 1;
            //}

            return View(model);
        }

        [HttpPost]
        [ApplicationAuthorize(Permissions = Permissions.Garantia.ConferirGarantia)]
        public async Task<ActionResult> ObterDadosReferenciaConferenciaGarantia(string codigoBarrasOuReferencia, long idGarantia, long idNotaFiscal)
        {
            //Validações do produto.
            var conferencia = _conferenciaService.ValidarProdutoGarantia(idGarantia, idNotaFiscal, codigoBarrasOuReferencia, IdEmpresa);

            if (!conferencia.Sucesso)
            {
                return Json(new AjaxGenericResultModel
                {
                    Success = conferencia.Sucesso,
                    Message = conferencia.Mensagem
                });
            }

            //Captura as quantidade conferida e não conferida do lote.
            int quantidadeConferida = 0;
            int quantidadeNaoConferida = 0;

            //_conferenciaService.ConsultarQuantidadeConferidaENaoConferidaGarantia(conferencia.Garantia, conferencia.Produto, ref quantidadeConferida, ref quantidadeNaoConferida);

            //Captura o Usuário que está iniciando a conferência novamente.
            var usuario = _uow.PerfilUsuarioRepository.GetByUserId(User.Identity.GetUserId());

            var model = new GarantiaEntradaConferenciaViewModel
            {
                Descricao = conferencia.Produto.Descricao,
                Fabricante = conferencia.Produto.NomeFabricante,
                Unidade = conferencia.Produto.UnidadeMedida.Sigla

                //IdNotaFiscal = conferencia.Lote.NotaFiscal.IdNotaFiscal,
                //IdLote = conferencia.Lote.IdLote,
                //NumeroNotaFiscal = string.Concat(conferencia.Lote.NotaFiscal.Numero, " - ", conferencia.Lote.NotaFiscal.Serie),
                //IdUuarioConferente = usuario.UsuarioId,
                //NomeConferente = usuario.Nome,
                //DataHoraRecebimento = conferencia.Lote.DataRecebimento.ToString("dd/MM/yyyy HH:mm"),
                //NomeFornecedor = conferencia.Lote.NotaFiscal.Fornecedor.NomeFantasia,
                //QuantidadeVolume = conferencia.Lote.QuantidadeVolume,
                //TipoConferencia = conferencia.EmpresaConfig.TipoConferencia.Descricao,
                //IdTipoConferencia = conferencia.EmpresaConfig.TipoConferencia.IdTipoConferencia.GetHashCode(),
                //Referencia = conferencia.Produto.Referencia,
                //DescricaoReferencia = conferencia.Produto.Descricao,
                //Embalagem = conferencia.Produto.MultiploVenda.ToString("N2"),
                //Unidade = conferencia.Produto.UnidadeMedida.Sigla,
                //QuantidadeEstoque = conferencia.ProdutoEstoque == null ? 0 : conferencia.ProdutoEstoque.Saldo,
                //QuantidadeNaoConferida = quantidadeNaoConferida,
                //QuantidadeConferida = quantidadeConferida,
                //InicioConferencia = DateTime.Now.ToString(),
                //QuantidadePorCaixa = null,
                //Multiplo = conferencia.Produto.MultiploVenda,
                //QuantidadeReservada = await _produtoService.ConsultarQuantidadeReservada(conferencia.Produto.IdProduto, IdEmpresa),
                //MediaVenda = conferencia.ProdutoEstoque.MediaVenda.HasValue ? conferencia.ProdutoEstoque.MediaVenda.Value.ToString("N2") : string.Empty,
                //QuantidadeCaixa = conferencia.EmpresaConfig.IdTipoConferencia == TipoConferenciaEnum.ConferenciaCemPorcento ? 1 : null as int?
            };

            string json = JsonConvert.SerializeObject(model);

            return Json(new AjaxGenericResultModel
            {
                Success = true,
                Message = conferencia.Mensagem,
                Data = json
            });
        }

        public AutoCompleteResult BuscarMotivoLaudoAutoComplete(string query)
        {
            try
            {
                int takeCount = 10;
                IEnumerable<MotivoLaudo> search = _uow.MotivoLaudoRepository.SearchByDescrption(query, takeCount);
                var suggestions = search.Select(x => new AutoCompleteSuggestionModel(value: x.Descricao, data: x.IdMotivoLaudo));
                var response = new AutoCompleteResponseModel(suggestions);

                return AutoCompleteResult.FromModel(response);
            }
            catch (Exception e)
            {
                throw;
            }
        }
    }
}