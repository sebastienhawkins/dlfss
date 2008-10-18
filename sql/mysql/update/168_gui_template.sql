-- Update the GUI rank
DELETE FROM `gui_template` WHERE `entry` IN(5);
INSERT INTO `gui_template` (`entry`,`description`,`button_entry`,`text_button_entry`,`text`) VALUES 
(5, 'rank', '47 48 57 58 59 60 61 62 63 64', 49, '^7Top20 ^2is the 20 Best Driver for this Track/Car.\r\n^7Search ^2search for a Track/Car/Driver rank.\r\n^7Current ^2show rank for all online driver by Track/Car.');
