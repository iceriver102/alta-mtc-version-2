using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alta.Class;

namespace MTC_Server.Code.Media
{
   public class TypeMedia
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public string Code { get; set; }
        internal static List<TypeMedia> getList()
        {
            //p_type_media_all
            try
            {
                List<TypeMedia> datas = new List<TypeMedia>();
                using (MySqlConnection conn = new MySqlConnection(App.setting.connectString))
                {
                    conn.Open();
                    string query = "`p_type_media_all`";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {                        
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        var tmp=cmd.ExecuteScalar();
                        if (Convert.ToInt32(tmp) > 0)
                        {
                            using (MySqlDataReader reader = cmd.ExecuteReader())
                            {
                                datas = reader.toTypeMedias();
                            }
                        }

                    };
                    conn.Close();
                };
                return datas;
            }
            catch (Exception)
            {

            }
            return null;
        }
    }
}
