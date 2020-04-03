using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FWLog.Data.Models
{
    public class AtividadeEstoque
    {
        [Key]
        [Required]
        public long IdAtividadeEstoque { get; set; }
        [Index]
        [Required]
        public long IdEmpresa { get; set; }
        [Index]
        [Required]
        public long IdProduto { get; set; }
        [Index]
        [Required]
        public long IdEnderecoArmazenagem { get; set; }
        [Index]
        [Required]
        public AtividadeEstoqueTipoEnum IdAtividadeEstoqueTipo { get; set; }
        public int QuantidadeInicial { get; set; }
        public int QuantidadeFinal { get; set; }
        [Required]
        public DateTime DataSolicitacao { get; set; }
        public DateTime DataExecucao { get; set; }
        [Index]
        public string IdUsuarioSolicitacao { get; set; }
        [Index]
        public string IdUsuarioExecucao { get; set; }
        [Index]
        [Required]
        public bool Finalizado { get; set; }

        [ForeignKey(nameof(IdProduto))]
        public virtual Produto Produto { get; set; }

        [ForeignKey(nameof(IdEnderecoArmazenagem))]
        public virtual EnderecoArmazenagem EnderecoArmazenagem { get; set; }

        [ForeignKey(nameof(IdAtividadeEstoqueTipo))]
        public virtual AtividadeEstoqueTipo AtividadeEstoqueTipo { get; set; }

        [ForeignKey(nameof(IdEmpresa))]
        public virtual Empresa Empresa { get; set; }

        [ForeignKey(nameof(IdUsuarioSolicitacao))]
        public virtual AspNetUsers UsuarioSolicitacao { get; set; }

        [ForeignKey(nameof(IdUsuarioExecucao))]
        public virtual AspNetUsers UsuarioExecucao { get; set; }
    }
}
