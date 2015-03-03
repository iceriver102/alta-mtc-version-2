using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MTC_Server.Code.User;

namespace MTC_Server.Code.Media
{
    public partial class MediaData
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Duration { get; set; }
        public string Url { get; set; }
        public string Comment { get; set; }
        private int type;
        public int Type
        {
            get { return this.type; }
            set { this.type = value; }
        }
        public TypeMedia TypeMedia
        {
            get
            {
                if (App.TypeMedias != null)
                {
                    foreach(TypeMedia type in App.TypeMedias)
                    {
                        if(type.Id== this.type)
                        {
                            return type;
                        }
                    }
                }
                return null;
            }
        }
        public string FileSize { get; set; }
        private bool _status = false;
        public bool Status
        {
            get
            {
                return this._status;
            }
            set
            {
                this._status = value;
            }
        }
        public DateTime Time { get; set; }

        private int _user_id;
        public int User_ID
        {
            get
            {
                return this._user_id;
            }
            set
            {
                this._user_id = value;
            }
        }
        public UserData User
        {
            get
            {
                return UserData.Info(this._user_id);
            }
        }
    }
}
