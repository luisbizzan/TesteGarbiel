ALTER TABLE DART."Empresa" ADD "IdCliente" NUMBER (19,0) NULL;


ALTER TABLE DART."Empresa" ADD CONSTRAINT "Empresa_FK1" FOREIGN KEY ("IdCliente") REFERENCES DART."Cliente"("IdCliente");

CREATE INDEX "Empresa_INDEX5" ON DART."Empresa" ("IdCliente");