-- Add a new Colum, that will contain the brake distance needed by a car from 100Khm, this should be a average.
ALTER TABLE `car_template` ADD COLUMN `brake_distance` smallint(3) unsigned NOT NULL default 0 after `name`;

-- Add sample data, this is not accurate, just a start.
UPDATE `car_template` SET `brake_distance` = 40 WHERE `entry`=1;
UPDATE `car_template` SET `brake_distance` = 35 WHERE `entry`=2;
UPDATE `car_template` SET `brake_distance` = 35 WHERE `entry`=3;
UPDATE `car_template` SET `brake_distance` = 30 WHERE `entry`=4;
UPDATE `car_template` SET `brake_distance` = 25 WHERE `entry`=5;
UPDATE `car_template` SET `brake_distance` = 35 WHERE `entry`=6;
UPDATE `car_template` SET `brake_distance` = 35 WHERE `entry`=7;
UPDATE `car_template` SET `brake_distance` = 35 WHERE `entry`=8;
UPDATE `car_template` SET `brake_distance` = 35 WHERE `entry`=9;
UPDATE `car_template` SET `brake_distance` = 30 WHERE `entry`=10;
UPDATE `car_template` SET `brake_distance` = 20 WHERE `entry`=11;
UPDATE `car_template` SET `brake_distance` = 20 WHERE `entry`=12;
UPDATE `car_template` SET `brake_distance` = 20 WHERE `entry`=13;
UPDATE `car_template` SET `brake_distance` = 20 WHERE `entry`=14;
UPDATE `car_template` SET `brake_distance` = 20 WHERE `entry`=15;
UPDATE `car_template` SET `brake_distance` = 10 WHERE `entry`=16;
UPDATE `car_template` SET `brake_distance` = 15 WHERE `entry`=17;
UPDATE `car_template` SET `brake_distance` = 15 WHERE `entry`=18;
UPDATE `car_template` SET `brake_distance` = 20 WHERE `entry`=19;
UPDATE `car_template` SET `brake_distance` = 15 WHERE `entry`=20;

