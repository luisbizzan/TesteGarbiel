CREATE TABLE "NivelArmazenagem"
(
	"IdNivelArmazenagem" NUMBER,
	"IdEmpresa"          NUMBER NOT NULL,
	"Descricao"          VARCHAR2(1000) NOT NULL,
	"Ativo"              INTEGER NOT NULL,
	CONSTRAINT "NivelArmazenagem_PK" PRIMARY KEY ("IdNivelArmazenagem"),
	CONSTRAINT "NivelArmazenagem_FK1" FOREIGN KEY ("IdEmpresa") REFERENCES "Company" ("CompanyId")
);