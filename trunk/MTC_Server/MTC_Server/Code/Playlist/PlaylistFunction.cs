using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alta.Class;
namespace MTC_Server.Code.Playlist
{
    public partial class Playlist
    {
        public static Playlist Info(int id)
        {
            Playlist data = null;
            try
            {
               
                using (MySqlConnection conn = new MySqlConnection(App.setting.connectString))
                {
                    conn.Open();
                    string query = "p_info_playlist";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.Add(new MySqlParameter("@_playlist_id", MySqlDbType.Int32) { Direction = System.Data.ParameterDirection.Input, Value = id });

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
            catch (Exception)
            {
               
            }
            return data;
        }
        public static int Insert(Playlist playlist)
        {
            try
            {

                using (MySqlConnection conn = new MySqlConnection(App.setting.connectString))
                {
                    conn.Open();
                    string query = "p_insert_playlist";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.Add(new MySqlParameter("@_playlist_name", MySqlDbType.VarChar,100) { Direction = System.Data.ParameterDirection.Input, Value = playlist.Name });
                        cmd.Parameters.Add(new MySqlParameter("@_playlist_user", MySqlDbType.Int32) { Direction = System.Data.ParameterDirection.Input, Value = playlist.user_id });
                        cmd.Parameters.Add(new MySqlParameter("@_playlist_comment", MySqlDbType.VarChar, 100) { Direction = System.Data.ParameterDirection.Input, Value = playlist.Comment });
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.ExecuteScalar();
                    };
                    conn.Close();
                };
                return 1;
            }
            catch (MySqlException)
            {
                return -1;
            }
            catch (Exception)
            {
                return -2;
            }
        }

        public void ChangeStatus(bool status)
        {
            
            try
            {

                using (MySqlConnection conn = new MySqlConnection(App.setting.connectString))
                {
                    conn.Open();
                    string query = "p_change_status_playlist";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.Add(new MySqlParameter("@_playlist_id", MySqlDbType.Int32) { Direction = System.Data.ParameterDirection.Input, Value = this.ID });
                        cmd.Parameters.Add(new MySqlParameter("@_playlist_status", MySqlDbType.Int32, 2) { Direction = System.Data.ParameterDirection.Input, Value = status });
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.ExecuteScalar();
                    };
                    conn.Close();
                };
                this.Status = status;
            }
            catch (MySqlException)
            {
              
            }
            catch (Exception ex)
            {
                Console.Write(ex.GetBaseException().ToString());
            }
        }
        public void ChangeDefault(bool status)
        {

            try
            {

                using (MySqlConnection conn = new MySqlConnection(App.setting.connectString))
                {
                    conn.Open();
                    string query = "p_change_default_playlist";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.Add(new MySqlParameter("@_playlist_id", MySqlDbType.Int32) { Direction = System.Data.ParameterDirection.Input, Value = this.ID });
                        cmd.Parameters.Add(new MySqlParameter("@_playlist_status", MySqlDbType.Int32, 2) { Direction = System.Data.ParameterDirection.Input, Value = status });
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.ExecuteScalar();
                    };
                    conn.Close();
                };
                this.Default = status;
            }
            catch (MySqlException myex)
            {
                Console.WriteLine(myex.GetBaseException().ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.GetBaseException().ToString());
            }
        }

        public int save()
        {
            try
            {

                using (MySqlConnection conn = new MySqlConnection(App.setting.connectString))
                {
                    conn.Open();
                    string query = "p_save_playlist";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.Add(new MySqlParameter("@_playlist_id", MySqlDbType.VarChar, 100) { Direction = System.Data.ParameterDirection.Input, Value = this.ID });
                        cmd.Parameters.Add(new MySqlParameter("@_playlist_name", MySqlDbType.VarChar, 100) { Direction = System.Data.ParameterDirection.Input, Value = this.Name });
                        cmd.Parameters.Add(new MySqlParameter("@_playlist_user", MySqlDbType.Int32) { Direction = System.Data.ParameterDirection.Input, Value = this.user_id });
                        cmd.Parameters.Add(new MySqlParameter("@_playlist_comment", MySqlDbType.VarChar, 100) { Direction = System.Data.ParameterDirection.Input, Value = this.Comment });
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.ExecuteScalar();
                    };
                    conn.Close();
                };
                return 1;
            }
            catch (MySqlException)
            {
                return -1;
            }
            catch (Exception)
            {
                return -2;
            }
        }

        public List<MediaEvent> LoadMediaEvent()
        {
            //p_get_media_in_playlist
            try
            {
                List<MediaEvent> list = null;
                using (MySqlConnection conn = new MySqlConnection(App.setting.connectString))
                {
                    conn.Open();
                    string query = "p_get_media_in_playlist";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.Add(new MySqlParameter("@_playlist_id", MySqlDbType.Int32) { Direction = System.Data.ParameterDirection.Input, Value = this.ID });
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.ExecuteScalar();
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            list = reader.toMediaEvents();
                        }
                    };
                    conn.Close();
                };
                return list;
            }
            catch (MySqlException mysqlEx)
            {
                Console.WriteLine(mysqlEx.GetBaseException().ToString());
            }
            catch (Exception Ex)
            {
                Console.WriteLine(Ex.GetBaseException().ToString());
            }
            return null;
        }

        public void delete()
        {
            try
            {

                using (MySqlConnection conn = new MySqlConnection(App.setting.connectString))
                {
                    conn.Open();
                    string query = "p_delete_playlist";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.Add(new MySqlParameter("@_playlist_id", MySqlDbType.Int32) { Direction = System.Data.ParameterDirection.Input, Value = this.ID });
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.ExecuteScalar();
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
    }
}
