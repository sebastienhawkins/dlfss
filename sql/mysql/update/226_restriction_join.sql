-- Table "restriction_join"
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
