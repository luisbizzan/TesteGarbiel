using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FWLog.Data.Models
{
    public class LoteProdutoEndereco
    {
        [Key]
        [Required]
        public long IdLoteProdutoEndereco { get; set; }

        [Index]
        [Required]
        public long IdEmpresa { get; set; }

        [Index]
        [Required]
        public long IdLote { get; set; }

        [Index]
        [Required]
        public long IdProduto { get; set; }

        [Index]
        [Required]
        public long IdEnderecoArmazenagem { get; set; }

        [Required]
        public long Quantidade { get; set; }

        [Required]
        public string IdUsuarioInstalacao { get; set; }

        [Required]
        public DateTime DataHoraInstalacao { get; set; }

        [Required]
        public decimal Peso { get; set; }

        [ForeignKey(nameof(IdEmpresa))]
        public virtual Empresa Empresa { get; set; }

        [ForeignKey(nameof(IdLote))]
        public virtual Lote Lote { get; set; }

        [ForeignKey(nameof(IdProduto))]
        public virtual Produto Produto { get; set; }

        [ForeignKey(nameof(IdProduto))]
        public virtual EnderecoArmazenagem EnderecoArmazenagem { get; set; }
    }
}
