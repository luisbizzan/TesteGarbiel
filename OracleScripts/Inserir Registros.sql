--Fornecedor
INSERT INTO "Fornecedor"("Codigo", "NomeFantasia", "RazaoSocial", "CNPJ") VALUES ('', 'NAKATA AUTOMOTIVA', 'NAKATA AUTOMOTIVA S.A', '04156194000170');

--Transportadora
INSERT INTO "Transportadora"("NomeFantasia", "RazaoSocial", "CNPJ") VALUES ('TRANSJOI TRANSPORTES', 'TRANSJOI TRANSPORTES LTDA', '83630053000890');

--Unidade de Medida
INSERT INTO "UnidadeMedida"("Descricao") VALUES ('PC');

--Produto
INSERT INTO "Produto"("Descricao", "Referencia", "IdUnidadeMedida", "Peso") VALUES ('BOMBA D AGUA (LEVE) (19/2777)', 'NKBA01766', 1, 450);
INSERT INTO "Produto"("Descricao", "Referencia", "IdUnidadeMedida", "Peso") VALUES ('BOMBA D AGUA (LEVE) (19/2682)', 'NKBA03147', 1, 450);
INSERT INTO "Produto"("Descricao", "Referencia", "IdUnidadeMedida", "Peso") VALUES ('BOMBA D AGUA (LEVE) (19/2682)', 'NKBA03162', 1, 450);
INSERT INTO "Produto"("Descricao", "Referencia", "IdUnidadeMedida", "Peso") VALUES ('BOMBA D AGUA (LEVE) (19/2682)', 'NKBA07628', 1, 450);
INSERT INTO "Produto"("Descricao", "Referencia", "IdUnidadeMedida", "Peso") VALUES ('BOMBA D AGUA (LEVE) (19/2777)', 'NKBA07630', 1, 450);

--NotaFiscal
INSERT INTO "NotaFiscal"("Numero", "Serie", "DANFE", "IdFornecedor", "ValorTotal") VALUES (351022, 1, '31191104156194000412550010003510221450513316', 1, 11393.79);

--Nota Fiscal Item
INSERT INTO "NotaFiscalItem"("IdNotaFiscal", "IdProduto", "IdUnidadeMedida", "Quantidade", "ValorUnitario", "ValorTotal") VALUES (2, 1, 1, 4, 40.33, 161.32);
INSERT INTO "NotaFiscalItem"("IdNotaFiscal", "IdProduto", "IdUnidadeMedida", "Quantidade", "ValorUnitario", "ValorTotal") VALUES (2, 2, 1, 4, 34.32, 137.28);
INSERT INTO "NotaFiscalItem"("IdNotaFiscal", "IdProduto", "IdUnidadeMedida", "Quantidade", "ValorUnitario", "ValorTotal") VALUES (2, 3, 1, 4, 43.87, 175.48);
INSERT INTO "NotaFiscalItem"("IdNotaFiscal", "IdProduto", "IdUnidadeMedida", "Quantidade", "ValorUnitario", "ValorTotal") VALUES (2, 4, 1, 4, 33.06, 132.24);
INSERT INTO "NotaFiscalItem"("IdNotaFiscal", "IdProduto", "IdUnidadeMedida", "Quantidade", "ValorUnitario", "ValorTotal") VALUES (2, 5, 1, 8, 38, 304);

--Frete Tipo
INSERT INTO "FreteTipo"("Descricao") VALUES ('FOB');

--Lote Status
INSERT INTO "LoteStatus"("Descricao") VALUES ('Aguardando recebimento');
INSERT INTO "LoteStatus"("Descricao") VALUES ('Recebido');
INSERT INTO "LoteStatus"("Descricao") VALUES ('Em conferência');
INSERT INTO "LoteStatus"("Descricao") VALUES ('Finalizado');
INSERT INTO "LoteStatus"("Descricao") VALUES ('Conferido com divergência');
INSERT INTO "LoteStatus"("Descricao") VALUES ('Finalizado com divergência (A+)');
INSERT INTO "LoteStatus"("Descricao") VALUES ('Finalizado com divergência (A-)');
INSERT INTO "LoteStatus"("Descricao") VALUES ('Finalizado com divergência (invertido)');
INSERT INTO "LoteStatus"("Descricao") VALUES ('Finalizado com divergência (A+, A- e invertido)');

-- Lote
INSERT INTO "Lote"("IdLoteStatus", "IdNotaFiscal", "DataCompra", "DataRecebimento", "QuantidadePeca", "QuantidadeVolume", "RecebidoPor", "ConferidoPor") VALUES (10, 2, null, null, null, null, null, null);
