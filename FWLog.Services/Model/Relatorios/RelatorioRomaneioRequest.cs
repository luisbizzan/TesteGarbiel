using FWLog.Data.Models;
using System;

namespace FWLog.Services.Model.Relatorios
{
    public class RelatorioRomaneioRequest
    {
        public Romaneio Romaneio { get; set; }

        public DateTime DataHoraEmissaoRomaneio { get; set; }

        public long IdEmpresa { get; set; }

        public string IdUsuarioExecucao { get; set; }
    }
}