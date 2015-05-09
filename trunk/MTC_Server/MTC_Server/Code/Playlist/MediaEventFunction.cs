using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alta.Class;

namespace MTC_Server.Code.Playlist
{
    public partial class MediaEvent
    {
        public double Width;
        public double Left
        {
            get
            {
                return 0;
            }
        }
        public double Top
        {
            get
            {
                if (this.TimeBegin == null)
                    return double.NaN;
                return this.TimeBegin.Minutes * 2;
            }
        }
        public double Height
        {
            get
            {
                if (this.TimeBegin == null)
                    return double.NaN;
                if (this.TimeEnd == null)
                    return 0;
                return ((this.TimeEnd.TotalMinutes - this.TimeBegin.TotalMinutes) * 2 < 20) ? 20 : (this.TimeEnd.TotalMinutes - this.TimeBegin.TotalMinutes) * 2;
            }
        }

        public int EndIndex()
        {
            return this.TimeEnd.Hours + 1;
        }
        public int BeginIndex()
        {            
            return this.TimeBegin.Hours + 1;
        }

        public static MediaEvent Info(int id)
        {
            //p_info_playlist_details
            MediaEvent m = null;
            try
            {
                using (MySqlConnection conn = new MySqlConnection(App.setting.connectString))
                {
                    conn.Open();
                    string query = "`p_info_playlist_details`";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.Add(new MySqlParameter("@_id", MySqlDbType.Int32) { Direction = System.Data.ParameterDirection.Input, Value = id });
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        var tmp = cmd.ExecuteScalar();
                        if (Convert.ToInt32(tmp) > 0)
                        {

                            using (MySqlDataReader reader = cmd.ExecuteReader())
                            {
                                m = reader.toMediaEvent();
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
        public void delete()
        {
            //p_delete_playlist_details
            try
            {
                using (MySqlConnection conn = new MySqlConnection(App.setting.connectString))
                {
                    conn.Open();
                    string query = "`p_delete_playlist_details`";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.Add(new MySqlParameter("@_id", MySqlDbType.Int32) { Direction = System.Data.ParameterDirection.Input, Value = this.ID });

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

        public int Save()
        {
            //p_insert_playlist_details
            try
            {
                using (MySqlConnection conn = new MySqlConnection(App.setting.connectString))
                {
                    conn.Open();
                    string query = "`p_save_playlist_details`";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.Add(new MySqlParameter("@_id", MySqlDbType.Int32) { Direction = System.Data.ParameterDirection.Input, Value = this.ID });
                        cmd.Parameters.Add(new MySqlParameter("@_playlist_id", MySqlDbType.Int32) { Direction = System.Data.ParameterDirection.Input, Value = this.playlist_id });
                        cmd.Parameters.Add(new MySqlParameter("@_media_id", MySqlDbType.Int32) { Direction = System.Data.ParameterDirection.Input, Value = this.media_id });
                        cmd.Parameters.Add(new MySqlParameter("@_time_begin", MySqlDbType.Time) { Direction = System.Data.ParameterDirection.Input, Value = this.TimeBegin });
                        cmd.Parameters.Add(new MySqlParameter("@_time_end", MySqlDbType.Time) { Direction = System.Data.ParameterDirection.Input, Value = this.TimeEnd });
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.ExecuteScalar();

                    };
                    conn.Close();
                };
                return 1;
            }
            catch (Exception)
            {
                return -1;
            }
        }

        public static int Insert(MediaEvent media)
        {
            //p_insert_playlist_details
            try
            {
                using (MySqlConnection conn = new MySqlConnection(App.setting.connectString))
                {
                    conn.Open();
                    string query = "`p_insert_playlist_details`";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.Add(new MySqlParameter("@_playlist_id", MySqlDbType.Int32) { Direction = System.Data.ParameterDirection.Input, Value = media.playlist_id });
                        cmd.Parameters.Add(new MySqlParameter("@_media_id", MySqlDbType.Int32) { Direction = System.Data.ParameterDirection.Input, Value = media.media_id });
                        cmd.Parameters.Add(new MySqlParameter("@_time_begin", MySqlDbType.Time) { Direction = System.Data.ParameterDirection.Input, Value = media.TimeBegin });
                        cmd.Parameters.Add(new MySqlParameter("@_time_end", MySqlDbType.Time) { Direction = System.Data.ParameterDirection.Input, Value = media.TimeEnd });
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.ExecuteScalar();

                    };
                    conn.Close();
                };
                return 1;
            }
            catch (Exception)
            {
                return -1;
            }
        }
    }
}
