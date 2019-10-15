-- TABLE Application
CREATE TABLE "Application" (
	"IdApplication" NUMBER(10) NOT NULL,
	"Name" VARCHAR2(50) NOT NULL,
	PRIMARY KEY ("IdApplication"),
	UNIQUE ("Name")
) ;

-------------------------------------
-- ASP.NET Identity
-------------------------------------

-- TABLE AspNetRoles
CREATE TABLE "AspNetRoles" (
  "Id" VARCHAR2(128) NOT NULL,
  "Name" VARCHAR2(256) NOT NULL,
  "ApplicationId" NUMBER(10) NOT NULL,
  PRIMARY KEY ("Id"),
  FOREIGN KEY ("ApplicationId") REFERENCES "Application" ("IdApplication")
) ;

-- TABLE AspNetUsers
CREATE TABLE "AspNetUsers" (
  "Id" VARCHAR2(128) NOT NULL,
  "ApplicationId" NUMBER(10) NOT NULL,
  "Email" VARCHAR2(256) DEFAULT NULL,
  "EmailConfirmed" NUMBER(3) NOT NULL,
  "PasswordHash" CLOB,
  "SecurityStamp" CLOB,
  "PhoneNumber" CLOB,
  "PhoneNumberConfirmed" NUMBER(3) NOT NULL,
  "TwoFactorEnabled" NUMBER(3) NOT NULL,
  "LockoutEndDateUtc" TIMESTAMP(0) DEFAULT NULL,
  "LockoutEnabled" NUMBER(3) NOT NULL,
  "AccessFailedCount" NUMBER(10) NOT NULL,
  "UserName" VARCHAR2(256) NOT NULL,
  "IdApplicationSession" NUMBER(10),
  PRIMARY KEY ("Id"),
  FOREIGN KEY ("ApplicationId") REFERENCES "Application" ("IdApplication")
) ;

-- TABLE AspNetPermissions
CREATE TABLE "AspNetPermissions" (
	"Id" VARCHAR2(128) NOT NULL,
	"ApplicationId" NUMBER(10) NOT NULL,
	"Name" VARCHAR2(256) NOT NULL,
	PRIMARY KEY ("Id"),
	FOREIGN KEY ("ApplicationId") REFERENCES "Application" ("IdApplication")
) ;

-- TABLE AspNetRolePermissions
CREATE TABLE "AspNetRolePermissions" (
	"RoleId" VARCHAR2(128) NOT NULL,
	"PermissionId" VARCHAR2(128) NOT NULL,
	FOREIGN KEY ("RoleId") REFERENCES "AspNetRoles" ("Id")
		ON DELETE CASCADE
	,	
	FOREIGN KEY ("PermissionId") REFERENCES "AspNetPermissions" ("Id")
		ON DELETE CASCADE
) ;

-- TABLE AspNetUserPermissions
CREATE TABLE "AspNetUserPermissions" (
	"UserId" VARCHAR2(128) NOT NULL,
	"PermissionId" VARCHAR2(128) NOT NULL,
	FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id")
		ON DELETE CASCADE
	,		
	FOREIGN KEY ("PermissionId") REFERENCES "AspNetPermissions" ("Id")
		ON DELETE CASCADE
) ;

-- TABLE AspNetUserClaims
CREATE TABLE "AspNetUserClaims" (
  "Id" number(10) NOT NULL,
  "UserId" varchar2(128) NOT NULL,
  "ClaimType" clob,
  "ClaimValue" clob,
  PRIMARY KEY ("Id")
) ;

-- Generate ID using sequence and trigger
CREATE SEQUENCE AspNetUserClaims_seq START WITH 1 INCREMENT BY 1;

CREATE OR REPLACE TRIGGER AspNetUserClaims_seq_tr
 BEFORE INSERT ON "AspNetUserClaims" FOR EACH ROW
 WHEN (NEW."Id" IS NULL)
BEGIN
 SELECT AspNetUserClaims_seq.NEXTVAL INTO :NEW."Id" FROM DUAL;
END;


CREATE INDEX "UserId" ON "AspNetUserClaims" ("UserId");

-- TABLE AspNetUserLogins
CREATE TABLE "AspNetUserLogins" (
  "LoginProvider" VARCHAR2(128) NOT NULL,
  "ProviderKey" VARCHAR2(128) NOT NULL,
  "UserId" VARCHAR2(128) NOT NULL,
  PRIMARY KEY ("LoginProvider", "ProviderKey", "UserId")
) ;

CREATE INDEX ApplicationUser_Logins ON "AspNetUserLogins" ("UserId");

-- TABLE AspNetUserRoles
CREATE TABLE "AspNetUserRoles" (
  "UserId" VARCHAR2(128) NOT NULL,
  "RoleId" VARCHAR2(128) NOT NULL,
  "CompanyId" NUMBER NOT NULL,
  PRIMARY KEY ("UserId", "RoleId", "CompanyId")
) ;

CREATE INDEX IdentityRole_Users ON "AspNetUserRoles" ("RoleId");

-- Constraints for table AspNetUserClaims
ALTER TABLE "AspNetUserClaims"
	ADD CONSTRAINT ApplicationUser_Claims FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE;

-- Constraints for table AspNetUserLogins
ALTER TABLE "AspNetUserLogins"
	ADD CONSTRAINT ApplicationUser_Logins FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE;

-- Constraints for table AspNetUserRoles
ALTER TABLE "AspNetUserRoles"
	ADD CONSTRAINT ApplicationUser_Roles FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE;

ALTER TABLE "AspNetUserRoles"
	ADD CONSTRAINT IdentityRole_Users_Delete FOREIGN KEY ("RoleId") REFERENCES "AspNetRoles" ("Id") ON DELETE CASCADE;


-------------------------------------
-- Geral
-------------------------------------

-- TABLE BOLogSystem
CREATE TABLE "BOLogSystem" (
	"IdBOLogSystem" NUMBER(19) NOT NULL,
	"UserId" VARCHAR2(128) DEFAULT NULL,
	"ActionType" VARCHAR2(30) DEFAULT NULL,
	"IP" VARCHAR2(50) DEFAULT NULL,
	"ExecutionDate" TIMESTAMP(3) NOT NULL,
	"Entity" VARCHAR2(100) DEFAULT NULL,
	"OldEntity" clob,
	"NewEntity" clob,
	"ScopeIdentifier" VARCHAR2(50) NOT NULL,
	PRIMARY KEY ("IdBOLogSystem"),
	FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id")
) ;

-- Generate ID using sequence and trigger
CREATE SEQUENCE BOLogSystem_seq START WITH 1 INCREMENT BY 1;

CREATE OR REPLACE TRIGGER BOLogSystem_seq_tr
 BEFORE INSERT ON "BOLogSystem" FOR EACH ROW
 WHEN (NEW."IdBOLogSystem" IS NULL)
BEGIN
 SELECT BOLogSystem_seq.NEXTVAL INTO :NEW."IdBOLogSystem" FROM DUAL;
END;
/

-- TABLE ApplicationLog
CREATE TABLE "ApplicationLog" (
	"IdApplicationLog" NUMBER(10) NOT NULL,
	"Created" TIMESTAMP(3) NOT NULL,
	"Level" VARCHAR2(50) NOT NULL,
	"Message" VARCHAR2(4000) NOT NULL,
	"Exception" CLOB NOT NULL,
	"IdApplication" NUMBER(10) NOT NULL,
	PRIMARY KEY ("IdApplicationLog"),
	FOREIGN KEY ("IdApplication") REFERENCES "Application" ("IdApplication")
) ;

-- Generate ID using sequence and trigger
CREATE SEQUENCE ApplicationLog_seq START WITH 1 INCREMENT BY 1;

CREATE OR REPLACE TRIGGER ApplicationLog_seq_tr
 BEFORE INSERT ON "ApplicationLog" FOR EACH ROW
 WHEN (NEW."IdApplicationLog" IS NULL)
BEGIN
 SELECT ApplicationLog_seq.NEXTVAL INTO :NEW."IdApplicationLog" FROM DUAL;
END;
/

-- TABLE ApplicationLanguage
CREATE TABLE "ApplicationLanguage" (
  "IdApplicationLanguage" NUMBER(10) NOT NULL,
  "CultureName" VARCHAR2(10) NOT NULL,
  "DisplayName" VARCHAR2(100) NOT NULL,
  "IsDisabled" 	NUMBER(3) NOT NULL,
  PRIMARY KEY ("IdApplicationLanguage")
) ;



-- Generate ID using sequence and trigger
CREATE SEQUENCE ApplicationLanguage_seq START WITH 1 INCREMENT BY 1;

CREATE OR REPLACE TRIGGER ApplicationLanguage_seq_tr
 BEFORE INSERT ON "ApplicationLanguage" FOR EACH ROW
 WHEN (NEW."IdApplicationLanguage" IS NULL)
BEGIN
 SELECT ApplicationLanguage_seq.NEXTVAL INTO :NEW."IdApplicationLanguage" FROM DUAL;
END;
/

INSERT INTO "ApplicationLanguage" ("CultureName", "DisplayName"," IsDisabled")
VALUES ('pt-BR', 'Português (Brasil)', 0);

INSERT INTO "Application"("IdApplication", "Name")
VALUES (1, 'BackOffice');

INSERT INTO "Application"("IdApplication", "Name")
VALUES (2, 'Api');

CREATE TABLE "ApplicationSession" (
"IdApplicationSession"	NUMBER(10) NOT NULL,
"IdAspNetUsers" 			VARCHAR2(128) NOT NULL,
"IdApplication" 			NUMBER(10) NOT NULL,
"DataLogin" 				TIMESTAMP(0) NOT NULL,
"DataUltimaAcao" 			TIMESTAMP(0) NOT NULL,
"DataLogout" 				TIMESTAMP(0),
PRIMARY KEY ("IdApplicationSession"),  
FOREIGN KEY ("IdApplication") REFERENCES "Application" ("IdApplication")
);

-- Generate ID using sequence and trigger
CREATE SEQUENCE ApplicationSession_seq START WITH 1 INCREMENT BY 1;

CREATE OR REPLACE TRIGGER ApplicationSession_seq_tr
 BEFORE INSERT ON "ApplicationSession" FOR EACH ROW
 WHEN (NEW."IdApplicationSession" IS NULL)
BEGIN
 SELECT ApplicationSession_seq.NEXTVAL INTO :NEW."IdApplicationSession" FROM DUAL;
END;
/



-- Constraints for table AspNetUser
ALTER TABLE "AspNetUsers"
	ADD CONSTRAINT ApplicationSession_Users FOREIGN KEY ("IdApplicationSession") REFERENCES "ApplicationSession" ("IdApplicationSession") ON DELETE CASCADE;
	

