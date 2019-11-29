using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FWLog.Data.Models
{
    public enum EmpresaTipoEnum
    {
        [Display(Name = "Matriz")]
        Matriz = 1,
        [Display(Name = "Filial")]
        Grupo = 2,
        [Display(Name = "Grupo")]
        Filial = 3,
    }

    public class EmpresaTipo
    {
        [Key]
        [Index(IsUnique = true)]
        [Required]
        public EmpresaTipoEnum IdEmpresaTipo { get; set; }

        [StringLength(50)]
        [Index(IsUnique = true)]
        [Required]
        public string Descricao { get; set; }
    }
}
