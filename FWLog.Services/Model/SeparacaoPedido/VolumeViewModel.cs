﻿using FWLog.Services.Model.Caixa;
using System.Collections.Generic;

namespace FWLog.Services.Model.SeparacaoPedido
{
    public class VolumeViewModel
    {
        public List<PedidoItemViewModel> ListaItensDoPedido { get; set; }
        public CaixaViewModel Caixa { get; set; }
        public bool CaixaFornecedor { get; set; }
    }
}