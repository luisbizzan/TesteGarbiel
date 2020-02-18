--------------------------------------------------------
--  Arquivo criado - Sexta-feira-Novembro-29-2019   
--------------------------------------------------------
--------------------------------------------------------
--  DDL for Table Empresa
--------------------------------------------------------

  CREATE TABLE "DART"."Empresa" 
   (	"IdEmpresa" NUMBER, 
	"RazaoSocial" VARCHAR2(40 BYTE), 
	"Sigla" VARCHAR2(3 BYTE), 
	"NomeFantasia" VARCHAR2(40 BYTE), 
	"CNPJ" VARCHAR2(14 BYTE), 
	"CEP" NUMBER, 
	"Endereco" VARCHAR2(76 BYTE), 
	"Numero" NVARCHAR2(6), 
	"Complemento" VARCHAR2(10 BYTE), 
	"Bairro" VARCHAR2(50 BYTE), 
	"Estado" VARCHAR2(40 BYTE), 
	"Cidade" VARCHAR2(50 BYTE), 
	"Telefone" VARCHAR2(15 BYTE), 
	"Ativo" NUMBER(1,0) DEFAULT 0, 
	"CodigoIntegracao" NUMBER(5,0)
   ) SEGMENT CREATION IMMEDIATE 
  PCTFREE 10 PCTUSED 40 INITRANS 1 MAXTRANS 255 
 NOCOMPRESS LOGGING
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "USERS" ;
--------------------------------------------------------
--  DDL for Index Empresa_PK
--------------------------------------------------------

  CREATE UNIQUE INDEX "DART"."Empresa_PK" ON "DART"."Empresa" ("IdEmpresa") 
  PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "USERS" ;
--------------------------------------------------------
--  DDL for Index Empresa_INDEX2
--------------------------------------------------------

  CREATE INDEX "DART"."Empresa_INDEX2" ON "DART"."Empresa" ("CodigoIntegracao") 
  PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "USERS" ;
--------------------------------------------------------
--  DDL for Index Empresa_INDEX1
--------------------------------------------------------

  CREATE INDEX "DART"."Empresa_INDEX1" ON "DART"."Empresa" ("CNPJ") 
  PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "USERS" ;
--------------------------------------------------------
--  DDL for Index Empresa_INDEX4
--------------------------------------------------------

  CREATE INDEX "DART"."Empresa_INDEX4" ON "DART"."Empresa" ("NomeFantasia") 
  PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "USERS" ;
--------------------------------------------------------
--  DDL for Index Empresa_INDEX3
--------------------------------------------------------

  CREATE INDEX "DART"."Empresa_INDEX3" ON "DART"."Empresa" ("RazaoSocial") 
  PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "USERS" ;
--------------------------------------------------------
--  DDL for Trigger Empresa_SEQ_TR
--------------------------------------------------------

  CREATE OR REPLACE EDITIONABLE TRIGGER "DART"."Empresa_SEQ_TR" 
   before insert on "DART"."Empresa" 
   for each row 
begin  
   if inserting then 
      if :NEW."IdEmpresa" is null then 
         select "Empresa_SEQ".nextval into :NEW."IdEmpresa" from dual; 
      end if; 
   end if; 
end;
/
ALTER TRIGGER "DART"."Empresa_SEQ_TR" ENABLE;
--------------------------------------------------------
--  Constraints for Table Empresa
--------------------------------------------------------

  ALTER TABLE "DART"."Empresa" MODIFY ("Ativo" NOT NULL ENABLE);
  ALTER TABLE "DART"."Empresa" MODIFY ("RazaoSocial" NOT NULL ENABLE);
  ALTER TABLE "DART"."Empresa" MODIFY ("CNPJ" NOT NULL ENABLE);
  ALTER TABLE "DART"."Empresa" MODIFY ("NomeFantasia" NOT NULL ENABLE);
  ALTER TABLE "DART"."Empresa" ADD CONSTRAINT "Empresa_PK" PRIMARY KEY ("IdEmpresa")
  USING INDEX PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "USERS"  ENABLE;
  ALTER TABLE "DART"."Empresa" MODIFY ("Sigla" NOT NULL ENABLE);
  ALTER TABLE "DART"."Empresa" MODIFY ("IdEmpresa" NOT NULL ENABLE);
  ALTER TABLE "DART"."Empresa" MODIFY ("CodigoIntegracao" NOT NULL ENABLE);
