-- Fixing some default value
ALTER TABLE `car_template` CHANGE `name_prefix` `name_prefix` varchar(3) NOT NULL default '';
ALTER TABLE `car_template` CHANGE `name` `name` varchar(16) NOT NULL default '';
