--------------------------------------------------------
--  DDL for Table PerfilImpressora
--------------------------------------------------------

  CREATE TABLE "DART"."PerfilImpressora" 
   (	"IdPerfilImpressora" NUMBER(19,0), 
	"IdEmpresa" NUMBER(19,0), 
	"Nome" VARCHAR2(50 BYTE), 
	"Ativo" NUMBER(1,0)
   ) SEGMENT CREATION DEFERRED 
  PCTFREE 10 PCTUSED 40 INITRANS 1 MAXTRANS 255 
 NOCOMPRESS LOGGING
  TABLESPACE "USERS" ;
--------------------------------------------------------
--  DDL for Index PerfilImpressora_INDEX1
--------------------------------------------------------

  CREATE INDEX "DART"."PerfilImpressora_INDEX1" ON "DART"."PerfilImpressora" ("IdEmpresa") 
  PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  TABLESPACE "USERS" ;
--------------------------------------------------------
--  DDL for Index PerfilImpressora_PK
--------------------------------------------------------

  CREATE UNIQUE INDEX "DART"."PerfilImpressora_PK" ON "DART"."PerfilImpressora" ("IdPerfilImpressora") 
  PCTFREE 10 INITRANS 2 MAXTRANS 255 
  TABLESPACE "USERS" ;
  
--------------------------------------------------------
--  DDL for Sequence PerfilImpressora_SEQ
--------------------------------------------------------

   CREATE SEQUENCE  "DART"."PerfilImpressora_SEQ"  MINVALUE 1 MAXVALUE 9999999999999999999999999999 INCREMENT BY 1 START WITH 1 CACHE 20 NOORDER  NOCYCLE   ;

--------------------------------------------------------
--  DDL for Trigger PerfilImpressora_SEQ_TR
--------------------------------------------------------

  CREATE OR REPLACE EDITIONABLE TRIGGER "DART"."PerfilImpressora_SEQ_TR" 
BEFORE INSERT ON "PerfilImpressora" 
FOR EACH ROW 
BEGIN  
  BEGIN
    IF INSERTING AND :NEW."IdPerfilImpressora" IS NULL THEN
      SELECT "PerfilImpressora_SEQ".NEXTVAL INTO :NEW."IdPerfilImpressora" FROM SYS.DUAL;
    END IF;
  END COLUMN_SEQUENCES;
END;
/
ALTER TRIGGER "DART"."PerfilImpressora_SEQ_TR" ENABLE;
--------------------------------------------------------
--  Constraints for Table PerfilImpressora
--------------------------------------------------------

  ALTER TABLE "DART"."PerfilImpressora" ADD CONSTRAINT "PerfilImpressora_PK" PRIMARY KEY ("IdPerfilImpressora")
  USING INDEX PCTFREE 10 INITRANS 2 MAXTRANS 255 
  TABLESPACE "USERS"  ENABLE;
  ALTER TABLE "DART"."PerfilImpressora" MODIFY ("Ativo" NOT NULL ENABLE);
  ALTER TABLE "DART"."PerfilImpressora" MODIFY ("Nome" NOT NULL ENABLE);
  ALTER TABLE "DART"."PerfilImpressora" MODIFY ("IdEmpresa" NOT NULL ENABLE);
  ALTER TABLE "DART"."PerfilImpressora" MODIFY ("IdPerfilImpressora" NOT NULL ENABLE);
--------------------------------------------------------
--  Ref Constraints for Table PerfilImpressora
--------------------------------------------------------

  ALTER TABLE "DART"."PerfilImpressora" ADD CONSTRAINT "PerfilImpressora_FK1" FOREIGN KEY ("IdEmpresa")
	  REFERENCES "DART"."Empresa" ("IdEmpresa") ENABLE;
