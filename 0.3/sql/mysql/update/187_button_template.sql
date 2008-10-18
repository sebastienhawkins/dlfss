-- change result title
UPDATE `button_template`  SET `height`=4,`width`=27,`top`=56 WHERE `entry`IN(73);
UPDATE `button_template`  SET `height`=3,`width`=20,`top`=59 WHERE `entry`IN(74);
UPDATE `button_template`  SET `height`=3,`width`=7,`top`=59,`left`=21 WHERE `entry`IN(75);
UPDATE `button_template`  SET `style_mask`=16 WHERE `entry`IN(78);

UPDATE `button_template`  SET `text`='^2CLOSE' WHERE `entry`IN(71);
UPDATE `button_template`  SET `text`='^3Result' WHERE `entry`IN(72);

UPDATE `button_template`  SET `text`='^2CLOSE' WHERE `entry`IN(62);
UPDATE `button_template`  SET `text`='^3Ranking' WHERE `entry`IN(63);

UPDATE `button_template`  SET `text`='^2CLOSE' WHERE `entry`IN(23);
UPDATE `button_template`  SET `text`='^3UserConfig',`left`=113,`width`=10 WHERE `entry`IN(24);
