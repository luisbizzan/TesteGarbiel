CREATE UNIQUE INDEX "Pedido_INDEX7" ON DART."Pedido" ("IdEmpresa", "NroPedido");

CREATE UNIQUE INDEX "PedidoVenda_INDEX7" ON DART."PedidoVenda" ("IdEmpresa", "NroPedidoVenda");

CREATE UNIQUE INDEX "PedidoVendaVolume_INDEX10" ON DART."PedidoVendaVolume" ("EtiquetaVolume");