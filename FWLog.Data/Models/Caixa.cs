using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FWLog.Data.Models
{
    public class Caixa
    {
        [Key]
        [Index]
        [Required]
        public long IdCaixa { get; set; }

        [Required]
        public long IdEmpresa { get; set; }

        [Required]
        public CaixaTipoEnum IdCaixaTipo { get; set; }

        [Required]
        [StringLength(50)]
        public string Nome { get; set; }

        [Required]
        [StringLength(50)]
        public string TextoEtiqueta { get; set; }

        [Required]
        public decimal Largura { get; set; }

        [Required]
        public decimal Altura { get; set; }

        [Required]
        public decimal Comprimento { get; set; }

        [Required]
        public decimal Cubagem { get; set; }

        [Required]
        public decimal PesoCaixa { get; set; }

        [Required]
        public decimal PesoMaximo { get; set; }

        [Required]
        public decimal Sobra { get; set; }

        [Required]
        public int Prioridade { get; set; }

        [Required]
        public bool Ativo { get; set; }

        [ForeignKey(nameof(IdEmpresa))]
        public virtual Empresa Empresa { get; set; }

        [ForeignKey(nameof(IdCaixaTipo))]
        public virtual CaixaTipo CaixaTipo { get; set; }
    }
}