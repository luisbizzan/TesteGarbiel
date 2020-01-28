CREATE TABLE "ProdutoEmpresa"
(
    "IdProdutoEmpresa" NUMBER(19,0) PRIMARY KEY NOT NULL ,
    "IdProduto" NUMBER(19,0) REFERENCES "Produto"("IdProduto") NOT NULL,
	"IdEmpresa" NUMBER(19,0) REFERENCES "Empresa" ("IdEmpresa") NOT NULL,
    "Ativo" NUMBER(1,0) NOT NULL
);
    
CREATE SEQUENCE ProdutoEmpresa_Sequence;

CREATE OR REPLACE TRIGGER ProdutoEmpresa_On_Insert
    BEFORE INSERT ON "ProdutoEmpresa"
    FOR EACH ROW
BEGIN
    SELECT ProdutoEmpresa_Sequence.nextval
    INTO :new."IdProdutoEmpresa"
    FROM dual;
END;