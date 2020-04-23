CREATE TABLE "Pedido" 
(
  "IdPedido" NUMBER(19) NOT NULL 
, "NroPedido" NUMBER(10) NOT NULL 
, "IdEmpresa" NUMBER(19) NOT NULL 
, "IdCliente" NUMBER(19) NOT NULL 
, "IdTransportadora" NUMBER(19) NOT NULL 
, "CodigoIntegracao" NUMBER(10,0) NOT NULL
, "IdRepresentante" NUMBER(19) NOT NULL
, "DataCriacao" DATE NOT NULL
, "IdPedidoVendaStatus" NUMBER(10) NOT NULL 
, CONSTRAINT "Pedido_PK" PRIMARY KEY 
  (
    "IdPedido" 
  )
  USING INDEX 
  (
      CREATE UNIQUE INDEX "Pedido_PK" ON "Pedido" ("IdPedido" ASC) 
  )
  ENABLE 
);

CREATE SEQUENCE "Pedido_SEQ";

CREATE TRIGGER "Pedido_SEQ_TRG" 
BEFORE INSERT ON "Pedido" 
FOR EACH ROW 
BEGIN
  <<COLUMN_SEQUENCES>>
  BEGIN
    IF INSERTING AND :NEW."IdPedido" IS NULL THEN
      SELECT "Pedido_SEQ".NEXTVAL INTO :NEW."IdPedido" FROM SYS.DUAL;
    END IF;
  END COLUMN_SEQUENCES;
END;
/

CREATE INDEX "Pedido_INDEX1" ON DART."Pedido" ("IdEmpresa");
CREATE INDEX "Pedido_INDEX2" ON DART."Pedido" ("IdCliente");
CREATE INDEX "Pedido_INDEX3" ON DART."Pedido" ("IdTransportadora");
CREATE INDEX "Pedido_INDEX4" ON DART."Pedido" ("IdPedidoVendaStatus");
CREATE INDEX "Pedido_INDEX5" ON DART."Pedido" ("IdUsuarioSeparacao");
CREATE INDEX "Pedido_INDEX6" ON DART."Pedido" ("IdRepresentante");

ALTER TABLE DART."Pedido" ADD CONSTRAINT "Pedido_FK1" FOREIGN KEY ("IdCliente") REFERENCES DART."Cliente"("IdCliente");
ALTER TABLE DART."Pedido" ADD CONSTRAINT "Pedido_FK2" FOREIGN KEY ("IdEmpresa") REFERENCES DART."Empresa"("IdEmpresa");
ALTER TABLE DART."Pedido" ADD CONSTRAINT "Pedido_FK3" FOREIGN KEY ("IdTransportadora") REFERENCES DART."Transportadora"("IdTransportadora");
ALTER TABLE DART."Pedido" ADD CONSTRAINT "Pedido_FK4" FOREIGN KEY ("IdUsuarioSeparacao") REFERENCES DART."AspNetUsers"("Id");
ALTER TABLE DART."Pedido" ADD CONSTRAINT "Pedido_FK5" FOREIGN KEY ("IdPedidoVendaStatus") REFERENCES DART."PedidoVendaStatus"("IdPedidoVendaStatus");
ALTER TABLE DART."Pedido" ADD CONSTRAINT "Pedido_FK6" FOREIGN KEY ("IdRepresentante") REFERENCES DART."Representante"("IdRepresentante");

ALTER TABLE DART."PedidoVenda" ADD "IdPedido" NUMBER(19,0);
ALTER TABLE DART."PedidoVenda" MODIFY "IdPedido" NUMBER(19,0) NOT NULL;
ALTER TABLE DART."PedidoVenda" ADD CONSTRAINT "PedidoVenda_FK7" FOREIGN KEY ("IdPedido") REFERENCES DART."Pedido"("IdPedido");
CREATE INDEX "PedidoVenda_INDEX7" ON DART."PedidoVenda" ("IdPedido");


	












