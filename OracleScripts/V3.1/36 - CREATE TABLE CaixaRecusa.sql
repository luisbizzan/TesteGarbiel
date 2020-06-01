CREATE TABLE "CaixaRecusa"
(
    "IdEmpresa" NUMBER(19) REFERENCES "Empresa"("IdEmpresa") NOT NULL,
    "IdCaixa" NUMBER(19) REFERENCES "Caixa"("IdCaixa") NOT NULL,
    "IdProduto" NUMBER(19) REFERENCES "Produto"("IdProduto") NOT NULL
);