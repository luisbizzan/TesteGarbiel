ALTER TABLE "LoteProdutoEndereco" ADD "AguardandoConferencia" NUMBER(1,0);

UPDATE "LoteProdutoEndereco" SET "AguardandoConferencia" = 0;

ALTER  TABLE "LoteProdutoEndereco" MODIFY "AguardandoConferencia" NUMBER(1,0) NOT NULL;

COMMIT;