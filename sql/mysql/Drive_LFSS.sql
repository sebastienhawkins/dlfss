/*
MySQL Data Transfer
Source Host: 192.168.101.200
Source Database: drive_lfss
Target Host: 192.168.101.200
Target Database: drive_lfss
Date: 11/13/2008 7:25:01 PM
*/

SET FOREIGN_KEY_CHECKS=0;
-- ----------------------------
-- Table structure for bad_word
-- ----------------------------
DROP TABLE IF EXISTS `bad_word`;
CREATE TABLE `bad_word` (
  `word` varchar(16) character set latin1 collate latin1_general_ci NOT NULL default '',
  `mask` tinyint(1) unsigned NOT NULL default '0',
  PRIMARY KEY  (`word`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;

-- ----------------------------
-- Table structure for button_template
-- ----------------------------
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
  `caption_text` varchar(240) character set latin1 collate latin1_general_ci NOT NULL default '',
  PRIMARY KEY  (`entry`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for car_template
-- ----------------------------
DROP TABLE IF EXISTS `car_template`;
CREATE TABLE `car_template` (
  `entry` smallint(2) unsigned NOT NULL,
  `name_prefix` varchar(3) NOT NULL default '',
  `name` varchar(16) NOT NULL default '',
  `brake_distance` smallint(3) unsigned NOT NULL default '0',
  `mask` int(11) unsigned NOT NULL default '0',
  PRIMARY KEY  (`entry`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for driver
-- ----------------------------
DROP TABLE IF EXISTS `driver`;
CREATE TABLE `driver` (
  `guid` int(11) unsigned NOT NULL auto_increment,
  `licence_name` varchar(24) NOT NULL,
  `driver_name` varchar(24) NOT NULL,
  `password` varchar(128) NOT NULL default '',
  `config_data` blob,
  `tutorial` varchar(255) NOT NULL default '',
  `warning_driving_count` int(8) unsigned NOT NULL default '0',
  `warning_chat_count` int(8) unsigned NOT NULL default '0',
  `flood_chat_count` int(8) unsigned NOT NULL default '0',
  `last_connection_time` bigint(12) unsigned NOT NULL default '0',
  `time_spec` int(12) unsigned NOT NULL default '0',
  `time_garage` int(12) unsigned NOT NULL default '0',
  `time_racing` int(12) unsigned NOT NULL default '0',
  PRIMARY KEY  (`guid`),
  UNIQUE KEY `licence_name` (`licence_name`)
) ENGINE=MyISAM AUTO_INCREMENT=3353 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for driver_ban
-- ----------------------------
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

-- ----------------------------
-- Table structure for driver_lap
-- ----------------------------
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

-- ----------------------------
-- Table structure for gui_template
-- ----------------------------
DROP TABLE IF EXISTS `gui_template`;
CREATE TABLE `gui_template` (
  `entry` int(11) unsigned NOT NULL,
  `description` varchar(255) NOT NULL default '',
  `button_entry` varchar(512) NOT NULL default '' COMMENT 'space separated value',
  `button_entry_ext` varchar(255) NOT NULL default '',
  `text_button_entry` mediumint(5) unsigned NOT NULL default '0',
  `text` blob NOT NULL,
  PRIMARY KEY  (`entry`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for race
-- ----------------------------
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
) ENGINE=MyISAM AUTO_INCREMENT=14297 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for race_map
-- ----------------------------
DROP TABLE IF EXISTS `race_map`;
CREATE TABLE `race_map` (
  `entry` mediumint(5) unsigned NOT NULL default '0',
  `race_template_entry` mediumint(5) unsigned NOT NULL default '0',
  `description` varchar(64) NOT NULL default '',
  PRIMARY KEY  (`entry`,`race_template_entry`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for race_template
-- ----------------------------
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
  `maximun_race_finish` tinyint(3) unsigned NOT NULL default '0',
  `race_template_mask` smallint(5) unsigned NOT NULL default '0',
  `restriction_join_entry` mediumint(5) unsigned NOT NULL default '0',
  `restriction_race_entry` mediumint(5) unsigned NOT NULL default '0',
  PRIMARY KEY  (`entry`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for restriction_join
-- ----------------------------
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
  `wr_diff_min` int(12) unsigned NOT NULL default '0',
  `wr_diff_max` int(12) unsigned NOT NULL default '0',
  `wr_diff_kick` tinyint(1) unsigned NOT NULL default '0',
  `skin_name` varchar(16) character set latin1 collate latin1_general_ci NOT NULL default '',
  `skin_name_kick` tinyint(1) unsigned NOT NULL default '0',
  `driver_name` varchar(16) NOT NULL default '',
  `driver_name_kick` tinyint(1) unsigned NOT NULL default '0',
  `rank_best_min` smallint(6) unsigned NOT NULL default '0',
  `rank_best_max` smallint(6) unsigned NOT NULL default '0',
  `rank_avg_min` smallint(6) unsigned NOT NULL default '0',
  `rank_avg_max` smallint(6) unsigned NOT NULL default '0',
  `rank_sta_min` smallint(6) unsigned NOT NULL default '0',
  `rank_sta_max` smallint(6) unsigned NOT NULL default '0',
  `rank_win_min` smallint(6) unsigned NOT NULL default '0',
  `rank_win_max` smallint(6) unsigned NOT NULL default '0',
  `rank_total_min` smallint(6) unsigned NOT NULL default '0',
  `rank_total_max` smallint(6) unsigned NOT NULL default '0',
  `rank_kick` tinyint(1) unsigned NOT NULL default '0',
  PRIMARY KEY  (`entry`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for restriction_race
-- ----------------------------
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

-- ----------------------------
-- Table structure for session
-- ----------------------------
DROP TABLE IF EXISTS `session`;
CREATE TABLE `session` (
  `session_name` varchar(64) NOT NULL default '',
  `config_data` varchar(255) NOT NULL default '',
  `motd_message` blob NOT NULL,
  `rules_message` blob NOT NULL,
  PRIMARY KEY  (`session_name`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for stats_rank_driver
-- ----------------------------
DROP TABLE IF EXISTS `stats_rank_driver`;
CREATE TABLE `stats_rank_driver` (
  `licence_name` varchar(32) NOT NULL,
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

-- ----------------------------
-- Table structure for stats_rank_team
-- ----------------------------
DROP TABLE IF EXISTS `stats_rank_team`;
CREATE TABLE `stats_rank_team` (
  `team_name` varchar(64) character set latin1 collate latin1_general_ci NOT NULL,
  `tag` varchar(32) character set latin1 collate latin1_general_ci NOT NULL default '',
  `member_count` smallint(5) NOT NULL default '0',
  `track_prefix` varchar(4) NOT NULL default '',
  `car_prefix` varchar(3) NOT NULL default '',
  `best_lap_rank` smallint(5) NOT NULL default '0',
  `average_lap_rank` smallint(5) NOT NULL default '0',
  `stability_rank` smallint(5) NOT NULL default '0',
  `race_win_rank` mediumint(8) unsigned NOT NULL default '0',
  `total_rank` mediumint(8) unsigned NOT NULL default '0',
  `position` int(8) unsigned NOT NULL default '0',
  `change_mask` int(11) unsigned NOT NULL default '0',
  PRIMARY KEY  (`team_name`,`track_prefix`,`car_prefix`),
  KEY `prefix` (`track_prefix`,`car_prefix`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for track_template
-- ----------------------------
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
