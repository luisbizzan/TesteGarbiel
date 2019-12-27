CREATE TABLE "TipoEtiquetagem"
(
    "IdTipoEtiquetagem" SMALLINT PRIMARY KEY NOT NULL ,
    "Descricao" VARCHAR(50) NOT NULL
);
    
CREATE SEQUENCE TipoEtiquetagem_Sequence;

CREATE OR REPLACE TRIGGER TipoEtiquetagem_On_Insert
    BEFORE INSERT ON "TipoEtiquetagem"
    FOR EACH ROW
BEGIN
    SELECT TipoEtiquetagem_Sequence.nextval
    INTO :new."IdTipoEtiquetagem"
    FROM dual;
END;