CREATE TABLE "QuarentenaStatus"
(
    "IdQuarentenaStatus" SMALLINT PRIMARY KEY NOT NULL,
    "Descricao" VARCHAR(2)
);

CREATE SEQUENCE QuarentenaStatus_Sequence;

CREATE OR REPLACE TRIGGER QuarentenaStatus_On_Insert
    BEFORE INSERT ON "QuarentenaStatus"
    FOR EACH ROW
BEGIN
    SELECT QuarentenaStatus_Sequence.nextval
    INTO :new."IdQuarentenaStatus"
    FROM dual;
END;
