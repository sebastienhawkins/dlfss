-- Change laps count for FernBay Gold and Green
UPDATE `race_template` SET `lap_count`=5 WHERE `entry`IN(22,29);
