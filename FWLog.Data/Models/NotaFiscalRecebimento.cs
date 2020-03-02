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

        public string   IdUsuarioRecebimento { get; set; }

        [Index]                                  
        [Required]                               
        public long     IdFornecedor             { get; set; }
                                                 
        [Required]                               
        public int?     NumeroNF                 { get; set; }
                                                 
        [Required]                               
        public string   Serie                    { get; set; }
        public string   ChaveAcesso              { get; set; }

        [Required]                              
        public decimal  Valor                    { get; set; }
                                                
        [Required]                              
        public int?     QuantidadeVolumes        { get; set; }
                                                
        [Required]                              
        public DateTime DataHora                 { get; set; }

        [Required]
        public NotaRecebimentoStatusEnum IdNotaRecebimentoStatus { get; set; }

        [ForeignKey(nameof(IdNotaRecebimentoStatus))]
        public virtual NotaRecebimentoStatus NotaRecebimentoStatus { get; set; }


        [ForeignKey(nameof(IdUsuarioRecebimento))]
        public virtual AspNetUsers UsuarioRecebimento { get; set; }


        [ForeignKey(nameof(IdFornecedor))]
        public virtual Fornecedor Fornecedor     { get; set; }
    }
}
