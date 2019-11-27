using System.ComponentModel.DataAnnotations;

namespace FWLog.Data.EnumsAndConsts
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
}
