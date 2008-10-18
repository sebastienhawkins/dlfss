-- add some motd config button
UPDATE `gui_template` SET `button_entry`='22 23 24 25 26 27 28 29 37 38 40 41 42 43' WHERE `entry`=2;

-- Update Help Text
UPDATE `gui_template` SET `text`=
'^7A^3leajecta ^7is powered by Drive_LFSS 0.2 Alpha\r\n^7\r\n^7This is a alpha stage of the developement\r\n^7there is a lot more to come, please be patien.\r\n^7\r\n^7List Of Commands:\r\n^2!help^7, ^2!config^7, ^5!kick^7, ^5!exit^7, ^5!reload\r\n^7\r\n^7Did you know "SHIFT-i" will reset button and make Config screen appear.\r\n^7\r\n^5Greenseed^7.'
WHERE `entry`=3;