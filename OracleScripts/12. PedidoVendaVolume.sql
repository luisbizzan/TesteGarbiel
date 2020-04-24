CREATE TABLE "PedidoVendaVolume" 
(
  "IdPedidoVendaVolume" NUMBER(19) NOT NULL 
, "IdPedidoVenda" NUMBER(19) NOT NULL 
, "NroVolume" NUMBER(10) NOT NULL
, "NroCentena" NUMBER(10) NOT NULL
, "EtiquetaVolume" VARCHAR2(100) NOT NULL
, "IdCaixaCubagem" NUMBER(19) NOT NULL
, "CubagemVolume" FLOAT NOT NULL
, "PesoVolumeKG" FLOAT NOT NULL
, "IdGrupoCorredorArmazenagem" NUMBER (19) NOT NULL
, "CorredorInicio" NUMBER (10) NOT NULL
, "CorredorFim" NUMBER (10) NOT NULL
, "IdPerfilImpressora" NUMBER (19) NOT NULL
, "IdCaixaVolume"  NUMBER (19) NULL
, "DataHoraInicioSeparacao" DATE
, "DataHoraFimSeparacao" DATE
, "IdPedidoVendaStatus" NUMBER (19)
, "IdUsuarioSeparacao" VARCHAR (128)

, CONSTRAINT "PedidoVendaVolume_PK" PRIMARY KEY 
  (
    "IdPedidoVendaVolume" 
  )
  USING INDEX 
  (
      CREATE UNIQUE INDEX "PedidoVendaVolume_PK" ON "PedidoVendaVolume" ("IdPedidoVendaVolume" ASC) 
  )
  ENABLE 
);

CREATE SEQUENCE "PedidoVendaVolume_SEQ";

CREATE TRIGGER "PedidoVendaVolume_SEQ_TRG" 
BEFORE INSERT ON "PedidoVendaVolume" 
FOR EACH ROW 
BEGIN
  <<COLUMN_SEQUENCES>>
  BEGIN
    IF INSERTING AND :NEW."IdPedidoVendaVolume" IS NULL THEN
      SELECT "PedidoVendaVolume_SEQ".NEXTVAL INTO :NEW."IdPedidoVendaVolume" FROM SYS.DUAL;
    END IF;
  END COLUMN_SEQUENCES;
END;
/

CREATE INDEX "PedidoVendaVolume_INDEX1" ON DART."PedidoVendaVolume" ("IdPedidoVenda");
CREATE INDEX "PedidoVendaVolume_INDEX2" ON DART."PedidoVendaVolume" ("IdCaixaCubagem");
CREATE INDEX "PedidoVendaVolume_INDEX3" ON DART."PedidoVendaVolume" ("IdGrupoCorredorArmazenagem");
CREATE INDEX "PedidoVendaVolume_INDEX4" ON DART."PedidoVendaVolume" ("IdPerfilImpressora");
CREATE INDEX "PedidoVendaVolume_INDEX5" ON DART."PedidoVendaVolume" ("IdCaixaVolume");
CREATE INDEX "PedidoVendaVolume_INDEX6" ON DART."PedidoVendaVolume" ("IdPedidoVendaStatus");
CREATE INDEX "PedidoVendaVolume_INDEX7" ON DART."PedidoVendaVolume" ("IdUsuarioSeparacao");


ALTER TABLE DART."PedidoVendaVolume" ADD CONSTRAINT "PedidoVendaVolume_FK1" FOREIGN KEY ("IdPedidoVenda") REFERENCES DART."PedidoVenda"("IdPedidoVenda");
ALTER TABLE DART."PedidoVendaVolume" ADD CONSTRAINT "PedidoVendaVolume_FK2" FOREIGN KEY ("IdCaixaCubagem") REFERENCES DART."Caixa"("IdCaixa");
ALTER TABLE DART."PedidoVendaVolume" ADD CONSTRAINT "PedidoVendaVolume_FK3" FOREIGN KEY ("IdGrupoCorredorArmazenagem") REFERENCES DART."GrupoCorredorArmazenagem"("IdGrupoCorredorArmazenagem");
ALTER TABLE DART."PedidoVendaVolume" ADD CONSTRAINT "PedidoVendaVolume_FK4" FOREIGN KEY ("IdPerfilImpressora") REFERENCES DART."PerfilImpressora"("IdPerfilImpressora");
ALTER TABLE DART."PedidoVendaVolume" ADD CONSTRAINT "PedidoVendaVolume_FK5" FOREIGN KEY ("IdCaixaVolume") REFERENCES DART."Caixa"("IdCaixa");
ALTER TABLE DART."PedidoVendaVolume" ADD CONSTRAINT "PedidoVendaVolume_FK6" FOREIGN KEY ("IdPedidoVendaStatus") REFERENCES DART."PedidoVendaStatus"("IdPedidoVendaStatus");
ALTER TABLE DART."PedidoVendaVolume" ADD CONSTRAINT "PedidoVendaVolume_FK7" FOREIGN KEY ("IdUsuarioSeparacao") REFERENCES DART."AspNetUsers"("Id");
ALTER TABLE DART."PedidoVendaVolume" MODIFY "IdPedidoVendaStatus" NUMBER(19,0) NOT NULL;

