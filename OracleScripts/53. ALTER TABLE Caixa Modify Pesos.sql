ALTER TABLE DART."Caixa" ADD "PesoCaixa2" NUMBER(38,4) NULL;
ALTER TABLE DART."Caixa" ADD "PesoMaximo2" NUMBER(38,4) NULL;

UPDATE "Caixa" SET "PesoCaixa2" = "PesoCaixa", "PesoMaximo2" = "PesoMaximo";

ALTER TABLE DART."Caixa" DROP COLUMN "PesoCaixa";
ALTER TABLE DART."Caixa" DROP COLUMN "PesoMaximo";

ALTER TABLE DART."Caixa" RENAME COLUMN "PesoCaixa2" TO "PesoCaixa";
ALTER TABLE DART."Caixa" RENAME COLUMN "PesoMaximo2" TO "PesoMaximo";
  
ALTER TABLE DART."Caixa" MODIFY "PesoCaixa" NUMBER(38,4) NOT NULL;
ALTER TABLE DART."Caixa" MODIFY "PesoMaximo" NUMBER(38,4) NOT NULL;

SELECT * FROM "Caixa";