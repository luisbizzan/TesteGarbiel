using System;
using System.ComponentModel.DataAnnotations;

namespace FWLog.Web.Backoffice.Models.BORecebimentoNotaCtx
{
    public class RelatorioResumoProducaoViewModel
    {
        public RelatorioResumoProducaoRecebimentoListItemViewModel ListaRecebimento { get; set; }

        public RelatorioResumoProducaoConferenciaListItemViewModel ListaConferencia { get; set; }

        public RelatorioResumoProducaoFilterViewModel Filter { get; set; }

        public RelatorioResumoProducaoViewModel()
        {
            ListaRecebimento = new RelatorioResumoProducaoRecebimentoListItemViewModel();
            ListaConferencia = new RelatorioResumoProducaoConferenciaListItemViewModel();
            Filter = new RelatorioResumoProducaoFilterViewModel();
        }
    }

    public class RelatorioResumoProducaoRecebimentoListItemViewModel
    {
        [Display(Name = "Notas Recebidas")]
        public long NotasRecebidas { get; set; }

        [Display(Name = "Peças Recebidas")]
        public long PecasRecebidas { get; set; }

        [Display(Name = "Ranking ")]
        public long Ranking { get; set; }

        [Display(Name = "Usuário")]
        public string NomeUsuario { get; set; }

        [Display(Name = "Notas Recebidas (Usuario)")]
        public long NotasRecebidasUsuario { get; set; }

        [Display(Name = "Peças Recebidas (Usuario)")]
        public long PecasRecebidasUsuario { get; set; }

        [Display(Name = "Percentual")]
        public decimal Percentual { get; set; }
    }

    public class RelatorioResumoProducaoConferenciaListItemViewModel
    {
        [Display(Name = "Lotes Recebidas")]
        public long LotesRecebidas { get; set; }

        [Display(Name = "Peças Recebidas")]
        public long PecasRecebidas { get; set; }

        [Display(Name = "Ranking ")]
        public long Ranking { get; set; }

        [Display(Name = "Usuário")]
        public string NomeUsuario { get; set; }

        [Display(Name = "Lotes Recebidas (Usuario)")]
        public long LotesRecebidasUsuario { get; set; }

        [Display(Name = "Peças Recebidas (Usuario)")]
        public long PecasRecebidasUsuario { get; set; }

        [Display(Name = "Percentual")]
        public decimal Percentual { get; set; }
    }

    public class RelatorioResumoProducaoFilterViewModel
    {
        public string IdUsuario { get; set; }

        [Display(Name = "Data")]
        public DateTime DataRecebimentoMinima { get; set; } = DateTime.Today;

        [Display(Name = "Data")]
        public DateTime? DataRecebimentoMaxima { get; set; }


    }
}