﻿using AutoMapper;
using DartDigital.Library.Exceptions;
using ExtensionMethods.String;
using FWLog.AspNet.Identity;
using FWLog.Data;
using FWLog.Data.Models.DataTablesCtx;
using FWLog.Data.Models.FilterCtx;
using FWLog.Web.Backoffice.Helpers;
using FWLog.Web.Backoffice.Models.CommonCtx;
using FWLog.Web.Backoffice.Models.PedidoVendaVolumeCtx;
using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace FWLog.Web.Backoffice.Controllers
{
    public class PedidoVendaProdutoController : BOBaseController
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly ILog _log;

        public PedidoVendaProdutoController(UnitOfWork unitOfWork, ILog log)
        {
            _unitOfWork = unitOfWork;
            _log = log;
        }

        [HttpPost]
        [ApplicationAuthorize(Permissions = Permissions.RelatoriosExpedicao.RelatorioPedidosCadastrarVolume)]
        public ActionResult ConsultarDadosProduto(long idPedidoVendaVolume, long idProduto)
        {
            try
            {
                var result = _unitOfWork.PedidoVendaProdutoRepository.ObterPorIdPedidoVendaVolumeEIdProduto(idPedidoVendaVolume, idProduto);

                if (result == null)
                {
                    throw new BusinessException("Produto não encontrado no volume.");
                }

                var model = new ConsultarDadosProdutoModelView()
                {
                    IdGrupoCorredorArmazenagem = result.PedidoVendaVolume.IdGrupoCorredorArmazenagem,
                    IdProduto = result.IdProduto,
                    MultiploVenda = result.Produto.MultiploVenda,
                    QuantidadeSeparar = result.QtdSeparar,
                    CorredorInicio = result.PedidoVendaVolume.CorredorInicio,
                    CorredorFim = result.PedidoVendaVolume.CorredorFim,
                    IdLote = result.IdLote
                };

                return Json(new AjaxGenericResultModel
                {
                    Success = true,
                    Message = "",
                    Data = JsonConvert.SerializeObject(model)
                });
            }
            catch (Exception e)
            {
                _log.Error(e.Message, e);

                return Json(new AjaxGenericResultModel
                {
                    Success = false,
                    Message = e is BusinessException ? e.Message : "Ocorreu um erro na consulta dos dados do volume."
                }, JsonRequestBehavior.DenyGet);
            }
        }

        [HttpPost]
        [ApplicationAuthorize(Permissions = Permissions.RelatoriosExpedicao.RelatorioPedidosCadastrarVolume)]
        public ActionResult ConsultarVolumeProdutoQtd(long idPedidoVendaVolume, long idProduto)
        {
            try
            {
                var result = _unitOfWork.PedidoVendaProdutoRepository.ObterPorIdPedidoVendaVolumeEIdProduto(idPedidoVendaVolume, idProduto);

                if (result == null)
                {
                    throw new BusinessException("Produto não encontrado no volume.");
                }

                return Json(new AjaxGenericResultModel
                {
                    Success = true,
                    Message = "",
                    Data = result.QtdSeparar.ToString()
                });
            }
            catch (Exception e)
            {
                _log.Error(e.Message, e);

                return Json(new AjaxGenericResultModel
                {
                    Success = false,
                    Message = e is BusinessException ? e.Message : "Ocorreu um erro na consulta da quantidade do volume."
                }, JsonRequestBehavior.DenyGet);
            }
        }
    }
}

