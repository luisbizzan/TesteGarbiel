using FWLog.Data.Models;

namespace FWLog.Services.Model.Armazenagem
{
    public class AbastecerPickingResponse
    {
        public LoteProduto LoteProduto { get; set; }
        public EnderecoArmazenagem EnderecoArmazenagem { get; set; }
    }
}
