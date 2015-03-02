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
        private void saveStatus()
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
                        cmd.Parameters.Add(new MySqlParameter("@_media_id", MySqlDbType.Int32, 100) { Direction = System.Data.ParameterDirection.Input, Value = this.ID });
                        cmd.Parameters.Add(new MySqlParameter("@_status", MySqlDbType.Int16, 100) { Direction = System.Data.ParameterDirection.Input, Value = this.Status });
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
            return 0;     
        }

        public void Save()
        {

        }

        public static void Delete(MediaData m)
        {

        }

        public static List<MediaData> GetListMedia(int from, int to, out int total)
        {
            total = 0;
            return null;
        }

        public static List<MediaData> Find(string key, int from, int to, out int total)
        {
            total = 0;
            return null;
        }

    }
}
