-- MySQL dump 10.10
--
-- Host: 127.0.0.1    Database: drive_lfss
-- ------------------------------------------------------
-- Server version	5.0.27-community-nt

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `button_template`
--

DROP TABLE IF EXISTS `button_template`;
CREATE TABLE `button_template` (
  `entry` mediumint(8) unsigned NOT NULL,
  `description` varchar(255) NOT NULL default '',
  `style_mask` tinyint(3) unsigned NOT NULL default '0',
  `is_allways_visible` tinyint(1) unsigned NOT NULL default '0',
  `max_input_char` tinyint(3) unsigned NOT NULL default '0',
  `left` tinyint(3) unsigned NOT NULL default '0',
  `top` tinyint(3) unsigned NOT NULL default '0',
  `width` tinyint(3) unsigned NOT NULL default '0',
  `height` tinyint(3) unsigned NOT NULL default '0',
  `text` varchar(240) NOT NULL default '',
  `caption_text` varchar(240) NOT NULL default '',
  PRIMARY KEY  (`entry`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

--
-- Dumping data for table `button_template`
--

LOCK TABLES `button_template` WRITE;
/*!40000 ALTER TABLE `button_template` DISABLE KEYS */;
INSERT INTO `button_template` VALUES (1,'banner',66,1,0,35,194,20,7,'^7A^3leajecta','');
INSERT INTO `button_template` VALUES (2,'motd background',98,1,0,0,0,200,200,'','');
INSERT INTO `button_template` VALUES (10,'vote option 1',106,0,0,2,73,45,6,'vote option 1','');
INSERT INTO `button_template` VALUES (11,'vote option 2',106,0,0,2,80,45,6,'vote option 2','');
INSERT INTO `button_template` VALUES (3,'motd upper',144,1,0,45,60,110,7,'^8Motd ^7A^3leajecta','');
INSERT INTO `button_template` VALUES (6,'message bar top',0,0,0,25,27,150,12,'^1Message bar top','');
INSERT INTO `button_template` VALUES (7,'message bar middle',0,0,0,25,52,150,12,'^1Message bar Middle','');
INSERT INTO `button_template` VALUES (8,'Collision Warning',2,0,0,25,68,150,8,'^1CollisionWarning','');
INSERT INTO `button_template` VALUES (9,'vote title',146,0,0,2,65,50,8,'vote title','');
INSERT INTO `button_template` VALUES (4,'motd text line',98,1,0,50,67,100,8,'','');
INSERT INTO `button_template` VALUES (12,'vote option 3',106,0,0,2,87,45,6,'vote option 3','');
INSERT INTO `button_template` VALUES (13,'vote option 4',106,0,0,2,94,45,6,'vote option 4','');
INSERT INTO `button_template` VALUES (14,'vote option 5',106,0,0,2,101,45,6,'vote option 5','');
INSERT INTO `button_template` VALUES (15,'vote option 6',106,0,0,2,108,45,6,'vote option 6','');
INSERT INTO `button_template` VALUES (16,'track prefix',66,1,0,56,194,10,7,'^8','');
INSERT INTO `button_template` VALUES (17,'info 1',96,0,0,180,85,20,5,'info 1','');
INSERT INTO `button_template` VALUES (18,'info 2',80,0,0,180,91,20,5,'info 2','');
INSERT INTO `button_template` VALUES (19,'info 3',80,0,0,180,97,20,5,'info 3','');
INSERT INTO `button_template` VALUES (20,'info 4',80,0,0,180,103,20,5,'info 4','');
INSERT INTO `button_template` VALUES (21,'info 5',80,0,0,180,109,20,5,'info 5','');
INSERT INTO `button_template` VALUES (22,'config bg',32,0,0,0,0,200,200,'','');
INSERT INTO `button_template` VALUES (23,'config title and close',40,0,0,75,0,50,13,'^3User Config','');
INSERT INTO `button_template` VALUES (24,'config text close',0,0,0,115,8,8,5,'^2 Close','');
INSERT INTO `button_template` VALUES (25,'config acceleration bg',32,0,0,1,30,22,24,'','');
INSERT INTO `button_template` VALUES (26,'config acceleration title',8,0,0,2,31,20,7,'^7Acceleration','');
INSERT INTO `button_template` VALUES (27,'config acceleration start',40,0,131,2,39,9,6,'^2start','^3Start Kmh Speed ex: 0');
INSERT INTO `button_template` VALUES (28,'config acceleration end',40,0,131,13,39,9,6,'^2 end ','^3End Kmh Speed ex: 100');
INSERT INTO `button_template` VALUES (29,'config acceleration current',32,0,0,2,46,20,6,'^7','');
INSERT INTO `button_template` VALUES (30,'motd button config',24,1,0,120,146,35,12,'^2Config','');
INSERT INTO `button_template` VALUES (36,'motd button help',24,1,0,83,146,35,12,'^2Help','');
INSERT INTO `button_template` VALUES (5,'motd button drive',24,1,0,46,146,35,12,'^2Drive','');
INSERT INTO `button_template` VALUES (35,'help button config',24,1,0,102,146,48,12,'^2Config','');
INSERT INTO `button_template` VALUES (34,'help button drive',24,1,0,51,146,48,12,'^2Drive','');
INSERT INTO `button_template` VALUES (33,'help text',96,1,0,50,67,100,6,'^7','');
INSERT INTO `button_template` VALUES (32,'help upper',144,1,0,45,60,110,7,'^8Help ^7A^3leajecta','');
INSERT INTO `button_template` VALUES (31,'help bg',96,1,0,0,0,200,200,'','');
INSERT INTO `button_template` VALUES (39,'config help text',32,0,0,70,120,75,7,'','');
INSERT INTO `button_template` VALUES (38,'config drift title',8,0,0,26,31,20,7,'^7Drift Score','');
INSERT INTO `button_template` VALUES (37,'config drift bg',32,0,0,25,30,22,24,'','');
INSERT INTO `button_template` VALUES (43,'config timediff pb->split',8,0,0,50,46,20,6,'^7PB vs Split','');
INSERT INTO `button_template` VALUES (42,'config timediff pb->lap',8,0,0,50,39,20,6,'^7PB vs Lap','');
INSERT INTO `button_template` VALUES (40,'config timediff bg',32,0,0,49,30,22,24,'','');
INSERT INTO `button_template` VALUES (41,'config timediff title',8,0,0,50,31,20,7,'^7Time Diff','');
INSERT INTO `button_template` VALUES (44,'text line',32,0,0,1,45,110,6,'','');
INSERT INTO `button_template` VALUES (45,'text title and close',40,0,0,30,32,50,12,'^3Text Display','');
INSERT INTO `button_template` VALUES (46,'text close button',0,0,0,71,39,8,4,'^2close','');
/*!40000 ALTER TABLE `button_template` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `car_template`
--

DROP TABLE IF EXISTS `car_template`;
CREATE TABLE `car_template` (
  `entry` smallint(2) unsigned NOT NULL,
  `name_prefix` varchar(3) NOT NULL default '',
  `name` varchar(16) NOT NULL default '',
  `brake_distance` smallint(3) unsigned NOT NULL default '0',
  `mask` int(11) unsigned NOT NULL default '0',
  PRIMARY KEY  (`entry`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

--
-- Dumping data for table `car_template`
--

LOCK TABLES `car_template` WRITE;
/*!40000 ALTER TABLE `car_template` DISABLE KEYS */;
INSERT INTO `car_template` VALUES (1,'UF1','UF 1000',40,0);
INSERT INTO `car_template` VALUES (2,'XFG','XF GTI',31,0);
INSERT INTO `car_template` VALUES (3,'XRG','XR GT',31,0);
INSERT INTO `car_template` VALUES (4,'LX4','LX4',28,0);
INSERT INTO `car_template` VALUES (5,'LX6','LX6',22,0);
INSERT INTO `car_template` VALUES (6,'RB4','RB4 GT',26,0);
INSERT INTO `car_template` VALUES (7,'FXO','FXO TURBO',30,0);
INSERT INTO `car_template` VALUES (8,'XRT','XR GT TURBO',30,0);
INSERT INTO `car_template` VALUES (9,'RAC','RACEABOUT',32,0);
INSERT INTO `car_template` VALUES (10,'FZ5','FZ5',28,0);
INSERT INTO `car_template` VALUES (11,'UFR','UF GTR',24,0);
INSERT INTO `car_template` VALUES (12,'XFR','XF GTR',21,0);
INSERT INTO `car_template` VALUES (13,'FXR','FXO GTR',17,0);
INSERT INTO `car_template` VALUES (14,'XRR','XR GTR',18,0);
INSERT INTO `car_template` VALUES (15,'FZR','FZ50 GTR',17,0);
INSERT INTO `car_template` VALUES (16,'MRT','MRT5',17,0);
INSERT INTO `car_template` VALUES (17,'FBM','FORMULA BMW FB02',22,0);
INSERT INTO `car_template` VALUES (18,'FOX','FORMULA XR',16,0);
INSERT INTO `car_template` VALUES (19,'FO8','FORMULA V8',18,0);
INSERT INTO `car_template` VALUES (20,'BF1','BMW SAUBER F1.06',16,0);
/*!40000 ALTER TABLE `car_template` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `driver`
--

DROP TABLE IF EXISTS `driver`;
CREATE TABLE `driver` (
  `guid` int(11) unsigned NOT NULL auto_increment,
  `licence_name` varchar(24) NOT NULL,
  `driver_name` varchar(24) NOT NULL,
  `config_data` blob,
  `last_connection_time` bigint(12) unsigned NOT NULL default '0',
  PRIMARY KEY  (`guid`),
  UNIQUE KEY `MapLicenceDriver` (`licence_name`,`driver_name`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;

--
-- Dumping data for table `driver`
--

LOCK TABLES `driver` WRITE;
/*!40000 ALTER TABLE `driver` DISABLE KEYS */;
/*!40000 ALTER TABLE `driver` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `driver_ban`
--

DROP TABLE IF EXISTS `driver_ban`;
CREATE TABLE `driver_ban` (
  `entry` int(10) unsigned NOT NULL,
  `licence_name` varchar(16) NOT NULL default '',
  `from_licence_name` varchar(16) NOT NULL default '',
  `reason` varchar(255) NOT NULL default '',
  `start_timestamp` int(12) unsigned NOT NULL default '0',
  `duration_timestamp` int(12) unsigned NOT NULL default '0',
  `expired` tinyint(1) unsigned NOT NULL default '0',
  PRIMARY KEY  (`entry`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;

--
-- Dumping data for table `driver_ban`
--

LOCK TABLES `driver_ban` WRITE;
/*!40000 ALTER TABLE `driver_ban` DISABLE KEYS */;
/*!40000 ALTER TABLE `driver_ban` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `driver_lap`
--

DROP TABLE IF EXISTS `driver_lap`;
CREATE TABLE `driver_lap` (
  `guid` int(11) unsigned NOT NULL auto_increment,
  `guid_race` int(11) unsigned NOT NULL default '0',
  `guid_driver` int(11) unsigned NOT NULL,
  `car_prefix` varchar(3) NOT NULL,
  `track_prefix` varchar(4) NOT NULL,
  `driver_mask` mediumint(5) unsigned NOT NULL default '0',
  `split_time_1` int(12) unsigned NOT NULL,
  `split_time_2` int(12) unsigned NOT NULL default '0',
  `split_time_3` int(12) unsigned NOT NULL default '0',
  `lap_time` int(12) unsigned NOT NULL,
  `total_time` int(12) unsigned NOT NULL,
  `lap_completed` mediumint(5) unsigned NOT NULL default '0',
  `max_speed_ms` float NOT NULL default '0',
  `current_penalty` tinyint(2) unsigned NOT NULL default '0',
  `pit_stop_count` tinyint(3) unsigned NOT NULL,
  `yellow_flag_count` smallint(4) unsigned NOT NULL default '0',
  `blue_flag_count` mediumint(4) unsigned NOT NULL default '0',
  PRIMARY KEY  (`guid`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

--
-- Dumping data for table `driver_lap`
--

LOCK TABLES `driver_lap` WRITE;
/*!40000 ALTER TABLE `driver_lap` DISABLE KEYS */;
/*!40000 ALTER TABLE `driver_lap` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `gui_template`
--

DROP TABLE IF EXISTS `gui_template`;
CREATE TABLE `gui_template` (
  `entry` int(11) unsigned NOT NULL,
  `description` varchar(255) NOT NULL default '',
  `button_entry` varchar(255) NOT NULL default '' COMMENT 'space separated value',
  `text_button_entry` mediumint(5) unsigned NOT NULL default '0',
  `text` blob NOT NULL,
  PRIMARY KEY  (`entry`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

--
-- Dumping data for table `gui_template`
--

LOCK TABLES `gui_template` WRITE;
/*!40000 ALTER TABLE `gui_template` DISABLE KEYS */;
INSERT INTO `gui_template` VALUES (1,'motd','2 3 5 30 36',4,'^7Welcome To A^3leajecta ^7Live For Speed Server\r\n^7\r\n^7Our Server Are Currentely Under Developement\r\n^7We Implemente Drive_LFSS a InSim Addons. \r\n^7\r\n^7To know more about this Visit www.lfsforum.net\r\n^7 Your Host A^3leajecta^7.');
INSERT INTO `gui_template` VALUES (2,'user config','22 23 24 25 26 27 28 29 37 38 40 41 42 43',39,'^8Config Help\r\n^7Title ^3 will enable or disable the feature\r\n^3Green ^2button ^3will trigger a action for this feature.');
INSERT INTO `gui_template` VALUES (3,'help','31 32 33 34 35',33,'^7A^3leajecta ^7is powered by Drive_LFSS 0.2 Alpha\r\n^7\r\n^7This is a alpha stage of the developement\r\n^7there is a lot more to come, please be patien.\r\n^7\r\n^7List Of Commands:\r\n^2!help^7, ^2!config^7,^2!status^7, ^5!kick^7, ^5!exit^7, ^5!reload\r\n^7\r\n^7Did you know \"SHIFT-i\" will reset button and make Config screen appear.\r\n^7\r\n^5Greenseed^7.');
INSERT INTO `gui_template` VALUES (4,'text','45 46',44,'');
/*!40000 ALTER TABLE `gui_template` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `race`
--

DROP TABLE IF EXISTS `race`;
CREATE TABLE `race` (
  `guid` int(11) unsigned NOT NULL auto_increment,
  `qualify_race_guid` int(11) unsigned NOT NULL default '0',
  `track_prefix` varchar(4) NOT NULL,
  `start_timestamp` bigint(12) unsigned NOT NULL,
  `end_timestamp` bigint(12) unsigned NOT NULL default '0',
  `grid_order` varchar(255) NOT NULL COMMENT 'Driver GUID seperated by Space.',
  `finish_order` varchar(255) NOT NULL default '',
  `race_laps` tinyint(3) unsigned NOT NULL default '0',
  `race_status` tinyint(1) unsigned NOT NULL default '0',
  `race_feature` mediumint(4) unsigned NOT NULL default '0',
  `qualification_minute` tinyint(3) unsigned NOT NULL default '0',
  `weather_status` tinyint(2) unsigned NOT NULL default '0',
  `wind_status` tinyint(2) unsigned NOT NULL default '0',
  PRIMARY KEY  (`guid`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

--
-- Dumping data for table `race`
--

LOCK TABLES `race` WRITE;
/*!40000 ALTER TABLE `race` DISABLE KEYS */;
/*!40000 ALTER TABLE `race` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `race_map`
--

DROP TABLE IF EXISTS `race_map`;
CREATE TABLE `race_map` (
  `entry` mediumint(5) unsigned NOT NULL default '0',
  `race_template_entry` mediumint(5) unsigned NOT NULL default '0',
  PRIMARY KEY  (`entry`,`race_template_entry`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

--
-- Dumping data for table `race_map`
--

LOCK TABLES `race_map` WRITE;
/*!40000 ALTER TABLE `race_map` DISABLE KEYS */;
INSERT INTO `race_map` VALUES (2,1);
INSERT INTO `race_map` VALUES (2,2);
INSERT INTO `race_map` VALUES (2,3);
INSERT INTO `race_map` VALUES (2,4);
INSERT INTO `race_map` VALUES (2,5);
INSERT INTO `race_map` VALUES (2,6);
INSERT INTO `race_map` VALUES (2,7);
INSERT INTO `race_map` VALUES (2,8);
INSERT INTO `race_map` VALUES (2,9);
INSERT INTO `race_map` VALUES (2,10);
INSERT INTO `race_map` VALUES (2,11);
INSERT INTO `race_map` VALUES (2,12);
INSERT INTO `race_map` VALUES (2,13);
INSERT INTO `race_map` VALUES (2,14);
INSERT INTO `race_map` VALUES (2,15);
INSERT INTO `race_map` VALUES (2,16);
INSERT INTO `race_map` VALUES (2,17);
INSERT INTO `race_map` VALUES (2,18);
INSERT INTO `race_map` VALUES (2,19);
INSERT INTO `race_map` VALUES (2,20);
/*!40000 ALTER TABLE `race_map` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `race_template`
--

DROP TABLE IF EXISTS `race_template`;
CREATE TABLE `race_template` (
  `entry` mediumint(5) unsigned NOT NULL,
  `description` varchar(34) NOT NULL default '',
  `track_entry` tinyint(2) unsigned NOT NULL default '0',
  `car_entry_allowed` varchar(64) NOT NULL default '0',
  `weather` tinyint(2) unsigned NOT NULL default '0',
  `wind` tinyint(2) unsigned NOT NULL default '0',
  `lap_count` tinyint(3) unsigned NOT NULL default '0',
  `qualify_minute` tinyint(3) unsigned NOT NULL default '0',
  `grid_start_beviator` tinyint(3) unsigned NOT NULL default '0',
  `race_template_mask` tinyint(3) unsigned NOT NULL default '0',
  PRIMARY KEY  (`entry`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

--
-- Dumping data for table `race_template`
--

LOCK TABLES `race_template` WRITE;
/*!40000 ALTER TABLE `race_template` DISABLE KEYS */;
INSERT INTO `race_template` VALUES (1,'South City Classic, 115Hp',6,'2 3',0,0,11,5,0,1);
INSERT INTO `race_template` VALUES (2,'South City Classic Rev,  115Hp',7,'2 3',1,1,12,5,0,1);
INSERT INTO `race_template` VALUES (3,'South City Sprint 1, 115Hp',8,'2 3',2,2,13,5,0,1);
INSERT INTO `race_template` VALUES (4,'South City Sprint 1 Rev, 115Hp',9,'2 3',3,3,14,5,0,1);
INSERT INTO `race_template` VALUES (5,'South City Sprint 2, 115Hp',10,'2 3',1,1,16,5,0,1);
INSERT INTO `race_template` VALUES (6,'South City Sprint 2 Rev, 115Hp',11,'2 3',2,1,15,5,0,1);
INSERT INTO `race_template` VALUES (7,'Blackwood GP Track, F08',1,'19',0,0,25,8,0,1);
INSERT INTO `race_template` VALUES (8,'Westhill, Turbo GTR',40,'13 14 15',0,0,10,8,0,1);
INSERT INTO `race_template` VALUES (9,'Westhill Rev, GTR Turbo',41,'13 14 15',0,0,10,8,0,1);
INSERT INTO `race_template` VALUES (10,'Aston Cadet Pratice, Formule',42,'16 17 18 19 20',0,0,0,0,0,5);
INSERT INTO `race_template` VALUES (11,'Aston Cadet, FBM',42,'17',0,0,10,12,0,1);
INSERT INTO `race_template` VALUES (12,'South City Sprint 1 Rev, FBM',9,'17',0,0,11,5,0,1);
INSERT INTO `race_template` VALUES (13,'Fern Bay Black, FOX R1',24,'18',0,0,8,5,0,1);
INSERT INTO `race_template` VALUES (14,'Fern Bay Black Rev, FOX R1',25,'18',0,0,8,0,0,1);
INSERT INTO `race_template` VALUES (15,'Aston Club, Formule Pratice',44,'16 17 18 19',0,0,0,0,0,5);
INSERT INTO `race_template` VALUES (16,'Aston Club Rev, Formule Pratice',45,'16 17 18 19',0,0,0,0,0,5);
INSERT INTO `race_template` VALUES (17,'Blackwood Rallycross, LX6',3,'5',2,0,18,5,0,1);
INSERT INTO `race_template` VALUES (18,'Blackwood Rallycross Rev, LX6',4,'5',2,0,18,5,0,1);
INSERT INTO `race_template` VALUES (19,'Fern Bay Club, MRT',18,'2 3',0,0,11,5,0,1);
INSERT INTO `race_template` VALUES (20,'Fern Bay Club Rev, F GTR',19,'11 12',0,0,14,5,0,1);
/*!40000 ALTER TABLE `race_template` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `stats_rank_driver`
--

DROP TABLE IF EXISTS `stats_rank_driver`;
CREATE TABLE `stats_rank_driver` (
  `licence_name` varchar(32) character set latin1 collate latin1_general_ci NOT NULL,
  `track_prefix` varchar(4) NOT NULL default '',
  `car_prefix` varchar(3) NOT NULL default '',
  `best_lap_rank` smallint(5) NOT NULL default '0',
  `average_lap_rank` smallint(5) NOT NULL default '0',
  `stability_rank` smallint(5) NOT NULL default '0',
  `race_win_rank` smallint(5) NOT NULL,
  `total_rank` smallint(5) NOT NULL default '0',
  PRIMARY KEY  (`licence_name`,`track_prefix`,`car_prefix`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

--
-- Dumping data for table `stats_rank_driver`
--

LOCK TABLES `stats_rank_driver` WRITE;
/*!40000 ALTER TABLE `stats_rank_driver` DISABLE KEYS */;
/*!40000 ALTER TABLE `stats_rank_driver` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `track_template`
--

DROP TABLE IF EXISTS `track_template`;
CREATE TABLE `track_template` (
  `entry` tinyint(2) unsigned NOT NULL,
  `name_prefix` varchar(4) NOT NULL default '',
  `name` varchar(16) NOT NULL default '',
  `configuration` varchar(16) NOT NULL default '',
  `reverse` tinyint(1) unsigned NOT NULL default '0',
  `split_node_index_1` tinyint(3) unsigned NOT NULL default '0',
  `split_node_index_2` tinyint(3) unsigned NOT NULL default '0',
  `split_node_index_3` tinyint(3) unsigned NOT NULL default '0',
  `total_length` mediumint(8) unsigned NOT NULL default '0',
  PRIMARY KEY  (`entry`),
  UNIQUE KEY `abreviation_name` (`name_prefix`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

--
-- Dumping data for table `track_template`
--

LOCK TABLES `track_template` WRITE;
/*!40000 ALTER TABLE `track_template` DISABLE KEYS */;
INSERT INTO `track_template` VALUES (1,'BL1','Blackwood','GP Track',0,0,0,0,0);
INSERT INTO `track_template` VALUES (2,'BL1R','Blackwood','GP Track',1,0,0,0,0);
INSERT INTO `track_template` VALUES (3,'BL2','Blackwood','Rallycross',0,0,0,0,0);
INSERT INTO `track_template` VALUES (4,'BL2R','Blackwood','Rallycross',1,0,0,0,0);
INSERT INTO `track_template` VALUES (5,'BL3','Blackwood','Car Park',0,0,0,0,0);
INSERT INTO `track_template` VALUES (6,'SO1','South City','Classic',0,0,0,0,0);
INSERT INTO `track_template` VALUES (7,'SO1R','South City','Classic',1,0,0,0,0);
INSERT INTO `track_template` VALUES (8,'SO2','South City','Sprint 1',0,0,0,0,0);
INSERT INTO `track_template` VALUES (9,'SO2R','South City','Sprint 1',1,0,0,0,0);
INSERT INTO `track_template` VALUES (10,'SO3','South City','Sprint 2',0,0,0,0,0);
INSERT INTO `track_template` VALUES (11,'SO3R','South City','Sprint 2',1,0,0,0,0);
INSERT INTO `track_template` VALUES (12,'SO4','South City','City Long',0,0,0,0,0);
INSERT INTO `track_template` VALUES (13,'SO4R','South City','City Long',1,0,0,0,0);
INSERT INTO `track_template` VALUES (14,'SO5','South City','Town Course',0,0,0,0,0);
INSERT INTO `track_template` VALUES (15,'SO5R','South City','Town Course',1,0,0,0,0);
INSERT INTO `track_template` VALUES (16,'SO6','South City','Chicane Route',0,0,0,0,0);
INSERT INTO `track_template` VALUES (17,'SO6R','South City','Chicane Route',1,0,0,0,0);
INSERT INTO `track_template` VALUES (18,'FE1','Fern Bay','Club',0,0,0,0,0);
INSERT INTO `track_template` VALUES (19,'FE1R','Fern Bay','Club',1,0,0,0,0);
INSERT INTO `track_template` VALUES (20,'FE2','Fern Bay','Green',0,0,0,0,0);
INSERT INTO `track_template` VALUES (21,'FE2R','Fern Bay','Green',1,0,0,0,0);
INSERT INTO `track_template` VALUES (22,'FE3','Fern Bay','Gold',0,0,0,0,0);
INSERT INTO `track_template` VALUES (23,'FE3R','Fern Bay','Gold',1,0,0,0,0);
INSERT INTO `track_template` VALUES (24,'FE4','Fern Bay','Black',0,0,0,0,0);
INSERT INTO `track_template` VALUES (25,'FE4R','Fern Bay','Black',1,0,0,0,0);
INSERT INTO `track_template` VALUES (26,'FE5','Fern Bay','Rallycross',0,0,0,0,0);
INSERT INTO `track_template` VALUES (27,'FE5R','Fern Bay','Rallycross',1,0,0,0,0);
INSERT INTO `track_template` VALUES (28,'FE6','Fern Bay','RallyX Green',0,0,0,0,0);
INSERT INTO `track_template` VALUES (29,'FE6R','Fern Bay','RallyX Green',1,0,0,0,0);
INSERT INTO `track_template` VALUES (30,'AU1','Autocross','Autocross',0,0,0,0,0);
INSERT INTO `track_template` VALUES (31,'AU2','Autocross','Skid Pad',0,0,0,0,0);
INSERT INTO `track_template` VALUES (32,'AU3','Autocross','Drag Strip',0,0,0,0,0);
INSERT INTO `track_template` VALUES (33,'AU4','Autocross','8 Lane Drag',0,0,0,0,0);
INSERT INTO `track_template` VALUES (34,'KY1','Kyoto Ring','Oval',0,0,0,0,0);
INSERT INTO `track_template` VALUES (35,'KY1R','Kyoto Ring','Oval',1,0,0,0,0);
INSERT INTO `track_template` VALUES (36,'KY2','Kyoto Ring','National',0,0,0,0,0);
INSERT INTO `track_template` VALUES (37,'KY2R','Kyoto Ring','National',1,0,0,0,0);
INSERT INTO `track_template` VALUES (38,'KY3','Kyoto Ring','GP Long',0,0,0,0,0);
INSERT INTO `track_template` VALUES (39,'KY3R','Kyoto Ring','GP Long',1,0,0,0,0);
INSERT INTO `track_template` VALUES (40,'WE1','Westhill','International',0,0,0,0,0);
INSERT INTO `track_template` VALUES (41,'WE1R','Westhill','International',1,0,0,0,0);
INSERT INTO `track_template` VALUES (42,'AS1','Aston','Cadet',0,0,0,0,0);
INSERT INTO `track_template` VALUES (43,'AS1R','Aston','Cadet',1,0,0,0,0);
INSERT INTO `track_template` VALUES (44,'AS2','Aston','Club',0,0,0,0,0);
INSERT INTO `track_template` VALUES (45,'AS2R','Aston','Club',1,0,0,0,0);
INSERT INTO `track_template` VALUES (46,'AS3','Aston','National',0,0,0,0,0);
INSERT INTO `track_template` VALUES (47,'AS3R','Aston','National',1,0,0,0,0);
INSERT INTO `track_template` VALUES (48,'AS4','Aston','Historic',0,0,0,0,0);
INSERT INTO `track_template` VALUES (49,'AS4R','Aston','Historic',1,0,0,0,0);
INSERT INTO `track_template` VALUES (50,'AS5','Aston','Grand Prix',0,0,0,0,0);
INSERT INTO `track_template` VALUES (51,'AS5R','Aston','Grand Prix',1,0,0,0,0);
INSERT INTO `track_template` VALUES (52,'AS6','Aston','Grand Touring',0,0,0,0,0);
INSERT INTO `track_template` VALUES (53,'AS6R','Aston','Grand Touring',1,0,0,0,0);
INSERT INTO `track_template` VALUES (54,'AS7','Aston','North',0,0,0,0,0);
INSERT INTO `track_template` VALUES (55,'AS7R','Aston','North',1,0,0,0,0);
/*!40000 ALTER TABLE `track_template` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2008-10-04  1:53:16
