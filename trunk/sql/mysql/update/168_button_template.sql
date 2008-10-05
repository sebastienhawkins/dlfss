-- add button for gui Rank

DELETE FROM `button_template` WHERE `entry` IN(59,65,66,67);
INSERT INTO `button_template` 
(`entry`,`description`,`style_mask`,`is_allways_visible`,`max_input_char`,`left`,`top`,`width`,`height`,`text`,`caption_text`) values 
(65, 'rank search button track', 40, 0, 4, 1, 151, 26, 7, '^2Track','Track Prefix'),
(66, 'rank search button car', 40, 0, 3, 27, 151, 26, 7, '^2Car','Car Prefix'),
(67, 'rank search button add', 40, 0, 32, 53, 151, 26, 7, '^2Add','Licence Name'),
(59, 'rank top10 button', 40, 0, 0, 1, 158, 26, 10, '^2Top20','');
