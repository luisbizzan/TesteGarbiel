CREATE TABLE "PedidoVendaProduto" 
(
  "IdPedidoVendaProduto" NUMBER(19) NOT NULL 
, "IdPedidoVenda" NUMBER(19) NOT NULL 
, "IdPedidoVendaVolume" NUMBER(19) NOT NULL 
, "IdProduto" NUMBER(19) NOT NULL 
, "IdPedidoVendaStatus" NUMBER(10) NOT NULL
, "QtdSeparar" NUMBER(10) NOT NULL 
, "Sequencia" NUMBER(10) NOT NULL 
, "CubagemProduto" FLOAT NOT NULL
, "PesoProdutoKg" FLOAT NOT NULL
, "IdEnderecoArmazenagem" NUMBER(19,0) NOT NULL
, "QtdSeparada" NUMBER(10,0)
, "DataHoraFimSeparacao" DATE
, "DataHoraInicioSeparacao" DATE
, "IdUsuarioAutorizacaoZerar" NVARCHAR2(128)
, "DataHoraAutorizacaoZerarPedido" NVARCHAR2(128)
, "IdLote" NUMBER(19)
, "IdUsuarioSeparacao" VARCHAR2(128)
, CONSTRAINT "PedidoVendaProduto_PK" PRIMARY KEY 
  (
    "IdPedidoVendaProduto" 
  )
  USING INDEX 
  (
      CREATE UNIQUE INDEX "PedidoVendaProduto_PK" ON "PedidoVendaProduto" ("IdPedidoVendaProduto" ASC) 
  )
  ENABLE 
);

CREATE SEQUENCE "PedidoVendaProduto_SEQ";

CREATE trigger "PedidoVendaProduto_SEQ_TRG"  
   before insert on "DART"."PedidoVendaProduto" 
   for each row 
begin  
   if inserting then 
      if :NEW."IdPedidoVendaProduto" is null then 
         select "PedidoVendaProduto_SEQ".nextval into :NEW."IdPedidoVendaProduto" from dual; 
      end if; 
   end if; 
end;
/

CREATE INDEX "PedidoVendaProduto_INDEX1" ON DART."PedidoVendaProduto" ("IdPedidoVenda");
CREATE INDEX "PedidoVendaProduto_INDEX2" ON DART."PedidoVendaProduto" ("IdPedidoVendaVolume");
CREATE INDEX "PedidoVendaProduto_INDEX3" ON DART."PedidoVendaProduto" ("IdProduto");
CREATE INDEX "PedidoVendaProduto_INDEX4" ON DART."PedidoVendaProduto" ("IdPedidoVendaStatus");
CREATE INDEX "PedidoVendaProduto_INDEX5" ON DART."PedidoVendaProduto" ("IdLote");
CREATE INDEX "PedidoVendaProduto_INDEX6" ON DART."PedidoVendaProduto" ("IdUsuarioSeparacao");

ALTER TABLE DART."PedidoVendaProduto" ADD CONSTRAINT "PedidoVendaProduto_FK1" FOREIGN KEY ("IdPedidoVenda") REFERENCES DART."PedidoVenda"("IdPedidoVenda");
ALTER TABLE DART."PedidoVendaProduto" ADD CONSTRAINT "PedidoVendaProduto_FK2" FOREIGN KEY ("IdPedidoVendaVolume") REFERENCES DART."PedidoVendaVolume"("IdPedidoVendaVolume");
ALTER TABLE DART."PedidoVendaProduto" ADD CONSTRAINT "PedidoVendaProduto_FK3" FOREIGN KEY ("IdProduto") REFERENCES DART."Produto"("IdProduto");
ALTER TABLE DART."PedidoVendaProduto" ADD CONSTRAINT "PedidoVendaProduto_FK4" FOREIGN KEY ("IdPedidoVendaStatus") REFERENCES DART."PedidoVendaStatus"("IdPedidoVendaStatus");
ALTER TABLE DART."PedidoVendaProduto" ADD CONSTRAINT "PedidoVendaProduto_FK5" FOREIGN KEY ("IdLote") REFERENCES DART."Lote"("IdLote");
ALTER TABLE DART."PedidoVendaProduto" ADD CONSTRAINT "PedidoVendaProduto_FK6" FOREIGN KEY ("IdUsuarioSeparacao") REFERENCES DART."AspNetUsers"("Id");