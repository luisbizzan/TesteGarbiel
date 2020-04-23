CREATE TABLE "PedidoStatus" 
(
  "IdPedidoStatus" NUMBER(19) NOT NULL 
, "Descricao" VARCHAR2(50) NOT NULL 
, CONSTRAINT "PedidoStatus_PK" PRIMARY KEY 
  (
    "IdPedidoStatus" 
  )
  USING INDEX 
  (
      CREATE UNIQUE INDEX "PedidoStatus_PK" ON "PedidoStatus" ("IdPedidoStatus" ASC) 
  )
  ENABLE 
);

CREATE UNIQUE INDEX "PedidoStatus_INDEX" ON DART."PedidoStatus" ("IdPedidoStatus","Descricao");

INSERT INTO DART."PedidoStatus" ("IdPedidoStatus","Descricao") VALUES (0,'Processando Integracao');
INSERT INTO DART."PedidoStatus" ("IdPedidoStatus","Descricao") VALUES (1,'Integrado');
INSERT INTO DART."PedidoStatus" ("IdPedidoStatus","Descricao") VALUES (2,'Confirmado');
INSERT INTO DART."PedidoStatus" ("IdPedidoStatus","Descricao") VALUES (3,'Cancelado');