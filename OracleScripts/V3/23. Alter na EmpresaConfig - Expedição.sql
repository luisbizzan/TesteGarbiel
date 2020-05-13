ALTER TABLE "EmpresaConfig" ADD "IdDiasDaSemana" NUMBER (10,0) NULL;
ALTER TABLE "EmpresaConfig" ADD "IdTransportadora" NUMBER (19,0) NULL;


CREATE INDEX "EmpresaConfig_INDEX6" ON DART."EmpresaConfig" ("IdDiasDaSemana");
CREATE INDEX "EmpresaConfig_INDEX7" ON DART."EmpresaConfig" ("IdTransportadora");

ALTER TABLE DART."EmpresaConfig" ADD CONSTRAINT "EmpresaConfig_FK6" FOREIGN KEY ("IdDiasDaSemana") REFERENCES DART."DiasDaSemana"("IdDiasDaSemana");
ALTER TABLE DART."EmpresaConfig" ADD CONSTRAINT "EmpresaConfig_FK7" FOREIGN KEY ("IdTransportadora") REFERENCES DART."Transportadora"("IdTransportadora");