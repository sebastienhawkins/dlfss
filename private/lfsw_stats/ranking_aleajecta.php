<?php 

//Collect Official World Record Time.
$wrt = file_get_contents ("http://www.lfsworld.net/pubstat/get_stat2.php?version=1.4&user=dddd&pass=dl66fs&action=wr");
$wrt = explode("\n",$wrt);
if(count($wrt) < 941)
{
	echo "WR Invalid\r\n";
	exit;	
}
$wr = array();
foreach($wrt as &$value)
{
	if($value == "") 
		continue;
	$value = explode(" ",$value);
	
	$trackt = LFSWTrackToTrackPrefix($value[1]);
	if( !isset($wr[$trackt])) 
		$wr[$trackt] = array();
	$wr[$trackt][$value[2]] = $value[6];
}

//Connect to DLFSS Database
$link = mysql_connect('192.168.77.101:33306', 'www', 'dexxa', true /*,MYSQL_CLIENT_COMPRESS*/);
if (!$link) {die(mysql_error());}
if (!mysql_select_db('drive_lfss')){die(mysql_error());}

//Collect CarPrefix
$result = mysql_query("SELECT `name_prefix` FROM `car_template` ORDER BY `entry`",$link);
if (!$result) {die(mysql_error());}
$carPrefixs = array();
while ($row = mysql_fetch_array($result)) 
	array_push($carPrefixs,$row[0]);
mysql_free_result($result);

//Collect TrackPrefix
$result = mysql_query("SELECT `name_prefix` FROM `track_template` ORDER BY `entry`",$link);
if (!$result) {die(mysql_error());}
$trackNames = array();
while ($row = mysql_fetch_array($result)) 
	array_push($trackNames,$row[0]);
mysql_free_result($result);

//Collect Driver Data
$result = mysql_query("SELECT guid,licence_name FROM `driver`",$link);
if (!$result) 
	die(mysql_error());
$drivers = array();
while ($row = mysql_fetch_assoc($result))
	array_push($drivers,$row);
mysql_free_result($result);
//Update Button Text For Last Rank Update
mysql_query("UPDATE `button_template` SET `width`=25,`text`='^0Last ^72009^2/^7Jan^2/^729' WHERE `entry`IN(57)",$link);

foreach($trackNames as $trackName)
{
	echo "TrackName: {$trackName}\n";
	
	if(strstr($trackName,"AU") ) //later do this for Drag rank...
		continue;

	foreach($carPrefixs as $carPrefix)
	{
		echo "CarName: {$carPrefix}\n";
		//if($carPrefix != "FBM") continue;
		
		if(!isset($wr[$trackName][$carPrefix]))
		{
			//echo "Bypass: $trackName, $carPrefix no WR Value\n";
			continue;
		}
		$bestEver = $wr[$trackName][$carPrefix];
		//echo "$trackName, $carPrefix BestEver: $bestEver \n";

		//Average
		$average = 0;
		$query = "SELECT AVG(`lap_time`) FROM `driver_lap`
		WHERE `driver_lap`.`guid_race`!=0 
		AND `driver_lap`.`track_prefix`='$trackName'
		AND `driver_lap`.`car_prefix`='$carPrefix'
		AND `driver_lap`.`lap_time` < ".($bestEver+($bestEver/5))."
		AND `driver_lap`.`lap_time` >= ".($bestEver-2000);
		$result = mysql_query($query ,$link);
		if (!$result) 
		{
			die(mysql_error());
		}
		if ($result)
		{
			$row = mysql_fetch_array($result);
			$average = ($row[0] == "" ? 0 : $row[0]);
		}
		mysql_free_result($result);
		//echo "Average: $average\n";
		foreach($drivers as $driver)
		{
			//if($driver['licence_name']!="greenseed")continue;
			//echo "Driver_Guid: {$driver['licence_name']}\n";
		
			$rank = 0;
			//Driver RaceCount
			/*$query = "SELECT COUNT(`guid_driver`)
			FROM `driver_lap`
			WHERE `driver_lap`.`guid_race`!=0 
			AND `driver_lap`.`guid_driver`={$driver['guid']}
			AND `driver_lap`.`track_prefix`='$trackName'
			AND `driver_lap`.`car_prefix`='$carPrefix'";

			$result = mysql_query($query ,$link);
			if (!$result) {die(mysql_error());}
			$row = mysql_fetch_array($result);
			//var_dump($row);
			$driverRaceCount = $row[0];
			//echo "RaceCount: $driverRaceCount\n";
			if($driverRaceCount < 1)
				continue;
			*/
			//Driver Best
			$query =   "SELECT `lap_time`
			FROM `driver_lap`
			WHERE `driver_lap`.`track_prefix`='$trackName'
			AND `driver_lap`.`guid_driver`={$driver['guid']}
			AND `driver_lap`.`car_prefix`='$carPrefix'
			AND `driver_lap`.`lap_time` >= ".($bestEver -2000)."
			ORDER BY `driver_lap`.`lap_time` LIMIT 1";

			$result = mysql_query($query ,$link);
			if (!$result) {die(mysql_error());}
			$row = mysql_fetch_array($result);
			//var_dump($row);
			$driverBest = $row[0];
			if($driverBest != "" && $driverBest != 0)
			{
				$driverBestS = ($bestEver / $driverBest * 9999 );
				$driverBestS = $driverBestS-(((9999)-$driverBestS)*2);
				$driverBestS = $driverBestS * 5000 / 9999;
				$driverBestS = (int)$driverBestS ;
			}
			else
			{
				$driverBestS = $driverBest = 0;
			}
			//echo "Best | Score : $driverBest | $driverBestS\n";
			mysql_free_result($result);
			//Driver Average
			$query = "SELECT AVG(`lap_time`),COUNT(`lap_time`)
			FROM `driver_lap`
			WHERE `driver_lap`.`guid_race`!=0 
			AND `driver_lap`.`guid_driver`={$driver['guid']}
			AND `driver_lap`.`track_prefix`='$trackName'
			AND `driver_lap`.`car_prefix`='$carPrefix'
			AND `driver_lap`.`lap_time` >= ".($bestEver-2000)."
			AND `driver_lap`.`lap_time` < ".($driverBest+($driverBest/5));
			
			$result = mysql_query($query ,$link);
			if (!$result) {die(mysql_error());}
			$row = mysql_fetch_array($result);
			//var_dump($row);
			$driverAverage = $row[0];
			if($driverAverage != "" && $driverAverage != 0)
			{
				$driverAverageS = ( $average/$driverAverage*9999);
				$driverAverageS = $driverAverageS-(((9999)-$driverAverageS)*2);
				$driverAverageS = $driverAverageS * 4999 / 9999;
				$driverAverageS = (int)$driverAverageS ;
				if($row[1]<10)
						$driverAverageS = (int)($driverAverageS/2);
	
				$driverStabilityS = ($driverBest / $driverAverage *9999);
				$driverStabilityS = $driverStabilityS-(((9999)-$driverStabilityS)*2);
				$driverStabilityS = $driverStabilityS * 4999 / 9999;
				$driverStabilityS = (int)$driverStabilityS;
				if($row[1]<10)
						$driverStabilityS = (int)($driverStabilityS/2);
				//echo "Average $average | Score: $driverAverage | $driverAverageS\n";
			}
			else
				$driverAverage = $driverAverageS = $driverStabilityS = 0;
			
			mysql_free_result($result);
			//Wins Scoring
			$driverWinS = 0;
			
			$query = "SELECT `race`.`finish_order`,`race`.`race_laps`,`driver_lap`.`total_time`
			FROM `driver_lap`,`race`
			WHERE `race`.`finish_order` != ''
			AND `race`.`track_prefix`='$trackName'
			AND `race`.`guid`=`driver_lap`.`guid_race`
			AND `driver_lap`.`guid_driver`={$driver['guid']}
			AND `driver_lap`.`car_prefix`='$carPrefix'
			AND `driver_lap`.`lap_completed`=`race`.`race_laps`";

			$result = mysql_query($query ,$link);
			if (!$result) {die(mysql_error());}
			
			$winK = 0;
			$averageLapTime = 0;
			while($row = mysql_fetch_array($result))
			{
				$datas = split(" ",$row[0]);
				$driverCount = count($datas);
				$averageLapTime = $row[2]/$row[1];
				$lapRatio = ($row[1]+20) / 2;
				$lapRatio = ($lapRatio * 20 / 100)-2;
				$winK = ($driverCount + ((($bestEver+3000)-$averageLapTime)/($bestEver+3000)*100) - 5)+$lapRatio;
//				- (($averageLapTime - $bestEver)/1000)) * (16-$driverCount)))/50;
				$itr = 0;
				foreach($datas as $itr => $value)
				{
					$itr++;
					
					if($value != "" && strstr($driver['guid'],$value))
					{
						$_driverWinS = $winK + $driverCount-$itr + 1;
						if($_driverWinS > 0)
								$driverWinS += $_driverWinS ;
						echo "$trackName | $carPrefix | $winK | $averageLapTime | $bestEver | $driverCount position: $itr | $_driverWinS | $driverWinS\n";
						break;
					}
				}
			}
			mysql_free_result($result);

			//Compute total
			
			$rank = (int)(($driverBestS + ($driverBestS/2)) + ($driverAverageS+($driverAverageS/5))/*+ $driverStabilityS*/ + $driverWinS);
			if($rank < 0)
				$rank = 0;
			
			$PB_LOW = 1;
			$PB_HIGH = 2;
			$AVG_LOW = 4;
			$AVG_HIGH = 8;
			$STA_LOW = 16;
			$STA_HIGH = 32;
			$WIN_LOW = 64;
			$WIN_HIGH = 128;
			$TOTAL_LOW = 256;
			$TOTAL_HIGH = 512;
			$POSITION_LOW = 1024;
			$POSITION_HIGH = 2048;

			$changeMask = 0;
			$result = mysql_query("SELECT * FROM `drive_lfss`.`stats_rank_driver` WHERE `licence_name`LIKE'{$driver['licence_name']}' AND `track_prefix`='$trackName' AND `car_prefix`='$carPrefix'",$link);
			if ($result) 
			{
				$row = mysql_fetch_array($result);
				
				if($row[3] < $driverBestS)
					$changeMask += $PB_HIGH;
				else if($row[3] > $driverBestS)
					$changeMask += $PB_LOW;
				
				if($row[4] < $driverAverageS)
					$changeMask += $AVG_HIGH;
				else if($row[4] > $driverAverageS)
					$changeMask += $AVG_LOW;
				
				if($row[5] < $driverStabilityS)
					$changeMask += $STA_HIGH;
				else if($row[5] > $driverStabilityS)
					$changeMask += $STA_LOW;
				
				if($row[6] < $driverWinS)
					$changeMask += $WIN_HIGH;
				else if($row[6] > $driverWinS)
					$changeMask += $WIN_LOW;
				
				if($row[7] < $rank)
					$changeMask += $TOTAL_HIGH;
				else if($row[7] > $rank)
					$changeMask += $TOTAL_LOW;
				
				/*if($row[8] > $driverBestS)
					$changeMask += $POSITION_HIGH;
				else if($row[8] < $driverBestS)
					$changeMask += $POSITION_LOW;*/
			}
			//continue;
			
			//echo "$driver[1] Global Ranking Score: $rank\n";
			$result = mysql_query("DELETE FROM `drive_lfss`.`stats_rank_driver` WHERE `licence_name`LIKE'{$driver['licence_name']}' AND `track_prefix`='$trackName' AND `car_prefix`='$carPrefix'",$link);
			if (!$result) {die(mysql_error());}
			
			
			
			$result = mysql_query("INSERT INTO `drive_lfss`.`stats_rank_driver` VALUES('{$driver['licence_name']}','$trackName','$carPrefix','$driverBestS','$driverAverageS','$driverStabilityS','$driverWinS','$rank','0','$changeMask')",$link);
			if (!$result) {die(mysql_error());}
		}
	}
}

foreach($drivers as $driver)
{
	echo "Position: {$driver['licence_name']}\n";
	foreach($trackNames as $trackName)
	{
		if(strstr($trackName,"AU") ) //later do this for Drag rank...
			continue;
			
		foreach($carPrefixs as $carPrefix)
		{
			$query = "SELECT `total_rank` As `myTotal`,
					(SELECT COUNT(`total_rank`) FROM `drive_lfss`.`stats_rank_driver` WHERE `track_prefix`='$trackName' AND `car_prefix`='$carPrefix' AND `total_rank` > `myTotal`) As Position 
					FROM `drive_lfss`.`stats_rank_driver` WHERE `track_prefix`='$trackName' AND `car_prefix`='$carPrefix' AND `licence_name`LIKE'{$driver['licence_name']}'";
			$result = mysql_query($query,$link);
			if (!$result) {die(mysql_error());}
			if($row = mysql_fetch_array($result))
			{
				mysql_free_result($result);
				$changeMask = 0;
				$result = mysql_query("SELECT `position` FROM `drive_lfss`.`stats_rank_driver` WHERE `licence_name`LIKE'{$driver['licence_name']}' AND `track_prefix`='$trackName' AND `car_prefix`='$carPrefix'",$link);
				
				if ($row2 = mysql_fetch_array($result)) 
				{
					mysql_free_result($result);
					if($row2[0] > $row[1])
						$changeMask += 2048;
					else if($row2[0] < $row[1])
						$changeMask += 1024;
				}
				mysql_query("UPDATE `drive_lfss`.`stats_rank_driver` SET `position`='".($row[1]+1)."', `change_mask`=`change_mask`|$changeMask  WHERE `licence_name`LIKE'{$driver['licence_name']}' AND `track_prefix`='$trackName' AND `car_prefix`LIKE'$carPrefix'",$link);
			}
		}
	}
}

mysql_close($link);



function LFSWTrackToTrackPrefix(&$lfswTrack)
{
	$trackPrefix = "";
	$lfswTrack = str_split($lfswTrack);

	switch ($lfswTrack[0])
	{
		case '0': $trackPrefix .= "BL"; break;
		case '1': $trackPrefix .= "SO"; break;
		case '2': $trackPrefix .= "FE"; break;
		case '3': $trackPrefix .= "AU"; break;
		case '4': $trackPrefix .= "KY"; break;
		case '5': $trackPrefix .= "WE"; break;
		case '6': $trackPrefix .= "AS"; break;
	}
	$trackPrefix .= ((int)$lfswTrack[1] + 1);
	$trackPrefix .= ($lfswTrack[2] == '1' ? "R" : "");

	return $trackPrefix;
}

function LFSWParsedTractTotrackPrefix($string)
{
	$trackPrefix = "";
	

	if(stristr($string,"blackw"))
	{
		if(stristr($string,"gp"))
			$trackPrefix = "BL1";
		else if(stristr($string,"rally"))
			$trackPrefix = "BL2";
		else if(stristr($string,"park"))
			$trackPrefix = "BL3";
	}
	if(stristr($string,"south"))
	{
		if(stristr($string,"classic"))
			$trackPrefix = "SO1";
		else if(stristr($string,"1"))
			$trackPrefix = "SO2";
		else if(stristr($string,"2"))
			$trackPrefix = "SO3";
		else if(stristr($string,"long"))
			$trackPrefix = "SO4";
		else if(stristr($string,"course"))
			$trackPrefix = "SO5";
		else if(stristr($string,"chica"))
			$trackPrefix = "SO6";
	}
	if(stristr($string,"fern"))
	{
		if(stristr($string,"club"))
			$trackPrefix = "FE1";
		else if(stristr($string,"green") && !stristr($string,"rally"))
			$trackPrefix = "FE2";
		else if(stristr($string,"gold"))
			$trackPrefix = "FE3";
		else if(stristr($string,"black"))
			$trackPrefix = "FE4";
		else if(stristr($string,"rally") && !stristr($string,"green"))
			$trackPrefix = "FE5";
		else if(stristr($string,"rally") && stristr($string,"green"))
			$trackPrefix = "FE6";
	}
	if(stristr($string,"autocross"))
	{
		if(!stristr($string,"skid") && !stristr($string,"strip") && !stristr($string,"lane"))
			$trackPrefix = "AU1";
		else if(stristr($string,"skid"))
			$trackPrefix = "AU2";
		else if(stristr($string,"strip"))
			$trackPrefix = "AU3";
		else if(stristr($string,"lane"))
			$trackPrefix = "AU4";
	}
	if(stristr($string,"kyo"))
	{
		if(stristr($string,"oval"))
			$trackPrefix = "KY1";
		else if(stristr($string,"national"))
			$trackPrefix = "KY2";
		else if(stristr($string,"gp"))
			$trackPrefix = "KY2";
	}
	if(stristr($string,"west"))
	{
		if(stristr($string,"interna"))
			$trackPrefix = "WE1";
	}
	if(stristr($string,"aston"))
	{
		if(stristr($string,"cadet"))
			$trackPrefix = "AS1";
		else if(stristr($string,"club"))
			$trackPrefix = "AS2";
		else if(stristr($string,"national"))
			$trackPrefix = "AS3";
		else if(stristr($string,"historic"))
			$trackPrefix = "AS4";
		else if(stristr($string,"prix"))
			$trackPrefix = "AS5";
		else if(stristr($string,"touring"))
			$trackPrefix = "AS6";
		else if(stristr($string,"north"))
			$trackPrefix = "AS7";
	}
	if(strstr($string,"Rev"))
		$trackPrefix .= "R";
		
	return $trackPrefix;
}
?>