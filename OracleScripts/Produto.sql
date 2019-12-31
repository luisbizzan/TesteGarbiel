ALTER TABLE DART."Produto" ADD "IdUnidadeMedida" NUMBER(19,0);

ALTER TABLE DART."Produto" ADD CONSTRAINT "Produto_FK" FOREIGN KEY ("IdUnidadeMedida") REFERENCES DART."UnidadeMedida"("IdUnidadeMedida");

UPDATE DART."Produto" SET "IdUnidadeMedida" = (SELECT "IdUnidadeMedida" FROM "UnidadeMedida" WHERE "Sigla" = 'PC')

ALTER TABLE DART."Produto" MODIFY "IdUnidadeMedida" NUMBER(19,0) NOT NULL;

COMMIT