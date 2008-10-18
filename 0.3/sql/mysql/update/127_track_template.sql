-- Fixing some default value
ALTER TABLE `track_template` CHANGE `name_prefix` `name_prefix` varchar(4) NOT NULL default '';
ALTER TABLE `track_template` CHANGE `name` `name` varchar(16) NOT NULL default '';
ALTER TABLE `track_template` CHANGE `configuration` `configuration` varchar(16) NOT NULL default '';
