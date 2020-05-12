CREATE TABLE "Romaneio" 
(
  "IdRomaneio" NUMBER(19,0) NOT NULL 
, "IdTransportadora" NUMBER(19,0) NOT NULL
, "IdEmpresa" NUMBER(19,0) NOT NULL 
, "NroRomaneio" NUMBER(10,0) NOT NULL 
, CONSTRAINT "Romaneio_PK" PRIMARY KEY 
  (
    "IdRomaneio" 
  )
  USING INDEX 
  (
      CREATE UNIQUE INDEX "Romaneio_PK" ON "Romaneio" ("IdRomaneio" ASC) 
  )
  ENABLE 
);

CREATE SEQUENCE "Romaneio_SEQ";

CREATE TRIGGER "Romaneio_SEQ_TRG" 
BEFORE INSERT ON "Romaneio" 
FOR EACH ROW 
BEGIN
  <<COLUMN_SEQUENCES>>
  BEGIN
    IF INSERTING AND :NEW."IdRomaneio" IS NULL THEN
      SELECT "Romaneio_SEQ".NEXTVAL INTO :NEW."IdRomaneio" FROM SYS.DUAL;
    END IF;
  END COLUMN_SEQUENCES;
END;
/

CREATE INDEX "Romaneio_INDEX1" ON DART."Romaneio" ("IdTransportadora");
CREATE INDEX "Romaneio_INDEX2" ON DART."Romaneio" ("IdEmpresa");

ALTER TABLE DART."Romaneio" ADD CONSTRAINT "Romaneio_FK1" FOREIGN KEY ("IdTransportadora") REFERENCES DART."Transportadora"("IdTransportadora");
ALTER TABLE DART."Romaneio" ADD CONSTRAINT "Romaneio_FK2" FOREIGN KEY ("IdEmpresa") REFERENCES DART."Empresa"("IdEmpresa");