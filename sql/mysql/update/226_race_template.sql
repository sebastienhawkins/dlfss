-- Add  new Race Template for race map 5
DELETE FROM `race_template` WHERE `entry`IN(40,41,42,43,44,45,46,47,48);
insert into `race_template` values 
(40, 'Aston Grand Prix, BF1', 50, '20', 0, 0, 5, 0, 0, 1),
(41, 'Western Hill, GTR', 40, '13 14 15', 0, 0, 5, 0, 0, 1),
(42, 'Kyoto GP, FO8', 38, '19', 0, 0, 5, 0, 0, 1),
(43, 'Blackwood Rally, LX6', 2, '5', 0, 0, 5, 0, 0, 1),
(44, 'Western Hill Rev, BF1', 41, '20', 0, 0, 5, 0, 0, 1),
(45, 'Aston Historic Rev, BF1', 49, '20', 0, 0, 5, 0, 0, 1),
(46, 'Fernbay Black Rev, FO8', 25, '19', 0, 0, 5, 0, 0, 1),
(47, 'Aston Club Rev, FO8', 45, '20', 0, 0, 5, 0, 0, 1),
(48, 'Kyoto Ring, BF1', 34, '20', 0, 0, 7, 0, 0, 1);

-- Add  new Race Template for race map 6
DELETE FROM `race_template` WHERE `entry`IN(49,50);
insert into `race_template` values 
(49, 'Kyoto Ring, Nascar', 34, '14 15', 0, 0, 10, 0, 0, 1),
(50, 'Kyoto Ring Rev, Nascar', 35, '14 15', 0, 0, 10, 0, 0, 1);

-- Update AleajectaC Blackwood
UPDATE `race_template` SET `lap_count`=5 WHERE `entry`=39;

ALTER TABLE `race_template` ADD COLUMN `entry_restriction` mediumint(5) unsigned NOT NULL default '0' AFTER `description`;

