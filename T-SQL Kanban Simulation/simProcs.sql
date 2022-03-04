-- ---------------------------------------------------------------
-- Filename: simProcs.sql
-- Project: ASQL Assignment 2 (W14)
-- Author: Ryan Pink & Dan Hieronimus
-- Date Modified: 2014-02-09
-- Description:
-- This file defines the stored procedures and functions
-- associated with the KANBAN simulation.
-- ---------------------------------------------------------------
-- Start by defining the Stored Procs
-- ---------------------------------------------------------------
DELIMITER //


DROP TRIGGER IF EXISTS KANBAN.FlagFinished //
-- Flag for finished parts
CREATE TRIGGER KANBAN.FlagFinished
BEFORE UPDATE ON KANBAN.Lamp FOR EACH ROW
BEGIN
	IF ( NEW.time2complete = 0 ) THEN
		IF ( NEW.asid = 1 ) THEN
			CALL PlaceInTestTray( NEW.asid, NEW.lampid );

			SET @station1finished = TRUE;
		END IF;

		IF ( NEW.asid = 2 ) then
			CALL PlaceInTestTray( NEW.asid, NEW.lampid );

			SET @station2finished = TRUE;
		END IF;

		IF ( NEW.asid = 3 ) THEN
			CALL PlaceInTestTray( NEW.asid, NEW.lampid );

			SET @station3finished = TRUE;
		END IF;
	END IF;
END //


DROP TRIGGER IF EXISTS KANBAN.CardBin //
-- Check the station bins to see if they need to be refilled.
CREATE TRIGGER KANBAN.CardBin
AFTER UPDATE ON KANBAN.AssemblyStation FOR EACH ROW
BEGIN
	SET @AssID = (SELECT asid FROM KANBAN.CardTray WHERE asid = NEW.asid AND bin = 1);
	IF ( NEW.harnesses = @minBinAmount ) AND ( @AssID IS NULL ) THEN
		INSERT INTO KANBAN.CardTray
			( asid, bin )
		VALUES
			( NEW.asid, 1 );

		UPDATE KANBAN.RequestMadeToJamieTheRunnerToFillUpMyBin
		SET harnessReqs = harnessReqs+1
		WHERE 1;
	END IF;

	SET @AssID = (SELECT asid FROM KANBAN.CardTray WHERE asid = NEW.asid AND bin = 2);
	IF ( NEW.reflectors = @minBinAmount ) AND ( @AssID IS NULL ) THEN
		INSERT INTO KANBAN.CardTray
			( asid, bin )
		VALUES
			( NEW.asid, 2 );

		UPDATE KANBAN.RequestMadeToJamieTheRunnerToFillUpMyBin
		SET reflectorReqs = reflectorReqs+1
		WHERE 1;
	END IF;

	SET @AssID = (SELECT asid FROM KANBAN.CardTray WHERE asid = NEW.asid AND bin = 3);
	IF ( NEW.housings = @minBinAmount ) AND ( @AssID IS NULL ) THEN
		INSERT INTO KANBAN.CardTray
			( asid, bin )
		VALUES
			( NEW.asid, 3 );

		UPDATE KANBAN.RequestMadeToJamieTheRunnerToFillUpMyBin
		SET housingReqs = housingReqs+1
		WHERE 1;
	END IF;

	SET @AssID = (SELECT asid FROM KANBAN.CardTray WHERE asid = NEW.asid AND bin = 4);
	IF ( NEW.lenses = @minBinAmount ) AND ( @AssID IS NULL ) THEN
		INSERT INTO KANBAN.CardTray
			( asid, bin )
		VALUES
			( NEW.asid, 4 );

		UPDATE KANBAN.RequestMadeToJamieTheRunnerToFillUpMyBin
		SET lensReqs = lensReqs+1
		WHERE 1;
	END IF;

	SET @AssID = (SELECT asid FROM KANBAN.CardTray WHERE asid = NEW.asid AND bin = 5);
	IF ( NEW.bulbs = @minBinAmount ) AND ( @AssID IS NULL ) THEN
		INSERT INTO KANBAN.CardTray
			( asid, bin )
		VALUES
			( NEW.asid, 5 );

		UPDATE KANBAN.RequestMadeToJamieTheRunnerToFillUpMyBin
		SET bulbReqs = bulbReqs+1
		WHERE 1;
	END IF;

	SET @AssID = (SELECT asid FROM KANBAN.CardTray WHERE asid = NEW.asid AND bin = 6);
	IF ( NEW.bezels = @minBinAmount ) AND ( @AssID IS NULL ) THEN
		INSERT INTO KANBAN.CardTray
			( asid, bin )
		VALUES
			( NEW.asid, 6 );

		UPDATE KANBAN.RequestMadeToJamieTheRunnerToFillUpMyBin
		SET bezelReqs = bezelReqs+1
		WHERE 1;
	END IF;
END //


DROP PROCEDURE IF EXISTS KANBAN.SimLoop //
-- SimLoop()
-- This is the main loop for the simulation. It makes calls to other procs,
-- as well as increments through time and imposes a delay to adjust the
-- speed of the demo.
CREATE PROCEDURE KANBAN.SimLoop()
BEGIN
	WHILE ( @timeElapsed < @simDuration ) DO
		
		IF ( @station1finished ) THEN
			SET @station1finished = FALSE;
			CALL StartPart( 1 );
		END IF;

		IF ( @station2finished ) THEN
			SET @station2finished = FALSE;
			CALL StartPart( 2 );
		END IF;

		IF ( @station3finished ) THEN
			SET @station3finished = FALSE;
			CALL StartPart( 3 );
		END IF;

		IF ( @timeElapsed%@runnerTimer = 0 ) THEN
			CALL Runner();
		END IF;

		CALL ProgressParts();

		CALL Timer();
	END WHILE;
END //


DROP PROCEDURE IF EXISTS KANBAN.StartPart //
-- StartPart()
-- This stored procedure starts the given station on a new part.
-- It checks for the test tray being full, and generates a new
-- one if needed.
CREATE PROCEDURE KANBAN.StartPart( IN inASID integer )
BEGIN
	UPDATE KANBAN.AssemblyStation
		SET harnesses  =  harnesses-1,
			reflectors = reflectors-1,
			housings   =   housings-1,
			lenses     =     lenses-1,
			bulbs      =      bulbs-1,
			bezels     =     bezels-1
		WHERE asid = inASID;

	SET @variance = (SELECT variance FROM KANBAN.AssemblyStation WHERE asid = inASID);
	SET @lampTime = (SELECT time FROM KANBAN.AssemblyStation WHERE asid = inASID) + (FLOOR(RAND()*100) % 3 - 1)*(FLOOR(RAND()*100) % @variance + 1);

	INSERT INTO KANBAN.Lamp
			( asid, time2complete )
		VALUES
			( inASID, @lampTime );
END //


DROP PROCEDURE IF EXISTS KANBAN.ProgressParts //
-- ProgressParts()
-- This stored procedure updates all incomplete entries in the TestTray
-- table by incrementing their progress by one second. This proc gets
-- called every tick.
CREATE PROCEDURE KANBAN.ProgressParts()
BEGIN
	UPDATE KANBAN.Lamp
	SET time2complete = time2complete-1
	WHERE time2complete > 0;
END //


DROP PROCEDURE IF EXISTS KANBAN.Runner //
-- Runner()
-- This stored procedure simulates the Runner, who checks the CardTray
-- table at each station every 5 seconds. If an assembly station requires
-- a bin to be filled, the runner fills it instantaneously. (Speedy, eh?)
CREATE PROCEDURE KANBAN.Runner()
BEGIN
	SET @nextID = (SELECT asid FROM KANBAN.CardTray ORDER BY asid ASC LIMIT 1);

	WHILE ( @nextID IS NOT NULL ) DO
		UPDATE KANBAN.AssemblyStation
		SET harnesses = harnesses + @fullHarnesses
		WHERE asid = @nextID
		AND harnesses <= @minBinAmount;

		DELETE FROM KANBAN.CardTray WHERE asid = @nextID AND bin = 1;

		UPDATE KANBAN.AssemblyStation
		SET reflectors = reflectors + @fullReflectors
		WHERE asid = @nextID
		AND reflectors <= @minBinAmount;

		DELETE FROM KANBAN.CardTray WHERE asid = @nextID AND bin = 2;

		UPDATE KANBAN.AssemblyStation
		SET housings = housings + @fullHousings
		WHERE asid = @nextID
		AND housings <= @minBinAmount;

		DELETE FROM KANBAN.CardTray WHERE asid = @nextID AND bin = 3;

		UPDATE KANBAN.AssemblyStation
		SET lenses = lenses + @fullLenses
		WHERE asid = @nextID
		AND lenses <= @minBinAmount;

		DELETE FROM KANBAN.CardTray WHERE asid = @nextID AND bin = 4;

		UPDATE KANBAN.AssemblyStation
		SET bulbs = bulbs + @fullBulbs
		WHERE asid = @nextID
		AND bulbs <= @minBinAmount;

		DELETE FROM KANBAN.CardTray WHERE asid = @nextID AND bin = 5;

		UPDATE KANBAN.AssemblyStation
		SET bezels = bezels + @fullBezels
		WHERE asid = @nextID
		AND bezels <= @minBinAmount;

		DELETE FROM KANBAN.CardTray WHERE asid = @nextID AND bin = 6;

		SET @nextID = (SELECT asid FROM KANBAN.CardTray ORDER BY asid ASC LIMIT 1);
	END WHILE;
END //


DROP PROCEDURE IF EXISTS KANBAN.Timer //
-- Timer procedure
CREATE PROCEDURE KANBAN.Timer()
BEGIN
	SET @timeElapsed = @timeElapsed+1;

	SELECT SLEEP(1/@tickRate) INTO @Cthulhu;
END //


DROP FUNCTION IF EXISTS KANBAN.GetLampCode //
CREATE FUNCTION KANBAN.GetLampCode(trayID INT, trayPosition INT) 
RETURNS CHAR(40)
DETERMINISTIC
BEGIN

	DECLARE lampCode CHAR(40) DEFAULT "FL";

	SET lampCode = CONCAT(lampCode, LPAD(trayID, 6, 0));
	SET lampCode = CONCAT(lampCode, LPAD(trayPosition, 2, 0));

	RETURN lampCode;

END //


DROP PROCEDURE IF EXISTS KANBAN.PlaceInTestTray //
CREATE PROCEDURE KANBAN.PlaceInTestTray(IN stationID INT, IN lampID INT)
BEGIN
	DECLARE currentTray INT DEFAULT NULL;
	DECLARE lampsInCurrentTray INT DEFAULT NULL;
	DECLARE workingLamp BOOL DEFAULT FALSE;

	SET currentTray = (SELECT ttid FROM TestTray WHERE TestTray.asid=stationID ORDER BY ttid DESC LIMIT 1);
	SET lampsInCurrentTray = (SELECT COUNT(TestTray.LampID) FROM TestTray WHERE TestTray.ttid = currentTray AND TestTray.asid = StationID);


	IF ((FLOOR(RAND() * 10) % 2) = 1) THEN
		SET workingLamp = TRUE;
	END IF;


	IF currentTray IS NULL THEN
		SET currentTray = 1;
	END IF;

	IF lampsInCurrentTray < 60 THEN
		-- Inserting into the current tray
		INSERT INTO TestTray
			(ttid, asid, lampID, LampCode, IsThisFineElegantLampWorking)
		VALUES
			(currentTray, stationID, lampID, GetLampCode(currentTray, lampsInCurrentTray + 1), workingLamp);

	ELSE
		-- Create new tray
		INSERT INTO TestTray 
			(ttid, asid, lampID, LampCode, IsThisFineElegantLampWorking) 
		VALUES
			(currentTray + 1, stationID, lampID, GetLampCode(currentTray + 1, 1), workingLamp);

	END IF;
END //


DROP PROCEDURE IF EXISTS KANBAN.RejectTestTray //
CREATE PROCEDURE KANBAN.RejectTestTray( IN trayID INT, IN stationID INT )
BEGIN
	UPDATE KANBAN.TestTray
	SET IsThisFineElegantLampWorking = 0
	WHERE ttid = trayID
	AND asid = stationID;
END //


DELIMITER ;