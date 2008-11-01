<?php 

$wrt = file_get_contents ("http://www.lfsworld.net/pubstat/get_stat2.php?version=1.4&user=dlfss&pass=dlfss&action=wr");

$wrt = explode("\n",$wrt);

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

$link = mysql_connect('192.168.101.200:33306', 'www', 'dexxa', true ,MYSQL_CLIENT_COMPRESS);
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
$licenceGuids = array();
$licenceNames = array();
while ($row = mysql_fetch_array($result))
{
	array_push($licenceNames,$row[0]);
}
mysql_query("UPDATE `button_template` SET `width`=25,`text`='^0Last ^72008^2/^7Nov^2/^701' WHERE `entry`IN(57)",$link);


foreach($licenceNames as $licenceName)
{
	//if(!strstr($licenceName,"OBP 55"))
	//	continue;
	echo "Driver_Guid: $licenceName\n";
	$driverGuids = "";
	$result = mysql_query("SELECT `guid` FROM `driver` WHERE `licence_name`LIKE'$licenceName'" ,$link);
	if (!$result) {die(mysql_error());}
	while( ($row = mysql_fetch_array($result)) )
	{
		$driverGuids .= $row[0]." ";
	}
	$driverGuids = " ".$driverGuids;
	foreach($trackNames as $trackName)
	{
		if(strstr($trackName,"AU") ) //later do this for Drag rank...
			continue;
		
		//if(!strstr($trackName,"BL1") ) //later do this for Drag rank...
		//	continue;
		foreach($carPrefixs as $carPrefix)
		{
			//if(!strstr($carPrefix,"FBM") ) //later do this for Drag rank...
			//	continue;
			
			if(!isset($wr[$trackName][$carPrefix]))
			{
				//echo "Bypass: $trackName, $carPrefix no VALUE\n";
				continue;
			}
			$bestEver = $wr[$trackName][$carPrefix];
			//echo "$trackName, $carPrefix BestEver: $bestEver \n";

			//Average
			$average = 0;
			$query = "SELECT AVG(`lap_time`)
			FROM `driver_lap`
			WHERE `driver_lap`.`track_prefix`='$trackName'
			AND `driver_lap`.`car_prefix`='$carPrefix'
			AND `driver_lap`.`lap_time` < ".($bestEver*2)."
			AND `driver_lap`.`lap_time` >= ".($bestEver-2000);
			$result = mysql_query($query ,$link);
			if (!$result) {die(mysql_error());}
			if ($result)
			{
				$row = mysql_fetch_array($result);
				$average = ($row[0] == "" ? 0 : $row[0]);
			}
			//echo "Average: $average\n";

			$rank = 0;
			//Driver RaceCount
			$query = "SELECT COUNT(`guid_driver`)
			FROM `driver_lap`
			WHERE `driver_lap`.`track_prefix`='$trackName'
			AND `driver_lap`.`guid_driver`IN(SELECT `guid` FROM `driver` WHERE `licence_name`LIKE'$licenceName')
			AND `driver_lap`.`car_prefix`='$carPrefix'
			AND `driver_lap`.`lap_time` >= ".($bestEver -2000)."
			AND `driver_lap`.`lap_time` < ".($bestEver *2);

			$result = mysql_query($query ,$link);
			if (!$result) {die(mysql_error());}
			$row = mysql_fetch_array($result);
			//var_dump($row);
			$driverRaceCount = $row[0];
			//echo "RaceCount: $driverRaceCount\n";
			if($driverRaceCount < 1)
				continue;

			//Driver Best
			$query =   "SELECT `lap_time`
			FROM `driver_lap`
			WHERE `driver_lap`.`track_prefix`='$trackName'
			AND `driver_lap`.`guid_driver`IN(SELECT `guid` FROM `driver` WHERE `licence_name`LIKE'$licenceName')
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

			//Driver Average
			$query = "SELECT AVG(`lap_time`),COUNT(`lap_time`)
			FROM `driver_lap`
			WHERE `driver_lap`.`track_prefix`='$trackName'
			AND `driver_lap`.`guid_driver`IN(SELECT `guid` FROM `driver` WHERE `licence_name`LIKE'$licenceName')
			AND `driver_lap`.`car_prefix`='$carPrefix'
			AND `driver_lap`.`lap_time` >= ".($bestEver )."
			AND `driver_lap`.`lap_time` < ".($bestEver *2);

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
						$driverAverageS = $driverAverageS/2;
	
				$driverStabilityS = ($driverBest / $driverAverage *9999);
				$driverStabilityS = $driverStabilityS-(((9999)-$driverStabilityS)*2);
				$driverStabilityS = $driverStabilityS * 4999 / 9999;
				$driverStabilityS = (int)$driverStabilityS ;
				if($row[1]<10)
						$driverStabilityS = $driverStabilityS/2;
				
			}
			else
				$driverAverage = $driverAverageS = $driverStabilityS = 0;
			//echo "Average $average | Score: $driverAverage | $driverAverageS\n";
			//echo "Stability Score: $driverStabilityS\n";

			//Driver Win
			//$result = mysql_query("SELECT `race_win_rank` FROM `stats_rank_driver` WHERE `licence_name` LIKE'$licenceName'" ,$link);
			//if ($result && ($row = mysql_fetch_array($result))) 
			//	$driverWinS = $row[0];
			//else
			$driverWinS = 0;

			$query = "SELECT `race`.`finish_order`,`race`.`race_laps`,`driver_lap`.`total_time`
			FROM `driver_lap`,`race`
			WHERE `race`.`finish_order` != ''
			AND `race`.`track_prefix`='$trackName'
			AND `race`.`guid`=`driver_lap`.`guid_race`
			AND `driver_lap`.`guid_driver`IN(SELECT `guid` FROM `driver` WHERE `licence_name`LIKE'$licenceName')
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
					if($value != "" && strstr($driverGuids," ".$value." "))
					{
						$_driverWinS = $winK + $driverCount-$itr + 1;
						if($_driverWinS > 0)
								$driverWinS += $_driverWinS ;
						echo "$trackName | $carPrefix | $winK | $averageLapTime | $bestEver | $driverCount position: $itr | $_driverWinS | $driverWinS\n";
						break;
					}
				}
			}
			

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
			$result = mysql_query("SELECT * FROM `drive_lfss`.`stats_rank_driver` WHERE `licence_name`LIKE'$licenceName' AND `track_prefix`='$trackName' AND `car_prefix`='$carPrefix'",$link);
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
			
			//echo "$licenceName Global Ranking Score: $rank\n";
			$result = mysql_query("DELETE FROM `drive_lfss`.`stats_rank_driver` WHERE `licence_name`LIKE'$licenceName' AND `track_prefix`='$trackName' AND `car_prefix`='$carPrefix'",$link);
			if (!$result) {die(mysql_error());}
			
			
			
			$result = mysql_query("INSERT INTO `drive_lfss`.`stats_rank_driver` VALUES('$licenceName','$trackName','$carPrefix','$driverBestS','$driverAverageS','$driverStabilityS','$driverWinS','$rank','0','$changeMask')",$link);
			if (!$result) {die(mysql_error());}
			

			
		}
	}
}

mysql_close($link);
include("ranking_position_aleajecta.php");

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