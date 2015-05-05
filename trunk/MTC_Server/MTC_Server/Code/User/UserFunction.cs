using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alta.Class;
using MTC_Server.Code.Media;

namespace MTC_Server.Code.User
{
    public partial class UserData
    {
        public static UserData Info(int id)
        {
            return MysqlHelper.InfoUser(id);
        }
        public List<UserData> getListUser(int from, int to, out int total)
        {
            if (this.Permision.mana_user)
            {
                return MysqlHelper.getAllUser(from, to - from, out total);
            }
            List<UserData> data = new List<UserData>();
            data.Add(this);
            total = 1;
            return data;
        }
        public static void deleteUser(UserData u)
        {
            // CALL `p_delete_user`(@_user_id);
            try
            {
                using (MySqlConnection conn = new MySqlConnection(App.setting.connectString))
                {
                    conn.Open();
                    string query = "CALL `p_delete_user` (@_user_id);";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@_user_id", u.ID);
                        cmd.ExecuteNonQuery();
                    };
                    conn.Close();
                };
            }
            catch (MySqlException)
            {

            }
            catch (Exception)
            {

            }
        }
        public static List<UserData> SearchUser(string key, int from, int to, out int total)
        {
            //CALL `p_search_user` (@p0, @p1, @p2, @p3);
            if (key[0] != '%')
            {
                key = key.Insert(0, "%");
            }
            if (key[key.Length - 1] != '%')
            {
                key = key.Insert(key.Length, "%");
            }
            List<UserData> datas = null;
            total = 0;
            try
            {

                using (MySqlConnection conn = new MySqlConnection(App.setting.connectString))
                {
                    conn.Open();
                    string query = "`p_search_user`";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.Add(new MySqlParameter("@_key", MySqlDbType.VarChar, 100) { Direction = System.Data.ParameterDirection.Input, Value = key });
                        cmd.Parameters.Add(new MySqlParameter("@_from", MySqlDbType.Int32) { Direction = System.Data.ParameterDirection.Input, Value = from });
                        cmd.Parameters.Add(new MySqlParameter("@_number", MySqlDbType.Int32) { Direction = System.Data.ParameterDirection.Input, Value = to - from });
                        cmd.Parameters.Add(new MySqlParameter("@_total", MySqlDbType.Int32) { Direction = System.Data.ParameterDirection.Output, Value = 0 });
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        var tmp = cmd.ExecuteScalar();
                        if (Convert.ToInt32(tmp) > 0)
                        {
                            total = Convert.ToInt32(cmd.Parameters["@_total"].Value);
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
        public static int insertUser(UserData u)
        {
            int result = 0;
            try
            {

                using (MySqlConnection conn = new MySqlConnection(App.setting.connectString))
                {
                    conn.Open();
                    string query = "`p_insert_user`";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.Add(new MySqlParameter("@_fullname", MySqlDbType.Text) { Direction = System.Data.ParameterDirection.Input, Value = u.Full_Name });
                        cmd.Parameters.Add(new MySqlParameter("@_user_name", MySqlDbType.VarChar, 100) { Direction = System.Data.ParameterDirection.Input, Value = u.User_Name });
                        cmd.Parameters.Add(new MySqlParameter("@_user_pass", MySqlDbType.VarChar, 100) { Direction = System.Data.ParameterDirection.Input, Value = u.Pass });
                        cmd.Parameters.Add(new MySqlParameter("@_user_type", MySqlDbType.Int32) { Direction = System.Data.ParameterDirection.Input, Value = u.Type });
                        cmd.Parameters.Add(new MySqlParameter("@_user_phone", MySqlDbType.VarChar,100) { Direction = System.Data.ParameterDirection.Input, Value = u.Phone });
                        cmd.Parameters.Add(new MySqlParameter("@_user_email", MySqlDbType.VarChar,100) { Direction = System.Data.ParameterDirection.Input, Value = u.Email });
                        cmd.Parameters.Add(new MySqlParameter("@_result", MySqlDbType.Int32) { Direction = System.Data.ParameterDirection.Output, Value = 0 });
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.ExecuteScalar();
                        result = Convert.ToInt32(cmd.Parameters["@_result"].Value);

                    };
                    conn.Close();
                };
            }
            catch (Exception )
            {
               
            }
            return result;
        }
        public void savePermission()
        {
            // CALL `p_save_permision_user`(@p0, @p1);
            try
            {
                using (MySqlConnection conn = new MySqlConnection(App.setting.connectString))
                {
                    conn.Open();
                    string query = "CALL `p_save_permision_user` (@_user_id , @_permision);";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@_user_id", this.ID);
                        cmd.Parameters.AddWithValue("@_permision", Permision.Write(this.Permision));
                        cmd.ExecuteNonQuery();
                    };
                    conn.Close();
                };
            }
            catch (MySqlException)
            {

            }
            catch (Exception)
            {

            }
        }
        public void UpdateStatus()
        {
            //CALL `p_set_status_user` (@p0 , @p1);
            try
            {
                using (MySqlConnection conn = new MySqlConnection(App.setting.connectString))
                {
                    conn.Open();
                    string query = "CALL `p_set_status_user` (@_user_id , @_status);";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@_user_id", this.ID);
                        cmd.Parameters.AddWithValue("@_status", this.Status);
                        cmd.ExecuteNonQuery();
                    };
                    conn.Close();
                };
            }
            catch (MySqlException)
            {

            }
            catch (Exception)
            {

            }
        }
        public void setDirectpass(string md5Pass)
        {
            this._pass = md5Pass;
        }
        public void Save(bool permision = true)
        {
            //CALL `p_save_info_user` (@_user_id , @_fullName , @_email , @_phone , @_pass , @_type , @_content);
            try
            {
                using (MySqlConnection conn = new MySqlConnection(App.setting.connectString))
                {
                    conn.Open();
                    string query = "CALL `p_save_info_user` (@_user_id , @_fullName , @_email , @_phone , @_pass , @_type , @_content);";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@_user_id", this.ID);
                        cmd.Parameters.AddWithValue("@_fullName", this.Full_Name);
                        cmd.Parameters.AddWithValue("@_email", this.Email);
                        cmd.Parameters.AddWithValue("@_phone", this.Phone);
                        cmd.Parameters.AddWithValue("@_pass", this.Pass);
                        cmd.Parameters.AddWithValue("@_type", this.Type);
                        cmd.Parameters.AddWithValue("@_content", this.Comment);
                        cmd.ExecuteNonQuery();
                    };
                    conn.Close();
                };
            }
            catch (MySqlException)
            {

            }
            catch (Exception)
            {

            }
            if (permision)
                this.savePermission();
        }
        public List<MediaData> LoadMedias(int from, int to, out int total, int type=1)
        {
            if (this.Permision.view_all_media)
                return MediaData.GetListMedia(from, to, out total,type);
            else
                return MediaData.GetListMedia(this.ID, from, out total,type);
        }

        internal List<MediaData> FindMedias(string key, int from, int to, out int total, int type=1)
        {
            //p_find_media

            if (key[0] != '%')
            {
                key = key.Insert(0, "%");
            }
            if (key[key.Length - 1] != '%')
            {
                key = key.Insert(key.Length, "%");
            }
            List<MediaData> datas = null;
            total = 0;
            try
            {
                using (MySqlConnection conn = new MySqlConnection(App.setting.connectString))
                {
                    conn.Open();
                    string query = "`p_find_media`";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.Add(new MySqlParameter("@_key", MySqlDbType.VarChar,255) { Direction = System.Data.ParameterDirection.Input, Value = key });
                        cmd.Parameters.Add(new MySqlParameter("@_user_id", MySqlDbType.Int32) { Direction = System.Data.ParameterDirection.Input, Value = this.ID });
                        cmd.Parameters.Add(new MySqlParameter("@_type_id", MySqlDbType.Int32) { Direction = System.Data.ParameterDirection.Input, Value = type});
                        cmd.Parameters.Add(new MySqlParameter("@_from", MySqlDbType.Int32) { Direction = System.Data.ParameterDirection.Input, Value = from });
                        cmd.Parameters.Add(new MySqlParameter("@_number", MySqlDbType.Int32) { Direction = System.Data.ParameterDirection.Input, Value = to - from });
                        cmd.Parameters.Add(new MySqlParameter("@_total", MySqlDbType.Int32) { Direction = System.Data.ParameterDirection.Output, Value = 0 });
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.ExecuteScalar();
                        total = Convert.ToInt32(cmd.Parameters["@_total"].Value);
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            datas = reader.toMedias();
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
        public List<Device.DeviceData> FindDevices(string key,int from, ref int to, out int total)
        {
            total = 0;
            if (key[0] != '%')
            {
                key=key.Insert(0, "%");
            }
            if (key[key.Length - 1] != '%')
            {
                key = key.Insert(key.Length, "%");
            }
            List<Device.DeviceData> datas = null;
            if (this.Permision.mana_device)
            {
                //p_find_device                
                total = 0;
                try
                {
                    using (MySqlConnection conn = new MySqlConnection(App.setting.connectString))
                    {
                        conn.Open();
                        string query = "`p_find_device`";
                        using (MySqlCommand cmd = new MySqlCommand(query, conn))
                        {
                            cmd.Parameters.Add(new MySqlParameter("@_key", MySqlDbType.VarChar,255) { Direction = System.Data.ParameterDirection.Input, Value = key });
                            cmd.Parameters.Add(new MySqlParameter("@_from", MySqlDbType.Int32) { Direction = System.Data.ParameterDirection.Input, Value = from });
                            cmd.Parameters.Add(new MySqlParameter("@_number", MySqlDbType.Int32) { Direction = System.Data.ParameterDirection.Input, Value = to-from });
                            cmd.Parameters.Add(new MySqlParameter("@_total", MySqlDbType.Int32) { Direction = System.Data.ParameterDirection.Output, Value = 0 });
                            cmd.CommandType = System.Data.CommandType.StoredProcedure;
                            cmd.ExecuteScalar();
                            total = Convert.ToInt32(cmd.Parameters["@_total"].Value);
                            using (MySqlDataReader reader = cmd.ExecuteReader())
                            {
                                datas = reader.toDevices();
                            }
                        };
                        conn.Close();
                    };
                    return datas;
                }
                catch (Exception)
                {
                }
            }
            else
            {
                try
                {
                    using (MySqlConnection conn = new MySqlConnection(App.setting.connectString))
                    {
                        conn.Open();
                        string query = "`p_find_device_by_user`";
                        using (MySqlCommand cmd = new MySqlCommand(query, conn))
                        {
                            cmd.Parameters.Add(new MySqlParameter("@_user_id", MySqlDbType.Int32) { Direction = System.Data.ParameterDirection.Input, Value = this.ID });
                            cmd.Parameters.Add(new MySqlParameter("@_key", MySqlDbType.VarChar, 255) { Direction = System.Data.ParameterDirection.Input, Value = key });
                            cmd.Parameters.Add(new MySqlParameter("@_from_date", MySqlDbType.DateTime) { Direction = System.Data.ParameterDirection.Input, Value = DateTime.Now });
                            cmd.Parameters.Add(new MySqlParameter("@_total", MySqlDbType.Int32) { Direction = System.Data.ParameterDirection.Output, Value = 0 });
                            cmd.CommandType = System.Data.CommandType.StoredProcedure;
                            cmd.ExecuteScalar();
                            total = Convert.ToInt32(cmd.Parameters["@_total"].Value);
                            to = total;
                            using (MySqlDataReader reader = cmd.ExecuteReader())
                            {
                                datas = reader.toDevices();
                            }
                        };
                        conn.Close();
                    };
                    return datas;
                }
                catch (Exception)
                {
                }
            }
            return null;
        }
        public List<Schedule.Event> getEvents(DateTime date)
        {
            List<Schedule.Event> datas = null;
            if (this.Permision.mana_schedule)
            {
                //p_get_schedule_date
                try
                {
                    using (MySqlConnection conn = new MySqlConnection(App.setting.connectString))
                    {
                        conn.Open();
                        string query = "p_get_schedule_date";
                        using (MySqlCommand cmd = new MySqlCommand(query, conn))
                        {
                            cmd.Parameters.Add(new MySqlParameter("@_date", MySqlDbType.DateTime) { Direction = System.Data.ParameterDirection.Input, Value = date.Date });
                           
                            cmd.CommandType = System.Data.CommandType.StoredProcedure;
                            cmd.ExecuteScalar();
                            using (MySqlDataReader reader = cmd.ExecuteReader())
                            {
                                datas = reader.toSchedules();
                            }
                        };
                        conn.Close();
                    };
                    return datas;
                }
                catch (Exception)
                {
                    
                }
            }
            else
            {
                //p_get_schedule_date_user

                try
                {
                    using (MySqlConnection conn = new MySqlConnection(App.setting.connectString))
                    {
                        conn.Open();
                        string query = "p_get_schedule_date_user";
                        using (MySqlCommand cmd = new MySqlCommand(query, conn))
                        {
                            cmd.Parameters.Add(new MySqlParameter("@_date", MySqlDbType.DateTime) { Direction = System.Data.ParameterDirection.Input, Value = date.Date });
                            cmd.Parameters.Add(new MySqlParameter("@_user", MySqlDbType.DateTime) { Direction = System.Data.ParameterDirection.Input, Value = this.ID });

                            cmd.CommandType = System.Data.CommandType.StoredProcedure;
                            cmd.ExecuteScalar();
                            using (MySqlDataReader reader = cmd.ExecuteReader())
                            {
                                datas = reader.toSchedules();
                            }
                        };
                        conn.Close();
                    };
                    return datas;
                }
                catch (Exception)
                {

                }
            }
            return null;
        }
        public List<Device.DeviceData> LoadDevices(int from,ref int to,out int total)
        {
            total = 0;
            List<Device.DeviceData> datas = null;
            if (this.Permision.mana_device)
            {
                total = 0;
                try
                {
                    using (MySqlConnection conn = new MySqlConnection(App.setting.connectString))
                    {
                        conn.Open();
                        string query = "`p_get_all_device`";
                        using (MySqlCommand cmd = new MySqlCommand(query, conn))
                        {
                            cmd.Parameters.Add(new MySqlParameter("@_from", MySqlDbType.Int32) { Direction = System.Data.ParameterDirection.Input, Value = from });
                            cmd.Parameters.Add(new MySqlParameter("@_number", MySqlDbType.Int32) { Direction = System.Data.ParameterDirection.Input, Value = to-from });
                            cmd.Parameters.Add(new MySqlParameter("@_total", MySqlDbType.Int32) { Direction = System.Data.ParameterDirection.Output, Value = 0 });
                            cmd.CommandType = System.Data.CommandType.StoredProcedure;
                            cmd.ExecuteScalar();
                            total = Convert.ToInt32(cmd.Parameters["@_total"].Value);
                            using (MySqlDataReader reader = cmd.ExecuteReader())
                            {
                                datas = reader.toDevices();
                            }
                        };
                        conn.Close();
                    };
                    return datas;
                }
                catch (Exception)
                {
                }
            }
            else
            {
                //p_get_all_devive_by_user
                try
                {
                    using (MySqlConnection conn = new MySqlConnection(App.setting.connectString))
                    {
                        conn.Open();
                        string query = "`p_get_all_device_by_user`";
                        using (MySqlCommand cmd = new MySqlCommand(query, conn))
                        {
                            cmd.Parameters.Add(new MySqlParameter("@_user_id", MySqlDbType.Int32) { Direction = System.Data.ParameterDirection.Input, Value = this.ID });
                            cmd.Parameters.Add(new MySqlParameter("@_from_date", MySqlDbType.DateTime) { Direction = System.Data.ParameterDirection.Input, Value = DateTime.Now });
                            cmd.Parameters.Add(new MySqlParameter("@_total", MySqlDbType.Int32) { Direction = System.Data.ParameterDirection.Output, Value = 0 });
                            cmd.CommandType = System.Data.CommandType.StoredProcedure;
                            cmd.ExecuteScalar();
                            total = Convert.ToInt32(cmd.Parameters["@_total"].Value);
                            to = total;
                            using (MySqlDataReader reader = cmd.ExecuteReader())
                            {
                                datas = reader.toDevices();
                            }
                        };
                        conn.Close();
                    };
                    return datas;
                }
                catch (Exception)
                {
                }
            }
            return null;
        }

    }
}
