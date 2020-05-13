INSERT INTO "ColetorHistoricoTipo" VALUES (7, 'Cancelamento Separação');
INSERT INTO "ColetorHistoricoTipo" VALUES (8, 'Finalização Separação');
INSERT INTO "ColetorHistoricoTipo" ("IdColetorHistoricoTipo","Descricao") VALUES (9,'MoverDoca');
INSERT INTO "ColetorHistoricoTipo" ("IdColetorHistoricoTipo","Descricao") VALUES (10,'Despacho NF');
INSERT INTO "ColetorHistoricoTipo" ("IdColetorHistoricoTipo","Descricao") VALUES (11,'Romaneio');
INSERT INTO "ColetorHistoricoTipo" ("IdColetorHistoricoTipo","Descricao") VALUES (12,'Reimpressão Romaneio');
INSERT INTO "ColetorHistoricoTipo" ("IdColetorHistoricoTipo","Descricao") VALUES (13,'Instalação Múltipla');

COMMIT;

SELECT * FROM "ColetorHistoricoTipo" ORDER BY 1 asc;