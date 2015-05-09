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
        public static UserData Info(string userName)
        {
            try
            {
                UserData data = null;
                using (MySqlConnection conn = new MySqlConnection(App.setting.connectString))
                {
                    conn.Open();
                    string query = "p_get_info_user_by_text";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.Add(new MySqlParameter("@_user_name", MySqlDbType.VarChar, 100) { Direction = System.Data.ParameterDirection.Input, Value = userName });

                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.ExecuteScalar();
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            data = reader.toUser();
                        }

                    };
                    conn.Close();
                };
                return data;
            }
            catch (Exception)
            {
                return null;
            }
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
                        cmd.Parameters.Add(new MySqlParameter("@_user_phone", MySqlDbType.VarChar, 100) { Direction = System.Data.ParameterDirection.Input, Value = u.Phone });
                        cmd.Parameters.Add(new MySqlParameter("@_user_email", MySqlDbType.VarChar, 100) { Direction = System.Data.ParameterDirection.Input, Value = u.Email });
                        cmd.Parameters.Add(new MySqlParameter("@_user_finger_print", MySqlDbType.LongBlob) { Direction = System.Data.ParameterDirection.Input, Value = u.Finger_Print });
                        cmd.Parameters.Add(new MySqlParameter("@_result", MySqlDbType.Int32) { Direction = System.Data.ParameterDirection.Output, Value = 0 });
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.ExecuteScalar();
                        result = Convert.ToInt32(cmd.Parameters["@_result"].Value);

                    };
                    conn.Close();
                };
            }
            catch (Exception)
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

        public static UserTypeData getType(int user_id)
        {
            //p_get_type_user_by_id
            UserTypeData type = null;
            try
            {

                using (MySqlConnection conn = new MySqlConnection(App.setting.connectString))
                {
                    conn.Open();
                    string query = "fc_get_type_user_by_id";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.Add(new MySqlParameter("@_user_id", MySqlDbType.Int32) { Direction = System.Data.ParameterDirection.Input, Value = user_id });
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.ExecuteScalar();

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            type = reader.toTypeUser();
                        }
                    };
                    conn.Close();
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.GetBaseException().ToString());
            }
            return type;
        }

        public static int getTypeUser(int user_id)
        {
            int type = 0;
            try
            {

                using (MySqlConnection conn = new MySqlConnection(App.setting.connectString))
                {
                    conn.Open();
                    string query = "fc_get_type_user_by_id";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.Add(new MySqlParameter("@_user_id", MySqlDbType.Int32) { Direction = System.Data.ParameterDirection.Input, Value = user_id });
                        cmd.Parameters.Add(new MySqlParameter("@_result", MySqlDbType.Int32) { Direction = System.Data.ParameterDirection.ReturnValue });
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.ExecuteScalar();
                        type = (int)cmd.Parameters["@_result"].Value;

                    };
                    conn.Close();
                };
            }
            catch (Exception)
            {

            }
            return type;
        }
        public int Save(bool permision = true)
        {
            int result = -1;
            //CALL `p_save_info_user` (@_user_id , @_fullName , @_email , @_phone , @_pass , @_type , @_content);
            try
            {
                //using (MySqlConnection conn = new MySqlConnection(App.setting.connectString))
                //{
                //    conn.Open();
                //    string query = "CALL `p_save_info_user` (@_user_id , @_fullName , @_email , @_phone , @_pass , @_type , @_content);";

                //    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                //    {
                //        cmd.Parameters.AddWithValue("@_user_id", this.ID);
                //        cmd.Parameters.AddWithValue("@_fullName", this.Full_Name);
                //        cmd.Parameters.AddWithValue("@_email", this.Email);
                //        cmd.Parameters.AddWithValue("@_phone", this.Phone);
                //        cmd.Parameters.AddWithValue("@_pass", this.Pass);
                //        cmd.Parameters.AddWithValue("@_type", this.Type);
                //        cmd.Parameters.AddWithValue("@_content", this.Comment);
                //        cmd.ExecuteNonQuery();
                //    };
                //    conn.Close();
                //};

                using (MySqlConnection conn = new MySqlConnection(App.setting.connectString))
                {
                    conn.Open();
                    string query = "`p_save_info_user`";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.Add(new MySqlParameter("@_user_id", MySqlDbType.Int32) { Direction = System.Data.ParameterDirection.Input, Value = this.ID });
                        cmd.Parameters.Add(new MySqlParameter("@_fullname", MySqlDbType.Text) { Direction = System.Data.ParameterDirection.Input, Value = this.Full_Name });
                        cmd.Parameters.Add(new MySqlParameter("@_email", MySqlDbType.VarChar, 100) { Direction = System.Data.ParameterDirection.Input, Value = this.Email });
                        cmd.Parameters.Add(new MySqlParameter("@_phone", MySqlDbType.VarChar, 100) { Direction = System.Data.ParameterDirection.Input, Value = this.Phone });
                        cmd.Parameters.Add(new MySqlParameter("@_user_pass", MySqlDbType.VarChar, 100) { Direction = System.Data.ParameterDirection.Input, Value = this.Pass });
                        cmd.Parameters.Add(new MySqlParameter("@_user_type", MySqlDbType.Int32) { Direction = System.Data.ParameterDirection.Input, Value = this.Type });
                        cmd.Parameters.Add(new MySqlParameter("@_user_content", MySqlDbType.Text) { Direction = System.Data.ParameterDirection.Input, Value = this.Comment });
                        cmd.Parameters.Add(new MySqlParameter("@_user_finger_print", MySqlDbType.LongBlob) { Direction = System.Data.ParameterDirection.Input, Value = this.Finger_Print });
                        cmd.Parameters.Add(new MySqlParameter("@_result", MySqlDbType.Int32) { Direction = System.Data.ParameterDirection.Output, Value = 0 });
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.ExecuteScalar();
                        result = Convert.ToInt32(cmd.Parameters["@_result"].Value);

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
            return result;
        }
        public List<MediaData> LoadMedias(int from, int to, out int total, int type = 1,bool self= false)
        {
            if (this.Permision.view_all_media && !self)
                return MediaData.GetListMedia(from, to, out total, type);
            else
                return MediaData.GetListMedia(this.ID, from, to,out total, type);
        }

        internal List<MediaData> FindMedias(string key, int from, int to, out int total, int type = 1,bool self= false)
        {
            total = 0;
            //p_find_media
            if (key == "")
                return null;
            if (key[0] != '%')
            {
                key = key.Insert(0, "%");
            }
            if (key[key.Length - 1] != '%')
            {
                key = key.Insert(key.Length, "%");
            }
            List<MediaData> datas = null;
            int id = this.ID;
            if (this.Permision.view_all_media && !self)
                id = -1;
            try
            {
                using (MySqlConnection conn = new MySqlConnection(App.setting.connectString))
                {
                    conn.Open();
                    string query = "`p_find_media`";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.Add(new MySqlParameter("@_key", MySqlDbType.VarChar, 255) { Direction = System.Data.ParameterDirection.Input, Value = key });
                        cmd.Parameters.Add(new MySqlParameter("@_user_id", MySqlDbType.Int32) { Direction = System.Data.ParameterDirection.Input, Value = id });
                        cmd.Parameters.Add(new MySqlParameter("@_type_id", MySqlDbType.Int32) { Direction = System.Data.ParameterDirection.Input, Value = type });
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
        public List<Device.DeviceData> FindDevices(string key, int from, ref int to, out int total)
        {
            total = 0;
            if (string.IsNullOrEmpty(key))
                return null;
            if (key[0] != '%')
            {
                key = key.Insert(0, "%");
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
                            cmd.Parameters.Add(new MySqlParameter("@_key", MySqlDbType.VarChar, 255) { Direction = System.Data.ParameterDirection.Input, Value = key });
                            cmd.Parameters.Add(new MySqlParameter("@_from", MySqlDbType.Int32) { Direction = System.Data.ParameterDirection.Input, Value = from });
                            cmd.Parameters.Add(new MySqlParameter("@_number", MySqlDbType.Int32) { Direction = System.Data.ParameterDirection.Input, Value = to - from });
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
                            cmd.Parameters.Add(new MySqlParameter("@_user", MySqlDbType.Int32) { Direction = System.Data.ParameterDirection.Input, Value = this.ID });

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
                catch (Exception ex)
                {
                    Console.Write(ex.GetBaseException().ToString());
                }
            }
            return null;
        }


        public static string getFullName(string _user_name)
        {
            try
            {
                string datas = null;
                using (MySqlConnection conn = new MySqlConnection(App.setting.connectString))
                {
                    conn.Open();
                    string query = "fc_get_full_name_by_text";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.Add(new MySqlParameter("@_user_name", MySqlDbType.VarChar, 100) { Direction = System.Data.ParameterDirection.Input, Value = _user_name });
                        cmd.Parameters.Add(new MySqlParameter("@_result", MySqlDbType.VarChar, 100) { Direction = System.Data.ParameterDirection.ReturnValue });
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.ExecuteScalar();
                        datas = (string)cmd.Parameters["@_result"].Value;

                    };
                    conn.Close();
                };
                return datas;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static byte[] getFingerPrinter(string _user_name)
        {

            try
            {
                byte[] datas = null;
                using (MySqlConnection conn = new MySqlConnection(App.setting.connectString))
                {
                    conn.Open();
                    string query = "fc_get_finger_printer_user";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.Add(new MySqlParameter("@_user_name", MySqlDbType.VarChar, 100) { Direction = System.Data.ParameterDirection.Input, Value = _user_name });
                        cmd.Parameters.Add(new MySqlParameter("@_result", MySqlDbType.LongBlob) { Direction = System.Data.ParameterDirection.ReturnValue });
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.ExecuteScalar();
                        datas = (byte[])cmd.Parameters["@_result"].Value;

                    };
                    conn.Close();
                };
                return datas;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static byte[] getFingerPrinter(int id)
        {

            try
            {
                byte[] datas = null;
                using (MySqlConnection conn = new MySqlConnection(App.setting.connectString))
                {
                    conn.Open();
                    string query = "fc_get_finger_printer_user_by_int";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.Add(new MySqlParameter("@_user_id", MySqlDbType.Int32) { Direction = System.Data.ParameterDirection.Input, Value = id });
                        cmd.Parameters.Add(new MySqlParameter("@_result", MySqlDbType.LongBlob) { Direction = System.Data.ParameterDirection.ReturnValue });
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.ExecuteScalar();
                        datas = (byte[])cmd.Parameters["@_result"].Value;

                    };
                    conn.Close();
                };
                return datas;
            }
            catch (Exception)
            {
                return null;
            }
        }
        internal static string getFullName(int id)
        {
            try
            {
                string datas = null;
                using (MySqlConnection conn = new MySqlConnection(App.setting.connectString))
                {
                    conn.Open();
                    string query = "fc_get_full_name_id_user";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.Add(new MySqlParameter("@_id_user", MySqlDbType.VarChar, 100) { Direction = System.Data.ParameterDirection.Input, Value = id });
                        cmd.Parameters.Add(new MySqlParameter("@_result", MySqlDbType.VarChar, 100) { Direction = System.Data.ParameterDirection.ReturnValue });
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.ExecuteScalar();
                        datas = (string)cmd.Parameters["@_result"].Value;

                    };
                    conn.Close();
                };
                return datas;
            }
            catch (Exception)
            {
                return null;
            }
        }
        internal static string getUserName(int id)
        {
            try
            {
                string datas = null;
                using (MySqlConnection conn = new MySqlConnection(App.setting.connectString))
                {
                    conn.Open();
                    string query = "fc_get_user_name_by_id";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.Add(new MySqlParameter("@_id_user", MySqlDbType.VarChar, 100) { Direction = System.Data.ParameterDirection.Input, Value = id });
                        cmd.Parameters.Add(new MySqlParameter("@_result", MySqlDbType.VarChar, 100) { Direction = System.Data.ParameterDirection.ReturnValue });
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.ExecuteScalar();
                        datas = (string)cmd.Parameters["@_result"].Value;

                    };
                    conn.Close();
                };
                return datas;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static int getUserIdByHash(string hash)
        {
            try
            {
                int datas = -1;
                using (MySqlConnection conn = new MySqlConnection(App.setting.connectString))
                {
                    conn.Open();
                    string query = "fc_login_hash";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.Add(new MySqlParameter("@_hash", MySqlDbType.VarChar, 100) { Direction = System.Data.ParameterDirection.Input, Value = hash });
                        cmd.Parameters.Add(new MySqlParameter("@_result", MySqlDbType.Int32) { Direction = System.Data.ParameterDirection.ReturnValue });
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.ExecuteScalar();
                        datas = (int)cmd.Parameters["@_result"].Value;

                    };
                    conn.Close();
                };
                return datas;
            }
            catch (Exception)
            {
                return -1;
            }
        }
        public List<Playlist.Playlist> SearchPlaylist(string key, int from, int to, out int total)
        {
            total = 0;
            List<Playlist.Playlist> datas = null;
            try
            {
                using (MySqlConnection conn = new MySqlConnection(App.setting.connectString))
                {
                    conn.Open();
                    string query = "`p_get_all_playlist_by_user`";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.Add(new MySqlParameter("@_user_id", MySqlDbType.Int32) { Direction = System.Data.ParameterDirection.Input, Value = this.ID });
                        cmd.Parameters.Add(new MySqlParameter("@_from", MySqlDbType.Int32) { Direction = System.Data.ParameterDirection.Input, Value = from });
                        cmd.Parameters.Add(new MySqlParameter("@_number", MySqlDbType.Int32) { Direction = System.Data.ParameterDirection.Input, Value = to - from });
                        cmd.Parameters.Add(new MySqlParameter("@_total", MySqlDbType.Int32) { Direction = System.Data.ParameterDirection.Output, Value = 0 });
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.ExecuteScalar();
                        total = Convert.ToInt32(cmd.Parameters["@_total"].Value);
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            datas = reader.toPlaylists();
                        }
                    };
                    conn.Close();
                };
            }
            catch (Exception ex)
            {
                Console.Write(ex.GetBaseException().ToString());
            }
            return datas;
        }
        public List<Playlist.Playlist> LoadPlaylist(int from, int to, out int total)
        {
            total = 0;
            List<Playlist.Playlist> datas = null;
            try
            {
                using (MySqlConnection conn = new MySqlConnection(App.setting.connectString))
                {
                    conn.Open();
                    string query = "`p_get_all_playlist_by_user`";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.Add(new MySqlParameter("@_user_id", MySqlDbType.Int32) { Direction = System.Data.ParameterDirection.Input, Value = this.ID });
                        cmd.Parameters.Add(new MySqlParameter("@_from", MySqlDbType.Int32) { Direction = System.Data.ParameterDirection.Input, Value = from });
                        cmd.Parameters.Add(new MySqlParameter("@_number", MySqlDbType.Int32) { Direction = System.Data.ParameterDirection.Input, Value = to - from });
                        cmd.Parameters.Add(new MySqlParameter("@_total", MySqlDbType.Int32) { Direction = System.Data.ParameterDirection.Output, Value = 0 });
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.ExecuteScalar();
                        total = Convert.ToInt32(cmd.Parameters["@_total"].Value);
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            datas = reader.toPlaylists();
                        }
                    };
                    conn.Close();
                };
            }
            catch (Exception ex)
            {
                Console.Write(ex.GetBaseException().ToString());
            }
            return datas;
        }

        public List<Device.DeviceData> LoadDevices(int from, ref int to, out int total)
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
                            cmd.Parameters.Add(new MySqlParameter("@_number", MySqlDbType.Int32) { Direction = System.Data.ParameterDirection.Input, Value = to - from });
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
        internal Playlist.Playlist getDefaultPlaylist()
        {
            //p_get_playlist_default
            Playlist.Playlist data = null;
            try
            {
                using (MySqlConnection conn = new MySqlConnection(App.setting.connectString))
                {
                    conn.Open();
                    string query = "`p_get_playlist_default`";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.Add(new MySqlParameter("@_user_id", MySqlDbType.Int32) { Direction = System.Data.ParameterDirection.Input, Value = this.ID });

                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.ExecuteScalar();
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            data = reader.toPlaylist();
                        }
                    };
                    conn.Close();
                };
            }
            catch (Exception ex)
            {
                Console.Write(ex.GetBaseException().ToString());
            }
            return data;
        }
    }
}
