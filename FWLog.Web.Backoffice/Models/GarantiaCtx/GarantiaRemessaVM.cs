using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace FWLog.Web.Backoffice.Models.GarantiaCtx
{
    public class GarantiaRemessaVM
    {
        public GarantiaRemessaListVM EmptyItem { get; set; }

        public GarantiaRemessaFilterVM Filter { get; set; }

        public GarantiaRemessaVM()
        {
            EmptyItem = new GarantiaRemessaListVM();
            Filter = new GarantiaRemessaFilterVM();
        }
    }

    public class GarantiaRemessaListVM
    {
        [Display(Name = "Cod Fornecedor")]
        public string Cod_Fornecedor { get; set; }

        [Display(Name = "Nr. Remessa")]
        public long Id { get; set; }

        [Display(Name = "Data")]
        public DateTime? Dt_Criacao { get; set; }

        public String Tipo { get; set; }

        [Display(Name = "Status")]
        public long Id_Status { get; set; }

        public String Status { get; set; }

        [Display(Name = "Filial")]
        public string Filial { get; set; }
    }

    public class GarantiaRemessaFilterVM
    {
        [Display(Name = "Cnpj")]
        public string Cli_Cnpj { get; set; }

        [Display(Name = "Nr. Remessa")]
        public long? Id { get; set; }

        [Display(Name = "Nota Fiscal")]
        public string Nota_Fiscal { get; set; }

        [Display(Name = "Série")]
        public string Serie { get; set; }

        [Display(Name = "Status")]
        public long? Id_Status { get; set; }

        public String Status { get; set; }

        [Display(Name = "Data Inicial")]
        public DateTime? Data_Inicial { get; set; }

        [Display(Name = "Data Final")]
        public DateTime? Data_Final { get; set; }

        public SelectList Lista_Status { get; set; }
    }
}