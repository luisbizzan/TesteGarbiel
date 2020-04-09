using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FWLog.Web.Backoffice.Models.GeralCtx
{
    public class GeralUploadVM
    {
        public long Id { get; set; }

        [Required]
        public long Id_Categoria { get; set; }

        public string Tabela { get; set; }

        public string Categoria { get; set; }

        public List<string> Formatos { get; set; }

        [Required]
        public long Id_Ref { get; set; }

        public string Id_Usr { get; set; }

        [Required]
        public string Arquivo { get; set; }

        public string Arquivo_Tipo { get; set; }

        public string Usuario { get; set; }

        public DateTime Dt_Cad { get; set; }

        public List<GeralUploadVM> Lista_Uploads { get; set; }
    }
}