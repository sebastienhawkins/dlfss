-- add button to cancel bad driving warning

DELETE FROM `button_template` WHERE `entry` IN(71,72,73,74,75,76);
INSERT INTO `button_template` 
(`entry`,`description`,`style_mask`,`is_allways_visible`,`max_input_char`,`left`,`top`,`width`,`height`,`text`,`caption_text`) values 
(71, 'result title and close', 40, 0, 0, 75, 0, 50, 13, '^3Result',''),
(72, 'precollision text close', 0, 0, 0, 115, 8, 8, 5, '^2CLOSE',''),
(73, 'result title', 16, 0, 0, 1, 31, 75, 7, '^7A^3leajecta ^7Race Result',''),
(74, 'result name display', 96, 0, 0, 1, 38, 60, 5, 'greenseed',''),
(75, 'result score display', 160, 0, 0, 62, 38, 14, 5, '^224^7pt',''),
(76, 'result help text', 32, 0, 0, 80, 76, 25, 5, '','');

