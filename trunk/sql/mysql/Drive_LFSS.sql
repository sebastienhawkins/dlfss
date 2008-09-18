/*
MySQL Backup
Source Host:           192.168.101.200
Source Server Version: 5.0.51a-community-nt
Source Database:       drive_lfss
Date:                  2008/09/18 06:05:54
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

lock tables button_template write ;

insert  into button_template (entry,description,style_mask,is_allways_visible,max_input_char,`left`,top,width,height,`text`) values 
(1, 'banner', 66, 1, 0, 35, 194, 20, 7, '^7A^3leajecta'), 
(2, 'motd background', 98, 1, 0, 0, 0, 200, 200, ''), 
(10, 'vote option 1', 106, 0, 0, 2, 73, 45, 6, 'vote option 1'), 
(11, 'vote option 2', 106, 0, 0, 2, 80, 45, 6, 'vote option 2'), 
(3, 'motd upper', 146, 1, 0, 45, 60, 110, 7, '^7A^3leajecta'), 
(5, 'motd button', 26, 1, 0, 75, 125, 50, 12, '^2Drive'), 
(6, 'message bar top', 2, 0, 0, 60, 25, 80, 16, '^1Message bar top'), 
(7, 'message bar middle', 2, 0, 0, 25, 48, 150, 16, '^1Message bar Middle'), 
(8, 'Collision Warning', 2, 0, 0, 25, 68, 150, 8, '^1CollisionWarning'), 
(9, 'vote title', 146, 0, 0, 2, 65, 50, 8, 'vote title'), 
(4, 'motd text line', 98, 1, 0, 50, 67, 100, 8, ''), 
(12, 'vote option 3', 106, 0, 0, 2, 87, 45, 6, 'vote option 3'), 
(13, 'vote option 4', 106, 0, 0, 2, 94, 45, 6, 'vote option 4'), 
(14, 'vote option 5', 106, 0, 0, 2, 101, 45, 6, 'vote option 5'), 
(15, 'vote option 6', 106, 0, 0, 2, 108, 45, 6, 'vote option 6'), 
(16, 'track prefix', 66, 1, 0, 56, 194, 10, 7, ''), 
(17, 'info 1', 82, 0, 0, 180, 85, 20, 5, 'info 1'), 
(18, 'info 2', 85, 0, 0, 180, 91, 20, 5, 'info 2'), 
(19, 'info 3', 87, 0, 0, 180, 97, 20, 5, 'info 3'), 
(20, 'info 4', 86, 0, 0, 180, 103, 20, 5, 'info 4'), 
(21, 'info 5', 84, 0, 0, 180, 109, 20, 5, 'info 5');
unlock tables ;
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

lock tables car_template write ;

insert  into car_template (entry,name_prefix,name,mask) values 
(1, 'UF1', 'UF 1000', 0), 
(2, 'XFG', 'XF GTI', 0), 
(3, 'XRG', 'XR GT', 0), 
(4, 'LX4', 'LX4', 0), 
(5, 'LX6', 'LX6', 0), 
(6, 'RB4', 'RB4 GT', 0), 
(7, 'FXO', 'FXO TURBO', 0), 
(8, 'XRT', 'XR GT TURBO', 0), 
(9, 'RAC', 'RACEABOUT', 0), 
(10, 'FZ5', 'FZ5', 0), 
(11, 'UFR', 'UF GTR', 0), 
(12, 'XFR', 'XF GTR', 0), 
(13, 'FXR', 'FXO GTR', 0), 
(14, 'XRR', 'XR GTR', 0), 
(15, 'FZR', 'FZ50 GTR', 0), 
(16, 'MRT', 'MRT5', 0), 
(17, 'FBM', 'FORMULA BMW FB02', 0), 
(18, 'FOX', 'FORMULA XR', 0), 
(19, 'FO8', 'FORMULA V8', 0), 
(20, 'BF1', 'BMW SAUBER F1.06', 0);
unlock tables ;
#----------------------------
# Table structure for driver
#----------------------------
drop table if exists driver;
CREATE TABLE `driver` (
  `guid` int(11) unsigned NOT NULL auto_increment,
  `licence_name` varchar(24) NOT NULL,
  `driver_name` varchar(24) NOT NULL,
  `config_mask` smallint(5) unsigned NOT NULL default '0',
  `last_connection_time` bigint(12) unsigned NOT NULL,
  PRIMARY KEY  (`guid`),
  UNIQUE KEY `MapLicenceDriver` (`licence_name`,`driver_name`)
) ENGINE=MyISAM AUTO_INCREMENT=36 DEFAULT CHARSET=utf8;
#----------------------------
# No records for table driver
#----------------------------

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
  `car_prefix` varchar(3) NOT NULL,
  `track_prefix` varchar(4) NOT NULL,
  `driver_mask` mediumint(5) unsigned NOT NULL default '0',
  `split_time_1` int(12) unsigned NOT NULL,
  `split_time_2` int(12) unsigned NOT NULL default '0',
  `split_time_3` int(12) unsigned NOT NULL default '0',
  `lap_time` int(12) unsigned NOT NULL,
  `total_time` int(12) unsigned NOT NULL,
  `lap_completed` mediumint(5) unsigned NOT NULL default '0',
  `current_penalty` tinyint(2) unsigned NOT NULL default '0',
  `pit_stop_count` tinyint(3) unsigned NOT NULL,
  `yellow_flag_count` smallint(4) unsigned NOT NULL default '0',
  `blue_flag_count` mediumint(4) unsigned NOT NULL default '0',
  PRIMARY KEY  (`guid`)
) ENGINE=MyISAM AUTO_INCREMENT=1153 DEFAULT CHARSET=utf8;
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

lock tables gui_template write ;

insert  into gui_template (entry,description,button_entry,text_button_entry,`text`) values 
(1, 'motd', '2 3 5', 4, '^7Welcome To A^3leajecta ^7Live For Speed Server\r\n^7\r\n^7Our Server Are Currentely Under Developement\r\n^7We Implemente Drive_LFSS a InSim Addons. \r\n^7\r\n^7To know more about this Visit www.lfsforum.net\r\n^7 Your Host A^3leajecta^7.');
unlock tables ;
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
  `grid_order` varchar(255) NOT NULL COMMENT 'Driver GUID seperated by Space.',
  `finish_order` varchar(255) NOT NULL,
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

lock tables race_map write ;

insert  into race_map (entry,race_template_entry) values 
(2, 1), 
(2, 2), 
(2, 3), 
(2, 4), 
(2, 5), 
(2, 6), 
(2, 7), 
(2, 8), 
(2, 9), 
(2, 10), 
(2, 11), 
(2, 12), 
(2, 13), 
(2, 14), 
(2, 15), 
(2, 16), 
(2, 17), 
(2, 18), 
(2, 19), 
(2, 20);
unlock tables ;
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
  `race_template_mask` tinyint(3) unsigned NOT NULL default '0',
  PRIMARY KEY  (`entry`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;
#----------------------------
# Records for table race_template
#----------------------------

lock tables race_template write ;

insert  into race_template (entry,description,track_entry,car_entry_allowed,weather,wind,lap_count,qualify_minute,grid_start_beviator,race_template_mask) values 
(1, 'South City Classic, 115Hp', 6, '2 3', 0, 0, 11, 5, 0, 1), 
(2, 'South City Classic Rev,  115Hp', 7, '2 3', 1, 1, 12, 5, 0, 1), 
(3, 'South City Sprint 1, 115Hp', 8, '2 3', 2, 2, 13, 5, 0, 1), 
(4, 'South City Sprint 1 Rev, 115Hp', 9, '2 3', 3, 3, 14, 5, 0, 1), 
(5, 'South City Sprint 2, 115Hp', 10, '2 3', 1, 1, 16, 5, 0, 1), 
(6, 'South City Sprint 2 Rev, 115Hp', 11, '2 3', 2, 1, 15, 5, 0, 1), 
(7, 'Blackwood GP Track, F08', 1, '19', 0, 0, 25, 8, 0, 1), 
(8, 'Westhill, Turbo GTR', 40, '13 14 15', 0, 0, 10, 8, 0, 1), 
(9, 'Westhill Rev, GTR Turbo', 41, '13 14 15', 0, 0, 10, 8, 0, 1), 
(10, 'Aston Cadet Pratice, Formule', 42, '16 17 18 19 20', 0, 0, 0, 0, 0, 5), 
(11, 'Aston Cadet, FBM', 42, '17', 0, 0, 10, 12, 0, 1), 
(12, 'South City Sprint 1 Rev, FBM', 9, '17', 0, 0, 11, 5, 0, 1), 
(13, 'Fern Bay Black, FOX R1', 24, '18', 0, 0, 8, 5, 0, 1), 
(14, 'Fern Bay Black Rev, FOX R1', 25, '18', 0, 0, 8, 0, 0, 1), 
(15, 'Aston Club, Formule Pratice', 44, '16 17 18 19', 0, 0, 0, 0, 0, 5), 
(16, 'Aston Club Rev, Formule Pratice', 45, '16 17 18 19', 0, 0, 0, 0, 0, 5), 
(17, 'Blackwood Rallycross, LX6', 3, '5', 2, 0, 18, 5, 0, 1), 
(18, 'Blackwood Rallycross Rev, LX6', 4, '5', 2, 0, 18, 5, 0, 1), 
(19, 'Fern Bay Club, MRT', 18, '2 3', 0, 0, 11, 5, 0, 1), 
(20, 'Fern Bay Club Rev, F GTR', 19, '11 12', 0, 0, 14, 5, 0, 1);
unlock tables ;
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

lock tables track_template write ;

insert  into track_template (entry,name_prefix,name,configuration,`reverse`,split_node_index_1,split_node_index_2,split_node_index_3,total_length) values 
(1, 'BL1', 'Blackwood', 'GP Track', 0, 0, 0, 0, 0), 
(2, 'BL1R', 'Blackwood', 'GP Track', 1, 0, 0, 0, 0), 
(3, 'BL2', 'Blackwood', 'Rallycross', 0, 0, 0, 0, 0), 
(4, 'BL2R', 'Blackwood', 'Rallycross', 1, 0, 0, 0, 0), 
(5, 'BL3', 'Blackwood', 'Car Park', 0, 0, 0, 0, 0), 
(6, 'SO1', 'South City', 'Classic', 0, 0, 0, 0, 0), 
(7, 'SO1R', 'South City', 'Classic', 1, 0, 0, 0, 0), 
(8, 'SO2', 'South City', 'Sprint 1', 0, 0, 0, 0, 0), 
(9, 'SO2R', 'South City', 'Sprint 1', 1, 0, 0, 0, 0), 
(10, 'SO3', 'South City', 'Sprint 2', 0, 0, 0, 0, 0), 
(11, 'SO3R', 'South City', 'Sprint 2', 1, 0, 0, 0, 0), 
(12, 'SO4', 'South City', 'City Long', 0, 0, 0, 0, 0), 
(13, 'SO4R', 'South City', 'City Long', 1, 0, 0, 0, 0), 
(14, 'SO5', 'South City', 'Town Course', 0, 0, 0, 0, 0), 
(15, 'SO5R', 'South City', 'Town Course', 1, 0, 0, 0, 0), 
(16, 'SO6', 'South City', 'Chicane Route', 0, 0, 0, 0, 0), 
(17, 'SO6R', 'South City', 'Chicane Route', 1, 0, 0, 0, 0), 
(18, 'FE1', 'Fern Bay', 'Club', 0, 0, 0, 0, 0), 
(19, 'FE1R', 'Fern Bay', 'Club', 1, 0, 0, 0, 0), 
(20, 'FE2', 'Fern Bay', 'Green', 0, 0, 0, 0, 0), 
(21, 'FE2R', 'Fern Bay', 'Green', 1, 0, 0, 0, 0), 
(22, 'FE3', 'Fern Bay', 'Gold', 0, 0, 0, 0, 0), 
(23, 'FE3R', 'Fern Bay', 'Gold', 1, 0, 0, 0, 0), 
(24, 'FE4', 'Fern Bay', 'Black', 0, 0, 0, 0, 0), 
(25, 'FE4R', 'Fern Bay', 'Black', 1, 0, 0, 0, 0), 
(26, 'FE5', 'Fern Bay', 'Rallycross', 0, 0, 0, 0, 0), 
(27, 'FE5R', 'Fern Bay', 'Rallycross', 1, 0, 0, 0, 0), 
(28, 'FE6', 'Fern Bay', 'RallyX Green', 0, 0, 0, 0, 0), 
(29, 'FE6R', 'Fern Bay', 'RallyX Green', 1, 0, 0, 0, 0), 
(30, 'AU1', 'Autocross', 'Autocross', 0, 0, 0, 0, 0), 
(31, 'AU2', 'Autocross', 'Skid Pad', 0, 0, 0, 0, 0), 
(32, 'AU3', 'Autocross', 'Drag Strip', 0, 0, 0, 0, 0), 
(33, 'AU4', 'Autocross', '8 Lane Drag', 0, 0, 0, 0, 0), 
(34, 'KY1', 'Kyoto Ring', 'Oval', 0, 0, 0, 0, 0), 
(35, 'KY1R', 'Kyoto Ring', 'Oval', 1, 0, 0, 0, 0), 
(36, 'KY2', 'Kyoto Ring', 'National', 0, 0, 0, 0, 0), 
(37, 'KY2R', 'Kyoto Ring', 'National', 1, 0, 0, 0, 0), 
(38, 'KY3', 'Kyoto Ring', 'GP Long', 0, 0, 0, 0, 0), 
(39, 'KY3R', 'Kyoto Ring', 'GP Long', 1, 0, 0, 0, 0), 
(40, 'WE1', 'Westhill', 'International', 0, 0, 0, 0, 0), 
(41, 'WE1R', 'Westhill', 'International', 1, 0, 0, 0, 0), 
(42, 'AS1', 'Aston', 'Cadet', 0, 0, 0, 0, 0), 
(43, 'AS1R', 'Aston', 'Cadet', 1, 0, 0, 0, 0), 
(44, 'AS2', 'Aston', 'Club', 0, 0, 0, 0, 0), 
(45, 'AS2R', 'Aston', 'Club', 1, 0, 0, 0, 0), 
(46, 'AS3', 'Aston', 'National', 0, 0, 0, 0, 0), 
(47, 'AS3R', 'Aston', 'National', 1, 0, 0, 0, 0), 
(48, 'AS4', 'Aston', 'Historic', 0, 0, 0, 0, 0), 
(49, 'AS4R', 'Aston', 'Historic', 1, 0, 0, 0, 0), 
(50, 'AS5', 'Aston', 'Grand Prix', 0, 0, 0, 0, 0), 
(51, 'AS5R', 'Aston', 'Grand Prix', 1, 0, 0, 0, 0), 
(52, 'AS6', 'Aston', 'Grand Touring', 0, 0, 0, 0, 0), 
(53, 'AS6R', 'Aston', 'Grand Touring', 1, 0, 0, 0, 0), 
(54, 'AS7', 'Aston', 'North', 0, 0, 0, 0, 0), 
(55, 'AS7R', 'Aston', 'North', 1, 0, 0, 0, 0);
unlock tables ;

