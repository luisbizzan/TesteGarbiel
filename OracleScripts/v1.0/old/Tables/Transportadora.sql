CREATE TABLE "Transportadora"
(
    "IdTransportadora" NUMBER PRIMARY KEY NOT NULL,
    "NomeFantasia" VARCHAR2(100),
    "RazaoSocial" VARCHAR2(100),
    "CNPJ" VARCHAR2(20)
);

CREATE SEQUENCE Transportadora_Sequence;

CREATE OR REPLACE TRIGGER Transportadora_On_Insert
    BEFORE INSERT ON "Transportadora"
    FOR EACH ROW
BEGIN
    SELECT Transportadora_Sequence.nextval
    INTO :new."IdTransportadora"
    FROM dual;
END;