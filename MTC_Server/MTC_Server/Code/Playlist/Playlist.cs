using MTC_Server.Code.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTC_Server.Code.Playlist
{
    public partial class Playlist
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Comment { get; set; }
        public int user_id;
        public DateTime Time { get; set; }
        public bool Status { get; set; }
        public bool Default { get; set; }
        public UserData User
        {
            get
            {
                return UserData.Info(this.user_id);
            }
        }
    }
}
