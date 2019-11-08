CREATE TABLE UnidadeMedida
(
    IdUnidadeMedida SMALLINT PRIMARY KEY NOT NULL,
    Descricao VARCHAR2(10)
);

CREATE SEQUENCE UnidadeMedida_Sequence;

CREATE OR REPLACE TRIGGER UnidadeMedida_On_Insert
    BEFORE INSERT ON UnidadeMedida
    FOR EACH ROW
BEGIN
    SELECT UnidadeMedida_Sequence.nextval
    INTO :new.IdUnidadeMedida
    FROM dual;
END;