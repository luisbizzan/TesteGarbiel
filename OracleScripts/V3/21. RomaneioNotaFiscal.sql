CREATE TABLE "RomaneioNotaFiscal" 
(
  "IdRomaneioNotaFiscal" NUMBER(19,0) NOT NULL
, "IdRomaneio" NUMBER(19,0) NOT NULL 
, "IdPedidoVenda" NUMBER(19,0) NOT NULL 
, "NroNotaFiscal" NUMBER(10,0) NOT NULL 
, "IdCliente" NUMBER(19,0) NOT NULL 
, "NroVolumes" NUMBER(10,0) NOT NULL 
, "TotalPesoLiquidoVolumes" FLOAT NULL
, "TotalPesoBrutoVolumes" FLOAT NULL
, CONSTRAINT "RomaneioNotaFiscal_PK" PRIMARY KEY 
  (
    "IdRomaneioNotaFiscal" 
  )
  USING INDEX 
  (
      CREATE UNIQUE INDEX "RomaneioNotaFiscal_PK" ON "RomaneioNotaFiscal" ("IdRomaneioNotaFiscal" ASC) 
  )
  ENABLE 
);

CREATE SEQUENCE "RomaneioNotaFiscal_SEQ";

CREATE TRIGGER "RomaneioNotaFiscal_SEQ_TRG" 
BEFORE INSERT ON "RomaneioNotaFiscal" 
FOR EACH ROW 
BEGIN
  <<COLUMN_SEQUENCES>>
  BEGIN
    IF INSERTING AND :NEW."IdRomaneioNotaFiscal" IS NULL THEN
      SELECT "RomaneioNotaFiscal_SEQ".NEXTVAL INTO :NEW."IdRomaneioNotaFiscal" FROM SYS.DUAL;
    END IF;
  END COLUMN_SEQUENCES;
END;
/

CREATE INDEX "RomaneioNotaFiscal_INDEX1" ON DART."RomaneioNotaFiscal" ("IdRomaneio");
CREATE INDEX "RomaneioNotaFiscal_INDEX2" ON DART."RomaneioNotaFiscal" ("IdPedidoVenda");
CREATE INDEX "RomaneioNotaFiscal_INDEX3" ON DART."RomaneioNotaFiscal" ("IdCliente");

ALTER TABLE DART."RomaneioNotaFiscal" ADD CONSTRAINT "RomaneioNotaFiscal_FK1" FOREIGN KEY ("IdRomaneio") REFERENCES DART."Romaneio"("IdRomaneio");
ALTER TABLE DART."RomaneioNotaFiscal" ADD CONSTRAINT "RomaneioNotaFiscal_FK2" FOREIGN KEY ("IdPedidoVenda") REFERENCES DART."PedidoVenda"("IdPedidoVenda");
ALTER TABLE DART."RomaneioNotaFiscal" ADD CONSTRAINT "RomaneioNotaFiscal_FK3" FOREIGN KEY ("IdCliente") REFERENCES DART."Cliente"("IdCliente");
