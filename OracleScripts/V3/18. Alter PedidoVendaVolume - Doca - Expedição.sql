ALTER TABLE "PedidoVendaVolume" ADD "IdUsuarioInstalacaoDOCA" VARCHAR2(128) NULL;
ALTER TABLE "PedidoVendaVolume" ADD "DataHoraInstalacaoDOCA" DATE NULL;

CREATE INDEX "PedidoVendaVolume_INDEX9" ON DART."PedidoVendaVolume" ("IdUsuarioInstalacaoDOCA");

ALTER TABLE DART."PedidoVendaVolume" ADD CONSTRAINT "PedidoVendaVolume_FK9" FOREIGN KEY ("IdUsuarioInstalacaoDOCA") REFERENCES DART."AspNetUsers"("Id");