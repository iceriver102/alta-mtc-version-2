using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alta.Class;
namespace MTC_Server.Code.Device
{
    public partial class DeviceData
    {
        public static DeviceData Info(int _d_id)
        {
            DeviceData m = null;
            //p_save_info_device
            try
            {
                using (MySqlConnection conn = new MySqlConnection(App.setting.connectString))
                {
                    conn.Open();
                    string query = "`p_save_info_device`";
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
