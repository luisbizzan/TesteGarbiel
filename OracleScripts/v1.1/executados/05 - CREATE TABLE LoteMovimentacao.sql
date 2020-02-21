--------------------------------------------------------
--  DDL for Table LoteProdutoEndereco
--------------------------------------------------------

  CREATE TABLE "DART"."LoteProdutoEndereco" 
   (	"IdLoteProdutoEndereco" NUMBER(19,0), 
	"IdEmpresa" NUMBER(19,0), 
	"IdLote" NUMBER(19,0), 
	"IdProduto" NUMBER(19,0), 
	"IdEnderecoArmazenagem" NUMBER(19,0), 
	"Quantidade" NUMBER(10,0), 
	"IdUsuarioInstalacao" VARCHAR2(128 BYTE), 
	"DataHoraInstalacao" TIMESTAMP (6), 
	"PesoTotal" NUMBER(38,4)
   ) SEGMENT CREATION DEFERRED 
  PCTFREE 10 PCTUSED 40 INITRANS 1 MAXTRANS 255 
 NOCOMPRESS LOGGING
  TABLESPACE "USERS" ;
--------------------------------------------------------
--  DDL for Index LoteProdutoEndereco_INDEX4
--------------------------------------------------------

  CREATE INDEX "DART"."LoteProdutoEndereco_INDEX4" ON "DART"."LoteProdutoEndereco" ("IdEnderecoArmazenagem") 
  PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  TABLESPACE "USERS" ;
--------------------------------------------------------
--  DDL for Index LoteProdutoEndereco_INDEX3
--------------------------------------------------------

  CREATE INDEX "DART"."LoteProdutoEndereco_INDEX3" ON "DART"."LoteProdutoEndereco" ("IdProduto") 
  PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  TABLESPACE "USERS" ;
--------------------------------------------------------
--  DDL for Index LoteProdutoEndereco_INDEX2
--------------------------------------------------------

  CREATE INDEX "DART"."LoteProdutoEndereco_INDEX2" ON "DART"."LoteProdutoEndereco" ("IdLote") 
  PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  TABLESPACE "USERS" ;
--------------------------------------------------------
--  DDL for Index LoteProdutoEndereco_INDEX1
--------------------------------------------------------

  CREATE INDEX "DART"."LoteProdutoEndereco_INDEX1" ON "DART"."LoteProdutoEndereco" ("IdEmpresa") 
  PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  TABLESPACE "USERS" ;
--------------------------------------------------------
--  DDL for Index IdLoteProdutoEndereco_INDEX
--------------------------------------------------------

  CREATE UNIQUE INDEX "DART"."IdLoteProdutoEndereco_INDEX" ON "DART"."LoteProdutoEndereco" ("IdLoteProdutoEndereco") 
  PCTFREE 10 INITRANS 2 MAXTRANS 255 
  TABLESPACE "USERS" ;
--------------------------------------------------------
--  DDL for Trigger LOTEPRODUTOENDERECO_ON_INSERT
--------------------------------------------------------

  CREATE OR REPLACE EDITIONABLE TRIGGER "DART"."LOTEPRODUTOENDERECO_ON_INSERT" 
    BEFORE INSERT ON "LoteProdutoEndereco"
    FOR EACH ROW
BEGIN
    SELECT LoteProdutoEndereco_Sequence.nextval
    INTO :new."IdLoteProdutoEndereco"
    FROM dual;
END;
/
ALTER TRIGGER "DART"."LOTEPRODUTOENDERECO_ON_INSERT" ENABLE;
--------------------------------------------------------
--  Constraints for Table LoteProdutoEndereco
--------------------------------------------------------

  ALTER TABLE "DART"."LoteProdutoEndereco" ADD PRIMARY KEY ("IdLoteProdutoEndereco")
  USING INDEX PCTFREE 10 INITRANS 2 MAXTRANS 255 
  TABLESPACE "USERS"  ENABLE;
  ALTER TABLE "DART"."LoteProdutoEndereco" MODIFY ("PesoTotal" NOT NULL ENABLE);
  ALTER TABLE "DART"."LoteProdutoEndereco" MODIFY ("DataHoraInstalacao" NOT NULL ENABLE);
  ALTER TABLE "DART"."LoteProdutoEndereco" MODIFY ("IdUsuarioInstalacao" NOT NULL ENABLE);
  ALTER TABLE "DART"."LoteProdutoEndereco" MODIFY ("Quantidade" NOT NULL ENABLE);
  ALTER TABLE "DART"."LoteProdutoEndereco" MODIFY ("IdEnderecoArmazenagem" NOT NULL ENABLE);
  ALTER TABLE "DART"."LoteProdutoEndereco" MODIFY ("IdProduto" NOT NULL ENABLE);
  ALTER TABLE "DART"."LoteProdutoEndereco" MODIFY ("IdLote" NOT NULL ENABLE);
  ALTER TABLE "DART"."LoteProdutoEndereco" MODIFY ("IdEmpresa" NOT NULL ENABLE);
  ALTER TABLE "DART"."LoteProdutoEndereco" MODIFY ("IdLoteProdutoEndereco" NOT NULL ENABLE);
--------------------------------------------------------
--  Ref Constraints for Table LoteProdutoEndereco
--------------------------------------------------------

  ALTER TABLE "DART"."LoteProdutoEndereco" ADD FOREIGN KEY ("IdEmpresa")
	  REFERENCES "DART"."Empresa" ("IdEmpresa") ENABLE;
  ALTER TABLE "DART"."LoteProdutoEndereco" ADD FOREIGN KEY ("IdLote")
	  REFERENCES "DART"."Lote" ("IdLote") ENABLE;
  ALTER TABLE "DART"."LoteProdutoEndereco" ADD FOREIGN KEY ("IdProduto")
	  REFERENCES "DART"."Produto" ("IdProduto") ENABLE;
  ALTER TABLE "DART"."LoteProdutoEndereco" ADD FOREIGN KEY ("IdEnderecoArmazenagem")
	  REFERENCES "DART"."EnderecoArmazenagem" ("IdEnderecoArmazenagem") ENABLE;
  ALTER TABLE "DART"."LoteProdutoEndereco" ADD FOREIGN KEY ("IdUsuarioInstalacao")
	  REFERENCES "DART"."AspNetUsers" ("Id") ENABLE;
