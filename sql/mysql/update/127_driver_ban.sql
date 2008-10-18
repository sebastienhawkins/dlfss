-- Fixing some default value
ALTER TABLE `driver_ban` CHANGE `licence_name` `licence_name` varchar(16) NOT NULL default '';
ALTER TABLE `driver_ban` CHANGE `from_licence_name` `from_licence_name` varchar(16) NOT NULL default '';
ALTER TABLE `driver_ban` CHANGE `reason` `reason` varchar(255) NOT NULL default '';
ALTER TABLE `driver_ban` CHANGE `start_timestamp` `start_timestamp` int(12) unsigned NOT NULL default '0';
