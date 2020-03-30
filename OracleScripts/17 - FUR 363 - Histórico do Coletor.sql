CREATE TABLE "ColetorAplicacao" 
( 
 "IdColetorAplicacao" NUMBER(10) NOT NULL 
, "Descricao" VARCHAR2(50) UNIQUE NOT NULL

, CONSTRAINT "ColetorAplicacao_PK" PRIMARY KEY 
  (
    "IdColetorAplicacao" 
  )
  USING INDEX 
  (
      CREATE UNIQUE INDEX "ColetorAplicacao_PK" ON "ColetorAplicacao" ("IdColetorAplicacao" ASC ) 
  )
  ENABLE 
);

CREATE TABLE "ColetorHistoricoTipo" 
( 
 "IdColetorHistoricoTipo" NUMBER(10) NOT NULL 
, "Descricao" VARCHAR2(50) UNIQUE NOT NULL

, CONSTRAINT "ColetorHistoricoTipo_PK" PRIMARY KEY 
  (
    "IdColetorHistoricoTipo" 
  )
  USING INDEX 
  (
      CREATE UNIQUE INDEX "ColetorHistoricoTipo_PK" ON "ColetorHistoricoTipo" ("IdColetorHistoricoTipo" ASC ) 
  )
  ENABLE 
);

CREATE TABLE "ColetorHistorico" 
(
  "IdColetorHistorico" NUMBER(19) NOT NULL 
, "IdEmpresa" NUMBER(19) NOT NULL 
, "IdColetorAplicacao" NUMBER(10) NOT NULL 
, "IdColetorHistoricoTipo " NUMBER(10) NOT NULL 
, "IdUsuario" VARCHAR2(128) NOT NULL 
, "Descricao" VARCHAR2(500) NOT NULL
, "DataHora" TIMESTAMP NOT NULL
 
, CONSTRAINT "ColetorHistorico_PK" PRIMARY KEY 
  (
    "IdColetorHistorico" 
  )
  
  USING INDEX 
  (
      CREATE UNIQUE INDEX "ColetorHistorico_PK" ON "ColetorHistorico" ("IdColetorHistorico" ASC ) 
  )
  ENABLE 
);


CREATE INDEX "ColetorHistorico_INDEX1" ON "ColetorHistorico" ("IdColetorAplicacao" ASC);

CREATE INDEX "ColetorHistorico_INDEX2" ON "ColetorHistorico" ("IdColetorHistoricoTipo" ASC);

CREATE INDEX "ColetorHistorico_INDEX3" ON "ColetorHistorico" ("IdUsuario" ASC);

CREATE INDEX "ColetorHistorico_INDEX4" ON "ColetorHistorico" ("IdEmpresa" ASC);

CREATE SEQUENCE "ColetorHistorico_SEQ";

CREATE TRIGGER "ColetorHistorico_SEQ_TRG" 
BEFORE INSERT ON "ColetorHistorico" 
FOR EACH ROW 
BEGIN
  <<COLUMN_SEQUENCES>>
  BEGIN
    IF INSERTING AND :NEW."IdColetorHistorico" IS NULL THEN
      SELECT "ColetorHistorico_SEQ".NEXTVAL INTO :NEW."IdColetorHistorico" FROM SYS.DUAL;
    END IF;
  END COLUMN_SEQUENCES;
END;

ALTER TABLE DART."ColetorHistorico" ADD CONSTRAINT "ColetorHistorico_FK1" FOREIGN KEY ("IdColetorAplicacao") REFERENCES DART."ColetorAplicacao"("IdColetorAplicacao");
ALTER TABLE DART."ColetorHistorico" ADD CONSTRAINT "ColetorHistorico_FK2" FOREIGN KEY ("IdColetorHistoricoTipo") REFERENCES DART."ColetorHistoricoTipo"("IdColetorHistoricoTipo");
ALTER TABLE DART."ColetorHistorico" ADD CONSTRAINT "ColetorHistorico_FK3" FOREIGN KEY ("IdUsuario") REFERENCES DART."AspNetUsers"("Id");
ALTER TABLE DART."ColetorHistorico" ADD CONSTRAINT "ColetorHistorico_FK4" FOREIGN KEY ("IdEmpresa") REFERENCES DART."Empresa"("IdEmpresa");



INSERT INTO DART."ColetorAplicacao"("IdColetorAplicacao","Descricao") VALUES 
(1,'Armazenagem');
INSERT INTO DART."ColetorAplicacao"("IdColetorAplicacao","Descricao") VALUES 
(2,'Separação');
INSERT INTO DART."ColetorAplicacao"("IdColetorAplicacao","Descricao") VALUES 
(3,'Expedição');





INSERT INTO DART."ColetorHistoricoTipo"("IdColetorHistoricoTipo","Descricao") VALUES 
(1,'Instalar Produto');
INSERT INTO DART."ColetorHistoricoTipo"("IdColetorHistoricoTipo","Descricao") VALUES 
(2,'Retirar Produto');
INSERT INTO DART."ColetorHistoricoTipo"("IdColetorHistoricoTipo","Descricao") VALUES 
(3,'Ajustar Quantidade');
INSERT INTO DART."ColetorHistoricoTipo"("IdColetorHistoricoTipo","Descricao") VALUES 
(4,'Conferir Endereço');
INSERT INTO DART."ColetorHistoricoTipo"("IdColetorHistoricoTipo","Descricao") VALUES 
(5,'Imprimir Etiqueta');





