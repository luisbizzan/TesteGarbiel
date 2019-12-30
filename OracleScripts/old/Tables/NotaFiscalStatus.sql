CREATE TABLE "NotaFiscalStatus"
(
    "IdNotaFiscalStatus" SMALLINT PRIMARY KEY NOT NULL,
    "Descricao" VARCHAR2(50)
);
    
CREATE SEQUENCE NotaFiscalStatus_Sequence;

CREATE OR REPLACE TRIGGER NotaFiscalStatus_On_Insert
    BEFORE INSERT ON "NotaFiscalStatus"
    FOR EACH ROW
BEGIN
    SELECT NotaFiscalStatus_Sequence.nextval
    INTO :new."IdNotaFiscalStatus"
    FROM dual;
END;

INSERT INTO "NotaFiscalStatus" ("Descricao") VALUES ('Aguardando recebimento');
INSERT INTO "NotaFiscalStatus" ("Descricao") VALUES ('Recebida');
INSERT INTO "NotaFiscalStatus" ("Descricao") VALUES ('Conferida com sucesso');
INSERT INTO "NotaFiscalStatus" ("Descricao") VALUES ('Conferida com divergência');

COMMIT