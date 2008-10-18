-- update the help gui text.
UPDATE `gui_template` SET `text`='^7A^3leajecta ^7is powered by Drive_LFSS 0.4 Alpha
^7
^7This is a alpha stage of the developement
^7there is a lot more to come, please be patien.
^7
^7List Of Commands:
^2!help^7, ^2!config^7, ^2!rank^7, ^2!result^7, ^2!status^7
^5!say, !kick^7, ^5!exit^7, ^5!reload
^7
^7Did you know "SHIFT-i" will reset button and make Config screen appear.
^7
^5Greenseed^7.'
WHERE `entry`=3;

-- add green flag
DELETE FROM `gui_template` WHERE `entry`IN(7,8,9,10,11,12,13,14,15,16,17);
INSERT INTO `gui_template` (`entry`,`description`,`button_entry`,`button_entry_ext`,`text_button_entry`,`text`) VALUES 
(7, 'green flag', '79 80 81 82 83 84 85','', 0, ''),
(8, 'pit close flag', '79 86 87 88 89 90 91 92 93','', 0, ''),
(9, 'yellow flag local', '79 94 95 96 97 98 99','', 0, ''),
(10, 'yellow flag global', '79 94 95 96 97 98 99 100','', 0, ''),
(11, 'red flag stop race', '79 86 87 88 89 90 91','', 0, ''),
(12, 'black flag penality', '79 107 108 109 110 111 112','', 0, ''),
(13, 'blue flag slow car', '79 101 102 103 104 105 106','', 0, ''),
(14, 'white flag final lap', '79 113 114 115 116 117 118','', 0, ''),
(15, 'black flag car probleme', '79 107 108 109 110 111 112 119','', 0, ''),
(16, 'black no longer scored', '79 107 108 109 110 111 112 120 121','', 0, ''),
(17, 'end race flag', '79 107 108 109 110 111 112 132 122 123 124 125 126 127 128 129 130 131','', 0, '');