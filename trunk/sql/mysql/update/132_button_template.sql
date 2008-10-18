-- add button for user config gui

DELETE FROM `button_template` WHERE `entry` IN(3,5,30,31,32,33,34,35,36);
INSERT INTO `button_template` 
(`entry`,`description`,`style_mask`,`is_allways_visible`,`max_input_char`,`left`,`top`,`width`,`height`,`text`,`caption_text`) values 
(31, 'help bg', 96, 1, 0, 0, 0, 200, 200, '',''),
(32, 'help upper', 144, 1, 0, 45, 60, 110, 7, '^8Help ^7A^3leajecta',''),
(33, 'help text', 96, 1, 0, 50, 67, 100, 6, '^7',''),
(34, 'help button drive', 24, 1, 0, 51, 125, 48, 12, '^2Drive',''),
(35, 'help button config', 24, 1, 0, 102, 125, 48, 12, '^2Config',''),
(5, 'motd button drive', 24, 1, 0, 46, 125, 35, 12, '^2Drive', ''), 
(36, 'motd button help', 24, 1, 0, 83, 125, 35, 12, '^2Help',''),
(30, 'motd button config', 24, 1, 0, 120, 125, 35, 12, '^2Config', ''),
(3, 'motd upper', 144, 1, 0, 45, 60, 110, 7, '^8Motd ^7A^3leajecta', '');
