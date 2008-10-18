-- add button for user config gui
DELETE FROM `button_template` WHERE `entry` IN(23,24,37,38,39);
INSERT INTO `button_template` 
(`entry`,`description`,`style_mask`,`is_allways_visible`,`max_input_char`,`left`,`top`,`width`,`height`,`text`,`caption_text`) values 
(23, 'config title and close', 40, 0, 0, 75, 0, 50, 13, '^3User Config', ''), 
(24, 'config text close', 0, 0, 0, 115, 8, 8, 5, '^2 Close', ''),
(37, 'config drift bg', 32, 0, 0, 25, 30, 22, 24, '', ''), 
(38, 'config drift title', 8, 0, 0, 26, 31, 20, 7, '^7Drift Score', ''),
(39, 'config help text', 32, 0, 0, 100, 150, 75, 7, '', '');
