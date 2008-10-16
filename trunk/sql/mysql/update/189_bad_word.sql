-- Table "bad_word" DDL
DROP TABLE `bad_word` IF EXISTS;
CREATE TABLE `bad_word` (
  `word` varchar(16) character set latin1 collate latin1_general_ci NOT NULL default '',
  `mask` tinyint(1) unsigned NOT NULL default '0',
  PRIMARY KEY  (`word`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;

-- Value
INSERT IGNORE INTO bad_word
   (`word`, `mask`)
VALUES
   ('@sshole', 3);

INSERT IGNORE INTO bad_word
   (`word`, `mask`)
VALUES
   ('fuck.', 2);

INSERT IGNORE INTO bad_word
   (`word`, `mask`)
VALUES
   ('you', 1);

INSERT IGNORE INTO bad_word
   (`word`, `mask`)
VALUES
   ('idiot', 3);

INSERT IGNORE INTO bad_word
   (`word`, `mask`)
VALUES
   ('shit..', 2);

INSERT IGNORE INTO bad_word
   (`word`, `mask`)
VALUES
   ('piece', 1);

INSERT IGNORE INTO bad_word
   (`word`, `mask`)
VALUES
   ('mother', 1);

INSERT IGNORE INTO bad_word
   (`word`, `mask`)
VALUES
   ('jerk', 3);

INSERT IGNORE INTO bad_word
   (`word`, `mask`)
VALUES
   ('bastard', 3);

INSERT IGNORE INTO bad_word
   (`word`, `mask`)
VALUES
   ('nigger...', 3);

INSERT IGNORE INTO bad_word
   (`word`, `mask`)
VALUES
   ('fag', 3);

INSERT IGNORE INTO bad_word
   (`word`, `mask`)
VALUES
   ('chienne', 2);

INSERT IGNORE INTO bad_word
   (`word`, `mask`)
VALUES
   ('enfant', 1);

INSERT IGNORE INTO bad_word
   (`word`, `mask`)
VALUES
   ('chier..', 2);

INSERT IGNORE INTO bad_word
   (`word`, `mask`)
VALUES
   ('va', 1);

INSERT IGNORE INTO bad_word
   (`word`, `mask`)
VALUES
   ('vachier', 3);

INSERT IGNORE INTO bad_word
   (`word`, `mask`)
VALUES
   ('marde', 2);

INSERT IGNORE INTO bad_word
   (`word`, `mask`)
VALUES
   ('mange', 1);

INSERT IGNORE INTO bad_word
   (`word`, `mask`)
VALUES
   ('putain.', 3);

INSERT IGNORE INTO bad_word
   (`word`, `mask`)
VALUES
   ('enculer', 3);

INSERT IGNORE INTO bad_word
   (`word`, `mask`)
VALUES
   ('cul ', 2);

INSERT IGNORE INTO bad_word
   (`word`, `mask`)
VALUES
   ('trou', 1);

INSERT IGNORE INTO bad_word
   (`word`, `mask`)
VALUES
   ('bitch..', 3);

INSERT IGNORE INTO bad_word
   (`word`, `mask`)
VALUES
   ('morron', 3);

INSERT IGNORE INTO bad_word
   (`word`, `mask`)
VALUES
   ('pute.', 3);

INSERT IGNORE INTO bad_word
   (`word`, `mask`)
VALUES
   ('stupid', 3);

INSERT IGNORE INTO bad_word
   (`word`, `mask`)
VALUES
   ('fucker', 3);

INSERT IGNORE INTO bad_word
   (`word`, `mask`)
VALUES
   ('cock.', 3);

INSERT IGNORE INTO bad_word
   (`word`, `mask`)
VALUES
   ('slut.', 3);

INSERT IGNORE INTO bad_word
   (`word`, `mask`)
VALUES
   ('pussy', 2);

INSERT IGNORE INTO bad_word
   (`word`, `mask`)
VALUES
   ('lick', 1);

INSERT IGNORE INTO bad_word
   (`word`, `mask`)
VALUES
   ('retard', 2);

INSERT IGNORE INTO bad_word
   (`word`, `mask`)
VALUES
   ('fuckking', 3);

INSERT IGNORE INTO bad_word
   (`word`, `mask`)
VALUES
   ('fukka', 3);

INSERT IGNORE INTO bad_word
   (`word`, `mask`)
VALUES
   ('him', 1);

INSERT IGNORE INTO bad_word
   (`word`, `mask`)
VALUES
   ('suck.', 2);

INSERT IGNORE INTO bad_word
   (`word`, `mask`)
VALUES
   ('ass.', 2);

INSERT IGNORE INTO bad_word
   (`word`, `mask`)
VALUES
   ('nazi', 3);

INSERT IGNORE INTO bad_word
   (`word`, `mask`)
VALUES
   ('gay ', 2);

