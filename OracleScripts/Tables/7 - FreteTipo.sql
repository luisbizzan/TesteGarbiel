CREATE TABLE FreteTipo
(
    IdFreteTipo SMALLINT PRIMARY KEY NOT NULL,
    Descricao VARCHAR2(20)
);

CREATE SEQUENCE FreteTipo_Sequence;

CREATE OR REPLACE TRIGGER FreteTipo_On_Insert
    BEFORE INSERT ON FreteTipo
    FOR EACH ROW
BEGIN
    SELECT FreteTipo_Sequence.nextval
    INTO :new.IdFreteTipo
    FROM dual;
END;