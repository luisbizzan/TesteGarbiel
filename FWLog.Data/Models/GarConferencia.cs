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
        public long Id_Tipo_Solicitacao { get; set; }

        public int Ativo { get; set; }
        public String Id_Usr { get; set; }
        public String Usr { get; set; }
    }

    public class GarConferenciaItem : GarSolicitacaoItem
    {
        public long Id { get; set; }
        public long Id_Conf { get; set; }
        public long Id_Item { get; set; }
        public DateTime Dt_Conf { get; set; }
        public long? Quant_Conferida { get; set; }
        public long? Quant_Max { get; set; }
        public long Divergencia { get; set; }
        public String Id_Usr { get; set; }
        public String Usr { get; set; }

        public long Tem_Na_Nota { get; set; }
        public long Tem_No_Excesso { get; set; }
    }

    public class GarConferenciaHist
    {
        public long Id { get; set; }
        public long Id_Conf { get; set; }
        public long Id_Solicitacao { get; set; }
        public DateTime Dt_Conf { get; set; }
        public String Refx { get; set; }
        public String Volume { get; set; }
        public String Id_Usr { get; set; }
        public long? Quant_Conferida { get; set; }
        public String Usr { get; set; }
    }

    public class GarConferenciaExcesso
    {
        public String Refx { get; set; }
        public long Id { get; set; }
        public long Id_Conf { get; set; }
        public long Id_Item { get; set; }
        public long Id_Tipo { get; set; }
        public DateTime Dt_Conf { get; set; }
        public long? Quant_Conferida { get; set; }
        public String Id_Usr { get; set; }
        public String Usr { get; set; }
        public String Id_Usr_Acao { get; set; }
        public String Usr_Acao { get; set; }
        public DateTime Dt_Usr_Acao { get; set; }
        public long Id_Doc { get; set; }
    }
}