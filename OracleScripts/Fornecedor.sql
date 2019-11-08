CREATE TABLE "Fornecedor"
(
    "IdFornecedor" NUMBER PRIMARY KEY NOT NULL,
    "Codigo" VARCHAR2(100),
    "NomeFantasia" VARCHAR2(100),
    "RazaoSocial" VARCHAR2(100),
    "CNPJ" VARCHAR2(20)
);

CREATE SEQUENCE Fornecedor_Sequence;

CREATE OR REPLACE TRIGGER Fornecedor_On_Insert
    BEFORE INSERT ON "Fornecedor"
    FOR EACH ROW
BEGIN
    SELECT Fornecedor_Sequence.nextval
    INTO :new."IdFornecedor"
    FROM dual;
END;