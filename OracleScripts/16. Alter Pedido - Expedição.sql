ALTER TABLE "Pedido" ADD "CodigoIntegracaoNotaFiscal" NUMBER (10,0) NULL;

CREATE INDEX "Pedido_INDEX6" ON DART."Pedido" ("CodigoIntegracaoNotaFiscal");