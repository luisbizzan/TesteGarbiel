INSERT INTO "ColetorHistoricoTipo" VALUES (7, 'Cancelamento Separa��o');
INSERT INTO "ColetorHistoricoTipo" VALUES (8, 'Finaliza��o Separa��o');
INSERT INTO "ColetorHistoricoTipo" ("IdColetorHistoricoTipo","Descricao") VALUES (9,'MoverDoca');
INSERT INTO "ColetorHistoricoTipo" ("IdColetorHistoricoTipo","Descricao") VALUES (10,'Despacho NF');
INSERT INTO DART."ColetorHistoricoTipo" ("IdColetorHistoricoTipo","Descricao") VALUES (11,'Reimpress�o Romaneio');



COMMIT;

SELECT * FROM "ColetorHistoricoTipo" ORDER BY 1;