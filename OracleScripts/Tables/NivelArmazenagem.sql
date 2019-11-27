CREATE TABLE "NivelArmazenagem"
(
	"IdNivelArmazenagem" NUMBER,
	"IdEmpresa"          NUMBER NOT NULL,
	"Descricao"          VARCHAR2(1000) NOT NULL,
	"Ativo"              INTEGER NOT NULL,
	CONSTRAINT "NivelArmazenagem_PK" PRIMARY KEY ("IdNivelArmazenagem"),
	CONSTRAINT "NivelArmazenagem_FK1" FOREIGN KEY ("IdEmpresa") REFERENCES "Company" ("CompanyId")
);

CREATE SEQUENCE "NivelArmazenagem_SEQ";

CREATE OR replace TRIGGER "NivelArmazenagem_SEQ_TR"
  BEFORE INSERT ON "NivelArmazenagem"
  FOR EACH ROW
BEGIN
    SELECT "NivelArmazenagem_SEQ".nextval
    INTO   :new."IdNivelArmazenagem"
    FROM   DUAL;
END;