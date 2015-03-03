using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alta.Class;

namespace MTC_Server.Code.Media
{
    public partial class MediaData
    {
        public static MediaData Info(int id)
        {
            MediaData m=null;
            //p_info_media
            try
            {
                using (MySqlConnection conn = new MySqlConnection(App.setting.connectString))
                {
                    conn.Open();
                    string query = "`p_info_media`";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.Add(new MySqlParameter("@_media_id", MySqlDbType.Int32, 100) { Direction = System.Data.ParameterDirection.Input, Value = id });                     
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        var tmp = cmd.ExecuteScalar();
                        if (Convert.ToInt32(tmp) > 0)
                        {
                            
                            using (MySqlDataReader reader = cmd.ExecuteReader())
                            {
                                m = reader.toMedia();
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
        public void saveStatus()
        {
            //p_save_status_media
            try
            {
                using (MySqlConnection conn = new MySqlConnection(App.setting.connectString))
                {
                    conn.Open();
                    string query = "`p_save_status_media`";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.Add(new MySqlParameter("@_media_id", MySqlDbType.Int32) { Direction = System.Data.ParameterDirection.Input, Value = this.ID });
                        cmd.Parameters.Add(new MySqlParameter("@_status", MySqlDbType.Int16) { Direction = System.Data.ParameterDirection.Input, Value = this.Status });
                        //_status
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
        public static int Insert(MediaData m)
        {
            int result = -1;
            //p_insert_media
            try
            {
                using (MySqlConnection conn = new MySqlConnection(App.setting.connectString))
                {
                    conn.Open();
                    string query = "`p_insert_media`";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.Add(new MySqlParameter("@_media_name", MySqlDbType.VarChar, 255) { Direction = System.Data.ParameterDirection.Input, Value = m.Name });
                        cmd.Parameters.Add(new MySqlParameter("@_media_url", MySqlDbType.VarChar, 255) { Direction = System.Data.ParameterDirection.Input, Value = m.Url });
                        cmd.Parameters.Add(new MySqlParameter("@_media_type", MySqlDbType.Int32) { Direction = System.Data.ParameterDirection.Input, Value = m.Type });
                        cmd.Parameters.Add(new MySqlParameter("@_media_comment", MySqlDbType.Text) { Direction = System.Data.ParameterDirection.Input, Value = m.Comment });
                        cmd.Parameters.Add(new MySqlParameter("@_media_size", MySqlDbType.VarChar,100) { Direction = System.Data.ParameterDirection.Input, Value = m.FileSize });
                        cmd.Parameters.Add(new MySqlParameter("@_media_duration", MySqlDbType.VarChar, 100) { Direction = System.Data.ParameterDirection.Input, Value = m.Duration });
                        cmd.Parameters.Add(new MySqlParameter("@_media_user", MySqlDbType.Int32) { Direction = System.Data.ParameterDirection.Input, Value = m.User_ID });
                        cmd.Parameters.Add(new MySqlParameter("@_media_id", MySqlDbType.Int32) { Direction = System.Data.ParameterDirection.Output, Value = 0 });
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.ExecuteScalar();
                        result = Convert.ToInt32(cmd.Parameters["@_media_id"].Value);

                    };
                    conn.Close();
                };

            }
            catch (Exception)
            {
            }
            return result;
        }

        public void Save()
        {
            //p_save_info_media
            try
            {
                using (MySqlConnection conn = new MySqlConnection(App.setting.connectString))
                {
                    conn.Open();
                    string query = "`p_save_info_media`";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.Add(new MySqlParameter("@_media_id", MySqlDbType.Int32) { Direction = System.Data.ParameterDirection.Input, Value = this.ID });
                        cmd.Parameters.Add(new MySqlParameter("@_media_name", MySqlDbType.VarChar, 255) { Direction = System.Data.ParameterDirection.Input, Value = this.Name });
                        cmd.Parameters.Add(new MySqlParameter("@_media_url", MySqlDbType.VarChar, 255) { Direction = System.Data.ParameterDirection.Input, Value = this.Url });
                        cmd.Parameters.Add(new MySqlParameter("@_media_type", MySqlDbType.Int32) { Direction = System.Data.ParameterDirection.Input, Value = this.Type });
                        cmd.Parameters.Add(new MySqlParameter("@_media_comment", MySqlDbType.Text) { Direction = System.Data.ParameterDirection.Input, Value = this.Comment });
                        cmd.Parameters.Add(new MySqlParameter("@_media_size", MySqlDbType.VarChar, 100) { Direction = System.Data.ParameterDirection.Input, Value = this.FileSize });
                        cmd.Parameters.Add(new MySqlParameter("@_media_duration", MySqlDbType.VarChar, 100) { Direction = System.Data.ParameterDirection.Input, Value = this.Duration });
                        cmd.Parameters.Add(new MySqlParameter("@_media_user", MySqlDbType.Int32) { Direction = System.Data.ParameterDirection.Input, Value = this.User_ID });
                        
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

        public static void Delete(MediaData m)
        {

        }

        public static List<MediaData> GetListMedia(int user_id,int from, int to, out int total)
        {
            List<MediaData> datas = null;
            total = 0;
            try
            {
                using (MySqlConnection conn = new MySqlConnection(App.setting.connectString))
                {
                    conn.Open();
                    string query = "`p_get_all_media`";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.Add(new MySqlParameter("@_user_id", MySqlDbType.Int32) { Direction = System.Data.ParameterDirection.Input, Value = user_id });
                        cmd.Parameters.Add(new MySqlParameter("@_media_type", MySqlDbType.Int32) { Direction = System.Data.ParameterDirection.Input, Value = 1 });
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
        public static List<MediaData> GetListMedia( int from, int to, out int total)
        {
            List<MediaData> datas = null;
            total = 0;
            try
            {
                using (MySqlConnection conn = new MySqlConnection(App.setting.connectString))
                {
                    conn.Open();
                    string query = "`p_get_all_media`";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.Add(new MySqlParameter("@_user_id", MySqlDbType.Int32) { Direction = System.Data.ParameterDirection.Input, Value = -1 });
                        cmd.Parameters.Add(new MySqlParameter("@_media_type", MySqlDbType.Int32) { Direction = System.Data.ParameterDirection.Input, Value = 1 });
                        cmd.Parameters.Add(new MySqlParameter("@_from", MySqlDbType.Int32) { Direction = System.Data.ParameterDirection.Input, Value = from });
                        cmd.Parameters.Add(new MySqlParameter("@_number", MySqlDbType.Int32) { Direction = System.Data.ParameterDirection.Input, Value = to-from });
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

        public static List<MediaData> Find(string key, int from, int to, out int total)
        {
            total = 0;
            return null;
        }

    }
}
