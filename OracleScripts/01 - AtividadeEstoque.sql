CREATE TABLE "AtividadeEstoqueTipo" 
(
  "IdAtividadeEstoqueTipo" NUMBER(10) NOT NULL 
, "Descricao" VARCHAR2(50) NOT NULL 
, CONSTRAINT "AtividadeEstoqueTipo_PK" PRIMARY KEY 
  (
    "IdAtividadeEstoqueTipo" 
  )
  USING INDEX 
  (
      CREATE UNIQUE INDEX "AtividadeEstoqueTipo_PK" ON "AtividadeEstoqueTipo" ("IdAtividadeEstoqueTipo" ASC) 
  )
  ENABLE 
);

CREATE UNIQUE INDEX "AtividadeEstoqueTipo_INDEX1" ON DART."AtividadeEstoqueTipo" ("Descricao");


CREATE TABLE "AtividadeEstoque" 
(
  "IdAtividadeEstoque" NUMBER(19) NOT NULL 
, "IdEmpresa" NUMBER(19) NOT NULL 
, "IdAtividadeEstoqueTipo" NUMBER(10) NOT NULL 
, "IdEnderecoArmazenagem" NUMBER(19) NOT NULL 
, "IdProduto" NUMBER(19) NOT NULL 
, "QuantidadeSistema" NUMBER(10) NOT NULL 
, "DataSolicitacao" DATE NOT NULL 
, "IdUsuarioSolicitacao" VARCHAR2(128) 
, "QuantidadeConferida" NUMBER(10) 
, "DataExecucao" DATE 
, "IdUsuarioExecucao" VARCHAR2(20) 
, "Finalizado" NUMBER(1) NOT NULL 
, CONSTRAINT "AtividadeEstoque_PK" PRIMARY KEY 
  (
    "IdAtividadeEstoque" 
  )
  USING INDEX 
  (
      CREATE UNIQUE INDEX "AtividadeEstoque_PK" ON "AtividadeEstoque" ("IdAtividadeEstoque" ASC) 
  )
  ENABLE 
);

CREATE SEQUENCE "AtividadeEstoque_SEQ";

CREATE TRIGGER "AtividadeEstoque_TRG" 
BEFORE INSERT ON "AtividadeEstoque" 
FOR EACH ROW 
BEGIN
  <<COLUMN_SEQUENCES>>
  BEGIN
    IF INSERTING AND :NEW."Finalizado" IS NULL THEN
      SELECT "AtividadeEstoque_SEQ".NEXTVAL INTO :NEW."Finalizado" FROM SYS.DUAL;
    END IF;
  END COLUMN_SEQUENCES;
END;
/

ALTER TABLE DART."AtividadeEstoque" ADD CONSTRAINT "AtividadeEstoque_FK1" FOREIGN KEY ("IdEmpresa") REFERENCES DART."Empresa"("IdEmpresa");
ALTER TABLE DART."AtividadeEstoque" ADD CONSTRAINT "AtividadeEstoque_FK2" FOREIGN KEY ("IdAtividadeEstoqueTipo") REFERENCES DART."AtividadeEstoqueTipo"("IdAtividadeEstoqueTipo");
ALTER TABLE DART."AtividadeEstoque" ADD CONSTRAINT "AtividadeEstoque_FK3" FOREIGN KEY ("IdEnderecoArmazenagem") REFERENCES DART."EnderecoArmazenagem"("IdEnderecoArmazenagem");
ALTER TABLE DART."AtividadeEstoque" ADD CONSTRAINT "AtividadeEstoque_FK4" FOREIGN KEY ("IdProduto") REFERENCES DART."Produto"("IdProduto");
ALTER TABLE DART."AtividadeEstoque" ADD CONSTRAINT "AtividadeEstoque_FK5" FOREIGN KEY ("IdUsuarioSolicitacao") REFERENCES DART."AspNetUsers"("Id");
ALTER TABLE DART."AtividadeEstoque" ADD CONSTRAINT "AtividadeEstoque_FK6" FOREIGN KEY ("IdUsuarioExecucao") REFERENCES DART."AspNetUsers"("Id");

INSERT INTO DART."AtividadeEstoqueTipo" ("IdAtividadeEstoqueTipo","Descricao")
	VALUES (1,'Conferência Endereço');
INSERT INTO DART."AtividadeEstoqueTipo" ("IdAtividadeEstoqueTipo","Descricao")
	VALUES (2,'Conferência 399/400');
INSERT INTO DART."AtividadeEstoqueTipo" ("IdAtividadeEstoqueTipo","Descricao")
	VALUES (3,'Abastecer Picking');

ALTER TABLE DART."AtividadeEstoque" RENAME COLUMN "QuantidadeSistema" TO "QuantidadeInicial";
ALTER TABLE DART."AtividadeEstoque" RENAME COLUMN "QuantidadeConferida" TO "QuantidadeFinal";
ALTER TABLE DART."AtividadeEstoque" MODIFY "QuantidadeInicial" NUMBER(10,0) NULL;


/
commit;

