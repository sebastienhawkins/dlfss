ALTER TABLE `driver_lap` DROP COLUMN `guid`;
ALTER TABLE `driver_lap` ADD KEY `car_prefix` (`car_prefix`);
ALTER TABLE `driver_lap` ADD KEY `track_prefix` (`track_prefix`);
