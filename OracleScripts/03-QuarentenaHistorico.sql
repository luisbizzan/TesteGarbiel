--------------------------------------------------------
--  DDL for Table QuarentenaHistorico
--------------------------------------------------------

  CREATE TABLE "DART"."QuarentenaHistorico" 
   (	"IdQuarentenaHistorico" NUMBER(19,0), 
	"IdQuarentena" NUMBER(19,0), 
	"IdUsuario" VARCHAR2(128 BYTE), 
	"Data" DATE, 
	"Descricao" VARCHAR2(500 BYTE)
   ) SEGMENT CREATION IMMEDIATE 
  PCTFREE 10 PCTUSED 40 INITRANS 1 MAXTRANS 255 
 NOCOMPRESS LOGGING
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "USERS" ;
--------------------------------------------------------
--  DDL for Index QuarentenaHistorico_PK
--------------------------------------------------------

  CREATE UNIQUE INDEX "DART"."QuarentenaHistorico_PK" ON "DART"."QuarentenaHistorico" ("IdQuarentenaHistorico") 
  PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "USERS" ;
--------------------------------------------------------
--  DDL for Index QuarentenaHistorico_INDEX1
--------------------------------------------------------

  CREATE INDEX "DART"."QuarentenaHistorico_INDEX1" ON "DART"."QuarentenaHistorico" ("IdQuarentena") 
  PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "USERS" ;
--------------------------------------------------------
--  DDL for Index QuarentenaHistorico_INDEX2
--------------------------------------------------------

  CREATE INDEX "DART"."QuarentenaHistorico_INDEX2" ON "DART"."QuarentenaHistorico" ("IdUsuario") 
  PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "USERS" ;
  
--------------------------------------------------------
--  DDL for Sequence QuarentenaHistorico_SEQ
--------------------------------------------------------

   CREATE SEQUENCE  "DART"."QuarentenaHistorico_SEQ"  MINVALUE 1 MAXVALUE 9999999999999999999999999999 INCREMENT BY 1 START WITH 1 CACHE 20 NOORDER  NOCYCLE;

--------------------------------------------------------
--  DDL for Trigger QuarentenaHistorico_SEQ_TR
--------------------------------------------------------

  CREATE OR REPLACE EDITIONABLE TRIGGER "DART"."QuarentenaHistorico_SEQ_TR" 
   before insert on "DART"."QuarentenaHistorico" 
   for each row 
begin  
   if inserting then 
      if :NEW."IdQuarentenaHistorico" is null then 
         select "QuarentenaHistorico_SEQ".nextval into :NEW."IdQuarentenaHistorico" from dual; 
      end if; 
   end if; 
end;
/
ALTER TRIGGER "DART"."QuarentenaHistorico_SEQ_TR" ENABLE;
--------------------------------------------------------
--  Constraints for Table QuarentenaHistorico
--------------------------------------------------------

  ALTER TABLE "DART"."QuarentenaHistorico" ADD CONSTRAINT "QuarentenaHistorico_PK" PRIMARY KEY ("IdQuarentenaHistorico")
  USING INDEX PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "USERS"  ENABLE;
  ALTER TABLE "DART"."QuarentenaHistorico" MODIFY ("Data" NOT NULL ENABLE);
  ALTER TABLE "DART"."QuarentenaHistorico" MODIFY ("IdQuarentena" NOT NULL ENABLE);
  ALTER TABLE "DART"."QuarentenaHistorico" MODIFY ("IdQuarentenaHistorico" NOT NULL ENABLE);
--------------------------------------------------------
--  Ref Constraints for Table QuarentenaHistorico
--------------------------------------------------------

  ALTER TABLE "DART"."QuarentenaHistorico" ADD CONSTRAINT "QuarentenaHistorico_FK1" FOREIGN KEY ("IdQuarentena")
	  REFERENCES "DART"."Quarentena" ("IdQuarentena") ENABLE;
  ALTER TABLE "DART"."QuarentenaHistorico" ADD CONSTRAINT "QuarentenaHistorico_FK2" FOREIGN KEY ("IdUsuario")
	  REFERENCES "DART"."AspNetUsers" ("Id") ENABLE;
