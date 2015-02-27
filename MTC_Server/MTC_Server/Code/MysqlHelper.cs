using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alta.Class;
using MTC_Server.Code.User;

namespace MTC_Server.Code
{

    public static class MysqlHelper
    {
        public static List<UserData> getAllUser(int from, int number)
        {
            List<UserData> datas = null;
            try
            {
                using (MySqlConnection conn = new MySqlConnection(App.setting.connectString))
                {
                    conn.Open();
                    string query = "CALL `p_get_all_user` (@_number , @_from); ";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@_number", number);
                        cmd.Parameters.AddWithValue("@_from", from);
                        var tmp = cmd.ExecuteScalar();
                        if (Convert.ToInt32(tmp) > 0)
                        {
                            using (MySqlDataReader reader = cmd.ExecuteReader())
                            {
                                datas = reader.toUsers();
                            }
                        }
                    };
                    conn.Close();
                };
            }
            catch (Exception)
            {

            }
            return datas;
        }
        public static UserData InfoUser(int id)
        {
            UserData u = null;
            try
            {
                using (MySqlConnection conn = new MySqlConnection(App.setting.connectString))
                {
                    conn.Open();
                    string query = "CALL `InfoUser` (@userid);";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@userid", id);
                        var tmp = cmd.ExecuteScalar();
                        if (Convert.ToInt32(tmp) > 0)
                        {
                            using (MySqlDataReader reader = cmd.ExecuteReader())
                            {
                                u = reader.toUser();
                            }
                        }
                    };
                    conn.Close();
                };
            }
            catch (Exception)
            {

            }
            return u;
        }
        public static List<User.UserTypeData> getTypeUserAll()
        {
            List<User.UserTypeData> tmpData = null;

            try
            {
                using (MySqlConnection conn = new MySqlConnection(App.setting.connectString))
                {
                    conn.Open();
                    string query = "CALL `p_get_all_type_user` ();";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        var tmp = cmd.ExecuteScalar();
                        if (Convert.ToInt32(tmp) > 0)
                        {
                            using (MySqlDataReader reader = cmd.ExecuteReader())
                            {
                                tmpData = reader.toTypeUsers();
                            }
                        }
                    };
                    conn.Close();
                };
            }
            catch (MySqlException)
            {

            }
            catch (Exception )
            {

            }
            return tmpData;
        }
    }
}
