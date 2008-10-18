-- add some motd config button
UPDATE `gui_template` SET `text_button_entry`=39,`button_entry`='22 23 24 25 26 27 28 29 37 38', 
`text`= '^8Config Help\r\n^7Title ^3 will enable or disable the feature\r\n^3Green ^2button ^3will trigger a action for this feature.'
WHERE `entry`=2;
