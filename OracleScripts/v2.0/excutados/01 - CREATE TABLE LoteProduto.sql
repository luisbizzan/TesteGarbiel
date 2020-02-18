--------------------------------------------------------
--  DDL for Table LoteProduto
--------------------------------------------------------

  CREATE TABLE "DART"."LoteProduto" 
   (	"IdLoteProduto" NUMBER(19,0), 
	"IdEmpresa" NUMBER(19,0), 
	"IdLote" NUMBER(19,0), 
	"IdProduto" NUMBER(19,0), 
	"QuantidadeRecebida" NUMBER(10,0), 
	"Saldo" NUMBER(10,0)
   ) SEGMENT CREATION DEFERRED 
  PCTFREE 10 PCTUSED 40 INITRANS 1 MAXTRANS 255 
 NOCOMPRESS LOGGING
  TABLESPACE "USERS" ;
--------------------------------------------------------
--  DDL for Index LoteProduto_INDEX3
--------------------------------------------------------

  CREATE INDEX "DART"."LoteProduto_INDEX3" ON "DART"."LoteProduto" ("IdProduto") 
  PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  TABLESPACE "USERS" ;
--------------------------------------------------------
--  DDL for Index LoteProduto_INDEX2
--------------------------------------------------------

  CREATE INDEX "DART"."LoteProduto_INDEX2" ON "DART"."LoteProduto" ("IdLote") 
  PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  TABLESPACE "USERS" ;
--------------------------------------------------------
--  DDL for Index LoteProduto_INDEX1
--------------------------------------------------------

  CREATE INDEX "DART"."LoteProduto_INDEX1" ON "DART"."LoteProduto" ("IdEmpresa") 
  PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  TABLESPACE "USERS" ;
--------------------------------------------------------
--  DDL for Index IdLoteProduto_PK
--------------------------------------------------------

  CREATE UNIQUE INDEX "DART"."IdLoteProduto_PK" ON "DART"."LoteProduto" ("IdLoteProduto") 
  PCTFREE 10 INITRANS 2 MAXTRANS 255 
  TABLESPACE "USERS" ;
--------------------------------------------------------
--  DDL for Trigger LOTEPRODUTO_ON_INSERT
--------------------------------------------------------

  CREATE OR REPLACE EDITIONABLE TRIGGER "DART"."LOTEPRODUTO_ON_INSERT" 
    BEFORE INSERT ON "LoteProduto"
    FOR EACH ROW
BEGIN
    SELECT LoteProduto_Sequence.nextval
    INTO :new."IdLoteProduto"
    FROM dual;
END;
/
ALTER TRIGGER "DART"."LOTEPRODUTO_ON_INSERT" ENABLE;
--------------------------------------------------------
--  Constraints for Table LoteProduto
--------------------------------------------------------

  ALTER TABLE "DART"."LoteProduto" ADD PRIMARY KEY ("IdLoteProduto")
  USING INDEX PCTFREE 10 INITRANS 2 MAXTRANS 255 
  TABLESPACE "USERS"  ENABLE;
  ALTER TABLE "DART"."LoteProduto" MODIFY ("Saldo" NOT NULL ENABLE);
  ALTER TABLE "DART"."LoteProduto" MODIFY ("QuantidadeRecebida" NOT NULL ENABLE);
  ALTER TABLE "DART"."LoteProduto" MODIFY ("IdProduto" NOT NULL ENABLE);
  ALTER TABLE "DART"."LoteProduto" MODIFY ("IdLote" NOT NULL ENABLE);
  ALTER TABLE "DART"."LoteProduto" MODIFY ("IdEmpresa" NOT NULL ENABLE);
  ALTER TABLE "DART"."LoteProduto" MODIFY ("IdLoteProduto" NOT NULL ENABLE);
--------------------------------------------------------
--  Ref Constraints for Table LoteProduto
--------------------------------------------------------

  ALTER TABLE "DART"."LoteProduto" ADD FOREIGN KEY ("IdEmpresa")
	  REFERENCES "DART"."Empresa" ("IdEmpresa") ENABLE;
  ALTER TABLE "DART"."LoteProduto" ADD FOREIGN KEY ("IdLote")
	  REFERENCES "DART"."Lote" ("IdLote") ENABLE;
  ALTER TABLE "DART"."LoteProduto" ADD FOREIGN KEY ("IdProduto")
	  REFERENCES "DART"."Produto" ("IdProduto") ENABLE;
