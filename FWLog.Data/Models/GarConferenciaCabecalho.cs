using System;

namespace FWLog.Data.Models
{
    public class GarConferenciaCabecalho
    {
        public long Id { get; set; }
        public long Id_Tipo_Conf { get; set; }
        public String Tipo_Conf { get; set; }
        public DateTime Dt_Conf { get; set; }
        public String Id_Usr { get; set; }
        public String Usr { get; set; }
        public long Id_Remessa { get; set; }
        public int Qtde { get; set; }
    }

    public class GarConferenciaHist
    {
        public long Id { get; set; }
        public long Id_Conf { get; set; }
        public DateTime Dt_Conf { get; set; }
        public String Refx { get; set; }
        public String Volume { get; set; }
        public String Id_Usr { get; set; }
        public String Usr { get; set; }
    }

    public class GarConferenciaExcesso
    {
        public long Id { get; set; }
        public long Id_Conf { get; set; }
        public DateTime Dt_Conf { get; set; }
        public String Refx { get; set; }
        public String Id_Usr { get; set; }
        public String Usr { get; set; }
    }

    public class GarConferenciaItem
    {
        public long Id { get; set; }
        public long Id_Conf { get; set; }
        public long Id_Item { get; set; }
        public long Id_Item_Nf { get; set; }
        public DateTime Dt_Conf { get; set; }
        public long Quant { get; set; }
        public long Quant_Conferida { get; set; }
        public String Id_Usr { get; set; }
        public String Usr { get; set; }
    }
}