using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTC_Server.Code.Device
{
    public partial class DeviceData
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Comment { get; set; }
        private int type;
        public int Type
        {
            get
            {
                return this.type;
            }
            set
            {
                this.type = value;
            }
        }
        public DateTime Time { get; set; }

        public bool Status { get; set; }
    }
}
