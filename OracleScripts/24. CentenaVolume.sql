CREATE TABLE "CentenaVolume"
(
    "IdEmpresa" NUMBER(19) REFERENCES "Empresa"("IdEmpresa") NOT NULL,
    "Numero" NUMBER(10,0) NOT NULL
);

CREATE SEQUENCE CentenaVolume_Sequence;

CREATE OR REPLACE TRIGGER CentenaVolume_On_Insert
    BEFORE INSERT ON "CentenaVolume"
    FOR EACH ROW
BEGIN
    SELECT CentenaVolume_Sequence.nextval
    INTO :new."Numero"
    FROM dual;
END;