DELETE FROM DART."ImpressaoItem";

REM INSERTING into DART."ImpressaoItem"
SET DEFINE OFF;
Insert into DART."ImpressaoItem" ("IdImpressaoItem","Descricao") values ('3','Etiqueta Individual');
Insert into DART."ImpressaoItem" ("IdImpressaoItem","Descricao") values ('1','Relatório A4');
Insert into DART."ImpressaoItem" ("IdImpressaoItem","Descricao") values ('2','Etiqueta de Lote');
Insert into DART."ImpressaoItem" ("IdImpressaoItem","Descricao") values ('4','Etiqueta Avulso');
Insert into DART."ImpressaoItem" ("IdImpressaoItem","Descricao") values ('5','Etiqueta de Recebimento');
Insert into DART."ImpressaoItem" ("IdImpressaoItem","Descricao") values ('6','Etiqueta de Endereco');
Insert into DART."ImpressaoItem" ("IdImpressaoItem","Descricao") values ('8','Etiqueta Personalizada');
Insert into DART."ImpressaoItem" ("IdImpressaoItem","Descricao") values ('7','Etiqueta de Devolução');
COMMIT;