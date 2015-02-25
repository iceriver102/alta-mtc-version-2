using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTC_Server.Code
{
    public class User
    {
        public int ID { get; set; }
        public string User_Name { get; set; }
        public string Full_Name { get; set; }
        public bool Status { get; set; }
        public Permision @Permision { get; set; }

    }
}
