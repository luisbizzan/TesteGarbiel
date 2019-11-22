﻿using System.ComponentModel.DataAnnotations;

namespace FWLog.Data.Models
{
    public class EmpresaTipo
    {
        [Key]
        public int IdEmpresaTipo { get; set; }

        [StringLength(50)]
        public string Descricao { get; set; }
    }
}
