-- ---------------------------------------------------------------
-- Filename: simPopulate.sql
-- Project: ASQL Assignment 2 (W14)
-- Author: Ryan Pink & Dan Hieronimus
-- Date Modified: 2014-02-09
-- Description:
-- This file declares and sets global variables for use
-- during the simulation, and populates the tables with
-- initial data.
-- ---------------------------------------------------------------
-- Start by initializing some variables
-- ---------------------------------------------------------------
-- -- Full Bin Values
SET @fullHarnesses  =  75; -- Harnesses per bin (full)
SET @fullReflectors =  35; -- Reflectors per bin (full)
SET @fullHousings   =  20; -- Housings per bin (full)
SET @fullLenses     =  40; -- Lenses per bin (full)
SET @fullBulbs      =  50; -- Bulbs per bin (full)
SET @fullBezels     =  75; -- Bezels per bin (full)

SET @fullTestTray   =  60; -- Test Tray capacity
SET @runnerTimer    = 300; -- Time between runner calls (in seconds)
SET @minBinAmount   =   5; -- At what part amount will the bin be carded for refill

SET @station1finished = TRUE;
SET @station2finished = TRUE;
SET @station3finished = TRUE;

-- -- Loop Variables
SET @simDuration =  25200; -- The total duration of the simulation in seconds
SET @timeElapsed =      0; -- The incremental seconds elapsed in the simulation

SET @tickRate    =   1000; -- The number of ticks per second


-- ---------------------------------------------------------------
-- Creating a View to display the part requests
-- ---------------------------------------------------------------
CREATE VIEW KANBAN.PartRequests AS 
 	SELECT harnessReqs AS HarnessBins, 
 		   reflectorReqs AS ReflectorBins, 
 		   housingReqs AS HousingBins, 
 		   lensReqs AS LensBins, 
 		   bulbReqs AS BulbBins, 
 		   bezelReqs AS BezelBins
	FROM KANBAN.RequestMadeToJamieTheRunnerToFillUpMyBin
	LIMIT 1;


CREATE VIEW KANBAN.DefectiveLamps AS
	SELECT DISTINCT ttid AS TrayID, asid AS AssemblyStationID, (
		SELECT COUNT(IsThisFineElegantLampWorking) 
		FROM KANBAN.TestTray 
		WHERE ttid = TrayID 
		AND asid = AssemblyStationID
		AND IsThisFineElegantLampWorking = 1)*(5/3) AS PercentPassed
	FROM KANBAN.TestTray;


DELETE FROM KANBAN.AssemblyStation WHERE 1;
-- ---------------------------------------------------------------
-- Inserting data into the AssemblyStation table
-- ---------------------------------------------------------------
INSERT INTO KANBAN.AssemblyStation
	( time, variance,      harnesses,      reflectors,      housings,      lenses,      bulbs,      bezels )
VALUES
	(   60,        6, @fullHarnesses, @fullReflectors, @fullHousings, @fullLenses, @fullBulbs, @fullBezels ),
	(   90,        9, @fullHarnesses, @fullReflectors, @fullHousings, @fullLenses, @fullBulbs, @fullBezels ),
	(   51,        5, @fullHarnesses, @fullReflectors, @fullHousings, @fullLenses, @fullBulbs, @fullBezels );


DELETE FROM KANBAN.RequestMadeToJamieTheRunnerToFillUpMyBin WHERE 1;
INSERT INTO KANBAN.RequestMadeToJamieTheRunnerToFillUpMyBin
VALUES
	( 0, 0, 0, 0, 0, 0 );