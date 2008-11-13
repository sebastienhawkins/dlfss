-- Fixing some default value
-- ALTER TABLE `driver` CHANGE `last_connection_time` `last_connection_time` bigint(12) unsigned NOT NULL default '0';
ALTER TABLE `driver` ADD COLUMN `tutorial` varchar(255) NOT NULL DEFAULT '' AFTER `config_data`;
ALTER TABLE `driver` ADD COLUMN `time_spec` int(12) unsigned NOT NULL DEFAULT '0' AFTER  `last_connection_time`;
ALTER TABLE `driver` ADD COLUMN `time_garage` int(12) unsigned NOT NULL DEFAULT '0' AFTER  `time_spec`;
ALTER TABLE `driver` ADD COLUMN `time_racing` int(12) unsigned NOT NULL DEFAULT '0' AFTER  `time_garage`;
ALTER TABLE `driver` ADD COLUMN `password` varchar(128) NOT NULL DEFAULT '' AFTER  `driver_name`;
ALTER TABLE `driver` ADD COLUMN `flood_chat_count` int(8) unsigned NOT NULL DEFAULT '0' AFTER  `warning_chat_count`;