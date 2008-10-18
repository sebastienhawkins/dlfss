--
-- Table structure for table `driver_rank`
--
DROP TABLE IF EXISTS `stats_rank_driver`;
CREATE TABLE `stats_rank_driver` (
  `licence_name` varchar(32) character set latin1 collate latin1_general_ci NOT NULL,
  `track_prefix` varchar(4) NOT NULL default '',
  `car_prefix` varchar(3) NOT NULL default '',
  `best_lap_rank` smallint(5) NOT NULL default '0',
  `average_lap_rank` smallint(5) NOT NULL default '0',
  `stability_rank` smallint(5) NOT NULL default '0',
  `race_win_rank` smallint(5) NOT NULL default '0',
  `total_rank` smallint(5) NOT NULL default '0',
  PRIMARY KEY  (`licence_name`,`track_prefix`,`car_prefix`)
) DEFAULT CHARSET=latin1;
