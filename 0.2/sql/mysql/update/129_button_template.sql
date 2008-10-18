-- Add colum , for text caption on button requesting user input.
ALTER TABLE `button_template` ADD COLUMN `caption_text` varchar(240) NOT NULL default '' after `text`;

-- add button for user config gui
INSERT INTO `button_template` 
(`entry`,`description`,`style_mask`,`is_allways_visible`,`max_input_char`,`left`,`top`,`width`,`height`,`text`,`caption_text`) values 
(22, 'config bg', 32, 0, 0, 0, 0, 200, 200, '',''),
(23, 'config title', 32, 0, 0, 75, 1, 50, 13,'^3User Config',''),
(24, 'config button close', 0, 0, 0, 75, 7, 50, 13,'^2 Close',''),
(25, 'config acceleration bg', 32, 0, 0, 1, 30, 22, 24, '',''),
(26, 'config acceleration title', 0, 0, 0, 2, 31, 20, 7, '^7Acceleration',''),
(27, 'config acceleration start', 40, 0, 131, 2, 39, 9, 6, '^2start','^3Start Kmh Speed ex: 0'), 
(28, 'config acceleration end', 40, 0, 131, 13, 39, 9, 6, '^2 end ','^3End Kmh Speed ex: 100'),
(29, 'config acceleration current', 32, 0, 0, 2, 46, 20, 6, '^7','');
(30, 'motd button config', 26, 0, 0, 102, 125, 48, 13, '^3Config','');

-- update the motd "Drive" button
UPDATE `button_template` SET `left`=51, `width`=48 WHERE `entry`=5;
