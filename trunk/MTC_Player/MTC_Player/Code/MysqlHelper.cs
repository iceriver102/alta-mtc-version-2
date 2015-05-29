using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alta.Class;
using MTC_Player;
using Code.User;

namespace Code
{

    public static class MysqlHelper
    {
        public static DateTime? getServerTime()
        {
            String _query = "SELECT now() AS Time";
            DateTime? time = null;

            try
            {
                using (MySqlConnection conn = new MySqlConnection(App.setting.connectString))
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(_query, conn))
                    {
                        var tmp = cmd.ExecuteScalar();
                        if (tmp != null)
                        {
                            time = Convert.ToDateTime(tmp);
                        }
                    }
                    conn.Close();
                }
            }
            catch (Exception)
            {
                
            }
            return time;
        }
        public static string getNameDevice(int device_id)
        {
            string result = string.Empty;
            try
            {
                using (MySqlConnection conn = new MySqlConnection(App.setting.connectString))
                {
                    conn.Open();
                    //string query = "CALL `p_get_all_user` (@_number , @_from, @total); ";
                    string query = "`fc_get_name_device_by_id`";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.Add(new MySqlParameter("@_id_device", MySqlDbType.Int32) { Direction = System.Data.ParameterDirection.Input, Value = device_id });
                        cmd.Parameters.Add(new MySqlParameter("@result", MySqlDbType.String) { Direction = System.Data.ParameterDirection.ReturnValue });
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        var tmp = cmd.ExecuteScalar();
                        result = cmd.Parameters["@result"].Value.ToString();
                    };
                    conn.Close();
                };
            }
            catch (Exception)
            {

            }
            return result;
        }
        public static string getIPDevice(int device_id)
        {
            string result = string.Empty;
            try
            {
                using (MySqlConnection conn = new MySqlConnection(App.setting.connectString))
                {
                    conn.Open();
                    //string query = "CALL `p_get_all_user` (@_number , @_from, @total); ";
                    string query = "`fc_get_ip_device`";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.Add(new MySqlParameter("@_id_device", MySqlDbType.Int32) { Direction = System.Data.ParameterDirection.Input, Value = device_id });
                        cmd.Parameters.Add(new MySqlParameter("@result", MySqlDbType.String) { Direction = System.Data.ParameterDirection.ReturnValue });
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        var tmp = cmd.ExecuteScalar();
                        result = cmd.Parameters["@result"].Value.ToString();
                    };
                    conn.Close();
                };
            }
            catch (Exception)
            {

            }
            return result;
        }
        public static string getNameUser(int user_id)
        {
            string result= string.Empty;
            try
            {
                using (MySqlConnection conn = new MySqlConnection(App.setting.connectString))
                {
                    conn.Open();
                    //string query = "CALL `p_get_all_user` (@_number , @_from, @total); ";
                    string query = "`fc_get_full_name_id_user`";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.Add(new MySqlParameter("@_id_user", MySqlDbType.Int32) { Direction = System.Data.ParameterDirection.Input, Value = user_id });
                        cmd.Parameters.Add(new MySqlParameter("@result", MySqlDbType.String) { Direction = System.Data.ParameterDirection.ReturnValue});
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        var tmp = cmd.ExecuteScalar();
                        result = cmd.Parameters["@result"].Value.ToString();
                    };
                    conn.Close();
                };
            }
            catch (Exception)
            {
              
            }
            return result;
        }
        public static string getNameUserBySchedule(int schedule_id)
        {
            string result = string.Empty;
            try
            {
                using (MySqlConnection conn = new MySqlConnection(App.setting.connectString))
                {
                    conn.Open();
                    //string query = "CALL `p_get_all_user` (@_number , @_from, @total); ";
                    string query = "`fc_get_fullname_of_user_by_schedule_id`";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.Add(new MySqlParameter("@_schedule_id", MySqlDbType.Int32) { Direction = System.Data.ParameterDirection.Input, Value = schedule_id });
                        cmd.Parameters.Add(new MySqlParameter("@result", MySqlDbType.String) { Direction = System.Data.ParameterDirection.ReturnValue });
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        var tmp = cmd.ExecuteScalar();
                        result = cmd.Parameters["@result"].Value.ToString();
                    };
                    conn.Close();
                };
            }
            catch (Exception)
            {

            }
            return result;
        }

        public static List<UserData> getAllUser(int from, int number, out int total)
        {
            List<UserData> datas = null;
            try
            {
                using (MySqlConnection conn = new MySqlConnection(App.setting.connectString))
                {
                    conn.Open();
                    //string query = "CALL `p_get_all_user` (@_number , @_from, @total); ";
                    string query = "`p_get_all_user`";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.Add(new MySqlParameter("@_number", MySqlDbType.Int32) { Direction= System.Data.ParameterDirection.Input, Value= number}) ;
                        cmd.Parameters.Add(new MySqlParameter("@_from", MySqlDbType.Int32) { Direction= System.Data.ParameterDirection.Input, Value= from });
                        cmd.Parameters.Add(new MySqlParameter("@total", MySqlDbType.Int32) { Direction = System.Data.ParameterDirection.Output,Value=0 });
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        var tmp = cmd.ExecuteScalar();

                        if (Convert.ToInt32(tmp) > 0)
                        {
                            total = Convert.ToInt32( cmd.Parameters["@total"].Value);
                            using (MySqlDataReader reader = cmd.ExecuteReader())
                            {
                                datas = reader.toUsers();
                            }
                        }
                        else
                        {
                            total = 0;
                        }
                    };
                    conn.Close();
                };
            }
            catch (Exception )
            {
                total = 0;
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
