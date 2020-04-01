CREATE TABLE "LoteMovimentacaoTipo"
(
    "IdLoteMovimentacaoTipo"  NUMBER(10,0) PRIMARY KEY NOT NULL,
    "Descricao" VARCHAR(50) NOT NULL
);

CREATE SEQUENCE LoteMovimentacaoTipo_Sequence;

CREATE OR REPLACE TRIGGER LoteMovimentacaoTipo_On_Insert
    BEFORE INSERT ON "LoteMovimentacaoTipo"
    FOR EACH ROW
BEGIN
    SELECT LoteMovimentacaoTipo_Sequence.nextval
    INTO :new."IdLoteMovimentacaoTipo"
    FROM dual;
END;