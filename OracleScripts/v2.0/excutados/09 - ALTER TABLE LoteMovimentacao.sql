
ALTER TABLE "LoteMovimentacao" ADD ("IdEmpresa" NUMBER(19) NOT NULL);

ALTER TABLE "LoteMovimentacao" RENAME CONSTRAINT SYS_C0011519 TO "LoteMovimentacao_PK";

ALTER TABLE "LoteMovimentacao" RENAME CONSTRAINT SYS_C0011527 TO "LoteMovimentacao_FK1";

ALTER TABLE "LoteMovimentacao" RENAME CONSTRAINT SYS_C0011528 TO "LoteMovimentacao_FK2";

ALTER TABLE "LoteMovimentacao" RENAME CONSTRAINT SYS_C0011529 TO "LoteMovimentacao_FK3";

ALTER TABLE "LoteMovimentacao" RENAME CONSTRAINT SYS_C0011530 TO "LoteMovimentacao_FK4";

ALTER TABLE "LoteMovimentacao" RENAME CONSTRAINT SYS_C0011531 TO "LoteMovimentacao_FK5";