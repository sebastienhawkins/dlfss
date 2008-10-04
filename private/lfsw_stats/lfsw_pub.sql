DELETE FROM `pub_race` WHERE `guid`='';
DELETE FROM `pub_race` WHERE `guid` IS NULL;

DELETE FROM `pub_race` WHERE `server_name`='';
DELETE FROM `pub_race` WHERE `server_name` IS NULL;

DELETE FROM `pub_race` WHERE `time`='';
DELETE FROM `pub_race` WHERE `time` IS NULL;

DELETE FROM `pub_race` WHERE `track_name`='';
DELETE FROM `pub_race` WHERE `track_name` IS NULL;


-- Lap Data
DELETE FROM `lapdata` WHERE `guid`='';
DELETE FROM `lapdata` WHERE `guid` IS NULL;

DELETE FROM `lapdata` WHERE `licencename`='';
DELETE FROM `lapdata` WHERE `licencename` IS NULL;

DELETE FROM `lapdata` WHERE `time`='';
DELETE FROM `lapdata` WHERE `time` IS NULL;

DELETE FROM `lapdata` WHERE `track_name`='';
DELETE FROM `lapdata` WHERE `track_name` IS NULL;

SELECT DISTINCT `guid`,`licencename` FROM `lapdata`;


-- 15:10, 21 Aug 2007
DROP TABLE IF EXISTS `race_data`;
CREATE TABLE `race_data` (
  `guid` int(11) NOT NULL,
  `server_name` varchar(128) NOT NULL default '',
  `time_start` timestamp NOT NULL default '0000-00-00 00:00:00',
  `track_name` varchar(64) NOT NULL default '',
  PRIMARY KEY  (`guid`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;


DROP TABLE IF EXISTS `lap_data`;
CREATE TABLE `lap_data` (
  `guid` int(11) NOT NULL,
  `position` tinyint(2) NOT NULL default '0',
  `licence_name` varchar(128) NOT NULL default '',
  `car_prefix` varchar(3) NOT NULL default '',
  `lap_count` smallint(3) NOT NULL default '0',
  `time_total` int(11) NOT NULL default '0',
  `penality` varchar(10) NOT NULL default '',
  `time_best_lap` int(11) NOT NULL default '0',
  `best_lap_number` smallint(3) NOT NULL default '0',
  `pit_stop_count` smallint(3) NOT NULL default '0',
  `takeover` smallint(3) NOT NULL default '0',
  `laps_time` blob NOT NULL,
  PRIMARY KEY  (`guid`,`licence_name`(32))
) ENGINE=MyISAM DEFAULT CHARSET=latin1;

INSERT IGNORE INTO lfs_pub_stats SELECT * FROM lap_data;
SELECT `guid` FROM `race_data` WHERE guid NOT IN (SELECT `guid` FROM lap_data);
DELETE  FROM `race_data` WHERE guid NOT IN (SELECT `guid` FROM lap_data);

SELECT `guid` FROM `lap_data` WHERE guid NOT IN (SELECT `guid` FROM race_data);

SELECT COUNT(`guid`) FROM `lapdata`;

-- All racer for a Track+Car from last 500000 
SELECT `licence_name` FROM `lapdata` WHERE `car_prefix`='FBM' AND `guid` > ((SELECT MAX(`guid`) FROM `lapdata`)-500000) AND `guid` IN (SELECT `guid` FROM `racedata` WHERE `guid` > ((SELECT MAX(`guid`) FROM `lapdata`)-500000) AND `track_name`='Blackwood Gp');

-- find driver
SELECT DISTINCT `licence_name` FROM `lapdata` WHERE `licence_name` LIKE '%bun%';

-- ALL best time for a Racer+Track+Car last 500000
SELECT `time_best_lap` FROM `lap_data` WHERE `guid` IN (SELECT `guid` FROM `race_data` WHERE `guid` > ((SELECT MAX(`guid`) FROM `lap_data`)-500000) AND `track_name`='Southcity Sprint Track 1' ) AND `car_prefix`='XFG' AND `licence_name`='greenseed';


-- ALL best time and car for a Racer+Track last 500000
SELECT `time_best_lap`,`car_prefix` FROM `lap_data` WHERE `guid` IN (SELECT `guid` FROM `race_data` WHERE `guid` > ((SELECT MAX(`guid`) FROM `lap_data`)-500000) AND `track_name`='Blackwood Gp' ) AND `licence_name`='Greenseed';


-- ALL best time and car and track for a Racer last 500000
SELECT `lap_data`.`licence_name`,`lap_data`.`time_best_lap`,`lap_data`.`car_prefix`,`race_data`.`track_name` 
FROM `lap_data`,`race_data` 
WHERE  
`race_data`.`guid` > ((SELECT MAX(`guid`) FROM `lap_data`)-500000) 
AND `lap_data`.`licence_name`='greenseed' 
AND `lap_data`.`guid`=`race_data`.`guid` 
ORDER BY `race_data`.`track_name`,`lap_data`.`car_prefix`;


-- Fastest with Car,Track from driver
SELECT `lap_data`.`licence_name`,MIN(`lap_data`.`time_best_lap`) ,`lap_data`.`car_prefix`,`race_data`.`track_name`
FROM `lap_data`,`race_data`
WHERE
`lap_data`.`licence_name`='greenseed' 
AND `lap_data`.`guid`=`race_data`.`guid`
GROUP BY `lap_data`.`car_prefix`,`race_data`.`track_name`;

SELECT count(guid) from lap_data where time_best_lap =0;

-- fastest car track anyone
SELECT `lap_data`.`licence_name`,MIN(`lap_data`.`time_best_lap`) ,`lap_data`.`car_prefix`,`race_data`.`track_name`
FROM `lap_data`,`race_data`
WHERE `lap_data`.`guid`=`race_data`.`guid`
GROUP BY `lap_data`.`car_prefix`,`race_data`.`track_name` ;


-- Average , car,track
SELECT `lap_data`.`licence_name`,AVG(`lap_data`.`time_best_lap`) ,`lap_data`.`car_prefix`,`race_data`.`track_name`
FROM `lap_data`,`race_data`
WHERE `lap_data`.`guid`=`race_data`.`guid`
GROUP BY `lap_data`.`car_prefix`,`race_data`.`track_name` ;

-- Average , car,track from driver
SELECT `lap_data`.`licence_name`,AVG(`lap_data`.`time_best_lap`) ,`lap_data`.`car_prefix`,`race_data`.`track_name`
FROM `lap_data`,`race_data`
WHERE
`lap_data`.`licence_name`='greenseed' 
AND `lap_data`.`guid`=`race_data`.`guid`
GROUP BY `lap_data`.`car_prefix`,`race_data`.`track_name`;


SELECT `lap_data`.`time_best_lap`
FROM `lap_data`,`race_data`
WHERE `lap_data`.`licence_name`='greenseed' 
AND `lap_data`.`car_prefix`='XFG'
AND `race_data`.`track_name`='Blackwood GP Track'
AND `lap_data`.`guid`=`race_data`.`guid`
AND `lap_data`.`time_best_lap` <> '0'
ORDER BY `lap_data`.`time_best_lap` LIMIT 1;
						
						
SELECT `lap_data`.`time_best_lap`
FROM `lap_data`,`race_data`
WHERE `lap_data`.`car_prefix`='FBM'
AND `lap_data`.`time_best_lap` != '0'
AND `lap_data`.`guid`=`race_data`.`guid`
AND `race_data`.`track_name`='Blackwood GP'
ORDER BY `lap_data`.`time_best_lap` LIMIT 1

SELECT DISTINCT `guid`,COUNT(`guid`) FROM `lap_data` GROUP BY `guid` ORDER by 2;

LOCK TABLES `race_data`,`lap_data`



DELETE `lap_data`,`race_data` FROM `race_data`,`lap_data` WHERE `lap_data`.`guid`=`race_data`.`guid` AND `race_data`.`time_start` <= '2007-12-31 23:59:59' ;
UNLOCK TABLES;

UPDATE `lap_data` SET time_best_lap=0 WHERE < 5000;