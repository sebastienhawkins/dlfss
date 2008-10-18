ALTER TABLE `driver` ADD COLUMN `warning_driving_count` int(8) unsigned NOT NULL DEFAULT '0' AFTER  `config_data`;
ALTER TABLE `driver` ADD COLUMN `warning_chat_count` int(8) unsigned NOT NULL DEFAULT '0' AFTER  `warning_driving_count`;
