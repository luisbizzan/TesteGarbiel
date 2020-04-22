CREATE TABLE "PedidoVenda" 
(
  "IdPedidoVenda" NUMBER(19) NOT NULL 
, "NroPedidoVenda" NUMBER(10) NOT NULL 
, "IdEmpresa" NUMBER(19) NOT NULL 
, "IdCliente" NUMBER(19) NOT NULL 
, "IdTransportadora" NUMBER(19) NOT NULL 
, "NroVolumes" NUMBER(10) 
, "IdUsuarioSeparacao" VARCHAR2(128) 
, "DataHoraInicioSeparacao" DATE 
, "DataHoraFimSeparacao" DATE 
, "IdPedidoVendaStatus" NUMBER(10) NOT NULL 
, CONSTRAINT "PedidoVenda_PK" PRIMARY KEY 
  (
    "IdPedidoVenda" 
  )
  USING INDEX 
  (
      CREATE UNIQUE INDEX "PedidoVenda_PK" ON "PedidoVenda" ("IdPedidoVenda" ASC) 
  )
  ENABLE 
);

CREATE SEQUENCE "PedidoVenda_SEQ";

CREATE TRIGGER "PedidoVenda_SEQ_TRG" 
BEFORE INSERT ON "PedidoVenda" 
FOR EACH ROW 
BEGIN
  <<COLUMN_SEQUENCES>>
  BEGIN
    IF INSERTING AND :NEW."IdPedidoVenda" IS NULL THEN
      SELECT "PedidoVenda_SEQ".NEXTVAL INTO :NEW."IdPedidoVenda" FROM SYS.DUAL;
    END IF;
  END COLUMN_SEQUENCES;
END;
/

CREATE TABLE "PedidoVendaStatus" 
(
  "IdPedidoVendaStatus" NUMBER(19) NOT NULL 
, "Descricao" VARCHAR2(50) NOT NULL 
, CONSTRAINT "PedidoVendaStatus_PK" PRIMARY KEY 
  (
    "IdPedidoVendaStatus" 
  )
  USING INDEX 
  (
      CREATE UNIQUE INDEX "PedidoVendaStatus_PK" ON "PedidoVendaStatus" ("IdPedidoVendaStatus" ASC) 
  )
  ENABLE 
);


CREATE INDEX "PedidoVenda_INDEX1" ON DART."PedidoVenda" ("IdEmpresa");
CREATE INDEX "PedidoVenda_INDEX2" ON DART."PedidoVenda" ("IdCliente");
CREATE INDEX "PedidoVenda_INDEX3" ON DART."PedidoVenda" ("IdTransportadora");
CREATE INDEX "PedidoVenda_INDEX4" ON DART."PedidoVenda" ("IdPedidoVendaStatus");
CREATE INDEX "PedidoVenda_INDEX5" ON DART."PedidoVenda" ("IdUsuarioSeparacao");

CREATE UNIQUE INDEX "PedidoVendaStatus_INDEX" ON DART."PedidoVendaStatus" ("IdPedidoVendaStatus","Descricao");

ALTER TABLE DART."PedidoVenda" ADD CONSTRAINT "PedidoVenda_FK1" FOREIGN KEY ("IdCliente") REFERENCES DART."Cliente"("IdCliente");
ALTER TABLE DART."PedidoVenda" ADD CONSTRAINT "PedidoVenda_FK2" FOREIGN KEY ("IdEmpresa") REFERENCES DART."Empresa"("IdEmpresa");
ALTER TABLE DART."PedidoVenda" ADD CONSTRAINT "PedidoVenda_FK3" FOREIGN KEY ("IdTransportadora") REFERENCES DART."Transportadora"("IdTransportadora");
ALTER TABLE DART."PedidoVenda" ADD CONSTRAINT "PedidoVenda_FK4" FOREIGN KEY ("IdUsuarioSeparacao") REFERENCES DART."AspNetUsers"("Id");
ALTER TABLE DART."PedidoVenda" ADD CONSTRAINT "PedidoVenda_FK5" FOREIGN KEY ("IdPedidoVendaStatus") REFERENCES DART."PedidoVendaStatus"("IdPedidoVendaStatus");

CREATE TABLE "PedidoVendaProduto" 
(
  "IdPedidoVendaProduto" NUMBER(19) NOT NULL 
, "IdPedidoVenda" NUMBER(19) NOT NULL 
, "IdProduto" NUMBER(19) NOT NULL 
, "QtdPedido" NUMBER(10) NOT NULL 
, "Sequence" NUMBER(10) NOT NULL 
, CONSTRAINT "PedidoVendaProduto_PK" PRIMARY KEY 
  (
    "IdPedidoVendaProduto" 
  )
  USING INDEX 
  (
      CREATE UNIQUE INDEX "PedidoVendaProduto_PK" ON "PedidoVendaProduto" ("IdPedidoVendaProduto" ASC) 
  )
  ENABLE 
);

CREATE SEQUENCE "PedidoVendaProduto_SEQ";

CREATE TRIGGER "PedidoVendaProduto_SEQ_TRG" 
BEFORE INSERT ON "PedidoVendaProduto" 
FOR EACH ROW 
BEGIN
  <<COLUMN_SEQUENCES>>
  BEGIN
    IF INSERTING AND :NEW."Sequence" IS NULL THEN
      SELECT "PedidoVendaProduto_SEQ".NEXTVAL INTO :NEW."Sequence" FROM SYS.DUAL;
    END IF;
  END COLUMN_SEQUENCES;
END;
/

ALTER TABLE DART."PedidoVenda" ADD "CodigoIntegracao" NUMBER NOT NULL;
ALTER TABLE DART."PedidoVenda" MODIFY "CodigoIntegracao" NUMBER(10,0);

CREATE TABLE "PedidoVendaProdutoStatus" 
(
  "IdPedidoVendaProdutoStatus" NUMBER(19) NOT NULL 
, "Descricao" VARCHAR2(50) NOT NULL 
, CONSTRAINT "PedidoVendaProdutoStatus_PK" PRIMARY KEY 
  (
    "IdPedidoVendaProdutoStatus" 
  )
  USING INDEX 
  (
      CREATE UNIQUE INDEX "PedidoVendaProdutoStatus_PK" ON "PedidoVendaProdutoStatus" ("IdPedidoVendaProdutoStatus" ASC) 
  )
  ENABLE 
);

CREATE UNIQUE INDEX "PedidoVendaProdutoStatus_INDEX" ON DART."PedidoVendaProdutoStatus" ("IdPedidoVendaProdutoStatus","Descricao");

CREATE INDEX "PedidoVendaProduto_INDEX1" ON DART."PedidoVendaProduto" ("IdPedidoVenda");
CREATE INDEX "PedidoVendaProduto_INDEX2" ON DART."PedidoVendaProduto" ("IdProduto");

ALTER TABLE DART."PedidoVendaProduto" ADD CONSTRAINT "PedidoVendaProduto_FK1" FOREIGN KEY ("IdPedidoVenda") REFERENCES DART."PedidoVenda"("IdPedidoVenda");
ALTER TABLE DART."PedidoVendaProduto" ADD CONSTRAINT "PedidoVendaProduto_FK2" FOREIGN KEY ("IdProduto") REFERENCES DART."Produto"("IdProduto");

ALTER TABLE DART."PedidoVendaProduto" RENAME COLUMN "Sequence" TO "Sequencia";

ALTER TABLE DART."PedidoVendaProduto" ADD "IdPedidoVendaProdutoStatus" NUMBER(10,0) NOT NULL;

ALTER TABLE DART."PedidoVendaProduto" ADD CONSTRAINT "PedidoVendaProdutoStatus_FK3" FOREIGN KEY ("IdPedidoVendaProdutoStatus") REFERENCES DART."PedidoVendaProdutoStatus"("IdPedidoVendaProdutoStatus");

ALTER TABLE DART."PedidoVenda" ADD "DataCriacao" DATE NOT NULL;







