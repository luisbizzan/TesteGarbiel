ALTER TABLE DART."PedidoVendaProduto" ADD "CubagemProduto" FLOAT;
ALTER TABLE DART."PedidoVendaProduto" ADD "PesoProdutoKg" FLOAT;
ALTER TABLE DART."PedidoVendaProduto" ADD "IdEnderecoArmazenagem" NUMBER(19,0);
ALTER TABLE DART."PedidoVendaProduto" ADD "QtdSeparada" NUMBER(10,0);
ALTER TABLE DART."PedidoVendaProduto" ADD "DataHoraFimSeparacao" DATE;
ALTER TABLE DART."PedidoVendaProduto" ADD "DataHoraInicioSeparacao" DATE;
ALTER TABLE DART."PedidoVendaProduto" ADD "IdUsuarioAutorizacaoZerar" NVARCHAR2(128);

ALTER TABLE DART."PedidoVendaProduto" MODIFY "CubagemProduto" FLOAT NOT NULL;
ALTER TABLE DART."PedidoVendaProduto" MODIFY "PesoProdutoKg" FLOAT NOT NULL;
ALTER TABLE DART."PedidoVendaProduto" MODIFY "IdEnderecoArmazenagem" NUMBER(19,0) NOT NULL;
