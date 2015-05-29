using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Code.Schedule
{
    public partial class Event
    {
        public int Id { get; set; }
        public int parent_id;
        public int device_id;
        public int user_id;
        public int playlist_id;
        public bool status { get; set; }
        public string Content { get; set; }
        public DateTime Begin { get; set; }
        public DateTime End { get; set; }
        public int Col
        {
            get;
            set;
        }
        public double Top { get; set; }
        public double Left { get; set; }
        public double Height { get; set; }
        public double Width { get; set; }
        public bool loop { get; set; }
        public int BeginIndex { get; set; }
        public string NameDevice
        {
            get
            {
                return MysqlHelper.getNameDevice(this.device_id);
            }
        }
        public string IPDevice
        {
            get
            {
                return MysqlHelper.getIPDevice(this.device_id);
            }
        }
        public string NameUser
        {
            get
            {
                return MysqlHelper.getNameUser(this.user_id);
            }
        }
        public string NameParent
        {
            get
            {
                return MysqlHelper.getNameUserBySchedule(this.parent_id);
            }
        }
        public Code.User.UserData User
        {
            get
            {
                if (this.user_id == 0)
                    return null;
                return Code.User.UserData.Info(this.user_id);
            }
        }

        public Code.Device.DeviceData Device
        {
            get
            {
                if (this.device_id == 0)
                    return null;
                return Code.Device.DeviceData.Info(this.device_id);
            }
        }

        public Code.User.UserData Parent
        {
            get
            {
                if (this.parent_id == 0)
                {
                    return null;
                }
                return Code.User.UserData.Info(this.parent_id);
            }
        }

        public Code.Playlist.Playlist Playlist
        {
            get
            {
                if (this.playlist_id == 0)
                {
                    if(this.User !=null)
                    {
                        Code.Playlist.Playlist tmp = this.User.getDefaultPlaylist();
                        if (tmp == null)
                            return tmp;
                        this.playlist_id = tmp.ID;
                        return tmp;
                    }
                    else
                        return null;
                }
                return Code.Playlist.Playlist.Info(this.playlist_id);
            }
        }
    }
}
