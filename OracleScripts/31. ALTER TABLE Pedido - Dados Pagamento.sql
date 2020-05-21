ALTER TABLE DART."Pedido" ADD "PagamentoCodigoIntegracao" NUMBER (22,0) NULL;
ALTER TABLE DART."Pedido" ADD "PagamentoDescricaoIntegracao" VARCHAR2 (36) NULL;
ALTER TABLE DART."Pedido" ADD "PagamentoIsDebitoIntegracao" NUMBER (1,0) DEFAULT 0 NOT NULL;
ALTER TABLE DART."Pedido" ADD "PagamentoIsCreditoIntegracao" NUMBER (1,0) DEFAULT 0 NOT NULL;