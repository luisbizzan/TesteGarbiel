using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FWLog.Data.Models
{
    public class TransportadoraEndereco
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public long IdTransportadoraEndereco { get; set; }

        [Index]
        [Required]
        public long IdTransportadora { get; set; }

        [Index(IsUnique = true)]
        [Required]
        public long IdEnderecoArmazenagem { get; set; }

        [ForeignKey(nameof(IdTransportadora))]
        public virtual Transportadora Transportadora { get; set; }

        [ForeignKey(nameof(IdEnderecoArmazenagem))]
        public virtual EnderecoArmazenagem EnderecoArmazenagem { get; set; }
    }
}
