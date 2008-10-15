-- add maxSpeed button for user config gui
DELETE FROM `button_template` WHERE `entry` IN(77);
INSERT INTO `button_template` 
(`entry`,`description`,`style_mask`,`is_allways_visible`,`max_input_char`,`left`,`top`,`width`,`height`,`text`,`caption_text`) values 
(77, 'config maxspeed title', 8, 0, 0, 26, 39, 20, 7, '^7Max Speed', '');
