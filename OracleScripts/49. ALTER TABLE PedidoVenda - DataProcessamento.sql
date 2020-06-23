ALTER TABLE DART."PedidoVenda" ADD "DataProcessamento" DATE NULL;

UPDATE "PedidoVenda" SET "DataProcessamento" = CURRENT_DATE;

ALTER TABLE DART."PedidoVenda" MODIFY "DataProcessamento" DATE NOT NULL;