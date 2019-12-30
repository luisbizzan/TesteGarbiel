CREATE TABLE "PrinterType"
  (
     "Id"   INTEGER,
     "Name" VARCHAR2(100) NOT NULL,
     CONSTRAINT printertype_pk PRIMARY KEY ("Id")
  );

CREATE SEQUENCE printer_type_s;

CREATE OR replace TRIGGER printer_type_on_insert_tr
  BEFORE INSERT ON "PrinterType"
  FOR EACH ROW
BEGIN
    SELECT printer_type_s.nextval
    INTO   :new."Id"
    FROM   DUAL;
END;