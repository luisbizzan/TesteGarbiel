--------------------------------------------------------
--  DDL for Table LoteMovimentacao
--------------------------------------------------------

  CREATE TABLE "DART"."LoteMovimentacao" 
   (	"IdLoteMovimentacao" NUMBER(19,0), 
	"IdLote" NUMBER, 
	"IdProduto" NUMBER, 
	"IdEnderecoArmazenagem" NUMBER, 
	"IdUsuarioMovimentacao" VARCHAR2(128 BYTE), 
	"IdLoteMovimentacaoTipo" NUMBER(10,0), 
	"Quantidade" NUMBER(10,0), 
	"DataHora" TIMESTAMP (6)
   ) SEGMENT CREATION DEFERRED 
  PCTFREE 10 PCTUSED 40 INITRANS 1 MAXTRANS 255 
 NOCOMPRESS LOGGING
  TABLESPACE "USERS" ;
--------------------------------------------------------
--  DDL for Index LoteMovimentacao_INDEX3
--------------------------------------------------------

  CREATE INDEX "DART"."LoteMovimentacao_INDEX3" ON "DART"."LoteMovimentacao" ("IdEnderecoArmazenagem") 
  PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  TABLESPACE "USERS" ;
--------------------------------------------------------
--  DDL for Index LoteMovimentacao_INDEX2
--------------------------------------------------------

  CREATE INDEX "DART"."LoteMovimentacao_INDEX2" ON "DART"."LoteMovimentacao" ("IdProduto") 
  PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  TABLESPACE "USERS" ;
--------------------------------------------------------
--  DDL for Index LoteMovimentacao_INDEX1
--------------------------------------------------------

  CREATE INDEX "DART"."LoteMovimentacao_INDEX1" ON "DART"."LoteMovimentacao" ("IdLote") 
  PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  TABLESPACE "USERS" ;
--------------------------------------------------------
--  DDL for Index LoteMovimentacao_INDEX
--------------------------------------------------------

  CREATE UNIQUE INDEX "DART"."LoteMovimentacao_INDEX" ON "DART"."LoteMovimentacao" ("IdLoteMovimentacao") 
  PCTFREE 10 INITRANS 2 MAXTRANS 255 
  TABLESPACE "USERS" ;
--------------------------------------------------------
--  DDL for Trigger LOTEMOVIMENTACAO_ON_INSERT
--------------------------------------------------------

  CREATE OR REPLACE EDITIONABLE TRIGGER "DART"."LOTEMOVIMENTACAO_ON_INSERT" 
    BEFORE INSERT ON "LoteMovimentacao"
    FOR EACH ROW
BEGIN
    SELECT LoteMovimentacao_Sequence.nextval
    INTO :new."IdLoteMovimentacao"
    FROM dual;
END;
/
ALTER TRIGGER "DART"."LOTEMOVIMENTACAO_ON_INSERT" ENABLE;
--------------------------------------------------------
--  Constraints for Table LoteMovimentacao
--------------------------------------------------------

  ALTER TABLE "DART"."LoteMovimentacao" ADD PRIMARY KEY ("IdLoteMovimentacao")
  USING INDEX PCTFREE 10 INITRANS 2 MAXTRANS 255 
  TABLESPACE "USERS"  ENABLE;
  ALTER TABLE "DART"."LoteMovimentacao" MODIFY ("DataHora" NOT NULL ENABLE);
  ALTER TABLE "DART"."LoteMovimentacao" MODIFY ("Quantidade" NOT NULL ENABLE);
  ALTER TABLE "DART"."LoteMovimentacao" MODIFY ("IdLoteMovimentacaoTipo" NOT NULL ENABLE);
  ALTER TABLE "DART"."LoteMovimentacao" MODIFY ("IdUsuarioMovimentacao" NOT NULL ENABLE);
  ALTER TABLE "DART"."LoteMovimentacao" MODIFY ("IdEnderecoArmazenagem" NOT NULL ENABLE);
  ALTER TABLE "DART"."LoteMovimentacao" MODIFY ("IdLote" NOT NULL ENABLE);
  ALTER TABLE "DART"."LoteMovimentacao" MODIFY ("IdLoteMovimentacao" NOT NULL ENABLE);
--------------------------------------------------------
--  Ref Constraints for Table LoteMovimentacao
--------------------------------------------------------

  ALTER TABLE "DART"."LoteMovimentacao" ADD FOREIGN KEY ("IdLote")
	  REFERENCES "DART"."Lote" ("IdLote") ENABLE;
  ALTER TABLE "DART"."LoteMovimentacao" ADD FOREIGN KEY ("IdProduto")
	  REFERENCES "DART"."Produto" ("IdProduto") ENABLE;
  ALTER TABLE "DART"."LoteMovimentacao" ADD FOREIGN KEY ("IdEnderecoArmazenagem")
	  REFERENCES "DART"."EnderecoArmazenagem" ("IdEnderecoArmazenagem") ENABLE;
  ALTER TABLE "DART"."LoteMovimentacao" ADD FOREIGN KEY ("IdUsuarioMovimentacao")
	  REFERENCES "DART"."AspNetUsers" ("Id") ENABLE;
  ALTER TABLE "DART"."LoteMovimentacao" ADD FOREIGN KEY ("IdLoteMovimentacaoTipo")
	  REFERENCES "DART"."LoteMovimentacaoTipo" ("IdLoteMovimentacaoTipo") ENABLE;
