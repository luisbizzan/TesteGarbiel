ALTER TABLE "PedidoVendaVolume" ADD "IdUsuarioInstalTransportadora" VARCHAR2(128) NULL;
ALTER TABLE "PedidoVendaVolume" ADD "DataHoraInstalTransportadora" DATE NULL;
ALTER TABLE "PedidoVendaVolume" ADD "IdEnderecoArmazTransportadora" NUMBER(19,0) NULL;

CREATE INDEX "PedidoVendaVolume_INDEX7" ON DART."PedidoVendaVolume" ("IdUsuarioInstalTransportadora");
CREATE INDEX "PedidoVendaVolume_INDEX8" ON DART."PedidoVendaVolume" ("IdEnderecoArmazTransportadora");

ALTER TABLE DART."PedidoVendaVolume" ADD CONSTRAINT "PedidoVendaVolume_FK7" FOREIGN KEY ("IdUsuarioInstalTransportadora") REFERENCES DART."AspNetUsers"("Id");
ALTER TABLE DART."PedidoVendaVolume" ADD CONSTRAINT "PedidoVendaVolume_FK8" FOREIGN KEY ("IdEnderecoArmazTransportadora") REFERENCES DART."EnderecoArmazenagem"("IdEnderecoArmazenagem");