-- add the user config gui
DELETE FROM `gui_template` WHERE `entry`=(3);
INSERT INTO `gui_template` (`entry`,`description`,`button_entry`,`text_button_entry`,`text`) VALUES 
(3, 'help', '31 32 33 34 35', 33, '^7A^3leajecta ^7is powered by Drive_LFSS 0.2 Alpha\r\n^7\r\n^7This is a alpha stage of the developement\r\n^7there is a lot more to come, please be patien.\r\n^7\r\n^7Your ^2Admin.\r\n^7\r\n^7List Of Commands:\r\n^2!help^7, ^2!config^7, ^5!kick^7, ^5!exit^7, ^5!reload');

-- add the motd help button
UPDATE `gui_template` SET `button_entry`='2 3 5 30 36' WHERE `entry`=1;
