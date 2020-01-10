ALTER TABLE DART."Lote" ADD "DataInicioConferencia" DATE;
ALTER TABLE DART."Lote" ADD "DataFinalConferencia" DATE;
ALTER TABLE DART."Lote" ADD "TempoTotalConferencia" NUMBER(19,0);


-- Auto-generated SQL script #202001091414
UPDATE DART."TipoConferencia" x
	SET x."Descricao"='Conferência 100%'
	WHERE x."IdTipoConferencia"=2;
