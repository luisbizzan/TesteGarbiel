INSERT INTO "ColetorHistoricoTipo" VALUES (7, 'Cancelamento Separação');
INSERT INTO "ColetorHistoricoTipo" VALUES (8, 'Finalização Separação');
INSERT INTO "ColetorHistoricoTipo" ("IdColetorHistoricoTipo","Descricao") VALUES (9,'MoverDoca');
INSERT INTO "ColetorHistoricoTipo" ("IdColetorHistoricoTipo","Descricao") VALUES (10,'Despacho NF');


COMMIT;

SELECT * FROM "ColetorHistoricoTipo" ORDER BY 1;