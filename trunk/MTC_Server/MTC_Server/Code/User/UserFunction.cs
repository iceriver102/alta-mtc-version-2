using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTC_Server.Code.User
{
    public partial class UserData
    {
        public static UserData Info(int id)
        {
            return MysqlHelper.InfoUser(id);
        }
        public List<UserData> getListUser()
        {
            if (this.Permision.mana_user)
            {
                return MysqlHelper.getAllUser(0, 0);
            }
            List<UserData> data = new List<UserData>();
            data.Add(this);
            return data;
        }
        public void UpdateStatus()
        {
            //CALL `p_set_status_user` (@p0 , @p1);
            string result = string.Empty;
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
            catch (Exception )
            {

            }
        }
        public void setDirectpass(string md5Pass)
        { this._pass = md5Pass; }
        public void Save()
        {
            //CALL `p_save_info_user` (@_user_id , @_fullName , @_email , @_phone , @_pass , @_type , @_content);
            string result = string.Empty;
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
            catch (Exception )
            {

            }
        }
    }
}
