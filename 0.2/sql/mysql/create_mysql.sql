GRANT USAGE ON * . * TO 'dlfss'@'localhost' IDENTIFIED BY 'dlfss' WITH MAX_QUERIES_PER_HOUR 0 MAX_CONNECTIONS_PER_HOUR 0 MAX_UPDATES_PER_HOUR 0 ;

CREATE DATABASE `drive_lfss` DEFAULT CHARACTER SET utf8 COLLATE utf8_general_ci;

GRANT ALL PRIVILEGES ON `dlfss` . * TO 'dlfss'@'localhost' WITH GRANT OPTION;