/*
MySQL Backup
Source Host:           localhost
Source Server Version: 5.0.27-community-nt
Source Database:       drive_lfss
Date:                  2008/09/08 23:44:25
*/

SET FOREIGN_KEY_CHECKS=0;
#----------------------------
# Table structure for button_template
#----------------------------
drop table if exists button_template;
CREATE TABLE `button_template` (
  `entry` mediumint(8) unsigned NOT NULL,
  `description` varchar(255) default NULL,
  `style_mask` tinyint(3) unsigned NOT NULL default '0',
  `is_allways_visible` tinyint(1) unsigned NOT NULL default '0',
  `max_input_char` tinyint(3) unsigned NOT NULL,
  `left` tinyint(3) unsigned NOT NULL,
  `top` tinyint(3) unsigned NOT NULL,
  `width` tinyint(3) unsigned NOT NULL,
  `height` tinyint(3) unsigned NOT NULL,
  `text` varchar(240) NOT NULL,
  PRIMARY KEY  (`entry`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;
#----------------------------
# Records for table button_template
#----------------------------


insert  into button_template values (1, 'banner', 66, 1, 0, 35, 194, 20, 7, '^7A^3leajecta') ;
insert  into button_template values (2, 'motd background', 98, 1, 0, 0, 0, 200, 200, '') ;
insert  into button_template values (10, 'vote option 1', 106, 0, 0, 2, 78, 45, 9, 'vote option 1') ;
insert  into button_template values (11, 'vote option 2', 106, 0, 0, 2, 88, 45, 9, 'vote option 2') ;
insert  into button_template values (3, 'motd upper', 146, 1, 0, 45, 60, 110, 7, '^7A^3leajecta') ;
insert  into button_template values (5, 'motd button', 26, 1, 0, 75, 125, 50, 12, '^2Drive') ;
insert  into button_template values (6, 'message bar top', 2, 0, 0, 60, 10, 80, 12, '^1Message bar top') ;
insert  into button_template values (7, 'message bar middle', 2, 0, 0, 25, 85, 150, 14, '^1Message bar Middle') ;
insert  into button_template values (8, 'Collision Warning', 2, 0, 0, 25, 70, 150, 14, '^1CollisionWarning') ;
insert  into button_template values (9, 'vote title', 146, 0, 0, 2, 65, 50, 11, 'vote title') ;
insert  into button_template values (4, 'motd text line', 98, 1, 0, 50, 67, 100, 8, '') ;
insert  into button_template values (12, 'vote option 3', 106, 0, 0, 2, 98, 45, 9, 'vote option 3') ;
insert  into button_template values (13, 'vote option 4', 106, 0, 0, 2, 108, 45, 9, 'vote option 4') ;
insert  into button_template values (14, 'vote option 5', 106, 0, 0, 2, 118, 45, 9, 'vote option 5') ;
insert  into button_template values (15, 'vote option 6', 106, 0, 0, 2, 128, 45, 9, 'vote option 6') ;
insert  into button_template values (16, 'track prefix', 66, 1, 0, 56, 194, 10, 7, '') ;
#----------------------------
# Table structure for car_template
#----------------------------
drop table if exists car_template;
CREATE TABLE `car_template` (
  `entry` smallint(2) unsigned NOT NULL,
  `name_prefix` varchar(3) NOT NULL,
  `name` varchar(16) NOT NULL,
  `mask` int(11) unsigned NOT NULL default '0',
  PRIMARY KEY  (`entry`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;
#----------------------------
# Records for table car_template
#----------------------------


insert  into car_template values (1, 'UF1', 'UF 1000', 0) ;
insert  into car_template values (2, 'XFG', 'XF GTI', 0) ;
insert  into car_template values (3, 'XRG', 'XR GT', 0) ;
insert  into car_template values (4, 'LX4', 'LX4', 0) ;
insert  into car_template values (5, 'LX6', 'LX6', 0) ;
insert  into car_template values (6, 'RB4', 'RB4 GT', 0) ;
insert  into car_template values (7, 'FXO', 'FXO TURBO', 0) ;
insert  into car_template values (8, 'XRT', 'XR GT TURBO', 0) ;
insert  into car_template values (9, 'RAC', 'RACEABOUT', 0) ;
insert  into car_template values (10, 'FZ5', 'FZ5', 0) ;
insert  into car_template values (11, 'UFR', 'UF GTR', 0) ;
insert  into car_template values (12, 'XFR', 'XF GTR', 0) ;
insert  into car_template values (13, 'FXR', 'FXO GTR', 0) ;
insert  into car_template values (14, 'XRR', 'XR GTR', 0) ;
insert  into car_template values (15, 'FZR', 'FZ50 GTR', 0) ;
insert  into car_template values (16, 'MRT', 'MRT5', 0) ;
insert  into car_template values (17, 'FBM', 'FORMULA BMW FB02', 0) ;
insert  into car_template values (18, 'FOX', 'FORMULA XR', 0) ;
insert  into car_template values (19, 'FO8', 'FORMULA V8', 0) ;
insert  into car_template values (20, 'BF1', 'BMW SAUBER F1.06', 0) ;
#----------------------------
# Table structure for driver
#----------------------------
drop table if exists driver;
CREATE TABLE `driver` (
  `guid` int(11) unsigned NOT NULL auto_increment,
  `licence_name` varchar(24) NOT NULL,
  `driver_name` varchar(24) NOT NULL,
  `config_mask` smallint(4) unsigned NOT NULL default '0',
  `last_connection_time` bigint(12) unsigned NOT NULL,
  PRIMARY KEY  (`guid`),
  UNIQUE KEY `MapLicenceDriver` (`licence_name`,`driver_name`)
) ENGINE=MyISAM AUTO_INCREMENT=36 DEFAULT CHARSET=utf8;
#----------------------------
# Records for table driver
#----------------------------


insert  into driver values (1, 'Greenseed', '^5Greenseed', 0, 63356514059) ;
insert  into driver values (2, 'ghostbr', '^0Ghost^2B^3R', 0, 63356484985) ;
insert  into driver values (3, 'Wenca01', '^7BUENO ^1\"', 0, 63356488761) ;
insert  into driver values (4, 'Richardrfo', '^4Richard', 0, 63356486588) ;
insert  into driver values (5, 'Psight', '^7=^4GGIC^7=^4D^7anne', 0, 63356489934) ;
insert  into driver values (6, 'tim8956', 'sillet', 0, 63356493276) ;
insert  into driver values (7, 'maxdoel', 'maxdoel', 0, 63356508737) ;
insert  into driver values (8, 'jimmy bones', 'shark', 0, 63356504341) ;
insert  into driver values (9, 'thomazrangel', '^3Thomaz ^2Rangel', 0, 63356507424) ;
insert  into driver values (10, 'mekac', 'MeKac', 0, 63356508707) ;
insert  into driver values (11, 'ketvos', 'ketvos', 0, 63356504158) ;
insert  into driver values (12, 'Jawndus', 'Jaun', 0, 63356506433) ;
#----------------------------
# Table structure for driver_ban
#----------------------------
drop table if exists driver_ban;
CREATE TABLE `driver_ban` (
  `entry` int(10) unsigned NOT NULL,
  `licence_name` varchar(16) NOT NULL,
  `from_licence_name` varchar(16) NOT NULL,
  `reason` varchar(255) NOT NULL,
  `start_timestamp` int(12) unsigned NOT NULL,
  `duration_timestamp` int(12) unsigned NOT NULL default '0',
  `expired` tinyint(1) unsigned NOT NULL default '0',
  PRIMARY KEY  (`entry`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;
#----------------------------
# No records for table driver_ban
#----------------------------

#----------------------------
# Table structure for driver_lap
#----------------------------
drop table if exists driver_lap;
CREATE TABLE `driver_lap` (
  `guid` int(11) unsigned NOT NULL auto_increment,
  `guid_race` int(11) unsigned NOT NULL default '0',
  `guid_driver` int(11) unsigned NOT NULL,
  `car_abreviation` varchar(3) NOT NULL,
  `track_abreviation` varchar(4) NOT NULL,
  `driver_mask` mediumint(4) unsigned NOT NULL default '0',
  `split_time_1` int(12) unsigned NOT NULL,
  `split_time_2` int(12) unsigned NOT NULL default '0',
  `split_time_3` int(12) unsigned NOT NULL default '0',
  `lap_time` int(12) unsigned NOT NULL,
  `total_time` int(12) unsigned NOT NULL,
  `lap_completed` mediumint(5) unsigned NOT NULL default '0',
  `current_penalty` smallint(2) unsigned NOT NULL default '0',
  `pit_stop_count` smallint(3) unsigned NOT NULL,
  `yellow_flag_count` mediumint(4) unsigned NOT NULL default '0',
  `blue_flag_count` mediumint(4) unsigned NOT NULL default '0',
  PRIMARY KEY  (`guid`)
) ENGINE=MyISAM AUTO_INCREMENT=1082 DEFAULT CHARSET=utf8;
#----------------------------
# No records for table driver_lap
#----------------------------

#----------------------------
# Table structure for gui_template
#----------------------------
drop table if exists gui_template;
CREATE TABLE `gui_template` (
  `entry` int(11) unsigned NOT NULL,
  `description` varchar(255) NOT NULL,
  `button_entry` varchar(255) NOT NULL COMMENT 'space separated value',
  `text_button_entry` mediumint(5) unsigned NOT NULL default '0',
  `text` blob NOT NULL,
  PRIMARY KEY  (`entry`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;
#----------------------------
# Records for table gui_template
#----------------------------


insert  into gui_template values (1, 'motd', '2 3 5', 4, 0x5E3757656C636F6D6520546F20415E336C65616A65637461205E374C69766520466F72205370656564205365727665720D0A5E370D0A5E374F757220536572766572204172652043757272656E74656C7920556E64657220446576656C6F70656D656E740D0A5E37576520496D706C656D656E74652044726976655F4C465353206120496E53696D204164646F6E732E200D0A5E370D0A5E37546F206B6E6F77206D6F72652061626F75742074686973205669736974207777772E6C6673666F72756D2E6E65740D0A5E3720596F757220486F737420415E336C65616A656374615E372E) ;
#----------------------------
# Table structure for race
#----------------------------
drop table if exists race;
CREATE TABLE `race` (
  `guid` int(11) unsigned NOT NULL,
  `qualify_race_guid` int(11) unsigned NOT NULL,
  `track_prefix` varchar(4) NOT NULL,
  `start_timestamp` bigint(12) unsigned NOT NULL,
  `end_timestamp` bigint(12) unsigned NOT NULL,
  `grid_order` blob NOT NULL COMMENT 'Driver GUID seperated by Space.',
  `finish_order` blob NOT NULL,
  `race_laps` tinyint(3) unsigned NOT NULL,
  `race_status` tinyint(1) unsigned NOT NULL default '0',
  `race_feature` mediumint(4) unsigned NOT NULL default '0',
  `qualification_minute` tinyint(3) unsigned NOT NULL,
  `weather_status` tinyint(2) unsigned NOT NULL,
  `wind_status` tinyint(2) unsigned NOT NULL,
  PRIMARY KEY  (`guid`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;
#----------------------------
# No records for table race
#----------------------------

#----------------------------
# Table structure for race_map
#----------------------------
drop table if exists race_map;
CREATE TABLE `race_map` (
  `entry` mediumint(5) unsigned NOT NULL default '0',
  `race_template_entry` mediumint(5) unsigned NOT NULL,
  PRIMARY KEY  (`entry`,`race_template_entry`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;
#----------------------------
# Records for table race_map
#----------------------------


insert  into race_map values (2, 1) ;
insert  into race_map values (2, 2) ;
insert  into race_map values (2, 3) ;
insert  into race_map values (2, 4) ;
insert  into race_map values (2, 5) ;
insert  into race_map values (2, 6) ;
insert  into race_map values (2, 7) ;
insert  into race_map values (2, 8) ;
insert  into race_map values (2, 9) ;
insert  into race_map values (2, 10) ;
insert  into race_map values (2, 11) ;
#----------------------------
# Table structure for race_template
#----------------------------
drop table if exists race_template;
CREATE TABLE `race_template` (
  `entry` mediumint(5) unsigned NOT NULL,
  `description` varchar(34) NOT NULL,
  `track_entry` tinyint(2) unsigned NOT NULL default '0',
  `car_entry_allowed` varchar(64) NOT NULL default '0',
  `weather` tinyint(2) unsigned NOT NULL default '0',
  `wind` tinyint(2) unsigned NOT NULL default '0',
  `lap_count` tinyint(3) unsigned NOT NULL default '0',
  `qualify_minute` tinyint(3) unsigned NOT NULL default '0',
  `grid_start_beviator` tinyint(3) unsigned NOT NULL default '0',
  `mask` tinyint(3) unsigned NOT NULL default '0',
  PRIMARY KEY  (`entry`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;
#----------------------------
# Records for table race_template
#----------------------------


insert  into race_template values (1, 'South City Classic, LowHP', 6, '2 3', 0, 0, 11, 5, 0, 1) ;
insert  into race_template values (2, 'South City Classic Rev, LowHP', 7, '2 3', 1, 1, 12, 5, 0, 1) ;
insert  into race_template values (3, 'South City Sprint 1, LowHP', 8, '2 3', 2, 2, 13, 5, 0, 1) ;
insert  into race_template values (4, 'South City Sprint 1 Rev, LowHP', 9, '2 3', 3, 3, 14, 5, 0, 1) ;
insert  into race_template values (5, 'South City Sprint 2, LowHp', 10, '2 3', 1, 1, 16, 5, 0, 1) ;
insert  into race_template values (6, 'South City Sprint 2 Rev, LowHp', 11, '2 3', 2, 1, 15, 5, 0, 1) ;
insert  into race_template values (7, 'Blackwood GP Track, F08', 1, '19', 0, 0, 25, 8, 0, 1) ;
insert  into race_template values (8, 'Westhill, Turbo GTR', 40, '13 14 15', 0, 0, 10, 8, 0, 1) ;
insert  into race_template values (9, 'Westhill Rev, GTR Turbo', 41, '13 14 15', 0, 0, 10, 8, 0, 1) ;
insert  into race_template values (10, 'Aston Cadet Pratice, FBM', 42, '17', 0, 0, 0, 0, 0, 5) ;
insert  into race_template values (11, 'Aston Cadet, FBM', 42, '17', 0, 0, 0, 12, 0, 1) ;
#----------------------------
# Table structure for track_template
#----------------------------
drop table if exists track_template;
CREATE TABLE `track_template` (
  `entry` tinyint(2) unsigned NOT NULL,
  `name_prefix` varchar(4) NOT NULL,
  `name` varchar(16) NOT NULL,
  `configuration` varchar(16) NOT NULL,
  `reverse` tinyint(1) unsigned NOT NULL default '0',
  `split_node_index_1` tinyint(3) unsigned NOT NULL default '0',
  `split_node_index_2` tinyint(3) unsigned NOT NULL default '0',
  `split_node_index_3` tinyint(3) unsigned NOT NULL default '0',
  `total_length` mediumint(8) unsigned NOT NULL default '0',
  PRIMARY KEY  (`entry`),
  UNIQUE KEY `abreviation_name` (`name_prefix`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;
#----------------------------
# Records for table track_template
#----------------------------


insert  into track_template values (1, 'BL1', 'Blackwood', 'GP Track', 0, 0, 0, 0, 0) ;
insert  into track_template values (2, 'BL1R', 'Blackwood', 'GP Track', 1, 0, 0, 0, 0) ;
insert  into track_template values (3, 'BL2', 'Blackwood', 'Rallycross', 0, 0, 0, 0, 0) ;
insert  into track_template values (4, 'BL2R', 'Blackwood', 'Rallycross', 1, 0, 0, 0, 0) ;
insert  into track_template values (5, 'BL3', 'Blackwood', 'Car Park', 0, 0, 0, 0, 0) ;
insert  into track_template values (6, 'SO1', 'South City', 'Classic', 0, 0, 0, 0, 0) ;
insert  into track_template values (7, 'SO1R', 'South City', 'Classic', 1, 0, 0, 0, 0) ;
insert  into track_template values (8, 'SO2', 'South City', 'Sprint 1', 0, 0, 0, 0, 0) ;
insert  into track_template values (9, 'SO2R', 'South City', 'Sprint 1', 1, 0, 0, 0, 0) ;
insert  into track_template values (10, 'SO3', 'South City', 'Sprint 2', 0, 0, 0, 0, 0) ;
insert  into track_template values (11, 'SO3R', 'South City', 'Sprint 2', 1, 0, 0, 0, 0) ;
insert  into track_template values (12, 'SO4', 'South City', 'City Long', 0, 0, 0, 0, 0) ;
insert  into track_template values (13, 'SO4R', 'South City', 'City Long', 1, 0, 0, 0, 0) ;
insert  into track_template values (14, 'SO5', 'South City', 'Town Course', 0, 0, 0, 0, 0) ;
insert  into track_template values (15, 'SO5R', 'South City', 'Town Course', 1, 0, 0, 0, 0) ;
insert  into track_template values (16, 'SO6', 'South City', 'Chicane Route', 0, 0, 0, 0, 0) ;
insert  into track_template values (17, 'SO6R', 'South City', 'Chicane Route', 1, 0, 0, 0, 0) ;
insert  into track_template values (18, 'FE1', 'Fern Bay', 'Club', 0, 0, 0, 0, 0) ;
insert  into track_template values (19, 'FE1R', 'Fern Bay', 'Club', 1, 0, 0, 0, 0) ;
insert  into track_template values (20, 'FE2', 'Fern Bay', 'Green', 0, 0, 0, 0, 0) ;
insert  into track_template values (21, 'FE2R', 'Fern Bay', 'Green', 1, 0, 0, 0, 0) ;
insert  into track_template values (22, 'FE3', 'Fern Bay', 'Gold', 0, 0, 0, 0, 0) ;
insert  into track_template values (23, 'FE3R', 'Fern Bay', 'Gold', 1, 0, 0, 0, 0) ;
insert  into track_template values (24, 'FE4', 'Fern Bay', 'Black', 0, 0, 0, 0, 0) ;
insert  into track_template values (25, 'FE4R', 'Fern Bay', 'Black', 1, 0, 0, 0, 0) ;
insert  into track_template values (26, 'FE5', 'Fern Bay', 'Rallycross', 0, 0, 0, 0, 0) ;
insert  into track_template values (27, 'FE5R', 'Fern Bay', 'Rallycross', 1, 0, 0, 0, 0) ;
insert  into track_template values (28, 'FE6', 'Fern Bay', 'RallyX Green', 0, 0, 0, 0, 0) ;
insert  into track_template values (29, 'FE6R', 'Fern Bay', 'RallyX Green', 1, 0, 0, 0, 0) ;
insert  into track_template values (30, 'AU1', 'Autocross', 'Autocross', 0, 0, 0, 0, 0) ;
insert  into track_template values (31, 'AU2', 'Autocross', 'Skid Pad', 0, 0, 0, 0, 0) ;
insert  into track_template values (32, 'AU3', 'Autocross', 'Drag Strip', 0, 0, 0, 0, 0) ;
insert  into track_template values (33, 'AU4', 'Autocross', '8 Lane Drag', 0, 0, 0, 0, 0) ;
insert  into track_template values (34, 'KY1', 'Kyoto Ring', 'Oval', 0, 0, 0, 0, 0) ;
insert  into track_template values (35, 'KY1R', 'Kyoto Ring', 'Oval', 1, 0, 0, 0, 0) ;
insert  into track_template values (36, 'KY2', 'Kyoto Ring', 'National', 0, 0, 0, 0, 0) ;
insert  into track_template values (37, 'KY2R', 'Kyoto Ring', 'National', 1, 0, 0, 0, 0) ;
insert  into track_template values (38, 'KY3', 'Kyoto Ring', 'GP Long', 0, 0, 0, 0, 0) ;
insert  into track_template values (39, 'KY3R', 'Kyoto Ring', 'GP Long', 1, 0, 0, 0, 0) ;
insert  into track_template values (40, 'WE1', 'Westhill', 'International', 0, 0, 0, 0, 0) ;
insert  into track_template values (41, 'WE1R', 'Westhill', 'International', 1, 0, 0, 0, 0) ;
insert  into track_template values (42, 'AS1', 'Aston', 'Cadet', 0, 0, 0, 0, 0) ;
insert  into track_template values (43, 'AS1R', 'Aston', 'Cadet', 1, 0, 0, 0, 0) ;
insert  into track_template values (44, 'AS2', 'Aston', 'Club', 0, 0, 0, 0, 0) ;
insert  into track_template values (45, 'AS2R', 'Aston', 'Club', 1, 0, 0, 0, 0) ;
insert  into track_template values (46, 'AS3', 'Aston', 'National', 0, 0, 0, 0, 0) ;
insert  into track_template values (47, 'AS3R', 'Aston', 'National', 1, 0, 0, 0, 0) ;
insert  into track_template values (48, 'AS4', 'Aston', 'Historic', 0, 0, 0, 0, 0) ;
insert  into track_template values (49, 'AS4R', 'Aston', 'Historic', 1, 0, 0, 0, 0) ;
insert  into track_template values (50, 'AS5', 'Aston', 'Grand Prix', 0, 0, 0, 0, 0) ;
insert  into track_template values (51, 'AS5R', 'Aston', 'Grand Prix', 1, 0, 0, 0, 0) ;
insert  into track_template values (52, 'AS6', 'Aston', 'Grand Touring', 0, 0, 0, 0, 0) ;
insert  into track_template values (53, 'AS6R', 'Aston', 'Grand Touring', 1, 0, 0, 0, 0) ;
insert  into track_template values (54, 'AS7', 'Aston', 'North', 0, 0, 0, 0, 0) ;
insert  into track_template values (55, 'AS7R', 'Aston', 'North', 1, 0, 0, 0, 0) ;

