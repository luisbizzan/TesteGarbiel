CREATE TABLE "Lote"
(
    "IdLote" NUMBER PRIMARY KEY not null ,
    "IdLoteStatus" SMALLINT REFERENCES "LoteStatus"("IdLoteStatus") NOT NULL,
	"IdNotaFiscal" NUMBER REFERENCES "NotaFiscal" ("IdNotaFiscal") NOT NULL,
    "DataCompra" DATE,
    "DataRecebimento" DATE,
    "QuantidadePeca" NUMBER,
    "QuantidadeVolume" NUMBER,
    "IdUsuarioRecebimento" VARCHAR2(128 BYTE) REFERENCES "AspNetUsers"("Id")
);
    
CREATE SEQUENCE Lote_Sequence;

CREATE OR REPLACE TRIGGER Lote_On_Insert
    BEFORE INSERT ON "Lote"
    FOR EACH ROW
BEGIN
    SELECT Lote_Sequence.nextval
    INTO :new."IdLote"
    FROM dual;
END;