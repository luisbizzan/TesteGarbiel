CREATE TABLE "NotaFiscal"
(
    "IdNotaFiscal" NUMBER PRIMARY KEY NOT NULL,
    "Numero" NUMBER,
    "Serie" NUMBER,
    "DANFE" VARCHAR2(100),
    "IdFornecedor" NUMBER REFERENCES "Fornecedor"("IdFornecedor") NOT NULL,    
    "ValorTotal" DECIMAL,    
    "IdTransportadora" NUMBER REFERENCES "Transportadora"("IdTransportadora") NOT NULL,	
    "IdFreteTipo" SMALLINT REFERENCES "FreteTipo"("IdFreteTipo") NOT NULL,
    "Valor" DECIMAL,
    "NumeroConhecimento" NUMBER,
    "PesoBruto" DECIMAL,
    "PesoLiquido" DECIMAL,
    "Especie" VARCHAR(20),
    "Quantidade" NUMBER
);

CREATE SEQUENCE NotaFiscal_Sequence;

CREATE OR REPLACE TRIGGER NotaFiscal_On_Insert
    BEFORE INSERT ON "NotaFiscal"
    FOR EACH ROW
BEGIN
    SELECT NotaFiscal_Sequence.nextval
    INTO :new."IdNotaFiscal"
    FROM dual;
END;
