-- Fixing some default value
ALTER TABLE `race_map` CHANGE `race_template_entry` `race_template_entry` mediumint(5) unsigned NOT NULL default '0';
