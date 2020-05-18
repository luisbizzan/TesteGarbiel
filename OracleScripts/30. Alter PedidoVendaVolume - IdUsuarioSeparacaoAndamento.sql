ALTER TABLE "PedidoVendaVolume" ADD "IdUsuarioSeparacaoAndamento" VARCHAR2(128) NULL;

CREATE INDEX "PedidoVendaVolume_INDEX11" ON DART."PedidoVendaVolume" ("IdUsuarioSeparacaoAndamento");

ALTER TABLE DART."PedidoVendaVolume" ADD CONSTRAINT "PedidoVendaVolume_FK11" FOREIGN KEY ("IdUsuarioSeparacaoAndamento") REFERENCES DART."AspNetUsers"("Id");