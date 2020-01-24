--------------------------------------------------------
--  File created - Friday-January-17-2020   
--------------------------------------------------------
--------------------------------------------------------
--  DDL for Trigger Transportadora_SEQ_TR
--------------------------------------------------------

  CREATE OR REPLACE EDITIONABLE TRIGGER "DART"."Transportadora_SEQ_TR" 
   before insert on "DART"."Transportadora" 
   for each row 
begin  
   if inserting then 
      if :NEW."IdTransportadora" is null then 
         select "Transportadora_SEQ".nextval into :NEW."IdTransportadora" from dual; 
      end if; 
   end if; 
end;
/
ALTER TRIGGER "DART"."Transportadora_SEQ_TR" ENABLE;
