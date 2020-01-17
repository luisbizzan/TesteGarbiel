--------------------------------------------------------
--  File created - Friday-January-17-2020   
--------------------------------------------------------
--------------------------------------------------------
--  DDL for Trigger FreteTipo_SEQ_TR
--------------------------------------------------------

  CREATE OR REPLACE EDITIONABLE TRIGGER "DART"."FreteTipo_SEQ_TR" 
   before insert on "DART"."FreteTipo" 
   for each row 
begin  
   if inserting then 
      if :NEW."IdFreteTipo" is null then 
         select "FreteTipo_SEQ".nextval into :NEW."IdFreteTipo" from dual; 
      end if; 
   end if; 
end;
/
ALTER TRIGGER "DART"."FreteTipo_SEQ_TR" ENABLE;
