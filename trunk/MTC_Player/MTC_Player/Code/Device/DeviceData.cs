using Code.User;
using MTC_Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Code.Device
{
    public partial class DeviceData
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Comment { get; set; }
        public string IP { get; set; }
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
        private int _details_id = 0;
        public int Detail_ID_Media
        {
            get
            {
                return this._details_id;
            }
            private set
            {
                this._details_id = value;
                this._media_change = true;

            }
        }

        bool _media_change= false;

        public bool MediaChange
        {
            get
            {
                bool tmp = _media_change;
                _media_change = false;
                return tmp;
            }
        }

        public DateTime Time { get; set; }
        public bool Status { get; set; }
        public TypeDevice TypeDevice
        {
            get
            {
                try
                {
                    return App.TypeDevices[this.type];
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }
        
        public Media.MediaData Media
        {
            get
            {
                if (this.ID == 0)
                    return null;
                return getMedia();
            }
        }
        
       private int user_id = 0;
       public UserData User
       {
            get
            {
                if (this.user_id == 0)
                    return null;
                return UserData.Info(this.user_id);
           }
        }

    }
}
