-- adding a MaxSpeedKmh stats for each lap.
ALTER TABLE `driver_lap` ADD COLUMN `max_speed_kmh` float NOT NULL default 0 after `lap_completed`;

-- Fixing some default value
ALTER TABLE `driver_lap` CHANGE `split_time_1` `split_time_1` int(12) unsigned NOT NULL default '0';
ALTER TABLE `driver_lap` CHANGE `pit_stop_count` `pit_stop_count` tinyint(3) unsigned NOT NULL default '0';
