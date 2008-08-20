/*
MySQL Backup
Source Host:           localhost
Source Server Version: 5.0.27-community-nt
Source Database:       drive_lfss
Date:                  2008/08/20 01:25:26
*/

SET FOREIGN_KEY_CHECKS=0;
#----------------------------
# Table structure for car_template
#----------------------------
drop table if exists car_template;
CREATE TABLE `car_template` (
  `entry` smallint(2) unsigned NOT NULL,
  `abreviation_name` varchar(3) NOT NULL,
  `name` varchar(16) NOT NULL,
  `traction_flag` tinyint(3) unsigned NOT NULL default '0',
  PRIMARY KEY  (`entry`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;
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
) ENGINE=MyISAM AUTO_INCREMENT=13 DEFAULT CHARSET=utf8;
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
  `lap_completed` tinyint(2) unsigned NOT NULL default '0',
  `current_penalty` smallint(2) unsigned NOT NULL default '0',
  `pit_stop_count` smallint(3) unsigned NOT NULL,
  `yellow_flag_count` mediumint(4) unsigned NOT NULL default '0',
  `blue_flag_count` mediumint(4) unsigned NOT NULL default '0',
  PRIMARY KEY  (`guid`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;
#----------------------------
# Table structure for race
#----------------------------
drop table if exists race;
CREATE TABLE `race` (
  `guid` int(11) unsigned NOT NULL,
  `track_abreviation` varchar(4) NOT NULL,
  `start_timestamp` bigint(12) unsigned NOT NULL,
  `end_timestamp` bigint(12) unsigned NOT NULL,
  `grid_order` blob NOT NULL COMMENT 'Driver GUID seperated by Space.',
  `finish_order` blob NOT NULL,
  `race_laps` tinyint(3) unsigned NOT NULL,
  `race_feature` mediumint(4) unsigned NOT NULL default '0',
  `qualification_minute` tinyint(3) unsigned NOT NULL,
  `weather_status` tinyint(2) unsigned NOT NULL,
  `wind_status` tinyint(2) unsigned NOT NULL,
  PRIMARY KEY  (`guid`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;
#----------------------------
# Table structure for track_template
#----------------------------
drop table if exists track_template;
CREATE TABLE `track_template` (
  `entry` tinyint(2) unsigned NOT NULL,
  `abreviation_name` varchar(4) NOT NULL,
  `name` varchar(16) NOT NULL,
  `split_node_index_1` tinyint(3) NOT NULL default '0',
  `split_node_index_2` tinyint(3) unsigned NOT NULL default '0',
  `split_node_index_3` tinyint(3) unsigned NOT NULL default '0',
  `total_length` mediumint(8) unsigned NOT NULL,
  PRIMARY KEY  (`entry`),
  UNIQUE KEY `abreviation_name` (`abreviation_name`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

