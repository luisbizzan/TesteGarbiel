CREATE TABLE Produto
(
    IdProduto NUMBER PRIMARY KEY NOT NULL,
    Descricao VARCHAR2(200),
    Referencia VARCHAR2(50),
    IdUnidadeMedida SMALLINT REFERENCES UnidadeMedida(IdUnidadeMedida) NOT NULL,
    Peso DECIMAL
);

CREATE SEQUENCE Produto_Sequence;

CREATE OR REPLACE TRIGGER Produto_On_Insert
    BEFORE INSERT ON Produto
    FOR EACH ROW
BEGIN
    SELECT Produto_Sequence.nextval
    INTO :new.IdProduto
    FROM dual;
END;