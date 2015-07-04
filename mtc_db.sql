-- phpMyAdmin SQL Dump
-- version 4.1.6
-- http://www.phpmyadmin.net
--
-- Host: 127.0.0.1
-- Generation Time: May 12, 2015 at 09:18 AM
-- Server version: 5.5.36
-- PHP Version: 5.4.25

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8 */;

--
-- Database: `mtc_db`
--

DELIMITER $$
--
-- Procedures
--
CREATE DEFINER=`root`@`localhost` PROCEDURE `InfoUser`(IN `_user_id` int)
BEGIN
	#Routine body goes here...
	SELECT * FROM mtc_user_tbl WHERE user_id=_user_id;
END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `p_change_default_playlist`(IN _playlist_id int, in _playlist_status  tinyint(2))
BEGIN
	#Routine body goes here... 
  DECLARE user_id int DEFAULT 0;
	SELECT playlist_user INTO user_id FROM mtc_playlist WHERE playlist_id=_playlist_id;
	IF user_id<>0 THEN
	 UPDATE mtc_playlist SET playlist_default=0 WHERE playlist_user=user_id and playlist_id<> _playlist_id;		
	END IF;
	UPDATE `mtc_playlist` SET `playlist_default`=_playlist_status WHERE playlist_id=_playlist_id;
END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `p_change_status_playlist`(IN _playlist_id int, in _playlist_status tinyint(2))
BEGIN
	#Routine body goes here...  
	UPDATE `mtc_playlist` SET `playlist_status`=_playlist_status WHERE playlist_id=_playlist_id;
END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `p_delete_device`(IN `_d_id` int)
BEGIN
	#Routine body goes here...
	DELETE FROM mtc_device_tbl WHERE mtc_device_tbl.device_id=_d_id;
END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `p_delete_media`(IN `_media_id` int)
BEGIN
	#Routine body goes here...
	DELETE FROM mtc_media_tbl WHERE mtc_media_tbl.media_id=_media_id;
END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `p_delete_playlist`(IN _playlist_id int)
BEGIN
	#Routine body goes here...    
	DELETE FROM mtc_playlist WHERE mtc_playlist.playlist_id=_playlist_id;
END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `p_delete_playlist_details`(IN `_id` int)
BEGIN
	#Routine body goes here...
	DELETE FROM `mtc_playlist_details` WHERE `detail_id`=_id; 
END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `p_delete_schedule`(IN `_s_id` int)
BEGIN
	#Routine body goes here...
	DELETE FROM mtc_schedule_tbl WHERE mtc_schedule_tbl.schedule_id=_s_id OR mtc_schedule_tbl.schedule_parent=_s_id;
	
END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `p_delete_user`(IN `_user_id` int)
BEGIN
	#Routine body goes here...
DELETE FROM `mtc_user_tbl` WHERE `user_id`=_user_id;
END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `p_device_login`(IN `_d_hash` varchar(100),OUT _result int)
BEGIN
	#Routine body goes here...
	DECLARE _id INT DEFAULT 0;
	SET _result=0;
	SELECT device_id INTO _id FROM mtc_device_tbl WHERE device_status=1 AND MD5(CONCAT(device_ip,device_pass))=_d_hash; 
	IF _id<>0 THEN
		SET _result=1;
		SELECT * FROM mtc_device_tbl WHERE device_id=_id;
	END IF;
END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `p_find_device`(IN `_key` varchar(100),IN `_from` int,IN `_number` int,OUT `_total` int)
BEGIN
	#Routine body goes here...
	SELECT COUNT(*) INTO _total FROM mtc_device_tbl WHERE (mtc_device_tbl.device_comment LIKE _key OR mtc_device_tbl.device_name LIKE _key OR mtc_device_tbl.device_ip LIKE _key);
	SELECT * FROM mtc_device_tbl WHERE (mtc_device_tbl.device_comment LIKE _key OR mtc_device_tbl.device_name LIKE _key OR mtc_device_tbl.device_ip LIKE _key) LIMIT _from,_number;
END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `p_find_device_by_user`(IN `_user_id` int,IN _key varchar(255),IN `_from_date` Datetime,OUT `_total` int)
BEGIN
	#Routine body goes here...  
	SET _total=0;
	SELECT COUNT(*) INTO _total FROM mtc_device_tbl B, mtc_schedule_tbl A 
	WHERE B.device_id=A.schedule_device and A.schedule_user= _user_id 
	and DATE_FORMAT(_from_date,"%Y%m%d%H%i")<DATE_FORMAT(A.schedule_time_end,"%Y%m%d%H%i") and DATE_FORMAT(A.schedule_time_begin,"%Y%m%d%H%i")<= DATE_FORMAT(_from_date,"%Y%m%d%H%i")
  and (B.device_name LIKE _key OR B.device_ip LIKE _key OR B.device_comment LIKE _key); 
	
	SELECT B.* FROM mtc_device_tbl B, mtc_schedule_tbl A 
	WHERE B.device_id=A.schedule_device and A.schedule_user= _user_id 
	and DATE_FORMAT(_from_date,"%Y%m%d%H%i")<DATE_FORMAT(A.schedule_time_end,"%Y%m%d%H%i") and DATE_FORMAT(A.schedule_time_begin,"%Y%m%d%H%i")<= DATE_FORMAT(_from_date,"%Y%m%d%H%i")
  and (B.device_name LIKE _key OR B.device_ip LIKE _key OR B.device_comment LIKE _key);
	
END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `p_find_media`(IN `_key` varchar(255),IN `_user_id` int,IN _type_id int,IN `_from` int,IN `_number` int,OUT `_total` int)
BEGIN
	#Routine body goes here...
	SET _total=0; 
	IF _user_id=-1 THEN		
		IF _type_id<>0 THEN
			SELECT COUNT(*) INTO _total FROM mtc_media_tbl WHERE (mtc_media_tbl.media_name LIKE _key OR mtc_media_tbl.media_comment LIKE _key) AND mtc_media_tbl.media_type=_type_id;
			SELECT *  FROM mtc_media_tbl WHERE (mtc_media_tbl.media_name LIKE _key OR mtc_media_tbl.media_comment LIKE _key ) AND mtc_media_tbl.media_type=_type_id LIMIT _from,_number;
		ELSE 
			SELECT COUNT(*) INTO _total FROM mtc_media_tbl WHERE (mtc_media_tbl.media_name LIKE _key OR mtc_media_tbl.media_comment LIKE _key) AND mtc_media_tbl.media_status=1;
			SELECT *  FROM mtc_media_tbl WHERE (mtc_media_tbl.media_name LIKE _key OR mtc_media_tbl.media_comment LIKE _key ) AND mtc_media_tbl.media_status=1 LIMIT _from,_number;
		END IF;
	ELSE
		IF _type_id <>0 THEN
			SELECT COUNT(*) INTO _total FROM mtc_media_tbl WHERE (mtc_media_tbl.media_name LIKE _key OR mtc_media_tbl.media_comment LIKE _key )AND mtc_media_tbl.media_user=_user_id AND mtc_media_tbl.media_type=_type_id;
			SELECT *  FROM mtc_media_tbl WHERE (mtc_media_tbl.media_name LIKE _key OR mtc_media_tbl.media_comment LIKE _key) AND mtc_media_tbl.media_user=_user_id AND mtc_media_tbl.media_type=_type_id LIMIT _from,_number;
		ELSE
			SELECT COUNT(*) INTO _total FROM mtc_media_tbl WHERE (mtc_media_tbl.media_name LIKE _key OR mtc_media_tbl.media_comment LIKE _key )AND mtc_media_tbl.media_user=_user_id AND mtc_media_tbl.media_status=1;
			SELECT *  FROM mtc_media_tbl WHERE (mtc_media_tbl.media_name LIKE _key OR mtc_media_tbl.media_comment LIKE _key) AND mtc_media_tbl.media_user=_user_id AND mtc_media_tbl.media_status=1 LIMIT _from,_number;
		END IF;
	END IF;
END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `p_find_playlist`(IN _key varchar(100),IN `_user_id` int,IN `_from` int,IN `_number` int,OUT `_total` int)
BEGIN
	#Routine body goes here... 
	SET _total=0; 
	IF _user_id=-1 THEN		
		SELECT COUNT(*) INTO _total FROM mtc_playlist WHERE (mtc_playlist.playlist_name LIKE _key OR mtc_playlist.playlist_comment LIKE _key);
		SELECT *  FROM mtc_playlist WHERE (mtc_playlist.playlist_name LIKE _key OR mtc_playlist.playlist_comment LIKE _key) LIMIT _from,_number;
	ELSE
		SELECT COUNT(*) INTO _total FROM mtc_playlist WHERE (mtc_playlist.playlist_name LIKE _key OR mtc_playlist.playlist_comment LIKE _key) AND mtc_playlist.playlist_user=_user_id ;
		SELECT *  FROM mtc_playlist WHERE (mtc_playlist.playlist_name LIKE _key OR mtc_playlist.playlist_comment LIKE _key) AND mtc_playlist.playlist_user=_user_id  LIMIT _from,_number;
	END IF;
END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `p_get_all_device`(IN `_from` int,IN `_number` int,OUT `_total` int)
BEGIN
	#Routine body goes here...
	SET _total=0;
	SELECT COUNT(*) INTO _total FROM mtc_device_tbl;
	SELECT * FROM mtc_device_tbl LIMIT _from,_number;
END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `p_get_all_device_by_user`(IN `_user_id` int,IN `_from_date` Datetime,OUT `_total` int)
BEGIN
	#Routine body goes here...  
	SET _total=0;
	SELECT COUNT(DISTINCT B.device_id) INTO _total FROM mtc_device_tbl B, mtc_schedule_tbl A 
	WHERE B.device_id=A.schedule_device and A.schedule_user= _user_id 
	and DATE_FORMAT(_from_date,"%Y%m%d%H%i")< DATE_FORMAT(A.schedule_time_end,"%Y%m%d%H%i") and DATE_FORMAT(A.schedule_time_begin,"%Y%m%d%H%i")<= DATE_FORMAT(_from_date,"%Y%m%d%H%i");
	
	SELECT DISTINCT B.* FROM mtc_device_tbl B, mtc_schedule_tbl A 
	WHERE B.device_id=A.schedule_device and A.schedule_user= _user_id 
	and DATE_FORMAT(_from_date,"%Y%m%d%H%i")<DATE_FORMAT(A.schedule_time_end,"%Y%m%d%H%i") and DATE_FORMAT(A.schedule_time_begin,"%Y%m%d%H%i")<= DATE_FORMAT(_from_date,"%Y%m%d%H%i");
	
END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `p_get_all_media`(IN `_user_id` int,IN `_media_type` int,IN `_from` int,IN `_number` int,OUT `_total` int)
BEGIN
	#Routine body goes here...
IF _user_id<>-1 THEN
	SELECT COUNT(*) INTO _total FROM mtc_media_tbl WHERE mtc_media_tbl.media_user= _user_id and media_type=_media_type; 
	SELECT * FROM mtc_media_tbl WHERE mtc_media_tbl.media_user = _user_id and media_type=_media_type LIMIT _from,_number;
ELSE 
	SELECT COUNT(*) INTO _total FROM mtc_media_tbl WHERE  media_type=_media_type; 
	SELECT * FROM mtc_media_tbl WHERE  media_type=_media_type LIMIT _from,_number;
END IF;
END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `p_get_all_playlist_by_user`(IN `_user_id` int,IN `_from` int,IN `_number` int,OUT `_total` int)
BEGIN
	#Routine body goes here... 
	SET _total=0; 
	IF _user_id=-1 THEN		
		SELECT COUNT(*) INTO _total FROM mtc_playlist;
		SELECT *  FROM mtc_playlist LIMIT _from,_number;
	ELSE
		SELECT COUNT(*) INTO _total FROM mtc_playlist WHERE mtc_playlist.playlist_user=_user_id ;
		SELECT *  FROM mtc_playlist WHERE mtc_playlist.playlist_user=_user_id  LIMIT _from,_number;
	END IF;
END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `p_get_all_type_device`()
BEGIN
	#Routine body goes here...
	SELECT * FROM mtc_device_type;
END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `p_get_all_type_user`()
BEGIN
	#Routine body goes here...
	SELECT * FROM mtc_type_user_tbl WHERE type_status =1;
END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `p_get_all_user`(IN `_number` int,IN `_from` int,OUT `total` int)
BEGIN
	#Routine body goes here...
if `_number` =0 THEN
	SELECT * FROM `mtc_user_tbl`;
ELSE 
	SELECT * FROM `mtc_user_tbl` LIMIT _from,`_number` ;
end if;
SELECT COUNT(*) INTO total FROM mtc_user_tbl;
END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `p_get_info_user_by_text`(IN `_user_name` varchar(100))
BEGIN
	#Routine body goes here...
	SELECT * FROM mtc_user_tbl WHERE mtc_user_tbl.user_name=_user_name;
END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `p_get_media_by_device_id`(IN `_d_id` int,out _user_id int,OUT _details_id int)
BEGIN
	#Routine body goes here... 
	DECLARE _play_list INT DEFAULT 0; 
	DECLARE _media_id INT DEFAULT 0;
	SET _user_id=0;
	SET _details_id=0;
	SELECT schedule_user, schedule_playlist INTO _user_id,_play_list FROM mtc_schedule_tbl WHERE ((schedule_time_begin <= NOW() and schedule_time_end>= NOW() AND schedule_loop=0) OR(schedule_loop=1 AND DATE_FORMAT(schedule_time_begin,"%T")<=DATE_FORMAT(NOW(),"%T") and DATE_FORMAT(schedule_time_end,"%T")>=DATE_FORMAT(NOW(),"%T"))) and schedule_status=1 and schedule_device=_d_id ORDER BY schedule_time_begin desc LIMIT 0,1;
	IF _play_list<>0 THEN
		SELECT detail_id INTO _details_id FROM mtc_playlist_details WHERE (time_begin<= CURTIME() AND time_end >= CURTIME()) AND playlist_id= _play_list;
		#SELECT A.* FROM mtc_media_tbl A, mtc_playlist_details C WHERE (C.time_begin<= CURTIME() AND C.time_end > CURTIME()) AND C.media_id=A.media_id AND C.playlist_id= _play_list;
	ELSE
		IF _user_id<>0 THEN
			SELECT A.detail_id INTO _details_id FROM mtc_playlist B, mtc_playlist_details A WHERE B.playlist_user= _user_id AND B.playlist_default=1 AND (A.time_begin<= CURTIME() AND A.time_end > CURTIME()) AND B.playlist_id=A.playlist_id;
		END IF;
	END IF;
	IF _details_id<>0 THEN
		SELECT A.* FROM mtc_media_tbl A, mtc_playlist_details B WHERE A.media_id=B.media_id AND A.media_status=1 AND B.detail_id=_details_id;
	END IF;
END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `p_get_media_event_by_device_id`(IN `_d_id` int,out _user_id int)
BEGIN
	#Routine body goes here...
	DECLARE _playlist_id INT DEFAULT 0;
	SET _user_id=0;
	SELECT schedule_user,schedule_playlist INTO _user_id,_playlist_id FROM mtc_schedule_tbl  WHERE ((schedule_time_begin <= NOW() and schedule_time_end> NOW() AND schedule_loop=0) OR(schedule_loop=0 AND DATE_FORMAT(schedule_time_begin,"%T")<=DATE_FORMAT(NOW(),"%T") and DATE_FORMAT(schedule_time_end,"%T")>DATE_FORMAT(NOW(),"%T"))) and schedule_status=1 and schedule_device=_d_id ORDER BY schedule_time_begin desc LIMIT 0,1;
	IF _playlist_id<>0 THEN
		SELECT * FROM mtc_playlist_details WHERE playlist_id=_playlist_id AND CURTIME()>= time_begin AND CURTIME()< time_end;
	ELSE
		IF _user_id<>0 THEN
			SELECT B.* FROM mtc_playlist A, mtc_playlist_details B WHERE A.playlist_default=1 AND A.playlist_user=_user_id AND A.playlist_id=B.playlist_id AND CURTIME()>= B.time_begin AND CURTIME()< B.time_end;
		END IF;
	END IF;
END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `p_get_media_in_playlist`(In `_playlist_id` int)
BEGIN
	#Routine body goes here...
		SELECT* FROM mtc_playlist_details WHERE playlist_id=_playlist_id ORDER BY time_begin; 
END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `p_get_playlist_default`(IN `_user_id` int)
BEGIN
	#Routine body goes here...
	SELECT * FROM mtc_playlist WHERE playlist_user=_user_id AND playlist_default=1;
END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `p_get_schedule_by_device_id`(IN `_d_id` int)
BEGIN
	SELECT * FROM mtc_schedule_tbl WHERE ((schedule_time_begin <= NOW() and schedule_time_end> NOW() AND schedule_loop=0) OR(schedule_loop=0 AND DATE_FORMAT(schedule_time_begin,"%T")<=DATE_FORMAT(NOW(),"%T") and DATE_FORMAT(schedule_time_end,"%T")>DATE_FORMAT(NOW(),"%T"))) and schedule_status=1 and schedule_device=_d_id ORDER BY schedule_time_begin desc LIMIT 0,1;
	
END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `p_get_schedule_date`(IN `_date` datetime)
BEGIN
	#Routine body goes here...
	SELECT * FROM mtc_schedule_tbl WHERE (DATE_FORMAT(_date,"%Y%m%d")<=DATE_FORMAT(schedule_time_end,"%Y%m%d") AND DATE_FORMAT(_date,"%Y%m%d")>= DATE_FORMAT(schedule_time_begin,"%Y%m%d") AND schedule_loop=0) OR schedule_loop=1;
END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `p_get_schedule_date_user`(IN `_date` datetime, IN `_user` int)
BEGIN
	#Routine body goes here... 
	SELECT * FROM mtc_schedule_tbl WHERE DATE_FORMAT(_date,"%Y%m%d")<=DATE_FORMAT(schedule_time_end,"%Y%m%d") AND DATE_FORMAT(_date,"%Y%m%d")>= DATE_FORMAT(schedule_time_begin,"%Y%m%d") AND schedule_user=_user;
END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `p_get_type_user_by_id`(IN `_user_id` int)
BEGIN
	#Routine body goes here...
	SELECT B.* FROM mtc_user_tbl A, mtc_type_user_tbl B WHERE A.user_type=B.type_id and A.user_id=_user_id; 
END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `p_get_user_of_device`(IN `_device_id` int)
BEGIN
	#Routine body goes here...
	DECLARE _user_id int DEFAULT -1;
  DECLARE _num_rows INT DEFAULT 0;
	DECLARE _parent_id INT DEFAULT 0;
	DECLARE _cache_id INT DEFAULT 0;
	REPEAT 
		SELECT COUNT(*),schedule_id,schedule_user INTO _num_rows,_parent_id,_user_id FROM mtc_schedule_tbl WHERE schedule_parent=_parent_id
		AND schedule_time_begin <= NOW() and schedule_time_end> NOW() AND schedule_device =_device_id;
		IF _parent_id <>0 THEN 
			SET _cache_id= _user_id;
		END IF;
	UNTIL _num_rows =0 END REPEAT;
	SELECT * FROM mtc_user_tbl  WHERE user_id= _cache_id;
END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `p_info_device`(IN `_d_id` int)
BEGIN
	#Routine body goes here... 
	SELECT * FROM mtc_device_tbl WHERE mtc_device_tbl.device_id=_d_id;
END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `p_info_media`(IN `_media_id` int)
BEGIN
	#Routine body goes here...
	SELECT * FROM mtc_media_tbl WHERE media_id =_media_id; 
END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `p_info_playlist`(IN `_playlist_id` int)
BEGIN
	#Routine body goes here...
	SELECT * FROM mtc_playlist WHERE playlist_id=_playlist_id; 
END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `p_info_playlist_details`(IN `_id` int)
BEGIN
	#Routine body goes here...
	SELECT * FROM mtc_playlist_details WHERE detail_id= _id;
END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `p_insert_device`(In `_d_name` varchar(255),in _d_ip varchar(30),IN `_d_type` int,in _d_pass varchar(100),IN `_d_comment` text,out _d_id int)
BEGIN
	#Routine body goes here...
	SET _d_id=-1;
	INSERT INTO `mtc_device_tbl`(`device_name`, `device_type`,  `device_comment`, device_ip,device_pass) VALUES (_d_name,_d_type,_d_comment,_d_ip,_d_pass);
	SET _d_id= LAST_INSERT_ID(); 
END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `p_insert_media`(IN `_media_name` varchar(255),IN `_media_url` varchar(255),IN `_media_type` int,IN `_media_comment` text,IN `_media_size` varchar(100),IN `_media_duration` varchar(100),IN `_media_user` int,in `_media_md5` varchar(50),OUT `_media_id` int)
BEGIN
	#Routine body goes here...
DECLARE _status TINYINT(2) DEFAULT 0;
DECLARE _code VARCHAR(10) DEFAULT 'FILE';

SELECT type_code INTO _code FROM mtc_media_type_tbl WHERE type_id=_media_type;
IF _code='URL' THEN
	SET _status=1;
END IF;
SET _media_id=-1;
INSERT INTO `mtc_media_tbl`( `media_name`, `media_url`, `media_type`, `media_comment`, `media_size`, `media_duration`, `media_user`,`media_status`,`media_md5`) 
VALUES (_media_name,_media_url,_media_type,_media_comment,_media_size,_media_duration,_media_user,_status,_media_md5); 
SET _media_id= LAST_INSERT_ID();
END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `p_insert_playlist`(IN `_playlist_name` varchar(100),IN `_playlist_user` int,IN `_playlist_comment` text)
BEGIN
	#Routine body goes here... 
	INSERT INTO `mtc_playlist`( `playlist_name`, `playlist_user`, `playlist_comment`) VALUES (_playlist_name,_playlist_user,_playlist_comment);
END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `p_insert_playlist_details`(IN `_media_id` int,IN `_playlist_id` int,IN `_time_begin` time,IN `_time_end` time)
BEGIN
	#Routine body goes here...
	INSERT INTO `mtc_playlist_details`( `playlist_id`, `media_id`, `time_begin`, `time_end`) VALUES (`_playlist_id`,`_media_id` ,`_time_begin` ,`_time_end` );
END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `p_insert_schedule_device`(IN `_device` int,IN `_user` int,IN `_parent` int,IN `_time_begin` datetime,IN `_time_end` datetime,IN `_loop` tinyint ,IN `_comment` varchar(100),out result int)
BEGIN
	#Routine body goes here...
	DECLARE tmp_result INT; 
	DECLARE parent_time_begin DateTime;
	DECLARE parent_time_end DateTime;
	DECLARE parent_loop TINYINT(2) DEFAULT 0;
	DECLARE parent_check TINYINT(2) DEFAULT 0;
	SET result=0;
  SET tmp_result=0;

IF _time_begin<_time_end THEN
	IF _parent<>0 THEN
		SELECT schedule_time_begin, schedule_time_end,schedule_loop INTO parent_time_begin,parent_time_end,parent_loop FROM mtc_schedule_tbl WHERE schedule_id=_parent;
		#Cha Khong looop
		IF parent_loop=0 THEN
			#con khong Loop
			IF _loop=0 THEN
				IF parent_time_begin<= _time_begin AND parent_time_end>= _time_end THEN
					SET parent_check =1;
				END IF;
			#con loop
			ELSE
				IF DATE_FORMAT(_time_begin,'%Y%m%d')>DATE_FORMAT(_time_end,'%Y%m%d') THEN
					IF DATE_FORMAT(parent_time_begin,'%T')<=DATE_FORMAT(_time_begin,'%T') AND DATE_FORMAT(parent_time_end,'%T') >= DATE_FORMAT(_time_end,'%T') THEN
							SET parent_check =1;
					END IF;
				ELSE
					SET _loop=0;
					SET _time_begin= ADDTIME(DATE(parent_time_begin),TIME(_time_begin));
					SET _time_end= ADDTIME(DATE(parent_time_end),TIME(_time_end));
				END IF;
			END IF;
	 #cha loop
		ELSE
			IF _loop=0 THEN
				IF DATE_FORMAT(parent_time_begin,'%T')<=DATE_FORMAT(_time_begin,'%T') AND DATE_FORMAT(parent_time_end,'%T') >= DATE_FORMAT(_time_end,'%T') THEN
					SET parent_check =1;
				END IF;
			ELSE
				IF DATE_FORMAT(_time_begin,'%Y%m%d')=DATE_FORMAT(_time_end,'%Y%m%d') THEN
					IF DATE_FORMAT(parent_time_begin,'%T')<=DATE_FORMAT(_time_begin,'%T') AND DATE_FORMAT(parent_time_end,'%T') >= DATE_FORMAT(_time_end,'%T') THEN
						SET parent_check =1;
					END IF;
				END IF;
			END IF;
		END IF;
		IF parent_check=1 THEN
			IF _loop =0 THEN
				SELECT schedule_id INTO tmp_result FROM mtc_schedule_tbl WHERE 
				schedule_parent = _parent AND 
				((schedule_loop<>1 and ((schedule_time_begin<= _time_begin AND schedule_time_end>= _time_begin )
					OR (schedule_time_begin<= _time_end AND schedule_time_end>= _time_end) OR (schedule_time_begin>= _time_begin AND schedule_time_end<= _time_begin ))) 
					OR (schedule_loop=1 AND 
					((DATE_FORMAT(schedule_time_begin,'%T')<= DATE_FORMAT(_time_begin,'%T') AND DATE_FORMAT(schedule_time_end,'%T')>= DATE_FORMAT(_time_begin,'%T')) OR
					 (DATE_FORMAT(schedule_time_begin,'%T')<= DATE_FORMAT(_time_end,'%T') AND DATE_FORMAT(schedule_time_end,'%T')>= DATE_FORMAT(_time_end,'%T')) OR
					 (DATE_FORMAT(schedule_time_begin,'%T')>= DATE_FORMAT(_time_begin,'%T') AND DATE_FORMAT(schedule_time_end,'%T')<= DATE_FORMAT(_time_end,'%T'))
					)))
				AND schedule_status=1 AND schedule_device=_device LIMIT 0,1;
			ELSE
				SELECT schedule_id INTO tmp_result FROM mtc_schedule_tbl WHERE 
				schedule_parent = _parent AND 
					(DATE_FORMAT(schedule_time_end,'%Y%m%d')>= DATE_FORMAT(NOW(),'%Y%m%d') AND( (DATE_FORMAT(schedule_time_begin,'%T')<= DATE_FORMAT(_time_begin,'%T') AND DATE_FORMAT(schedule_time_end,'%T')>= DATE_FORMAT(_time_begin,'%T')) OR
					 (DATE_FORMAT(schedule_time_begin,'%T')<= DATE_FORMAT(_time_end,'%T') AND DATE_FORMAT(schedule_time_end,'%T')>= DATE_FORMAT(_time_end,'%T')) OR
					 (DATE_FORMAT(schedule_time_begin,'%T')>= DATE_FORMAT(_time_begin,'%T') AND DATE_FORMAT(schedule_time_end,'%T')<= DATE_FORMAT(_time_end,'%T'))))
				AND schedule_status=1 AND schedule_device=_device LIMIT 0,1;
			END IF;
		END IF;

		IF tmp_result=0 THEN
			INSERT INTO `mtc_schedule_tbl`(`schedule_device`, `schedule_user`, `schedule_parent`, `schedule_time_begin`, `schedule_time_end`, `schedule_comment`,`schedule_loop`)
			VALUES (_device,_user,_parent,_time_begin,_time_end,_comment,_loop);
			SET result =1;
		ELSE 
			IF parent_check<>1 THEN
				SET result=-3;
			ELSE
					SET result=-4;
			END IF;
		END IF;
	ELSE
		INSERT INTO `mtc_schedule_tbl`(`schedule_device`, `schedule_user`, `schedule_parent`, `schedule_time_begin`, `schedule_time_end`, `schedule_comment`,`schedule_loop`)
		VALUES (_device,_user,_parent,_time_begin,_time_end,_comment,_loop);
		SET result =1;
	END IF;
ELSE
	SET result=-1;
END IF;

END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `p_insert_user`(IN `_fullname` text,IN `_user_name` varchar(100),IN `_user_pass` varchar(100),IN `_user_type` int,IN `_user_phone` varchar(100),IN `_user_email` varchar(100),IN `_user_finger_print` longblob ,out _result int)
BEGIN
	set _result=0;
	SELECT user_id INTO _result FROM mtc_user_tbl WHERE user_name=_user_name;
	if _result=0 THEN
		INSERT INTO `mtc_user_tbl`( `user_name`, `user_full_name`, `user_pass`, `user_type`,  `user_phone`, `user_email`,`user_finger_print`) 
		VALUES (_user_name,_fullname,_user_pass,_user_type,_user_phone,_user_email,_user_finger_print);
		set _result = LAST_INSERT_ID();
	
	ELSE 
		set _result=-1; 
	end if;

END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `p_reset_playlist_schedule`(IN `_s_id` int)
BEGIN
	#Routine body goes here...
	UPDATE mtc_schedule_tbl SET schedule_playlist=NULL WHERE schedule_id=_s_id; 
	
END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `p_save_info_device`(in _d_id int,In `_d_name` varchar(255),in _d_ip varchar(30),IN `_d_type` int,in _d_pass varchar(100),IN `_d_comment` text)
BEGIN
	#Routine body goes here...
	UPDATE `mtc_device_tbl` SET `device_name`=_d_name,`device_type`=_d_type, `device_comment`=_d_comment, device_ip=_d_ip,device_pass=_d_pass WHERE mtc_device_tbl.device_id=_d_id;
END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `p_save_info_media`(in `_media_id` int,IN `_media_name` varchar(255),IN `_media_url` varchar(255),IN `_media_type` int,IN `_media_comment` text,IN `_media_size` varchar(100),IN `_media_duration` varchar(100),IN `_media_user` int, in _media_md5 varchar(100))
BEGIN
	#Routine body goes here...
	UPDATE `mtc_media_tbl` SET `media_md5`=_media_md5, `media_name`=_media_name,`media_url`=_media_url,`media_type`=_media_type,`media_comment`=_media_comment,`media_size`=_media_size,`media_duration`=_media_duration ,`media_user`=_media_user WHERE media_id=_media_id;
END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `p_save_info_user`(IN `_user_id` int,IN `_fullname` text,IN `_email` varchar(100),IN `_phone` varchar(100),IN `_user_pass` varchar(100),IN `_user_type` int,IN `_user_content` text,IN `_user_finger_print` longblob ,out _result int)
BEGIN
	#Routine body goes here...
	UPDATE mtc_user_tbl SET user_full_name=_fullname, user_email=_email, user_phone=_phone, user_pass=_user_pass, user_type=_user_type, user_content=_user_content, user_finger_print=_user_finger_print WHERE user_id = _user_id;
	SET _result=1;
END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `p_save_permision_user`(IN `_user_id` int,IN `_permission` text)
BEGIN
	#Routine body goes here...
UPDATE mtc_user_tbl SET user_permision=_permission WHERE user_id= _user_id;
END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `p_save_playlist`(IN _playlist_id int,IN `_playlist_name` varchar(100),IN `_playlist_user` int,IN `_playlist_comment` text)
BEGIN
	#Routine body goes here... 
	UPDATE `mtc_playlist` SET `playlist_name`=`_playlist_name` ,`playlist_user`=`_playlist_user` ,`playlist_comment`=`_playlist_comment`  WHERE playlist_id=_playlist_id;
END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `p_save_playlist_details`(in _id int,IN `_media_id` int,IN `_playlist_id` int,IN `_time_begin` time,IN `_time_end` time)
BEGIN 
	#Routine body goes here...
	UPDATE `mtc_playlist_details` SET `playlist_id`=`_playlist_id` ,`media_id`=`_media_id` ,`time_begin`=`_time_begin` ,`time_end`=`_time_end`  WHERE `detail_id`=_id;
END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `p_save_status_device`(IN `_d_id` int,IN `_d_status` tinyint(4))
BEGIN
	#Routine body goes here...
	UPDATE mtc_device_tbl SET mtc_device_tbl.device_status=_d_status WHERE mtc_device_tbl.device_id=_d_id;
END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `p_save_status_media`(IN `_media_id` int,IN `_status` tinyint(2))
BEGIN
	#Routine body goes here....
	UPDATE mtc_media_tbl SET media_status= _status WHERE media_id=_media_id;
END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `p_save_status_schedule`(IN `_id` int,IN `_status` int)
BEGIN
	#Routine body goes here...
	if _status=0 THEN
		UPDATE `mtc_schedule_tbl` SET schedule_status=_status WHERE schedule_parent=_id;
	END if;
	UPDATE `mtc_schedule_tbl` SET schedule_status=_status WHERE schedule_id=_id;
END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `p_search_user`(IN `_key` varchar(100),IN `_from` int,IN `_number` int,OUT `_total` int)
BEGIN
	#Routine body goes here...
	SELECT COUNT(*) INTO _total FROM mtc_user_tbl WHERE user_name LIKE _key OR user_full_name LIKE _key OR user_email LIKE _key OR user_phone LIKE _key;
	SELECT *  FROM mtc_user_tbl WHERE user_name LIKE _key OR user_full_name LIKE _key OR user_email LIKE _key OR user_phone LIKE _key LIMIT _from,_number;
END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `p_set_playlist_to_schedule`(IN `_s_id` int,in _playlist_id int)
BEGIN
	#Routine body goes here...
	UPDATE mtc_schedule_tbl SET schedule_playlist=_playlist_id WHERE schedule_id=_s_id;
	
END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `p_set_status_user`(IN `_user_id` int,IN `_status` tinyint)
BEGIN
	#Routine body goes here...
	UPDATE mtc_user_tbl SET user_status = _status WHERE user_id=_user_id;
END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `P_TMP`()
BEGIN
	#Routine body goes here...
DECLARE _time_begin DATETIME;
DECLARE _time_end DATETIME;
DECLARE _device INT DEFAULT 1;
DECLARE _parent INT DEFAULT 2;
DECLARE tmp_result INT DEFAULT 0;

SET _time_begin='2015-05-07 04:59:59.000000'; 
SET _time_end='2015-05-07 06:59:59.000000'; 

SELECT schedule_id  FROM mtc_schedule_tbl WHERE 
				schedule_parent = _parent AND 
				((schedule_loop<>1 and ((schedule_time_begin<= _time_begin AND schedule_time_end>= _time_begin )
					OR (schedule_time_begin<= _time_end AND schedule_time_end>= _time_end) OR (schedule_time_begin>= _time_begin AND schedule_time_end<= _time_begin ))) 
					OR (schedule_loop=1 AND 
					((DATE_FORMAT(schedule_time_begin,'%T')<= DATE_FORMAT(_time_begin,'%T') AND DATE_FORMAT(schedule_time_end,'%T')>= DATE_FORMAT(_time_begin,'%T')) OR
					 (DATE_FORMAT(schedule_time_begin,'%T')<= DATE_FORMAT(_time_end,'%T') AND DATE_FORMAT(schedule_time_end,'%T')>= DATE_FORMAT(_time_end,'%T')) OR
					 (DATE_FORMAT(schedule_time_begin,'%T')>= DATE_FORMAT(_time_begin,'%T') AND DATE_FORMAT(schedule_time_end,'%T')<= DATE_FORMAT(_time_end,'%T'))
					)))
				AND schedule_status=1 AND schedule_device=_device LIMIT 0,1;
END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `p_type_media_all`()
BEGIN
	#Routine body goes here...
SELECT * FROM mtc_media_type_tbl; 
END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `p_update_schedule`(IN _id int,IN `_device` int,IN `_user` int,IN `_parent` int,IN `_time_begin` datetime,IN `_time_end` datetime,IN `_loop` tinyint ,IN `_comment` varchar(100),out result int)
BEGIN
	#Routine body goes here...
	DECLARE tmp_result INT; 
	DECLARE parent_time_begin DateTime;
	DECLARE parent_time_end DateTime;
	DECLARE parent_loop TINYINT(2) DEFAULT 0;
	DECLARE parent_check TINYINT(2) DEFAULT 0;
	SET result=0;
  SET tmp_result=0;

IF _time_begin<_time_end THEN
	IF _parent<>0 THEN
		SELECT schedule_time_begin, schedule_time_end,schedule_loop INTO parent_time_begin,parent_time_end,parent_loop FROM mtc_schedule_tbl WHERE schedule_id=_parent;
		IF parent_loop=0 THEN
			IF _loop=0 THEN
				IF parent_time_begin<= _time_begin AND parent_time_end>= _time_end THEN
					SET parent_check =1;
				END IF;
			ELSE
				IF DATE_FORMAT(parent_time_begin,'%Y%m%d')<DATE_FORMAT(parent_time_end,'%Y%m%d') THEN
					IF DATE_FORMAT(parent_time_begin,'%T')<=DATE_FORMAT(_time_begin,'%T') AND DATE_FORMAT(parent_time_end,'%T') >= DATE_FORMAT(_time_end,'%T') THEN
							SET parent_check =1;
					END IF;
				ELSE
					SET _loop=0;
					SET _time_begin= ADDTIME(DATE(parent_time_begin),TIME(_time_begin));
					SET _time_end= ADDTIME(DATE(parent_time_end),TIME(_time_end));
				END IF;
			END IF;
		ELSE
			IF _loop=0 THEN
				IF DATE_FORMAT(parent_time_begin,'%T')<=DATE_FORMAT(_time_begin,'%T') AND DATE_FORMAT(parent_time_end,'%T') >= DATE_FORMAT(_time_end,'%T') THEN
					SET parent_check =1;
				END IF;
			ELSE
				IF DATE_FORMAT(_time_begin,'%Y%m%d')=DATE_FORMAT(_time_end,'%Y%m%d') THEN
					IF DATE_FORMAT(parent_time_begin,'%T')<=DATE_FORMAT(_time_begin,'%T') AND DATE_FORMAT(parent_time_end,'%T') >= DATE_FORMAT(_time_end,'%T') THEN
						SET parent_check =1;
					END IF;
				END IF;
			END IF;
		END IF;
		IF parent_check=1 THEN
			IF _loop =0 THEN
				SELECT schedule_id INTO tmp_result FROM mtc_schedule_tbl WHERE 
				schedule_parent = _parent AND 
				((schedule_loop<>1 and ((schedule_time_begin<= _time_begin AND schedule_time_end>= _time_begin )
					OR (schedule_time_begin<= _time_end AND schedule_time_end>= _time_end) OR (schedule_time_begin>= _time_begin AND schedule_time_end<= _time_begin ))) 
					OR (schedule_loop=1 AND 
					((DATE_FORMAT(schedule_time_begin,'%T')<= DATE_FORMAT(_time_begin,'%T') AND DATE_FORMAT(schedule_time_end,'%T')>= DATE_FORMAT(_time_begin,'%T')) OR
					 (DATE_FORMAT(schedule_time_begin,'%T')<= DATE_FORMAT(_time_end,'%T') AND DATE_FORMAT(schedule_time_end,'%T')>= DATE_FORMAT(_time_end,'%T')) OR
					 (DATE_FORMAT(schedule_time_begin,'%T')>= DATE_FORMAT(_time_begin,'%T') AND DATE_FORMAT(schedule_time_end,'%T')<= DATE_FORMAT(_time_end,'%T'))
					)))
				AND schedule_status=1 AND schedule_device=_device AND schedule_id <> _id;
			ELSE
				SELECT schedule_id INTO tmp_result FROM mtc_schedule_tbl WHERE 
				schedule_parent = _parent AND 
					(DATE_FORMAT(schedule_time_end,'%Y%m%d')>= DATE_FORMAT(NOW(),'%Y%m%d') AND( (DATE_FORMAT(schedule_time_begin,'%T')<= DATE_FORMAT(_time_begin,'%T') AND DATE_FORMAT(schedule_time_end,'%T')>= DATE_FORMAT(_time_begin,'%T')) OR
					 (DATE_FORMAT(schedule_time_begin,'%T')<= DATE_FORMAT(_time_end,'%T') AND DATE_FORMAT(schedule_time_end,'%T')>= DATE_FORMAT(_time_end,'%T')) OR
					 (DATE_FORMAT(schedule_time_begin,'%T')>= DATE_FORMAT(_time_begin,'%T') AND DATE_FORMAT(schedule_time_end,'%T')<= DATE_FORMAT(_time_end,'%T'))))
				AND schedule_status=1 AND schedule_device=_device AND schedule_id <> _id;
			END IF;
		END IF;

		IF tmp_result=0 THEN
		  UPDATE mtc_schedule_tbl SET schedule_device=_device, schedule_user=_user,schedule_parent=_parent,schedule_time_begin=_time_begin,schedule_time_end=_time_end,schedule_comment=_comment,schedule_loop=_loop WHERE schedule_id=_id;
			SET result =1;
		ELSE 
			IF parent_check<>1 THEN
				SET result=-3;
			ELSE
					SET result=-4;
			END IF;
		END IF;
	ELSE
	UPDATE mtc_schedule_tbl SET schedule_device=_device, schedule_user=_user,schedule_parent=_parent,schedule_time_begin=_time_begin,schedule_time_end=_time_end,schedule_comment=_comment,schedule_loop=_loop WHERE schedule_id=_id;
		#INSERT INTO `mtc_schedule_tbl`(`schedule_device`, `schedule_user`, `schedule_parent`, `schedule_time_begin`, `schedule_time_end`, `schedule_comment`,`schedule_loop`)
		#VALUES (_device,_user,_parent,_time_begin,_time_end,_comment,_loop);
		SET result =1;
	END IF;
ELSE
	SET result=-1;
END IF;

END$$

--
-- Functions
--
CREATE DEFINER=`root`@`localhost` FUNCTION `fc_check_exist_schedule_in_date`(`_date` datetime) RETURNS tinyint(2)
BEGIN
	#Routine body goes here...
	DECLARE result int DEFAULT 0;
	DECLARE tmp_id int DEFAULT 0;
	SELECT schedule_id INTO tmp_id FROM mtc_schedule_tbl WHERE DATE_FORMAT(schedule_time_begin,"%Y%m%d") <= DATE_FORMAT(_date,"%Y%m%d")
	AND DATE_FORMAT(schedule_time_end,"%Y%m%d") >= DATE_FORMAT(_date,"%Y%m%d") AND schedule_status=1;
	if tmp_id<>0 THEN
		SET result=1;
	END if;
	RETURN result;
END$$

CREATE DEFINER=`root`@`localhost` FUNCTION `fc_get_finger_printer_user`(`_user_name` varchar(100)) RETURNS longblob
BEGIN
	#Routine body goes here... 
	DECLARE _fingerPrinter LONGBLOB DEFAULT NULL;
	SELECT user_finger_print INTO _fingerPrinter FROM mtc_user_tbl WHERE user_name=_user_name;
	RETURN _fingerPrinter;
END$$

CREATE DEFINER=`root`@`localhost` FUNCTION `fc_get_finger_printer_user_by_int`(`_user_id` int) RETURNS longblob
BEGIN
	#Routine body goes here...
	DECLARE result LONGBLOB;
	SET result=NULL;
	SELECT mtc_user_tbl.user_finger_print INTO result FROM mtc_user_tbl WHERE user_id=_user_id AND user_status=1;
	RETURN result;
END$$

CREATE DEFINER=`root`@`localhost` FUNCTION `fc_get_fullname_of_user_by_schedule_id`(`_schedule_id` int) RETURNS varchar(100) CHARSET utf8
BEGIN
	#Routine body goes here...
	DECLARE _full_name VARCHAR(100) DEFAULT '';
	SELECT A.user_full_name INTO _full_name FROM mtc_user_tbl A, mtc_schedule_tbl B WHERE A.user_id=B.schedule_user AND B.schedule_id=`_schedule_id` ;
	RETURN _full_name;
END$$

CREATE DEFINER=`root`@`localhost` FUNCTION `fc_get_full_name_by_text`(`_user_name` varchar(100)) RETURNS varchar(100) CHARSET utf8
BEGIN
	#Routine body goes here... 
	DECLARE tmp VARCHAR(100);
	SET tmp='';
	SELECT user_full_name INTO tmp FROM mtc_user_tbl WHERE user_name=_user_name; 
	RETURN tmp;
END$$

CREATE DEFINER=`root`@`localhost` FUNCTION `fc_get_full_name_id_user`(`_id_user` int) RETURNS varchar(100) CHARSET utf8
BEGIN
	#Routine body goes here...
	DECLARE tmp VARCHAR(100);
	SET tmp='';
	SELECT user_full_name INTO tmp FROM mtc_user_tbl WHERE user_id=_id_user; 
	RETURN tmp;
END$$

CREATE DEFINER=`root`@`localhost` FUNCTION `fc_get_hash_device`(`_id` int) RETURNS varchar(100) CHARSET utf8
BEGIN
	#Routine body goes here...
	DECLARE _hash VARCHAR(100) DEFAULT ''; 
	SELECT MD5(CONCAT(mtc_device_tbl.device_ip,mtc_device_tbl.device_pass)) INTO _hash FROM mtc_device_tbl WHERE mtc_device_tbl.device_id=_id AND mtc_device_tbl.device_status=1;
	RETURN _hash;
END$$

CREATE DEFINER=`root`@`localhost` FUNCTION `fc_get_hash_user`(`_user_id` int) RETURNS varchar(100) CHARSET utf8
BEGIN
	#Routine body goes here...
	DECLARE result VARCHAR(100);
	SET result='';
	SELECT MD5(CONCAT(user_name,user_pass)) INTO result FROM mtc_user_tbl WHERE user_id=_user_id AND user_status=1;
	RETURN result;
END$$

CREATE DEFINER=`root`@`localhost` FUNCTION `fc_get_ip_device`(`_id_device` int) RETURNS varchar(100) CHARSET utf8
BEGIN
	#Routine body goes here...
	DECLARE tmp VARCHAR(100);
	SELECT device_ip INTO tmp FROM mtc_device_tbl WHERE device_ip=_id_device;
	RETURN tmp;
END$$

CREATE DEFINER=`root`@`localhost` FUNCTION `fc_get_name_device_by_id`(`_id_device` int) RETURNS varchar(100) CHARSET utf8
BEGIN
	#Routine body goes here...
	DECLARE result VARCHAR(100);
	SET result='';
	SELECT device_name INTO result FROM mtc_device_tbl WHERE mtc_device_tbl.device_id=_id_device;
	RETURN result;
END$$

CREATE DEFINER=`root`@`localhost` FUNCTION `fc_get_type_user_by_id`(`_user_id` int) RETURNS int(11)
BEGIN
	#Routine body goes here...
	DECLARE type INT DEFAULT 0;
	SELECT user_type INTO type FROM mtc_user_tbl WHERE user_id=_user_id;
	RETURN type;
END$$

CREATE DEFINER=`root`@`localhost` FUNCTION `fc_get_user_name_by_id`(`_id_user` int) RETURNS varchar(100) CHARSET utf8
BEGIN
	#Routine body goes here...
	DECLARE tmp VARCHAR(100);
	SET tmp='';
	SELECT mtc_user_tbl.user_name INTO tmp FROM mtc_user_tbl WHERE user_id=_id_user; 
	RETURN tmp;
END$$

CREATE DEFINER=`root`@`localhost` FUNCTION `Fc_Login`(`_userName` varchar(100),`_password` varchar(100)) RETURNS int(11)
BEGIN
	#Routine body goes here...
	DECLARE tmp_id INT;
	SET tmp_id=0;
	SELECT user_id INTO tmp_id FROM mtc_user_tbl WHERE user_name=_userName And user_pass = _password and user_status=1;
	RETURN tmp_id;
END$$

CREATE DEFINER=`root`@`localhost` FUNCTION `fc_login_hash`(`_hash` varchar(100)) RETURNS int(11)
BEGIN
	#Routine body goes here...
	DECLARE result INT;
	SET result =0;
	SELECT user_id INTO result FROM mtc_user_tbl WHERE MD5(CONCAT(user_name,user_pass))=_hash AND user_status=1; 
	RETURN result;
END$$

DELIMITER ;

-- --------------------------------------------------------

--
-- Table structure for table `mtc_device_tbl`
--

CREATE TABLE IF NOT EXISTS `mtc_device_tbl` (
  `device_id` int(11) NOT NULL AUTO_INCREMENT,
  `device_name` varchar(255) NOT NULL,
  `device_ip` varchar(30) NOT NULL,
  `device_type` int(11) NOT NULL,
  `device_pass` varchar(100) NOT NULL,
  `device_status` tinyint(4) DEFAULT '1',
  `device_comment` text,
  `device_time` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`device_id`),
  KEY `type_device_fk` (`device_type`)
) ENGINE=InnoDB  DEFAULT CHARSET=utf8 AUTO_INCREMENT=3 ;

--
-- Dumping data for table `mtc_device_tbl`
--

INSERT INTO `mtc_device_tbl` (`device_id`, `device_name`, `device_ip`, `device_type`, `device_pass`, `device_status`, `device_comment`, `device_time`) VALUES
(1, 'demo 3', '192.168.10.1', 1, '202cb962ac59075b964b07152d234b70', 1, 'demo', '2015-03-05 07:31:26'),
(2, 'demo 2', '127.0.0.1', 2, '202cb962ac59075b964b07152d234b70', 1, 'dâda', '2015-03-05 08:18:44');

--
-- Triggers `mtc_device_tbl`
--
DROP TRIGGER IF EXISTS `delete_device`;
DELIMITER //
CREATE TRIGGER `delete_device` AFTER DELETE ON `mtc_device_tbl`
 FOR EACH ROW begin
DELETE FROM mtc_schedule_tbl WHERE mtc_schedule_tbl.schedule_device=old.device_id;
end
//
DELIMITER ;
DROP TRIGGER IF EXISTS `insert_device`;
DELIMITER //
CREATE TRIGGER `insert_device` AFTER INSERT ON `mtc_device_tbl`
 FOR EACH ROW begin
INSERT INTO `mtc_schedule_tbl`( `schedule_device`, `schedule_user`, `schedule_time_begin`, `schedule_time_end`, `schedule_comment`) VALUES (new.device_id,1,NOW(), DATE_ADD(NOW(),INTERVAL 1 YEAR),"Root");
end
//
DELIMITER ;

-- --------------------------------------------------------

--
-- Table structure for table `mtc_device_type`
--

CREATE TABLE IF NOT EXISTS `mtc_device_type` (
  `type_id` int(11) NOT NULL AUTO_INCREMENT,
  `type_name` varchar(100) NOT NULL,
  `type_icon` varchar(100) DEFAULT NULL,
  `type_comment` text,
  `type_time` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`type_id`)
) ENGINE=InnoDB  DEFAULT CHARSET=utf8 AUTO_INCREMENT=3 ;

--
-- Dumping data for table `mtc_device_type`
--

INSERT INTO `mtc_device_type` (`type_id`, `type_name`, `type_icon`, `type_comment`, `type_time`) VALUES
(1, 'LCD', NULL, 'LCDmàn hình LCD', '2015-03-04 15:54:01'),
(2, 'LED', NULL, 'màn hình LED', '2015-03-04 15:54:01');

-- --------------------------------------------------------

--
-- Table structure for table `mtc_media_tbl`
--

CREATE TABLE IF NOT EXISTS `mtc_media_tbl` (
  `media_id` int(11) NOT NULL AUTO_INCREMENT,
  `media_name` varchar(255) NOT NULL,
  `media_url` varchar(255) NOT NULL,
  `media_status` tinyint(2) NOT NULL DEFAULT '0',
  `media_type` int(11) NOT NULL,
  `media_comment` text,
  `media_size` varchar(100) DEFAULT NULL,
  `media_duration` varchar(100) DEFAULT NULL,
  `media_user` int(11) DEFAULT NULL,
  `media_md5` varchar(50) DEFAULT NULL,
  `media_time` timestamp NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`media_id`),
  KEY `media_type_fk` (`media_type`),
  KEY `media_user_fk` (`media_user`)
) ENGINE=InnoDB  DEFAULT CHARSET=utf8 AUTO_INCREMENT=13 ;

--
-- Dumping data for table `mtc_media_tbl`
--

INSERT INTO `mtc_media_tbl` (`media_id`, `media_name`, `media_url`, `media_status`, `media_type`, `media_comment`, `media_size`, `media_duration`, `media_user`, `media_md5`, `media_time`) VALUES
(3, 'Camera', 'rtsp://192.168.10.213:554/user=admin&password=&channel=1&stream=0.sdp?real_stream--rtp-caching=100', 1, 2, '', '0kb', '00:00:00', 4, NULL, '2015-05-09 04:08:36'),
(10, 'demo', 'ftp://127.0.0.1/Medias/video_635669780879475546.mp4', 1, 1, '', '13789kb', '00:00:55', 4, '5f246d071db01ed532ea349e01f94a48', '2015-05-11 14:58:20'),
(11, 'demo 2', 'ftp://127.0.0.1/Medias/video_635669781201618951.mp4', 1, 1, '', '7833kb', '00:01:26', 4, '9c58b640abbaa4cf5eca938f0d6e30b5', '2015-05-14 17:16:48'),
(12, 'admin', 'ftp://127.0.0.1/Medias/video_635672458384295984.mp4', 1, 1, 'admin', '27311kb', '00:00:47', 1, 'eacf8a6fe00ebec670b55358ebfaf240', '2015-05-14 17:18:07');

-- --------------------------------------------------------

--
-- Table structure for table `mtc_media_type_tbl`
--

CREATE TABLE IF NOT EXISTS `mtc_media_type_tbl` (
  `type_id` int(11) NOT NULL AUTO_INCREMENT,
  `type_name` varchar(100) NOT NULL,
  `type_code` varchar(5) NOT NULL,
  `type_icon` varchar(100) DEFAULT NULL,
  `type_comment` text,
  `type_time` timestamp NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`type_id`)
) ENGINE=InnoDB  DEFAULT CHARSET=utf8 AUTO_INCREMENT=3 ;

--
-- Dumping data for table `mtc_media_type_tbl`
--

INSERT INTO `mtc_media_type_tbl` (`type_id`, `type_name`, `type_code`, `type_icon`, `type_comment`, `type_time`) VALUES
(1, 'Video', 'FILE', 'fa-puzzle-piece', NULL, '2015-05-09 04:31:21'),
(2, 'Camera', 'URL', 'fa-video-camera', NULL, '2015-05-09 04:32:58');

-- --------------------------------------------------------

--
-- Table structure for table `mtc_plan`
--

CREATE TABLE IF NOT EXISTS `mtc_plan` (
  `plan_id` int(11) NOT NULL AUTO_INCREMENT,
  `playlist_id` int(11) NOT NULL,
  `schedule_id` int(11) NOT NULL,
  `plan_date` timestamp NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`plan_id`),
  KEY `fk_plan_schedule` (`schedule_id`),
  KEY `fk_plan_playlist` (`playlist_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 AUTO_INCREMENT=1 ;

-- --------------------------------------------------------

--
-- Table structure for table `mtc_playlist`
--

CREATE TABLE IF NOT EXISTS `mtc_playlist` (
  `playlist_id` int(11) NOT NULL AUTO_INCREMENT,
  `playlist_name` varchar(100) NOT NULL,
  `playlist_user` int(11) NOT NULL,
  `playlist_comment` text,
  `playlist_status` tinyint(2) NOT NULL DEFAULT '1',
  `playlist_default` tinyint(2) NOT NULL DEFAULT '0',
  `playlist_datetime` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`playlist_id`),
  KEY `fk_playlist_user` (`playlist_user`)
) ENGINE=InnoDB  DEFAULT CHARSET=utf8 AUTO_INCREMENT=5 ;

--
-- Dumping data for table `mtc_playlist`
--

INSERT INTO `mtc_playlist` (`playlist_id`, `playlist_name`, `playlist_user`, `playlist_comment`, `playlist_status`, `playlist_default`, `playlist_datetime`) VALUES
(2, 'demo 2', 4, 'demo 2', 1, 1, '2015-05-07 20:53:07'),
(3, 'demo 3', 4, 'demo', 1, 0, '2015-05-07 21:09:59'),
(4, 'Default', 1, 'default', 1, 1, '2015-05-14 17:17:57');

--
-- Triggers `mtc_playlist`
--
DROP TRIGGER IF EXISTS `insert_default_playlist`;
DELIMITER //
CREATE TRIGGER `insert_default_playlist` BEFORE INSERT ON `mtc_playlist`
 FOR EACH ROW begin
if new.playlist_default=1  then
 UPDATE mtc_playlist SET playlist_default=0 WHERE playlist_user=new.playlist_user;
end if;
end
//
DELIMITER ;

-- --------------------------------------------------------

--
-- Table structure for table `mtc_playlist_details`
--

CREATE TABLE IF NOT EXISTS `mtc_playlist_details` (
  `detail_id` int(11) NOT NULL AUTO_INCREMENT,
  `playlist_id` int(11) NOT NULL,
  `media_id` int(11) NOT NULL,
  `time_begin` time NOT NULL,
  `time_end` time NOT NULL,
  `detail_date` timestamp NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`detail_id`),
  KEY `fk_detail_media` (`media_id`),
  KEY `fk_detail_playlist` (`playlist_id`)
) ENGINE=InnoDB  DEFAULT CHARSET=utf8 AUTO_INCREMENT=6 ;

--
-- Dumping data for table `mtc_playlist_details`
--

INSERT INTO `mtc_playlist_details` (`detail_id`, `playlist_id`, `media_id`, `time_begin`, `time_end`, `detail_date`) VALUES
(4, 2, 10, '06:01:28', '23:59:59', '2015-05-11 17:04:38'),
(5, 4, 12, '00:00:00', '23:59:59', '2015-05-14 17:18:27');

-- --------------------------------------------------------

--
-- Table structure for table `mtc_schedule_tbl`
--

CREATE TABLE IF NOT EXISTS `mtc_schedule_tbl` (
  `schedule_id` int(11) NOT NULL AUTO_INCREMENT,
  `schedule_device` int(11) NOT NULL,
  `schedule_user` int(11) NOT NULL,
  `schedule_playlist` int(2) DEFAULT NULL,
  `schedule_parent` int(11) NOT NULL DEFAULT '0',
  `schedule_time_begin` datetime NOT NULL,
  `schedule_time_end` datetime NOT NULL,
  `schedule_loop` tinyint(2) NOT NULL DEFAULT '0',
  `schedule_status` tinyint(2) NOT NULL DEFAULT '1',
  `schedule_comment` text,
  `schedule_date` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`schedule_id`),
  KEY `schedule_user_fk` (`schedule_user`),
  KEY `schedule_device_fk` (`schedule_device`),
  KEY `fk_playlist_schedule` (`schedule_playlist`)
) ENGINE=InnoDB  DEFAULT CHARSET=utf8 AUTO_INCREMENT=8 ;

--
-- Dumping data for table `mtc_schedule_tbl`
--

INSERT INTO `mtc_schedule_tbl` (`schedule_id`, `schedule_device`, `schedule_user`, `schedule_playlist`, `schedule_parent`, `schedule_time_begin`, `schedule_time_end`, `schedule_loop`, `schedule_status`, `schedule_comment`, `schedule_date`) VALUES
(1, 1, 1, NULL, 0, '2015-05-05 18:28:35', '2016-05-05 18:28:35', 0, 1, 'Root', '2015-05-05 11:29:47'),
(2, 1, 3, NULL, 1, '2015-05-07 00:00:00', '2015-05-08 23:59:59', 0, 1, 'demo', '2015-05-07 16:50:10'),
(6, 1, 4, 3, 2, '2015-05-07 04:17:59', '2015-05-08 05:18:59', 0, 1, 'demo', '2015-05-10 13:22:34'),
(7, 1, 4, NULL, 1, '2015-05-09 00:18:13', '2015-05-10 23:19:13', 0, 1, 'demo', '2015-05-09 08:18:41');

-- --------------------------------------------------------

--
-- Table structure for table `mtc_type_user_tbl`
--

CREATE TABLE IF NOT EXISTS `mtc_type_user_tbl` (
  `type_id` int(11) NOT NULL AUTO_INCREMENT,
  `type_name` varchar(100) NOT NULL,
  `default_permision` text,
  `type_icon` varchar(50) DEFAULT NULL,
  `type_comment` text NOT NULL,
  `type_status` tinyint(2) NOT NULL DEFAULT '1',
  PRIMARY KEY (`type_id`)
) ENGINE=InnoDB  DEFAULT CHARSET=utf8 AUTO_INCREMENT=3 ;

--
-- Dumping data for table `mtc_type_user_tbl`
--

INSERT INTO `mtc_type_user_tbl` (`type_id`, `type_name`, `default_permision`, `type_icon`, `type_comment`, `type_status`) VALUES
(1, 'Administrator', '<?xml version="1.0" encoding="utf-16"?><Permision xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema"><mana_user>true</mana_user><view_all_media>true</view_all_media><mana_schedule>true</mana_schedule><confirm_media>true</confirm_media><mana_device>true</mana_device></Permision>', '\\uf21b', 'admin', 1),
(2, 'User', '<?xml version="1.0" encoding="utf-16"?><Permision xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema"><mana_user>false</mana_user><view_all_media>false</view_all_media><mana_schedule>false</mana_schedule><confirm_media>false</confirm_media></Permision>', '\\uf007', '', 1);

-- --------------------------------------------------------

--
-- Table structure for table `mtc_user_tbl`
--

CREATE TABLE IF NOT EXISTS `mtc_user_tbl` (
  `user_id` int(11) NOT NULL AUTO_INCREMENT,
  `user_name` varchar(100) NOT NULL,
  `user_full_name` text NOT NULL,
  `user_pass` varchar(100) NOT NULL,
  `user_finger_print` longblob,
  `user_type` int(11) NOT NULL,
  `user_status` tinyint(2) NOT NULL DEFAULT '1',
  `user_permision` text NOT NULL,
  `user_content` text NOT NULL,
  `user_phone` varchar(100) DEFAULT NULL,
  `user_email` varchar(100) DEFAULT NULL,
  `user_time` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`user_id`),
  KEY `user_type_fk` (`user_type`)
) ENGINE=InnoDB  DEFAULT CHARSET=utf8 AUTO_INCREMENT=5 ;

--
-- Dumping data for table `mtc_user_tbl`
--

INSERT INTO `mtc_user_tbl` (`user_id`, `user_name`, `user_full_name`, `user_pass`, `user_finger_print`, `user_type`, `user_status`, `user_permision`, `user_content`, `user_phone`, `user_email`, `user_time`) VALUES
(1, 'admin', 'Admin', '21232f297a57a5a743894a0e4a801fc3', 0x00f83c01c82ae3735ca45e24935f70d5c234ac578cdbbe8450899738d96ab136e3136143bf7e7e19e5cef2d66407aae4b61f48888c9f60a70b9c5d3413950eae48a7f468bf7ccdaf361f837f3c775a2dcd16dca31ce3ec6a4c22ec62aa9de523a086e0b852da8a5c42551b1fbf4e76ae935fc4c968a3ed2558ea53eb7df4aa2e7683de8bd68668569ced081d9e6a27c6486b09f15813e97a337e2c6990ff61387b33a5ce1e9197869937c2596c0522280ebbda31f78c0e5436cfcb1b633379c7ece3c61a7a325f3296992e3f7376ca54d930f9c97237068c36e974ced3352e1d625774c297cfc6a77aa7fb7fc7928c71281ea87bf7728df642e568ba1608e3740d1af043e4fcde04495fd7d5d5312170d827bef622149c7ebf9873c8e66691d47139c50d166c78cce7f4790a9b8e191ff0de0f2efb2d59a9626793ec34d577186f00f82c01c82ae3735cb05b38587206f517cb48f9fcbca240ebf772dde1718a2dec29f8e51be1ad4c554c30f13bd46da140ade47f843de79d1352ea2066be88043eae0aeed83665e363be7642443664fbcfa556442757a06d4bf5bdb5b008ec5cdeba151f76cc27f2fea700b13c63292898fd0ff501a356073dd3b66eafb58c2ce74b01f59402d97475e03ad9e19d2419fbc98d454583642daa5326d28d645b92973bf30aef2188662834c47b8aa8441e7edbcbfe6bf74cf5e17841b5706d44995af6cfeae8e283cbb165fc974d1eb7d573564bc0434b9cbfb66b726520752d102f34a4c1809733a387d5abc9d7d82fe879876d8672205128b94eb12a10eeda66c87107aefacf5799a7dc615f1da857456811cd87b52069d240a1e7a5afd38c31262ef91d8ef4ac62219570d1ca9dd5af6f00f85a01c82ae3735ca25d274ecab7ffca82cd62ad2cbbb4fe8dd434d4597bf7b474489fc692a16174024309fdcca6e31514d79a11feff4c5a997473f36ee87d4dd03488c25b376a92f079667330250ffdb0f2983d975b506782a3fece0aabadb94199fa956731f5587b360ddc73a384314d7ac34a85f91fb58a419fd7cb1c0af72784e6932d260a1c20ef77980d9fdd8e484f392fd7c9204ed80f3a7aab1d209cf02f8fbc141f8284d7c0533f109bb40d1ba1a6d7fa946473c755988e4f57ff76af08322d1f304b0a1a2947da6f00aab0025ce30d232d9c51c5620f3021d0dc9b402a78303e2ed39e4d516ef8343c45d22d7e5304f9e3f0a34ab2692b8b65b5013254c9baaa233545857976048952151204afb16df15eaa3caea2fb7f18d36e58dc3b1eb042d33398e54db27554abd985cc10d9dbff1f59e76a86495999466fa41854c76d5c6d4f39a494e72a6f8c64962fdcddf722fa04c50a5332bff06f00e81c01c82ae3735cb05a24cafacfda367d0695e96de1bc5c358d3bd57185a460be7871176d181cb28fe0592de7dea540643e5ed0afc1af5682fb8aa6ce0c2cd4ccf8bfccdafe16582b79e29c9f160b0322d18d52b868ba19ec30c285851ec10a363a668cb0feba5e8ed945eee66590851fae1d4c638808dc2e17a2de1b02da259ef5a138c71158183c7fc7ba7a54080f21f7d15eac2f3554b09f96b5c82795cc48384c8dcb28eaeb04269068024ae4252c6b71529e8baf1e01a52137009e2485b93b705bf725f9c57f93aaa53896a1f1c1d8efaf23cacb1a22ac6f4384592f686628c61022070bed99c4b36a4aed4d4ea57e50b35e7c2d2ce8a12ba6eccc8aa23a2a65b833fa5f1cc5ce0577913d666be866268c59d5f5721ced2cbeb6c4c06f000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000, 1, 1, '<?xml version="1.0" encoding="utf-16"?><Permision xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema"><mana_user>true</mana_user><view_all_media>true</view_all_media><mana_schedule>true</mana_schedule><confirm_media>true</confirm_media><mana_device>true</mana_device></Permision>', 'demo', '09796232175', 'admin@gmail.com', '2015-05-06 09:26:52'),
(3, 'giang.phan', 'Phan Thanh Giang', 'e762cb5fff16ea2f20faf9b67d83522a', 0x00f8ff00c82ae3735cd1540c49e5013efeb6a8929216031814e958bae10bdff6bd8bb1751e013e2d86f81e56748976135d49bc68979d33ab75abf0d6732226d100b07b99e6b5657aa592ba429edc3efc9c16c887ba8620115332b327892c96700e8a302b8fca3111189730fd26d85791fdd552523af07e32442200635529118190b7b9970b77170333c447c0764d8ead7cd8e16e4a27fd559832da030bf7048edbcf377ade73d2707b64a692bbb76976226719a30cb1094fb6d970205b2ee105d21663b03a6402ccdcbeb493b302d92c3f20fb650fcb88b45d176aad5bf4329fbf8923fd03709a2f492f54f84ded328a67ded4c4ef59b97ea6fff80efa6e926f8269346f00f8f200c82ae3735ca05366f57933438f681258f6ce8840b9950ee1a75a1b2cb104f4822709314f9afc9f9288c90e1268ea5acc056671bb992bb6dcab60534da6359b05aedd5901919142fd72497136c45df9b28ba90ebf3e23cff90da34b9a593cbee4ad883c2bd9049c6ef689cd2469bad438c75eeb03c07bae8ca32d17322019628acc44e9c711e578c5f2d293aa05bd05614baa470858fdc0e350b4d9d372078e2d40482ee0c151634b95e508d539036891fe3e29b5ca22b0f05ea7dcb219ec93b890df8d195377c33a4255e856941bb1ba21dfe83ceaf5a09223624a5855141d3a5305e052e0bf4a54fe430e0c19173ac721486f00f80b01c82ae3735cf5570b71bd94c62bf32180a8dadf6e60fb4215201a28a13684d0afcad02b447841c81b166a324e8d9706bb69d65b800200bd5a5949fe576a7c3d6ad53a2a3f38b07754045a364eb290c0ed0f56398631486183f866d6c7dc5dc52c2794bc13e4308b732e3c0fc6db8d02bd94829a0f7ce875c1cb00acc84508e4763bd0eea6b104ba825f1743f9f6906c6b29af7a7055395ff51b560be07f124f91cdccf8fc64ba4d767b2bebb19854dbd68ce269f098bb9af5046474f60df22deb304c54147503aff42a9a90eac5c43effcc6a9747ecb82dc1296012d6cd90a4e09b9c9d293e1ee726ecb459ac125fc8ad453e6649a7099cbc20afa5738306176ecc694774a63259b42643a66f00e8fc00c82ae3735cab55150d44495e2c9d23b39ad5860f4d9fe2192b1b386b9265c88513278f0acbb1bc2eac091627210cf20b9fe804f26729224d1e9abc9f40cb4339668be0f44495658af793a0ff360a6f6ffa4d83cf67ff5495f8936f122f64e53cde015c2e27540a243007631d1fa245ee5998c8af4cd8dd093cfc1246bd4ae06db3c356c349e8ed99136211a5b7a44c6a2c5f2fe560292278f57ae2f42e84df8083e5dfce512ff0409703f92c58dd89ccf89ed5c2d222214802408c9a9271508e30dca33d5068f1be4c0fdf57313be4e472c2ff5bcaed73b4da39a41741b4c181c3e2d907c6a0be3a37044b465b8c26d2e958ad509b24bf981d318f8c6f0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000, 1, 1, '<?xml version="1.0" encoding="utf-16"?><Permision xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema"><mana_user>true</mana_user><view_all_media>true</view_all_media><mana_schedule>true</mana_schedule><confirm_media>true</confirm_media><mana_device>true</mana_device></Permision>', '', '0979632175', 'giang.phan@alta.com.vn', '2015-05-07 00:24:50'),
(4, 'user', 'User', 'e807f1fcf82d132f9bb018ca6738a19f', 0x00f80201c82ae3735ce35262998509a01f50ab04227dee8bedb9de777af5e6c9fb3a101cb8ac150f36c1ebbddb47b67cca5718c6c017368ee984174ffeb0ea57df52d2f8a89b600b2b23b6886b01d54df91809c803611432b552270c0700aa1e594345f16a0c20ff4e5fb16f9c0c09114f605f815236b746352ae99eefe9682e6f399d1928ba6b572fe095e89cc70a802c379dea0157045119c00f872e09b9851e6a63e68b64a0d74a0bc895c04ea771d88114ce0ad404877fd3e691cadd97b935647ae8781060a527b3a966075e846a30a1fa79263e9188b2d95f3777d6a080155b79184a11c68c2458a0c6db26e3caeecb62daec929ecfcd18edc3cfa8bfd5981ab42f538a6f00f84801c82ae3735c37537be03935c9e8a87c4c152ec00923ce79b2525adeb34d9aadbcb2150a8f567638dfb6f938e2fda5a95260aff14ab43cee3a3784900e1b0f7b34b68c71b8814b51e282c0f814652c1648dd4bae4a66a518532ea58d713cf1e2406a63e618db19d1bd0f7aafa4db225185dbcf172b0f0a592d1a0a6c3c0efc14c8ea88a032afffdd294a93e78357924be4e877dee75235f8c997397e53fcb58d33b3b68fd4d23b4aeff6607bf0d860ea902d2a877ed61f4503a1e11e8c6af9b6af10f335a5e62f5a441009e8f69a014474a69e48e55a5c86c046b9e4d031e9b449d8ed5a47ce98fa87d718705ff5dbd9810bb3cecd10f303bc079717dff6a71f7b39b92bc204d51be75578a0164e605e4c5e9c37df499bf845d92362dfbe87590bc8f32bd6dfb2a0df1ef72c57d4af384da7947f79ecdb20a91c2b5bca449ce469d9b75c31091a662f6f00f84801c82ae3735c315206fc7e89dee473bbd6e81173417caf57568b658cfb1664c5175d43bd10b504a788de421fcb76e5764d80d120371a053537d4f6721af5021bc810553b866a97510b5a9c704137f057972e0ab75407d65d208ffc2ec83883dcc6dec3da15e0ee5b1933583aff208538b983ea06dbb7a5f3470bc506ef5db62675456bd912664c8bead92e6f21685ea1697ebde1b09843a2530707a80c17cace3df7e070325486938df59a0585ce29a39211e0a327b359080332f55b4e9929ab718c4bccec7b62ca337138004059f52e667db2533a90ddcf47f8a818caa96a5b30a01e4c7bcf7abee7158499786d2b194c1276c1e5582ac92aace0367c36fabc11310dbf42da5ac535aba223829cd6dfae13018c653a01a8a2b244c2513f171e27b02ecbba8801c7ff1925622c6456ad2a538b93b0f68ef8683de58fc9f69e68c81d820359ecf11d9d6f00e8ff00c82ae3735c165103d70ebd855f4eb93e208f7b21ab78e8da5e7eb4941a6b7abb51af148dbfdfbd488addc6db7d5b4cb41a960e5a718d1f25afd3c41bb2078deed7803ebca7d75845cf924a396189e4cea25588796cabc70aec52d04c772e01889879671aa36c8860f625d9e70eb5f2ff122a087276ccc8075993964fdb80e657083a096f9cf793621f8c8d2a04dfcf031fcae5f9460ed266288d756887f199f42976b21873d4934dcac577bd2aa0446404e98096a4020fc0275417c446dc3b6e99c6adac2951b1ce08844f32edeeea2eba0490329dffc307a16a136601888213c5909c4ba1f23388ff1a99606014f36ef52824a8bc1692c803af60c9761df66f0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000, 2, 1, '<?xml version="1.0" encoding="utf-16"?><Permision xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema"><mana_user>false</mana_user><view_all_media>false</view_all_media><mana_schedule>false</mana_schedule><confirm_media>false</confirm_media></Permision>', '', '1234567890', 'user@gmail.com', '2015-05-07 12:55:50');

--
-- Triggers `mtc_user_tbl`
--
DROP TRIGGER IF EXISTS `insert_triger`;
DELIMITER //
CREATE TRIGGER `insert_triger` BEFORE INSERT ON `mtc_user_tbl`
 FOR EACH ROW begin
declare _permision text;
declare _result int default 0;
SELECT user_id INTO _result FROM mtc_user_tbl WHERE user_name=new.user_name;
if _result<>0 then
 SIGNAL SQLSTATE '12345'
            SET MESSAGE_TEXT = 'Tên đăng nhập đã tồn tại';
else
if(new.user_permision="") then
select default_permision into _permision from mtc_type_user_tbl where type_id = new.user_type;
set new.user_permision=_permision;
end if;
end if;
end
//
DELIMITER ;
DROP TRIGGER IF EXISTS `update_triger`;
DELIMITER //
CREATE TRIGGER `update_triger` BEFORE UPDATE ON `mtc_user_tbl`
 FOR EACH ROW begin
declare _result int default 0;
if new.user_name<>old.user_name then
SELECT user_id INTO _result FROM mtc_user_tbl WHERE user_name=new.user_name;
if _result<>0 then
 SIGNAL SQLSTATE '12345'
            SET MESSAGE_TEXT = 'Tên đăng nhập đã tồn tại';
end if;
end if;
end
//
DELIMITER ;

--
-- Constraints for dumped tables
--

--
-- Constraints for table `mtc_device_tbl`
--
ALTER TABLE `mtc_device_tbl`
  ADD CONSTRAINT `type_device_fk` FOREIGN KEY (`device_type`) REFERENCES `mtc_device_type` (`type_id`) ON DELETE CASCADE ON UPDATE CASCADE;

--
-- Constraints for table `mtc_media_tbl`
--
ALTER TABLE `mtc_media_tbl`
  ADD CONSTRAINT `media_type_fk` FOREIGN KEY (`media_type`) REFERENCES `mtc_media_type_tbl` (`type_id`) ON DELETE CASCADE ON UPDATE CASCADE,
  ADD CONSTRAINT `media_user_fk` FOREIGN KEY (`media_user`) REFERENCES `mtc_user_tbl` (`user_id`) ON DELETE NO ACTION ON UPDATE CASCADE;

--
-- Constraints for table `mtc_plan`
--
ALTER TABLE `mtc_plan`
  ADD CONSTRAINT `fk_plan_playlist` FOREIGN KEY (`playlist_id`) REFERENCES `mtc_playlist` (`playlist_id`) ON DELETE CASCADE ON UPDATE CASCADE,
  ADD CONSTRAINT `fk_plan_schedule` FOREIGN KEY (`schedule_id`) REFERENCES `mtc_schedule_tbl` (`schedule_id`) ON DELETE CASCADE ON UPDATE CASCADE;

--
-- Constraints for table `mtc_playlist`
--
ALTER TABLE `mtc_playlist`
  ADD CONSTRAINT `fk_playlist_user` FOREIGN KEY (`playlist_user`) REFERENCES `mtc_user_tbl` (`user_id`) ON DELETE CASCADE ON UPDATE CASCADE;

--
-- Constraints for table `mtc_playlist_details`
--
ALTER TABLE `mtc_playlist_details`
  ADD CONSTRAINT `fk_detail_media` FOREIGN KEY (`media_id`) REFERENCES `mtc_media_tbl` (`media_id`) ON DELETE CASCADE ON UPDATE CASCADE,
  ADD CONSTRAINT `fk_detail_playlist` FOREIGN KEY (`playlist_id`) REFERENCES `mtc_playlist` (`playlist_id`) ON DELETE CASCADE ON UPDATE CASCADE;

--
-- Constraints for table `mtc_schedule_tbl`
--
ALTER TABLE `mtc_schedule_tbl`
  ADD CONSTRAINT `fk_playlist_schedule` FOREIGN KEY (`schedule_playlist`) REFERENCES `mtc_playlist` (`playlist_id`) ON DELETE CASCADE ON UPDATE CASCADE,
  ADD CONSTRAINT `schedule_device_fk` FOREIGN KEY (`schedule_device`) REFERENCES `mtc_device_tbl` (`device_id`) ON DELETE CASCADE ON UPDATE CASCADE,
  ADD CONSTRAINT `schedule_user_fk` FOREIGN KEY (`schedule_user`) REFERENCES `mtc_user_tbl` (`user_id`) ON DELETE CASCADE ON UPDATE CASCADE;

--
-- Constraints for table `mtc_user_tbl`
--
ALTER TABLE `mtc_user_tbl`
  ADD CONSTRAINT `user_type_fk` FOREIGN KEY (`user_type`) REFERENCES `mtc_type_user_tbl` (`type_id`) ON DELETE CASCADE ON UPDATE CASCADE;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
