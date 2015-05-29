using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Alta.Class
{
    public class MainModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void SetImageData(byte[] data)
        {
#if DEBUG
            if (data == null || data.Length == 0)
                Console.WriteLine("null");
            else Console.WriteLine(data.Length);
#endif
            var source = new BitmapImage();

            source.BeginInit();
            source.CacheOption = BitmapCacheOption.OnLoad;
            source.StreamSource = new MemoryStream(data);
            source.EndInit();
            // use public setter
            ImageSource = source;
        }
        private int flipX = -1;
        private int flipY = -1;
        public int FlipX
        {
            get
            {
                return this.flipX;
            }
            set
            {
                this.flipX = value;
                OnPropertyChanged("FlipX");
            }
        }
        public int FlipY
        {
            get
            {
                return this.flipY;
            }
            set
            {
                this.flipY = value;
                OnPropertyChanged("FlipY");
            }
        }

        private ImageSource imageSource;
        public ImageSource ImageSource
        {
            get { return this.imageSource; }
            set
            {
                this.imageSource = value;
                this.OnPropertyChanged("ImageSource");
            }
        }

        private Stretch imageStretch = Stretch.Uniform;
        public Stretch ImageStretch
        {
            get
            {
                return this.imageStretch;
            }
            set
            {
                this.imageStretch = value;
                this.OnPropertyChanged("ImageStretch");
            }
        }

        protected void OnPropertyChanged(string name)
        {
            var handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}
