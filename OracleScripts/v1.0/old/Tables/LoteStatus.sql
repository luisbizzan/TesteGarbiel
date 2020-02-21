CREATE TABLE "LoteStatus"
(
    "IdLoteStatus" SMALLINT PRIMARY KEY NOT NULL,
    "Descricao" VARCHAR2(50)
);
    
CREATE SEQUENCE LoteStatus_Sequence;

CREATE OR REPLACE TRIGGER LoteStatus_On_Insert
    BEFORE INSERT ON "LoteStatus"
    FOR EACH ROW
BEGIN
    SELECT LoteStatus_Sequence.nextval
    INTO :new."IdLoteStatus"
    FROM dual;
END;