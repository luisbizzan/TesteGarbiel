--------------------------------------------------------
--  DDL for Table PerfilImpressoraItem
--------------------------------------------------------

  CREATE TABLE "DART"."PerfilImpressoraItem" 
   (	"IdPerfilImpressora" NUMBER(19,0), 
	"IdImpressaoItem" NUMBER(10,0), 
	"IdImpressora" NUMBER(19,0)
   ) SEGMENT CREATION DEFERRED 
  PCTFREE 10 PCTUSED 40 INITRANS 1 MAXTRANS 255 
 NOCOMPRESS LOGGING
  TABLESPACE "USERS" ;
--------------------------------------------------------
--  DDL for Index PerfilImpressoraItem_PK
--------------------------------------------------------

  CREATE UNIQUE INDEX "DART"."PerfilImpressoraItem_PK" ON "DART"."PerfilImpressoraItem" ("IdPerfilImpressora", "IdImpressaoItem", "IdImpressora") 
  PCTFREE 10 INITRANS 2 MAXTRANS 255 
  TABLESPACE "USERS" ;
--------------------------------------------------------
--  Constraints for Table PerfilImpressoraItem
--------------------------------------------------------

  ALTER TABLE "DART"."PerfilImpressoraItem" ADD CONSTRAINT "PerfilImpressoraItem_PK" PRIMARY KEY ("IdPerfilImpressora", "IdImpressaoItem", "IdImpressora")
  USING INDEX PCTFREE 10 INITRANS 2 MAXTRANS 255 
  TABLESPACE "USERS"  ENABLE;
  ALTER TABLE "DART"."PerfilImpressoraItem" MODIFY ("IdImpressora" NOT NULL ENABLE);
  ALTER TABLE "DART"."PerfilImpressoraItem" MODIFY ("IdImpressaoItem" NOT NULL ENABLE);
  ALTER TABLE "DART"."PerfilImpressoraItem" MODIFY ("IdPerfilImpressora" NOT NULL ENABLE);
--------------------------------------------------------
--  Ref Constraints for Table PerfilImpressoraItem
--------------------------------------------------------

  ALTER TABLE "DART"."PerfilImpressoraItem" ADD CONSTRAINT "PERFILIMPRESSORAITEM_FK1" FOREIGN KEY ("IdPerfilImpressora")
	  REFERENCES "DART"."PerfilImpressora" ("IdPerfilImpressora") ENABLE;
  ALTER TABLE "DART"."PerfilImpressoraItem" ADD CONSTRAINT "PERFILIMPRESSORAITEM_FK2" FOREIGN KEY ("IdImpressaoItem")
	  REFERENCES "DART"."ImpressaoItem" ("IdImpressaoItem") ENABLE;
  ALTER TABLE "DART"."PerfilImpressoraItem" ADD CONSTRAINT "PERFILIMPRESSORAITEM_FK3" FOREIGN KEY ("IdImpressora")
	  REFERENCES "DART"."Printer" ("Id") ENABLE;
