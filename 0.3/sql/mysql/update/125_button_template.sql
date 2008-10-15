-- Update Middle and Top Button
UPDATE `button_template` SET `height` = 13, `width`=80, `left`=60 ,`style_mask`=32 WHERE `entry`IN(6,7);
UPDATE `button_template` SET `top`= 27 WHERE `entry`=6;
UPDATE `button_template` SET `top`= 50 WHERE `entry`=7;




