CREATE TABLE "CaixaTipo" 
(
  "IdCaixaTipo" NUMBER(10) NOT NULL,
  "Descricao" VARCHAR2(50) NOT NULL, 
  CONSTRAINT "CaixaTipo_PK" PRIMARY KEY 
  (
    "IdCaixaTipo" 
  )
  USING INDEX 
  (
      CREATE UNIQUE INDEX "CaixaTipo_PK" ON "CaixaTipo" ("IdCaixaTipo" ASC) 
  )
  ENABLE 
);

CREATE UNIQUE INDEX "CaixaTipo_INDEX" ON DART."CaixaTipo" ("Descricao");

INSERT INTO DART."CaixaTipo" ("IdCaixaTipo","Descricao") VALUES (1,'Separa��o');
INSERT INTO DART."CaixaTipo" ("IdCaixaTipo","Descricao") VALUES (2,'Expedi��o');
INSERT INTO DART."CaixaTipo" ("IdCaixaTipo","Descricao") VALUES (3,'Recebimento');
INSERT INTO DART."CaixaTipo" ("IdCaixaTipo","Descricao") VALUES (4,'Garantia');
INSERT INTO DART."CaixaTipo" ("IdCaixaTipo","Descricao") VALUES (5,'Devolu��o');

COMMIT;