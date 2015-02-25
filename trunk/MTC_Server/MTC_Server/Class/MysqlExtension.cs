using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MTC_Server.Code;
using MySql.Data.MySqlClient;

namespace Alta.Class
{
    public static class MysqlExtensions
    {
        public static User toUser(this MySqlDataReader reader)
        {
            User u = null;
            while (reader.Read())
            {
                //alta_class_playlist tmp = new alta_class_playlist();
                if (!reader.IsDBNull(0))
                {
                    u = new User();
                    u.ID = reader.GetInt32(Define.user_id);
                    u.Full_Name = reader.GetString(Define.user_full_name);
                    u.Permision = Permision.Read(reader.GetString(Define.user_permision));
                    u.Status = reader.GetBoolean(Define.user_status);
                }
            }
            return u;
        }
    }
}
