using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alta.Class;
using MTC_Player;
using MTC_Player.Code;
namespace Code.Device
{
    
    public partial class DeviceData
    {
        public Playlist.MediaEvent getEvent()
        {
            Playlist.MediaEvent m = null;
            return m;
        }

        public Media.MediaData getMedia()
        {
            Media.MediaData media = null;
            try
            {
                using (MySqlConnection conn = new MySqlConnection(App.setting.connectString))
                {
                    conn.Open();
                    string query = "`p_get_media_by_device_id`";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.Add(new MySqlParameter("@_d_id", MySqlDbType.Int32) { Direction = System.Data.ParameterDirection.Input, Value = this.ID });
                        cmd.Parameters.Add(new MySqlParameter("@_user_id", MySqlDbType.Int32) { Direction = System.Data.ParameterDirection.Output });
                        cmd.Parameters.Add(new MySqlParameter("@_details_id", MySqlDbType.Int32) { Direction = System.Data.ParameterDirection.Output });
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.ExecuteScalar();
                        this.user_id = (int) cmd.Parameters["@_user_id"].Value;
                        if (this.user_id == -1)
                        {
                            throw new LogoutException("Thiet bi da bi khoa");
                        }
                        int tmp = (int)cmd.Parameters["@_details_id"].Value;
                        if (tmp != this._details_id)
                            this.Detail_ID_Media = tmp;
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            media = reader.toMedia();
                        }
                        if (media != null && media.ID !=0)
                        {
                            media.baseMedia();
                        }
                    };
                    conn.Close();
                };

            }
            catch (LogoutException)
            {
                throw new LogoutException("Thiet bi da bi khoa");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.GetBaseException().ToString());
            }
            return media;
        }
        public Schedule.Event getPlan()
        {
            Schedule.Event plan = null;
            try
            {
                using (MySqlConnection conn = new MySqlConnection(App.setting.connectString))
                {
                    conn.Open();
                    string query = "`p_get_schedule_by_device_id`";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.Add(new MySqlParameter("@_d_id", MySqlDbType.Int32) { Direction = System.Data.ParameterDirection.Input, Value = this.ID });
                       
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.ExecuteScalar();
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            plan = reader.toSchedule();
                        }
                    };
                    conn.Close();
                };

            }
            catch (Exception)
            {
            }
            return plan;
        }
        public static DeviceData Login(string username, string pass)
        {
            string _hash = (username + pass.MD5String()).MD5String();
            DeviceData d = null;
            try
            {
                using (MySqlConnection conn = new MySqlConnection(App.setting.connectString))
                {
                    conn.Open();
                    string query = "p_device_login";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.Add(new MySqlParameter("@_d_hash", MySqlDbType.VarChar, 100) { Direction = System.Data.ParameterDirection.Input, Value = _hash });
                        cmd.Parameters.Add(new MySqlParameter("@_result", MySqlDbType.Int32, 2) { Direction = System.Data.ParameterDirection.Output });
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.ExecuteScalar();
                        bool result = Convert.ToBoolean(cmd.Parameters["@_result"].Value);
                        if (result)
                        {
                            using (MySqlDataReader reader = cmd.ExecuteReader())
                            {
                                d= reader.toDevice();
                            }
                        }
                    };
                    conn.Close();
                };
            }
            catch (MySqlException myEx)
            {
                Console.WriteLine(myEx.GetBaseException().ToString());
            }
            catch (Exception )
            {

            }
            return d;
        }
        public static DeviceData Login(string _hash)
        {
            DeviceData d = null;
            try
            {
                using (MySqlConnection conn = new MySqlConnection(App.setting.connectString))
                {
                    conn.Open();
                    string query = "p_device_login";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.Add(new MySqlParameter("@_d_hash", MySqlDbType.VarChar, 100) { Direction = System.Data.ParameterDirection.Input, Value = _hash });
                        cmd.Parameters.Add(new MySqlParameter("@_result", MySqlDbType.Int32, 2) { Direction = System.Data.ParameterDirection.Output });
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.ExecuteScalar();
                        bool result = Convert.ToBoolean(cmd.Parameters["@_result"].Value);
                        if (result)
                        {
                            using (MySqlDataReader reader = cmd.ExecuteReader())
                            {
                                d = reader.toDevice();
                            }
                        }
                    };
                    conn.Close();
                };
            }
            catch (MySqlException myEx)
            {
                Console.WriteLine(myEx.GetBaseException().ToString());
            }
            catch (Exception )
            {

            }
            return d;
        }
        public string getHash()
        {
            string result = string.Empty;
            try
            {
                using (MySqlConnection conn = new MySqlConnection(App.setting.connectString))
                {
                    conn.Open();
                    string query = "fc_get_hash_device";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.Add(new MySqlParameter("@_id", MySqlDbType.Int32) { Direction = System.Data.ParameterDirection.Input, Value = this.ID });
                        cmd.Parameters.Add(new MySqlParameter("@result", MySqlDbType.VarChar,100) { Direction = System.Data.ParameterDirection.ReturnValue });
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        var tmp = cmd.ExecuteScalar();
                        result = Convert.ToString(cmd.Parameters["@result"].Value);
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
            return result;
        }
        public static DeviceData Info(int _d_id)
        {
            DeviceData m = null;
            //p_save_info_device
            try
            {
                using (MySqlConnection conn = new MySqlConnection(App.setting.connectString))
                {
                    conn.Open();
                    string query = "`p_info_device`";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.Add(new MySqlParameter("@_d_id", MySqlDbType.Int32) { Direction = System.Data.ParameterDirection.Input, Value = _d_id });
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        var tmp = cmd.ExecuteScalar();
                        if (Convert.ToInt32(tmp) > 0)
                        {
                            using (MySqlDataReader reader = cmd.ExecuteReader())
                            {
                                m = reader.toDevice();
                            }
                        }
                    };
                    conn.Close();
                };

            }
            catch (Exception)
            {
            }
            return m;
        }
        public static int Insert(DeviceData d)
        {
            int result = -1;
            //p_insert_device
            try
            {
                using (MySqlConnection conn = new MySqlConnection(App.setting.connectString))
                {
                    conn.Open();
                    string query = "`p_insert_device`";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.Add(new MySqlParameter("@_d_name", MySqlDbType.VarChar, 255) { Direction = System.Data.ParameterDirection.Input, Value = d.Name });
                        cmd.Parameters.Add(new MySqlParameter("@_d_ip", MySqlDbType.VarChar, 30) { Direction = System.Data.ParameterDirection.Input, Value = d.IP });
                        cmd.Parameters.Add(new MySqlParameter("@_d_type", MySqlDbType.Int32) { Direction = System.Data.ParameterDirection.Input, Value =d.Type });
                        cmd.Parameters.Add(new MySqlParameter("@_d_comment", MySqlDbType.Text) { Direction = System.Data.ParameterDirection.Input, Value = d.Comment });
                        cmd.Parameters.Add(new MySqlParameter("@_d_id", MySqlDbType.Int32) { Direction = System.Data.ParameterDirection.Output, Value = 0 });
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.ExecuteScalar();
                        result = Convert.ToInt32(cmd.Parameters["@_d_id"].Value);

                    };
                    conn.Close();
                };

            }
            catch (Exception)
            {
            }
            return result;
        }

        internal void save()
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(App.setting.connectString))
                {
                    conn.Open();
                    string query = "`p_save_info_device`";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.Add(new MySqlParameter("@_d_id", MySqlDbType.Int32) { Direction = System.Data.ParameterDirection.Input, Value = this.ID });
                        cmd.Parameters.Add(new MySqlParameter("@_d_name", MySqlDbType.VarChar, 255) { Direction = System.Data.ParameterDirection.Input, Value = this.Name });
                        cmd.Parameters.Add(new MySqlParameter("@_d_ip", MySqlDbType.VarChar, 255) { Direction = System.Data.ParameterDirection.Input, Value = this.IP });
                        cmd.Parameters.Add(new MySqlParameter("@_d_type", MySqlDbType.VarChar, 255) { Direction = System.Data.ParameterDirection.Input, Value = this.Type });
                        cmd.Parameters.Add(new MySqlParameter("@_d_comment", MySqlDbType.VarChar, 255) { Direction = System.Data.ParameterDirection.Input, Value = this.Comment });
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.ExecuteScalar();
                    };
                    conn.Close();
                };
            }
            catch (Exception)
            {

            }
        }
        
        public User.UserData getCurUser()
        {
            User.UserData data = null;
            //p_get_user_of_device
            try
            {
                
                using (MySqlConnection conn = new MySqlConnection(App.setting.connectString))
                {
                    conn.Open();
                    string query = "`p_get_user_of_device`";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.Add(new MySqlParameter("@_device_id", MySqlDbType.Int32) { Direction = System.Data.ParameterDirection.Input, Value = this.ID });
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.ExecuteScalar();
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            data = reader.toUser();
                        }
                    };
                    conn.Close();
                };

            }
            catch (Exception)
            {
            }
            return data;
        }
        
        
        internal void setStatus()
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(App.setting.connectString))
                {
                    conn.Open();
                    string query = "`p_save_status_device`";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.Add(new MySqlParameter("@_d_id", MySqlDbType.Int32) { Direction = System.Data.ParameterDirection.Input, Value = this.ID });
                        cmd.Parameters.Add(new MySqlParameter("@_d_status", MySqlDbType.Int16) { Direction = System.Data.ParameterDirection.Input, Value =this.Status });
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.ExecuteScalar();
                    };
                    conn.Close();
                };

            }
            catch (Exception)
            {
            }
        }
    }
}
