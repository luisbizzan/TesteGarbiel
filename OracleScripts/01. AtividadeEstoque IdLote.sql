ALTER TABLE "AtividadeEstoque" ADD "IdLote" NUMBER(19,0) NULL;

CREATE INDEX "AtividadeEstoque_INDEX_1" ON DART."AtividadeEstoque" ("IdLote");

ALTER TABLE DART."AtividadeEstoque" ADD CONSTRAINT "AtividadeEstoque_FK7" FOREIGN KEY ("IdLote") REFERENCES DART."Lote"("IdLote");

-- Verificar antes de rodar esse trecho:
-- DELETE FROM "AtividadeEstoque" WHERE "IdAtividadeEstoqueTipo" = 1 AND "Finalizado" = 0 AND "IdLote" IS NULL;