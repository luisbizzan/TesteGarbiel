
ALTER INDEX SYS_C0011518 RENAME TO "LoteMovimentacaoTipo_INDEX1";

ALTER TABLE "LoteMovimentacaoTipo" RENAME CONSTRAINT SYS_C0011518 TO "LoteMovimentacaoTipo_PK";