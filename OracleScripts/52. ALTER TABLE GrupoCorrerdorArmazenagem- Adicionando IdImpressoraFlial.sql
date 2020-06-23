ALTER TABLE DART."GrupoCorredorArmazenagem" ADD "IdImpressoraPedidoFilial" NUMBER (38,0) NULL;

UPDATE DART."GrupoCorredorArmazenagem"
SET "IdImpressoraPedidoFilial" = "IdImpressora";

ALTER TABLE DART."GrupoCorredorArmazenagem" MODIFY ( "IdImpressoraPedidoFilial" NOT NULL);

ALTER TABLE DART."GrupoCorredorArmazenagem" ADD CONSTRAINT "GrupoCorredorArmazenagem_FK1" FOREIGN KEY ("IdImpressoraPedidoFilial") REFERENCES DART."Printer"("Id");