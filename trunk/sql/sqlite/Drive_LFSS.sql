



-- Car Template
DROP TABLE IF EXISTS "car_template";
CREATE TABLE `car_template` (
	`entry` INTEGER PRIMARY KEY  NOT NULL  DEFAULT '',
	`abreviation_name` VARCHAR NOT NULL  DEFAULT '',
	`name` VARCHAR NOT NULL  DEFAULT '', 
	`traction_flag` INTEGER NOT NULL  DEFAULT '');

-- Driver
DROP TABLE IF EXISTS "driver";
CREATE TABLE `driver` (
	`guid` INTEGER PRIMARY KEY  NOT NULL  DEFAULT '',
	`guid_licence` INTEGER NOT NULL  DEFAULT '',
	`name` VARCHAR NOT NULL  DEFAULT '');
CREATE UNIQUE INDEX `guid_licence_driver_name` 
ON `driver` (`guid_licence` ASC, `name` ASC);

-- Licence
DROP TABLE IF EXISTS "licence";
CREATE TABLE `licence` (
	`guid` INTEGER PRIMARY KEY  NOT NULL  DEFAULT '',
	`name` VARCHAR NOT NULL  DEFAULT '');
CREATE UNIQUE INDEX `name` ON `licence` (`name` ASC);

-- Licence Config
DROP TABLE IF EXISTS "licence_config";
CREATE TABLE `licence_config` (
	`guid_licence` INTEGER PRIMARY KEY  NOT NULL  DEFAULT '',
	`config_flag` INTEGER NOT NULL  DEFAULT '0');

-- Race
DROP TABLE IF EXISTS "race";
CREATE TABLE `race` (
	`guid` INTEGER PRIMARY KEY  NOT NULL  DEFAULT '',
	`entry_track` INTEGER NOT NULL  DEFAULT '',
	`start_time` INTEGER NOT NULL  DEFAULT '',
	`end_time` INTEGER NOT NULL  DEFAULT '',
	`grid_order` VARCHAR DEFAULT '',
	`start_car_count` INTEGER NOT NULL  DEFAULT '0',
	`end_car_count` INTEGER NOT NULL  DEFAULT '0',
	`start_connection_count` INTEGER NOT NULL  DEFAULT '0',
	`end_connection_count` INTEGER NOT NULL  DEFAULT '0');

-- Race Lap
DROP TABLE IF EXISTS "race_lap";
CREATE TABLE `race_lap` (
	`guid` INTEGER PRIMARY KEY  NOT NULL  DEFAULT '',
	`guid_race` INTEGER NOT NULL  DEFAULT '',
	`guid_driver` INTEGER NOT NULL  DEFAULT '', 
	`entry_car` INTEGER NOT NULL  DEFAULT '0',
	`split1` INTEGER NOT NULL  DEFAULT '0',
	`split2` INTEGER NOT NULL  DEFAULT '0',
	`split3` INTEGER NOT NULL  DEFAULT '0',
	`lap_out_of_total` INTEGER NOT NULL  DEFAULT '0',
	`yellow_flag_count` INTEGER NOT NULL  DEFAULT '0',
	`blue_flag_count` INTEGER NOT NULL  DEFAULT '0');
CREATE UNIQUE INDEX `race` ON `race_lap` (`guid_race` ASC);
CREATE UNIQUE INDEX `driver` ON `race_lap` (`guid_driver` ASC);

-- Race Penality
DROP TABLE IF EXISTS "race_penality";
CREATE TABLE `race_penality` (
	`guid` INTEGER PRIMARY KEY  DEFAULT '',
	`guid_lap` INTEGER NOT NULL  DEFAULT '',
	`guid_race` INTEGER NOT NULL  DEFAULT '',
	`guid_driver` INTEGER NOT NULL  DEFAULT '',
	`apply_flag` INTEGER NOT NULL  DEFAULT '0',
	`remove_flag` INTEGER NOT NULL  DEFAULT '0');
CREATE UNIQUE INDEX `lap` ON `race_penality` (`guid_lap` ASC);
CREATE UNIQUE INDEX `race` ON `race_penality` (`guid_race` ASC);
CREATE UNIQUE INDEX `driver` ON `race_penality` (`guid_driver` ASC);

-- Track Template
DROP TABLE IF EXISTS "track_template";
CREATE TABLE `track_template` (
	`entry` INTEGER PRIMARY KEY  NOT NULL  DEFAULT '',
	`abreviation_name` VARCHAR NOT NULL  DEFAULT '',
	`name` VARCHAR NOT NULL  DEFAULT '',
	`node_count` INTEGER NOT NULL  DEFAULT '',
	`node_split1_index` INTEGER NOT NULL  DEFAULT '0',
	`node_split2_index` INTEGER NOT NULL  DEFAULT '0', 
	`node_split3_index` INTEGER NOT NULL  DEFAULT '0');

	

