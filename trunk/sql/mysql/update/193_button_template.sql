-- add green racing flag button
DELETE FROM `button_template` WHERE `entry` IN(79,80,81,82,83,84,85);
INSERT INTO `button_template` 
(`entry`,`description`,`style_mask`,`is_allways_visible`,`max_input_char`,`left`,`top`,`width`,`height`,`text`,`caption_text`) values 
(79, 'Race flag text', 0, 0, 0, 138, 8, 10, 4, '^0Race Flag', ''),
(80, 'flag green square', 0, 0, 0, 133, 7, 15, 25, '^2', ''),
(81, 'flag green square', 0, 0, 0, 134, 7, 15, 25, '^2', ''),
(82, 'flag green square', 0, 0, 0, 135, 7, 15, 25, '^2', ''),
(83, 'flag green square', 0, 0, 0, 136, 7, 15, 25, '^2', ''),
(84, 'flag green square', 0, 0, 0, 137, 7, 15, 25, '^2', ''),
(85, 'flag green square', 0, 0, 0, 138, 7, 15, 25, '^2', '');

-- add pit closed racing flag button
DELETE FROM `button_template` WHERE `entry` IN(86,87,88,89,90,91,92,93);
INSERT INTO `button_template` 
(`entry`,`description`,`style_mask`,`is_allways_visible`,`max_input_char`,`left`,`top`,`width`,`height`,`text`,`caption_text`) values 
(86, 'flag red square', 0, 0, 0, 133, 7, 15, 25, '^1', ''),
(87, 'flag red square', 0, 0, 0, 134, 7, 15, 25, '^1', ''),
(88, 'flag red square', 0, 0, 0, 135, 7, 15, 25, '^1', ''),
(89, 'flag red square', 0, 0, 0, 136, 7, 15, 25, '^1', ''),
(90, 'flag red square', 0, 0, 0, 137, 7, 15, 25, '^1', ''),
(91, 'flag red square', 0, 0, 0, 138, 7, 15, 25, '^1', ''),
(92, 'pit close flag >', 64, 0, 0, 134, 3, 14, 33, '^3>', ''),
(93, 'pit close flag <', 128, 0, 0, 138, 3, 14, 33, '^3<', '');

-- add yellow square
DELETE FROM `button_template` WHERE `entry` IN(94,95,96,97,98,99);
INSERT INTO `button_template` 
(`entry`,`description`,`style_mask`,`is_allways_visible`,`max_input_char`,`left`,`top`,`width`,`height`,`text`,`caption_text`) values 
(94, 'flag yellow square', 0, 0, 0, 133, 7, 15, 25, '^3', ''),
(95, 'flag yellow square', 0, 0, 0, 134, 7, 15, 25, '^3', ''),
(96, 'flag yellow square', 0, 0, 0, 135, 7, 15, 25, '^3', ''),
(97, 'flag yellow square', 0, 0, 0, 136, 7, 15, 25, '^3', ''),
(98, 'flag yellow square', 0, 0, 0, 137, 7, 15, 25, '^3', ''),
(99, 'flag yellow square', 0, 0, 0, 138, 7, 15, 25, '^3', '');

-- Add SC caution for global Yellow
DELETE FROM `button_template` WHERE `entry` IN(100);
INSERT INTO `button_template` 
(`entry`,`description`,`style_mask`,`is_allways_visible`,`max_input_char`,`left`,`top`,`width`,`height`,`text`,`caption_text`) values 
(100, 'yellow red square', 0, 0, 0, 137, 12, 12, 15, '^0SC', '');

-- add blue square
DELETE FROM `button_template` WHERE `entry` IN(101,102,103,104,105,106);
INSERT INTO `button_template` 
(`entry`,`description`,`style_mask`,`is_allways_visible`,`max_input_char`,`left`,`top`,`width`,`height`,`text`,`caption_text`) values 
(101, 'flag blue square', 0, 0, 0, 133, 7, 15, 25, '^4', ''),
(102, 'flag blue square', 0, 0, 0, 134, 7, 15, 25, '^4', ''),
(103, 'flag blue square', 0, 0, 0, 135, 7, 15, 25, '^4', ''),
(104, 'flag blue square', 0, 0, 0, 136, 7, 15, 25, '^4', ''),
(105, 'flag blue square', 0, 0, 0, 137, 7, 15, 25, '^4', ''),
(106, 'flag blue square', 0, 0, 0, 138, 7, 15, 25, '^4', '');

-- add black square
DELETE FROM `button_template` WHERE `entry` IN(107,108,109,110,111,112);
INSERT INTO `button_template` 
(`entry`,`description`,`style_mask`,`is_allways_visible`,`max_input_char`,`left`,`top`,`width`,`height`,`text`,`caption_text`) values 
(107, 'flag black square', 0, 0, 0, 133, 7, 15, 25, '^0', ''),
(108, 'flag black square', 0, 0, 0, 134, 7, 15, 25, '^0', ''),
(109, 'flag black square', 0, 0, 0, 135, 7, 15, 25, '^0', ''),
(110, 'flag black square', 0, 0, 0, 136, 7, 15, 25, '^0', ''),
(111, 'flag black square', 0, 0, 0, 137, 7, 15, 25, '^0', ''),
(112, 'flag black square', 0, 0, 0, 138, 7, 15, 25, '^0', '');

-- Add Car probleme flag
DELETE FROM `button_template` WHERE `entry` IN(119);
INSERT INTO `button_template` 
(`entry`,`description`,`style_mask`,`is_allways_visible`,`max_input_char`,`left`,`top`,`width`,`height`,`text`,`caption_text`) values 
(119, 'flag car probleme dot', 0, 0, 0, 137, 8, 12, 22, '^3•', '');

-- Add no longer scored
DELETE FROM `button_template` WHERE `entry` IN(120,121);
INSERT INTO `button_template` 
(`entry`,`description`,`style_mask`,`is_allways_visible`,`max_input_char`,`left`,`top`,`width`,`height`,`text`,`caption_text`) values 
(120, 'pit close flag >', 64, 0, 0, 134, 3, 14, 33, '^7>', ''),
(121, 'pit close flag <', 128, 0, 0, 138, 3, 14, 33, '^7<', '');

-- Add end race flag
DELETE FROM `button_template` WHERE `entry` IN(122,123,124,125,126,127,128,129,130,131,132);
INSERT INTO `button_template` 
(`entry`,`description`,`style_mask`,`is_allways_visible`,`max_input_char`,`left`,`top`,`width`,`height`,`text`,`caption_text`) values 
(122, 'end race flag', 0, 0, 0, 136, 1, 7, 26, '^7-', ''),
(123, 'end race flag', 0, 0, 0, 136, 7, 7, 26, '^7-', ''),
(124, 'end race flag', 0, 0, 0, 138, 4, 7, 26, '^7-', ''),
(125, 'end race flag', 0, 0, 0, 138, 10, 7, 26, '^7-', ''),
(126, 'end race flag', 0, 0, 0, 140, 1, 7, 26, '^7-', ''),
(127, 'end race flag', 0, 0, 0, 140, 7, 7, 26, '^7-', ''),
(128, 'end race flag', 0, 0, 0, 142, 4, 7, 26, '^7-', ''),
(129, 'end race flag', 0, 0, 0, 142, 10, 7, 26, '^7-', ''),
(130, 'end race flag', 0, 0, 0, 144, 1, 7, 26, '^7-', ''),
(131, 'end race flag', 0, 0, 0, 144, 7, 7, 26, '^7-', ''),
(132, 'end race flag', 0, 0, 0, 139, 7, 15, 25, '^0', '');

-- add white square
DELETE FROM `button_template` WHERE `entry` IN(113,114,115,116,117,118);
INSERT INTO `button_template` 
(`entry`,`description`,`style_mask`,`is_allways_visible`,`max_input_char`,`left`,`top`,`width`,`height`,`text`,`caption_text`) values 
(113, 'flag black square', 0, 0, 0, 133, 7, 15, 25, '^7', ''),
(114, 'flag black square', 0, 0, 0, 134, 7, 15, 25, '^7', ''),
(115, 'flag black square', 0, 0, 0, 135, 7, 15, 25, '^7', ''),
(116, 'flag black square', 0, 0, 0, 136, 7, 15, 25, '^7', ''),
(117, 'flag black square', 0, 0, 0, 137, 7, 15, 25, '^7', ''),
(118, 'flag black square', 0, 0, 0, 138, 7, 15, 25, '^7', '');





/*
0139 = ‹
0149 = •
0155 = ›
0157 = 
#define ISB_C1			1		// you can choose a standard
#define ISB_C2			2		// interface colour using
#define ISB_C4			4		// these 3 lowest bits - see below
#define ISB_CLICK		8		// click this button to send IS_BTC
#define ISB_LIGHT		16		// light button
#define ISB_DARK		32		// dark button
#define ISB_LEFT		64		// align text to left
#define ISB_RIGHT		128		// align text to right
*/