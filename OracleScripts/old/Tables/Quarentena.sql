CREATE TABLE "Quarentena"
(
    "IdQuarentena" NUMBER PRIMARY KEY NOT NULL,
    "IdLote" NUMBER REFERENCES "Lote"("IdLote") NOT NULL,
    "DataAbertura" TIMESTAMP(6),
    "DataEncerramento" TIMESTAMP(6),
    "Observacao" VARCHAR2(500),
    "IdQuarentenaStatus" SMALLINT REFERENCES "QuarentenaStatus"("IdQuarentenaStatus") NOT NULL
);

CREATE SEQUENCE Quarentena_Sequence;

CREATE OR REPLACE TRIGGER Quarentena_On_Insert
    BEFORE INSERT ON "Quarentena"
    FOR EACH ROW
BEGIN
    SELECT Quarentena_Sequence.nextval
    INTO :new."IdQuarentena"
    FROM dual;
END;