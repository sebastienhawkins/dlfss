-- add button for gui Rank

DELETE FROM `button_template` WHERE `entry` IN(47,48,49,50,51,52,53,54,55,56,57,58,59,60,61,62,63,64);
INSERT INTO `button_template` 
(`entry`,`description`,`style_mask`,`is_allways_visible`,`max_input_char`,`left`,`top`,`width`,`height`,`text`,`caption_text`) values 
(47, 'rank bg', 32, 0, 0, 0, 30, 81, 141, '',''),
(48, 'rank title', 144, 0, 0, 1, 31, 79, 5, '^7A^3leajecta Ranking',''),
(49, 'rank text help', 32, 0, 0, 82, 108, 75, 7, '',''),
(50, 'rank name'	, 16, 0, 0, 1, 42, 27, 5, '^8name',''),
(51, 'rank pb'		, 16, 0, 0, 28, 42, 8, 5, '^8best',''),
(52, 'rank average'	, 16, 0, 0, 36, 42, 8, 5, '^8avera',''),
(53, 'rank sta'		, 16, 0, 0, 44, 42, 8, 5, '^8stabi',''),
(54, 'rank win'		, 16, 0, 0, 52, 42, 8, 5, '^8win',''),
(55, 'rank total'	, 16, 0, 0, 60, 42, 9, 5, '^8total',''),
(56, 'rank position', 144, 0, 0, 69, 42, 11, 5, '^8position',''),
(57, 'rank last update', 64, 0, 0, 1, 31, 20, 5, '^0Last ^72008^2/^7Oct^2/^704',''),
(58, 'rank next update', 64, 0, 0, 24, 31, 20, 5, '^6Next ^72008^2/^7Oct^2/^706',''),
(59, 'rank top10 button', 40, 0, 0, 1, 158, 26, 10, '^2Top10',''),
(60, 'rank search button', 40, 0, 0, 27, 158, 26, 10, '^2Search',''),
(61, 'rank current button', 40, 0, 0, 53, 158, 26, 10, '^2Current',''),
(62, 'rank tittle and close', 40, 0, 0, 75, 0, 50, 13, '^3Ranking',''),
(63, 'rank text close', 0, 0, 0, 115, 8, 8, 5, '^2 Close',''),
(64, 'rank info', 16, 0, 0, 1, 37, 79, 5, '^2Car: ^7FBM, ^2Track:^7 BL1R, ^2Count: ^7154356','');
