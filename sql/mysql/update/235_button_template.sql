DELETE FROM `gui_template` WHERE `entry`IN(18);
INSERT INTO `gui_template` (`entry`,`description`,`button_entry`,`button_entry_ext`,`text_button_entry`,`text`) VALUES 
(18, 'menu', '133 134 135 136 137 138 139 140 141 142 143 144 145','', 0, '');

DELETE FROM `button_template` WHERE `entry` IN(133,134,135,136,137,138,139,140,141,142,143,144,145);
INSERT INTO `button_template` 
(`entry`,`description`,`style_mask`,`is_allways_visible`,`max_input_char`,`left`,`top`,`width`,`height`,`text`,`caption_text`) values 
(133, 'menu bg', 0, 0, 0, 0, 0, 200, 200, '',''),
(134, 'menu title and close', 40, 0, 0, 75, 0, 50, 13,'^2Close',''),
(135, 'menu title', 0, 0, 0, 113, 8, 10, 5,'^3 Menu',''),
(136, 'menu button config', 24, 0, 0, 5, 66, 20, 12, '^2Config', ''),
(137, 'menu button rank', 24, 0, 0, 26, 66, 20, 12, '^2Rank', ''),
(138, 'menu button scoreboard', 24, 0, 0, 47, 66, 20, 12, '^2Scoreboard', ''),
(139, 'menu button result', 24, 0, 0, 68, 66, 20, 12, '^2Result', ''),
(140, 'menu button mystats', 24, 0, 0, 89, 66, 20, 12, '^2MyStats', ''),
(142, 'menu button status', 24, 0, 0, 5, 85, 20, 12, '^2Status', ''),
(141, 'menu button manager', 24, 0, 0, 5, 104, 20, 12, '^5Manager', ''),
(143, 'menu button say', 24, 0, 0, 26, 104, 20, 12, '^5Say', ''),
(144, 'menu button reload', 24, 0, 0, 47, 104, 20, 12, '^5Reload', ''),
(145, 'menu button exit', 24, 0, 0, 68, 104, 20, 12, '^5Exit', '');

/*
0139 = ã
0149 = ï
0155 = õ
0157 = ù
#define ISB_C1			1		// you can choose a standard
#define ISB_C2			2		// interface colour using
#define ISB_C4			4		// these 3 lowest bits - see below
#define ISB_CLICK		8		// click this button to send IS_BTC
#define ISB_LIGHT		16		// light button
#define ISB_DARK		32		// dark button
#define ISB_LEFT		64		// align text to left
#define ISB_RIGHT		128		// align text to right
*/
