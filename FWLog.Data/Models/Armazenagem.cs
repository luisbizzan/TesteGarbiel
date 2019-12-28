using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FWLog.Data.Models
{
    public class Armazenagem
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public long IdArmazenagem { get; set; }
        [Required]
        [Index]
        public long IdEmpresa { get; set; }
        [Required]
        [Index]
        public long IdEnderecoArmazenagem { get; set; }
        [Required]
        [Index]
        public long IdLote { get; set; }
        [Index]
        public long? IdProduto { get; set; }
        public int Quantidade { get; set; }
        [Required]
        [Index]
        public TipoArmazenagemEnum IdTipoArmazenagem { get; set; }
        [Required]
        [Index]
        [StringLength(128)]
        public string IdUsuario { get; set; }
        public DateTime DataArmazenagem { get; set; }
        public decimal? PesoUnitario { get; set; }
        public decimal? PesoTotal { get; set; }

        [ForeignKey(nameof(IdEmpresa))]
        public virtual Empresa Empresa { get; set; }
        [ForeignKey(nameof(IdEnderecoArmazenagem))]
        public virtual EnderecoArmazenagem EnderecoArmazenagem { get; set; }
        [ForeignKey(nameof(IdLote))]
        public virtual Lote Lote { get; set; }
        [ForeignKey(nameof(IdProduto))]
        public virtual Lote Produto { get; set; }
        [ForeignKey(nameof(IdTipoArmazenagem))]
        public virtual TipoArmazenagem TipoArmazenagem { get; set; }
        [ForeignKey(nameof(IdUsuario))]
        public virtual AspNetUsers Usuario { get; set; }
    }
}