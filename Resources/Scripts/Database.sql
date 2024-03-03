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

CREATE TABLE "Garden"."WeatherCondition" (
	"Date" timestamp with time zone NOT NULL,
	"Temperature" numeric NOT NULL,
	"Humidity" numeric NOT NULL,
	"Pressure" numeric NOT NULL,
	"Illuminance" numeric NOT NULL,
	CONSTRAINT "PK_WeatherCondition" PRIMARY KEY ("Date")
);