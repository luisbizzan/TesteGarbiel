CREATE TABLE "GrupoCorredorArmazenagem"
(
    "IdGrupoCorredorArmazenagem" NUMBER(19,0) PRIMARY KEY NOT NULL,
	"IdEmpresa" NUMBER(19) REFERENCES "Empresa"("IdEmpresa") NOT NULL,
    "CorredorInicial" NUMBER(10,0) NOT NULL,
    "CorredorFinal" NUMBER(10,0) NOT NULL,
    "IdPontoArmazenagem" NUMBER(19,0) REFERENCES "PontoArmazenagem"("IdPontoArmazenagem") NOT NULL,
    "IdImpressora" NUMBER(38,0) REFERENCES "Printer"("Id") NOT NULL,
	"Ativo" NUMBER(1) NOT NULL
);

CREATE SEQUENCE GrupoCorredorArm_Sequence;

CREATE OR REPLACE TRIGGER GrupoCorredorArm_On_Insert
    BEFORE INSERT ON "GrupoCorredorArmazenagem"
    FOR EACH ROW
BEGIN
    SELECT GrupoCorredorArm_Sequence.nextval
    INTO :new."IdGrupoCorredorArmazenagem"
    FROM dual;
END;