DROP DATABASE IF EXISTS TESTDB;
CREATE DATABASE TESTDB;
USE TESTDB;

SET @StationOneCurTray = 1;
SET @StationOneCurTrayCount = 0;

SET @StationTwoCurTray = 1;
SET @StationTwoCurTrayCount = 0;

SET @StationThreeCurTray = 1;
SET @StationThreeCurTrayCount = 0;




DROP TABLE IF EXISTS Lamp;
CREATE TABLE Lamp
(
	LampID INT NOT NULL AUTO_INCREMENT,

	PRIMARY KEY (LampID)
);



DROP TABLE IF EXISTS TestTray;
CREATE TABLE TestTray
(
	TrayID INT NOT NULL,
	StationID INT NOT NULL,
	LampID INT NOT NULL,
	LampCode VARCHAR(255),
	IsThisFineElegantLampWorking BOOL,

	PRIMARY KEY (TrayID, LampID)
);





DELIMITER //
DROP PROCEDURE IF EXISTS PlaceInTestTray //
CREATE PROCEDURE PlaceInTestTray(IN stationID INT, IN lampID INT)
BEGIN

DECLARE currentTray INT DEFAULT NULL;
DECLARE lampsInCurrentTray INT DEFAULT NULL;
DECLARE workingLamp BOOL DEFAULT FALSE;

SET currentTray = (SELECT TrayID FROM TestTray WHERE TestTray.stationID=stationID ORDER BY TrayID DESC LIMIT 1);
SET lampsInCurrentTray = (SELECT COUNT(TestTray.LampID) FROM TestTray WHERE TestTray.TrayID = currentTray AND TestTray.StationID = StationID);


IF ((FLOOR(RAND() * 10) % 2) = 1) THEN
	SET workingLamp = TRUE;
END IF;


IF currentTray IS NULL THEN
	SET currentTray = 1;
END IF;

	IF lampsInCurrentTray < 60 THEN
		-- Inserting into the current tray
		INSERT INTO TestTray(trayID, stationID, lampID, LampCode, IsThisFineElegantLampWorking) VALUES
		(currentTray, stationID, lampID, GetLampCode(currentTray, lampsInCurrentTray + 1), workingLamp);

	ELSE
		-- Create new tray
		INSERT INTO TestTray (trayID, stationID, lampID, LampCode, IsThisFineElegantLampWorking) VALUES
		(currentTray + 1, stationID, lampID, GetLampCode(currentTray + 1, 1), workingLamp);

	END IF;


END //
DELIMITER ;

DELIMITER //
DROP FUNCTION IF EXISTS GetLampCode //
CREATE FUNCTION GetLampCode(trayID INT, trayPosition INT) 
RETURNS CHAR(40)
DETERMINISTIC
BEGIN

DECLARE lampCode CHAR(40) DEFAULT "FL";

SET lampCode = CONCAT(lampCode, LPAD(trayID, 6, 0));
SET lampCode = CONCAT(lampCode, LPAD(trayPosition, 2, 0));

RETURN lampCode;

END //
DELIMITER ;


DELIMITER //
DROP PROCEDURE IF EXISTS PlaceInTrayTest //
CREATE PROCEDURE PlaceInTrayTest(IN numberToPlace INT)
BEGIN

DECLARE loopIteration INT DEFAULT 1;

WHILE loopIteration < numberToPlace DO


CALL PlaceInTestTray(FLOOR(RAND() * 10) % 3 + 1, loopIteration);
SET loopIteration = loopIteration + 1;
END WHILE;


END //
DELIMITER ;