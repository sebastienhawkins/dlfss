-- Change table name
ALTER TABLE `driver_lap` CHANGE `max_speed_kmh` `max_speed_ms` float NOT NULL default '0.0';
