ALTER TABLE DART."RomaneioNotaFiscal" ADD "SerieNotaFiscal" NVARCHAR2(3) NOT NULL;



CREATE INDEX "RomaneioNotaFiscal_INDEX4" ON DART."Pedido" ("NroNotaFiscal");
CREATE INDEX "RomaneioNotaFiscal_INDEX5" ON DART."Pedido" ("SerieNotaFiscal");