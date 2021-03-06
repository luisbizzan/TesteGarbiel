﻿using FWLog.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FWLog.Web.Backoffice.Models.BOQuarentenaCtx
{
    public class DetalhesQuarentenaViewModel
    {
        public long IdQuarentena { get; set; }

        [Display(Name = "Nota/Série")]
        public string NotaSerie { get; set; }

        [Display(Name = "Lote")]
        public string Lote { get; set; }

        [Display(Name = "Status Lote")]
        public string LoteStatus { get; set; }

        [Display(Name = "Data de Abertura")]
        public string DataAbertura { get; set; }

        [Display(Name = "Data de Encerramento")]
        public string DataEncerramento { get; set; }

        [Display(Name = "Status Quarentena")]
        public QuarentenaStatusEnum IdStatus { get; set; }

        [Display(Name = "Observação")]
        public string Observacao { get; set; }

        [Display(Name = "Código Termo Responsabilidade")]
        public string CodigoConfirmacao { get; set; }

        [Display(Name = "Chave de Acesso")]
        public string ChaveAcesso { get; set; }

        [Display(Name = "Observação")]
        public string ObservacaoDivergencia { get; set; }

        public bool PermiteEdicao
        {
            get
            {
                QuarentenaStatusEnum[] statusNaoPermite = new QuarentenaStatusEnum[] { QuarentenaStatusEnum.Retirado, QuarentenaStatusEnum.Finalizado };

                return !Array.Exists(statusNaoPermite, x => x == IdStatus);
            }
        }

        public List<DivergenciaItemViewModel> Divergencias { get; set; } = new List<DivergenciaItemViewModel>();
    }

    public class DivergenciaItemViewModel
    {
        [Display(Name = "Descrição")]
        public string Descricao { get; set; }
        [Display(Name = "Referência")]
        public string Referencia { get; set; }

        [Display(Name = "Qtd. Nota Fiscal")]
        public int QuantidadeNotaFiscal { get; set; }

        [Display(Name = "Qtde. Devolução")]
        public int? QuantidadeDevolucao { get; set; }

        [Display(Name = "Qtd. Conferência")]
        public int QuantidadeConferencia { get; set; }

        [Display(Name = "A+")]
        public int QuantidadeMais { get; set; }

        [Display(Name = "A-")]
        public int QuantidadeMenos { get; set; }

        [Display(Name = "Qtd. Tratado A+")]
        public int? QuantidadeMaisTratado { get; set; }

        [Display(Name = "Qtd. Tratado A-")]
        public int? QuantidadeMenosTratado { get; set; }
    }
}