-- Add the new Gui Text
DELETE FROM `gui_template` WHERE `entry` IN(4);
INSERT INTO `gui_template` (`entry`,`description`,`button_entry`,`text_button_entry`,`text`) VALUES 
(4, 'text', '45 46', 44, '');

-- Update the text for the Help gui
UPDATE `gui_template` SET`text`= 
'^7A^3leajecta ^7is powered by Drive_LFSS 0.2 Alpha
^7
^7This is a alpha stage of the developement
^7there is a lot more to come, please be patien.
^7
^7List Of Commands:
^2!help^7, ^2!config^7,^2!status^7, ^5!kick^7, ^5!exit^7, ^5!reload
^7
^7Did you know "SHIFT-i" will reset button and make Config screen appear.
^7
^5Greenseed^7.'
WHERE `entry` = 3;