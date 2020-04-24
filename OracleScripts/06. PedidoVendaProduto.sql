CREATE TABLE "PedidoVendaProduto" 
(
  "IdPedidoVendaProduto" NUMBER(19) NOT NULL 
, "IdPedidoVenda" NUMBER(19) NOT NULL 
, "IdProduto" NUMBER(19) NOT NULL 
, "QtdPedido" NUMBER(10) NOT NULL 
, "Sequence" NUMBER(10) NOT NULL 
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

ALTER TABLE DART."PedidoVendaProduto" RENAME COLUMN "Sequence" TO "Sequencia";
ALTER TABLE DART."PedidoVendaProduto" ADD "IdPedidoVendaProdutoStatus" NUMBER(10,0) NOT NULL;

CREATE INDEX "PedidoVendaProduto_INDEX1" ON DART."PedidoVendaProduto" ("IdPedidoVenda");
CREATE INDEX "PedidoVendaProduto_INDEX2" ON DART."PedidoVendaProduto" ("IdProduto");

ALTER TABLE DART."PedidoVendaProduto" ADD CONSTRAINT "PedidoVendaProduto_FK1" FOREIGN KEY ("IdPedidoVenda") REFERENCES DART."PedidoVenda"("IdPedidoVenda");
ALTER TABLE DART."PedidoVendaProduto" ADD CONSTRAINT "PedidoVendaProduto_FK2" FOREIGN KEY ("IdProduto") REFERENCES DART."Produto"("IdProduto");
ALTER TABLE DART."PedidoVendaProduto" ADD CONSTRAINT "PedidoVendaProdutoStatus_FK3" FOREIGN KEY ("IdPedidoVendaProdutoStatus") REFERENCES DART."PedidoVendaProdutoStatus"("IdPedidoVendaProdutoStatus");