using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alta.Class;
using MTC_Player;

namespace Code.Device
{
    public class TypeDevice
    {
        public int ID { get; set; }
        public string Icon { get; set; }
        public string Name { get; set; }
        public string Comment { get; set; }
        public DateTime Time { get; set; }

        public static List<TypeDevice> getList()
        {
            //p_get_all_type_device
            try
            {
                List<TypeDevice> datas = new List<TypeDevice>();
                using (MySqlConnection conn = new MySqlConnection(App.setting.connectString))
                {
                    conn.Open();
                    string query = "`p_get_all_type_device`";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        var tmp = cmd.ExecuteScalar();
                        if (Convert.ToInt32(tmp) > 0)
                        {
                            using (MySqlDataReader reader = cmd.ExecuteReader())
                            {
                                datas = reader.toTypeDevices();
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
