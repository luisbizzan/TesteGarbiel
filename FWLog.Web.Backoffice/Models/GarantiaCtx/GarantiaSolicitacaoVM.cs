using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace FWLog.Web.Backoffice.Models.GarantiaCtx
{
    public class GarantiaSolicitacaoVM
    {
        public GarantiaSolicitacaoListVM EmptyItem { get; set; }

        public GarantiaSolicitacaoFilterVM Filter { get; set; }

        public GarantiaSolicitacaoVM()
        {
            EmptyItem = new GarantiaSolicitacaoListVM();
            Filter = new GarantiaSolicitacaoFilterVM();
        }
    }

    public class GarantiaSolicitacaoListVM
    {
        [Display(Name = "Cnpj")]
        public string Cli_Cnpj { get; set; }

        [Display(Name = "Razão Social")]
        public string Razao_Social { get; set; }

        [Display(Name = "Repr.")]
        public string Rep { get; set; }

        [Display(Name = "Nr. Solicitação")]
        public long Id { get; set; }

        [Display(Name = "Data")]
        public DateTime? Dt_Criacao { get; set; }

        [Display(Name = "Tipo")]
        public long Id_Tipo { get; set; }

        public String Tipo { get; set; }

        [Display(Name = "Valor")]
        public long Valor { get; set; }

        [Display(Name = "Status")]
        public long Id_Status { get; set; }

        public String Status { get; set; }

        [Display(Name = "Nota Fiscal")]
        public string Nota_Fiscal { get; set; }

        [Display(Name = "Filial")]
        public string Empresa { get; set; }

        [Display(Name = "Filial")]
        public string Filial { get; set; }
    }

    public class GarantiaSolicitacaoFilterVM
    {
        [Display(Name = "Cnpj")]
        public string Cli_Cnpj { get; set; }

        [Display(Name = "Nr. Solicitação")]
        public long? Id { get; set; }

        [Display(Name = "Nota Fiscal")]
        public string Nota_Fiscal { get; set; }

        [Display(Name = "Série")]
        public string Serie { get; set; }

        [Display(Name = "Status")]
        public long? Id_Status { get; set; }

        public long? Id_Empresa { get; set; }

        public String Status { get; set; }

        [Display(Name = "Data Inicial")]
        public DateTime? Data_Inicial { get; set; }

        [Display(Name = "Data Final")]
        public DateTime? Data_Final { get; set; }

        public SelectList Lista_Status { get; set; }

        [Display(Name = "Tipo")]
        public long? Id_Tipo { get; set; }

        public String Tipo { get; set; }

        public SelectList Lista_Tipos { get; set; }
    }

    public class GarantiaSolicitacao
    {
        [Display(Name = "Chave de Acesso")]
        public string Chave_Acesso { get; set; }

        [Display(Name = "Código de Postagem")]
        public string Codigo_Postagem { get; set; }

        [Display(Name = "CNPJ")]
        public string Cnpj { get; set; }

        [Display(Name = "Número")]
        public long? Numero { get; set; }

        [Display(Name = "Série")]
        public string Serie { get; set; }

        [Display(Name = "Número Interno")]
        public long? Numero_Interno { get; set; }

        [Required]
        [Display(Name = "Tipo")]
        public long? Id_Tipo { get; set; }

        public String Tipo { get; set; }

        public SelectList Lista_Tipos { get; set; }
    }
}