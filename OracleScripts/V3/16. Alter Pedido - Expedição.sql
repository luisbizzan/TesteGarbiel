ALTER TABLE "Pedido" ADD "CodigoIntegracaoNotaFiscal" NUMBER (10,0) NULL;
ALTER TABLE "Pedido" ADD "CodigoIntegracaoTipoFrete" VARCHAR2 (3) NULL;
ALTER TABLE "Pedido" ADD "ChaveAcessoNotaFiscal" VARCHAR2 (44) NULL;

CREATE INDEX "Pedido_INDEX6" ON DART."Pedido" ("CodigoIntegracaoNotaFiscal");

CREATE INDEX "DART"."Pedido_INDEX4" ON "DART"."Pedido" ("IdPedidoVendaStatus") ;

CREATE INDEX "DART"."Pedido_INDEX5" ON "DART"."Pedido" ("IdRepresentante") 