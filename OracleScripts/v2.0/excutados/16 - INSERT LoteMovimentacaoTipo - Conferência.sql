ALTER TABLE "LoteMovimentacaoTipo" DISABLE ALL TRIGGERS;
INSERT INTO "LoteMovimentacaoTipo" ("IdLoteMovimentacaoTipo", "Descricao") VALUES (5, 'Confer�ncia');
ALTER TABLE "LoteMovimentacaoTipo" ENABLE ALL TRIGGERS;
COMMIT;

SELECT * FROM "LoteMovimentacaoTipo";

-- Remover TRIGGER e SEQUENCE