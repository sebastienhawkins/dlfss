-- add bad driving skill count
ALTER TABLE `driver_lap` ADD COLUMN `bad_driving_count` int(8) unsigned NOT NULL DEFAULT '0' AFTER `blue_flag_count`;
