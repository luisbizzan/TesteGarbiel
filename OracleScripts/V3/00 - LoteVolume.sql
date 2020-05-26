CREATE TABLE "LoteVolume" 
(
  "IdLote" NUMBER(19) NOT NULL 
, "NroVolume" NUMBER(10) NOT NULL 
, "IdEnderecoArmazenagem" NUMBER(19) 
, "DataInstalacao" DATE 
, "IdUsuarioInstalacao" VARCHAR2(128) 
, "DataDesinstalacao" DATE 
, "IdUsuarioDesinstalacao" VARCHAR2(128) 
, CONSTRAINT "LoteVolume_PK" PRIMARY KEY 
  (
    "IdLote" 
  , "NroVolume" 
  )
  ENABLE 
);

ALTER TABLE DART."LoteVolume" ADD CONSTRAINT "LoteVolume_FK1" FOREIGN KEY ("IdLote") REFERENCES DART."Lote"("IdLote");
ALTER TABLE DART."LoteVolume" ADD CONSTRAINT "LoteVolume_FK2" FOREIGN KEY ("IdUsuarioInstalacao") REFERENCES DART."AspNetUsers"("Id");
ALTER TABLE DART."LoteVolume" ADD CONSTRAINT "LoteVolume_FK3" FOREIGN KEY ("IdUsuarioDesinstalacao") REFERENCES DART."AspNetUsers"("Id");
ALTER TABLE DART."LoteVolume" ADD CONSTRAINT "LoteVolume_FK4" FOREIGN KEY ("IdEnderecoArmazenagem") REFERENCES DART."EnderecoArmazenagem"("IdEnderecoArmazenagem");

ALTER TABLE DART."LoteVolume" DROP COLUMN "DataDesinstalacao";
ALTER TABLE DART."LoteVolume" DROP COLUMN "IdUsuarioDesinstalacao";

