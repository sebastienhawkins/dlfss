-- Add  new Race Template for race map 8
DELETE FROM `race_template` WHERE `entry`IN(51,52,53,54,55,56,57,58,59);
insert into `race_template` values 
(51, 'Blackwood GP Clear, 5 Laps', 1, '17', 1, 0, 5, 0, 0, 1 ,5,0),
(52, 'Blackwood GP Cloud, 5 Laps', 1, '17', 2, 0, 5, 0, 0, 1 ,5,0),
(53, 'Blackwood GP Cloud2, 5 Laps', 1, '17', 3, 0, 5, 0, 0, 1 ,5,0),
(54, 'Blackwood GP Clear, 7 Laps', 1, '17', 1, 0, 7, 0, 0, 1 ,5,0),
(55, 'Blackwood GP Cloud, 7 Laps', 1, '17', 2, 0, 7, 0, 0, 1 ,5,0),
(56, 'Blackwood GP Cloud2, 7 Laps', 1, '17', 3, 0, 7, 0, 0, 1 ,5,0),
(57, 'Blackwood GP Clear, 10 Laps', 1, '17', 1, 0, 10, 0, 0, 1 ,5,0),
(58, 'Blackwood GP Cloud, 10 Laps', 1, '17', 2, 0, 10, 0, 0, 1 ,5,0),
(59, 'Blackwood GP Cloud2, 10 Laps', 1, '17', 3, 0, 10, 0, 0, 1 ,5,0);

-- Add  new Race Template for race map 9
DELETE FROM `race_template` WHERE `entry`IN(60,61,62,63,64,65,67,68);
insert into `race_template` values 
(60, 'Blackwood GP Clear, 5 Laps', 1, '17', 1, 0, 5, 0, 0, 1 ,6,0),
(61, 'Blackwood GP Cloud, 5 Laps', 1, '17', 2, 0, 5, 0, 0, 1 ,6,0),
(62, 'Blackwood GP Cloud2, 5 Laps', 1, '17', 3, 0, 5, 0, 0, 1 ,6,0),
(63, 'Blackwood GP Clear, 7 Laps', 1, '17', 1, 0, 7, 0, 0, 1 ,6,0),
(64, 'Blackwood GP Cloud, 7 Laps', 1, '17', 2, 0, 7, 0, 0, 1 ,6,0),
(65, 'Blackwood GP Cloud2, 7 Laps', 1, '17', 3, 0, 7, 0, 0, 1 ,6,0),
(66, 'Blackwood GP Clear, 10 Laps', 1, '17', 1, 0, 10, 0, 0, 1 ,6,0),
(67, 'Blackwood GP Cloud, 10 Laps', 1, '17', 2, 0, 10, 0, 0, 1 ,6,0),
(68, 'Blackwood GP Cloud2, 10 Laps', 1, '17', 3, 0, 10, 0, 0, 1 ,6,0);
