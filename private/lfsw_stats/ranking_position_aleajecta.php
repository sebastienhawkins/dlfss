<?php

$link = mysql_connect('', 'root', 'dexxa');
if (!$link) {die(mysql_error());}
if (!mysql_select_db('drive_lfss')){die(mysql_error());}

$result = mysql_query("SELECT `name_prefix` FROM `car_template` ORDER BY `entry`",$link);
if (!$result) {die(mysql_error());}
$carPrefixs = array();
while ($row = mysql_fetch_array($result)) 
	array_push($carPrefixs,$row[0]);

$result = mysql_query("SELECT `name_prefix` FROM `track_template` ORDER BY `entry`",$link);
if (!$result) {die(mysql_error());}
$trackNames = array();
while ($row = mysql_fetch_array($result)) 
	array_push($trackNames,$row[0]);

$result = mysql_query("SELECT DISTINCT `licence_name` FROM `driver`",$link);
if (!$result) {die(mysql_error());}
$licenceNames = array();
while ($row = mysql_fetch_array($result)) 
	array_push($licenceNames,$row[0]);

foreach($licenceNames as $licenceName)
{
	echo "$licenceName\n";
	foreach($trackNames as $trackName)
	{
		if(strstr($trackName,"AU") ) //later do this for Drag rank...
			continue;
			
		foreach($carPrefixs as $carPrefix)
		{



			$query = "SELECT `total_rank` As `myTotal`,
					(SELECT COUNT(`total_rank`) FROM `drive_lfss`.`stats_rank_driver` WHERE `track_prefix`='$trackName' AND `car_prefix`='$carPrefix' AND `total_rank` > `myTotal`) As Position 
					FROM `drive_lfss`.`stats_rank_driver` WHERE `track_prefix`='$trackName' AND `car_prefix`='$carPrefix' AND `licence_name`LIKE'$licenceName'";
			$result = mysql_query($query,$link);
			if (!$result) {die(mysql_error());}
			if($row = mysql_fetch_array($result))
			{
				$changeMask = 0;
				$result = mysql_query("SELECT `position` FROM `drive_lfss`.`stats_rank_driver` WHERE `licence_name`LIKE'$licenceName' AND `track_prefix`='$trackName' AND `car_prefix`='$carPrefix'",$link);
				if ($row2 = mysql_fetch_array($result)) 
				{
					if($row2[0] > $row[1])
						$changeMask += 2048;
					else if($row2[0] < $row[1])
						$changeMask += 1024;
				}
				mysql_query("UPDATE `drive_lfss`.`stats_rank_driver` SET `position`='".($row[1]+1)."', `change_mask`=`change_mask`|$changeMask  WHERE `licence_name`LIKE'$licenceName' AND `track_prefix`='$trackName' AND `car_prefix`LIKE'$carPrefix'",$link);
			}
		}
	}
}

mysql_close($link);
?>