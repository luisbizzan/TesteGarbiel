CREATE TABLE "Printer"
  (
     "Id"            INTEGER,
     "Name"          VARCHAR2(100) NOT NULL,
     "PrinterTypeId" INTEGER NOT NULL,
     "CompanyId"     NUMBER(22) NOT NULL,
     "IP"            VARCHAR2(50) NOT NULL,
     CONSTRAINT printer_pk PRIMARY KEY ("Id"),
     CONSTRAINT printer_company_fk FOREIGN KEY ("CompanyId") REFERENCES
     "Company" ("CompanyId"),
     CONSTRAINT printer_printertype_fk FOREIGN KEY ("PrinterTypeId") REFERENCES
     "PrinterType" ("Id")
  );

CREATE SEQUENCE printer_s;

CREATE OR replace TRIGGER printer_on_insert_tr
  BEFORE INSERT ON "Printer"
  FOR EACH ROW
BEGIN
    SELECT printer_s.nextval
    INTO   :new."Id"
    FROM   DUAL;
END;