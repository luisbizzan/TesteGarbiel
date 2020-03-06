--Adicionar a tabela sem definir como not null
ALTER  TABLE "NotaFiscalItem"  "QuantidadeDevolucao" NUMBER(10,0) NOT NULL ;

--Adicionando o valor 0 no campo de quantidade de devolução
UPDATE "NotaFiscalItem"  SET "QuantidadeDevolucao" = 0;

--Colocando o campo de quantidade devolução como NOT NULL
ALTER  TABLE DART."NotaFiscalItem" MODIFY "QuantidadeDevolucao" NUMBER(10,0) NOT NULL;