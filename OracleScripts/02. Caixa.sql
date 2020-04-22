CREATE TABLE "Caixa" 
(
	"IdCaixa" NUMBER(19) NOT NULL,
	"IdEmpresa" NUMBER(19) NOT NULL,
	"IdCaixaTipo" NUMBER(10) NOT NULL,
	"Nome" VARCHAR2(50) NOT NULL,
	"TextoEtiqueta" VARCHAR2(50) NOT NULL,
	"Largura" NUMBER(38,2) NOT NULL,
	"Altura" NUMBER(38,2) NOT NULL,
	"Comprimento" NUMBER(38,2) NOT NULL,
	"Cubagem" NUMBER(38,2) NOT NULL,
	"PesoCaixa" NUMBER(38,2) NOT NULL,
	"PesoMaximo" NUMBER(38,2) NOT NULL,
	"Sobra" NUMBER(5,2) NOT NULL,
	"Prioridade" NUMBER(3) NOT NULL,
	"Ativo" NUMBER(1) NOT NULL,
	CONSTRAINT "Caixa_PK" PRIMARY KEY
	(
		"IdCaixa" 
	)
	USING INDEX 
	(
		CREATE UNIQUE INDEX "Caixa_PK" ON "Caixa" ("IdCaixa" ASC) 
	)
	ENABLE 
);

CREATE SEQUENCE "Caixa_Sequence";

CREATE OR REPLACE TRIGGER Caixa_On_Insert
    BEFORE INSERT ON "Caixa"
    FOR EACH ROW
BEGIN
    SELECT Caixa_Sequence.nextval
    INTO :new."IdCaixa"
    FROM dual;
END;

ALTER TABLE DART."Caixa" ADD CONSTRAINT "Caixa_Empresa" FOREIGN KEY ("IdEmpresa") REFERENCES DART."Empresa"("IdEmpresa");
ALTER TABLE DART."Caixa" ADD CONSTRAINT "Caixa_CaixaTipo" FOREIGN KEY ("IdCaixaTipo") REFERENCES DART."CaixaTipo"("IdCaixaTipo");

COMMIT;