using System;
using System.Collections.Generic;
using System.Drawing;
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
        public ImageSource TransformImg(string str)
        {
            BitmapImage bitmapImage = null;
            if (str != "")
            {
                bitmapImage = new BitmapImage(new Uri(str));
            }

            return bitmapImage;
        }

        // ImageSource > string
        public String TransformStr(BitmapImage bmp)
        {
            string imagePath = "";
            if (bmp is BitmapImage bitmapImage)
            {
                imagePath = bitmapImage.UriSource?.ToString();
            }
            return imagePath;
        }

        // ImageSource > BitmapImage
        public BitmapImage TransformBmp(ImageSource imageSource)
        {
            if (imageSource is BitmapImage bitmapImage)
            {
                return bitmapImage;
            }

            using (var memoryStream = new MemoryStream())
            {
                var encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create((BitmapSource)imageSource, null, null, null));
                encoder.Save(memoryStream);
                memoryStream.Seek(0, SeekOrigin.Begin);

                // 새로운 BitmapImage 생성
                var result = new BitmapImage();
                result.BeginInit();
                result.CacheOption = BitmapCacheOption.OnLoad; 
                result.StreamSource = memoryStream;
                result.EndInit();
                result.Freeze();

                return result;
            }
        }
    }
}
