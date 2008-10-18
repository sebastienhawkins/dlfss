-- Add position Colum
ALTER TABLE `stats_rank_driver` ADD COLUMN `change_mask` INT(11) unsigned NOT NULL default '0' after `position`;

-- update the gui
UPDATE `gui_template` SET `text`="^7Top20 ^2is the 20 Best Driver for this Track/Car.
^7Search ^2search for a Track/Car/Driver rank.
^7Current ^2show rank for all online driver by Track/Car.
^6Blue mean worst^2, ^3Yellow mean Better^2, ^7White mean event
^2
^2When button become ^1red ^2mean clearing for flood protection " 
WHERE `entry`=5;
