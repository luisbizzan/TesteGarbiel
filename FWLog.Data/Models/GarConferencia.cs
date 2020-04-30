using System;

namespace FWLog.Data.Models
{
    public class GarConferencia
    {
        public long Id { get; set; }
        public long Id_Tipo_Conf { get; set; }
        public String Tipo_Conf { get; set; }
        public DateTime Dt_Conf { get; set; }
        public long Id_Remessa { get; set; }
        public long Id_Solicitacao { get; set; }
        public int Ativo { get; set; }
        public String Id_Usr { get; set; }
        public String Usr { get; set; }
    }

    public class GarConferenciaItem : GarSolicitacaoItem
    {
        public long Id { get; set; }
        public long Id_Conf { get; set; }
        public long Id_Item { get; set; }
        public long Id_Item_Nf { get; set; }
        public DateTime Dt_Conf { get; set; }
        public long? Quant_Conferida { get; set; }
        public long Divergencia { get; set; }
        public String Id_Usr { get; set; }
        public String Usr { get; set; }

        public long Tem_Na_Nota { get; set; }
        public long Tem_No_Excesso { get; set; }
    }
}