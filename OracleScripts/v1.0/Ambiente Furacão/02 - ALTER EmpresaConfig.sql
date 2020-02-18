
DROP SEQUENCE  "DART"."EmpresaConfig_SEQ";
DROP TRIGGER "DART"."EmpresaConfig_SEQ_TR";
ALTER TABLE "DART"."EmpresaConfig" DROP COLUMN "IdEmpresaConfig";
ALTER TABLE "DART"."EmpresaConfig" ADD CONSTRAINT "EmpresaConfig_PK" PRIMARY KEY ("IdEmpresa");