ALTER TABLE "EnderecoArmazenagem" ADD "IsPicking" NUMBER(1,0) NULL;
UPDATE "EnderecoArmazenagem" SET "IsPicking" = 0;
ALTER TABLE "EnderecoArmazenagem" MODIFY "IsPicking" NUMBER(1,0) NOT NULL;