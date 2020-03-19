ALTER TABLE "LoteMovimentacaoTipo" DISABLE ALL TRIGGERS;

INSERT INTO "LoteMovimentacaoTipo" ("IdLoteMovimentacaoTipo", "Descricao") VALUES (4, 'Abastecimento');

ALTER TABLE "LoteMovimentacaoTipo" ENABLE ALL TRIGGERS;

COMMIT;

select * from "LoteMovimentacaoTipo";