--------------------------------------------------------
--  DDL for Table ImpressaoItem
--------------------------------------------------------

  CREATE TABLE "DART"."ImpressaoItem" 
   (	"IdImpressaoItem" NUMBER(10,0), 
	"Descricao" VARCHAR2(50 BYTE)
   ) SEGMENT CREATION DEFERRED 
  PCTFREE 10 PCTUSED 40 INITRANS 1 MAXTRANS 255 
 NOCOMPRESS LOGGING
  TABLESPACE "USERS" ;
--------------------------------------------------------
--  DDL for Index ImpressaoItem_PK
--------------------------------------------------------

  CREATE UNIQUE INDEX "DART"."ImpressaoItem_PK" ON "DART"."ImpressaoItem" ("IdImpressaoItem") 
  PCTFREE 10 INITRANS 2 MAXTRANS 255 
  TABLESPACE "USERS" ;
--------------------------------------------------------
--  Constraints for Table ImpressaoItem
--------------------------------------------------------

  ALTER TABLE "DART"."ImpressaoItem" ADD CONSTRAINT "ImpressaoItem_PK" PRIMARY KEY ("IdImpressaoItem")
  USING INDEX PCTFREE 10 INITRANS 2 MAXTRANS 255 
  TABLESPACE "USERS"  ENABLE;
  ALTER TABLE "DART"."ImpressaoItem" MODIFY ("Descricao" NOT NULL ENABLE);
  ALTER TABLE "DART"."ImpressaoItem" MODIFY ("IdImpressaoItem" NOT NULL ENABLE);
  
  
REM INSERTING into DART."ImpressaoItem"
SET DEFINE OFF;
Insert into DART."ImpressaoItem" ("IdImpressaoItem","Descricao") values ('1','Relatório A4');
Insert into DART."ImpressaoItem" ("IdImpressaoItem","Descricao") values ('2','Etiqueta de Volume');
Insert into DART."ImpressaoItem" ("IdImpressaoItem","Descricao") values ('3','Etiqueta Individual');
Insert into DART."ImpressaoItem" ("IdImpressaoItem","Descricao") values ('4','Etiqueta Padrão');
Insert into DART."ImpressaoItem" ("IdImpressaoItem","Descricao") values ('5','Etiqueta Avulso');
Insert into DART."ImpressaoItem" ("IdImpressaoItem","Descricao") values ('6','Etiqueta de Recebimento');
Insert into DART."ImpressaoItem" ("IdImpressaoItem","Descricao") values ('7','Etiqueta de Endereco');
Insert into DART."ImpressaoItem" ("IdImpressaoItem","Descricao") values ('8','Etiqueta de Picking');
commit;
