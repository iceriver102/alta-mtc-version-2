using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alta.Class;
using System.IO;

namespace MTC_Server.Code.Media
{
    public partial class MediaData
    {
        public FileInfo LocalFile
        {
            get
            {
                return new FileInfo(System.IO.Path.Combine(App.setting.temp_folder, this.Url.toFileName()));
            }
        }
    }
}
