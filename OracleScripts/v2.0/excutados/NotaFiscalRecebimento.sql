--  DDL for Table NotaFiscalRecebimento 
--------------------------------------------------------

CREATE TABLE "DART"."NotaFiscalRecebimento" ( 
		"IdNotaFiscalRecebimento"   NUMBER(19,0)       NOT NULL ENABLE, 
		"IdUsuarioRecebimento"      VARCHAR2(128)      NOT NULL ENABLE, 
		"IdFornecedor"              NUMBER(19,0)       NOT NULL ENABLE, 
        "NumeroNF"                  NUMBER(10,0)       NOT NULL ENABLE, 
		"Serie"                     VARCHAR2(3 BYTE)   NOT NULL ENABLE, 
		"ChaveAcesso"               VARCHAR2(44 BYTE)  NOT NULL ENABLE, 
		"Valor"                     FLOAT(126)         NOT NULL ENABLE, 
		"QuantidadeVolumes"         NUMBER(10,0)       NOT NULL ENABLE, 
		"DataHoraRegistro"          TIMESTAMP (6)      NOT NULL ENABLE, 
		"DataHoraSincronismo"       TIMESTAMP (6)          NULL ENABLE, 
	    "IdNotaRecebimentoStatus"   NUMBER(19,0)       NOT NULL ENABLE,
CONSTRAINT "NotaFiscalRecebimento_PK" PRIMARY KEY ("IdNotaFiscalRecebimento")
	USING INDEX PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645 PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1 BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT) TABLESPACE "USERS" ENABLE,
CONSTRAINT "NotaFiscalRecebimento_FK1"  FOREIGN KEY ("IdFornecedor")            REFERENCES "DART"."Fornecedor" ("IdFornecedor")                       ENABLE,
CONSTRAINT "NotaFiscalRecebimento_FK2"  FOREIGN KEY ("IdUsuarioRecebimento")    REFERENCES "DART"."AspNetUsers" ("Id")                                ENABLE,
CONSTRAINT "NotaFiscalRecebimento_FK3"  FOREIGN KEY ("IdNotaRecebimentoStatus") REFERENCES "DART"."NotaRecebimentoStatus" ("IdNotaRecebimentoStatus") ENABLE,
)
SEGMENT CREATION IMMEDIATE PCTFREE 10 PCTUSED 40 INITRANS 1 MAXTRANS 255 NOCOMPRESS LOGGING STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645 PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1 BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT) TABLESPACE "USERS";


-- Add Coluna IdEmpresa 
ALTER  TABLE "NotaFiscalRecebimento" ADD "IdEmpresa" NUMBER(19,0) REFERENCES "Empresa"("IdEmpresa") NOT NULL;


--  DDL for Sequence NotaFiscalRecebimento_SEQ
--------------------------------------------------------
CREATE SEQUENCE  "DART"."NotaFiscalRecebimento_SEQ"  MINVALUE 1 MAXVALUE 9999999999999999999999999999 INCREMENT BY 1 START WITH 92721 CACHE 20 NOORDER  NOCYCLE   ;
   
--  DDL for Trigger NotaFiscalRecebimento_SEQ_TR
--------------------------------------------------------
CREATE OR REPLACE TRIGGER NotaFiscalRecebimento_SEQ_TR
    BEFORE INSERT ON "NotaFiscalRecebimento"
    FOR EACH ROW
BEGIN
    SELECT "NotaFiscalRecebimento_SEQ".nextval
    INTO :new."IdNotaFiscalRecebimento"
    FROM dual;
END;

--Rodas os seguintes Scripts
INSERT INTO "ImpressaoItem"   VALUES('9', 'Etiqueta de Recebimento Sem Nota');
INSERT INTO "TipoEtiquetagem" VALUES('7', 'RecebimentoSemNota');



