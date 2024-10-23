using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Color = System.Drawing.Color;
using PixelFormat = System.Drawing.Imaging.PixelFormat;

namespace VideoProcess.Model
{
    public class ImageProcess
    {
        /* 이진화
           이미지를 그레이 스케일로 변환 > 히스토그램 > 임계값 계산 > 그레이 스케일한 이미지로 이진화
         */
        public unsafe void Binization(byte* pBitmap, Bitmap bitmap)
        {
            Color color;

            /* for (int i = 0; i < bitmap.Width; i++)
             {
                 for (int j = 0; j < bitmap.Height; j++)
                 {
                     color = bitmap.GetPixel(i, j);
                 }
             }*/
            Rectangle rectangle = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
            BitmapData bitmapData = bitmap.LockBits(rectangle, ImageLockMode.ReadOnly, bitmap.PixelFormat);
            int stride = bitmapData.Stride;

            for (int i = 0; i < bitmap.Width; i++)
            {
                for (int j = 0; j < bitmap.Height; j++)
                {
                    int pixelIndex = i * stride + j * 4;
                    byte red = pBitmap[pixelIndex + 2];
                    byte green = pBitmap[pixelIndex + 1];
                    byte blue = pBitmap[pixelIndex];

                    byte grayValue = (byte)(0.3 * red + 0.59 * green + 0.11 * blue);
                }
            }
        }
    }
}