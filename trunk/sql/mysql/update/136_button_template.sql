-- add button for user config gui
DELETE FROM `button_template` WHERE `entry` IN(40,41,42,43,44);
INSERT INTO `button_template` 
(`entry`,`description`,`style_mask`,`is_allways_visible`,`max_input_char`,`left`,`top`,`width`,`height`,`text`,`caption_text`) values 
(40, 'config timediff bg', 32, 0, 0, 49, 30, 22, 24, '', ''), 
(41, 'config timediff title', 8, 0, 0, 50, 31, 20, 7, '^7Time Diff', ''),
(42, 'config timediff pb->lap', 8, 0, 0, 50, 39, 20, 6, '^7PB vs Lap', ''),
(43, 'config timediff pb->split', 8, 0, 0, 50, 46, 20, 6, '^7PB vs Split', '');

-- removed test value, that make mouse allways appear.
UPDATE `button_template` SET `style_mask` =80 WHERE `entry` IN(18,19,20,21);
UPDATE `button_template` SET `style_mask` =96 WHERE `entry` =17;

-- Config help text little higher
UPDATE `button_template` SET `top` =120, `left`=70 WHERE `entry` =39;

-- put button lower for MOTD and Help
UPDATE `button_template` SET `top`=146 WHERE `entry` IN(30,36,5,35,34);
