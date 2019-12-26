CREATE TABLE "LogEtiquetagem"
(
    "IdLogEtiquetagem" NUMBER PRIMARY KEY NOT NULL ,
    "IdEmpresa" NUMBER REFERENCES "Empresa"("IdEmpresa") NOT NULL,
    "IdProduto" NUMBER REFERENCES "Produto"("IdProduto") NOT NULL,
	"IdTipoEtiquetagem" SMALLINT REFERENCES "TipoEtiquetagem"("IdTipoEtiquetagem") NOT NULL,
	"Quantidade" NUMBER NOT NULL,
    "DataHora" TIMESTAMP NOT NULL,
    "IdUsuario" VARCHAR2(128 BYTE) REFERENCES "AspNetUsers"("Id") NOT NULL
);
    
CREATE SEQUENCE LogEtiquetagem_Sequence;

CREATE OR REPLACE TRIGGER LogEtiquetagem_On_Insert
    BEFORE INSERT ON "LogEtiquetagem"
    FOR EACH ROW
BEGIN
    SELECT LogEtiquetagem_Sequence.nextval
    INTO :new."IdLogEtiquetagem"
    FROM dual;
END;
