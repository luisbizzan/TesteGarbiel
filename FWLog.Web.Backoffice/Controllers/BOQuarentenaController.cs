﻿using FWLog.AspNet.Identity;
using FWLog.Data;
using FWLog.Data.EnumsAndConsts;
using FWLog.Data.Models;
using FWLog.Data.Models.FilterCtx;
using FWLog.Data.Models.GeneralCtx;
using FWLog.Services.Model.Relatorios;
using FWLog.Services.Services;
using FWLog.Web.Backoffice.Helpers;
using FWLog.Web.Backoffice.Models.BOQuarentenaCtx;
using FWLog.Web.Backoffice.Models.CommonCtx;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using log4net;

namespace FWLog.Web.Backoffice.Controllers
{
    public class BOQuarentenaController : BOBaseController
    {
        private readonly UnitOfWork _uow;
        private ILog _log;
        private readonly QuarentenaService _quarentenaService;

        public BOQuarentenaController(UnitOfWork uow, QuarentenaService quarentenaService, ILog log)
        {
            _uow = uow;
            _quarentenaService = quarentenaService;
            _log = log;
        }

        private SelectList Status
        {
            get
            {
                if (status == null)
                {
                    status = new SelectList(_uow.QuarentenaStatusRepository.Todos().Select(x => new SelectListItem
                    {
                        Value = x.IdQuarentenaStatus.GetHashCode().ToString(),
                        Text = x.Descricao
                    }), "Value", "Text");
                }

                return status;
            }
        }
        private SelectList status;

        private void setViewBags()
        {
            ViewBag.Status = Status;
        }

        // GET: BOQuarentena
        public ActionResult Index()
        {
            var model = new BOQuarentenaListViewModel
            {
                Filter = new BOQuarentenaFilterViewModel()
                {
                    ListaQuarentenaStatus = new SelectList(
                    _uow.QuarentenaStatusRepository.Todos().Select(x => new SelectListItem
                    {
                        Value = x.IdQuarentenaStatus.GetHashCode().ToString(),
                        Text = x.Descricao
                    }), "Value", "Text")
                }
            };

            return View(model);
        }

        public ActionResult PageData(DataTableFilter<BOQuarentenaFilterViewModel> model)
        {
            int totalRecords = 0;
            int totalRecordsFiltered = 0;

            var query = _uow.QuarentenaRepository.All();

            totalRecords = query.Count();

            if (!string.IsNullOrEmpty(model.CustomFilter.ChaveAcesso))
                query = query.Where(x => x.Lote.NotaFiscal.ChaveAcesso.Contains(model.CustomFilter.ChaveAcesso));

            if (model.CustomFilter.Lote.HasValue)
                query = query.Where(x => x.IdLote == model.CustomFilter.Lote.Value);

            if (model.CustomFilter.Nota.HasValue)
                query = query.Where(x => x.Lote.NotaFiscal.Numero == model.CustomFilter.Nota);

            if (model.CustomFilter.IdFornecedor.HasValue)
                query = query.Where(x => x.Lote.NotaFiscal.IdFornecedor == model.CustomFilter.IdFornecedor);

            if (model.CustomFilter.IdQuarentenaStatus.HasValue)
                query = query.Where(x => (int)x.QuarentenaStatus.IdQuarentenaStatus == model.CustomFilter.IdQuarentenaStatus);

            if (model.CustomFilter.DataAberturaInicial.HasValue)
            {
                DateTime dataInicial = new DateTime(model.CustomFilter.DataAberturaInicial.Value.Year, model.CustomFilter.DataAberturaInicial.Value.Month, model.CustomFilter.DataAberturaInicial.Value.Day,
                    00, 00, 00);
                query = query.Where(x => x.DataAbertura >= dataInicial);
            }

            if (model.CustomFilter.DataAberturaFinal.HasValue)
            {
                DateTime dataFinal = new DateTime(model.CustomFilter.DataAberturaFinal.Value.Year, model.CustomFilter.DataAberturaFinal.Value.Month, model.CustomFilter.DataAberturaFinal.Value.Day,
                    23, 59, 59);
                query = query.Where(x => x.DataAbertura <= dataFinal);
            }

            if (model.CustomFilter.DataEncerramentoInicial.HasValue)
            {
                DateTime dataEncerramentoInicial = new DateTime(model.CustomFilter.DataEncerramentoInicial.Value.Year, model.CustomFilter.DataEncerramentoInicial.Value.Month, model.CustomFilter.DataEncerramentoInicial.Value.Day,
                    00, 00, 00);
                query = query.Where(x => x.DataEncerramento >= dataEncerramentoInicial);
            }

            if (model.CustomFilter.DataEncerramentoFinal.HasValue)
            {
                DateTime prazoEncerramentoFinal = new DateTime(model.CustomFilter.DataEncerramentoFinal.Value.Year, model.CustomFilter.DataEncerramentoFinal.Value.Month, model.CustomFilter.DataEncerramentoFinal.Value.Day,
                    23, 59, 59);
                query = query.Where(x => x.DataEncerramento <= prazoEncerramentoFinal);
            }

            IEnumerable<BOQuarentenaListItemViewModel> list = query.ToList()
                .Select(x => new BOQuarentenaListItemViewModel
                {
                    IdQuarentena       = x.IdQuarentena,
                    Lote               = x.IdLote,
                    ChaveAcesso        = x.Lote.NotaFiscal.ChaveAcesso,
                    Nota               = x.Lote.NotaFiscal.Numero,
                    Fornecedor         = x.Lote.NotaFiscal.Fornecedor.NomeFantasia,
                    DataAbertura       = x.DataAbertura.ToString("dd/MM/yyyy"),
                    DataEncerramento   = x.DataEncerramento.HasValue ? x.DataEncerramento.Value.ToString("dd/MM/yyyy") : string.Empty,
                    Atraso             = x.DataAbertura.Subtract(x.DataEncerramento ?? DateTime.Now).Days,
                    Status             = x.QuarentenaStatus.Descricao,
                    LoteStatus         = x.Lote.IdLoteStatus.ToString(),
                    IdQuarentenaStatus = (int)x.IdQuarentenaStatus
                });

            if (model.CustomFilter.Atraso.HasValue)
            {
                list = list.Where(x => x.Atraso == model.CustomFilter.Atraso);
            }

            totalRecordsFiltered = list.Count();

            var result = list
                .OrderBy(model.OrderByColumn, model.OrderByDirection)
                .Skip(model.Start)
                .Take(model.Length);

            return DataTableResult.FromModel(new DataTableResponseModel()
            {
                Draw = model.Draw,
                RecordsTotal = totalRecords,
                RecordsFiltered = totalRecordsFiltered,
                Data = result
            });
        }

        public JsonResult ValidarModalDetalhesQuarentena(long id)
        {
            bool existe = _uow.QuarentenaRepository.Any(x => x.IdQuarentena == id);

            if (!existe)
            {
                return Json(new AjaxGenericResultModel
                {
                    Success = false,
                    Message = "Não foi possível buscar Nota Fiscal."
                });
            }

            return Json(new AjaxGenericResultModel
            {
                Success = true
            });
        }

        [HttpPost]
        public JsonResult ValidarChaveAcessoIgualDaNFe(DetalhesQuarentenaViewModel model)
        {
            bool chaveNfeIgual = false;
            //bool existe = _uow.QuarentenaRepository.Any(x => x.Lote.NotaFiscal.ChaveAcesso == chaveAcesso);

            Quarentena quarentena = _uow.QuarentenaRepository.GetById(model.IdQuarentena);
            Lote       lote       = _uow.LoteRepository.GetById(quarentena.IdLote);


            if (lote.IdLoteStatus.GetHashCode() == 12)
            {
                //Valida chave de cesso
                var chaveValida = false;

                if (string.IsNullOrWhiteSpace(model.ChaveAcesso))
                {
                    return Json(new AjaxGenericResultModel
                    {
                        Success = false,
                        Message = "Informe a chave de acesso da NF-e.",
                    });
                }

                if (model.ChaveAcesso.Length != 44)
                {
                    return Json(new AjaxGenericResultModel
                    {
                        Success = false,
                        Message = "Chave de acesso não possui 44 digitos.",
                    });
                }


                if (lote.NotaFiscal.ChaveAcesso == model.ChaveAcesso)
                    chaveNfeIgual = true;
                else
                    chaveNfeIgual = false;

                if (!chaveNfeIgual)
                {
                    return Json(new AjaxGenericResultModel
                    {
                        Success = false,
                        Message = "A chave de acesso informada não confere com a chave Nota Fiscal."
                    });
                }
            }


            return Json(new AjaxGenericResultModel
            {
                Success = true
            });
        }

        [HttpGet]
        public ActionResult DetalhesQuarentena(long id)
        {
            setViewBags();
            var entidade = _uow.QuarentenaRepository.GetById(id);

            var model = new DetalhesQuarentenaViewModel
            {
                NotaSerie = entidade.Lote.NotaFiscal.Numero + " - " + entidade.Lote.NotaFiscal.Serie,
                Lote = entidade.IdLote.ToString(),
                LoteStatus = entidade.Lote.LoteStatus.Descricao,
                IdQuarentena = entidade.IdQuarentena,
                IdStatus = entidade.QuarentenaStatus.IdQuarentenaStatus,
                DataAbertura = entidade.DataAbertura.ToString("dd/MM/yyyy"),
                DataEncerramento = entidade.DataEncerramento.HasValue ? entidade.DataEncerramento.Value.ToString("dd/MM/yyyy") : string.Empty,
                Observacao = entidade.Observacao,
            };

            model.ObservacaoDivergencia = entidade.Lote.ObservacaoDivergencia;

            List<LoteDivergencia> loteDivergencias = _uow.LoteDivergenciaRepository.Todos().Where(x => x.IdLote == entidade.IdLote).ToList();

            foreach (LoteDivergencia divergencia in loteDivergencias)
            {
                NotaFiscalItem nfItem = divergencia.NotaFiscal.NotaFiscalItens.Where(w => w.Produto.IdProduto == divergencia.Produto.IdProduto).FirstOrDefault();

                var divergenciaItem = new DivergenciaItemViewModel
                {
                    Descricao = divergencia.Produto.Descricao,
                    Referencia = divergencia.Produto.Referencia,
                    QuantidadeDevolucao = divergencia.QuantidadeDevolucao,
                    QuantidadeConferencia = divergencia.QuantidadeConferencia,
                    QuantidadeMais = divergencia.QuantidadeConferenciaMais.GetValueOrDefault(),
                    QuantidadeMenos = divergencia.QuantidadeConferenciaMenos.GetValueOrDefault(),
                    QuantidadeNotaFiscal = nfItem?.Quantidade ?? 0,
                    QuantidadeMaisTratado = divergencia.QuantidadeDivergenciaMais.GetValueOrDefault(),
                    QuantidadeMenosTratado = divergencia.QuantidadeDivergenciaMenos.GetValueOrDefault()
                };

                model.Divergencias.Add(divergenciaItem);
            }

            return View(model);
        }

        [HttpPost]
        public JsonResult DetalhesQuarentena(DetalhesQuarentenaViewModel model)
        {
            ValidateModel(model);
            string mensagemErro = null;

            if (model.IdStatus == QuarentenaStatusEnum.Retirado)
            {
                if (string.IsNullOrEmpty(model.CodigoConfirmacao))
                {
                    ModelState.AddModelError(nameof(model.CodigoConfirmacao), (mensagemErro = "O código de confirmação é obrigatório para retirar a mercadoria."));
                }
                else
                {
                    bool existeCod = _uow.QuarentenaRepository.Any(x => x.IdQuarentena == model.IdQuarentena && x.CodigoConfirmacao == model.CodigoConfirmacao);

                    if (!existeCod)
                    {
                        ModelState.AddModelError(nameof(model.CodigoConfirmacao), (mensagemErro = "O código de confirmação está incorreto!"));
                    }
                }
            }

            //Valida a model.
            if (!ModelState.IsValid)
            {
                return Json(new AjaxGenericResultModel
                {
                    Success = false,
                    Message = mensagemErro ?? Resources.CommonStrings.RegisterEditedErrorMessage
                });
            }

            try
            {
                //Captura os dados para atualiza-los posteriormente.
                Quarentena entidade = _uow.QuarentenaRepository.GetById(model.IdQuarentena);

                //Captura os dados "antigo" para o log.
                Quarentena old = new Quarentena
                {
                    DataAbertura = entidade.DataAbertura,
                    DataEncerramento = entidade.DataEncerramento,
                    IdLote = entidade.IdLote,
                    IdQuarentena = entidade.IdQuarentena,
                    IdQuarentenaStatus = entidade.IdQuarentenaStatus,
                    CodigoConfirmacao = entidade.CodigoConfirmacao,
                    Observacao = entidade.Observacao
                };

                //Verifica se o status anterior era Encaminhado para Auditoria e o atual Aberto.
                //Caso seja, informa o usuário que a ação não é permitida.
                if (old.IdQuarentenaStatus == QuarentenaStatusEnum.EncaminhadoAuditoria && (model.IdStatus == QuarentenaStatusEnum.Aberto || model.IdStatus == QuarentenaStatusEnum.Retirado))
                {
                    ModelState.AddModelError(nameof(model.IdStatus), (mensagemErro = "A quarentena foi encaminhada para Auditoria. Não é permitido atualizar o status para " + model.IdStatus.ToString() + "."));

                    return Json(new AjaxGenericResultModel
                    {
                        Success = false,
                        Message = mensagemErro ?? Resources.CommonStrings.RegisterEditedErrorMessage
                    });
                }

                //Atualiza o status e observação da quarentena.
                entidade.IdQuarentenaStatus = model.IdStatus;
                entidade.Observacao = model.Observacao;

                //Verifica se o status é Retirado ou Finalizado.
                //Caso seja, insere a data de encerramento pois, considera-se que a mercadoria foi retirada ou alguma outra ação feita (finalizado).
                if (model.IdStatus == QuarentenaStatusEnum.Retirado || model.IdStatus == QuarentenaStatusEnum.Finalizado)
                {
                    entidade.DataEncerramento = DateTime.Now;
                }

                _uow.QuarentenaRepository.Update(entidade, IdUsuario);

                var userInfo = new BackOfficeUserInfo();
                
                return Json(new AjaxGenericResultModel
                {
                    Success = true,
                    Message = Resources.CommonStrings.RegisterEditedSuccessMessage
                });
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);

                return Json(new AjaxGenericResultModel
                {
                    Success = false,
                    Message = Resources.CommonStrings.RegisterEditedErrorMessage
                });
            }
        }

        [HttpPost]
        public JsonResult TermoResponsabilidade(BOImprimirTermoResponsabilidadeViewModel viewModel)
        {
            try
            {
                ValidateModel(viewModel);

                var userInfo = new BackOfficeUserInfo();

                var userLog = new UserLog
                {
                    IP = userInfo.IP,
                    UserId = IdUsuario
                };

                var request = new TermoResponsabilidadeRequest
                {
                    IdQuarentena = viewModel.IdQuarentena,
                    IdImpressora = viewModel.IdImpressora,
                    NomeUsuario = _uow.PerfilUsuarioRepository.GetByUserId(User.Identity.GetUserId())?.Nome,
                    UserLog = userLog
                };

                _quarentenaService.ImprimirTermoResponsabilidade(request);

                return Json(new AjaxGenericResultModel
                {
                    Success = true,
                    Message = "Impressão enviada com sucesso."
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);

                return Json(new AjaxGenericResultModel
                {
                    Success = false,
                    Message = "Ocorreu um erro na impressão."
                }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Historico(long id)
        {
            var entidade = _uow.QuarentenaRepository.GetById(id);

            var model = new HistoricoQuarentenaViewModel
            {
                NotaSerie = entidade.Lote.NotaFiscal.Numero + " - " + entidade.Lote.NotaFiscal.Serie,
                Lote = entidade.IdLote.ToString(),
                ChaveAcesso = entidade.Lote.NotaFiscal.ChaveAcesso,
                LoteStatus = entidade.Lote.LoteStatus.Descricao,
                IdQuarentena = entidade.IdQuarentena,
                IdStatus = entidade.QuarentenaStatus.IdQuarentenaStatus,
                DataAbertura = entidade.DataAbertura.ToString("dd/MM/yyyy"),
                DataEncerramento = entidade.DataEncerramento.HasValue ? entidade.DataEncerramento.Value.ToString("dd/MM/yyyy") : string.Empty,
                Itens = _uow.QuarentenaHistoricoRepository.QuarentenaHistoricos().Where(x => x.IdQuarentena == id).OrderByDescending(o => o.Data).ToList().Select(x => new HistoricoQuarentenaItemViewModel
                {
                    Data = x.Data.ToString("dd/MM/yyyy"),
                    Usuario = x.NomeUsuario,
                    Descricao = x.Descricao
                }).ToList()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<JsonResult> ValidarPermissao(string acao)
        {
            try
            {
                bool permissao = false;
                string mensagem = "";

                switch (acao)
                {
                    case "AtualizarStatus":
                        permissao = UserManager.GetPermissions(User.Identity.GetUserId()).Contains(Permissions.RecebimentoQuarentena.AtualizarStatus);
                        mensagem = "O usuário informado não possui permissão para atualizar o status. Solicite a permissão para o Administrador.";
                        break;
                    case "EmitirTermoResponsabilidade":
                        permissao = UserManager.GetPermissions(User.Identity.GetUserId()).Contains(Permissions.RecebimentoQuarentena.EmitirTermoResponsabilidade);
                        mensagem = "O usuário informado não possui permissão para emitir o termo de responsabilidade. Solicite a permissão para o Administrador.";
                        break;
                    case "ConsultarHistorico":
                        permissao = UserManager.GetPermissions(User.Identity.GetUserId()).Contains(Permissions.RecebimentoQuarentena.ConsultarHistorico);
                        mensagem = "O usuário informado não possui permissão para consultar o histórico. Solicite a permissão para o Administrador.";
                        break;
                }

                if (permissao)
                {
                    return Json(new AjaxGenericResultModel
                    {
                        Success = true,
                        Message = string.Empty
                    });
                }
                else
                {
                    return Json(new AjaxGenericResultModel
                    {
                        Success = false,
                        Message = mensagem
                    });
                }
            }
            catch (Exception e)
            {
                _log.Error(e.Message, e);

                return Json(new AjaxGenericResultModel
                {
                    Success = false,
                    Message = "Não foi possível validar a permissão do usuário. Por favor, tente novamente!"
                });
            }

        }
    }
}