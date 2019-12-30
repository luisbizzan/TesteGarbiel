CREATE TABLE "LoteConferencia"
(
    "IdLoteConferencia" NUMBER PRIMARY KEY NOT NULL ,
    "IdLote" NUMBER REFERENCES "Lote"("IdLote") NOT NULL,
    "IdTipoConferencia" NUMBER(5,0) REFERENCES "TipoConferencia"("IdTipoConferencia") NOT NULL,
	"IdProduto" NUMBER REFERENCES "Produto" ("IdProduto") NOT NULL,
    "Quantidade" SMALLINT NOT NULL,
    "DataHoraInicio" TIMESTAMP(6) NOT NULL,
    "DataHoraFim" TIMESTAMP(6),
    "Tempo" TIME,
    "IdUsuarioConferente" VARCHAR2(128 BYTE) REFERENCES "AspNetUsers"("Id") NOT NULL
);