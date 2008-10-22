-- Table "restriction_race"
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
