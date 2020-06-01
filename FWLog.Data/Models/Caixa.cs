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

        [Display(Name = "Caixa para")]
        [Required]
        public CaixaTipoEnum IdCaixaTipo { get; set; }

        [Display(Name = "Nome")]
        [Required]
        [StringLength(50)]
        public string Nome { get; set; }

        [Display(Name = "Texto Etiqueta")]
        [Required]
        [StringLength(50)]
        public string TextoEtiqueta { get; set; }

        [Display(Name = "Largura (CM)")]
        [Required]
        public decimal Largura { get; set; }

        [Display(Name = "Altura (CM)")]
        [Required]
        public decimal Altura { get; set; }

        [Display(Name = "Comprimento (CM)")]
        [Required]
        public decimal Comprimento { get; set; }

        [Display(Name = "Cubagem (CM³)")]
        [Required]
        public decimal Cubagem { get; set; }

        [Display(Name = "Peso Caixa (Kg)")]
        [Required]
        public decimal PesoCaixa { get; set; }

        [Display(Name = "Peso Máximo (Kg)")]
        [Required]
        public decimal PesoMaximo { get; set; }

        [Display(Name = "Sobra (%)")]
        [Required]
        [Range(0, 100)]
        public decimal Sobra { get; set; }

        [Display(Name = "Ativo")]
        [Required]
        public bool Ativo { get; set; }

        [ForeignKey(nameof(IdEmpresa))]
        public virtual Empresa Empresa { get; set; }

        [ForeignKey(nameof(IdCaixaTipo))]
        public virtual CaixaTipo CaixaTipo { get; set; }
    }
}