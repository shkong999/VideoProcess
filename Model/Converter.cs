using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
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
    public class Converter
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

        public unsafe byte* ImgSourceToBytePointer(Bitmap bitmap)
        {
            // BitmapData 객체에 비트맵 데이터를 잠금
            BitmapData bitmapData = bitmap.LockBits(
                new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                ImageLockMode.ReadWrite,
                bitmap.PixelFormat);
            byte* p;
            // 픽셀 데이터에 접근하기 위한 포인터 사용
            unsafe
            {
                p = (byte*)bitmapData.Scan0; // 비트맵의 첫 번째 픽셀 주소
            }
            bitmap.UnlockBits(bitmapData);

            return p;
        }

        public unsafe Bitmap BytePointerToBitmap(byte* bytePointer, int width, int height, int bytePerPixel)
        {
            Bitmap newBitmap = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format8bppIndexed);

            if (bytePerPixel == 4)
            {
                newBitmap = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            }
            else if (bytePerPixel == 1)
            {
                newBitmap = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format8bppIndexed);
                ColorPalette palette = newBitmap.Palette;
                for (int i = 0; i < 256; i++)
                {
                    palette.Entries[i] = System.Drawing.Color.FromArgb(i, i, i); // 흑백 팔레트
                }
                newBitmap.Palette = palette;
            }

            BitmapData bmpData = newBitmap.LockBits(new Rectangle(0, 0, newBitmap.Width, newBitmap.Height), ImageLockMode.WriteOnly, newBitmap.PixelFormat);

            IntPtr ptr = bmpData.Scan0;
            byte* bmpPtr = (byte*)ptr.ToPointer();

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    byte* pixel = bytePointer + (y * width + x) * bytePerPixel;

                    if (bytePerPixel == 4)
                    {
                        bmpPtr[(y * width + x) * 4] = pixel[0];  // Blue
                        bmpPtr[(y * width + x) * 4 + 1] = pixel[1];  // Green
                        bmpPtr[(y * width + x) * 4 + 2] = pixel[2];  // Red
                        bmpPtr[(y * width + x) * 4 + 3] = pixel[3];  // Alpha
                    }
                    else if (bytePerPixel == 1) // 8bpp (팔레트 인덱스)
                    {
                        bmpPtr[y * width + x] = *pixel;
                    }
                }
            }
            newBitmap.UnlockBits(bmpData);
            return newBitmap;
        }

        public Bitmap ConvertTo32bppArgb(Bitmap bitmap)
        {
            Bitmap converterBitmap = new Bitmap(bitmap.Width, bitmap.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            // 그래픽 객체 생성
            using (Graphics g = Graphics.FromImage(converterBitmap))
            {
                // 원본 이미지를 새로운 비트맵에 그려넣기
                g.DrawImage(bitmap, 0, 0, bitmap.Width, bitmap.Height);
            }

            return converterBitmap;
        }
    }
}
