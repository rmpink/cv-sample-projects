-- ---------------------------------------------------------------
-- Filename: simLoop.sql
-- Project: ASQL Assignment 2 (W14)
-- Author: Ryan Pink & Dan Hieronimus
-- Date Modified: 2014-02-09
-- Description:
-- This file runs the main simulation loop for the KANBAN
-- demonstration.
-- ---------------------------------------------------------------
START TRANSACTION;

CALL KANBAN.SimLoop();	  -- Start the simulation!

COMMIT;