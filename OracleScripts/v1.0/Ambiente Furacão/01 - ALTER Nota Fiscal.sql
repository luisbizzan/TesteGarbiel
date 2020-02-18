UPDATE "NotaFiscal" SET "NFDevolucaoConfirmada" = 0;
ALTER TABLE DART."NotaFiscal" MODIFY "NFDevolucaoConfirmada" NUMBER(1,0) DEFAULT 0 NOT NULL;

COMMIT;