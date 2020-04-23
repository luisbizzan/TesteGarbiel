CREATE TABLE "PedidoItemProduto" 
(
  "IdPedidoItemProduto" NUMBER(19) NOT NULL 
, "IdPedidoItem" NUMBER(19) NOT NULL 
, "IdProduto" NUMBER(19) NOT NULL 
, "QtdPedido" NUMBER(10) NOT NULL 
, "Sequence" NUMBER(10) NOT NULL 
, CONSTRAINT "PedidoItemProduto_PK" PRIMARY KEY 
  (
    "IdPedidoItemProduto" 
  )
  USING INDEX 
  (
      CREATE UNIQUE INDEX "PedidoItemProduto_PK" ON "PedidoItemProduto" ("IdPedidoItemProduto" ASC) 
  )
  ENABLE 
);

CREATE SEQUENCE "PedidoItemProduto_SEQ";

CREATE trigger "PedidoItemProduto_SEQ_TRG"  
   before insert on "DART"."PedidoItemProduto" 
   for each row 
begin  
   if inserting then 
      if :NEW."IdPedidoItemProduto" is null then 
         select "PedidoItemProduto_SEQ".nextval into :NEW."IdPedidoItemProduto" from dual; 
      end if; 
   end if; 
end;
/

ALTER TABLE DART."PedidoItemProduto" RENAME COLUMN "Sequence" TO "Sequencia";
ALTER TABLE DART."PedidoItemProduto" ADD "IdPedidoItemProdutoStatus" NUMBER(10,0) NOT NULL;

CREATE INDEX "PedidoItemProduto_INDEX1" ON DART."PedidoItemProduto" ("IdPedidoItem");
CREATE INDEX "PedidoItemProduto_INDEX2" ON DART."PedidoItemProduto" ("IdProduto");

ALTER TABLE DART."PedidoItemProduto" ADD CONSTRAINT "PedidoItemProduto_FK1" FOREIGN KEY ("IdPedidoItem") REFERENCES DART."PedidoItem"("IdPedidoItem");
ALTER TABLE DART."PedidoItemProduto" ADD CONSTRAINT "PedidoItemProduto_FK2" FOREIGN KEY ("IdProduto") REFERENCES DART."Produto"("IdProduto");
ALTER TABLE DART."PedidoItemProduto" ADD CONSTRAINT "PedidoItemProdutoStatus_FK3" FOREIGN KEY ("IdPedidoItemProdutoStatus") REFERENCES DART."PedidoItemProdutoStatus"("IdPedidoItemProdutoStatus");