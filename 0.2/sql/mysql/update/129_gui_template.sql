-- add the user config gui
INSERT INTO `gui_template` (`entry`,`description`,`button_entry`,`text_button_entry`,`text`) VALUES 
(2, 'user config', '22 23 24 25 26 27 28 29', 0, '');

-- update the motd gui to add a new config button.
UPDATE `gui_template` SET `button_entry`='2 3 5 30' WHERE `entry`=1;
