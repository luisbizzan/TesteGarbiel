using System;
using System.Collections.Generic;
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
        public long? IdLote { get; set; }

        [Index]
        [Required]
        public long IdProduto { get; set; }

        [Index]
        [Required]
        public long IdEnderecoArmazenagem { get; set; }

        [Required]
        public int Quantidade { get; set; }

        [Required]
        public string IdUsuarioInstalacao { get; set; }

        [Required]
        public DateTime DataHoraInstalacao { get; set; }

        [Required]
        public decimal PesoTotal { get; set; }

        [ForeignKey(nameof(IdEmpresa))]
        public virtual Empresa Empresa { get; set; }

        [ForeignKey(nameof(IdLote))]
        public virtual Lote Lote { get; set; }

        [ForeignKey(nameof(IdProduto))]
        public virtual Produto Produto { get; set; }

        [ForeignKey(nameof(IdEnderecoArmazenagem))]
        public virtual EnderecoArmazenagem EnderecoArmazenagem { get; set; }

        public ProdutoEstoque ProdutoEstoque { get; set; }

        [ForeignKey(nameof(IdUsuarioInstalacao))]
        public virtual AspNetUsers AspNetUsers { get; set; }
    }
}