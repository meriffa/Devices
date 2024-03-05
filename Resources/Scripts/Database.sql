--
-- Database
--

CREATE ROLE "DevicesUser" WITH LOGIN ENCRYPTED PASSWORD $$DevicesUserPassword$$;
CREATE DATABASE "Devices.Data";
ALTER DATABASE "Devices.Data" OWNER TO "DevicesUser";

--
-- Schemas
--

CREATE SCHEMA "Garden";

--
-- Tables
--

CREATE TABLE "Device" (
	"DeviceID" varchar(64) NOT NULL,
	"DeviceName" varchar(1024) NOT NULL,
	"Active" bool NOT NULL,
	CONSTRAINT "PK_Device" PRIMARY KEY ("DeviceID"),
	CONSTRAINT "IX_DeviceName" UNIQUE ("DeviceName")
);

CREATE TABLE "DeviceFingerprint" (
	"FingerprintType" int NOT NULL,
	"FingerprintValue" varchar(1024) NOT NULL,
	"DeviceID" varchar(64) NOT NULL,
	CONSTRAINT "PK_DeviceFingerprint" PRIMARY KEY ("FingerprintType", "FingerprintValue"),
	CONSTRAINT "FK_DeviceFingerprint_Device" FOREIGN KEY ("DeviceID") REFERENCES "Device" ("DeviceID")
);

CREATE TABLE "Garden"."WeatherCondition" (
	"Date" timestamp with time zone NOT NULL,
	"Temperature" numeric NOT NULL,
	"Humidity" numeric NOT NULL,
	"Pressure" numeric NOT NULL,
	"Illuminance" numeric NOT NULL,
	CONSTRAINT "PK_WeatherCondition" PRIMARY KEY ("Date")
);