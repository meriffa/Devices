--
-- Schemas
--

CREATE SCHEMA "Garden";

--
-- Tables
--

CREATE TABLE "Application" (
	"ApplicationID" int NOT NULL,
	"ApplicationName" varchar(1024) NOT NULL,
	"ApplicationEnabled" boolean NOT NULL,
	CONSTRAINT "PK_Application" PRIMARY KEY ("ApplicationID"),
	CONSTRAINT "IX_Application_ApplicationName" UNIQUE ("ApplicationName")
);

CREATE TABLE "Action" (
	"ActionID" int NOT NULL,
	"ActionType" int NOT NULL,
	"ActionParameters" varchar(1024) NOT NULL,
	"ActionArguments" varchar(1024) NULL,
	CONSTRAINT "PK_Action" PRIMARY KEY ("ActionID")
);

CREATE TABLE "Release" (
	"ReleaseID" int NOT NULL GENERATED BY DEFAULT AS IDENTITY,
	"ServiceDate" timestamp with time zone NOT NULL,
	"ApplicationID" int NOT NULL,
	"Package" varchar(1024) NOT NULL,
	"PackageHash" varchar(64) NULL,
	"Version" varchar(64) NOT NULL,
	"ActionID" int NOT NULL,
	"ReleaseEnabled" boolean NOT NULL,
	CONSTRAINT "PK_Release" PRIMARY KEY ("ReleaseID"),
	CONSTRAINT "FK_Release_Application" FOREIGN KEY ("ApplicationID") REFERENCES "Application" ("ApplicationID"),
	CONSTRAINT "FK_Release_Action" FOREIGN KEY ("ActionID") REFERENCES "Action" ("ActionID")
);

CREATE TABLE "ReleaseDependency" (
	"ParentReleaseID" int NOT NULL,
	"ChildReleaseID" int NOT NULL,
	CONSTRAINT "PK_ReleaseDependency" PRIMARY KEY ("ParentReleaseID", "ChildReleaseID"),
	CONSTRAINT "FK_ReleaseDependency_Release_Parent" FOREIGN KEY ("ParentReleaseID") REFERENCES "Release" ("ReleaseID"),
	CONSTRAINT "FK_ReleaseDependency_Release_Child" FOREIGN KEY ("ChildReleaseID") REFERENCES "Release" ("ReleaseID")
);

CREATE TABLE "Device" (
	"DeviceID" int NOT NULL GENERATED BY DEFAULT AS IDENTITY,	
	"DeviceToken" varchar(64) NOT NULL,
	"DeviceName" varchar(1024) NOT NULL,
	"DeviceLocation" varchar(1024) NOT NULL,
	"DeviceEnabled" boolean NOT NULL,
	CONSTRAINT "PK_Device" PRIMARY KEY ("DeviceID"),
	CONSTRAINT "IX_Device_DeviceName" UNIQUE ("DeviceName"),
	CONSTRAINT "IX_Device_DeviceToken" UNIQUE ("DeviceToken")
);

CREATE TABLE "DeviceFingerprint" (
	"FingerprintType" int NOT NULL,
	"FingerprintValue" varchar(1024) NOT NULL,
	"DeviceID" int NOT NULL,
	CONSTRAINT "PK_DeviceFingerprint" PRIMARY KEY ("FingerprintType", "FingerprintValue"),
	CONSTRAINT "FK_DeviceFingerprint_Device" FOREIGN KEY ("DeviceID") REFERENCES "Device" ("DeviceID")
);

CREATE TABLE "DeviceMetric" (
	"DeviceID" int NOT NULL,
	"ServiceDate" timestamp with time zone NOT NULL,
	"DeviceDate" timestamp with time zone NOT NULL,
	"LastReboot" timestamp with time zone NOT NULL,
	"CpuUser" real NOT NULL,
	"CpuSystem" real NOT NULL,
	"CpuIdle" real NOT NULL,
	"MemoryTotal" int NOT NULL,
	"MemoryUsed" int NOT NULL,
	"MemoryFree" int NOT NULL,
	CONSTRAINT "PK_DeviceMetric" PRIMARY KEY ("DeviceID", "ServiceDate"),
	CONSTRAINT "FK_DeviceMetric_Device" FOREIGN KEY ("DeviceID") REFERENCES "Device" ("DeviceID")
);

CREATE TABLE "DeviceApplication" (
	"DeviceID" int NOT NULL,
	"ApplicationID" int NOT NULL,
	"DeviceApplicationEnabled" boolean NOT NULL,	
	CONSTRAINT "PK_DeviceApplication" PRIMARY KEY ("DeviceID", "ApplicationID"),
	CONSTRAINT "FK_DeviceApplication_Device" FOREIGN KEY ("DeviceID") REFERENCES "Device" ("DeviceID"),
	CONSTRAINT "FK_DeviceApplication_Application" FOREIGN KEY ("ApplicationID") REFERENCES "Application" ("ApplicationID")
);

CREATE TABLE "DeviceDeployment" (
	"DeploymentID" int NOT NULL GENERATED BY DEFAULT AS IDENTITY,
	"DeviceID" int NOT NULL,
	"ReleaseID" int NOT NULL,
	"DeviceDate" timestamp with time zone NOT NULL,
	"Success" boolean NOT NULL,
	"Details" text NULL,
	CONSTRAINT "PK_DeviceDeployment" PRIMARY KEY ("DeploymentID"),
	CONSTRAINT "IX_DeviceDeployment_DeviceIDReleaseID" UNIQUE ("DeviceID", "ReleaseID"),
	CONSTRAINT "FK_DeviceDeployment_Device" FOREIGN KEY ("DeviceID") REFERENCES "Device" ("DeviceID"),
	CONSTRAINT "FK_DeviceDeployment_Release" FOREIGN KEY ("ReleaseID") REFERENCES "Release" ("ReleaseID")
);

CREATE TABLE "User" (
	"UserID" int4 NOT NULL GENERATED BY DEFAULT AS IDENTITY,
	"Username" varchar(128) NOT NULL,
	"Password" varchar(128) NOT NULL,
	"FullName" varchar(128) NOT NULL,
	"Email" varchar(128) NOT NULL,
	"Role" varchar(128) NOT NULL,
	"UserEnabled" boolean NOT NULL,
	CONSTRAINT "PK_User" PRIMARY KEY ("UserID"),
	CONSTRAINT "IX_User_Username" UNIQUE ("Username")
);

CREATE TABLE "Garden"."WeatherCondition" (
	"DeviceID" int NOT NULL,
	"DeviceDate" timestamp with time zone NOT NULL,
	"Temperature" numeric NOT NULL,
	"Humidity" numeric NOT NULL,
	"Pressure" numeric NOT NULL,
	"Illuminance" numeric NOT NULL,
	CONSTRAINT "PK_WeatherCondition" PRIMARY KEY ("DeviceID", "DeviceDate"),
	CONSTRAINT "FK_WeatherCondition_Device" FOREIGN KEY ("DeviceID") REFERENCES "Device" ("DeviceID")
);

--
-- Static Data
--

INSERT INTO "Application" ("ApplicationID", "ApplicationName", "ApplicationEnabled") VALUES
    (1, 'Devices.Client', TRUE),
    (2, 'Devices.Client.Solutions', TRUE),
    (3, 'Devices.Client Scheduled Jobs', TRUE),
    (4, 'Devices.Client.Solutions Scheduled Jobs', TRUE),
    (5, 'System', TRUE);
INSERT INTO "Action" ("ActionID", "ActionType", "ActionParameters", "ActionArguments") VALUES
    (1, 1, 'Install.sh', 'InstallClient "Devices.Client"'),
    (2, 1, 'Install.sh', 'InstallClient "Devices.Client.Solutions"'),
    (3, 1, 'Install.sh', 'SetupScheduledJobs "Devices.Client"'),
    (4, 1, 'Install.sh', 'SetupScheduledJobs "Devices.Client.Solutions"'),
    (5, 1, 'Install.sh', 'SystemUpdate'),
    (6, 1, 'Install.sh', 'SystemRestart'),
    (7, 1, 'Install.sh', 'SynchronizeClock'),
    (8, 1, 'Install.sh', 'ExecuteCommand "ls ~"'),
	(9, 1, 'Install.sh', 'UploadDeviceLogs');
INSERT INTO "Release" ("ReleaseID", "ServiceDate", "ApplicationID", "Package", "PackageHash", "Version", "ActionID", "ReleaseEnabled") VALUES
    (1, NOW(), 1, 'Devices.Client.zip', NULL, '1.0.0', 1, TRUE),
    (2, NOW(), 2, 'Devices.Client.Solutions.zip', NULL, '1.0.0', 2, TRUE),
    (3, NOW(), 3, 'Install.zip', NULL, '1.0.0', 3, TRUE),
    (4, NOW(), 4, 'Install.zip', NULL, '1.0.0', 4, TRUE),
    (5, NOW(), 5, 'Install.zip', NULL, '1.0.0', 5, FALSE),
    (6, NOW(), 5, 'Install.zip', NULL, '1.0.0', 6, FALSE),
    (7, NOW(), 5, 'Install.zip', NULL, '1.0.0', 7, FALSE),
    (8, NOW(), 5, 'Install.zip', NULL, '1.0.0', 8, FALSE),
	(9, NOW(), 5, 'Install.zip', NULL, '1.0.0', 9, FALSE);
SELECT SETVAL($$"Release_ReleaseID_seq"$$, COALESCE((SELECT MAX("ReleaseID") FROM "Release"), 0));
INSERT INTO "ReleaseDependency" ("ParentReleaseID", "ChildReleaseID") VALUES
    (1, 2),
	(1, 6),
	(2, 7);
