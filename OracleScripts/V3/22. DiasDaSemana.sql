CREATE TABLE "DiasDaSemana" 
(
  "IdDiasDaSemana" NUMBER(10,0) NOT NULL
, "Descricao" VARCHAR(25) NOT NULL 
, CONSTRAINT "DiasDaSemana_PK" PRIMARY KEY 
  (
    "IdDiasDaSemana" 
  )
  USING INDEX 
  (
      CREATE UNIQUE INDEX "DiasDaSemana_PK" ON "DiasDaSemana" ("IdDiasDaSemana" ASC) 
  )
  ENABLE 
);

CREATE UNIQUE INDEX "DiasDaSemana_INDEX" ON DART."DiasDaSemana" ("IdDiasDaSemana","Descricao");


INSERT INTO DART."DiasDaSemana"("IdDiasDaSemana","Descricao") VALUES (0,'Domingo');
INSERT INTO DART."DiasDaSemana"("IdDiasDaSemana","Descricao") VALUES (1,'Segunda-Feira');
INSERT INTO DART."DiasDaSemana"("IdDiasDaSemana","Descricao") VALUES (2,'Terça-Feira');
INSERT INTO DART."DiasDaSemana"("IdDiasDaSemana","Descricao") VALUES (3,'Quarta-Feira');
INSERT INTO DART."DiasDaSemana"("IdDiasDaSemana","Descricao") VALUES (4,'Quinta-Feira');
INSERT INTO DART."DiasDaSemana"("IdDiasDaSemana","Descricao") VALUES (5,'Sexta-Feira');
INSERT INTO DART."DiasDaSemana"("IdDiasDaSemana","Descricao") VALUES (6,'Sábado');

commit