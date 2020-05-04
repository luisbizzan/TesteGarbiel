CREATE TABLE "PedidoVenda" 
(
  "IdPedidoVenda" NUMBER(19) NOT NULL 
, "IdPedido" NUMBER(19,0) NOT NULL
, "NroPedidoVenda" NUMBER(10) NOT NULL 
, "IdEmpresa" NUMBER(19) NOT NULL 
, "IdCliente" NUMBER(19) NOT NULL 
, "IdTransportadora" NUMBER(19) NOT NULL 
, "NroVolumes" NUMBER(10) NOT NULL 
, "DataHoraInicioSeparacao" DATE 
, "DataHoraFimSeparacao" DATE 
, "IdPedidoVendaStatus" NUMBER(10) NOT NULL
, "IdRepresentante" NUMBER NOT NULL
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

CREATE INDEX "PedidoVenda_INDEX1" ON DART."PedidoVenda" ("IdEmpresa");
CREATE INDEX "PedidoVenda_INDEX2" ON DART."PedidoVenda" ("IdCliente");
CREATE INDEX "PedidoVenda_INDEX3" ON DART."PedidoVenda" ("IdTransportadora");
CREATE INDEX "PedidoVenda_INDEX4" ON DART."PedidoVenda" ("IdPedidoVendaStatus");
CREATE INDEX "PedidoVenda_INDEX5" ON DART."PedidoVenda" ("IdRepresentante");
CREATE INDEX "PedidoVenda_INDEX6" ON DART."PedidoVenda" ("IdPedido");

ALTER TABLE DART."PedidoVenda" ADD CONSTRAINT "PedidoVenda_FK1" FOREIGN KEY ("IdCliente") REFERENCES DART."Cliente"("IdCliente");
ALTER TABLE DART."PedidoVenda" ADD CONSTRAINT "PedidoVenda_FK2" FOREIGN KEY ("IdEmpresa") REFERENCES DART."Empresa"("IdEmpresa");
ALTER TABLE DART."PedidoVenda" ADD CONSTRAINT "PedidoVenda_FK3" FOREIGN KEY ("IdTransportadora") REFERENCES DART."Transportadora"("IdTransportadora");
ALTER TABLE DART."PedidoVenda" ADD CONSTRAINT "PedidoVenda_FK4" FOREIGN KEY ("IdPedidoVendaStatus") REFERENCES DART."PedidoVendaStatus"("IdPedidoVendaStatus");
ALTER TABLE DART."PedidoVenda" ADD CONSTRAINT "PedidoVenda_FK5" FOREIGN KEY ("IdRepresentante") REFERENCES DART."Representante"("IdRepresentante");
ALTER TABLE DART."PedidoVenda" ADD CONSTRAINT "PedidoVenda_FK6" FOREIGN KEY ("IdPedido") REFERENCES DART."Pedido"("IdPedido");





