﻿using System;

namespace FWLog.Data.Models.DataTablesCtx
{
    public class EntradaProduto
    {
        public long IdProduto { get; set; }

        public string ReferenciaProduto { get; set; }

        public DateTime DataFinalConferenciaLote { get; set; }

        public int QuantidadeRecebidaLoteProduto { get; set; }
    }
}