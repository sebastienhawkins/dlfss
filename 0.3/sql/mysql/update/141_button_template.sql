-- add new control button for the Gui Text.
DELETE FROM `button_template` WHERE `entry` IN(44,45,46);
INSERT INTO `button_template` 
(`entry`,`description`,`style_mask`,`is_allways_visible`,`max_input_char`,`left`,`top`,`width`,`height`,`text`,`caption_text`) values 
(44, 'text line', 32, 0, 0, 1, 45, 110, 6, '', ''), 
(45, 'text title and close', 40, 0, 0, 30, 32, 50, 12, '^3Text Display', ''),
(46, 'text close button', 0, 0, 0, 71, 39, 8, 4, '^2close', '');

-- Update message Middle and Top
UPDATE `button_template` SET `style_mask`=0,`left`=25,`width`=150,`height`=12 WHERE `entry` IN(6,7);
UPDATE `button_template` SET `top`=52 WHERE `entry` IN(7);