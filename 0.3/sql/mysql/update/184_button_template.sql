-- add maxSpeed button for user config gui
DELETE FROM `button_template` WHERE `entry` IN(78);
INSERT INTO `button_template` 
(`entry`,`description`,`style_mask`,`is_allways_visible`,`max_input_char`,`left`,`top`,`width`,`height`,`text`,`caption_text`) values 
(78, 'result bg', 16, 0, 0, 0, 30, 77, 121, '', '');

-- change result title
UPDATE `button_template`  SET `style_mask`=160 WHERE `entry`=73;
