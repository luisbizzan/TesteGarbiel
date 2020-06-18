ALTER TABLE DART."Pedido" ADD "NumeroPedido" VARCHAR2(100) NULL;

UPDATE "Pedido" SET "NumeroPedido" = "NroPedido";

SELECT "IdPedido", "NroPedido", "NumeroPedido" FROM "Pedido";

ALTER TABLE DART."Pedido" MODIFY "NumeroPedido" VARCHAR2(100) NOT NULL;

DROP INDEX DART."Pedido_INDEX7";

CREATE UNIQUE INDEX "Pedido_INDEX7" ON DART."Pedido" ("IdEmpresa", "NumeroPedido");

CREATE INDEX "Pedido_INDEX10" ON DART."Pedido" ("NumeroPedido");

-- ******* Rodar após migração de dados: *******
-- ALTER TABLE "Pedido" DROP COLUMN "NroPedido";
-- ALTER TABLE "PedidoVenda" DROP COLUMN "NroPedidoVenda";