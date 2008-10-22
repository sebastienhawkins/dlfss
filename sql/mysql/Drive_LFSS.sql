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
-- Table structure for table `bad_word`
--

DROP TABLE IF EXISTS `bad_word`;
CREATE TABLE `bad_word` (
  `word` varchar(16) character set latin1 collate latin1_general_ci NOT NULL default '',
  `mask` tinyint(1) unsigned NOT NULL default '0',
  PRIMARY KEY  (`word`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;

--
-- Dumping data for table `bad_word`
--

LOCK TABLES `bad_word` WRITE;
/*!40000 ALTER TABLE `bad_word` DISABLE KEYS */;
INSERT INTO `bad_word` VALUES ('asshole',3);
INSERT INTO `bad_word` VALUES ('fuck',2);
INSERT INTO `bad_word` VALUES ('you',1);
INSERT INTO `bad_word` VALUES ('idiot',3);
INSERT INTO `bad_word` VALUES ('shit',2);
INSERT INTO `bad_word` VALUES ('piece',1);
INSERT INTO `bad_word` VALUES ('mother',1);
INSERT INTO `bad_word` VALUES ('jerk',3);
INSERT INTO `bad_word` VALUES ('bastard',3);
INSERT INTO `bad_word` VALUES ('nigger',3);
INSERT INTO `bad_word` VALUES ('fag',3);
INSERT INTO `bad_word` VALUES ('chienne',2);
INSERT INTO `bad_word` VALUES ('enfant',1);
INSERT INTO `bad_word` VALUES ('chier',2);
INSERT INTO `bad_word` VALUES ('va ',1);
INSERT INTO `bad_word` VALUES ('vachier',3);
INSERT INTO `bad_word` VALUES ('marde',2);
INSERT INTO `bad_word` VALUES ('mange',1);
INSERT INTO `bad_word` VALUES ('putain',3);
INSERT INTO `bad_word` VALUES ('enculer',3);
INSERT INTO `bad_word` VALUES ('cul ',2);
INSERT INTO `bad_word` VALUES ('trou',1);
INSERT INTO `bad_word` VALUES ('bitch ',3);
INSERT INTO `bad_word` VALUES ('morron',3);
INSERT INTO `bad_word` VALUES ('pute ',3);
INSERT INTO `bad_word` VALUES ('stupid',3);
INSERT INTO `bad_word` VALUES ('fucker',3);
INSERT INTO `bad_word` VALUES ('cock',3);
INSERT INTO `bad_word` VALUES ('slut',3);
INSERT INTO `bad_word` VALUES ('pussy',2);
INSERT INTO `bad_word` VALUES ('lick',1);
INSERT INTO `bad_word` VALUES ('retard',2);
INSERT INTO `bad_word` VALUES ('fuckking',3);
INSERT INTO `bad_word` VALUES ('fukka',3);
INSERT INTO `bad_word` VALUES ('him',1);
INSERT INTO `bad_word` VALUES ('suck',2);
INSERT INTO `bad_word` VALUES ('ass.',1);
INSERT INTO `bad_word` VALUES ('nazi',3);
INSERT INTO `bad_word` VALUES ('gay',2);
INSERT INTO `bad_word` VALUES ('class',0);
INSERT INTO `bad_word` VALUES ('witch',0);
INSERT INTO `bad_word` VALUES ('cunt',3);
INSERT INTO `bad_word` VALUES ('fuk',2);
INSERT INTO `bad_word` VALUES ('dick',2);
INSERT INTO `bad_word` VALUES ('ass ',2);
INSERT INTO `bad_word` VALUES ('luck',0);
INSERT INTO `bad_word` VALUES ('rock',0);
INSERT INTO `bad_word` VALUES ('restart',0);
/*!40000 ALTER TABLE `bad_word` ENABLE KEYS */;
UNLOCK TABLES;

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
INSERT INTO `button_template` VALUES (4,'motd text line',98,1,0,50,67,100,7,'','');
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
INSERT INTO `button_template` VALUES (23,'config title and close',40,0,0,75,0,50,13,'^2CLOSE','');
INSERT INTO `button_template` VALUES (24,'config text close',0,0,0,113,8,10,5,'^3UserConfig','');
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
INSERT INTO `button_template` VALUES (46,'text close button',0,0,0,71,39,8,4,'^2close','');
INSERT INTO `button_template` VALUES (45,'text title and close',40,0,0,30,32,50,12,'^3Text Display','');
INSERT INTO `button_template` VALUES (44,'text line',32,0,0,1,45,110,6,'','');
INSERT INTO `button_template` VALUES (47,'rank bg',32,0,0,0,30,81,141,'','');
INSERT INTO `button_template` VALUES (48,'rank title',144,0,0,1,31,79,5,'^7A^3leajecta ^7Ranking','');
INSERT INTO `button_template` VALUES (49,'rank text help',32,0,0,82,108,75,7,'','');
INSERT INTO `button_template` VALUES (50,'rank name',16,0,0,1,42,27,5,'^8name','');
INSERT INTO `button_template` VALUES (51,'rank pb',16,0,0,28,42,8,5,'^8best','');
INSERT INTO `button_template` VALUES (52,'rank average',16,0,0,36,42,8,5,'^8avera','');
INSERT INTO `button_template` VALUES (53,'rank sta',16,0,0,44,42,8,5,'^8stabi','');
INSERT INTO `button_template` VALUES (54,'rank win',16,0,0,52,42,8,5,'^8win','');
INSERT INTO `button_template` VALUES (55,'rank total',16,0,0,60,42,9,5,'^8total','');
INSERT INTO `button_template` VALUES (56,'rank position',144,0,0,69,42,11,5,'^8position','');
INSERT INTO `button_template` VALUES (57,'rank last update',64,0,0,1,31,25,5,'^0Last ^72008^2/^7Oct^2/^721','');
INSERT INTO `button_template` VALUES (58,'rank next update',64,0,0,24,31,20,5,'','');
INSERT INTO `button_template` VALUES (67,'rank search button add',40,0,32,53,151,26,7,'^2Add','Licence Name');
INSERT INTO `button_template` VALUES (60,'rank search button',40,0,0,27,158,26,10,'^2Search','');
INSERT INTO `button_template` VALUES (61,'rank current button',40,0,0,53,158,26,10,'^2Current','');
INSERT INTO `button_template` VALUES (62,'rank tittle and close',40,0,0,75,0,50,13,'^2CLOSE','');
INSERT INTO `button_template` VALUES (63,'rank text close',0,0,0,115,8,8,5,'^3Ranking','');
INSERT INTO `button_template` VALUES (64,'rank info',16,0,0,1,37,79,5,'^2Car: ^7FBM, ^2Track:^7 BL1R, ^2Count: ^7154356','');
INSERT INTO `button_template` VALUES (66,'rank search button car',40,0,3,27,151,26,7,'^2Car','Car Prefix');
INSERT INTO `button_template` VALUES (65,'rank search button track',40,0,4,1,151,26,7,'^2Track','Track Prefix');
INSERT INTO `button_template` VALUES (59,'rank top10 button',40,0,0,1,158,26,10,'^2Top20','');
INSERT INTO `button_template` VALUES (68,'precollision cancel button1/3',40,0,0,1,140,56,10,'^7Click HERE To ^1CANCEL','');
INSERT INTO `button_template` VALUES (69,'precollision cancel button2/3',40,0,0,1,150,56,10,'^7Bad driving for driver','');
INSERT INTO `button_template` VALUES (70,'precollision cancel button3/3',40,0,0,1,160,56,10,'','');
INSERT INTO `button_template` VALUES (71,'result title and close',40,0,0,75,0,50,13,'^2CLOSE','');
INSERT INTO `button_template` VALUES (72,'precollision text close',0,0,0,115,8,8,5,'^3Result','');
INSERT INTO `button_template` VALUES (73,'result title',160,0,0,1,56,27,4,'^7A^3leajecta ^7Race Result','');
INSERT INTO `button_template` VALUES (74,'result name display',96,0,0,1,59,20,3,'greenseed','');
INSERT INTO `button_template` VALUES (75,'result score display',160,0,0,21,59,7,3,'^224^7pt','');
INSERT INTO `button_template` VALUES (76,'result help text',32,0,0,80,76,25,5,'','');
INSERT INTO `button_template` VALUES (77,'config maxspeed title',8,0,0,26,39,20,7,'^7Max Speed','');
INSERT INTO `button_template` VALUES (78,'result bg',16,0,0,0,30,77,121,'','');
INSERT INTO `button_template` VALUES (79,'Race flag text',0,0,0,138,8,10,4,'^0Race Flag','');
INSERT INTO `button_template` VALUES (80,'flag green square',0,0,0,133,7,15,25,'^2','');
INSERT INTO `button_template` VALUES (81,'flag green square',0,0,0,134,7,15,25,'^2','');
INSERT INTO `button_template` VALUES (82,'flag green square',0,0,0,135,7,15,25,'^2','');
INSERT INTO `button_template` VALUES (83,'flag green square',0,0,0,136,7,15,25,'^2','');
INSERT INTO `button_template` VALUES (84,'flag green square',0,0,0,137,7,15,25,'^2','');
INSERT INTO `button_template` VALUES (85,'flag green square',0,0,0,138,7,15,25,'^2','');
INSERT INTO `button_template` VALUES (86,'flag red square',0,0,0,133,7,15,25,'^1','');
INSERT INTO `button_template` VALUES (87,'flag red square',0,0,0,134,7,15,25,'^1','');
INSERT INTO `button_template` VALUES (88,'flag red square',0,0,0,135,7,15,25,'^1','');
INSERT INTO `button_template` VALUES (89,'flag red square',0,0,0,136,7,15,25,'^1','');
INSERT INTO `button_template` VALUES (90,'flag red square',0,0,0,137,7,15,25,'^1','');
INSERT INTO `button_template` VALUES (91,'flag red square',0,0,0,138,7,15,25,'^1','');
INSERT INTO `button_template` VALUES (92,'pit close flag >',64,0,0,134,3,14,33,'^3>','');
INSERT INTO `button_template` VALUES (93,'pit close flag <',128,0,0,138,3,14,33,'^3<','');
INSERT INTO `button_template` VALUES (94,'flag yellow square',0,0,0,133,7,15,25,'^3','');
INSERT INTO `button_template` VALUES (95,'flag yellow square',0,0,0,134,7,15,25,'^3','');
INSERT INTO `button_template` VALUES (96,'flag yellow square',0,0,0,135,7,15,25,'^3','');
INSERT INTO `button_template` VALUES (97,'flag yellow square',0,0,0,136,7,15,25,'^3','');
INSERT INTO `button_template` VALUES (98,'flag yellow square',0,0,0,137,7,15,25,'^3','');
INSERT INTO `button_template` VALUES (99,'flag yellow square',0,0,0,138,7,15,25,'^3','');
INSERT INTO `button_template` VALUES (100,'yellow red square',0,0,0,137,12,12,15,'^0SC','');
INSERT INTO `button_template` VALUES (101,'flag blue square',0,0,0,133,7,15,25,'^4','');
INSERT INTO `button_template` VALUES (102,'flag blue square',0,0,0,134,7,15,25,'^4','');
INSERT INTO `button_template` VALUES (103,'flag blue square',0,0,0,135,7,15,25,'^4','');
INSERT INTO `button_template` VALUES (104,'flag blue square',0,0,0,136,7,15,25,'^4','');
INSERT INTO `button_template` VALUES (105,'flag blue square',0,0,0,137,7,15,25,'^4','');
INSERT INTO `button_template` VALUES (106,'flag blue square',0,0,0,138,7,15,25,'^4','');
INSERT INTO `button_template` VALUES (107,'flag black square',0,0,0,133,7,15,25,'^0','');
INSERT INTO `button_template` VALUES (108,'flag black square',0,0,0,134,7,15,25,'^0','');
INSERT INTO `button_template` VALUES (109,'flag black square',0,0,0,135,7,15,25,'^0','');
INSERT INTO `button_template` VALUES (110,'flag black square',0,0,0,136,7,15,25,'^0','');
INSERT INTO `button_template` VALUES (111,'flag black square',0,0,0,137,7,15,25,'^0','');
INSERT INTO `button_template` VALUES (112,'flag black square',0,0,0,138,7,15,25,'^0','');
INSERT INTO `button_template` VALUES (119,'flag car probleme dot',0,0,0,137,8,12,22,'^3•','');
INSERT INTO `button_template` VALUES (120,'pit close flag >',64,0,0,134,3,14,33,'^7>','');
INSERT INTO `button_template` VALUES (121,'pit close flag <',128,0,0,138,3,14,33,'^7<','');
INSERT INTO `button_template` VALUES (122,'end race flag',0,0,0,136,1,7,26,'^7-','');
INSERT INTO `button_template` VALUES (123,'end race flag',0,0,0,136,7,7,26,'^7-','');
INSERT INTO `button_template` VALUES (124,'end race flag',0,0,0,138,4,7,26,'^7-','');
INSERT INTO `button_template` VALUES (125,'end race flag',0,0,0,138,10,7,26,'^7-','');
INSERT INTO `button_template` VALUES (126,'end race flag',0,0,0,140,1,7,26,'^7-','');
INSERT INTO `button_template` VALUES (127,'end race flag',0,0,0,140,7,7,26,'^7-','');
INSERT INTO `button_template` VALUES (128,'end race flag',0,0,0,142,4,7,26,'^7-','');
INSERT INTO `button_template` VALUES (129,'end race flag',0,0,0,142,10,7,26,'^7-','');
INSERT INTO `button_template` VALUES (130,'end race flag',0,0,0,144,1,7,26,'^7-','');
INSERT INTO `button_template` VALUES (131,'end race flag',0,0,0,144,7,7,26,'^7-','');
INSERT INTO `button_template` VALUES (132,'end race flag',0,0,0,139,7,15,25,'^0','');
INSERT INTO `button_template` VALUES (113,'flag black square',0,0,0,133,7,15,25,'^7','');
INSERT INTO `button_template` VALUES (114,'flag black square',0,0,0,134,7,15,25,'^7','');
INSERT INTO `button_template` VALUES (115,'flag black square',0,0,0,135,7,15,25,'^7','');
INSERT INTO `button_template` VALUES (116,'flag black square',0,0,0,136,7,15,25,'^7','');
INSERT INTO `button_template` VALUES (117,'flag black square',0,0,0,137,7,15,25,'^7','');
INSERT INTO `button_template` VALUES (118,'flag black square',0,0,0,138,7,15,25,'^7','');
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
  `licence_name` varchar(24) character set latin1 collate latin1_general_ci NOT NULL,
  `driver_name` varchar(24) NOT NULL,
  `config_data` blob,
  `warning_driving_count` int(8) unsigned NOT NULL default '0',
  `warning_chat_count` int(8) unsigned NOT NULL default '0',
  `last_connection_time` bigint(12) unsigned NOT NULL default '0',
  PRIMARY KEY  (`guid`),
  UNIQUE KEY `licence_name` (`licence_name`),
  UNIQUE KEY `licence_name_2` (`licence_name`),
  UNIQUE KEY `licence_name_3` (`licence_name`)
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
  KEY `car_prefix` (`car_prefix`),
  KEY `track_prefix` (`track_prefix`)
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
  `button_entry_ext` varchar(255) NOT NULL default '',
  `text_button_entry` mediumint(5) unsigned NOT NULL default '0',
  `text` blob NOT NULL,
  PRIMARY KEY  (`entry`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

--
-- Dumping data for table `gui_template`
--

LOCK TABLES `gui_template` WRITE;
/*!40000 ALTER TABLE `gui_template` DISABLE KEYS */;
INSERT INTO `gui_template` VALUES (1,'motd','2 3 5 30 36','',4,'^7Welcome To A^3leajecta ^7Live For Speed Server\r\n^7\r\n^7!Rank System ^2Online, ^3Global\r\n^7!Result System ^2Online \r\n^7DriftScoring System ^2Online, ^3NoRank\r\n^7Bad Driving Check ^2Online\r\n^7\r\n^7To know more about this Visit www.aleajecta.com\r\n^7\r\n^7 Your Host A^3leajecta^7.');
INSERT INTO `gui_template` VALUES (2,'user config','22 23 24 25 26 27 28 29 37 38 40 41 42 43 77','',39,'^8Config Help\r\n^7Title ^3 will enable or disable the feature\r\n^3Green ^2button ^3will trigger a action for this feature.');
INSERT INTO `gui_template` VALUES (3,'help','31 32 33 34 35','',33,'^7A^3leajecta ^7is powered by Drive_LFSS 0.4 Alpha\r\n^7\r\n^7This is a alpha stage of the developement\r\n^7there is a lot more to come, please be patien.\r\n^7\r\n^7List Of Commands:\r\n^2!help^7, ^2!config^7, ^2!rank^7, ^2!result^7, ^2!status^7\r\n^5!say, !kick^7, ^5!exit^7, ^5!reload\r\n^7\r\n^7Did you know \"SHIFT-i\" will reset button and make Config screen appear.\r\n^7\r\n^5Greenseed^7.');
INSERT INTO `gui_template` VALUES (4,'text','45 46','',44,'');
INSERT INTO `gui_template` VALUES (5,'rank','47 48 57 58 59 60 61 62 63 64','50 51 52 53 54 55 56 65 66 67',49,'^7Top20 ^2is the 20 Best Driver for this Track/Car.\r\n^7Search ^2search for a Track/Car/Driver rank.\r\n^7Current ^2show rank for all online driver by Track/Car.\r\n^6Blue mean worst^2, ^3Yellow mean Better^2, ^7White mean event\r\n^2\r\n^2When button become ^1red ^2mean clearing for flood protection ');
INSERT INTO `gui_template` VALUES (6,'result guid','71 72 73','74 75',76,'');
INSERT INTO `gui_template` VALUES (7,'green flag','79 80 81 82 83 84 85','',0,'');
INSERT INTO `gui_template` VALUES (8,'pit close flag','79 86 87 88 89 90 91 92 93','',0,'');
INSERT INTO `gui_template` VALUES (9,'yellow flag local','79 94 95 96 97 98 99','',0,'');
INSERT INTO `gui_template` VALUES (10,'yellow flag global','79 94 95 96 97 98 99 100','',0,'');
INSERT INTO `gui_template` VALUES (11,'red flag stop race','79 86 87 88 89 90 91','',0,'');
INSERT INTO `gui_template` VALUES (12,'black flag penality','79 107 108 109 110 111 112','',0,'');
INSERT INTO `gui_template` VALUES (13,'blue flag slow car','79 101 102 103 104 105 106','',0,'');
INSERT INTO `gui_template` VALUES (14,'white flag final lap','79 113 114 115 116 117 118','',0,'');
INSERT INTO `gui_template` VALUES (15,'black flag car probleme','79 107 108 109 110 111 112 119','',0,'');
INSERT INTO `gui_template` VALUES (16,'black no longer scored','79 107 108 109 110 111 112 120 121','',0,'');
INSERT INTO `gui_template` VALUES (17,'end race flag','79 107 108 109 110 111 112 132 122 123 124 125 126 127 128 129 130 131','',0,'');
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
INSERT INTO `race_map` VALUES (1,21);
INSERT INTO `race_map` VALUES (1,22);
INSERT INTO `race_map` VALUES (1,23);
INSERT INTO `race_map` VALUES (1,24);
INSERT INTO `race_map` VALUES (1,25);
INSERT INTO `race_map` VALUES (1,26);
INSERT INTO `race_map` VALUES (1,27);
INSERT INTO `race_map` VALUES (1,28);
INSERT INTO `race_map` VALUES (1,29);
INSERT INTO `race_map` VALUES (1,30);
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
INSERT INTO `race_map` VALUES (3,31);
INSERT INTO `race_map` VALUES (3,32);
INSERT INTO `race_map` VALUES (3,33);
INSERT INTO `race_map` VALUES (3,34);
INSERT INTO `race_map` VALUES (3,35);
INSERT INTO `race_map` VALUES (3,36);
INSERT INTO `race_map` VALUES (3,37);
INSERT INTO `race_map` VALUES (3,38);
INSERT INTO `race_map` VALUES (3,39);
INSERT INTO `race_map` VALUES (5,40);
INSERT INTO `race_map` VALUES (5,41);
INSERT INTO `race_map` VALUES (5,42);
INSERT INTO `race_map` VALUES (5,43);
INSERT INTO `race_map` VALUES (5,44);
INSERT INTO `race_map` VALUES (5,45);
INSERT INTO `race_map` VALUES (5,46);
INSERT INTO `race_map` VALUES (5,47);
INSERT INTO `race_map` VALUES (5,48);
INSERT INTO `race_map` VALUES (6,49);
INSERT INTO `race_map` VALUES (6,50);
/*!40000 ALTER TABLE `race_map` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `race_template`
--

DROP TABLE IF EXISTS `race_template`;
CREATE TABLE `race_template` (
  `entry` mediumint(5) unsigned NOT NULL,
  `description` varchar(34) NOT NULL default '',
  `entry_restriction` mediumint(5) unsigned NOT NULL default '0',
  `track_entry` tinyint(2) unsigned NOT NULL default '0',
  `car_entry_allowed` varchar(64) NOT NULL default '0',
  `weather` tinyint(2) unsigned NOT NULL default '0',
  `wind` tinyint(2) unsigned NOT NULL default '0',
  `lap_count` tinyint(3) unsigned NOT NULL default '0',
  `qualify_minute` tinyint(3) unsigned NOT NULL default '0',
  `maximun_race_finish` tinyint(3) unsigned NOT NULL default '0',
  `race_template_mask` smallint(5) unsigned NOT NULL default '0',
  PRIMARY KEY  (`entry`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

--
-- Dumping data for table `race_template`
--

LOCK TABLES `race_template` WRITE;
/*!40000 ALTER TABLE `race_template` DISABLE KEYS */;
INSERT INTO `race_template` VALUES (1,'South City Classic, 115Hp',0,6,'2 3',0,0,11,5,0,1);
INSERT INTO `race_template` VALUES (2,'South City Classic Rev,  115Hp',0,7,'2 3',1,1,12,5,0,1);
INSERT INTO `race_template` VALUES (3,'South City Sprint 1, 115Hp',0,8,'2 3',2,2,13,5,0,1);
INSERT INTO `race_template` VALUES (4,'South City Sprint 1 Rev, 115Hp',0,9,'2 3',3,3,14,5,0,1);
INSERT INTO `race_template` VALUES (5,'South City Sprint 2, 115Hp',0,10,'2 3',1,1,16,5,0,1);
INSERT INTO `race_template` VALUES (6,'South City Sprint 2 Rev, 115Hp',0,11,'2 3',2,1,15,5,0,1);
INSERT INTO `race_template` VALUES (7,'Blackwood GP Track, F08',0,1,'19',0,0,25,8,0,1);
INSERT INTO `race_template` VALUES (8,'Westhill, Turbo GTR',0,40,'13 14 15',0,0,10,8,0,1);
INSERT INTO `race_template` VALUES (9,'Westhill Rev, GTR Turbo',0,41,'13 14 15',0,0,10,8,0,1);
INSERT INTO `race_template` VALUES (10,'Aston Cadet Pratice, Formule',0,42,'16 17 18 19 20',0,0,0,0,0,5);
INSERT INTO `race_template` VALUES (11,'Aston Cadet, FBM',0,42,'17',0,0,10,12,0,1);
INSERT INTO `race_template` VALUES (12,'South City Sprint 1 Rev, FBM',0,9,'17',0,0,11,5,0,1);
INSERT INTO `race_template` VALUES (13,'Fern Bay Black, FOX R1',0,24,'18',0,0,8,5,0,1);
INSERT INTO `race_template` VALUES (14,'Fern Bay Black Rev, FOX R1',0,25,'18',0,0,8,0,0,1);
INSERT INTO `race_template` VALUES (15,'Aston Club, Formule Pratice',0,44,'16 17 18 19',0,0,0,0,0,5);
INSERT INTO `race_template` VALUES (16,'Aston Club Rev, Formule Pratice',0,45,'16 17 18 19',0,0,0,0,0,5);
INSERT INTO `race_template` VALUES (17,'Blackwood Rallycross, LX6',0,3,'5',2,0,18,5,0,1);
INSERT INTO `race_template` VALUES (18,'Blackwood Rallycross Rev, LX6',0,4,'5',2,0,18,5,0,1);
INSERT INTO `race_template` VALUES (19,'Fern Bay Club, MRT',0,18,'2 3',0,0,11,5,0,1);
INSERT INTO `race_template` VALUES (20,'Fern Bay Club Rev, F GTR',0,19,'11 12',0,0,14,5,0,1);
INSERT INTO `race_template` VALUES (30,'Blackwood Rally, RB4',0,3,'6',0,0,9,0,0,1);
INSERT INTO `race_template` VALUES (29,'Fernbay Green Rev, FOX',0,21,'18',0,0,5,0,0,1);
INSERT INTO `race_template` VALUES (28,'Southcity Town Rev, UFR/XFR',0,15,'11 12',0,0,7,0,0,1);
INSERT INTO `race_template` VALUES (27,'Southcity Chicane Rev, UFR/XFR',0,17,'11 12',0,0,7,0,0,1);
INSERT INTO `race_template` VALUES (26,'Southcity Sprint2 Rev, FXO/XRT',0,11,'7 8',0,0,14,0,0,1);
INSERT INTO `race_template` VALUES (25,'Southcity Classic, FXO/XRT',0,6,'7 8',0,0,7,0,0,1);
INSERT INTO `race_template` VALUES (24,'Southcity Town, FXO/XRT',0,14,'7 8',0,0,7,0,0,1);
INSERT INTO `race_template` VALUES (23,'Aston Club Rev, FOX',0,45,'18',0,0,7,0,0,1);
INSERT INTO `race_template` VALUES (22,'Fernbay Gold Rev, FOX',0,23,'18',0,0,5,0,0,1);
INSERT INTO `race_template` VALUES (37,'Aston Cadet Rev, FBM',0,43,'17',0,0,8,0,0,1);
INSERT INTO `race_template` VALUES (36,'Southcity Classic, FBM',0,6,'17',0,0,8,0,0,1);
INSERT INTO `race_template` VALUES (33,'Fernbay Club Rev, XFG/XRG',0,19,'2 3',0,0,12,0,0,1);
INSERT INTO `race_template` VALUES (35,'Southcity Classic Rev, XFG/XRG',0,7,'2 3',0,0,8,0,0,1);
INSERT INTO `race_template` VALUES (34,'Fernbay Rally, XFG/XRG',0,28,'2 3',0,0,9,0,0,1);
INSERT INTO `race_template` VALUES (21,'Fernbay Green Rev, FOX',0,21,'18',0,0,7,0,0,1);
INSERT INTO `race_template` VALUES (32,'Fernbay Club, XFG/XRG',0,18,'2 3',0,0,12,0,0,1);
INSERT INTO `race_template` VALUES (31,'Southcity Sprint2 Rev, XFG/XRG',0,11,'2 3',0,0,10,0,0,1);
INSERT INTO `race_template` VALUES (38,'Southcity Sprint1, FBM',0,9,'17',0,0,8,0,0,1);
INSERT INTO `race_template` VALUES (39,'Blackwood GP, FBM',0,1,'17',0,0,5,0,0,1);
/*!40000 ALTER TABLE `race_template` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `restriction_join`
--

DROP TABLE IF EXISTS `restriction_join`;
CREATE TABLE `restriction_join` (
  `entry` int(8) unsigned NOT NULL,
  `description` varchar(100) NOT NULL default '',
  `safe_driving_pct` tinyint(3) unsigned NOT NULL default '0',
  `safe_driving_pct_kick` tinyint(1) unsigned NOT NULL default '0',
  `bad_language_pct` tinyint(3) unsigned NOT NULL default '0',
  `bad_language_pct_kick` tinyint(1) unsigned NOT NULL default '0',
  `pb_min` int(12) unsigned NOT NULL default '0',
  `pb_max` int(12) unsigned NOT NULL default '0',
  `pb_kick` tinyint(1) unsigned NOT NULL default '0',
  `skin_name` varchar(16) character set latin1 collate latin1_general_ci NOT NULL default '',
  `skin_name_kick` tinyint(1) unsigned NOT NULL default '0',
  `driver_name` varchar(16) NOT NULL default '',
  `driver_name_kick` tinyint(1) unsigned NOT NULL default '0',
  `rank_best_min` smallint(6) unsigned NOT NULL default '0',
  `rank_best_max` smallint(6) unsigned NOT NULL,
  `rank_avg_min` smallint(6) unsigned NOT NULL default '0',
  `rank_avg_max` smallint(6) unsigned NOT NULL default '0',
  `rank_sta_min` smallint(6) unsigned NOT NULL default '0',
  `rank_sta_max` smallint(6) unsigned NOT NULL default '0',
  `rank_win_min` smallint(6) unsigned NOT NULL default '0',
  `rank_win_max` smallint(6) unsigned NOT NULL default '0',
  `rank_total_min` smallint(6) unsigned NOT NULL default '0',
  `rank_total_max` smallint(6) unsigned NOT NULL,
  `rank_kick` tinyint(1) unsigned NOT NULL default '0',
  PRIMARY KEY  (`entry`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

--
-- Dumping data for table `restriction_join`
--

LOCK TABLES `restriction_join` WRITE;
/*!40000 ALTER TABLE `restriction_join` DISABLE KEYS */;
/*!40000 ALTER TABLE `restriction_join` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `restriction_race`
--

DROP TABLE IF EXISTS `restriction_race`;
CREATE TABLE `restriction_race` (
  `entry` int(8) unsigned NOT NULL,
  `description` varchar(100) NOT NULL default '',
  `speed_ms_max` float(4,1) unsigned NOT NULL default '0.0',
  `speed_ms_max_lap_number` varchar(20) NOT NULL default '0',
  `speed_ms_max_pen_type` tinyint(3) unsigned NOT NULL default '0',
  `tyre_fl` tinyint(3) unsigned NOT NULL default '0',
  `tyre_fr` tinyint(3) unsigned NOT NULL default '0',
  `tyre_rl` tinyint(3) unsigned NOT NULL default '0',
  `tyre_rr` tinyint(3) unsigned NOT NULL default '0',
  `tyre_pen_type` tinyint(3) unsigned NOT NULL default '0',
  `pit_work_1` smallint(5) unsigned NOT NULL default '0',
  `pit_work_1_pen_type` tinyint(3) unsigned NOT NULL default '0',
  `pit_work_2` smallint(5) unsigned NOT NULL default '0',
  `pit_work_2_pen_type` tinyint(3) unsigned NOT NULL default '0',
  `passenger` tinyint(3) unsigned NOT NULL default '0',
  `passenger_pen_type` tinyint(3) unsigned NOT NULL default '0',
  `added_mass` tinyint(3) unsigned NOT NULL default '0',
  `added_mass_pen_type` tinyint(3) unsigned NOT NULL default '0',
  `intake_restriction` tinyint(3) unsigned NOT NULL default '0',
  `intake_restriction_pen_type` tinyint(3) unsigned NOT NULL default '0',
  `penality_reason` tinyint(3) unsigned NOT NULL default '0',
  `penality_reason_pen_type` tinyint(3) unsigned NOT NULL default '0',
  `driver_mask` smallint(5) unsigned NOT NULL default '0',
  `driver_mask_pen_type` tinyint(3) unsigned NOT NULL default '0',
  PRIMARY KEY  (`entry`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

--
-- Dumping data for table `restriction_race`
--

LOCK TABLES `restriction_race` WRITE;
/*!40000 ALTER TABLE `restriction_race` DISABLE KEYS */;
/*!40000 ALTER TABLE `restriction_race` ENABLE KEYS */;
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
  `race_win_rank` mediumint(8) unsigned NOT NULL default '0',
  `total_rank` mediumint(8) unsigned NOT NULL default '0',
  `position` int(8) unsigned NOT NULL default '0',
  `change_mask` int(11) unsigned NOT NULL default '0',
  PRIMARY KEY  (`licence_name`,`track_prefix`,`car_prefix`),
  KEY `prefix` (`track_prefix`,`car_prefix`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

--
-- Dumping data for table `stats_rank_driver`
--

LOCK TABLES `stats_rank_driver` WRITE;
/*!40000 ALTER TABLE `stats_rank_driver` DISABLE KEYS */;
/*!40000 ALTER TABLE `stats_rank_driver` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `stats_warning_driver`
--

DROP TABLE IF EXISTS `stats_warning_driver`;
CREATE TABLE `stats_warning_driver` (
  `guid_driver` int(11) unsigned NOT NULL default '0',
  `yellow_flag_count` int(8) unsigned NOT NULL default '0',
  `blue_flag_count` int(8) unsigned NOT NULL default '0',
  `warning_count` int(8) unsigned NOT NULL default '0',
  PRIMARY KEY  (`guid_driver`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

--
-- Dumping data for table `stats_warning_driver`
--

LOCK TABLES `stats_warning_driver` WRITE;
/*!40000 ALTER TABLE `stats_warning_driver` DISABLE KEYS */;
/*!40000 ALTER TABLE `stats_warning_driver` ENABLE KEYS */;
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

-- Dump completed on 2008-10-22 17:45:07
