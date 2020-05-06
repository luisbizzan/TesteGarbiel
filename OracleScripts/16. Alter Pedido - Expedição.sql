ALTER TABLE "Pedido" ADD "CodigoIntegracaoNotaFiscal" NUMBER (10,0) NULL;
ALTER TABLE "Pedido" ADD "CodigoIntegracaoTipoFrete" VARCHAR2 (3) NULL;

CREATE INDEX "Pedido_INDEX6" ON DART."Pedido" ("CodigoIntegracaoNotaFiscal");