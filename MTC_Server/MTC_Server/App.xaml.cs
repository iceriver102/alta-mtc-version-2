using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Alta.Class;
using MySql.Data.MySqlClient;
using MTC_Server.Code;
using MTC_Server.Code.User;
using MTC_Server.Code.Media;

namespace MTC_Server
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static Config setting;
        public static string FileName = "setting.xml";
        public static int curUserID = 0;
        public static UserData curUser;
        public static AltaCache cache;
        public static string CacheName = "cache.xml";
        public static List<UserTypeData> TypeUsers;
        public static List<TypeMedia> TypeMedias;
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            setting = Config.Read(FileName);
            cache = AltaCache.Read(CacheName);
            TypeUsers = MysqlHelper.getTypeUserAll();
            TypeMedias = TypeMedia.getList();

            if (cache.autoLogin && !string.IsNullOrEmpty(cache.hashUserName))
            {
                int tmpResult = Login(cache.hashUserName);
                if (tmpResult != 0)
                {
                    curUserID = tmpResult;
                    this.MainWindow = new MainWindow();
                    this.MainWindow.Show();
                }
                else
                {
                    this.MainWindow = new Login();
                    this.MainWindow.Show();
                }
            }
            else
            {
                this.MainWindow = new Login();
                this.MainWindow.Show();
            }
        }
        public static string getHash(int id)
        {
            //SELECT  `fc_get_hash_user` ( @user_id) AS  `hash` ;
            string result = string.Empty;
            try
            {
                using (MySqlConnection conn = new MySqlConnection(App.setting.connectString))
                {
                    conn.Open();
                    string query = "SELECT  `fc_get_hash_user` ( @user_id) AS " + Define.hash;

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@user_id", id);
                        var tmp = cmd.ExecuteScalar();
                        result = (string)tmp;
                    };
                    conn.Close();
                };
            }
            catch (MySqlException)
            {
                MessageBox.Show("Không thể kết nối với csdl");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            return result;
        }

        public static UserTypeData selectType(int id)
        {
            if (TypeUsers != null)
            {
                foreach (UserTypeData data in TypeUsers)
                {
                    if (data.Id == id)
                        return data;
                }
            }
            return null;
        }

        public int Login(string hash)
        {

            int result = 0;
            try
            {
                using (MySqlConnection conn = new MySqlConnection(App.setting.connectString))
                {
                    conn.Open();
                    string query = "SELECT  `fc_login_hash` (@hash) AS  `user_id`";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@hash", hash);
                        var tmp = cmd.ExecuteScalar();
                        result = (int)tmp;
                    };
                    conn.Close();
                };
            }
            catch (MySqlException)
            {
                MessageBox.Show("Không thể kết nối với csdl");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            return result;
        }
    }
}
