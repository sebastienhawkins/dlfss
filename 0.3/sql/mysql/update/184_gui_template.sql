-- add some motd config button
UPDATE `gui_template` SET `button_entry`='78 71 72 73 74 75', `text`='' WHERE `entry`=6;

UPDATE `gui_template` SET `text`='^7A^3leajecta ^7is powered by Drive_LFSS 0.3 Alpha
^7
^7This is a alpha stage of the developement
^7there is a lot more to come, please be patien.
^7
^7List Of Commands:
^2!help^7, ^2!config^7, ^2!rank^7, ^2!result^7, ^2!status^7
^5!kick^7, ^5!exit^7, ^5!reload
^7
^7Did you know "SHIFT-i" will reset button and make Config screen appear.
^7
^5Greenseed^7.' WHERE `entry`=3;
