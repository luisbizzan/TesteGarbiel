using FWLog.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FWLog.Web.Backoffice.Models.BOQuarentenaCtx
{
    public class DetalhesQuarentenaViewModel
    {
        public long IdQuarentena { get; set; }

        [Display(Name = "Data de Abertura")]
        public string DataAbertura { get; set; }

        [Display(Name = "Data de Encerramento")]
        public string DataEncerramento { get; set; }

        [Display(Name = "Status")]
        public QuarentenaStatusEnum IdStatus { get; set; }

        [Display(Name = "Observação")]
        public string Observacao { get; set; }

        [Display(Name = "Código Termo Responsabilidade")]
        public string CodigoConfirmacao { get; set; }

        public bool PermiteEdicao
        {
            get
            {
                QuarentenaStatusEnum[] statusNaoPermite = new QuarentenaStatusEnum[] { QuarentenaStatusEnum.EncaminhadoAuditoria, QuarentenaStatusEnum.Finalizado };

                return !Array.Exists(statusNaoPermite, x => x == IdStatus);
            }
        }

        public List<DivergenciaItemViewModel> Divergencias { get; set; } = new List<DivergenciaItemViewModel>();
    }

    public class DivergenciaItemViewModel
    {
        [Display(Name = "Referência")]
        public string Referencia { get; set; }

        [Display(Name = "Qtd. Nota Fiscal")]
        public int QuantidadeNotaFiscal { get; set; }

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