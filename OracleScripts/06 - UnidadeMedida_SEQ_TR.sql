--------------------------------------------------------
--  File created - Friday-January-17-2020   
--------------------------------------------------------
--------------------------------------------------------
--  DDL for Trigger UnidadeMedida_SEQ_TR
--------------------------------------------------------

  CREATE OR REPLACE EDITIONABLE TRIGGER "DART"."UnidadeMedida_SEQ_TR" 
   before insert on "DART"."UnidadeMedida" 
   for each row 
begin  
   if inserting then 
      if :NEW."IdUnidadeMedida" is null then 
         select "UnidadeMedida_SEQ".nextval into :NEW."IdUnidadeMedida" from dual; 
      end if; 
   end if; 
end;
/
ALTER TRIGGER "DART"."UnidadeMedida_SEQ_TR" ENABLE;
