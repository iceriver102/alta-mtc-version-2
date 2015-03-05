using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alta.Class;
using System.IO;
using Vlc.DotNet.Core.Medias;

namespace MTC_Server.Code.Media
{
    public partial class MediaData
    {
        public FileInfo LocalFile
        {
            get
            {
                if (this.TypeMedia.Code.ToUpper() == "FILE")
                {
                    return new FileInfo(System.IO.Path.Combine(App.setting.temp_folder, this.Url.toFileName()));
                }
                return null;
            }
        }
        public LocationMedia @LocationMedia
        {
            get
            {
                if (this.TypeMedia.Code.ToUpper() == "URL")
                {
                    return new LocationMedia(this.Url);
                }
                return null;
            }
        }
    }
}
