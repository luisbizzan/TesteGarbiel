CREATE TABLE "PedidoItem" 
(
  "IdPedidoItem" NUMBER(19) NOT NULL 
, "IdPedido" NUMBER(19) NOT NULL 
, "IdProduto" NUMBER(19) NOT NULL 
, "QtdPedido" NUMBER(10) NOT NULL 
, "Sequencia" NUMBER(10) NOT NULL 
, CONSTRAINT "PedidoItem_PK" PRIMARY KEY 
  (
    "IdPedidoItem" 
  )
  USING INDEX 
  (
      CREATE UNIQUE INDEX "PedidoItem_PK" ON "PedidoItem" ("IdPedidoItem" ASC) 
  )
  ENABLE 
);

CREATE SEQUENCE "PedidoItem_SEQ";

CREATE trigger "PedidoItem_SEQ_TRG"  
   before insert on "DART"."PedidoItem" 
   for each row 
begin  
   if inserting then 
      if :NEW."IdPedidoItem" is null then 
         select "PedidoItem_SEQ".nextval into :NEW."IdPedidoItem" from dual; 
      end if; 
   end if; 
end;
/

CREATE INDEX "PedidoItem_INDEX1" ON DART."PedidoItem" ("IdPedido");
CREATE INDEX "PedidoItem_INDEX2" ON DART."PedidoItem" ("IdProduto");

ALTER TABLE DART."PedidoItem" ADD CONSTRAINT "PedidoItem_FK1" FOREIGN KEY ("IdPedido") REFERENCES DART."Pedido"("IdPedido");
ALTER TABLE DART."PedidoItem" ADD CONSTRAINT "PedidoItem_FK2" FOREIGN KEY ("IdProduto") REFERENCES DART."Produto"("IdProduto");
ALTER TABLE DART."PedidoItem" ADD CONSTRAINT "PedidoItemStatus_FK3" FOREIGN KEY ("IdPedidoItemStatus") REFERENCES DART."PedidoItemStatus"("IdPedidoItemStatus");