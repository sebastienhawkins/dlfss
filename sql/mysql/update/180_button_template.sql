-- add button to cancel bad driving warning

DELETE FROM `button_template` WHERE `entry` IN(68,69,70);
INSERT INTO `button_template` 
(`entry`,`description`,`style_mask`,`is_allways_visible`,`max_input_char`,`left`,`top`,`width`,`height`,`text`,`caption_text`) values 
(68, 'precollision cancel button1/3', 40, 0, 0, 1, 140, 56, 10, '^7Click HERE To ^1CANCEL',''),
(69, 'precollision cancel button2/3', 40, 0, 0, 1, 150, 56, 10, '^7Bad driving for driver',''),
(70, 'precollision cancel button3/3', 40, 0, 0, 1, 160, 56, 10, '','');
