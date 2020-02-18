--Fornecedor
INSERT INTO Fornecedor(Codigo, NomeFantasia, RazaoSocial, CNPJ) VALUES ('', 'NAKATA AUTOMOTIVA', 'NAKATA AUTOMOTIVA S.A', '04156194000170');

--Transportadora
INSERT INTO Transportadora(NomeFantasia, RazaoSocial, CNPJ) VALUES ('TRANSJOI TRANSPORTES', 'TRANSJOI TRANSPORTES LTDA', '83630053000890');

--Unidade de Medida
INSERT INTO "UnidadeMedida"("Descricao") VALUES ('0');
INSERT INTO "UnidadeMedida"("Descricao") VALUES ('CX');
INSERT INTO "UnidadeMedida"("Descricao") VALUES ('DZ');
INSERT INTO "UnidadeMedida"("Descricao") VALUES ('KG');
INSERT INTO "UnidadeMedida"("Descricao") VALUES ('KW');
INSERT INTO "UnidadeMedida"("Descricao") VALUES ('LT');
INSERT INTO "UnidadeMedida"("Descricao") VALUES ('MT');
INSERT INTO "UnidadeMedida"("Descricao") VALUES ('UN');
INSERT INTO "UnidadeMedida"("Descricao") VALUES ('PC');


--Produto
INSERT INTO "Produto"("Descricao", "Referencia", "IdUnidadeMedida", "Peso") VALUES ('BOMBA D AGUA (LEVE) (19/2777)', 'NKBA01766', 41, 450);
INSERT INTO "Produto"("Descricao", "Referencia", "IdUnidadeMedida", "Peso") VALUES ('BOMBA D AGUA (LEVE) (19/2682)', 'NKBA03147', 41, 450);
INSERT INTO "Produto"("Descricao", "Referencia", "IdUnidadeMedida", "Peso") VALUES ('BOMBA D AGUA (LEVE) (19/2682)', 'NKBA03162', 41, 450);
INSERT INTO "Produto"("Descricao", "Referencia", "IdUnidadeMedida", "Peso") VALUES ('BOMBA D AGUA (LEVE) (19/2682)', 'NKBA07628', 41, 450);
INSERT INTO "Produto"("Descricao", "Referencia", "IdUnidadeMedida", "Peso") VALUES ('BOMBA D AGUA (LEVE) (19/2777)', 'NKBA07630', 41, 450);

--NotaFiscal
--INSERT INTO NotaFiscal(Numero, Serie, DANFE, IdFornecedor, ValorTotal) VALUES (351022, 1, '31191104156194000412550010003510221450513316', 1, 11393.79);

--Nota Fiscal Item
--INSERT INTO NotaFiscalItem(IdNotaFiscal, IdProduto, IdUnidadeMedida, Quantidade, ValorUnitario, ValorTotal) VALUES (2, 1, 1, 4, 40.33, 161.32);
--INSERT INTO NotaFiscalItem(IdNotaFiscal, IdProduto, IdUnidadeMedida, Quantidade, ValorUnitario, ValorTotal) VALUES (2, 2, 1, 4, 34.32, 137.28);
--INSERT INTO NotaFiscalItem(IdNotaFiscal, IdProduto, IdUnidadeMedida, Quantidade, ValorUnitario, ValorTotal) VALUES (2, 3, 1, 4, 43.87, 175.48);
--INSERT INTO NotaFiscalItem(IdNotaFiscal, IdProduto, IdUnidadeMedida, Quantidade, ValorUnitario, ValorTotal) VALUES (2, 4, 1, 4, 33.06, 132.24);
--INSERT INTO NotaFiscalItem(IdNotaFiscal, IdProduto, IdUnidadeMedida, Quantidade, ValorUnitario, ValorTotal) VALUES (2, 5, 1, 8, 38, 304);

--Frete Tipo
INSERT INTO FreteTipo(Descricao) VALUES ('FOB');

--Frete
INSERT INTO Frete(IdNotaFiscal, IdTransportadora, IdFreteTipo, Valor, NumeroConhecimento, PesoBruto, PesoLiquido, Especie, Quantidade) VALUES (2, 1, 1, 0, 0, 216.435, 215.358, 'Volume(s)', 24);

--Lote Status
INSERT INTO LoteStatus(Descricao) VALUES ('Aguardando recebimento');
INSERT INTO LoteStatus(Descricao) VALUES ('Recebido');
INSERT INTO LoteStatus(Descricao) VALUES ('Em conferência');
INSERT INTO LoteStatus(Descricao) VALUES ('Finalizado');
INSERT INTO LoteStatus(Descricao) VALUES ('Conferido com divergência');
INSERT INTO LoteStatus(Descricao) VALUES ('Finalizado com divergência (A+)');
INSERT INTO LoteStatus(Descricao) VALUES ('Finalizado com divergência (A-)');
INSERT INTO LoteStatus(Descricao) VALUES ('Finalizado com divergência (invertido)');
INSERT INTO LoteStatus(Descricao) VALUES ('Finalizado com divergência (A+, A- e invertido)');

-- Lote
INSERT INTO Lote(IdLoteStatus, IdNotaFiscal, DataCompra, DataRecebimento, QuantidadePeca, QuantidadeVolume, RecebidoPor, ConferidoPor) VALUES (10, 2, null, null, null, null, null, null);
