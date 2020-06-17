using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FWLog.Data.Models
{
    public enum PedidoVendaStatusEnum
    {
        ProcessandoIntegracao = 0,
        PendenteSeparacao = 1,
        EnviadoSeparacao = 2,
        ProcessandoSeparacao = 3,
        SeparacaoConcluidaComSucesso = 4,
        PendenteCancelamento = 5,
        Cancelado = 6,
        InstalandoVolumeTransportadora = 7,
        VolumeInstaladoTransportadora = 8,
        MovendoDOCA = 9,
        MovidoDOCA = 10,
        DespachandoNF = 11,
        NFDespachada = 12,
        RomaneioImpresso = 13,
        VolumeExcluido = 14,
        ProdutoZerado = 15,
        AguardandoRetirada = 16
    }

    public class PedidoVendaStatus
    {
        [Key]
        [Index(IsUnique = true)]
        [Required]
        public PedidoVendaStatusEnum IdPedidoVendaStatus { get; set; }

        [StringLength(50)]
        [Index(IsUnique = true)]
        [Required]
        public string Descricao { get; set; }
    }
}