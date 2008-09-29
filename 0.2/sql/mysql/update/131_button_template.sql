-- make the tittle clicable for the On/Off option
UPDATE `button_template` SET `style_mask` = 40 WHERE `entry`=23;
UPDATE `button_template` SET `style_mask` = 8 WHERE `entry`=26;
UPDATE `button_template` SET `left` = 115,`top`=8,`width`=8,`height`=5 WHERE `entry`=24;
