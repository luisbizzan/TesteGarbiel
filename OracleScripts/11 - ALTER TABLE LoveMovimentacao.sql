ALTER TABLE "LoteMovimentacao" DROP CONSTRAINT "LoteMovimentacao_FK5";

ALTER TABLE "LoteMovimentacao"
ADD CONSTRAINT "LoteMovimentacao_FK5" FOREIGN KEY
(
  "IdLoteMovimentacaoTipo" 
)
REFERENCES "LoteMovimentacaoTipo"
(
  "IdLoteMovimentacaoTipo" 
)
ENABLE;

CREATE INDEX "LoteMovimentacao_INDEX4" ON "LoteMovimentacao" ("IdEmpresa" ASC);

alter table "LoteMovimentacao" add constraint "LoteMovimentacao_FK6" foreign key("IdEmpresa") references "Empresa"("IdEmpresa");


