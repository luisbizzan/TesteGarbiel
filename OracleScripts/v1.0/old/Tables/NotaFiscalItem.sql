CREATE TABLE "NotaFiscalItem"
(
    "IdNotaFiscalItem" NUMBER PRIMARY KEY NOT NULL,
    "IdNotaFiscal" NUMBER REFERENCES "NotaFiscal"("IdNotaFiscal") NOT NULL,
    "IdProduto" NUMBER REFERENCES "Produto"("IdProduto") NOT NULL,
    "IdUnidadeMedida" SMALLINT REFERENCES "UnidadeMedida"("IdUnidadeMedida") NOT NULL,
    "Quantidade" NUMBER,
    "ValorUnitario" DECIMAL,
    "ValorTotal" DECIMAL
);

CREATE SEQUENCE NotaFiscalItem_Sequence;

CREATE OR REPLACE TRIGGER NotaFiscalItem_On_Insert
    BEFORE INSERT ON "NotaFiscalItem"
    FOR EACH ROW
BEGIN
    SELECT NotaFiscalItem_Sequence.nextval
    INTO :new."IdNotaFiscalItem"
    FROM dual;
END;
