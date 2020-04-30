CREATE TABLE "TransportadoraEndereco" 
(
  "IdTransportadoraEndereco" NUMBER(19,0) NOT NULL 
, "IdTransportadora" NUMBER(19,0) NOT NULL
, "IdEnderecoArmazenagem" NUMBER(19,0) NOT NULL 
, CONSTRAINT "TransportadoraEndereco_PK" PRIMARY KEY 
  (
    "IdTransportadoraEndereco" 
  )
  USING INDEX 
  (
      CREATE UNIQUE INDEX "TransportadoraEndereco_PK" ON "TransportadoraEndereco" ("IdTransportadoraEndereco" ASC) 
  )
  ENABLE 
);

CREATE SEQUENCE "TransportadoraEndereco_SEQ";

CREATE TRIGGER "TransportadoraEndereco_SEQ_TRG" 
BEFORE INSERT ON "TransportadoraEndereco" 
FOR EACH ROW 
BEGIN
  <<COLUMN_SEQUENCES>>
  BEGIN
    IF INSERTING AND :NEW."IdTransportadoraEndereco" IS NULL THEN
      SELECT "TransportadoraEndereco_SEQ".NEXTVAL INTO :NEW."IdTransportadoraEndereco" FROM SYS.DUAL;
    END IF;
  END COLUMN_SEQUENCES;
END;
/

CREATE INDEX "TransportadoraEndereco_INDEX1" ON DART."TransportadoraEndereco" ("IdTransportadora");
CREATE INDEX "TransportadoraEndereco_INDEX2" ON DART."TransportadoraEndereco" ("IdEnderecoArmazenagem");

ALTER TABLE DART."TransportadoraEndereco" ADD CONSTRAINT "TransportadoraEndereco_FK1" FOREIGN KEY ("IdTransportadora") REFERENCES DART."Transportadora"("IdTransportadora");
ALTER TABLE DART."TransportadoraEndereco" ADD CONSTRAINT "TransportadoraEndereco_FK2" FOREIGN KEY ("IdEnderecoArmazenagem") REFERENCES DART."EnderecoArmazenagem"("IdEnderecoArmazenagem");