using FWLog.Data.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FWLog.Web.Backoffice.Models.BOQuarentenaCtx
{
    public class HistoricoQuarentenaViewModel
    {
        public long IdQuarentena { get; set; }

        [Display(Name = "Nota/Série")]
        public string NotaSerie { get; set; }

        [Display(Name = "Lote")]
        public string Lote { get; set; }

        [Display(Name = "Status Lote")]
        public string LoteStatus { get; set; }

        [Display(Name = "Data de Abertura")]
        public string DataAbertura { get; set; }

        [Display(Name = "Data de Encerramento")]
        public string DataEncerramento { get; set; }
        [Display(Name = "Status Quarentena")]
        public QuarentenaStatusEnum IdStatus { get; set; }

        public List<HistoricoQuarentenaItemViewModel> Itens { get; set; } = new List<HistoricoQuarentenaItemViewModel>();
    }

    public class HistoricoQuarentenaItemViewModel
    {
        public string Data { get; set; }

        [Display(Name = "Usuário")]
        public string Usuario { get; set; }

        [Display(Name = "Descrição")]
        public string Descricao { get; set; }
    }

}