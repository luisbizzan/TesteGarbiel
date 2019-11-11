--------------------------------------------------------
--  Arquivo criado - Quinta-feira-Outubro-31-2019   
--------------------------------------------------------
--------------------------------------------------------
--  DDL for Sequence APPLICATIONLANGUAGE_SEQ
--------------------------------------------------------

   CREATE SEQUENCE  "DART"."APPLICATIONLANGUAGE_SEQ"  MINVALUE 1 MAXVALUE 9999999999999999999999999999 INCREMENT BY 1 START WITH 21 CACHE 20 NOORDER  NOCYCLE   ;
--------------------------------------------------------
--  DDL for Sequence APPLICATIONLOG_SEQ
--------------------------------------------------------

   CREATE SEQUENCE  "DART"."APPLICATIONLOG_SEQ"  MINVALUE 1 MAXVALUE 9999999999999999999999999999 INCREMENT BY 1 START WITH 21 CACHE 20 NOORDER  NOCYCLE   ;
--------------------------------------------------------
--  DDL for Sequence APPLICATIONSESSION_SEQ
--------------------------------------------------------

   CREATE SEQUENCE  "DART"."APPLICATIONSESSION_SEQ"  MINVALUE 1 MAXVALUE 9999999999999999999999999999 INCREMENT BY 1 START WITH 81 CACHE 20 NOORDER  NOCYCLE   ;
--------------------------------------------------------
--  DDL for Sequence ASPNETUSERCLAIMS_SEQ
--------------------------------------------------------

   CREATE SEQUENCE  "DART"."ASPNETUSERCLAIMS_SEQ"  MINVALUE 1 MAXVALUE 9999999999999999999999999999 INCREMENT BY 1 START WITH 1 CACHE 20 NOORDER  NOCYCLE   ;
--------------------------------------------------------
--  DDL for Sequence BOLOGSYSTEM_SEQ
--------------------------------------------------------

   CREATE SEQUENCE  "DART"."BOLOGSYSTEM_SEQ"  MINVALUE 1 MAXVALUE 9999999999999999999999999999 INCREMENT BY 1 START WITH 21 CACHE 20 NOORDER  NOCYCLE   ;
--------------------------------------------------------
--  DDL for Sequence COMPANY_SEQ
--------------------------------------------------------

   CREATE SEQUENCE  "DART"."COMPANY_SEQ"  MINVALUE 1 MAXVALUE 9999999999999999999999999999 INCREMENT BY 1 START WITH 61 CACHE 20 NOORDER  NOCYCLE   ;
--------------------------------------------------------
--  DDL for Sequence PERFILUSUARIO_SEQ
--------------------------------------------------------

   CREATE SEQUENCE  "DART"."PERFILUSUARIO_SEQ"  MINVALUE 1 MAXVALUE 9999999999999999999999999999 INCREMENT BY 1 START WITH 21 CACHE 20 NOORDER  NOCYCLE   ;
--------------------------------------------------------
--  DDL for Table Application
--------------------------------------------------------

  CREATE TABLE "DART"."Application" 
   (	"IdApplication" NUMBER, 
	"Name" VARCHAR2(50 BYTE)
   ) SEGMENT CREATION IMMEDIATE 
  PCTFREE 10 PCTUSED 40 INITRANS 1 MAXTRANS 255 
 NOCOMPRESS LOGGING
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "USERS" ;
--------------------------------------------------------
--  DDL for Table ApplicationLanguage
--------------------------------------------------------

  CREATE TABLE "DART"."ApplicationLanguage" 
   (	"IdApplicationLanguage" NUMBER, 
	"CultureName" VARCHAR2(10 BYTE), 
	"DisplayName" VARCHAR2(100 BYTE), 
	"IsDisabled" NUMBER(3,0)
   ) SEGMENT CREATION IMMEDIATE 
  PCTFREE 10 PCTUSED 40 INITRANS 1 MAXTRANS 255 
 NOCOMPRESS LOGGING
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "USERS" ;
--------------------------------------------------------
--  DDL for Table APPLICATIONLOG
--------------------------------------------------------

  CREATE TABLE "DART"."APPLICATIONLOG" 
   (	"IDAPPLICATIONLOG" NUMBER(10,0), 
	"CREATED" TIMESTAMP (3), 
	"LOGLEVEL" VARCHAR2(50 BYTE), 
	"MESSAGE" VARCHAR2(4000 BYTE), 
	"LOGEXCEPTION" CLOB, 
	"IDAPPLICATION" NUMBER
   ) SEGMENT CREATION IMMEDIATE 
  PCTFREE 10 PCTUSED 40 INITRANS 1 MAXTRANS 255 
 NOCOMPRESS LOGGING
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "USERS" 
 LOB ("LOGEXCEPTION") STORE AS SECUREFILE (
  TABLESPACE "USERS" ENABLE STORAGE IN ROW CHUNK 8192
  NOCACHE LOGGING  NOCOMPRESS  KEEP_DUPLICATES 
  STORAGE(INITIAL 106496 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)) ;
--------------------------------------------------------
--  DDL for Table ApplicationSession
--------------------------------------------------------

  CREATE TABLE "DART"."ApplicationSession" 
   (	"IdApplicationSession" NUMBER, 
	"IdAspNetUsers" VARCHAR2(128 BYTE), 
	"IdApplication" NUMBER, 
	"DataLogin" TIMESTAMP (0), 
	"DataUltimaAcao" TIMESTAMP (0), 
	"DataLogout" TIMESTAMP (0), 
	"CompanyId" NUMBER
   ) SEGMENT CREATION IMMEDIATE 
  PCTFREE 10 PCTUSED 40 INITRANS 1 MAXTRANS 255 
 NOCOMPRESS LOGGING
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "USERS" ;
--------------------------------------------------------
--  DDL for Table AspNetPermissions
--------------------------------------------------------

  CREATE TABLE "DART"."AspNetPermissions" 
   (	"Id" VARCHAR2(128 BYTE), 
	"ApplicationId" NUMBER(10,0), 
	"Name" VARCHAR2(256 BYTE)
   ) SEGMENT CREATION IMMEDIATE 
  PCTFREE 10 PCTUSED 40 INITRANS 1 MAXTRANS 255 
 NOCOMPRESS LOGGING
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "USERS" ;
--------------------------------------------------------
--  DDL for Table AspNetRolePermissions
--------------------------------------------------------

  CREATE TABLE "DART"."AspNetRolePermissions" 
   (	"RoleId" VARCHAR2(128 BYTE), 
	"PermissionId" VARCHAR2(128 BYTE)
   ) SEGMENT CREATION IMMEDIATE 
  PCTFREE 10 PCTUSED 40 INITRANS 1 MAXTRANS 255 
 NOCOMPRESS LOGGING
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "USERS" ;
--------------------------------------------------------
--  DDL for Table AspNetRoles
--------------------------------------------------------

  CREATE TABLE "DART"."AspNetRoles" 
   (	"Id" VARCHAR2(128 BYTE), 
	"Name" VARCHAR2(256 BYTE), 
	"ApplicationId" NUMBER(10,0)
   ) SEGMENT CREATION IMMEDIATE 
  PCTFREE 10 PCTUSED 40 INITRANS 1 MAXTRANS 255 
 NOCOMPRESS LOGGING
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "USERS" ;
--------------------------------------------------------
--  DDL for Table AspNetUserClaims
--------------------------------------------------------

  CREATE TABLE "DART"."AspNetUserClaims" 
   (	"Id" NUMBER(10,0), 
	"UserId" VARCHAR2(128 BYTE), 
	"ClaimType" CLOB, 
	"ClaimValue" CLOB
   ) SEGMENT CREATION DEFERRED 
  PCTFREE 10 PCTUSED 40 INITRANS 1 MAXTRANS 255 
 NOCOMPRESS LOGGING
  TABLESPACE "USERS" 
 LOB ("ClaimType") STORE AS SECUREFILE (
  TABLESPACE "USERS" ENABLE STORAGE IN ROW CHUNK 8192
  NOCACHE LOGGING  NOCOMPRESS  KEEP_DUPLICATES ) 
 LOB ("ClaimValue") STORE AS SECUREFILE (
  TABLESPACE "USERS" ENABLE STORAGE IN ROW CHUNK 8192
  NOCACHE LOGGING  NOCOMPRESS  KEEP_DUPLICATES ) ;
--------------------------------------------------------
--  DDL for Table AspNetUserLogins
--------------------------------------------------------

  CREATE TABLE "DART"."AspNetUserLogins" 
   (	"LoginProvider" VARCHAR2(128 BYTE), 
	"ProviderKey" VARCHAR2(128 BYTE), 
	"UserId" VARCHAR2(128 BYTE)
   ) SEGMENT CREATION DEFERRED 
  PCTFREE 10 PCTUSED 40 INITRANS 1 MAXTRANS 255 
 NOCOMPRESS LOGGING
  TABLESPACE "USERS" ;
--------------------------------------------------------
--  DDL for Table AspNetUserPermissions
--------------------------------------------------------

  CREATE TABLE "DART"."AspNetUserPermissions" 
   (	"UserId" VARCHAR2(128 BYTE), 
	"PermissionId" VARCHAR2(128 BYTE)
   ) SEGMENT CREATION IMMEDIATE 
  PCTFREE 10 PCTUSED 40 INITRANS 1 MAXTRANS 255 
 NOCOMPRESS LOGGING
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "USERS" ;
--------------------------------------------------------
--  DDL for Table AspNetUserRoles
--------------------------------------------------------

  CREATE TABLE "DART"."AspNetUserRoles" 
   (	"UserId" VARCHAR2(128 BYTE), 
	"RoleId" VARCHAR2(128 BYTE), 
	"CompanyId" NUMBER
   ) SEGMENT CREATION IMMEDIATE 
  PCTFREE 10 PCTUSED 40 INITRANS 1 MAXTRANS 255 
 NOCOMPRESS LOGGING
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "USERS" ;
--------------------------------------------------------
--  DDL for Table AspNetUsers
--------------------------------------------------------

  CREATE TABLE "DART"."AspNetUsers" 
   (	"Id" VARCHAR2(128 BYTE), 
	"ApplicationId" NUMBER, 
	"Email" VARCHAR2(256 BYTE) DEFAULT NULL, 
	"EmailConfirmed" NUMBER(3,0), 
	"PasswordHash" CLOB, 
	"SecurityStamp" CLOB, 
	"PhoneNumber" CLOB, 
	"PhoneNumberConfirmed" NUMBER(3,0), 
	"TwoFactorEnabled" NUMBER(3,0), 
	"LockoutEndDateUtc" TIMESTAMP (0) DEFAULT NULL, 
	"LockoutEnabled" NUMBER(3,0), 
	"AccessFailedCount" NUMBER(10,0), 
	"UserName" VARCHAR2(256 BYTE), 
	"IdApplicationSession" NUMBER
   ) SEGMENT CREATION IMMEDIATE 
  PCTFREE 10 PCTUSED 40 INITRANS 1 MAXTRANS 255 
 NOCOMPRESS LOGGING
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "USERS" 
 LOB ("PasswordHash") STORE AS SECUREFILE (
  TABLESPACE "USERS" ENABLE STORAGE IN ROW CHUNK 8192
  NOCACHE LOGGING  NOCOMPRESS  KEEP_DUPLICATES 
  STORAGE(INITIAL 106496 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)) 
 LOB ("SecurityStamp") STORE AS SECUREFILE (
  TABLESPACE "USERS" ENABLE STORAGE IN ROW CHUNK 8192
  NOCACHE LOGGING  NOCOMPRESS  KEEP_DUPLICATES 
  STORAGE(INITIAL 106496 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)) 
 LOB ("PhoneNumber") STORE AS SECUREFILE (
  TABLESPACE "USERS" ENABLE STORAGE IN ROW CHUNK 8192
  NOCACHE LOGGING  NOCOMPRESS  KEEP_DUPLICATES 
  STORAGE(INITIAL 106496 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)) ;
--------------------------------------------------------
--  DDL for Table BOLogSystem
--------------------------------------------------------

  CREATE TABLE "DART"."BOLogSystem" 
   (	"IdBOLogSystem" NUMBER, 
	"AspNetUsersId" VARCHAR2(128 BYTE) DEFAULT NULL, 
	"ActionType" VARCHAR2(30 BYTE) DEFAULT NULL, 
	"IP" VARCHAR2(50 BYTE) DEFAULT NULL, 
	"ExecutionDate" TIMESTAMP (3), 
	"Entity" VARCHAR2(100 BYTE) DEFAULT NULL, 
	"OldEntity" CLOB, 
	"NewEntity" CLOB, 
	"ScopeIdentifier" VARCHAR2(50 BYTE)
   ) SEGMENT CREATION IMMEDIATE 
  PCTFREE 10 PCTUSED 40 INITRANS 1 MAXTRANS 255 
 NOCOMPRESS LOGGING
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "USERS" 
 LOB ("OldEntity") STORE AS SECUREFILE (
  TABLESPACE "USERS" ENABLE STORAGE IN ROW CHUNK 8192
  NOCACHE LOGGING  NOCOMPRESS  KEEP_DUPLICATES 
  STORAGE(INITIAL 106496 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)) 
 LOB ("NewEntity") STORE AS SECUREFILE (
  TABLESPACE "USERS" ENABLE STORAGE IN ROW CHUNK 8192
  NOCACHE LOGGING  NOCOMPRESS  KEEP_DUPLICATES 
  STORAGE(INITIAL 106496 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)) ;
--------------------------------------------------------
--  DDL for Table Company
--------------------------------------------------------

  CREATE TABLE "DART"."Company" 
   (	"CompanyId" NUMBER, 
	"CompanyName" VARCHAR2(500 BYTE), 
	"Initials" VARCHAR2(2 BYTE), 
	"TradingName" VARCHAR2(500 BYTE), 
	"CNPJ" VARCHAR2(500 BYTE), 
	"AddressZipCode" NUMBER, 
	"Address" VARCHAR2(500 BYTE), 
	"AddressNumber" NUMBER(10,0), 
	"AddressComplement" VARCHAR2(500 BYTE), 
	"AddressNeighborhood" VARCHAR2(500 BYTE), 
	"AddressState" VARCHAR2(500 BYTE), 
	"AddressCity" VARCHAR2(500 BYTE), 
	"PhoneNumber" VARCHAR2(15 BYTE), 
	"CompanyType" NUMBER(10,0), 
	"Disabled" NUMBER(1,0)
   ) SEGMENT CREATION IMMEDIATE 
  PCTFREE 10 PCTUSED 40 INITRANS 1 MAXTRANS 255 
 NOCOMPRESS LOGGING
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "USERS" ;
--------------------------------------------------------
--  DDL for Table EMPRESA
--------------------------------------------------------

  CREATE TABLE "DART"."EMPRESA" 
   (	"CODEMP" NUMBER(22,0), 
	"NOMEFANTASIA" VARCHAR2(40 BYTE), 
	"RAZAOSOCIAL" VARCHAR2(40 BYTE), 
	"CODEND" NUMBER(22,0), 
	"NUMEND" NUMBER(6,0), 
	"COMPLEMENTO" VARCHAR2(10 BYTE), 
	"CODBAI" NUMBER(22,0), 
	"CODCID" NUMBER(22,0), 
	"CEP" VARCHAR2(8 BYTE), 
	"TELEFONE" VARCHAR2(13 BYTE), 
	"CGC" VARCHAR2(14 BYTE)
   ) SEGMENT CREATION IMMEDIATE 
  PCTFREE 10 PCTUSED 40 INITRANS 1 MAXTRANS 255 
 NOCOMPRESS LOGGING
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "USERS" ;
--------------------------------------------------------
--  DDL for Table LOG_SYNCHRONIZATION
--------------------------------------------------------

  CREATE TABLE "DART"."LOG_SYNCHRONIZATION" 
   (	"ID" NUMBER(15,0), 
	"IDENTIFICATION" VARCHAR2(100 BYTE), 
	"SYNCHRONIZATION_AT" DATE
   ) SEGMENT CREATION IMMEDIATE 
  PCTFREE 10 PCTUSED 40 INITRANS 1 MAXTRANS 255 
 NOCOMPRESS LOGGING
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "USERS" ;
--------------------------------------------------------
--  DDL for Table PerfilUsuario
--------------------------------------------------------

  CREATE TABLE "DART"."PerfilUsuario" 
   (	"PerfilUsuarioId" NUMBER, 
	"UsuarioId" VARCHAR2(128 BYTE), 
	"EmpresaId" NUMBER, 
	"Departamento" VARCHAR2(200 BYTE), 
	"Cargo" VARCHAR2(200 BYTE), 
	"DataNascimento" DATE
   ) SEGMENT CREATION IMMEDIATE 
  PCTFREE 10 PCTUSED 40 INITRANS 1 MAXTRANS 255 
 NOCOMPRESS LOGGING
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "USERS" ;
--------------------------------------------------------
--  DDL for Table PHILOMENA
--------------------------------------------------------

  CREATE TABLE "DART"."PHILOMENA" 
   (	"ID" NUMBER(3,0), 
	"USERID" NUMBER(3,0), 
	"TITLE" VARCHAR2(1000 BYTE), 
	"BODY" VARCHAR2(1000 BYTE)
   ) SEGMENT CREATION DEFERRED 
  PCTFREE 10 PCTUSED 40 INITRANS 1 MAXTRANS 255 
 NOCOMPRESS LOGGING
  TABLESPACE "USERS" ;
--------------------------------------------------------
--  DDL for Table UserCompany
--------------------------------------------------------

  CREATE TABLE "DART"."UserCompany" 
   (	"UserId" VARCHAR2(128 BYTE), 
	"CompanyId" NUMBER
   ) SEGMENT CREATION IMMEDIATE 
  PCTFREE 10 PCTUSED 40 INITRANS 1 MAXTRANS 255 
 NOCOMPRESS LOGGING
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "USERS" ;
REM INSERTING into DART."Application"
SET DEFINE OFF;
Insert into DART."Application" ("IdApplication","Name") values ('2','Api');
Insert into DART."Application" ("IdApplication","Name") values ('1','BackOffice');
REM INSERTING into DART."ApplicationLanguage"
SET DEFINE OFF;
Insert into DART."ApplicationLanguage" ("IdApplicationLanguage","CultureName","DisplayName","IsDisabled") values ('1','pt-BR','PortuguÃªs (Brasil)','0');
REM INSERTING into DART.APPLICATIONLOG
SET DEFINE OFF;
Insert into DART.APPLICATIONLOG (IDAPPLICATIONLOG,CREATED,LOGLEVEL,MESSAGE,IDAPPLICATION) values ('4',to_timestamp('30/10/19 12:41:50,673000000','DD/MM/RR HH24:MI:SSXFF'),'ERROR','The property ''CompanyId'' is part of the object''s key information and cannot be modified. ','1');
Insert into DART.APPLICATIONLOG (IDAPPLICATIONLOG,CREATED,LOGLEVEL,MESSAGE,IDAPPLICATION) values ('5',to_timestamp('30/10/19 12:50:17,401000000','DD/MM/RR HH24:MI:SSXFF'),'ERROR','An error occurred while updating the entries. See the inner exception for details.','2');
Insert into DART.APPLICATIONLOG (IDAPPLICATIONLOG,CREATED,LOGLEVEL,MESSAGE,IDAPPLICATION) values ('6',to_timestamp('31/10/19 13:59:39,680000000','DD/MM/RR HH24:MI:SSXFF'),'ERROR','The property ''EmpresaId'' is not a String or Byte array. Length can only be configured for String and Byte array properties.','1');
Insert into DART.APPLICATIONLOG (IDAPPLICATIONLOG,CREATED,LOGLEVEL,MESSAGE,IDAPPLICATION) values ('2',to_timestamp('29/10/19 15:34:05,758000000','DD/MM/RR HH24:MI:SSXFF'),'ERROR','Exceção do tipo ''System.Exception'' foi acionada.','1');
Insert into DART.APPLICATIONLOG (IDAPPLICATIONLOG,CREATED,LOGLEVEL,MESSAGE,IDAPPLICATION) values ('3',to_timestamp('29/10/19 15:35:06,033000000','DD/MM/RR HH24:MI:SSXFF'),'ERROR','Exceção do tipo ''System.Exception'' foi acionada.','1');
REM INSERTING into DART."ApplicationSession"
SET DEFINE OFF;
Insert into DART."ApplicationSession" ("IdApplicationSession","IdAspNetUsers","IdApplication","DataLogin","DataUltimaAcao","DataLogout","CompanyId") values ('58','9b94e2d8-cc77-4e0b-abdc-eaff0ae1e5cb','1',to_timestamp('29/10/19 09:38:34,000000000','DD/MM/RR HH24:MI:SSXFF'),to_timestamp('29/10/19 09:38:41,000000000','DD/MM/RR HH24:MI:SSXFF'),to_timestamp('29/10/19 09:38:41,000000000','DD/MM/RR HH24:MI:SSXFF'),'42');
Insert into DART."ApplicationSession" ("IdApplicationSession","IdAspNetUsers","IdApplication","DataLogin","DataUltimaAcao","DataLogout","CompanyId") values ('59','9b94e2d8-cc77-4e0b-abdc-eaff0ae1e5cb','1',to_timestamp('29/10/19 09:38:43,000000000','DD/MM/RR HH24:MI:SSXFF'),to_timestamp('30/10/19 09:23:47,000000000','DD/MM/RR HH24:MI:SSXFF'),to_timestamp('30/10/19 09:23:47,000000000','DD/MM/RR HH24:MI:SSXFF'),'42');
Insert into DART."ApplicationSession" ("IdApplicationSession","IdAspNetUsers","IdApplication","DataLogin","DataUltimaAcao","DataLogout","CompanyId") values ('64','9b94e2d8-cc77-4e0b-abdc-eaff0ae1e5cb','2',to_timestamp('30/10/19 09:48:24,000000000','DD/MM/RR HH24:MI:SSXFF'),to_timestamp('30/10/19 10:48:07,000000000','DD/MM/RR HH24:MI:SSXFF'),to_timestamp('30/10/19 10:48:07,000000000','DD/MM/RR HH24:MI:SSXFF'),'41');
Insert into DART."ApplicationSession" ("IdApplicationSession","IdAspNetUsers","IdApplication","DataLogin","DataUltimaAcao","DataLogout","CompanyId") values ('66','9b94e2d8-cc77-4e0b-abdc-eaff0ae1e5cb','1',to_timestamp('30/10/19 10:48:11,000000000','DD/MM/RR HH24:MI:SSXFF'),to_timestamp('30/10/19 10:48:11,000000000','DD/MM/RR HH24:MI:SSXFF'),null,'42');
Insert into DART."ApplicationSession" ("IdApplicationSession","IdAspNetUsers","IdApplication","DataLogin","DataUltimaAcao","DataLogout","CompanyId") values ('56','9b94e2d8-cc77-4e0b-abdc-eaff0ae1e5cb','1',to_timestamp('29/10/19 09:24:07,000000000','DD/MM/RR HH24:MI:SSXFF'),to_timestamp('29/10/19 09:24:15,000000000','DD/MM/RR HH24:MI:SSXFF'),to_timestamp('29/10/19 09:24:15,000000000','DD/MM/RR HH24:MI:SSXFF'),'41');
Insert into DART."ApplicationSession" ("IdApplicationSession","IdAspNetUsers","IdApplication","DataLogin","DataUltimaAcao","DataLogout","CompanyId") values ('57','9b94e2d8-cc77-4e0b-abdc-eaff0ae1e5cb','1',to_timestamp('29/10/19 09:24:17,000000000','DD/MM/RR HH24:MI:SSXFF'),to_timestamp('29/10/19 09:38:32,000000000','DD/MM/RR HH24:MI:SSXFF'),to_timestamp('29/10/19 09:38:32,000000000','DD/MM/RR HH24:MI:SSXFF'),'41');
Insert into DART."ApplicationSession" ("IdApplicationSession","IdAspNetUsers","IdApplication","DataLogin","DataUltimaAcao","DataLogout","CompanyId") values ('55','9b94e2d8-cc77-4e0b-abdc-eaff0ae1e5cb','1',to_timestamp('29/10/19 09:24:00,000000000','DD/MM/RR HH24:MI:SSXFF'),to_timestamp('29/10/19 09:24:04,000000000','DD/MM/RR HH24:MI:SSXFF'),to_timestamp('29/10/19 09:24:04,000000000','DD/MM/RR HH24:MI:SSXFF'),'41');
Insert into DART."ApplicationSession" ("IdApplicationSession","IdAspNetUsers","IdApplication","DataLogin","DataUltimaAcao","DataLogout","CompanyId") values ('67','9b94e2d8-cc77-4e0b-abdc-eaff0ae1e5cb','1',to_timestamp('30/10/19 14:25:36,000000000','DD/MM/RR HH24:MI:SSXFF'),to_timestamp('31/10/19 08:38:54,000000000','DD/MM/RR HH24:MI:SSXFF'),to_timestamp('31/10/19 08:38:54,000000000','DD/MM/RR HH24:MI:SSXFF'),'41');
Insert into DART."ApplicationSession" ("IdApplicationSession","IdAspNetUsers","IdApplication","DataLogin","DataUltimaAcao","DataLogout","CompanyId") values ('60','9b94e2d8-cc77-4e0b-abdc-eaff0ae1e5cb','1',to_timestamp('30/10/19 09:23:58,000000000','DD/MM/RR HH24:MI:SSXFF'),to_timestamp('30/10/19 09:24:08,000000000','DD/MM/RR HH24:MI:SSXFF'),to_timestamp('30/10/19 09:24:08,000000000','DD/MM/RR HH24:MI:SSXFF'),'42');
Insert into DART."ApplicationSession" ("IdApplicationSession","IdAspNetUsers","IdApplication","DataLogin","DataUltimaAcao","DataLogout","CompanyId") values ('61','9b94e2d8-cc77-4e0b-abdc-eaff0ae1e5cb','1',to_timestamp('30/10/19 09:24:09,000000000','DD/MM/RR HH24:MI:SSXFF'),to_timestamp('30/10/19 09:24:31,000000000','DD/MM/RR HH24:MI:SSXFF'),to_timestamp('30/10/19 09:24:31,000000000','DD/MM/RR HH24:MI:SSXFF'),'42');
Insert into DART."ApplicationSession" ("IdApplicationSession","IdAspNetUsers","IdApplication","DataLogin","DataUltimaAcao","DataLogout","CompanyId") values ('62','9b94e2d8-cc77-4e0b-abdc-eaff0ae1e5cb','1',to_timestamp('30/10/19 09:24:32,000000000','DD/MM/RR HH24:MI:SSXFF'),to_timestamp('30/10/19 09:24:32,000000000','DD/MM/RR HH24:MI:SSXFF'),null,'41');
Insert into DART."ApplicationSession" ("IdApplicationSession","IdAspNetUsers","IdApplication","DataLogin","DataUltimaAcao","DataLogout","CompanyId") values ('68','9b94e2d8-cc77-4e0b-abdc-eaff0ae1e5cb','1',to_timestamp('31/10/19 08:39:07,000000000','DD/MM/RR HH24:MI:SSXFF'),to_timestamp('31/10/19 08:39:56,000000000','DD/MM/RR HH24:MI:SSXFF'),to_timestamp('31/10/19 08:39:56,000000000','DD/MM/RR HH24:MI:SSXFF'),'41');
Insert into DART."ApplicationSession" ("IdApplicationSession","IdAspNetUsers","IdApplication","DataLogin","DataUltimaAcao","DataLogout","CompanyId") values ('69','9b94e2d8-cc77-4e0b-abdc-eaff0ae1e5cb','1',to_timestamp('31/10/19 08:40:02,000000000','DD/MM/RR HH24:MI:SSXFF'),to_timestamp('31/10/19 08:40:33,000000000','DD/MM/RR HH24:MI:SSXFF'),to_timestamp('31/10/19 08:40:33,000000000','DD/MM/RR HH24:MI:SSXFF'),'41');
Insert into DART."ApplicationSession" ("IdApplicationSession","IdAspNetUsers","IdApplication","DataLogin","DataUltimaAcao","DataLogout","CompanyId") values ('70','9b94e2d8-cc77-4e0b-abdc-eaff0ae1e5cb','1',to_timestamp('31/10/19 08:41:08,000000000','DD/MM/RR HH24:MI:SSXFF'),to_timestamp('31/10/19 08:41:08,000000000','DD/MM/RR HH24:MI:SSXFF'),null,'41');
REM INSERTING into DART."AspNetPermissions"
SET DEFINE OFF;
Insert into DART."AspNetPermissions" ("Id","ApplicationId","Name") values ('64951bdc-d606-4a8b-9102-a22b33e71925','1','ApplicationLogList');
Insert into DART."AspNetPermissions" ("Id","ApplicationId","Name") values ('5eb6865b-ed52-4d89-bfa0-e8c6a360e8b9','1','RoleList');
Insert into DART."AspNetPermissions" ("Id","ApplicationId","Name") values ('22c22e82-35b6-4df1-b60f-560785c1e705','1','RoleCreate');
Insert into DART."AspNetPermissions" ("Id","ApplicationId","Name") values ('9b2742ac-441c-40fe-964b-d52aff5b90bf','1','RoleEdit');
Insert into DART."AspNetPermissions" ("Id","ApplicationId","Name") values ('417db35c-2247-4e71-ad2b-dbf98c3eb494','1','RoleDelete');
Insert into DART."AspNetPermissions" ("Id","ApplicationId","Name") values ('440151f0-3539-4fe8-9db6-e37055cdf51e','1','BOLogSystemList');
Insert into DART."AspNetPermissions" ("Id","ApplicationId","Name") values ('1893fe2c-843b-486f-aa02-fa03a9b4a700','1','BOAccountList');
Insert into DART."AspNetPermissions" ("Id","ApplicationId","Name") values ('f9293337-ea5b-41b8-9832-8e8e08ce401a','1','BOAccountCreate');
Insert into DART."AspNetPermissions" ("Id","ApplicationId","Name") values ('2a0e9138-47ad-4290-baa2-7e3ce59db87d','1','BOAccountEdit');
Insert into DART."AspNetPermissions" ("Id","ApplicationId","Name") values ('9a186df5-e22f-47ae-832d-aed2c8c8b8c0','1','BOAccountDelete');
Insert into DART."AspNetPermissions" ("Id","ApplicationId","Name") values ('cb6eba3c-d20c-4011-bebb-49e649c5ffce','1','UserAppLogin');
Insert into DART."AspNetPermissions" ("Id","ApplicationId","Name") values ('2ab8c1c5-29e2-4abc-9bd2-3839bc40c916','2','ApplicationLogList');
Insert into DART."AspNetPermissions" ("Id","ApplicationId","Name") values ('88aeaa71-da11-4a16-8ba5-e42f1f43fdbe','2','RoleList');
Insert into DART."AspNetPermissions" ("Id","ApplicationId","Name") values ('b80d929f-2502-4eca-b659-10e9c8062f17','2','RoleCreate');
Insert into DART."AspNetPermissions" ("Id","ApplicationId","Name") values ('da0a8d30-0714-498b-b7e0-7fcaced90ebe','2','RoleEdit');
Insert into DART."AspNetPermissions" ("Id","ApplicationId","Name") values ('6bd6e9ff-8aa0-4d2f-8e9f-f9e206ada378','2','RoleDelete');
Insert into DART."AspNetPermissions" ("Id","ApplicationId","Name") values ('6fa009b2-0fbe-4ac0-996c-56fa446a2f8f','2','BOLogSystemList');
Insert into DART."AspNetPermissions" ("Id","ApplicationId","Name") values ('6abaec7f-ba72-49c9-89ca-affa634c9e65','2','BOAccountList');
Insert into DART."AspNetPermissions" ("Id","ApplicationId","Name") values ('3d7f455f-20c0-4f6e-99ab-604ce96d3c2e','2','BOAccountCreate');
Insert into DART."AspNetPermissions" ("Id","ApplicationId","Name") values ('27a5cfcc-7a32-4008-92b0-2184b731b00e','2','BOAccountEdit');
Insert into DART."AspNetPermissions" ("Id","ApplicationId","Name") values ('1114778c-f95a-4dd5-a81b-cf081bffc408','2','BOAccountDelete');
REM INSERTING into DART."AspNetRolePermissions"
SET DEFINE OFF;
Insert into DART."AspNetRolePermissions" ("RoleId","PermissionId") values ('c94d3fc4-5f0e-4040-b542-c253990efb4b','cb6eba3c-d20c-4011-bebb-49e649c5ffce');
Insert into DART."AspNetRolePermissions" ("RoleId","PermissionId") values ('b8353aba-9ff4-406b-8e36-0a95addfc3d9','cb6eba3c-d20c-4011-bebb-49e649c5ffce');
Insert into DART."AspNetRolePermissions" ("RoleId","PermissionId") values ('c94d3fc4-5f0e-4040-b542-c253990efb4b','1893fe2c-843b-486f-aa02-fa03a9b4a700');
Insert into DART."AspNetRolePermissions" ("RoleId","PermissionId") values ('c94d3fc4-5f0e-4040-b542-c253990efb4b','22c22e82-35b6-4df1-b60f-560785c1e705');
Insert into DART."AspNetRolePermissions" ("RoleId","PermissionId") values ('c94d3fc4-5f0e-4040-b542-c253990efb4b','2a0e9138-47ad-4290-baa2-7e3ce59db87d');
Insert into DART."AspNetRolePermissions" ("RoleId","PermissionId") values ('c94d3fc4-5f0e-4040-b542-c253990efb4b','417db35c-2247-4e71-ad2b-dbf98c3eb494');
Insert into DART."AspNetRolePermissions" ("RoleId","PermissionId") values ('c94d3fc4-5f0e-4040-b542-c253990efb4b','440151f0-3539-4fe8-9db6-e37055cdf51e');
Insert into DART."AspNetRolePermissions" ("RoleId","PermissionId") values ('c94d3fc4-5f0e-4040-b542-c253990efb4b','5eb6865b-ed52-4d89-bfa0-e8c6a360e8b9');
Insert into DART."AspNetRolePermissions" ("RoleId","PermissionId") values ('c94d3fc4-5f0e-4040-b542-c253990efb4b','64951bdc-d606-4a8b-9102-a22b33e71925');
Insert into DART."AspNetRolePermissions" ("RoleId","PermissionId") values ('c94d3fc4-5f0e-4040-b542-c253990efb4b','9a186df5-e22f-47ae-832d-aed2c8c8b8c0');
Insert into DART."AspNetRolePermissions" ("RoleId","PermissionId") values ('c94d3fc4-5f0e-4040-b542-c253990efb4b','9b2742ac-441c-40fe-964b-d52aff5b90bf');
Insert into DART."AspNetRolePermissions" ("RoleId","PermissionId") values ('c94d3fc4-5f0e-4040-b542-c253990efb4b','f9293337-ea5b-41b8-9832-8e8e08ce401a');
REM INSERTING into DART."AspNetRoles"
SET DEFINE OFF;
Insert into DART."AspNetRoles" ("Id","Name","ApplicationId") values ('b8353aba-9ff4-406b-8e36-0a95addfc3d9','App','1');
Insert into DART."AspNetRoles" ("Id","Name","ApplicationId") values ('c94d3fc4-5f0e-4040-b542-c253990efb4b','ADM2','1');
REM INSERTING into DART."AspNetUserClaims"
SET DEFINE OFF;
REM INSERTING into DART."AspNetUserLogins"
SET DEFINE OFF;
REM INSERTING into DART."AspNetUserPermissions"
SET DEFINE OFF;
Insert into DART."AspNetUserPermissions" ("UserId","PermissionId") values ('9b94e2d8-cc77-4e0b-abdc-eaff0ae1e5cb','1893fe2c-843b-486f-aa02-fa03a9b4a700');
Insert into DART."AspNetUserPermissions" ("UserId","PermissionId") values ('9b94e2d8-cc77-4e0b-abdc-eaff0ae1e5cb','22c22e82-35b6-4df1-b60f-560785c1e705');
Insert into DART."AspNetUserPermissions" ("UserId","PermissionId") values ('9b94e2d8-cc77-4e0b-abdc-eaff0ae1e5cb','2a0e9138-47ad-4290-baa2-7e3ce59db87d');
Insert into DART."AspNetUserPermissions" ("UserId","PermissionId") values ('9b94e2d8-cc77-4e0b-abdc-eaff0ae1e5cb','417db35c-2247-4e71-ad2b-dbf98c3eb494');
Insert into DART."AspNetUserPermissions" ("UserId","PermissionId") values ('9b94e2d8-cc77-4e0b-abdc-eaff0ae1e5cb','440151f0-3539-4fe8-9db6-e37055cdf51e');
Insert into DART."AspNetUserPermissions" ("UserId","PermissionId") values ('9b94e2d8-cc77-4e0b-abdc-eaff0ae1e5cb','5eb6865b-ed52-4d89-bfa0-e8c6a360e8b9');
Insert into DART."AspNetUserPermissions" ("UserId","PermissionId") values ('9b94e2d8-cc77-4e0b-abdc-eaff0ae1e5cb','64951bdc-d606-4a8b-9102-a22b33e71925');
Insert into DART."AspNetUserPermissions" ("UserId","PermissionId") values ('9b94e2d8-cc77-4e0b-abdc-eaff0ae1e5cb','9a186df5-e22f-47ae-832d-aed2c8c8b8c0');
Insert into DART."AspNetUserPermissions" ("UserId","PermissionId") values ('9b94e2d8-cc77-4e0b-abdc-eaff0ae1e5cb','9b2742ac-441c-40fe-964b-d52aff5b90bf');
Insert into DART."AspNetUserPermissions" ("UserId","PermissionId") values ('9b94e2d8-cc77-4e0b-abdc-eaff0ae1e5cb','cb6eba3c-d20c-4011-bebb-49e649c5ffce');
Insert into DART."AspNetUserPermissions" ("UserId","PermissionId") values ('9b94e2d8-cc77-4e0b-abdc-eaff0ae1e5cb','f9293337-ea5b-41b8-9832-8e8e08ce401a');
REM INSERTING into DART."AspNetUserRoles"
SET DEFINE OFF;
REM INSERTING into DART."AspNetUsers"
SET DEFINE OFF;
Insert into DART."AspNetUsers" ("Id","ApplicationId","Email","EmailConfirmed","PhoneNumberConfirmed","TwoFactorEnabled","LockoutEndDateUtc","LockoutEnabled","AccessFailedCount","UserName","IdApplicationSession") values ('a0b1bda7-41ab-4731-872f-a1653ace2ca2','2','teste@dartdigital.com.br','0','0','0',null,'0','0','teste@dartdigital.com.br',null);
Insert into DART."AspNetUsers" ("Id","ApplicationId","Email","EmailConfirmed","PhoneNumberConfirmed","TwoFactorEnabled","LockoutEndDateUtc","LockoutEnabled","AccessFailedCount","UserName","IdApplicationSession") values ('9b94e2d8-cc77-4e0b-abdc-eaff0ae1e5cb','1','suporte@dartdigital.com.br','0','0','0',null,'0','0','suporte@dartdigital.com.br','70');
REM INSERTING into DART."BOLogSystem"
SET DEFINE OFF;
Insert into DART."BOLogSystem" ("IdBOLogSystem","AspNetUsersId","ActionType",IP,"ExecutionDate","Entity","ScopeIdentifier") values ('2','9b94e2d8-cc77-4e0b-abdc-eaff0ae1e5cb','Editar','::1',to_timestamp('28/10/19 15:11:58,000000000','DD/MM/RR HH24:MI:SSXFF'),'Company','1a4d946c-5b1a-48a4-b905-8829a7461962');
Insert into DART."BOLogSystem" ("IdBOLogSystem","AspNetUsersId","ActionType",IP,"ExecutionDate","Entity","ScopeIdentifier") values ('5','9b94e2d8-cc77-4e0b-abdc-eaff0ae1e5cb','Adicionar','::1',to_timestamp('29/10/19 17:21:58,000000000','DD/MM/RR HH24:MI:SSXFF'),'AspNetRoles','9b059537-1819-42da-90c5-4e0d554e7445');
Insert into DART."BOLogSystem" ("IdBOLogSystem","AspNetUsersId","ActionType",IP,"ExecutionDate","Entity","ScopeIdentifier") values ('7','9b94e2d8-cc77-4e0b-abdc-eaff0ae1e5cb','Adicionar','::1',to_timestamp('30/10/19 12:45:48,000000000','DD/MM/RR HH24:MI:SSXFF'),'Company','926cc0fc-9176-45d8-ab49-744cc6fae458');
Insert into DART."BOLogSystem" ("IdBOLogSystem","AspNetUsersId","ActionType",IP,"ExecutionDate","Entity","ScopeIdentifier") values ('1','9b94e2d8-cc77-4e0b-abdc-eaff0ae1e5cb','Editar','::1',to_timestamp('28/10/19 11:44:39,000000000','DD/MM/RR HH24:MI:SSXFF'),'AspNetRoles','21409b10-489a-4e96-a53b-c86957ff90b0');
Insert into DART."BOLogSystem" ("IdBOLogSystem","AspNetUsersId","ActionType",IP,"ExecutionDate","Entity","ScopeIdentifier") values ('4','9b94e2d8-cc77-4e0b-abdc-eaff0ae1e5cb','Editar','::1',to_timestamp('28/10/19 17:52:35,000000000','DD/MM/RR HH24:MI:SSXFF'),'AspNetRoles','dfcc5551-f104-4c28-ba93-2fc681193ad5');
Insert into DART."BOLogSystem" ("IdBOLogSystem","AspNetUsersId","ActionType",IP,"ExecutionDate","Entity","ScopeIdentifier") values ('6','9b94e2d8-cc77-4e0b-abdc-eaff0ae1e5cb','Editar','::1',to_timestamp('30/10/19 12:39:28,000000000','DD/MM/RR HH24:MI:SSXFF'),'Company','d225b06c-50c8-4b7a-98f9-246d588c6c0e');
Insert into DART."BOLogSystem" ("IdBOLogSystem","AspNetUsersId","ActionType",IP,"ExecutionDate","Entity","ScopeIdentifier") values ('3','9b94e2d8-cc77-4e0b-abdc-eaff0ae1e5cb','Editar','::1',to_timestamp('28/10/19 17:52:21,000000000','DD/MM/RR HH24:MI:SSXFF'),'AspNetUsers','7eef4f32-8f82-4ae3-86fb-b60a7a239a84');
REM INSERTING into DART."Company"
SET DEFINE OFF;
Insert into DART."Company" ("CompanyId","CompanyName","Initials","TradingName",CNPJ,"AddressZipCode","Address","AddressNumber","AddressComplement","AddressNeighborhood","AddressState","AddressCity","PhoneNumber","CompanyType","Disabled") values ('41','FW - Matriz - Campinas','FW','Matriz','08203386000140','14014014','Rua teste','123','Complemento','Bairo','Estado','Cidade','5516955165516','1','0');
Insert into DART."Company" ("CompanyId","CompanyName","Initials","TradingName",CNPJ,"AddressZipCode","Address","AddressNumber","AddressComplement","AddressNeighborhood","AddressState","AddressCity","PhoneNumber","CompanyType","Disabled") values ('42','Ribeirão Preto','RB','Ribeirão Preto Filial','08203386000140','14014014','Rua teste','123','Complemento','Bairo','Estado','Cidade','5516955165516','1','0');
Insert into DART."Company" ("CompanyId","CompanyName","Initials","TradingName",CNPJ,"AddressZipCode","Address","AddressNumber","AddressComplement","AddressNeighborhood","AddressState","AddressCity","PhoneNumber","CompanyType","Disabled") values ('43','Manaus','MN','Manaus','08203386000140','14014014','Rua teste','123','Complemento','Bairo','Estado','Cidade','5516955165516','1','0');
REM INSERTING into DART.EMPRESA
SET DEFINE OFF;
Insert into DART.EMPRESA (CODEMP,NOMEFANTASIA,RAZAOSOCIAL,CODEND,NUMEND,COMPLEMENTO,CODBAI,CODCID,CEP,TELEFONE,CGC) values ('11','AUDAZ PERFUMARIA','AUDAZ PERFUMARIA','7','123',null,'2','5358','38402355',null,'26314062000161');
Insert into DART.EMPRESA (CODEMP,NOMEFANTASIA,RAZAOSOCIAL,CODEND,NUMEND,COMPLEMENTO,CODBAI,CODCID,CEP,TELEFONE,CGC) values ('1','Audaz Distribuidora','Audaz Comercio Ltda','7','369',null,'5','5358','38400000','003432390700','08897417000100');
Insert into DART.EMPRESA (CODEMP,NOMEFANTASIA,RAZAOSOCIAL,CODEND,NUMEND,COMPLEMENTO,CODBAI,CODCID,CEP,TELEFONE,CGC) values ('2','VANIA','VANIA A SOUSA','0','1250',null,'0','952',null,null,'00000000000000');
Insert into DART.EMPRESA (CODEMP,NOMEFANTASIA,RAZAOSOCIAL,CODEND,NUMEND,COMPLEMENTO,CODBAI,CODCID,CEP,TELEFONE,CGC) values ('3','Audaz Distribuidora Filial','Audaz Distribuidora Filial','8','524',null,'1','5358',null,null,'26314062000161');
Insert into DART.EMPRESA (CODEMP,NOMEFANTASIA,RAZAOSOCIAL,CODEND,NUMEND,COMPLEMENTO,CODBAI,CODCID,CEP,TELEFONE,CGC) values ('20','AUDAZ PERFUMARIA ','AUDAZ PERFUMARIA','0',null,null,'0','5358',null,null,'55615134000109');
Insert into DART.EMPRESA (CODEMP,NOMEFANTASIA,RAZAOSOCIAL,CODEND,NUMEND,COMPLEMENTO,CODBAI,CODCID,CEP,TELEFONE,CGC) values ('10','AUDAZ PERFUMARIA','AUDAZ PERFUMARIA','2','162',null,'1','5358','13154185',null,'55615134000109');
Insert into DART.EMPRESA (CODEMP,NOMEFANTASIA,RAZAOSOCIAL,CODEND,NUMEND,COMPLEMENTO,CODBAI,CODCID,CEP,TELEFONE,CGC) values ('30','EMPORIO MARTINS ','EMPORIO MARTINS LTDA','8','132',null,'4','5358','38400058','0034033018300','26314062000161');
Insert into DART.EMPRESA (CODEMP,NOMEFANTASIA,RAZAOSOCIAL,CODEND,NUMEND,COMPLEMENTO,CODBAI,CODCID,CEP,TELEFONE,CGC) values ('31','MARTINS ENTREGAS','MARTINS ENTREGAS LTDA','8','569',null,'3','5358','38400058',null,'26314062000161');
Insert into DART.EMPRESA (CODEMP,NOMEFANTASIA,RAZAOSOCIAL,CODEND,NUMEND,COMPLEMENTO,CODBAI,CODCID,CEP,TELEFONE,CGC) values ('11','AUDAZ PERFUMARIA','AUDAZ PERFUMARIA','7','123',null,'2','5358','38402355',null,'26314062000161');
Insert into DART.EMPRESA (CODEMP,NOMEFANTASIA,RAZAOSOCIAL,CODEND,NUMEND,COMPLEMENTO,CODBAI,CODCID,CEP,TELEFONE,CGC) values ('1','Audaz Distribuidora','Audaz Comercio Ltda','7','369',null,'5','5358','38400000','003432390700','08897417000100');
Insert into DART.EMPRESA (CODEMP,NOMEFANTASIA,RAZAOSOCIAL,CODEND,NUMEND,COMPLEMENTO,CODBAI,CODCID,CEP,TELEFONE,CGC) values ('2','VANIA','VANIA A SOUSA','0','1250',null,'0','952',null,null,'00000000000000');
Insert into DART.EMPRESA (CODEMP,NOMEFANTASIA,RAZAOSOCIAL,CODEND,NUMEND,COMPLEMENTO,CODBAI,CODCID,CEP,TELEFONE,CGC) values ('3','Audaz Distribuidora Filial','Audaz Distribuidora Filial','8','524',null,'1','5358',null,null,'26314062000161');
Insert into DART.EMPRESA (CODEMP,NOMEFANTASIA,RAZAOSOCIAL,CODEND,NUMEND,COMPLEMENTO,CODBAI,CODCID,CEP,TELEFONE,CGC) values ('20','AUDAZ PERFUMARIA ','AUDAZ PERFUMARIA','0',null,null,'0','5358',null,null,'55615134000109');
Insert into DART.EMPRESA (CODEMP,NOMEFANTASIA,RAZAOSOCIAL,CODEND,NUMEND,COMPLEMENTO,CODBAI,CODCID,CEP,TELEFONE,CGC) values ('10','AUDAZ PERFUMARIA','AUDAZ PERFUMARIA','2','162',null,'1','5358','13154185',null,'55615134000109');
Insert into DART.EMPRESA (CODEMP,NOMEFANTASIA,RAZAOSOCIAL,CODEND,NUMEND,COMPLEMENTO,CODBAI,CODCID,CEP,TELEFONE,CGC) values ('30','EMPORIO MARTINS ','EMPORIO MARTINS LTDA','8','132',null,'4','5358','38400058','0034033018300','26314062000161');
Insert into DART.EMPRESA (CODEMP,NOMEFANTASIA,RAZAOSOCIAL,CODEND,NUMEND,COMPLEMENTO,CODBAI,CODCID,CEP,TELEFONE,CGC) values ('31','MARTINS ENTREGAS','MARTINS ENTREGAS LTDA','8','569',null,'3','5358','38400058',null,'26314062000161');
Insert into DART.EMPRESA (CODEMP,NOMEFANTASIA,RAZAOSOCIAL,CODEND,NUMEND,COMPLEMENTO,CODBAI,CODCID,CEP,TELEFONE,CGC) values ('11','AUDAZ PERFUMARIA','AUDAZ PERFUMARIA','7','123',null,'2','5358','38402355',null,'26314062000161');
Insert into DART.EMPRESA (CODEMP,NOMEFANTASIA,RAZAOSOCIAL,CODEND,NUMEND,COMPLEMENTO,CODBAI,CODCID,CEP,TELEFONE,CGC) values ('1','Audaz Distribuidora','Audaz Comercio Ltda','7','369',null,'5','5358','38400000','003432390700','08897417000100');
Insert into DART.EMPRESA (CODEMP,NOMEFANTASIA,RAZAOSOCIAL,CODEND,NUMEND,COMPLEMENTO,CODBAI,CODCID,CEP,TELEFONE,CGC) values ('2','VANIA','VANIA A SOUSA','0','1250',null,'0','952',null,null,'00000000000000');
Insert into DART.EMPRESA (CODEMP,NOMEFANTASIA,RAZAOSOCIAL,CODEND,NUMEND,COMPLEMENTO,CODBAI,CODCID,CEP,TELEFONE,CGC) values ('3','Audaz Distribuidora Filial','Audaz Distribuidora Filial','8','524',null,'1','5358',null,null,'26314062000161');
Insert into DART.EMPRESA (CODEMP,NOMEFANTASIA,RAZAOSOCIAL,CODEND,NUMEND,COMPLEMENTO,CODBAI,CODCID,CEP,TELEFONE,CGC) values ('20','AUDAZ PERFUMARIA ','AUDAZ PERFUMARIA','0',null,null,'0','5358',null,null,'55615134000109');
Insert into DART.EMPRESA (CODEMP,NOMEFANTASIA,RAZAOSOCIAL,CODEND,NUMEND,COMPLEMENTO,CODBAI,CODCID,CEP,TELEFONE,CGC) values ('10','AUDAZ PERFUMARIA','AUDAZ PERFUMARIA','2','162',null,'1','5358','13154185',null,'55615134000109');
Insert into DART.EMPRESA (CODEMP,NOMEFANTASIA,RAZAOSOCIAL,CODEND,NUMEND,COMPLEMENTO,CODBAI,CODCID,CEP,TELEFONE,CGC) values ('30','EMPORIO MARTINS ','EMPORIO MARTINS LTDA','8','132',null,'4','5358','38400058','0034033018300','26314062000161');
Insert into DART.EMPRESA (CODEMP,NOMEFANTASIA,RAZAOSOCIAL,CODEND,NUMEND,COMPLEMENTO,CODBAI,CODCID,CEP,TELEFONE,CGC) values ('31','MARTINS ENTREGAS','MARTINS ENTREGAS LTDA','8','569',null,'3','5358','38400058',null,'26314062000161');
Insert into DART.EMPRESA (CODEMP,NOMEFANTASIA,RAZAOSOCIAL,CODEND,NUMEND,COMPLEMENTO,CODBAI,CODCID,CEP,TELEFONE,CGC) values ('11','AUDAZ PERFUMARIA','AUDAZ PERFUMARIA','7','123',null,'2','5358','38402355',null,'26314062000161');
Insert into DART.EMPRESA (CODEMP,NOMEFANTASIA,RAZAOSOCIAL,CODEND,NUMEND,COMPLEMENTO,CODBAI,CODCID,CEP,TELEFONE,CGC) values ('1','Audaz Distribuidora','Audaz Comercio Ltda','7','369',null,'5','5358','38400000','003432390700','08897417000100');
Insert into DART.EMPRESA (CODEMP,NOMEFANTASIA,RAZAOSOCIAL,CODEND,NUMEND,COMPLEMENTO,CODBAI,CODCID,CEP,TELEFONE,CGC) values ('2','VANIA','VANIA A SOUSA','0','1250',null,'0','952',null,null,'00000000000000');
Insert into DART.EMPRESA (CODEMP,NOMEFANTASIA,RAZAOSOCIAL,CODEND,NUMEND,COMPLEMENTO,CODBAI,CODCID,CEP,TELEFONE,CGC) values ('3','Audaz Distribuidora Filial','Audaz Distribuidora Filial','8','524',null,'1','5358',null,null,'26314062000161');
Insert into DART.EMPRESA (CODEMP,NOMEFANTASIA,RAZAOSOCIAL,CODEND,NUMEND,COMPLEMENTO,CODBAI,CODCID,CEP,TELEFONE,CGC) values ('20','AUDAZ PERFUMARIA ','AUDAZ PERFUMARIA','0',null,null,'0','5358',null,null,'55615134000109');
Insert into DART.EMPRESA (CODEMP,NOMEFANTASIA,RAZAOSOCIAL,CODEND,NUMEND,COMPLEMENTO,CODBAI,CODCID,CEP,TELEFONE,CGC) values ('10','AUDAZ PERFUMARIA','AUDAZ PERFUMARIA','2','162',null,'1','5358','13154185',null,'55615134000109');
Insert into DART.EMPRESA (CODEMP,NOMEFANTASIA,RAZAOSOCIAL,CODEND,NUMEND,COMPLEMENTO,CODBAI,CODCID,CEP,TELEFONE,CGC) values ('30','EMPORIO MARTINS ','EMPORIO MARTINS LTDA','8','132',null,'4','5358','38400058','0034033018300','26314062000161');
Insert into DART.EMPRESA (CODEMP,NOMEFANTASIA,RAZAOSOCIAL,CODEND,NUMEND,COMPLEMENTO,CODBAI,CODCID,CEP,TELEFONE,CGC) values ('31','MARTINS ENTREGAS','MARTINS ENTREGAS LTDA','8','569',null,'3','5358','38400058',null,'26314062000161');
REM INSERTING into DART.LOG_SYNCHRONIZATION
SET DEFINE OFF;
Insert into DART.LOG_SYNCHRONIZATION (ID,IDENTIFICATION,SYNCHRONIZATION_AT) values ('5','empresa',to_date('22/10/19','DD/MM/RR'));
Insert into DART.LOG_SYNCHRONIZATION (ID,IDENTIFICATION,SYNCHRONIZATION_AT) values ('6','empresa',to_date('22/10/19','DD/MM/RR'));
Insert into DART.LOG_SYNCHRONIZATION (ID,IDENTIFICATION,SYNCHRONIZATION_AT) values ('3','empresa',to_date('22/10/19','DD/MM/RR'));
Insert into DART.LOG_SYNCHRONIZATION (ID,IDENTIFICATION,SYNCHRONIZATION_AT) values ('4','empresa',to_date('22/10/19','DD/MM/RR'));
Insert into DART.LOG_SYNCHRONIZATION (ID,IDENTIFICATION,SYNCHRONIZATION_AT) values ('2','teste',to_date('17/10/19','DD/MM/RR'));
REM INSERTING into DART."PerfilUsuario"
SET DEFINE OFF;
Insert into DART."PerfilUsuario" ("PerfilUsuarioId","UsuarioId","EmpresaId","Departamento","Cargo","DataNascimento") values ('1','9b94e2d8-cc77-4e0b-abdc-eaff0ae1e5cb','41','Garantia','Técnico',to_date('10/09/89','DD/MM/RR'));
REM INSERTING into DART.PHILOMENA
SET DEFINE OFF;
REM INSERTING into DART."UserCompany"
SET DEFINE OFF;
Insert into DART."UserCompany" ("UserId","CompanyId") values ('9b94e2d8-cc77-4e0b-abdc-eaff0ae1e5cb','41');
Insert into DART."UserCompany" ("UserId","CompanyId") values ('9b94e2d8-cc77-4e0b-abdc-eaff0ae1e5cb','42');
--------------------------------------------------------
--  DDL for Index UserId
--------------------------------------------------------

  CREATE INDEX "DART"."UserId" ON "DART"."AspNetUserClaims" ("UserId") 
  PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  TABLESPACE "USERS" ;
--------------------------------------------------------
--  DDL for Index IDENTITYROLE_USERS
--------------------------------------------------------

  CREATE INDEX "DART"."IDENTITYROLE_USERS" ON "DART"."AspNetUserRoles" ("RoleId") 
  PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "USERS" ;
--------------------------------------------------------
--  DDL for Index APPLICATIONUSER_LOGINS
--------------------------------------------------------

  CREATE INDEX "DART"."APPLICATIONUSER_LOGINS" ON "DART"."AspNetUserLogins" ("UserId") 
  PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  TABLESPACE "USERS" ;
--------------------------------------------------------
--  DDL for Trigger APPLICATIONLANGUAGE_SEQ_TR
--------------------------------------------------------

  CREATE OR REPLACE EDITIONABLE TRIGGER "DART"."APPLICATIONLANGUAGE_SEQ_TR" 
 BEFORE INSERT ON "ApplicationLanguage" FOR EACH ROW
  WHEN (NEW."IdApplicationLanguage" IS NULL) BEGIN
 SELECT ApplicationLanguage_seq.NEXTVAL INTO :NEW."IdApplicationLanguage" FROM DUAL;
END;
/
ALTER TRIGGER "DART"."APPLICATIONLANGUAGE_SEQ_TR" ENABLE;
--------------------------------------------------------
--  DDL for Trigger APPLICATIONLOG_SEQ_TR
--------------------------------------------------------

  CREATE OR REPLACE EDITIONABLE TRIGGER "DART"."APPLICATIONLOG_SEQ_TR" 
 BEFORE INSERT ON "APPLICATIONLOG" FOR EACH ROW
  WHEN (NEW."IDAPPLICATIONLOG" IS NULL) BEGIN
 SELECT ApplicationLog_seq.NEXTVAL INTO :NEW."IDAPPLICATIONLOG" FROM DUAL;
END;
/
ALTER TRIGGER "DART"."APPLICATIONLOG_SEQ_TR" ENABLE;
--------------------------------------------------------
--  DDL for Trigger APPLICATIONSESSION_SEQ_TR
--------------------------------------------------------

  CREATE OR REPLACE EDITIONABLE TRIGGER "DART"."APPLICATIONSESSION_SEQ_TR" 
 BEFORE INSERT ON "ApplicationSession" FOR EACH ROW
  WHEN (NEW."IdApplicationSession" IS NULL) BEGIN
 SELECT ApplicationSession_seq.NEXTVAL INTO :NEW."IdApplicationSession" FROM DUAL;
END;
/
ALTER TRIGGER "DART"."APPLICATIONSESSION_SEQ_TR" ENABLE;
--------------------------------------------------------
--  DDL for Trigger ASPNETUSERCLAIMS_SEQ_TR
--------------------------------------------------------

  CREATE OR REPLACE EDITIONABLE TRIGGER "DART"."ASPNETUSERCLAIMS_SEQ_TR" 
 BEFORE INSERT ON "AspNetUserClaims" FOR EACH ROW
  WHEN (NEW."Id" IS NULL) BEGIN
 SELECT AspNetUserClaims_seq.NEXTVAL INTO :NEW."Id" FROM DUAL;
END;

/
ALTER TRIGGER "DART"."ASPNETUSERCLAIMS_SEQ_TR" ENABLE;
--------------------------------------------------------
--  DDL for Trigger BOLOGSYSTEM_SEQ_TR
--------------------------------------------------------

  CREATE OR REPLACE EDITIONABLE TRIGGER "DART"."BOLOGSYSTEM_SEQ_TR" 
 BEFORE INSERT ON "BOLogSystem" FOR EACH ROW
  WHEN (NEW."IdBOLogSystem" IS NULL) BEGIN
 SELECT BOLogSystem_seq.NEXTVAL INTO :NEW."IdBOLogSystem" FROM DUAL;
END;
/
ALTER TRIGGER "DART"."BOLOGSYSTEM_SEQ_TR" ENABLE;
--------------------------------------------------------
--  DDL for Trigger COMPANY_SEQ_TR
--------------------------------------------------------

  CREATE OR REPLACE EDITIONABLE TRIGGER "DART"."COMPANY_SEQ_TR" 
 BEFORE INSERT ON "Company" FOR EACH ROW
  WHEN (NEW."CompanyId" IS NULL) BEGIN
 SELECT Company_seq.NEXTVAL INTO :NEW."CompanyId" FROM DUAL;
END;
/
ALTER TRIGGER "DART"."COMPANY_SEQ_TR" ENABLE;
--------------------------------------------------------
--  DDL for Trigger PERFILUSUARIO_SEQ_TR
--------------------------------------------------------

  CREATE OR REPLACE EDITIONABLE TRIGGER "DART"."PERFILUSUARIO_SEQ_TR" 
 BEFORE INSERT ON "PerfilUsuario" FOR EACH ROW
  WHEN (NEW."PerfilUsuarioId" IS NULL) BEGIN
 SELECT PerfilUsuario_seq.NEXTVAL INTO :NEW."PerfilUsuarioId" FROM DUAL;
END;
/
ALTER TRIGGER "DART"."PERFILUSUARIO_SEQ_TR" ENABLE;
--------------------------------------------------------
--  Constraints for Table AspNetUserLogins
--------------------------------------------------------

  ALTER TABLE "DART"."AspNetUserLogins" ADD PRIMARY KEY ("LoginProvider", "ProviderKey", "UserId")
  USING INDEX PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  TABLESPACE "USERS"  ENABLE;
  ALTER TABLE "DART"."AspNetUserLogins" MODIFY ("UserId" NOT NULL ENABLE);
  ALTER TABLE "DART"."AspNetUserLogins" MODIFY ("ProviderKey" NOT NULL ENABLE);
  ALTER TABLE "DART"."AspNetUserLogins" MODIFY ("LoginProvider" NOT NULL ENABLE);
--------------------------------------------------------
--  Constraints for Table AspNetRolePermissions
--------------------------------------------------------

  ALTER TABLE "DART"."AspNetRolePermissions" MODIFY ("PermissionId" NOT NULL ENABLE);
  ALTER TABLE "DART"."AspNetRolePermissions" MODIFY ("RoleId" NOT NULL ENABLE);
--------------------------------------------------------
--  Constraints for Table Application
--------------------------------------------------------

  ALTER TABLE "DART"."Application" ADD UNIQUE ("Name")
  USING INDEX PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "USERS"  ENABLE;
  ALTER TABLE "DART"."Application" ADD PRIMARY KEY ("IdApplication")
  USING INDEX PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "USERS"  ENABLE;
  ALTER TABLE "DART"."Application" MODIFY ("Name" NOT NULL ENABLE);
  ALTER TABLE "DART"."Application" MODIFY ("IdApplication" NOT NULL ENABLE);
--------------------------------------------------------
--  Constraints for Table AspNetUserClaims
--------------------------------------------------------

  ALTER TABLE "DART"."AspNetUserClaims" ADD PRIMARY KEY ("Id")
  USING INDEX PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  TABLESPACE "USERS"  ENABLE;
  ALTER TABLE "DART"."AspNetUserClaims" MODIFY ("UserId" NOT NULL ENABLE);
  ALTER TABLE "DART"."AspNetUserClaims" MODIFY ("Id" NOT NULL ENABLE);
--------------------------------------------------------
--  Constraints for Table AspNetUserRoles
--------------------------------------------------------

  ALTER TABLE "DART"."AspNetUserRoles" ADD PRIMARY KEY ("UserId", "RoleId", "CompanyId")
  USING INDEX PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "USERS"  ENABLE;
  ALTER TABLE "DART"."AspNetUserRoles" MODIFY ("CompanyId" NOT NULL ENABLE);
  ALTER TABLE "DART"."AspNetUserRoles" MODIFY ("RoleId" NOT NULL ENABLE);
  ALTER TABLE "DART"."AspNetUserRoles" MODIFY ("UserId" NOT NULL ENABLE);
--------------------------------------------------------
--  Constraints for Table AspNetPermissions
--------------------------------------------------------

  ALTER TABLE "DART"."AspNetPermissions" ADD PRIMARY KEY ("Id")
  USING INDEX PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "USERS"  ENABLE;
  ALTER TABLE "DART"."AspNetPermissions" MODIFY ("Name" NOT NULL ENABLE);
  ALTER TABLE "DART"."AspNetPermissions" MODIFY ("ApplicationId" NOT NULL ENABLE);
  ALTER TABLE "DART"."AspNetPermissions" MODIFY ("Id" NOT NULL ENABLE);
--------------------------------------------------------
--  Constraints for Table LOG_SYNCHRONIZATION
--------------------------------------------------------

  ALTER TABLE "DART"."LOG_SYNCHRONIZATION" MODIFY ("IDENTIFICATION" NOT NULL ENABLE);
--------------------------------------------------------
--  Constraints for Table BOLogSystem
--------------------------------------------------------

  ALTER TABLE "DART"."BOLogSystem" ADD PRIMARY KEY ("IdBOLogSystem")
  USING INDEX PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "USERS"  ENABLE;
  ALTER TABLE "DART"."BOLogSystem" MODIFY ("ScopeIdentifier" NOT NULL ENABLE);
  ALTER TABLE "DART"."BOLogSystem" MODIFY ("ExecutionDate" NOT NULL ENABLE);
  ALTER TABLE "DART"."BOLogSystem" MODIFY ("IdBOLogSystem" NOT NULL ENABLE);
--------------------------------------------------------
--  Constraints for Table ApplicationLanguage
--------------------------------------------------------

  ALTER TABLE "DART"."ApplicationLanguage" ADD PRIMARY KEY ("IdApplicationLanguage")
  USING INDEX PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "USERS"  ENABLE;
  ALTER TABLE "DART"."ApplicationLanguage" MODIFY ("IsDisabled" NOT NULL ENABLE);
  ALTER TABLE "DART"."ApplicationLanguage" MODIFY ("DisplayName" NOT NULL ENABLE);
  ALTER TABLE "DART"."ApplicationLanguage" MODIFY ("CultureName" NOT NULL ENABLE);
  ALTER TABLE "DART"."ApplicationLanguage" MODIFY ("IdApplicationLanguage" NOT NULL ENABLE);
--------------------------------------------------------
--  Constraints for Table AspNetUserPermissions
--------------------------------------------------------

  ALTER TABLE "DART"."AspNetUserPermissions" MODIFY ("PermissionId" NOT NULL ENABLE);
  ALTER TABLE "DART"."AspNetUserPermissions" MODIFY ("UserId" NOT NULL ENABLE);
--------------------------------------------------------
--  Constraints for Table UserCompany
--------------------------------------------------------

  ALTER TABLE "DART"."UserCompany" ADD PRIMARY KEY ("UserId", "CompanyId")
  USING INDEX PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "USERS"  ENABLE;
  ALTER TABLE "DART"."UserCompany" MODIFY ("CompanyId" NOT NULL ENABLE);
  ALTER TABLE "DART"."UserCompany" MODIFY ("UserId" NOT NULL ENABLE);
--------------------------------------------------------
--  Constraints for Table APPLICATIONLOG
--------------------------------------------------------

  ALTER TABLE "DART"."APPLICATIONLOG" ADD PRIMARY KEY ("IDAPPLICATIONLOG")
  USING INDEX PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "USERS"  ENABLE;
  ALTER TABLE "DART"."APPLICATIONLOG" MODIFY ("IDAPPLICATION" NOT NULL ENABLE);
  ALTER TABLE "DART"."APPLICATIONLOG" MODIFY ("LOGEXCEPTION" NOT NULL ENABLE);
  ALTER TABLE "DART"."APPLICATIONLOG" MODIFY ("MESSAGE" NOT NULL ENABLE);
  ALTER TABLE "DART"."APPLICATIONLOG" MODIFY ("LOGLEVEL" NOT NULL ENABLE);
  ALTER TABLE "DART"."APPLICATIONLOG" MODIFY ("CREATED" NOT NULL ENABLE);
  ALTER TABLE "DART"."APPLICATIONLOG" MODIFY ("IDAPPLICATIONLOG" NOT NULL ENABLE);
--------------------------------------------------------
--  Constraints for Table PerfilUsuario
--------------------------------------------------------

  ALTER TABLE "DART"."PerfilUsuario" ADD PRIMARY KEY ("PerfilUsuarioId")
  USING INDEX PCTFREE 10 INITRANS 2 MAXTRANS 255 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "USERS"  ENABLE;
  ALTER TABLE "DART"."PerfilUsuario" MODIFY ("DataNascimento" NOT NULL ENABLE);
  ALTER TABLE "DART"."PerfilUsuario" MODIFY ("Cargo" NOT NULL ENABLE);
  ALTER TABLE "DART"."PerfilUsuario" MODIFY ("Departamento" NOT NULL ENABLE);
  ALTER TABLE "DART"."PerfilUsuario" MODIFY ("EmpresaId" NOT NULL ENABLE);
  ALTER TABLE "DART"."PerfilUsuario" MODIFY ("UsuarioId" NOT NULL ENABLE);
  ALTER TABLE "DART"."PerfilUsuario" MODIFY ("PerfilUsuarioId" NOT NULL ENABLE);
--------------------------------------------------------
--  Constraints for Table AspNetUsers
--------------------------------------------------------

  ALTER TABLE "DART"."AspNetUsers" ADD PRIMARY KEY ("Id")
  USING INDEX PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "USERS"  ENABLE;
  ALTER TABLE "DART"."AspNetUsers" MODIFY ("UserName" NOT NULL ENABLE);
  ALTER TABLE "DART"."AspNetUsers" MODIFY ("AccessFailedCount" NOT NULL ENABLE);
  ALTER TABLE "DART"."AspNetUsers" MODIFY ("LockoutEnabled" NOT NULL ENABLE);
  ALTER TABLE "DART"."AspNetUsers" MODIFY ("TwoFactorEnabled" NOT NULL ENABLE);
  ALTER TABLE "DART"."AspNetUsers" MODIFY ("PhoneNumberConfirmed" NOT NULL ENABLE);
  ALTER TABLE "DART"."AspNetUsers" MODIFY ("EmailConfirmed" NOT NULL ENABLE);
  ALTER TABLE "DART"."AspNetUsers" MODIFY ("ApplicationId" NOT NULL ENABLE);
  ALTER TABLE "DART"."AspNetUsers" MODIFY ("Id" NOT NULL ENABLE);
--------------------------------------------------------
--  Constraints for Table AspNetRoles
--------------------------------------------------------

  ALTER TABLE "DART"."AspNetRoles" ADD PRIMARY KEY ("Id")
  USING INDEX PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "USERS"  ENABLE;
  ALTER TABLE "DART"."AspNetRoles" MODIFY ("ApplicationId" NOT NULL ENABLE);
  ALTER TABLE "DART"."AspNetRoles" MODIFY ("Name" NOT NULL ENABLE);
  ALTER TABLE "DART"."AspNetRoles" MODIFY ("Id" NOT NULL ENABLE);
--------------------------------------------------------
--  Constraints for Table Company
--------------------------------------------------------

  ALTER TABLE "DART"."Company" MODIFY ("Disabled" NOT NULL ENABLE);
  ALTER TABLE "DART"."Company" MODIFY ("CompanyType" NOT NULL ENABLE);
  ALTER TABLE "DART"."Company" MODIFY ("PhoneNumber" NOT NULL ENABLE);
  ALTER TABLE "DART"."Company" MODIFY ("AddressCity" NOT NULL ENABLE);
  ALTER TABLE "DART"."Company" MODIFY ("AddressState" NOT NULL ENABLE);
  ALTER TABLE "DART"."Company" MODIFY ("AddressNeighborhood" NOT NULL ENABLE);
  ALTER TABLE "DART"."Company" MODIFY ("AddressNumber" NOT NULL ENABLE);
  ALTER TABLE "DART"."Company" MODIFY ("Address" NOT NULL ENABLE);
  ALTER TABLE "DART"."Company" MODIFY ("AddressZipCode" NOT NULL ENABLE);
  ALTER TABLE "DART"."Company" MODIFY ("CNPJ" NOT NULL ENABLE);
  ALTER TABLE "DART"."Company" MODIFY ("TradingName" NOT NULL ENABLE);
  ALTER TABLE "DART"."Company" ADD PRIMARY KEY ("CompanyId")
  USING INDEX PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "USERS"  ENABLE;
  ALTER TABLE "DART"."Company" MODIFY ("Initials" NOT NULL ENABLE);
  ALTER TABLE "DART"."Company" MODIFY ("CompanyName" NOT NULL ENABLE);
  ALTER TABLE "DART"."Company" MODIFY ("CompanyId" NOT NULL ENABLE);
--------------------------------------------------------
--  Constraints for Table ApplicationSession
--------------------------------------------------------

  ALTER TABLE "DART"."ApplicationSession" ADD PRIMARY KEY ("IdApplicationSession")
  USING INDEX PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "USERS"  ENABLE;
  ALTER TABLE "DART"."ApplicationSession" MODIFY ("DataUltimaAcao" NOT NULL ENABLE);
  ALTER TABLE "DART"."ApplicationSession" MODIFY ("DataLogin" NOT NULL ENABLE);
  ALTER TABLE "DART"."ApplicationSession" MODIFY ("IdApplication" NOT NULL ENABLE);
  ALTER TABLE "DART"."ApplicationSession" MODIFY ("IdAspNetUsers" NOT NULL ENABLE);
  ALTER TABLE "DART"."ApplicationSession" MODIFY ("IdApplicationSession" NOT NULL ENABLE);
--------------------------------------------------------
--  Ref Constraints for Table ApplicationSession
--------------------------------------------------------

  ALTER TABLE "DART"."ApplicationSession" ADD CONSTRAINT "APPLICATIONSESSION_COMPANY" FOREIGN KEY ("CompanyId")
	  REFERENCES "DART"."Company" ("CompanyId") ENABLE;
  ALTER TABLE "DART"."ApplicationSession" ADD FOREIGN KEY ("IdApplication")
	  REFERENCES "DART"."Application" ("IdApplication") ENABLE;
--------------------------------------------------------
--  Ref Constraints for Table AspNetPermissions
--------------------------------------------------------

  ALTER TABLE "DART"."AspNetPermissions" ADD FOREIGN KEY ("ApplicationId")
	  REFERENCES "DART"."Application" ("IdApplication") ENABLE;
--------------------------------------------------------
--  Ref Constraints for Table AspNetRolePermissions
--------------------------------------------------------

  ALTER TABLE "DART"."AspNetRolePermissions" ADD FOREIGN KEY ("RoleId")
	  REFERENCES "DART"."AspNetRoles" ("Id") ON DELETE CASCADE ENABLE;
  ALTER TABLE "DART"."AspNetRolePermissions" ADD FOREIGN KEY ("PermissionId")
	  REFERENCES "DART"."AspNetPermissions" ("Id") ON DELETE CASCADE ENABLE;
--------------------------------------------------------
--  Ref Constraints for Table AspNetRoles
--------------------------------------------------------

  ALTER TABLE "DART"."AspNetRoles" ADD FOREIGN KEY ("ApplicationId")
	  REFERENCES "DART"."Application" ("IdApplication") ENABLE;
--------------------------------------------------------
--  Ref Constraints for Table AspNetUserLogins
--------------------------------------------------------

  ALTER TABLE "DART"."AspNetUserLogins" ADD CONSTRAINT "APPLICATIONUSER_LOGINS" FOREIGN KEY ("UserId")
	  REFERENCES "DART"."AspNetUsers" ("Id") ON DELETE CASCADE ENABLE;
--------------------------------------------------------
--  Ref Constraints for Table AspNetUserPermissions
--------------------------------------------------------

  ALTER TABLE "DART"."AspNetUserPermissions" ADD FOREIGN KEY ("UserId")
	  REFERENCES "DART"."AspNetUsers" ("Id") ON DELETE CASCADE ENABLE;
  ALTER TABLE "DART"."AspNetUserPermissions" ADD FOREIGN KEY ("PermissionId")
	  REFERENCES "DART"."AspNetPermissions" ("Id") ON DELETE CASCADE ENABLE;
--------------------------------------------------------
--  Ref Constraints for Table AspNetUserRoles
--------------------------------------------------------

  ALTER TABLE "DART"."AspNetUserRoles" ADD CONSTRAINT "APPLICATIONUSER_ROLES" FOREIGN KEY ("UserId")
	  REFERENCES "DART"."AspNetUsers" ("Id") ON DELETE CASCADE ENABLE;
  ALTER TABLE "DART"."AspNetUserRoles" ADD CONSTRAINT "IDENTITYROLE_COMPANY_DELETE" FOREIGN KEY ("CompanyId")
	  REFERENCES "DART"."Company" ("CompanyId") ON DELETE CASCADE ENABLE;
  ALTER TABLE "DART"."AspNetUserRoles" ADD CONSTRAINT "IDENTITYROLE_USERS_DELETE" FOREIGN KEY ("RoleId")
	  REFERENCES "DART"."AspNetRoles" ("Id") ON DELETE CASCADE ENABLE;
--------------------------------------------------------
--  Ref Constraints for Table AspNetUsers
--------------------------------------------------------

  ALTER TABLE "DART"."AspNetUsers" ADD CONSTRAINT "APPLICATIONSESSION_USERS" FOREIGN KEY ("IdApplicationSession")
	  REFERENCES "DART"."ApplicationSession" ("IdApplicationSession") ON DELETE CASCADE ENABLE;
  ALTER TABLE "DART"."AspNetUsers" ADD FOREIGN KEY ("ApplicationId")
	  REFERENCES "DART"."Application" ("IdApplication") ENABLE;
--------------------------------------------------------
--  Ref Constraints for Table BOLogSystem
--------------------------------------------------------

  ALTER TABLE "DART"."BOLogSystem" ADD FOREIGN KEY ("AspNetUsersId")
	  REFERENCES "DART"."AspNetUsers" ("Id") ENABLE;
--------------------------------------------------------
--  Ref Constraints for Table PerfilUsuario
--------------------------------------------------------

  ALTER TABLE "DART"."PerfilUsuario" ADD CONSTRAINT "PerfilUsuario_AspNetUsers" FOREIGN KEY ("UsuarioId")
	  REFERENCES "DART"."AspNetUsers" ("Id") ON DELETE CASCADE ENABLE;
  ALTER TABLE "DART"."PerfilUsuario" ADD CONSTRAINT "PerfilUsuario_Company" FOREIGN KEY ("EmpresaId")
	  REFERENCES "DART"."Company" ("CompanyId") ON DELETE CASCADE ENABLE;
--------------------------------------------------------
--  Ref Constraints for Table UserCompany
--------------------------------------------------------

  ALTER TABLE "DART"."UserCompany" ADD CONSTRAINT "USERCOMPANY_ASPNETUSERS" FOREIGN KEY ("UserId")
	  REFERENCES "DART"."AspNetUsers" ("Id") ENABLE;
  ALTER TABLE "DART"."UserCompany" ADD CONSTRAINT "USERCOMPANY_COMPANY" FOREIGN KEY ("CompanyId")
	  REFERENCES "DART"."Company" ("CompanyId") ENABLE;
