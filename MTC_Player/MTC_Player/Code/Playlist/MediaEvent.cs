using Code.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Code.Playlist
{
    public partial class MediaEvent
    {
        public int ID { get; set; }
        public int media_id;
        public int playlist_id;
        public TimeSpan TimeBegin { get; set; }
        public TimeSpan TimeEnd { get; set; }
        public DateTime Time;

        public MediaData Media
        {
            get
            {
                if (this.media_id == 0)
                    return null;
                return MediaData.Info(this.media_id);
            }
        }

        public Playlist Playlist
        {
            get
            {
                return Playlist.Info(this.playlist_id);
            }
        }
    }
}
