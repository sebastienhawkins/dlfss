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

$result = mysql_query("SELECT DISTINCT `licence_name` FROM `drive_lfss`.`driver`",$link);
if (!$result) {die(mysql_error());}
$licenceNames = array();
while ($row = mysql_fetch_array($result)) 
	array_push($licenceNames,$row[0]);

//var_dump($trackName);

if (!mysql_select_db('lfs_pub_stats')){die(mysql_error());}

$query = "SELECT DISTINCT `guid`,COUNT(`guid`) FROM `lap_data` GROUP BY `guid`";
$result = mysql_query($query ,$link);
if (!$result) {die(mysql_error());}
$driverCount = array();
while ($row = mysql_fetch_array($result))
	$driverCount[$row[0]] = $row[1];



foreach($licenceNames as $licenceName)
{
	echo "$licenceName\n";
	
	//Guids
	/*$guids = "";
	$query = "SELECT DISTINCT `lap_data`.`guid`
	FROM `lap_data`
	WHERE `licence_name`='$licenceName'";
	
	$result = mysql_query($query ,$link);
	if (!$result) {die(mysql_error());}
	while(($row = mysql_fetch_array($result)))
		$guids .= $row[0].",";
	
	if(substr($guids,strlen($guids)-1) == ",")
		$guids = substr($guids,0,strlen($guids)-1);
	
	//echo "Guids: $guids\n";*/
	
	foreach($trackNames as $trackName)
	{
		if(strstr($trackName,"AU") ) //later do this for Drag rank...
			continue;
			
		foreach($carPrefixs as $carPrefix)
		{
			/*if(	$carPrefix != "FBM" && $carPrefix != "XFG" && 
				$carPrefix != "XRG" && $carPrefix != "MRT" && 
				$carPrefix != "F08" && $carPrefix != "FOX") 
				continue;*/
				
			//echo "Track: $trackName, Car: $carPrefix\n";
			
			if(!isset($wr[$trackName][$carPrefix]))
			{
				//echo "Bypass: $trackName, $carPrefix no VALUE\n";
				continue;
			}
			$bestEver = $wr[$trackName][$carPrefix];
			//echo "BestEver: $bestEver \n";
			
			//Average
			$query = "SELECT AVG(`lap_data`.`time_best_lap`)
			FROM `lap_data`,`race_data`
			WHERE `lap_data`.`car_prefix`='$carPrefix'
			AND `lap_data`.`guid`=`race_data`.`guid`
			AND `race_data`.`track_name`='$trackName'
			AND `lap_data`.`time_best_lap` < ".($bestEver *2)."
			AND `lap_data`.`time_best_lap` >= ".($bestEver );

			$result = mysql_query($query ,$link);
			if (!$result) {die(mysql_error());}
			$row = mysql_fetch_array($result);
			$average = $row[0];
			//echo "Average: $average\n";


			$rank = 0;
			//echo "Driver: $licenceName\n";

			//if($licenceName != "Greenseed")continue;
			//Driver RaceCount
			$query = "SELECT COUNT(`lap_data`.`guid`)
			FROM `lap_data`,`race_data`
			WHERE `lap_data`.`licence_name`='$licenceName'
			AND `lap_data`.`car_prefix`='$carPrefix'
			AND `lap_data`.`guid`=`race_data`.`guid`
			AND `race_data`.`track_name`='$trackName'
			AND `lap_data`.`time_best_lap` >= ".($bestEver )."
			AND `lap_data`.`time_best_lap` < ".($bestEver *2);

			$result = mysql_query($query ,$link);
			if (!$result) {die(mysql_error());}
			$row = mysql_fetch_array($result);
			//var_dump($row);
			$driverRaceCount = $row[0];
			if($driverRaceCount < 3)
				continue;
			
			//echo "Track: $trackName, Car: $carPrefix\n";
			//echo "BestEver: $bestEver \n";
			//echo "Average: $average\n";
			//echo "$licenceName RaceCount: ".$driverRaceCount."\n";

			

			//Driver Best
			$query =   "SELECT `lap_data`.`time_best_lap`
			FROM `lap_data`,`race_data`
			WHERE `lap_data`.`licence_name`='$licenceName'
			AND `lap_data`.`car_prefix`='$carPrefix'
			AND `lap_data`.`guid`=`race_data`.`guid`
			AND `race_data`.`track_name`='$trackName'
			AND `lap_data`.`time_best_lap` >= ".($bestEver )."
			ORDER BY `lap_data`.`time_best_lap` LIMIT 1";

			$result = mysql_query($query ,$link);
			if (!$result) {die(mysql_error());}
			$row = mysql_fetch_array($result);
			//var_dump($row);
			$driverBest = $row[0]; 
			$driverBestS = ($bestEver / $driverBest * 9999 );
			$driverBestS = $driverBestS-(((9999)-$driverBestS)*2);
			$driverBestS = $driverBestS * 5000 / 9999;
			$driverBestS = (int)$driverBestS ;
			//echo "$licenceName Best | Score : ".$driverBest." | ".$driverBestS."\n";
			

			//Driver Average
			$query = "SELECT AVG(`lap_data`.`time_best_lap`)
			FROM `lap_data`,`race_data`
			WHERE `lap_data`.`licence_name`='$licenceName'
			AND `lap_data`.`car_prefix`='$carPrefix'
			AND `lap_data`.`guid`=`race_data`.`guid`
			AND `race_data`.`track_name`='$trackName'
			AND `lap_data`.`time_best_lap` >= ".($bestEver )."
			AND `lap_data`.`time_best_lap` < ".($bestEver *2);

			$result = mysql_query($query ,$link);
			if (!$result) {die(mysql_error());}
			$row = mysql_fetch_array($result);
			//var_dump($row);
			$driverAverage = $row[0];
			
			$driverAverageS = ( $average/$driverAverage*9999);
			$driverAverageS = $driverAverageS-(((9999)-$driverAverageS)*2);
			$driverAverageS = $driverAverageS * 5000 / 9999 /2;
			$driverAverageS = (int)$driverAverageS ;
			//echo "$licenceName Average | Score: ".$driverAverage." | ".$driverAverageS."\n";
			$driverStabilityS = ($driverBest / $driverAverage *9999);
			$driverStabilityS = $driverStabilityS-(((9999)-$driverStabilityS)*2);
			$driverStabilityS = $driverStabilityS * 5000 / 9999;
			$driverStabilityS = (int)$driverStabilityS ;
			//echo "$licenceName Stability Score: ".$driverStabilityS."\n";
			
			//Driver Win
			$driverWin = 0;
			$query = "SELECT `lap_data`.`guid`,`lap_data`.`position`
			FROM `lap_data`,`race_data`
			WHERE `lap_data`.`licence_name`='$licenceName'
			AND `lap_data`.`car_prefix`='$carPrefix'
			AND `lap_data`.`position` > 0
			AND `lap_data`.`position` < 4
			AND `lap_data`.`guid`=`race_data`.`guid`
			AND `race_data`.`track_name`='$trackName'
			AND `lap_data`.`time_best_lap` >= ".($bestEver )."
			AND `lap_data`.`time_best_lap` < ".($bestEver *2);

			$result = mysql_query($query ,$link);
			if (!$result) {die(mysql_error());}
			while($row = mysql_fetch_array($result))
			{
				if($driverCount[$row[0]] > 2)
				{
					for($pos = 1; $pos < 33 ;$pos++)
					{
						if($row[1] == $pos)
						{
							
							$driverWin += ($driverCount[$row[0]]-$pos)/2;
							//echo "$pos | $driverWin\n";
						}
					}
				}
			}
			$driverWinS = 0;
			if($driverWin > 0 )
				$driverWinS = (int)(($driverWin));
			//echo "$licenceName Win | Score: ".$driverWin." | ".$driverWinS."\n";
			
			$rank = (int)(($driverBestS + ($driverBestS/2)) + ($driverAverageS+($driverAverageS/5))+ $driverStabilityS + $driverWinS);
			
			$driverWinS = (int)$driverWinS;
			$result = mysql_query("DELETE FROM `drive_lfss`.`stats_rank_driver` WHERE `licence_name`='$licenceName' AND `track_prefix`='$trackName' AND `car_prefix`='$carPrefix'",$link);
			if (!$result) {die(mysql_error());}
			$result = mysql_query("INSERT INTO `drive_lfss`.`stats_rank_driver` VALUES('$licenceName','$trackName','$carPrefix','$driverBestS','$driverAverageS','$driverStabilityS','$driverWinS','$rank')",$link);
			if (!$result) {die(mysql_error());}
			

			//echo "$licenceName Global Ranking Score: $rank\n";
		}
	}
}
//$result = mysql_query("DELETE FROM `drive_lfss`.`stats_rank_driver` WHERE `licence_name`=''",$link);
//if (!$result) {die(mysql_error());}


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

/*function LFSWParsedTractTotrackPrefix($string)
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
}*/
?>