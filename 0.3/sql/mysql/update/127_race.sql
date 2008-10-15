-- Fixing some default value
ALTER TABLE `race` CHANGE `qualify_race_guid` `qualify_race_guid` int(11) unsigned NOT NULL default '0';
ALTER TABLE `race` CHANGE `end_timestamp` `end_timestamp` bigint(12) unsigned NOT NULL default '0';
ALTER TABLE `race` CHANGE `finish_order` `finish_order` varchar(255) NOT NULL default '';
ALTER TABLE `race` CHANGE `race_laps` `race_laps` tinyint(3) unsigned NOT NULL default '0';
ALTER TABLE `race` CHANGE `qualification_minute` `qualification_minute` tinyint(3) unsigned NOT NULL default '0';
ALTER TABLE `race` CHANGE `weather_status` `weather_status` tinyint(2) unsigned NOT NULL default '0';
ALTER TABLE `race` CHANGE `wind_status` `wind_status` tinyint(2) unsigned NOT NULL default '0';
