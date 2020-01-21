ALTER TABLE DART."NotaFiscalItem" ADD "Sequencia" NUMBER(10,0);

UPDATE DART."NotaFiscalItem" SET "Sequencia" = 1 ;

ALTER TABLE DART."NotaFiscalItem" MODIFY "Sequencia" NUMBER(10,0) NOT NULL;

COMMIT;