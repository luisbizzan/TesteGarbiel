CREATE TABLE "PrinterType"
  (
     "Id"   INTEGER,
     "Name" VARCHAR2(100) NOT NULL,
     "Type" INTEGER NOT NULL,
     CONSTRAINT printertype_pk PRIMARY KEY ("Id"),
     CONSTRAINT printertype_type_uk UNIQUE ("Type")
  );

CREATE SEQUENCE printer_type_s;

CREATE OR replace TRIGGER printer_type_on_insert_tr
  BEFORE INSERT ON "PrinterType"
  FOR EACH ROW
BEGIN
    SELECT printer_type_s.nextval
    INTO   :new.id
    FROM   DUAL;
END; 