using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace VideoProcess.Model
{
    class Transformation
    {
        // string > ImageSource
        public ImageSource PictureTransformation(string str)
        {
            ImageSource imageSource = null;
            if (str != "")
            {
                imageSource = new BitmapImage(new Uri(str));
            }

            return imageSource;
        }
    }
}
