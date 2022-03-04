-- ---------------------------------------------------------------
-- Filename: simSchema.sql
-- Project: ASQL Assignment 2 (W14)
-- Author: Ryan Pink & Dan Hieronimus
-- Date Modified: 2014-02-09
-- Description:
-- This file builds the KANBAN schema necessary to run
-- a simulation as specified in the requirements. It also
-- creates the triggers for the tables.
-- ---------------------------------------------------------------
-- Initialize with a clean implementation of the kanban database
-- ---------------------------------------------------------------
DROP DATABASE IF EXISTS KANBAN;
CREATE DATABASE KANBAN;


-- ---------------------------------------------------------------
-- Create the tables in the KANBAN database
-- ---------------------------------------------------------------
-- -- AssemblyStation Table
DROP TABLE IF EXISTS KANBAN.AssemblyStation;
CREATE TABLE KANBAN.AssemblyStation (
	asid  	 integer AUTO_INCREMENT NOT NULL, -- Uniquely identifying integer
	time     integer,			 			  -- How quickly the worker completes a part
	variance integer,			 			  -- + or - variance on time above

	harnesses  integer,			 			  -- Number of harnesses in the bin
	reflectors integer,			 			  -- Number of reflectors in the bin
	housings   integer,			 			  -- Number of housings in the bin
	lenses     integer,			 			  -- Number of lenses in the bin
	bulbs	   integer,			 			  -- Number of bulbs in the bin
	bezels	   integer,			 			  -- Number of bezels in the bin

	PRIMARY KEY (asid)
);


-- -- TestTray Table
DROP TABLE IF EXISTS KANBAN.TestTray;
CREATE TABLE KANBAN.TestTray (
	ttid integer NOT NULL, -- Test Tray ID
	asid integer NOT NULL,				  -- Assembly Station ID
	LampID 		INT NOT NULL,
	LampCode 	VARCHAR(255),
	IsThisFineElegantLampWorking BOOL,

	PRIMARY KEY (ttid, LampID),

	FOREIGN KEY (asid) REFERENCES KANBAN.AssemblyStation(asid)
);


-- -- Lamp Table
DROP TABLE IF EXISTS KANBAN.Lamp;
CREATE TABLE KANBAN.Lamp (
	lampid 		  integer AUTO_INCREMENT NOT NULL,	-- Lamp Part ID
	asid		  integer NOT NULL,					-- Assembly Station ID
	time2complete integer NOT NULL, 				-- Time to Complete Part (in seconds)

	PRIMARY KEY (lampid),
	
	FOREIGN KEY (asid) REFERENCES KANBAN.AssemblyStation(asid)
);


-- -- CardTray Table
DROP TABLE IF EXISTS KANBAN.CardTray;
CREATE TABLE KANBAN.CardTray (
	asid integer NOT NULL,  -- Assembly Station ID
	bin  integer NOT NULL,  -- ID representing bin to fill

	PRIMARY KEY (asid, bin),

	FOREIGN KEY (asid) REFERENCES KANBAN.AssemblyStation(asid)
);


-- -- Bin Requests Table
DROP TABLE IF EXISTS KANBAN.RequestMadeToJamieTheRunnerToFillUpMyBin;
CREATE TABLE KANBAN.RequestMadeToJamieTheRunnerToFillUpMyBin (
	harnessReqs INT,
	reflectorReqs INT,
	housingReqs INT,
	lensReqs INT,
	bulbReqs INT,
	bezelReqs INT,

	PRIMARY KEY (harnessReqs, reflectorReqs, housingReqs, lensReqs, bulbReqs, bezelReqs)
);


DELIMITER ;