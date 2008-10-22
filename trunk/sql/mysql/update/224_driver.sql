-- Put Driver with Unique licenceName
-- Delete all Non Unique Laps and Driver
DROP TEMPORARY TABLE IF EXISTS `driver_2`;
CREATE TEMPORARY TABLE  driver_2  
(guid int(10),name varchar(64), 
PRIMARY KEY (`name`));
INSERT IGNORE INTO driver_2 SELECT `guid`,`licence_name` FROM `driver`;
DELETE FROM `driver_lap` WHERE `guid_driver` NOT IN (SELECT `guid` FROM `driver_2`);
DELETE FROM `driver` WHERE `guid` NOT IN (SELECT `guid` FROM `driver_2`);
DROP TEMPORARY TABLE IF EXISTS `driver_2`;

ALTER TABLE `driver` DROP INDEX `MapLicenceDriver`;
ALTER TABLE `driver` ADD UNIQUE INDEX (`licence_name`);
