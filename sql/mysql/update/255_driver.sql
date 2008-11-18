-- Fixing some default value
-- ALTER TABLE `driver` CHANGE `last_connection_time` `last_connection_time` bigint(12) unsigned NOT NULL default '0';
ALTER TABLE `driver` ADD COLUMN `tutorial` varchar(255) NOT NULL DEFAULT '' AFTER  `config_data`;
ALTER TABLE `driver` ADD COLUMN `time_spec` int(12) unsigned NOT NULL DEFAULT '0' AFTER `last_connection_time`;
ALTER TABLE `driver` ADD COLUMN `time_garage` int(12) unsigned NOT NULL DEFAULT '0' AFTER `time_spec`;
ALTER TABLE `driver` ADD COLUMN `time_racing` int(12) unsigned NOT NULL DEFAULT '0' AFTER `time_garage`;
ALTER TABLE `driver` ADD COLUMN `password` varchar(128) NOT NULL DEFAULT '' AFTER `driver_name`;

ALTER TABLE `driver` ADD COLUMN `race_count` int(8) unsigned NOT NULL DEFAULT '0' AFTER  `tutorial`;
ALTER TABLE `driver` ADD COLUMN `race_finish_count` int(8) unsigned NOT NULL DEFAULT '0' AFTER  `race_count`;
ALTER TABLE `driver` ADD COLUMN `lap_count` int(8) unsigned NOT NULL DEFAULT '0' AFTER  `race_finish_count`;

-- Laps count
UPDATE `driver`,
(SELECT `guid_driver`,COUNT(`guid_driver`) As `Count` 
FROM `driver_lap` GROUP BY `guid_driver`) As `Laps`
SET `lap_count`=`laps`.`Count` WHERE `guid`=`Laps`.`guid_driver`;

-- race count
UPDATE `driver` As iDriver
SET `race_count`= 
(SELECT COUNT(`race`.`guid`) FROM `race` WHERE (LOCATE(`iDriver`.`guid`,`grid_order`) > 0 OR LOCATE(`iDriver`.`guid`,`grid_order`) > 0) AND `race_laps`>1);

-- race finish count
UPDATE `driver` As iDriver
SET `race_finish_count`= 
(SELECT COUNT(`race`.`guid`) FROM `race` WHERE (LOCATE(`iDriver`.`guid`,`finish_order`) > 0 OR LOCATE(`iDriver`.`guid`,`finish_order`) > 0) AND `race_laps`>1);