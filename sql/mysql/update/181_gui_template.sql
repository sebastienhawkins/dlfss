-- add the user config gui
DELETE FROM `gui_template` WHERE `entry`=(6);
INSERT INTO `gui_template` (`entry`,`description`,`button_entry`,`text_button_entry`,`text`) VALUES 
(6, 'result guid', '71 72 73', 76, '^2Race result\r\n^7this made to show the\r\nscore for each player into this race.');
