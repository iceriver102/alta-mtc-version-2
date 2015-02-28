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
        public List<UserData> getListUser(int from,int to,out int total)
        {
            if (this.Permision.mana_user)
            {
                return MysqlHelper.getAllUser(from, to-from,out total);
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

    }
}
