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
        public long Do_Usr_Logado { get; set; }

        public DateTime? Dt_Criacao { get; set; }

        public String Tipo { get; set; }

        [Display(Name = "Status")]
        public long Id_Status { get; set; }

        public String Status { get; set; }

        [Display(Name = "Filial")]
        public string Empresa { get; set; }
    }

    public class GarantiaRemessaFilterVM
    {
        [Display(Name = "Cnpj")]
        public string Cod_Fornecedor { get; set; }

        [Display(Name = "Nr. Remessa")]
        public long? Id { get; set; }

        [Display(Name = "Status")]
        public long? Id_Status { get; set; }

        public string Id_User { get; set; }

        public long? Id_Empresa { get; set; }

        public String Status { get; set; }

        [Display(Name = "Data Inicial")]
        public DateTime? Data_Inicial { get; set; }

        [Display(Name = "Data Final")]
        public DateTime? Data_Final { get; set; }

        public SelectList Lista_Status { get; set; }
    }

    public class GarantiaRemessa
    {
        [Required]
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
        public string Empresa { get; set; }
    }
}