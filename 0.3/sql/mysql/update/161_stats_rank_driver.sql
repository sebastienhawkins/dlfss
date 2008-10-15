-- Add position Colum
ALTER TABLE `stats_rank_driver` ADD COLUMN `position` INT(8) unsigned NOT NULL default '0' after `total_rank`;
ALTER TABLE `stats_rank_driver` CHANGE `race_win_rank` `race_win_rank` mediumint(8) unsigned NOT NULL default '0';
ALTER TABLE `stats_rank_driver` CHANGE `total_rank` `total_rank` mediumint(8) unsigned NOT NULL default '0';

-- Add index to faster the loading of by track/car or by track
ALTER TABLE `stats_rank_driver` ADD INDEX `prefix`(`track_prefix`,`car_prefix`);
