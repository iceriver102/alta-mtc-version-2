-- phpMyAdmin SQL Dump
-- version 4.0.4
-- http://www.phpmyadmin.net
--
-- Host: 127.0.0.1
-- Generation Time: Mar 05, 2015 at 01:11 PM
-- Server version: 5.5.32
-- PHP Version: 5.4.16

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8 */;

--
-- Database: `mtc_db`
--
CREATE DATABASE IF NOT EXISTS `mtc_db` DEFAULT CHARACTER SET utf8 COLLATE utf8_general_ci;
USE `mtc_db`;

DELIMITER $$
--
-- Procedures
--
CREATE DEFINER=`root`@`localhost` PROCEDURE `InfoUser`(IN `_user_id` int)
BEGIN
	#Routine body goes here...
	SELECT * FROM mtc_user_tbl WHERE user_id=_user_id;
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

CREATE DEFINER=`root`@`localhost` PROCEDURE `p_delete_user`(IN `_user_id` int)
BEGIN
	#Routine body goes here...
DELETE FROM `mtc_user_tbl` WHERE `user_id`=_user_id;
END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `p_find_device`(IN `_key` varchar(100),IN `_from` int,IN `_number` int,OUT `_total` int)
BEGIN
	#Routine body goes here...
	SELECT COUNT(*) INTO _total FROM mtc_device_tbl WHERE (mtc_device_tbl.device_comment LIKE _key OR mtc_device_tbl.device_name LIKE _key OR mtc_device_tbl.device_ip LIKE _key);
	SELECT * FROM mtc_device_tbl WHERE (mtc_device_tbl.device_comment LIKE _key OR mtc_device_tbl.device_name LIKE _key OR mtc_device_tbl.device_ip LIKE _key) LIMIT _from,_number;
END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `p_find_media`(IN `_key` varchar(255),IN `_user_id` int,IN _type_id int,IN `_from` int,IN `_number` int,OUT `_total` int)
BEGIN
	#Routine body goes here...
	SET _total=0; 
	IF _user_id=-1 THEN		
		SELECT COUNT(*) INTO _total FROM mtc_media_tbl WHERE (mtc_media_tbl.media_name LIKE _key OR mtc_media_tbl.media_comment LIKE _key) AND mtc_media_tbl.media_type=_type_id;
		SELECT *  FROM mtc_media_tbl WHERE (mtc_media_tbl.media_name LIKE _key OR mtc_media_tbl.media_comment LIKE _key ) AND mtc_media_tbl.media_type=_type_id LIMIT _from,_number;
	ELSE
		SELECT COUNT(*) INTO _total FROM mtc_media_tbl WHERE (mtc_media_tbl.media_name LIKE _key OR mtc_media_tbl.media_comment LIKE _key )AND mtc_media_tbl.media_user=_user_id AND mtc_media_tbl.media_type=_type_id;
		SELECT *  FROM mtc_media_tbl WHERE (mtc_media_tbl.media_name LIKE _key OR mtc_media_tbl.media_comment LIKE _key) AND mtc_media_tbl.media_user=_user_id AND mtc_media_tbl.media_type=_type_id LIMIT _from,_number;
	END IF;
END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `p_get_all_device`(IN `_from` int,IN `_number` int,OUT `_total` int)
BEGIN
	#Routine body goes here...
	SET _total=0;
	SELECT COUNT(*) INTO _total FROM mtc_device_tbl;
	SELECT * FROM mtc_device_tbl LIMIT _from,_number;
END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `p_get_all_media`(IN `_user_id` int,IN `_media_type` int,IN `_from` int,IN `_number` int,OUT `_total` int)
BEGIN
	#Routine body goes here...
	if _user_id<>-1 THEN
	SELECT COUNT(*) INTO _total FROM mtc_media_tbl WHERE mtc_media_tbl.media_user= _user_id and media_type=_media_type; 
	SELECT * FROM mtc_media_tbl WHERE mtc_media_tbl.media_user = _user_id and media_type=_media_type LIMIT _from,_number;
ELSE 
	SELECT COUNT(*) INTO _total FROM mtc_media_tbl WHERE  media_type=_media_type; 
	SELECT * FROM mtc_media_tbl WHERE  media_type=_media_type LIMIT _from,_number;
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

CREATE DEFINER=`root`@`localhost` PROCEDURE `p_insert_device`(In `_d_name` varchar(255),in _d_ip varchar(30),IN `_d_type` int,IN `_d_comment` text,out _d_id int)
BEGIN
	#Routine body goes here...
	SET _d_id=-1;
	INSERT INTO `mtc_device_tbl`(`device_name`, `device_type`,  `device_comment`, device_ip) VALUES (_d_name,_d_type,_d_comment,_d_ip);
	SET _d_id= LAST_INSERT_ID(); 
END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `p_insert_media`(IN `_media_name` varchar(255),IN `_media_url` varchar(255),IN `_media_type` int,IN `_media_comment` text,IN `_media_size` varchar(100),IN `_media_duration` varchar(100),IN `_media_user` int,OUT `_media_id` int)
BEGIN
	#Routine body goes here...
SET _media_id=-1;
INSERT INTO `mtc_media_tbl`( `media_name`, `media_url`, `media_type`, `media_comment`, `media_size`, `media_duration`, `media_user`) 
VALUES (_media_name,_media_url,_media_type,_media_comment,_media_size,_media_duration,_media_user); 
SET _media_id= LAST_INSERT_ID();
END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `p_insert_user`(IN `_fullname` text,IN `_user_name` varchar(100),IN `_user_pass` varchar(100),IN `_user_type` int,IN `_user_phone` varchar(100),IN `_user_email` varchar(100),out _result int)
BEGIN
	#Routine body goes here...
	set _result=0;
	SELECT user_id INTO _result FROM mtc_user_tbl WHERE user_name=_user_name;
	if _result=0 THEN
		INSERT INTO `mtc_user_tbl`( `user_name`, `user_full_name`, `user_pass`, `user_type`,  `user_phone`, `user_email`) 
		VALUES (_user_name,_fullname,_user_pass,_user_type,_user_phone,_user_email);
		set _result = LAST_INSERT_ID();
	ELSE 
		set _result=-1; 
	end if;

END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `p_save_info_device`(in _d_id int,In `_d_name` varchar(255),in _d_ip varchar(30),IN `_d_type` int,IN `_d_comment` text)
BEGIN
	#Routine body goes here...
	UPDATE `mtc_device_tbl` SET `device_name`=_d_name,`device_type`=_d_type, `device_comment`=_d_comment, device_ip=_d_ip WHERE mtc_device_tbl.device_id=_d_id;
END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `p_save_info_media`(in `_media_id` int,IN `_media_name` varchar(255),IN `_media_url` varchar(255),IN `_media_type` int,IN `_media_comment` text,IN `_media_size` varchar(100),IN `_media_duration` varchar(100),IN `_media_user` int)
BEGIN
	#Routine body goes here...
	UPDATE `mtc_media_tbl` SET `media_name`=_media_name,`media_url`=_media_url,`media_type`=_media_type,`media_comment`=_media_comment,`media_size`=_media_size,`media_duration`=_media_duration ,`media_user`=_media_user WHERE media_id=_media_id;
END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `p_save_info_user`(IN `_user_id` int,IN `_fullname` text,IN `_email` varchar(100),IN `_phone` varchar(100),`_user_pass` varchar(100),`_user_type` int,`_user_content` text)
BEGIN
	#Routine body goes here...
	UPDATE mtc_user_tbl SET user_full_name=_fullname, user_email=_email, user_phone=_phone, user_pass=_user_pass, user_type=_user_type, user_content=_user_content WHERE user_id = _user_id;
END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `p_save_permision_user`(IN `_user_id` int,IN `_permission` text)
BEGIN
	#Routine body goes here...
UPDATE mtc_user_tbl SET user_permision=_permission WHERE user_id= _user_id;
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

CREATE DEFINER=`root`@`localhost` PROCEDURE `p_search_user`(IN `_key` varchar(100),IN `_from` int,IN `_number` int,OUT `_total` int)
BEGIN
	#Routine body goes here...
	SELECT COUNT(*) INTO _total FROM mtc_user_tbl WHERE user_name LIKE _key OR user_full_name LIKE _key OR user_email LIKE _key OR user_phone LIKE _key;
	SELECT *  FROM mtc_user_tbl WHERE user_name LIKE _key OR user_full_name LIKE _key OR user_email LIKE _key OR user_phone LIKE _key LIMIT _from,_number;
END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `p_set_status_user`(IN `_user_id` int,IN `_status` tinyint)
BEGIN
	#Routine body goes here...
	UPDATE mtc_user_tbl SET user_status = _status WHERE user_id=_user_id;
END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `p_type_media_all`()
BEGIN
	#Routine body goes here...
SELECT * FROM mtc_media_type_tbl; 
END$$

--
-- Functions
--
CREATE DEFINER=`root`@`localhost` FUNCTION `fc_get_hash_user`(`_user_id` int) RETURNS varchar(100) CHARSET utf8
BEGIN
	#Routine body goes here...
	DECLARE result VARCHAR(100);
	SET result='';
	SELECT MD5(CONCAT(user_name,user_pass)) INTO result FROM mtc_user_tbl WHERE user_id=_user_id AND user_status=1;
	RETURN result;
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
  `device_status` tinyint(4) DEFAULT '1',
  `device_comment` text,
  `device_time` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`device_id`),
  KEY `type_device_fk` (`device_type`)
) ENGINE=InnoDB  DEFAULT CHARSET=utf8 AUTO_INCREMENT=3 ;

--
-- Dumping data for table `mtc_device_tbl`
--

INSERT INTO `mtc_device_tbl` (`device_id`, `device_name`, `device_ip`, `device_type`, `device_status`, `device_comment`, `device_time`) VALUES
(1, 'demo', '192.168.10.1', 1, 1, 'demo', '2015-03-05 07:31:26'),
(2, 'demo 2', '127.0.0.1', 2, 1, 'dâda', '2015-03-05 08:18:44');

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
  `media_time` timestamp NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`media_id`),
  KEY `media_type_fk` (`media_type`),
  KEY `media_user_fk` (`media_user`)
) ENGINE=InnoDB  DEFAULT CHARSET=utf8 AUTO_INCREMENT=32 ;

--
-- Dumping data for table `mtc_media_tbl`
--

INSERT INTO `mtc_media_tbl` (`media_id`, `media_name`, `media_url`, `media_status`, `media_type`, `media_comment`, `media_size`, `media_duration`, `media_user`, `media_time`) VALUES
(3, 'Camera', 'rtsp://192.168.10.213:554/user=admin&password=&channel=1&stream=0.sdp?real_stream--rtp-caching=100', 0, 2, '', '0kb', '00:00:00', 1, '2015-03-05 09:58:22');

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
(1, 'Video', 'FILE', NULL, NULL, NULL),
(2, 'Camera', 'URL', NULL, NULL, NULL);

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
  `user_type` int(11) NOT NULL,
  `user_status` tinyint(2) NOT NULL DEFAULT '1',
  `user_permision` text NOT NULL,
  `user_content` text NOT NULL,
  `user_phone` varchar(100) DEFAULT NULL,
  `user_email` varchar(100) DEFAULT NULL,
  `user_time` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`user_id`),
  KEY `user_type_fk` (`user_type`)
) ENGINE=InnoDB  DEFAULT CHARSET=utf8 AUTO_INCREMENT=3 ;

--
-- Dumping data for table `mtc_user_tbl`
--

INSERT INTO `mtc_user_tbl` (`user_id`, `user_name`, `user_full_name`, `user_pass`, `user_type`, `user_status`, `user_permision`, `user_content`, `user_phone`, `user_email`, `user_time`) VALUES
(1, 'admin', 'Admin', '21232f297a57a5a743894a0e4a801fc3', 1, 1, '<?xml version="1.0" encoding="utf-16"?><Permision xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema"><mana_user>true</mana_user><view_all_media>true</view_all_media><mana_schedule>true</mana_schedule><confirm_media>true</confirm_media><mana_device>true</mana_device></Permision>', 'demo', '09796232175', 'admin@gmail.com', '2015-03-05 07:40:17'),
(2, 'giang.phan', 'Phan Thanh Giang', 'e10adc3949ba59abbe56e057f20f883e', 2, 1, '<?xml version="1.0" encoding="utf-16"?><Permision xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema"><mana_user>false</mana_user><view_all_media>false</view_all_media><mana_schedule>false</mana_schedule><confirm_media>false</confirm_media></Permision>', '', '0979632175', 'thanhgiang.009@gmail.com', '2015-03-05 08:43:16');

--
-- Triggers `mtc_user_tbl`
--
DROP TRIGGER IF EXISTS `insert_triger`;
DELIMITER //
CREATE TRIGGER `insert_triger` BEFORE INSERT ON `mtc_user_tbl`
 FOR EACH ROW begin

declare _permision text;
if(new.user_permision="") then
select default_permision into _permision from mtc_type_user_tbl where type_id = new.user_type;
set new.user_permision=_permision;
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
-- Constraints for table `mtc_user_tbl`
--
ALTER TABLE `mtc_user_tbl`
  ADD CONSTRAINT `user_type_fk` FOREIGN KEY (`user_type`) REFERENCES `mtc_type_user_tbl` (`type_id`) ON DELETE CASCADE ON UPDATE CASCADE;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
