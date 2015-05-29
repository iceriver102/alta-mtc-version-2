using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alta.Class;
using System.IO;
using Vlc.DotNet.Core.Medias;
using MTC_Player;

namespace Code.Media
{
    public partial class MediaData
    {
        private MediaBase _media;
        public MediaBase Media
        {
            get
            {
                return this._media;
            }
            set
            {
                this._media = value;
            }
        }
        public void baseMedia()
        {
            if (this.LocalFile!=null)
            {
                this._media = new PathMedia(this.LocalFile.FullName);
            }
            else if (this.TypeMedia.Code=="URL" )
            {
                this._media = new LocationMedia(this.Url);
            }
           
        }
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
