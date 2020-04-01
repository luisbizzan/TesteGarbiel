using System;

namespace FWLog.Data.Models.DataTablesCtx
{
    public class RastreabilidadeLoteListaLinhaTabela
    {
        public long IdLote { get; set; }

        public string Status { get; set; }

        public DateTime DataRecebimento { get; set; }

        public DateTime? DataConferencia { get; set; }

        public int QuantidadeVolume { get; set; }

        public int QuantidadePeca { get; set; }
    }
}
