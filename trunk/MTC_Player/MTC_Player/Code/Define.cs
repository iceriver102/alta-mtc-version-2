using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alta.Class;
using System.Runtime.Serialization;

namespace MTC_Player.Code
{
    public class LogoutException : Exception, ISerializable
    {
        public LogoutException(string message)
            : base(message)
        {

        }
    }
    public static class Define
    {
        /// <summary>
        /// Font Define
        /// </summary>
        public static FontIcons Fonts;
        /// <summary>
        /// dinh nghia table user
        /// </summary>
        public const string user_id = "user_id";
        public const string user_name = "user_name";
        public const string user_full_name = "user_full_name";
        public const string user_pass = "user_pass";
        public const string user_type = "user_type";
        public const string user_status = "user_status";
        public const string user_permision = "user_permision";
        public const string user_content = "user_content";
        public const string user_time = "user_time";
        public const string user_email = "user_email";
        public const string user_phone = "user_phone";
        public const string user_finger_print = "user_finger_print";
        /// <summary>
        /// dinh nghia trong code
        /// </summary>
        public const string hash = "hash";

        /// <summary>
        /// dinh nghia trong table mtc_type_user_tbl
        /// dinh nghie bang type media
		/// dinh nghie bang type device
        /// </summary>
        public const string type_id = "type_id";
        public const string type_name = "type_name";
        public const string default_permision = "default_permision";
        public const string type_icon = "type_icon";
        public const string type_comment = "type_comment";
        public const string type_status = "type_status";
        public const string type_time = "type_time";
        public const string type_code = "type_code";

        /// <summary>
        /// dinh nghia bang media
        /// </summary>
        public const string media_id = "media_id";
        public const string media_name = "media_name";
        public const string media_url = "media_url";
        public const string media_type = "media_type";
        public const string media_comment = "media_comment";
        public const string media_size = "media_size";
        public const string media_duration = "media_duration";
        public const string media_user = "media_user";
        public const string media_time = "media_time";
        public const string media_status = "media_status";
        public const string media_md5 = "media_md5";
		/// <summary>
        /// dinh nghia bang device
        /// </summary>
        public const string device_id = "device_id";
        public const string device_name = "device_name";
		public const string device_type="device_type";
		public const string device_status="device_status";
		public const string device_comment="device_comment";
		public const string device_time="device_time";
        public const string device_ip = "device_ip";
        /// <summary>
        /// dinh nghia bang schedule
        /// </summary>
        public const string schedule_id = "schedule_id";
        public const string schedule_user = "schedule_user";
        public const string schedule_parent = "schedule_parent";
        public const string schedule_time_begin = "schedule_time_begin";
        public const string schedule_time_end = "schedule_time_end";
        public const string schedule_status = "schedule_status";
        public const string schedule_comment = "schedule_comment";
        public const string schedule_date = "schedule_date";
        public const string schedule_device = "schedule_device";
        public const string schedule_loop = "schedule_loop";
        public const string schedule_playlist = "schedule_playlist";
        /// <summary>
        /// dinh nghia bang playlist
        /// </summary>
        public const string playlist_id = "playlist_id";
        public const string playlist_name = "playlist_name";
        public const string playlist_user = "playlist_user";
        public const string playlist_comment = "playlist_comment";
        public const string playlist_datetime = "playlist_datetime";
        public const string playlist_default = "playlist_default";
        public const string playlist_status = "playlist_status";
        /// <summary>
        /// dinh nghia bang mtc_playlist_details
        /// </summary>
        public const string detail_id = "detail_id";
        public const string detail_date = "detail_date";
        public const string time_begin = "time_begin";
        public const string time_end = "time_end";
        /// <summary>
        /// dinh nghia bang mtc_plan
        /// </summary>
        public const string plan_id = "plan_id";
        public const string plan_date = "plan_date";

        /// <summary>
        /// key noone
        /// </summary>
        public static string keySerect = "db8a04d2a0b07f841158fd9da9eaffb6";

    }
}
