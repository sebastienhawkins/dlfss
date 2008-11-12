-- TaskBar
DELETE FROM `button_template` WHERE `entry` IN(160,161,162,163,164,165);
INSERT INTO `button_template` 
(`entry`,`description`,`style_mask`,`is_allways_visible`,`max_input_char`,`left`,`top`,`width`,`height`,`text`,`caption_text`) values 
(160, 'task bar bg', 16, 0, 0, 69, 0, 91, 7, '',''),
(161, 'task bar menu button', 40, 0, 0, 71, 1, 10, 5, '^2Menu',''),
(162, 'task bar exit button', 40, 0, 0, 82, 1, 10, 5, '^2Exit',''),
(163, 'task bar current button', 0, 0, 0, 97, 0, 25, 7, '',''),
(164, 'task bar version', 0, 0, 0, 140, 0, 18, 6, '^7D^3rive_LFSS^7 0.4a',''),
(165, 'task bar real time', 0, 0, 0, 127, 0, 12, 6, '','');

-- NodePosition
DELETE FROM `button_template` WHERE `entry` IN(166,167,168,169,170,171);
INSERT INTO `button_template` 
(`entry`,`description`,`style_mask`,`is_allways_visible`,`max_input_char`,`left`,`top`,`width`,`height`,`text`,`caption_text`) values 
(166, 'node position border', 32, 0, 0, 183, 120, 16, 37, '',''),
(167, 'node position bg', 16, 0, 0, 184, 121, 14, 35, '',''),
(168, 'node position car', 0, 0, 0, 188, 133, 5, 10, '^0^EÈ',''),
(169, 'node tracjectory to track', 16, 0, 0,85, 194, 30, 2, '',''),
(170, 'node orientation to track', 16, 0, 0,85, 196, 30, 2, '',''),
(171, 'node pos to track side', 16, 0, 0,85, 198, 30, 2, '','');

-- MyStats
DELETE FROM `button_template` WHERE `entry` IN(172,173,174,175,176,177,178,179,180,182);
INSERT INTO `button_template` 
(`entry`,`description`,`style_mask`,`is_allways_visible`,`max_input_char`,`left`,`top`,`width`,`height`,`text`,`caption_text`) values 
(172, 'mystats bg'							, 16, 0, 0, 1, 31,  129, 147, '',''),
(173, 'mystats title'						, 16, 0, 0, 3, 33,  125,  7, '^7M^3yStatus',''),
(174, 'mystats aleajecta title'				, 32, 0, 0, 3, 42,   46, 6, '^7A^3leajecta',''),
(175, 'mystats aleajecta bg'				, 32, 0, 0, 3, 42,   46, 69, '',''),
(176, 'mystats lfsw Title'					, 32, 0, 0, 50, 42,  78,  6, '^7L^5FS World',''),
(177, 'mystats lfsw bg'						, 32, 0, 0, 50, 42,  78,  69, '',''),
(178, 'mystats rank title top best'			, 32, 0, 0, 3, 113,  60,  6, '^7T^3op3 ^7High Rank',''),
(179, 'mystats rank top bg'					, 32, 0, 0, 3, 113,  60,  55, '',''),
(180, 'mystats rank title bottom best'		, 32, 0, 0, 3, 142,  60,  6, '^7T^3op3 ^7Lower Rank',''),
(182, 'mystats search button'				, 40, 0, 32, 113, 170,  15,  5, '^2Search','^2Enter ^7L^5FS ^2licence name');

-- mystats detail
DELETE FROM `button_template` WHERE `entry` IN(183,184,185,186,187,188,189,190,191,192,193,194,195,196,197,198,199,200,201,202,203,204,205,206,208,209,210,211,212,213,214,215,216);
INSERT INTO `button_template` 
(`entry`,`description`,`style_mask`,`is_allways_visible`,`max_input_char`,`left`,`top`,`width`,`height`,`text`,`caption_text`) values 
(183, 'mystats alea licenceName' 	, 64, 0, 0, 5, 49, 44, 5, '^2LicenceName^7 : ^1NA',''),
(184, 'mystats alea drivername'  	, 64, 0, 0, 5, 54, 44, 5, '^2DriverName^7 : ^1NA',''),
(185, 'mystats alea safepct' 		,64, 0, 0,5, 59, 44, 5, '^2SafePct^7 : ^1NA^7%',''),
(186, 'mystats alea chat' 			, 64, 0, 0,5, 64, 44, 5, '^2ChatWarn^7 : ^1NA',''),
(187, 'mystats alea WinOverall' 	, 64, 0, 0,5, 69, 44, 5, '^2WinOverall^7 : ^1NA',''),
(188, 'mystats alea RankOverall'  	, 64, 0, 0,5, 74, 44, 5, '^2RankOverall^7 : ^1NA',''),
(189, 'mystats alea DriftBest40s' 	, 64, 0, 0, 5, 79,  44, 5, '^2DriftBest^7 : ^1NA',''),
(190, 'mystats alea time raced' 	, 64, 0, 0, 5, 84,  44, 5, '^2RacingTime^7 : ^1NA',''),
(191, 'mystats alea time spec' 		, 64, 0, 0, 5, 89,  44, 5, '^2SpecTime^7 : ^1NA',''),
(192, 'mystats alea time garage' 	, 64, 0, 0, 5, 94,  44, 5, '^2GarageTime^7 : ^1NA',''),
(193, 'mystats alea time total' 	, 64, 0, 0, 5, 99,  44, 5, '^2TotalTime^7 : ^1NA',''),
(213, 'mystats separator lfsw' 		, 32, 0, 0, 89, 49,  1, 61, '',''),
(194, 'mystats alea win' 			, 64, 0, 0, 52, 49,  36, 5, '^2First Place ^7: ^1NA',''),
(195, 'mystats alea second' 		, 64, 0, 0, 52, 54,  36, 5, '^2Second Place ^7: ^1NA',''),
(196, 'mystats alea third'  		, 64, 0, 0, 52, 59,  36, 5 ,'^2Third Place ^7: ^1NA',''),
(197, 'mystats alea finish' 		, 64, 0, 0, 52, 64,  36, 5, '^2Finish ^7: ^1NA',''),

(214, 'mystats alea separator'  	, 16, 0, 0, 62, 71,  15, 1, '',''),

(198, 'mystats alea qual' 			, 64, 0, 0, 52, 74,  36, 5, '^2Qualify Entry ^7: ^1NA',''),
(203, 'mystats alea pole'  			, 64, 0, 0, 52, 79,  36, 5, '^2Pole Count ^7: ^1NA',''),

(215, 'mystats alea separator'  	, 16, 0, 0, 62, 86,  15, 1, '',''),

(204, 'mystats alea drags'  		, 64, 0, 0, 52, 89,  36, 5, '^2Drag Count ^7: ^1NA',''),
(205, 'mystats alea drag win' 		, 64, 0, 0, 52, 94,  36, 5, '^2Drag Win ^7: ^1NA',''),

(202, 'mystats alea distance' 		, 64, 0, 0, 92, 49,  36, 5, '^2Distance ^7: ^1NA ^7Km',''),
(199, 'mystats alea fuel' 			, 64, 0, 0, 92, 54,  36, 5, '^2Fuel Burn ^7: ^1NA ^7L',''),
(200, 'mystats alea laps' 			, 64, 0, 0, 92, 59,  36, 5, '^2Laps Count ^7: ^1NA',''),
(201, 'mystats alea hostjoin' 		, 64, 0, 0, 92, 64,  36, 5, '^2Join Count ^7: ^1NA',''),
(212, 'mystats alea team' 			, 64, 0, 0, 92, 69,  36, 5, '^2Team Name ^7: ^1NA',''),

(216, 'mystats alea separator'  	, 16, 0, 0, 102, 76,  15, 1, '',''),

(206, 'mystats alea online' 		, 64, 0, 0, 92, 79,  36, 5, '^2Is Online ^7: ^1NA',''),
(208, 'mystats alea server' 		, 64, 0, 0, 92, 84,  36, 5, '^2Last Server ^7: ^1NA',''),
(209, 'mystats alea lasttime' 		, 64, 0, 0, 92, 89,  36, 5, '^2Last Seen ^7: ^1NA',''),
(210, 'mystats alea track' 			, 64, 0, 0, 92, 94,  36, 5, '^2Last Track ^7: ^1NA',''),
(211, 'mystats alea car' 			, 64, 0, 0, 92, 99,  36, 5, '^2Last Car ^7: ^1NA','');

-- mystatus rank
DELETE FROM `button_template` WHERE `entry` IN(217,218,219,220,221,222,223,224,225,226,227,228,229,230,231,232,233,234,235,236,237,238,239,240,241,242,243,244,245,246,247,248,249,250,251,252,253,254,255,256,257,258,259,260,261,262,263,264,265,266);
INSERT INTO `button_template` 
(`entry`,`description`,`style_mask`,`is_allways_visible`,`max_input_char`,`left`,`top`,`width`,`height`,`text`,`caption_text`) values 
(217, 'mystats alea ranktop 1Track' 	, 16, 0, 0, 5, 125, 8, 5, '^1NA',''),
(218, 'mystats alea ranktop 1Car' 	, 16, 0, 0, 13, 125, 8, 5, '^1NA',''),
(219, 'mystats alea ranktop 1Best' 	, 16, 0, 0, 21, 125, 8, 5, '^1NA',''),
(220, 'mystats alea ranktop 1Avg' 	, 16, 0, 0, 29, 125, 8, 5, '^1NA',''),
(221, 'mystats alea ranktop 1Sta' 	, 16, 0, 0, 37, 125, 8, 5, '^1NA',''),
(222, 'mystats alea ranktop 1Win' 	, 16, 0, 0, 45, 125, 8, 5, '^1NA',''),
(223, 'mystats alea ranktop 1overall' , 16, 0, 0, 53, 125, 8, 5, '^1NA',''),
(224, 'mystats alea ranktop 2Track' 	, 16, 0, 0, 5, 130, 8, 5, '^1NA',''),
(225, 'mystats alea ranktop 2Car' 	, 16, 0, 0, 13, 130, 8, 5, '^1NA',''),
(226, 'mystats alea ranktop 2Best' 	, 16, 0, 0, 21, 130, 8, 5, '^1NA',''),
(227, 'mystats alea ranktop 2Avg' 	, 16, 0, 0, 29, 130, 8, 5, '^1NA',''),
(228, 'mystats alea ranktop 2Sta' 	, 16, 0, 0, 37, 130, 8, 5, '^1NA',''),
(229, 'mystats alea ranktop 2Win' 	, 16, 0, 0, 45, 130, 8, 5, '^1NA',''),
(230, 'mystats alea ranktop 2overall' , 16, 0, 0, 53, 130, 8, 5, '^1NA',''),
(231, 'mystats alea ranktop 3Track' 	, 16, 0, 0, 5, 135, 8, 5, '^1NA',''),
(232, 'mystats alea ranktop 3Car' 	, 16, 0, 0, 13, 135, 8, 5, '^1NA',''),
(233, 'mystats alea ranktop 3Best' 	, 16, 0, 0, 21, 135, 8, 5, '^1NA',''),
(234, 'mystats alea ranktop 3Avg' 	, 16, 0, 0, 29, 135, 8, 5, '^1NA',''),
(235, 'mystats alea ranktop 3Sta' 	, 16, 0, 0, 37, 135, 8, 5, '^1NA',''),
(236, 'mystats alea ranktop 3Win' 	, 16, 0, 0, 45, 135, 8, 5, '^1NA',''),
(237, 'mystats alea ranktop 3overall' , 16, 0, 0, 53, 135, 8, 5, '^1NA',''),
(238, 'mystats alea ranktop dtrack' 	, 16, 0, 0, 5, 120, 8, 5, '^8Track',''),
(239, 'mystats alea ranktop dCar' 	, 16, 0, 0, 13, 120, 8, 5, '^8Car',''),
(240, 'mystats alea ranktop dBest' 	, 16, 0, 0, 21, 120, 8, 5, '^8Best',''),
(241, 'mystats alea ranktop dAvg' 	, 16, 0, 0, 29, 120, 8, 5, '^8Avg',''),
(242, 'mystats alea ranktop dSta' 	, 16, 0, 0, 37, 120, 8, 5, '^8Sta',''),
(243, 'mystats alea ranktop dWin' 	, 16, 0, 0, 45, 120, 8, 5, '^8Win',''),
(244, 'mystats alea ranktop doverall' , 16, 0, 0, 53, 120, 8, 5, '^8Total',''),
(245, 'mystats alea rankbottom 1Track' 	, 16, 0, 0, 5, 150, 8, 5, '^1NA',''),
(246, 'mystats alea rankbottom 1Car' 	, 16, 0, 0, 13, 150, 8, 5, '^1NA',''),
(247, 'mystats alea rankbottom 1Best' 	, 16, 0, 0, 21, 150, 8, 5, '^1NA',''),
(248, 'mystats alea rankbottom 1Avg' 	, 16, 0, 0, 29, 150, 8, 5, '^1NA',''),
(249, 'mystats alea rankbottom 1Sta' 	, 16, 0, 0, 37, 150, 8, 5, '^1NA',''),
(250, 'mystats alea rankbottom 1Win' 	, 16, 0, 0, 45, 150, 8, 5, '^1NA',''),
(251, 'mystats alea rankbottom 1overall' , 16, 0, 0, 53, 150, 8, 5, '^1NA',''),
(252, 'mystats alea rankbottom 2Track' 	, 16, 0, 0, 5, 155, 8, 5, '^1NA',''),
(253, 'mystats alea rankbottom 2Car' 	, 16, 0, 0, 13, 155, 8, 5, '^1NA',''),
(254, 'mystats alea rankbottom 2Best' 	, 16, 0, 0, 21, 155, 8, 5, '^1NA',''),
(255, 'mystats alea rankbottom 2Avg' 	, 16, 0, 0, 29, 155, 8, 5, '^1NA',''),
(256, 'mystats alea rankbottom 2Sta' 	, 16, 0, 0, 37, 155, 8, 5, '^1NA',''),
(257, 'mystats alea rankbottom 2Win' 	, 16, 0, 0, 45, 155, 8, 5, '^1NA',''),
(258, 'mystats alea rankbottom 2overall' , 16, 0, 0, 53, 155, 8, 5, '^1NA',''),
(259, 'mystats alea rankbottom 3Track' 	, 16, 0, 0, 5, 160, 8, 5, '^1NA',''),
(260, 'mystats alea rankbottom 3Car' 	, 16, 0, 0, 13, 160, 8, 5, '^1NA',''),
(261, 'mystats alea rankbottom 3Best' 	, 16, 0, 0, 21, 160, 8, 5, '^1NA',''),
(262, 'mystats alea rankbottom 3Avg' 	, 16, 0, 0, 29, 160, 8, 5, '^1NA',''),
(263, 'mystats alea rankbottom 3Sta' 	, 16, 0, 0, 37, 160, 8, 5, '^1NA',''),
(264, 'mystats alea rankbottom 3Win' 	, 16, 0, 0, 45, 160, 8, 5, '^1NA',''),
(265, 'mystats alea rankbottom 3overall' , 16, 0, 0, 53, 160, 8, 5, '^1NA',''),
(266, 'waiting data',  64, 0, 0, 86, 34,  50,  5, '^7Retriving L^5FSW^7 Data ^4|','');
/*

|
__ï
--
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
^L      Latin1                iso8859_1
^G      Greek                 iso8859_7
^C      Cyrillic              iso8859_5
^J      Japanese              ??? 
^E      Eastern Europe        iso8859_2
^T      Turkish               iso8859_9
^B      Baltic                iso8859_4
^H      Traditional Chinese   ???   
^S      Simpified Chinese     ???
^K      Korean                ???
*/
