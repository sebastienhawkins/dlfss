-- Fixing some default value
ALTER TABLE `gui_template` CHANGE `description` `description` varchar(255) NOT NULL default '';
ALTER TABLE `gui_template` CHANGE `button_entry` `button_entry` varchar(255) NOT NULL default '' COMMENT 'space separated value';
ALTER TABLE `gui_template` CHANGE `text_button_entry` `text_button_entry` mediumint(5) unsigned NOT NULL default '0';
ALTER TABLE `gui_template` CHANGE `text` `text` blob NOT NULL;
