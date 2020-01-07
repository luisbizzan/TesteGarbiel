namespace FWLog.Services.Model.Relatorios
{
    public class UserLog
    {
        public string IP { get; set; }
        public string UserId { get; set; }
    }

    public class TermoResponsabilidadeRequest
    {
        public string NomeUsuario { get; set; }
        public int IdImpressora { get; set; }
        public long IdQuarentena { get; set; }
        public UserLog UserLog { get; set; }
    }
}
