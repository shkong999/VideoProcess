using SkiaSharp;
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace VideoProcess.Model
{
    public class Transformation
    {
        // string > ImageSource
        public ImageSource StringToImgSource(string str)
        {
            BitmapImage bitmapImage = null;
            if (str != "")
            {
                bitmapImage = new BitmapImage(new Uri(str));
            }

            return bitmapImage;
        }

        // ImageSource > string
        public String ImgSourceToString(ImageSource img)
        {
            string imagePath = "";
            if (img is BitmapImage bitmapImage)
            {
                imagePath = bitmapImage.UriSource?.ToString();
            }
            return imagePath;
        }

        // String > Bitmap
        public SKBitmap StringToBitmap(String str)
        {
           using(var bitmap = File.OpenRead(str))
           {
               return SKBitmap.Decode(str);
           }
        }

        // ImageSource > Bitmap
        public Bitmap ImgSourceToBitmap(ImageSource imgSource)
        {
            Bitmap bitmap = null;
            using(var stream = new MemoryStream())
            {
                var encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create((BitmapSource)imgSource));
                encoder.Save(stream);
                bitmap = new Bitmap(stream);
            }
            return bitmap;
        }
    }
}
