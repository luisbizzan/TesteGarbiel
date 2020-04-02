ALTER  TABLE "EnderecoArmazenagem" ADD "IsEntrada" NUMBER(1,0);

UPDATE "EnderecoArmazenagem"  SET "IsEntrada" = 0;

ALTER TABLE "EnderecoArmazenagem" MODIFY "IsEntrada" NUMBER(1,0) NOT NULL;

COMMIT;