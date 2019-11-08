CREATE TABLE NotaFiscal
(
    IdNotaFiscal NUMBER PRIMARY KEY NOT NULL,
    Numero NUMBER,
    Serie NUMBER,
    DANFE VARCHAR2(100),
    IdFornecedor NUMBER REFERENCES Fornecedor(IdFornecedor) NOT NULL,    
    ValorTotal DECIMAL    
);

CREATE SEQUENCE NotaFiscal_Sequence;

CREATE OR REPLACE TRIGGER NotaFiscal_On_Insert
    BEFORE INSERT ON NotaFiscal
    FOR EACH ROW
BEGIN
    SELECT NotaFiscal_Sequence.nextval
    INTO :new.IdNotaFiscal
    FROM dual;
END;
