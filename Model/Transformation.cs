using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
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
            ImageSource imageSource = null;
            if (str != "")
            {
                imageSource = new BitmapImage(new Uri(str));
            }

            return imageSource;
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

        // ImageSource > Bitmap
        public Bitmap ImgSourceToBitmap(ImageSource imgSource)
        {
            BitmapSource bitmapSource = (BitmapSource)imgSource;
            Bitmap bitmap;

            using (MemoryStream memoryStream = new MemoryStream())
            {
                BitmapEncoder bitmapEncoder = new BmpBitmapEncoder();
                bitmapEncoder.Frames.Add(BitmapFrame.Create(bitmapSource));
                bitmapEncoder.Save(memoryStream);
                bitmap = new Bitmap(memoryStream);
            }
            return bitmap;
        }

        // Bitmap > ImageSource
        public ImageSource BitmapToImgSource(Bitmap bitmap)
        {
            BitmapSource bitmapSource;

            IntPtr hBitmap = bitmap.GetHbitmap();
            BitmapSizeOptions sizeOptions = BitmapSizeOptions.FromEmptyOptions();
            bitmapSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty, sizeOptions);
            bitmapSource.Freeze();

            return (ImageSource)bitmapSource;
        }

        // ImageSource > BitmapImage
        public BitmapImage ImgSourceToBitmapImg(ImageSource imageSource)
        {
            BitmapSource bitmapSource = imageSource as BitmapSource;

            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = new MemoryStream();

            BitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bitmapSource));
            encoder.Save(bitmapImage.StreamSource);

            bitmapImage.EndInit();
            bitmapImage.Freeze();

            return bitmapImage;
        }
    }
}
