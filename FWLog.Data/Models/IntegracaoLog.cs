using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FWLog.Data.Models
{
    public class IntegracaoLog
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public long IdIntegracaoLog                          { get; set; }
        [Required]                                           
        public long IdEmpresa                                { get; set; }
        [Required]                                           
        public IntegracaoTipoEnum     IdIntegracaoTipo       { get; set; }
        [Required]                                           
        public IntegracaoEntidadeEnum IdIntegracaoEntidade   { get; set; }
        [Required]                                           
        public DateTime DataRequisicao                       { get; set; }
        [Required]                                           
        public string   HttpVerbo                            { get; set; }
        [Required]                                           
        public string   Url                                  { get; set; }
        public string   CabecalhoRequisicao                  { get; set; }
        public string   CorpoRequisicao                      { get; set; }
        public int      Status                               { get; set; }
        public TimeSpan Duracao                              { get; set; }
        public string   CabecalhoResposta                    { get; set; }
        public string   CorpoResposta                        { get; set; }


        [ForeignKey(nameof(IdEmpresa))]
        public virtual Empresa            Empresa            { get; set; }
        [ForeignKey(nameof(IdIntegracaoTipo))]              
        public virtual IntegracaoTipo     IntegracaoTipo     { get; set; }
        [ForeignKey(nameof(IdIntegracaoEntidade))]
        public virtual IntegracaoEntidade IntegracaoEntidade { get; set; }
    }
}
