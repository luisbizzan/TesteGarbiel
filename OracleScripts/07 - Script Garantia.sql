CREATE TABLE "MotivoLaudo" 
(
  "IdMotivoLaudo" NUMBER(19) NOT NULL 
, "Descricao" VARCHAR2(30) NOT NULL 
, "Ativo" NUMBER(1) NOT NULL 
, CONSTRAINT "MotivoLaudo_PK" PRIMARY KEY 
  (
    "IdMotivoLaudo" 
  )
  USING INDEX 
  (
      CREATE UNIQUE INDEX "MotivoLaudo_PK" ON "MotivoLaudo" ("IdMotivoLaudo" ASC)
  )  
  ENABLE 
);

CREATE UNIQUE INDEX "MotivoLaudo_INDEX1" ON "MotivoLaudo" ("Descricao");
;

CREATE SEQUENCE "MotivoLaudo_SEQ";

CREATE TRIGGER "MotivoLaudo_SEQ_TRG" 
BEFORE INSERT ON "MotivoLaudo" 
FOR EACH ROW 
BEGIN
  <<COLUMN_SEQUENCES>>
  BEGIN
    IF INSERTING AND :NEW."IdMotivoLaudo" IS NULL THEN
      SELECT "MotivoLaudo_SEQ".NEXTVAL INTO :NEW."IdMotivoLaudo" FROM SYS.DUAL;
    END IF;
  END COLUMN_SEQUENCES;
END;
/
--Cliente

CREATE TABLE "Cliente" 
(
  "IdCliente" NUMBER(19) NOT NULL 
, "Classificacao" VARCHAR2(14) NOT NULL 
, "NomeFantasia" VARCHAR2(180) NOT NULL 
, "RazaoSocial" VARCHAR2(75) NOT NULL 
, "CNPJ" VARCHAR2(14) NOT NULL 
, "CodigoIntegracao" NUMBER(10) NOT NULL 
, "Ativo" NUMBER(1) NOT NULL 
, CONSTRAINT "Cliente_PK" PRIMARY KEY 
  (
    "IdCliente" 
  )  
);

CREATE UNIQUE INDEX "Cliente_PK" ON "Cliente" ("IdCliente" ASC) ;

CREATE INDEX "Cliente_INDEX1" ON "Cliente" ("NomeFantasia" ASC);

CREATE INDEX "Cliente_INDEX2" ON "Cliente" ("RazaoSocial" ASC);

CREATE INDEX "Cliente_INDEX3" ON "Cliente" ("CNPJ" ASC);

CREATE INDEX "Cliente_INDEX4" ON "Cliente" ("CodigoIntegracao" ASC);

CREATE SEQUENCE "Cliente_SEQ";

CREATE TRIGGER "Cliente_SEQ_TR" 
BEFORE INSERT ON "Cliente" 
FOR EACH ROW 
BEGIN
  <<COLUMN_SEQUENCES>>
  BEGIN
    IF INSERTING AND :NEW."IdCliente" IS NULL THEN
      SELECT "Cliente_SEQ".NEXTVAL INTO :NEW."IdCliente" FROM SYS.DUAL;
    END IF;
  END COLUMN_SEQUENCES;
END;
/
--Representante

CREATE TABLE "Representante" 
(
  "IdRepresentante" NUMBER(19) NOT NULL 
, "CodigoIntegracao" NUMBER(10) NOT NULL 
, "Nome" VARCHAR2(180) NOT NULL 
, "Ativo" NUMBER NOT NULL 
, CONSTRAINT "Representante_PK" PRIMARY KEY 
  (
    "IdRepresentante" 
  ) 
  ENABLE 
);

CREATE UNIQUE INDEX "Representante_PK" ON "Representante" ("IdRepresentante" ASC);
CREATE INDEX "Representante_INDEX1" ON "Representante" ("CodigoIntegracao" ASC);
/

CREATE TABLE "GarantiaStatus" 
(
  "IdGarantiaStatus" NUMBER(10) NOT NULL 
, "Descricao" VARCHAR2(20) NOT NULL 
, CONSTRAINT "GarantiaStatus_PK" PRIMARY KEY 
  (
    "IdGarantiaStatus" 
  )
  ENABLE 
);
CREATE UNIQUE INDEX "GarantiaStatus_PK" ON "GarantiaStatus" ("IdGarantiaStatus" ASC, Descricao ASC);
/

CREATE TABLE "Garantia" 
(
  "IdGarantia" NUMBER(19) NOT NULL 
, "IdNotaFiscal" NUMBER(19) NOT NULL 
, "IdGarantiaStatus" NUMBER(10) NOT NULL 
, "IdUsuarioRecebimento" VARCHAR2(128) NOT NULL 
, "DataRecebimento" VARCHAR2(20) NOT NULL 
, "Observacao" VARCHAR2(500) 
, "InformacaoTransporte" VARCHAR2(100) NOT NULL 
, "DataIncioConferencia" TIMESTAMP NOT NULL 
, "DataFimConferencia" TIMESTAMP NOT NULL 
, CONSTRAINT "Garantia_PK" PRIMARY KEY 
  (
    "IdGarantia" 
  )
  USING INDEX 
  (
      CREATE UNIQUE INDEX "Garantia_PK" ON "Garantia" ("IdGarantia" ASC) 
  )
  ENABLE 
);

CREATE INDEX "Garantia_INDEX1" ON "Garantia" ("IdNotaFiscal");

CREATE INDEX "Garantia_INDEX2" ON "Garantia" ("IdGarantiaStatus");

CREATE INDEX "Garantia_INDEX3" ON "Garantia" ("IdUsuarioRecebimento");

CREATE SEQUENCE "Garantia_SEQ";

CREATE TRIGGER "Garantia_SEQ_TRG" 
BEFORE INSERT ON "Garantia" 
FOR EACH ROW 
BEGIN
  <<COLUMN_SEQUENCES>>
  BEGIN
    IF INSERTING AND :NEW."IdGarantia" IS NULL THEN
      SELECT "Garantia_SEQ".NEXTVAL INTO :NEW."IdGarantia" FROM SYS.DUAL;
    END IF;
  END COLUMN_SEQUENCES;
END;

ALTER TABLE DART."Garantia" ADD CONSTRAINT "Garantia_FK1" FOREIGN KEY ("IdNotaFiscal") REFERENCES DART."NotaFiscal"("IdNotaFiscal");
ALTER TABLE DART."Garantia" ADD CONSTRAINT "Garantia_FK2" FOREIGN KEY ("IdUsuarioRecebimento") REFERENCES DART."AspNetUsers"("Id");
ALTER TABLE DART."Garantia" ADD CONSTRAINT "Garantia_FK3" FOREIGN KEY ("IdGarantiaStatus") REFERENCES DART."GarantiaStatus"("IdGarantiaStatus");
/

--------------------------------------------------------
--  DDL for Table GarantiaConferenciaTipo
--------------------------------------------------------

  CREATE TABLE "DART"."GarantiaConferenciaTipo" 
   (	"IdGarantiaConferenciaTipo" NUMBER(19,0), 
	"Descricao" VARCHAR2(20 BYTE)
   ) SEGMENT CREATION DEFERRED 
  PCTFREE 10 PCTUSED 40 INITRANS 1 MAXTRANS 255 
 NOCOMPRESS LOGGING
  TABLESPACE "USERS" ;
--------------------------------------------------------
--  DDL for Index GarantiaConferenciaTipo_PK
--------------------------------------------------------

  CREATE UNIQUE INDEX "DART"."GarantiaConferenciaTipo_PK" ON "DART"."GarantiaConferenciaTipo" ("IdGarantiaConferenciaTipo") 
  PCTFREE 10 INITRANS 2 MAXTRANS 255 
  TABLESPACE "USERS" ;
--------------------------------------------------------
--  DDL for Index GarantiaConferenciaTipo_INDEX1
--------------------------------------------------------

  CREATE INDEX "DART"."GarantiaConferenciaTipo_INDEX1" ON "DART"."GarantiaConferenciaTipo" ("Descricao") 
  PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  TABLESPACE "USERS" ;
--------------------------------------------------------
--  Constraints for Table GarantiaConferenciaTipo
--------------------------------------------------------

  ALTER TABLE "DART"."GarantiaConferenciaTipo" ADD CONSTRAINT "GarantiaConferenciaTipo_PK" PRIMARY KEY ("IdGarantiaConferenciaTipo")
  USING INDEX PCTFREE 10 INITRANS 2 MAXTRANS 255 
  TABLESPACE "USERS"  ENABLE;
  ALTER TABLE "DART"."GarantiaConferenciaTipo" MODIFY ("IdGarantiaConferenciaTipo" NOT NULL ENABLE);
/

CREATE TABLE "GarantiaProduto" 
(
  "IdGarantiaProduto" NUMBER(19) NOT NULL 
, "IdGarantia" NUMBER(19) NOT NULL 
, "IdNotaFiscalItem" NUMBER(19) NOT NULL 
, "IdProduto" NUMBER(19) NOT NULL 
, "IdGarantiaConferenciaTipo" NUMBER(10) NOT NULL 
, "IdMotivoLaudo" NUMBER(19) 
, "IdUsuarioConferencia" VARCHAR2(128) NOT NULL 
, "Quantidade" NUMBER(10) NOT NULL 
, CONSTRAINT "GarantiaProduto_PK" PRIMARY KEY 
  (
    "IdGarantiaProduto" 
  )
  USING INDEX 
  (
      CREATE UNIQUE INDEX "GarantiaProduto_PK" ON "GarantiaProduto" ("IdGarantiaProduto" ASC) 
  )
  ENABLE 
);

CREATE SEQUENCE "GarantiaProduto_SEQ";

CREATE TRIGGER "GarantiaProduto_SEQ_TRG" 
BEFORE INSERT ON "GarantiaProduto" 
FOR EACH ROW 
BEGIN
  <<COLUMN_SEQUENCES>>
  BEGIN
    IF INSERTING AND :NEW."IdGarantiaProduto" IS NULL THEN
      SELECT "GarantiaProduto_SEQ".NEXTVAL INTO :NEW."IdGarantiaProduto" FROM SYS.DUAL;
    END IF;
  END COLUMN_SEQUENCES;
END;

ALTER TABLE DART."GarantiaProduto" ADD CONSTRAINT "GarantiaProduto_FK1" FOREIGN KEY ("IdGarantia") REFERENCES DART."Garantia"("IdGarantia");
ALTER TABLE DART."GarantiaProduto" ADD CONSTRAINT "GarantiaProduto_FK2" FOREIGN KEY ("IdNotaFiscalItem") REFERENCES DART."NotaFiscalItem"("IdNotaFiscalItem");
ALTER TABLE DART."GarantiaProduto" ADD CONSTRAINT "GarantiaProduto_FK3" FOREIGN KEY ("IdProduto") REFERENCES DART."Produto"("IdProduto");
ALTER TABLE DART."GarantiaProduto" ADD CONSTRAINT "GarantiaProduto_FK4" FOREIGN KEY ("IdMotivoLaudo") REFERENCES DART."MotivoLaudo"("IdMotivoLaudo");
ALTER TABLE DART."GarantiaProduto" ADD CONSTRAINT "GarantiaProduto_FK5" FOREIGN KEY ("IdUsuarioConferencia") REFERENCES DART."AspNetUsers"("Id");
ALTER TABLE DART."GarantiaProduto" ADD CONSTRAINT "GarantiaProduto_FK6" FOREIGN KEY ("IdGarantiaConferenciaTipo") REFERENCES DART."GarantiaConferenciaTipo"("IdGarantiaConferenciaTipo");

CREATE INDEX "GarantiaPoduto_INDEX1" ON DART."GarantiaProduto" ("IdGarantia");
CREATE INDEX "GarantiaPoduto_INDEX2" ON DART."GarantiaProduto" ("IdNotaFiscalItem");
CREATE INDEX "GarantiaPoduto_INDEX3" ON DART."GarantiaProduto" ("IdProduto");
CREATE INDEX "GarantiaPoduto_INDEX4" ON DART."GarantiaProduto" ("IdGarantiaConferenciaTipo");
CREATE INDEX "GarantiaPoduto_INDEX5" ON DART."GarantiaProduto" ("IdMotivoLaudo");
CREATE INDEX "GarantiaPoduto_INDEX6" ON DART."GarantiaProduto" ("IdUsuarioConferencia");

/

CREATE TABLE "GarantiaQuarentenaStatus" 
(
  "IdGarantiaQuarentenaStatus" NUMBER(19) NOT NULL 
, "Descricao" VARCHAR2(20) 
, CONSTRAINT "GarantiaQuarentenaStatus_PK" PRIMARY KEY 
  (
    "IdGarantiaQuarentenaStatus" 
  )
  USING INDEX 
  (
      CREATE UNIQUE INDEX "GarantiaQuarentenaStatus_PK" ON "GarantiaQuarentenaStatus" ("IdGarantiaQuarentenaStatus" ASC) 
  )
  ENABLE 
);

CREATE INDEX "GarantiaQuarentenaStatus_INDE" ON "GarantiaQuarentenaStatus" ("Descricao");
/

CREATE TABLE "GarantiaQuarentena" 
(
  "IdGarantiaQuarentena" NUMBER NOT NULL 
, "IdGarantia" NUMBER NOT NULL 
, "IdGarantiaQuarentenaStatus" NUMBER NOT NULL 
, "DataCriacao" TIMESTAMP NOT NULL 
, "DataEncerramento" TIMESTAMP 
, "Observacao" VARCHAR2(500) 
, "CodigoConfirmacao" VARCHAR2(20) 
, CONSTRAINT "GarantiaQuarentena_PK" PRIMARY KEY 
  (
    "IdGarantiaQuarentena" 
  )
  USING INDEX 
  (
      CREATE UNIQUE INDEX "GarantiaQuarentena_PK" ON "GarantiaQuarentena" ("IdGarantiaQuarentena" ASC) 
  )
  ENABLE 
);

CREATE INDEX "GarantiaQuarentena_INDEX1" ON "GarantiaQuarentena" ("IdGarantia");

CREATE INDEX "GarantiaQuarentena_INDEX2" ON "GarantiaQuarentena" ("IdGarantiaQuarentenaStatus");

CREATE SEQUENCE "GarantiaQuarentena_SEQ";

CREATE TRIGGER "GarantiaQuarentena_SEQ_TRG" 
BEFORE INSERT ON "GarantiaQuarentena" 
FOR EACH ROW 
BEGIN
  <<COLUMN_SEQUENCES>>
  BEGIN
    IF INSERTING AND :NEW."IdGarantiaQuarentena" IS NULL THEN
      SELECT "GarantiaQuarentena_SEQ".NEXTVAL INTO :NEW."IdGarantiaQuarentena" FROM SYS.DUAL;
    END IF;
  END COLUMN_SEQUENCES;
END;

ALTER TABLE DART."GarantiaQuarentena" ADD CONSTRAINT "GarantiaQuarentena_FK1" FOREIGN KEY ("IdGarantia") REFERENCES DART."Garantia"("IdGarantia");
ALTER TABLE DART."GarantiaQuarentena" ADD CONSTRAINT "GarantiaQuarentena_FK2" FOREIGN KEY ("IdGarantiaQuarentenaStatus") REFERENCES DART."GarantiaQuarentenaStatus"("IdGarantiaQuarentenaStatus");

/

CREATE TABLE "GarantiaQuarentenaHis" 
(
  "IdGarantiaQuarentenaHis" NUMBER(19) NOT NULL 
, "IdGarantiaQuarentena" NUMBER(19) NOT NULL 
, "IdUsuario" VARCHAR2(127) NOT NULL 
, "Data" TIMESTAMP NOT NULL 
, "Descricao" VARCHAR2(500) NOT NULL 
, CONSTRAINT "GarantiaQuarentenaHis_PK" PRIMARY KEY 
  (
    "IdGarantiaQuarentenaHis" 
  )
  USING INDEX 
  (
      CREATE UNIQUE INDEX "GarantiaQuarentenaHis_PK" ON "GarantiaQuarentenaHis" ("IdGarantiaQuarentenaHis" ASC ) 
  )
  ENABLE 
);

CREATE INDEX "GarantiaQuarentenaHis_INDEX1" ON "GarantiaQuarentenaHis" ("IdGarantiaQuarentena");
CREATE INDEX "GarantiaQuarentenaHis_INDEX2" ON "GarantiaQuarentenaHis" ("IdUsuario");

CREATE SEQUENCE "GarantiaQuarentenaHis_SEQ";

CREATE TRIGGER "GarantiaQuarentenaHis_SEG_TRG" 
BEFORE INSERT ON "GarantiaQuarentenaHis" 
FOR EACH ROW 
BEGIN
  <<COLUMN_SEQUENCES>>
  BEGIN
    IF INSERTING AND :NEW."IdGarantiaQuarentenaHis" IS NULL THEN
      SELECT "GarantiaQuarentenaHis_SEQ".NEXTVAL INTO :NEW."IdGarantiaQuarentenaHis" FROM SYS.DUAL;
    END IF;
  END COLUMN_SEQUENCES;
END;
/

ALTER TABLE DART."Cliente" MODIFY "TipoCliente" CHAR(1);
ALTER TABLE DART."Cliente" RENAME COLUMN CNPJ TO CNPJCPF;
ALTER TABLE DART."NotaFiscal" ADD "IdCliente" NUMBER(19,0);
ALTER TABLE DART."NotaFiscal" MODIFY "IdFornecedor" NUMBER(19,0) NULL;
ALTER TABLE DART."NotaFiscal" ADD "BaseICMS" FLOAT;
ALTER TABLE DART."NotaFiscal" ADD "ValorICMS" FLOAT;
ALTER TABLE DART."NotaFiscal" ADD "BaseST" FLOAT;
ALTER TABLE DART."NotaFiscal" ADD "ValorST" FLOAT;
ALTER TABLE DART."NotaFiscal" ADD "ValorIPI" FLOAT;
ALTER TABLE DART."NotaFiscal" ADD "ValorSeguro" FLOAT;
ALTER TABLE DART."NotaFiscalItem" ADD "CodigoIntegracaoNFOrigem" NUMBER(10,0);
ALTER TABLE DART."NotaFiscalItem" ADD "SequenciaNFOrigem" VARCHAR2(22);
ALTER TABLE DART."NotaFiscalItem" ADD CFOP VARCHAR2(22);
ALTER TABLE DART."NotaFiscalItem" ADD "CodigoBarras" VARCHAR2(100);


INSERT INTO DART."NotaFiscalTipo" ("IdNotaFiscalTipo","Descricao") 	VALUES (3,'Garantia');


/