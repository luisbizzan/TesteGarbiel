ALTER TABLE DART."Pedido" ADD "DataIntegracao" DATE NULL;

UPDATE "Pedido" SET "DataIntegracao" = CURRENT_DATE;

ALTER TABLE DART."Pedido" MODIFY "DataIntegracao" DATE NOT NULL;