-- add new extended button
ALTER TABLE `gui_template` ADD COLUMN `button_entry_ext` VARCHAR(255) NOT NULL DEFAULT '' AFTER `button_entry`;

-- result display fix
UPDATE `gui_template` SET `button_entry`='78 71 72 73',`button_entry_ext`='74 75' WHERE `entry`=6;

-- rank display fix
UPDATE `gui_template` SET `button_entry`='47 48 57 58 59 60 61 62 63 64',`button_entry_ext`='50 51 52 53 54 55 56 65 66 67' WHERE `entry`=5;
