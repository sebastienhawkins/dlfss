<?php
///Racers|Car|Laps|Total Time|Gap|Penalty|Best Laptime|On lap|Pit stops|Takeovers

//Start 
//End RacesData_3037522-3038522.txt
// Formulate Query
// This is the best way to perform a SQL query
// For more examples, see mysql_real_escape_string()

///Racers|Car|Laps|Total Time|Penalty|Best Laptime|On lap|Pit stops|Takeovers


// Perform Query


$link = mysql_connect('', 'root', 'dexxa','lfsw_pub_stats');
if (!$link) {die('Could not connect: ' . mysql_error());}
if (!mysql_select_db('lfs_pub_stats')){die('Could not connect: ' . mysql_error());}

$result = mysql_query("SELECT MAX(`guid`) FROM `race_data`;",$link);
$maxGuid = mysql_fetch_array ( $result );
$maxGuid = $maxGuid[0];

//$maxGuid = 3037911;

$start = $maxGuid+1;
$end = $maxGuid+100400;

echo $maxGuid."\n";

passthru ('SuckRaces.exe '.$start." ".$end );

//$raceOut = "guid|serverName|time|trackname\r\n";
//$lapOut = "guid|position|licence_name|car_prefix|lap_count|timetotal|penality|time_best_lap|best_lap_number|pit_stop|takeover|laps\r\n";

//@unlink(".\Races\LapData.csv");
//@unlink(".\Races\RaceData.csv");
$guid = "";
if(($fh = fopen(".\Races\RacesData_$start-$end.txt","r"))!== FALSE )
{
	echo "File is: RacesData_$start-$end.txt\n";
	$ticks = 0;
	$lines = 0;
	while (!feof($fh)) 
	{
		++$lines;
		$buffer = fgets($fh);

		//if($lines != 184)
			//continue;

		//echo $lines."\n";
		
		$guidtest = substr($buffer,0,7);
		if($guidtest == "")
		{
			echo "skip\n";
			continue;
			
        }
		$lastGuid = $guidtest;
		if($lastGuid == $guid)
		{
			echo "Skip\n";
			continue;
		}
		$guid = $guidtest;
		
		if((int)$guid < (int)($maxGuid-1))
			continue;
		//echo $guid."\n";
		//$raceOut .= $guid."|";
		
		$buffer = substr($buffer,888,strlen($buffer)-1067);
		if($buffer == "")
			continue;
		//echo $buffer;
		
		//echo "&#65418;~&#65428;&#65384;&#65390;&#65416;~\n";

		
		$hostname = unhtmlentities(substr($buffer,0,strpos($buffer,"</b>")));

		//echo $hostname."\n";
		if(strlen($hostname)> 32)
		{
			preg_match_all ( "/>([^<]+)</", $hostname ,$all );
			$hostname = "";
			foreach($all[1] as $value)
			{
				$hostname .= $value;
			}
		}

		//echo $hostname."\n";
		//var_dump($all);
		//$raceOut .= $hostname."|";
		
		$buffer = substr($buffer,strpos($buffer,"</b>")+4);
		//echo $buffer."\n";

		preg_match ( "/\s-\s([^-]+)/", $buffer ,$all );
		$startTime = rtrim($all[1]);
		//echo $startTime."\n";
		
		$date = split(",",$startTime);
		$date[0] = $date[0].":00";
		$date[1] = split(" ",$date[1]);
		switch($date[1][2])
		{
			case "Jan": $date[1][2] = 1;break;
			case "Feb": $date[1][2] = 2;break;
			case "Mar": $date[1][2] = 3;break;
			case "Apr": $date[1][2] = 4;break;
			case "May": $date[1][2] = 5;break;
			case "Jun": $date[1][2] = 6;break;
			case "Jul": $date[1][2] = 7;break;
			case "Aug": $date[1][2] = 8;break;
			case "Sep": $date[1][2] = 9;break;
			case "Oct": $date[1][2] = 10;break;
			case "Nov": $date[1][2] = 11;break;
			case "Dec": $date[1][2] = 12;break;
			default: 
				echo "ERROR unknow month: ".$date[1][2]."\n";
		}

		//$raceOut .= $date[1][3]."-".$date[1][2]."-".$date[1][1]." ".$date[0]."|";
		
		$buffer = substr($buffer,strpos($buffer,"<b>")+3);
		//echo $buffer."\n";
		
		
		$track = substr($buffer,0,strpos($buffer,"</b>"));
		$track = LFSWParsedTractTotrackPrefix($track);
		
		//echo $track."\n";
		//$raceOut .= $track."\r\n";

		//RACE DATA SAVING
		$queryR = sprintf("INSERT INTO `race_data` VALUES ('%s','%s','%s','%s')",$guid,mysql_real_escape_string($hostname),$date[1][3]."-".$date[1][2]."-".$date[1][1]." ".$date[0],$track);
		if(!mysql_query($queryR,$link)){echo mysql_error()."\n";}
		

		$buffer = substr($buffer,strpos($buffer,".</td><",0)-2);
		//echo $buffer."\n";
	
		$players = array();

		$nextPos = strpos($buffer,">2.<",0);
		if($nextPos === false)
			$nextPos = strpos($buffer,"</table>",0);

		array_push($players, substr($buffer,0,$nextPos));
		//echo $players[0]."\n\n";

		
		if(strpos($buffer,">1.<",0) !== FALSE)
		{
			playerInfo($players[0],$guid);
		}
		$buffer = substr($buffer,$nextPos);
		//echo $buffer."\n";

		//echo "\n _____________________________________________________________________________________________________________\n";
		
		$playerCount = 1;
		while(strlen($buffer) > 100 &&  $playerCount < 44)
		{
			$nextPos = strpos($buffer,">".($playerCount+2).".<",0);
			if($nextPos === false)
				$nextPos = strpos($buffer,"</table>",0);

			array_push($players, substr($buffer,0,$nextPos));
			//echo $players[$playerCount]."\n\n";
			
			$buffer = substr($buffer,$nextPos);
			
			playerInfo($players[$playerCount],$guid);
			//echo "\n _____________________________________________________________________________________________________________\n";
			$playerCount++;
		}

		//TODO parse each lap for each player for each race, mouhaha!



		//var_dump($all);

		//echo "\n\n\n";
	}
	fclose($fh);
	
	$fro = fopen(".\Races\RaceData.csv","a");
	//fwrite($fro,//$raceOut);
	//$raceOut = "";
	//fclose($fro);

	$flo = fopen(".\Races\LapData.csv","a");
	//fwrite($flo,//$lapOut);
	//$lapOut = "";
	//fclose($flo);
	

	//exit;
}
mysql_query("UPDATE `lap_data` SET `time_best_lap` = '0' WHERE `time_best_lap`<'5000'" ,$link);
mysql_close($link);
include('ranking_x.php');

function playerInfo($player,$guid)
{
	global $lapOut,$link;
//$lapOut .= $guid."|";

	preg_match ( "/>([0-9]+)\./", $player ,$all );
	$position = $all[1];
	//echo $position;
	if($position == "")
		$position = "0";
//$lapOut .= $position."|";

	//echo $player ."\n";
	$player = substr($player,27);

	$nextIndex = strpos($player,"</td>",0);
	$licenceName = unhtmlentities(substr($player,0,$nextIndex));
	//<a class=\"gen\" href=\"#\"  onClick=\"SEARCH_racerOptions (\'Takayuki\', 55); return false;\">Takayuki</a><br />

	preg_match ( "/>([^<]+)/", $licenceName ,$all );
	$licenceName = $all[1];

	//echo $licenceName."\n";
//$lapOut .= $licenceName."|";

	$player = substr($player,$nextIndex+5);
	//echo $player."\n";

	//$nextIndex = strpos($player,"</td>",0);
	$carPrefix = substr($player,19,3);
	//echo $carPrefix."\n";
//$lapOut .= $carPrefix."|";


	$player = substr($player,27);
	//echo $player."\n";

	preg_match ( "/>([0-9]+)</", $player ,$lapCount );
	$lapCount = $lapCount[1];
	//echo $lapCount."\n";
//$lapOut .= $lapCount."|";
	$player = substr($player,19);
	$player = substr($player,strpos($player,"<td",0));

	//echo $player."\n";
	preg_match ( "/>([0-9:\.]+)</", $player ,$timeTotal );
	$timeTotal = $timeTotal[1];
	//echo $timeTotal."\n";
	
	$timeMs = 0;

	if(strstr($timeTotal,":"))
	{
		$times = split(':',$timeTotal);
		$timeMs += $times[0]*60000;
		$timeTotal = substr($timeTotal,strpos($timeTotal,":")+1);
	}
	if(strstr($timeTotal,"."))
	{
		$times = split("\.",$timeTotal);
		if(strlen($times[1]) == 2)
			$times[1] *= 10;
		else if(strlen($times[1]) == 1)
			$times[1] *= 100;
		$timeMs += ($times[0]*1000)+$times[1];
	}
	$timeTotal= $timeMs;
//$lapOut .= $timeMs."|";

	$player = substr($player,19);
	$player = substr($player,strpos($player,"<td",0)+1);
	
	//echo $player."\n";

	/*preg_match ( "/>([0-9]*)</", $player ,$gap );
	$gap = $gap[1];
	//echo $gap."\n";
//$lapOut .= $gap."|";*/
	$player = substr($player,19);
	$player = substr($player,strpos($player,"<td",0));

	//echo $player."\n";

	preg_match ( "/>([\+0-9]*)</", $player ,$penality );
	$penality = $penality[1];
	//echo $penality."\n";
//$lapOut .= $penality."|";
	$player = substr($player,19);
	$player = substr($player,strpos($player,"<td",0));

	//echo $player."\n";
	
	preg_match ( "/>([:\.0-9]*)</", $player ,$timeBestLap );
	$timeBestLap = $timeBestLap[1];
	//echo $timeBestLap."\n";
	
	$timeMs = 0;

	if(strstr($timeBestLap,":"))
	{
		$times = split(':',$timeBestLap);
		$timeMs += $times[0]*60000;
		$timeBestLap = substr($timeBestLap,strpos($timeBestLap,":")+1);
	}
	if(strstr($timeBestLap,"."))
	{
		$times = split("\.",$timeBestLap);
		if(strlen($times[1]) == 2)
			$times[1] *= 10;
		else if(strlen($times[1]) == 1)
			$times[1] *= 100;
		$timeMs += ($times[0]*1000)+$times[1];
	}
	$timeBestLap = $timeMs;
//$lapOut .= $timeMs."|";

	$player = substr($player,19);
	$player = substr($player,strpos($player,"<td",0));

	//echo $player."\n";
	
	preg_match ( "/>([0-9]*)</", $player ,$bestlapNumber );
	$bestlapNumber = $bestlapNumber[1];
	//echo $pitStopCount."\n";
//$lapOut .= $bestlapNumber."|";
	$player = substr($player,19);
	$player = substr($player,strpos($player,"<td",0));

	//echo $player."\n";
	
		preg_match ( "/>([0-9]*)</", $player ,$pitStopCount );
	$pitStopCount = $pitStopCount[1];
	//echo $pitStopCount."\n";
//$lapOut .= $pitStopCount."|";
	$player = substr($player,19);
	$player = substr($player,strpos($player,"<td",0));

	//echo $player."\n";

	preg_match ( "/>([0-9]*)</", $player ,$takeOverCount );
	$takeOverCount = $takeOverCount[1];
	//echo $takeOverCount."\n";
//$lapOut .= $takeOverCount."|";
	$player = substr($player,19);
	$player = substr($player,strpos($player,"<td",0));

	//echo $player."\n";
	
	preg_match_all ( "@[0-9]{1,3}(?:</b>){0,1}\s([\.:0-9]+)@", $player ,$lapsTime,PREG_SET_ORDER );
	
	//echo "LapFollow\n";
	//$lapOut .= "0 ";
	$laps = "0 ";
	foreach($lapsTime as $lapTime)
	{

		$lapTime[1] = str_replace("</b>","",$lapTime[1]);
		//echo $lapTime[0]."\n";
		
		$timeMs = 0;

		if(strstr($lapTime[1],":"))
		{
			$times = split(':',$lapTime[1]);
			$timeMs += $times[0]*60000;
			$lapTime[1] = substr($lapTime[1],strpos($lapTime[1],":")+1);
		}
		if(strstr($lapTime[1],"."))
		{
			$times = split("\.",$lapTime[1]);
			if(strlen($times[1]) == 2)
				$times[1] *= 10;
			else if(strlen($times[1]) == 1)
				$times[1] *= 100;
			$timeMs += ($times[0]*1000)+$times[1];
		}

		//echo $timeMs."\n";
		
		$laps .= $timeMs." ";
		//$lapOut .= $timeMs." ";
	}
	if($bestlapNumber == "") $bestlapNumber = 0;
	$queryL = sprintf("INSERT INTO `lap_data` VALUES ('%s','%s','%s','%s','%s','%s','%s','%s','%s','%s','%s','%s')",$guid,$position,mysql_real_escape_string($licenceName),$carPrefix,$lapCount,$timeTotal,$penality,$timeBestLap,$bestlapNumber,$pitStopCount,$takeOverCount,$laps);
	if(!mysql_query($queryL,$link)){echo mysql_error()."\n";}
	
	//$lapOut .= "\r\n";
	//var_dump($lapsTime);

	//echo $player."\n";
}

function unhtmlentities($string)
{
    // replace numeric entities
    $string = preg_replace('~&#x([0-9a-f]+);~ei', 'chr(hexdec("\\1"))', $string);
    $string = preg_replace('~&#([0-9]+);~e', 'chr("\\1")', $string);
    // replace literal entities
    $trans_tbl = get_html_translation_table(HTML_ENTITIES);
    $trans_tbl = array_flip($trans_tbl);
    return strtr($string, $trans_tbl);
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