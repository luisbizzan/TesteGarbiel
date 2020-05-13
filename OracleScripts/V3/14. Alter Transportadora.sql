ALTER TABLE DART."Transportadora" ADD "CodigoTransportadora" VARCHAR2(3) NULL;
UPDATE "Transportadora" SET "CodigoTransportadora" = 'ABC';
ALTER TABLE DART."Transportadora" MODIFY "CodigoTransportadora" VARCHAR2(3) NOT NULL;

COMMIT;
