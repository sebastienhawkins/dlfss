-- Fixing some default value
ALTER TABLE `button_template` CHANGE `description` `description` varchar(255) NOT NULL default '';
ALTER TABLE `button_template` CHANGE `max_input_char` `max_input_char` tinyint(3) unsigned NOT NULL default '0';
ALTER TABLE `button_template` CHANGE `left` `left` tinyint(3) unsigned NOT NULL default '0';
ALTER TABLE `button_template` CHANGE `top` `top` tinyint(3) unsigned NOT NULL default '0';
ALTER TABLE `button_template` CHANGE `width` `width` tinyint(3) unsigned NOT NULL default '0';
ALTER TABLE `button_template` CHANGE `height` `height` tinyint(3) unsigned NOT NULL default '0';
ALTER TABLE `button_template` CHANGE `text` `text` varchar(240) NOT NULL default '';
