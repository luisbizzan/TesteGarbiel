using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FWLog.Data.Models
{
    public class NotaFiscalRecebimento
    {
        [Key]
        [Required]
        public long     IdNotaFiscalRecebimento  { get; set; }
                                                 
        [Index]                                  
        [Required]                               
        public long     IdFornecedor             { get; set; }
                                                 
        [Required]                               
        public int      NumeroNF                 { get; set; }
                                                 
        [Required]                               
        public string   Serie                    { get; set; }
                                                
        [Required]                              
        public decimal  Valor                    { get; set; }
                                                
        [Required]                              
        public int      QuantidadeVolumes        { get; set; }
                                                
        [Required]                              
        public DateTime DataHora                 { get; set; }

        [Required]
        public StatusNotaRecebimentoEnum Status  { get; set; }


        [ForeignKey(nameof(IdFornecedor))]
        public virtual Fornecedor Fornecedor     { get; set; }
    }
}
