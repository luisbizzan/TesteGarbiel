ALTER TABLE "UsuarioEmpresa" ADD "IdPerfilImpressoraPadrao" NUMBER(19, 0);

ALTER TABLE "UsuarioEmpresa" ADD CONSTRAINT "UsuarioEmpresa_FK4" FOREIGN KEY ("IdPerfilImpressoraPadrao") REFERENCES "PerfilImpressora" ("IdPerfilImpressora");

CREATE INDEX "UsuarioEmpresa_INDEX3" ON "UsuarioEmpresa" ("IdPerfilImpressoraPadrao");