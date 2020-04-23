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

INSERT INTO DART."PedidoVendaProdutoStatus" ("IdPedidoVendaProdutoStatus","Descricao") VALUES (0,'Processando Integracao');
INSERT INTO DART."PedidoVendaProdutoStatus" ("IdPedidoVendaProdutoStatus","Descricao") VALUES (1,'Aguardando Separacao');