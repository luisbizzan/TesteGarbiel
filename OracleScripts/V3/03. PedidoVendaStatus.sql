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

CREATE UNIQUE INDEX "PedidoVendaStatus_INDEX" ON DART."PedidoVendaStatus" ("IdPedidoVendaStatus","Descricao");

INSERT INTO DART."PedidoVendaStatus" ("IdPedidoVendaStatus","Descricao") VALUES (0,'Processando Integracao');
INSERT INTO DART."PedidoVendaStatus" ("IdPedidoVendaStatus","Descricao") VALUES (1,'Aguardando Separacao');