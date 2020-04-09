using System;

namespace FWLog.Data.Models
{
    public class GeralUpload
    {
        public long Id { get; set; }
        public long Id_Categoria { get; set; }
        public long Id_Ref { get; set; }
        public string Id_Usr { get; set; }
        public string Arquivo { get; set; }
        public string Arquivo_Tipo { get; set; }
        public string Usuario { get; set; }
        public DateTime Dt_Cad { get; set; }
    }

    public class GeralUploadCategoria
    {
        public long Id { get; set; }
        public string Tabela { get; set; }
        public string Categoria { get; set; }
        public string Formatos { get; set; }
    }
}