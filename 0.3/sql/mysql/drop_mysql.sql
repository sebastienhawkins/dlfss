REVOKE ALL PRIVILEGES ON * . * FROM 'dlfss'@'localhost';

REVOKE ALL PRIVILEGES ON `drive_lfss` . * FROM 'dlfss'@'localhost';

REVOKE GRANT OPTION ON `drive_lfss` . * FROM 'dlfss'@'localhost';

DELETE FROM `user` WHERE CONVERT( User USING utf8 ) = CONVERT( 'dlfss' USING utf8 ) AND CONVERT( Host USING utf8 ) = CONVERT( 'localhost' USING utf8 ) ;

DROP DATABASE IF EXISTS `drive_lfss` ;
