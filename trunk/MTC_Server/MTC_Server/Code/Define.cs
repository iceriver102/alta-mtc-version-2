using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alta.Class;

namespace MTC_Server.Code
{
    public static class Define
    {
        /// <summary>
        /// Font Define
        /// </summary>
        public static FontIconDefine Fonts = new FontIconDefine();
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
        /// <summary>
        /// dinh nghia trong code
        /// </summary>
        public const string hash = "hash";

        /// <summary>
        /// dinh nghia trong table mtc_type_user_tbl
        /// </summary>
        public const string type_id = "type_id";
        public const string type_name = "type_name";
        public const string default_permision = "default_permision";
        public const string type_icon = "type_icon";
        public const string type_comment = "type_comment";
        public const string type_status = "type_status";



    }
}
