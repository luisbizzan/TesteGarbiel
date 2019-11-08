CREATE TABLE Frete
(
    IdFrete NUMBER PRIMARY KEY NOT NULL,
    IdNotaFiscal NUMBER REFERENCES NotaFiscal(IdNotaFiscal) NOT NULL,
    IdTransportadora NUMBER REFERENCES Transportadora(IdTransportadora) NOT NULL,	
    IdFreteTipo SMALLINT REFERENCES FreteTipo(IdFreteTipo) NOT NULL,
    Valor DECIMAL,
    NumeroConhecimento NUMBER,
    PesoBruto DECIMAL,
    PesoLiquido DECIMAL,
    Especie VARCHAR(20),
    Quantidade NUMBER
);

CREATE SEQUENCE Frete_Sequence;

CREATE OR REPLACE TRIGGER Frete_On_Insert
    BEFORE INSERT ON Frete
    FOR EACH ROW
BEGIN
    SELECT Frete_Sequence.nextval
    INTO :new.IdFrete
    FROM dual;
END;