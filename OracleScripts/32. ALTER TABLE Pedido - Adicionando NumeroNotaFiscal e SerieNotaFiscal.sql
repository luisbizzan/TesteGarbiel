ALTER TABLE DART."Pedido" ADD "NumeroNotaFiscal" NUMBER (10,0) NULL;
ALTER TABLE DART."Pedido" ADD "SerieNotaFiscal" NVARCHAR2(3) NULL;



CREATE INDEX "Pedido_INDEX8" ON DART."Pedido" ("NumeroNotaFiscal");
CREATE INDEX "Pedido_INDEX9" ON DART."Pedido" ("SerieNotaFiscal");