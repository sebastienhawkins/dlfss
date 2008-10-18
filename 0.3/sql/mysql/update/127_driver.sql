-- Fixing some default value
ALTER TABLE `driver` CHANGE `last_connection_time` `last_connection_time` bigint(12) unsigned NOT NULL default '0';
